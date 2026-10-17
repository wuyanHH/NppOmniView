using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NppPlugin.OmniView.Core
{
    public enum DocFormat
    {
        Unknown = 0,
        Markdown = 1,
        Json = 2,
        Csv = 3,
        Svg = 4,
        Yaml = 5,
        Mermaid = 6,
        Html = 7,
        Code = 8,
        Xml = 9
    }

    public class DetectionResult
    {
        public DocFormat Format { get; set; }
        public string Reason { get; set; }
        public string DetectedLanguage { get; set; }
    }

    public static class MultiFormatDetector
    {
        private static readonly HashSet<string> MdExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".md", ".markdown", ".mdown", ".mkd", ".mdx" };
        private static readonly HashSet<string> JsonExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".json", ".jsonl", ".har" };
        private static readonly HashSet<string> CsvExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".csv", ".tsv" };
        private static readonly HashSet<string> SvgExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".svg" };
        private static readonly HashSet<string> YamlExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".yaml", ".yml" };
        private static readonly HashSet<string> MermaidExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mmd", ".mermaid" };
        private static readonly HashSet<string> HtmlExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".html", ".htm", ".xhtml" };
        private static readonly HashSet<string> XmlExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".xml", ".xsd", ".xsl", ".xslt", ".config", ".manifest", ".resx", ".xaml", ".csproj", ".props", ".targets" };

        private static readonly Dictionary<string, string> CodeExtToLang = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { ".ps1", "powershell" }, { ".psm1", "powershell" }, { ".psd1", "powershell" },
            { ".sh", "bash" }, { ".bash", "bash" }, { ".zsh", "bash" }, { ".cmd", "batch" }, { ".bat", "batch" },
            { ".py", "python" }, { ".pyw", "python" },
            { ".js", "javascript" }, { ".mjs", "javascript" }, { ".cjs", "javascript" }, { ".jsx", "javascript" },
            { ".ts", "typescript" }, { ".tsx", "typescript" },
            { ".cs", "csharp" }, { ".fs", "fsharp" }, { ".vb", "vb" },
            { ".c", "c" }, { ".cpp", "cpp" }, { ".cc", "cpp" }, { ".cxx", "cpp" }, { ".h", "c" }, { ".hpp", "cpp" },
            { ".java", "java" }, { ".kt", "kotlin" }, { ".kts", "kotlin" }, { ".scala", "scala" },
            { ".go", "go" }, { ".rs", "rust" },
            { ".php", "php" }, { ".rb", "ruby" }, { ".pl", "perl" }, { ".lua", "lua" }, { ".swift", "swift" }, { ".dart", "dart" }, { ".r", "r" },
            { ".sql", "sql" },
            { ".css", "css" }, { ".scss", "scss" }, { ".less", "less" },
            { ".ini", "ini" }, { ".cfg", "ini" }, { ".conf", "ini" }, { ".toml", "toml" }
        };

        public static string GetLanguageForExtension(string ext)
        {
            if (!string.IsNullOrEmpty(ext))
            {
                if (XmlExts.Contains(ext)) return "xml";
                if (CodeExtToLang.TryGetValue(ext, out string lang)) return lang;
            }
            return "text";
        }

        private static readonly Regex RxFencedCode = new Regex(@"^(`{3,}|~{3,})\w*\s*[\s\S]*?^\1\s*$", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex RxTable = new Regex(@"^\|.+\|\r?\n\|[\s:\-\|]+\|", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex RxHeading = new Regex(@"^#{1,6}\s+\S+", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex RxLinkOrImage = new Regex(@"!?\[[^\]]+\]\([^)\s]+\)", RegexOptions.Compiled);
        private static readonly Regex RxList = new Regex(@"(^\s*([-*+]|\d+\.)\s+\S+.*(\r?\n|$)){2,}", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex RxJsonKey = new Regex(@"\""[^\""\\]*\""\s*:", RegexOptions.Compiled);
        private static readonly Regex RxYamlKey = new Regex(@"^\s*([a-zA-Z0-9_\-\.\$]+)\s*:\s*(\S.*)?$", RegexOptions.Compiled);
        private static readonly Regex RxMermaidKeyword = new Regex(@"^(flowchart|graph|sequenceDiagram|classDiagram|stateDiagram|erDiagram|gantt|pie|gitGraph|mindmap|timeline|quadrantChart|c4Context)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static DetectionResult Evaluate(DocumentContext doc, Dictionary<IntPtr, DocFormat> forcedOverrides)
        {
            if (forcedOverrides != null && forcedOverrides.TryGetValue(doc.BufferId, out DocFormat forcedFmt))
                return new DetectionResult { Format = forcedFmt, Reason = "手动锁定模式 (" + forcedFmt + ")" };

            if (!string.IsNullOrEmpty(doc.Extension))
            {
                if (MdExts.Contains(doc.Extension)) return new DetectionResult { Format = DocFormat.Markdown, Reason = "标准后缀 (" + doc.Extension + ")" };
                if (JsonExts.Contains(doc.Extension)) return new DetectionResult { Format = DocFormat.Json, Reason = "标准后缀 (" + doc.Extension + ")" };
                if (CsvExts.Contains(doc.Extension)) return new DetectionResult { Format = DocFormat.Csv, Reason = "标准后缀 (" + doc.Extension + ")" };
                if (SvgExts.Contains(doc.Extension)) return new DetectionResult { Format = DocFormat.Svg, Reason = "标准后缀 (" + doc.Extension + ")" };
                if (YamlExts.Contains(doc.Extension)) return new DetectionResult { Format = DocFormat.Yaml, Reason = "标准后缀 (" + doc.Extension + ")" };
                if (MermaidExts.Contains(doc.Extension)) return new DetectionResult { Format = DocFormat.Mermaid, Reason = "标准后缀 (" + doc.Extension + ")" };
                if (HtmlExts.Contains(doc.Extension)) return new DetectionResult { Format = DocFormat.Html, Reason = "标准后缀 (" + doc.Extension + ")" };
                if (XmlExts.Contains(doc.Extension)) return new DetectionResult { Format = DocFormat.Xml, Reason = "标准 XML 文档 (" + doc.Extension + ")" };
                if (CodeExtToLang.TryGetValue(doc.Extension, out string codeLang))
                    return new DetectionResult { Format = DocFormat.Code, DetectedLanguage = codeLang, Reason = "源码文档 (" + codeLang.ToUpperInvariant() + ", " + doc.Extension + ")" };
            }

            if (string.IsNullOrWhiteSpace(doc.Content))
                return new DetectionResult { Format = DocFormat.Unknown, Reason = "当前缓冲区内容为空" };

            string content = doc.Content;
            int len = content.Length;
            string sample = len > 4000 ? content.Substring(0, 4000) : content;
            string sampleTrim = sample.TrimStart();

            // 1. Check for SVG (支持任意大小、带 XML Prolog、注释、DOCTYPE 以及未闭合手写片段)
            if (IsLikelySvg(content, sample, sampleTrim))
            {
                return new DetectionResult { Format = DocFormat.Svg, Reason = "自动识别为 SVG 矢量图形" };
            }

            // 2. Check for HTML
            if (sampleTrim.StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase) ||
                sampleTrim.StartsWith("<html", StringComparison.OrdinalIgnoreCase) ||
                (sample.IndexOf("<body", StringComparison.OrdinalIgnoreCase) >= 0 && content.LastIndexOf("</html>", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return new DetectionResult { Format = DocFormat.Html, Reason = "自动识别为 HTML 网页文档" };
            }

            // 2.5 Check for XML (未保存的独立 XML 文档，支持带声明、常见典型根节点或命名空间)
            if (IsLikelyXml(content, sample, sampleTrim))
            {
                return new DetectionResult { Format = DocFormat.Xml, Reason = "自动识别为 XML 结构文档" };
            }

            // 3. Check for Mermaid diagram
            if (RxMermaidKeyword.IsMatch(sampleTrim))
            {
                return new DetectionResult { Format = DocFormat.Mermaid, Reason = "自动识别为 Mermaid 架构图" };
            }

            // Zero-allocation inspection of first and last non-whitespace characters for JSON
            int firstIdx = 0;
            while (firstIdx < len && char.IsWhiteSpace(content[firstIdx])) firstIdx++;
            int lastIdx = len - 1;
            while (lastIdx >= firstIdx && char.IsWhiteSpace(content[lastIdx])) lastIdx--;

            if (firstIdx <= lastIdx)
            {
                char firstChar = content[firstIdx];
                char lastChar = content[lastIdx];

                if ((firstChar == '{' && lastChar == '}') || (firstChar == '[' && lastChar == ']'))
                {
                    // Empty object or array
                    if ((firstChar == '{' && lastIdx == firstIdx + 1) || (firstChar == '[' && lastIdx == firstIdx + 1))
                        return new DetectionResult { Format = DocFormat.Json, Reason = "自动识别为空 JSON 结构" };

                    // Inspect sample tokens for JSON characteristics
                    if (RxJsonKey.IsMatch(sample) || (firstChar == '[' && (sample.Contains(",") || sample.Contains("\""))))
                        return new DetectionResult { Format = DocFormat.Json, Reason = "自动识别为 JSON 结构" };
                }
            }

            if (IsLikelyCsv(sample))
            {
                return new DetectionResult { Format = DocFormat.Csv, Reason = "自动识别为 CSV / TSV 表格数据" };
            }

            // 4. Check for code syntax characteristics before Markdown (avoids treating scripts with # comments as MD)
            if (IsLikelyCodeContent(sample, out string detectedCodeLang))
            {
                return new DetectionResult { Format = DocFormat.Code, DetectedLanguage = detectedCodeLang, Reason = "代码特征识别 (" + detectedCodeLang.ToUpperInvariant() + ")" };
            }

            int mdScore = 0;
            if (RxFencedCode.IsMatch(sample)) mdScore += 4;
            if (RxTable.IsMatch(sample)) mdScore += 4;
            if (RxLinkOrImage.IsMatch(sample)) mdScore += 3;
            if (RxList.IsMatch(sample)) mdScore += 2;
            if (RxHeading.IsMatch(sample)) mdScore += 2;
            if (sample.Contains("**") || sample.Contains("`")) mdScore += 1;

            if (mdScore >= 4)
                return new DetectionResult { Format = DocFormat.Markdown, Reason = "自动识别为 Markdown (特征分 " + mdScore + ")" };

            if (IsLikelyYaml(sample))
            {
                return new DetectionResult { Format = DocFormat.Yaml, Reason = "自动识别为 YAML 配置结构" };
            }

            return new DetectionResult { Format = DocFormat.Unknown, Reason = "未检测到已知的文档结构特征" };
        }

        private static bool IsLikelyCodeContent(string sample, out string detectedLang)
        {
            detectedLang = "powershell";
            if (sample.Contains("$") && (sample.Contains("::") || sample.Contains("-eq") || sample.Contains("param(") || sample.Contains("Write-Host") || sample.Contains("Assert-Equal") || sample.Contains("[ref]")))
            {
                detectedLang = "powershell";
                return true;
            }
            if (Regex.IsMatch(sample, @"^(using\s+System|namespace\s+\w+|public\s+class\s+\w+)", RegexOptions.Multiline))
            {
                detectedLang = "csharp";
                return true;
            }
            if (Regex.IsMatch(sample, @"^(import\s+\w+|from\s+\w+\s+import|def\s+\w+\s*\(|class\s+\w+\s*:)", RegexOptions.Multiline))
            {
                detectedLang = "python";
                return true;
            }
            if (Regex.IsMatch(sample, @"^(import\s+.*?from\s+['""]|const\s+\w+\s*=|function\s+\w+\s*\(|export\s+(default|const|function))", RegexOptions.Multiline))
            {
                detectedLang = "javascript";
                return true;
            }
            return false;
        }

        private static bool IsLikelyYaml(string sample)
        {
            string[] lines = sample.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) return false;

            if (lines[0].Trim() == "---") return true;

            int keyCount = 0;
            int checkLines = Math.Min(lines.Length, 12);
            for (int i = 0; i < checkLines; i++)
            {
                string trimmed = lines[i].TrimStart();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#")) continue;
                if (trimmed.StartsWith("<") || trimmed.StartsWith("|") || trimmed.StartsWith("{")) return false;
                if (RxYamlKey.IsMatch(lines[i])) keyCount++;
            }
            return keyCount >= 2;
        }

        private static bool IsLikelyCsv(string sample)
        {
            if (string.IsNullOrWhiteSpace(sample)) return false;

            char[] candidateDelims = { ',', '\t', ';', '|' };

            foreach (char delim in candidateDelims)
            {
                // 获取按 RFC 4180 引号状态机切分出的记录列数（支持字段内部换行与字段包含分隔符）
                var colCounts = GetCsvRecordColumnCounts(sample, delim, 10);
                if (colCounts.Count < 2) continue;

                int firstCount = colCounts[0];
                if (firstCount < 2) continue;

                // 排除 Markdown 表格 (以 | 起始且包含对齐虚线)
                if (delim == '|' && (sample.Contains("---") || sample.TrimStart().StartsWith("|")))
                {
                    if (Regex.IsMatch(sample, @"\|[\s:\-\|]+\|")) continue;
                }

                // 检查前多行记录的列数一致性
                bool consistent = true;
                for (int i = 1; i < colCounts.Count; i++)
                {
                    if (colCounts[i] != firstCount)
                    {
                        consistent = false;
                        break;
                    }
                }

                if (consistent)
                {
                    string firstNonEmpty = sample.TrimStart();
                    if (firstNonEmpty.StartsWith("<") || firstNonEmpty.StartsWith("{")) continue;
                    return true;
                }
            }
            return false;
        }

        public static List<int> GetCsvRecordColumnCounts(string text, char delim, int maxRecords = 12)
        {
            var results = new List<int>();
            if (string.IsNullOrEmpty(text)) return results;

            bool inQuotes = false;
            int curColCount = 1;
            int len = text.Length;

            for (int i = 0; i < len; i++)
            {
                char c = text[i];
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < len && text[i + 1] == '"')
                        {
                            i++; // RFC 4180 转义双引号 ""，跳过下一个
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    // 在引号内部，换行符与与分隔符一致的字符均不结束列或行
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == delim)
                    {
                        curColCount++;
                    }
                    else if (c == '\r' || c == '\n')
                    {
                        if (c == '\r' && i + 1 < len && text[i + 1] == '\n') i++;
                        results.Add(curColCount);
                        curColCount = 1;
                        if (results.Count >= maxRecords) break;
                    }
                }
            }

            if (results.Count < maxRecords && (curColCount > 1 || results.Count > 0))
            {
                results.Add(curColCount);
            }

            return results;
        }

        private static bool IsLikelyXml(string content, string sample, string sampleTrim)
        {
            if (string.IsNullOrEmpty(sampleTrim)) return false;

            // 1. 标准 XML 声明头
            if (sampleTrim.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
                return true;

            // 2. 剥离 XML prolog 与注释
            string rootCandidate = StripXmlPrologAndComments(sampleTrim);
            if (string.IsNullOrEmpty(rootCandidate) || !rootCandidate.StartsWith("<"))
                return false;

            // 排除 HTML 与 SVG (它们由专门的分支优先处理)
            if (rootCandidate.StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase) ||
                rootCandidate.StartsWith("<html", StringComparison.OrdinalIgnoreCase) ||
                rootCandidate.StartsWith("<svg", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // 3. 常见典型 XML 根节点
            string[] typicalRoots = {
                "<root", "<project", "<configuration", "<beans", "<feed", "<rss",
                "<manifest", "<plist", "<urlset", "<dataset", "<dependencies",
                "<assembly", "<package", "<ruleset", "<schema", "<wsdl", "<definitions",
                "<Envelope", "<soap:Envelope", "<component", "<application", "<metadata",
                "<model", "<settings", "<layout", "<resources", "<values", "<vector"
            };
            foreach (var r in typicalRoots)
            {
                if (rootCandidate.StartsWith(r, StringComparison.OrdinalIgnoreCase))
                {
                    int rLen = r.Length;
                    if (rootCandidate.Length == rLen || char.IsWhiteSpace(rootCandidate[rLen]) || rootCandidate[rLen] == '>')
                        return true;
                }
            }

            // 4. 命名空间特征与通用闭合标签特征
            if (rootCandidate.IndexOf("xmlns:", StringComparison.OrdinalIgnoreCase) >= 0 ||
                rootCandidate.IndexOf("xmlns=", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            // 5. 提取第一个 XML 根标签名，判断文中是否有对应的闭合标签 </tag>
            var match = Regex.Match(rootCandidate, @"^<([a-zA-Z0-9_\-\:]+)[\s\/>]");
            if (match.Success)
            {
                string tagName = match.Groups[1].Value;
                if (!string.Equals(tagName, "h1", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(tagName, "p", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(tagName, "div", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(tagName, "span", StringComparison.OrdinalIgnoreCase))
                {
                    string closeTag = "</" + tagName + ">";
                    if (content.IndexOf(closeTag, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;

                    // 自闭合根标签 如 <configuration ... />
                    int firstGt = rootCandidate.IndexOf('>');
                    if (firstGt > 0 && rootCandidate[firstGt - 1] == '/')
                        return true;
                }
            }

            return false;
        }

        private static bool IsLikelySvg(string content, string sample, string sampleTrim)
        {
            if (string.IsNullOrEmpty(sampleTrim)) return false;

            // 剥离 XML prolog (<?xml ... ?>), 注释 (<!-- ... -->) 和 DOCTYPE (<!DOCTYPE ... >)
            string rootCandidate = StripXmlPrologAndComments(sampleTrim);
            if (rootCandidate.StartsWith("<svg", StringComparison.OrdinalIgnoreCase))
            {
                if (rootCandidate.Length == 4 || char.IsWhiteSpace(rootCandidate[4]) || rootCandidate[4] == '>')
                {
                    // 只要根标签是 <svg，且全文存在 </svg>、或包含常见 SVG 属性/命名空间，或为正在输入的简短内容
                    if (content.LastIndexOf("</svg>", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        rootCandidate.IndexOf("xmlns", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        rootCandidate.IndexOf("viewBox", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        rootCandidate.IndexOf("<path", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        content.Length < 1000)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string StripXmlPrologAndComments(string text)
        {
            int idx = 0;
            int maxLen = text.Length;
            while (idx < maxLen)
            {
                while (idx < maxLen && char.IsWhiteSpace(text[idx])) idx++;
                if (idx >= maxLen) break;

                // 跳过 <?xml ... ?>
                if (idx + 5 <= maxLen && string.Compare(text, idx, "<?xml", 0, 5, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    int end = text.IndexOf("?>", idx, StringComparison.Ordinal);
                    if (end < 0) break;
                    idx = end + 2;
                    continue;
                }
                // 跳过 <!-- ... -->
                if (idx + 4 <= maxLen && string.Compare(text, idx, "<!--", 0, 4, StringComparison.Ordinal) == 0)
                {
                    int end = text.IndexOf("-->", idx, StringComparison.Ordinal);
                    if (end < 0) break;
                    idx = end + 3;
                    continue;
                }
                // 跳过 <!DOCTYPE ... >
                if (idx + 9 <= maxLen && string.Compare(text, idx, "<!DOCTYPE", 0, 9, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    int end = text.IndexOf(">", idx, StringComparison.Ordinal);
                    if (end < 0) break;
                    idx = end + 1;
                    continue;
                }
                break;
            }
            return idx > 0 ? text.Substring(idx).TrimStart() : text;
        }
    }
}
