using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace NppPlugin.OmniView.Core
{
    public static class FormatProcessor
    {
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };

        public static string FormatSvg(string input, out string errorMsg)
        {
            errorMsg = null;
            if (string.IsNullOrWhiteSpace(input)) return input ?? string.Empty;
            try
            {
                var doc = XDocument.Parse(input);
                var sb = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    OmitXmlDeclaration = !input.TrimStart().StartsWith("<?xml", StringComparison.OrdinalIgnoreCase),
                    NewLineChars = "\r\n"
                };
                using (var writer = XmlWriter.Create(sb, settings))
                {
                    doc.Save(writer);
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return input;
            }
        }

        public static string FormatXml(string input, out string errorMsg)
        {
            return FormatSvg(input, out errorMsg);
        }

        public static string BeautifyJson(string input, out string errorMsg)
        {
            errorMsg = null;
            if (string.IsNullOrWhiteSpace(input)) return input ?? string.Empty;
            try
            {
                Serializer.DeserializeObject(input);
                return PrettyPrintJson(input, true);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return input;
            }
        }

        public static string MinifyJson(string input, out string errorMsg)
        {
            errorMsg = null;
            if (string.IsNullOrWhiteSpace(input)) return input ?? string.Empty;
            try
            {
                Serializer.DeserializeObject(input);
                return PrettyPrintJson(input, false);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return input;
            }
        }

        private static string PrettyPrintJson(string json, bool indent)
        {
            if (string.IsNullOrWhiteSpace(json)) return string.Empty;
            var sb = new StringBuilder();
            bool inQuotes = false;
            bool escaped = false;
            int depth = 0;

            for (int i = 0; i < json.Length; i++)
            {
                char ch = json[i];
                if (escaped) { sb.Append(ch); escaped = false; continue; }
                if (ch == '\\' && inQuotes) { sb.Append(ch); escaped = true; continue; }
                if (ch == '"') { inQuotes = !inQuotes; sb.Append(ch); continue; }
                if (inQuotes) { sb.Append(ch); continue; }
                if (char.IsWhiteSpace(ch)) continue;

                if (!indent) { sb.Append(ch); continue; }

                // Check for empty {} or []
                if (ch == '{' || ch == '[')
                {
                    char closeChar = ch == '{' ? '}' : ']';
                    int peek = i + 1;
                    while (peek < json.Length && char.IsWhiteSpace(json[peek])) peek++;
                    if (peek < json.Length && json[peek] == closeChar)
                    {
                        sb.Append(ch).Append(closeChar);
                        i = peek;
                        continue;
                    }

                    sb.Append(ch);
                    depth++;
                    sb.AppendLine();
                    sb.Append(new string(' ', depth * 2));
                    continue;
                }

                switch (ch)
                {
                    case '}':
                    case ']':
                        depth = Math.Max(0, depth - 1);
                        sb.AppendLine();
                        sb.Append(new string(' ', depth * 2));
                        sb.Append(ch);
                        break;
                    case ',':
                        sb.Append(ch);
                        sb.AppendLine();
                        sb.Append(new string(' ', depth * 2));
                        break;
                    case ':':
                        sb.Append(indent ? ": " : ":");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        public static List<List<string>> ParseCsv(string text, char? delimiter = null)
        {
            var rows = new List<List<string>>();
            if (string.IsNullOrEmpty(text)) return rows;

            char delim = delimiter ?? DetectCsvDelimiter(text);
            var curRow = new List<string>();
            var curCell = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < text.Length && text[i + 1] == '"') { curCell.Append('"'); i++; }
                        else inQuotes = false;
                    }
                    else curCell.Append(c);
                }
                else
                {
                    if (c == '"') inQuotes = true;
                    else if (c == delim) { curRow.Add(curCell.ToString()); curCell.Clear(); }
                    else if (c == '\r' || c == '\n')
                    {
                        if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                        curRow.Add(curCell.ToString());
                        curCell.Clear();
                        if (curRow.Count > 1 || (curRow.Count == 1 && curRow[0].Length > 0))
                            rows.Add(curRow);
                        curRow = new List<string>();
                    }
                    else curCell.Append(c);
                }
            }
            if (curCell.Length > 0 || curRow.Count > 0)
            {
                curRow.Add(curCell.ToString());
                rows.Add(curRow);
            }
            return rows;
        }

        public static char DetectCsvDelimiter(string text)
        {
            if (string.IsNullOrEmpty(text)) return ',';
            char[] candidates = { ',', '\t', ';', '|', '^', '~', '#', ':', ' ' };
            char bestDelim = ',';
            int bestScore = 0;

            foreach (char cand in candidates)
            {
                var colCounts = MultiFormatDetector.GetCsvRecordColumnCounts(text, cand, 10);
                if (colCounts.Count == 0) continue;

                int count0 = colCounts[0] - 1; // 单行分隔符出现次数
                if (count0 <= 0) continue;

                bool consistent = true;
                int total = count0;
                for (int i = 1; i < colCounts.Count; i++)
                {
                    int countI = colCounts[i] - 1;
                    if (countI != count0)
                    {
                        consistent = false;
                        break;
                    }
                    total += countI;
                }

                int score = consistent ? (total * 10) : total;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestDelim = cand;
                }
            }

            return bestDelim;
        }

        private static int CountUnquotedChar(string line, char target)
        {
            bool inQuotes = false;
            int count = 0;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"') inQuotes = !inQuotes;
                else if (c == target && !inQuotes) count++;
            }
            return count;
        }

        public static string ConvertCsvDelimiter(string csvText, char newDelimiter, char? currentDelimiter = null)
        {
            if (string.IsNullOrEmpty(csvText)) return csvText ?? string.Empty;
            char oldDelim = currentDelimiter ?? DetectCsvDelimiter(csvText);
            if (oldDelim == newDelimiter) return csvText;

            var rows = ParseCsv(csvText, oldDelim);
            if (rows.Count == 0) return csvText;

            var sb = new StringBuilder();
            for (int r = 0; r < rows.Count; r++)
            {
                var row = rows[r];
                for (int c = 0; c < row.Count; c++)
                {
                    string cell = EscapeCsvCell(row[c], newDelimiter);
                    sb.Append(cell);
                    if (c < row.Count - 1)
                        sb.Append(newDelimiter);
                }
                if (r < rows.Count - 1)
                    sb.AppendLine();
            }
            return sb.ToString();
        }

        public static string AlignCsvColumns(string csvText, char? delimiter = null)
        {
            char delim = delimiter ?? DetectCsvDelimiter(csvText);
            var rows = ParseCsv(csvText, delim);
            if (rows.Count == 0) return csvText;

            // Normalize each cell by trimming extraneous outer whitespace from previous alignments (idempotent fix)
            var normRows = new List<List<string>>();
            foreach (var r in rows)
            {
                var normRow = new List<string>();
                foreach (var c in r) normRow.Add(c != null ? c.Trim() : "");
                normRows.Add(normRow);
            }

            int maxCols = normRows.Max(r => r.Count);
            int[] widths = new int[maxCols];
            foreach (var r in normRows)
            {
                for (int c = 0; c < r.Count; c++)
                    widths[c] = Math.Max(widths[c], EscapeCsvCell(r[c], delim).Length);
            }

            var sb = new StringBuilder();
            string sep = delim == '\t' ? "\t" : (delim == ',' ? ", " : delim.ToString() + " ");
            foreach (var r in normRows)
            {
                for (int c = 0; c < r.Count; c++)
                {
                    string cell = EscapeCsvCell(r[c], delim);
                    if (c < r.Count - 1)
                    {
                        if (delim == '\t') sb.Append(cell).Append('\t');
                        else sb.Append(cell.PadRight(widths[c])).Append(sep);
                    }
                    else sb.Append(cell);
                }
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        public static string MinifyCsv(string csvText, char? delimiter = null)
        {
            char delim = delimiter ?? DetectCsvDelimiter(csvText);
            var rows = ParseCsv(csvText, delim);
            if (rows.Count == 0) return csvText;
            var sb = new StringBuilder();

            foreach (var r in rows)
            {
                var cells = new List<string>();
                foreach (var c in r)
                {
                    cells.Add(EscapeCsvCell(c != null ? c.Trim() : "", delim));
                }
                sb.AppendLine(string.Join(delim.ToString(), cells));
            }
            return sb.ToString().TrimEnd();
        }

        public static string ConvertCsvToJson(string csvText)
        {
            var rows = ParseCsv(csvText);
            if (rows.Count < 2) return "[]";
            var headers = rows[0];
            var list = new List<Dictionary<string, object>>();

            for (int r = 1; r < rows.Count; r++)
            {
                var row = rows[r];
                var dict = new Dictionary<string, object>();
                for (int c = 0; c < headers.Count; c++)
                {
                    string key = string.IsNullOrWhiteSpace(headers[c]) ? "col_" + c : headers[c].Trim();
                    string rawVal = c < row.Count ? row[c].Trim() : "";

                    if (bool.TryParse(rawVal, out bool bVal))
                    {
                        dict[key] = bVal;
                    }
                    else if (rawVal.Length > 1 && rawVal.StartsWith("0") && !rawVal.StartsWith("0."))
                    {
                        // Preserve leading zeros for strings like "0123", zip codes, phone numbers
                        dict[key] = rawVal;
                    }
                    else if (decimal.TryParse(rawVal, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal dVal))
                    {
                        dict[key] = dVal;
                    }
                    else
                    {
                        dict[key] = rawVal;
                    }
                }
                list.Add(dict);
            }
            string rawJson = Serializer.Serialize(list);
            return PrettyPrintJson(rawJson, true);
        }

        public static string ConvertCsvToMarkdownTable(string csvText)
        {
            var rows = ParseCsv(csvText);
            if (rows.Count == 0) return csvText;
            int cols = rows.Max(r => r.Count);
            var sb = new StringBuilder();

            sb.Append("|");
            for (int c = 0; c < cols; c++)
            {
                string cell = c < rows[0].Count ? SanitizeForMarkdownCell(rows[0][c]) : "";
                sb.Append(" ").Append(cell).Append(" |");
            }
            sb.AppendLine();

            sb.Append("|");
            for (int c = 0; c < cols; c++) sb.Append(" --- |");
            sb.AppendLine();

            for (int r = 1; r < rows.Count; r++)
            {
                sb.Append("|");
                for (int c = 0; c < cols; c++)
                {
                    string cell = c < rows[r].Count ? SanitizeForMarkdownCell(rows[r][c]) : "";
                    sb.Append(" ").Append(cell).Append(" |");
                }
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        private static string SanitizeForMarkdownCell(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Replace("\r\n", "<br>")
                       .Replace("\n", "<br>")
                       .Replace("\r", "<br>")
                       .Replace("|", "\\|")
                       .Trim();
        }

        public static string ConvertJsonToCsv(string jsonText, out string errorMsg)
        {
            errorMsg = null;
            try
            {
                object obj = Serializer.DeserializeObject(jsonText);
                object[] items = obj as object[];
                if (items == null)
                {
                    if (obj is Dictionary<string, object> singleDict) items = new object[] { singleDict };
                    else { errorMsg = "JSON root must be an array of objects or a single object."; return jsonText; }
                }

                var headers = new List<string>();
                foreach (var item in items)
                {
                    if (item is Dictionary<string, object> d)
                    {
                        foreach (var k in d.Keys)
                            if (!headers.Contains(k)) headers.Add(k);
                    }
                }
                if (headers.Count == 0) return "";

                var sb = new StringBuilder();
                sb.AppendLine(string.Join(",", headers.Select(h => EscapeCsvCell(h, ','))));

                foreach (var item in items)
                {
                    if (item is Dictionary<string, object> d)
                    {
                        var cells = new List<string>();
                        foreach (var h in headers)
                        {
                            string cellStr = "";
                            if (d.TryGetValue(h, out object v) && v != null)
                            {
                                if (v is IDictionary || (v is IEnumerable && !(v is string)))
                                {
                                    // Re-serialize nested objects and arrays instead of printing C# class name
                                    cellStr = Serializer.Serialize(v);
                                }
                                else
                                {
                                    cellStr = Convert.ToString(v, CultureInfo.InvariantCulture);
                                }
                            }
                            cells.Add(EscapeCsvCell(cellStr, ','));
                        }
                        sb.AppendLine(string.Join(",", cells));
                    }
                }
                return sb.ToString().TrimEnd();
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return jsonText;
            }
        }

        public static string ConvertJsonToMarkdownTable(string jsonText, out string errorMsg)
        {
            string csv = ConvertJsonToCsv(jsonText, out errorMsg);
            if (errorMsg != null) return jsonText;
            return ConvertCsvToMarkdownTable(csv);
        }

        private static string EscapeCsvCell(string val, char delim = ',')
        {
            if (val == null) return "";
            string delimStr = delim.ToString();
            if (val.Contains(delimStr) || val.Contains("\"") || val.Contains("\n") || val.Contains("\r"))
                return "\"" + val.Replace("\"", "\"\"") + "\"";
            return val;
        }
    }
}
