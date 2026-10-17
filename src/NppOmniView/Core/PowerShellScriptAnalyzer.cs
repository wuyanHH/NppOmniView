using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NppPlugin.OmniView.Core
{
    /// <summary>
    /// PowerShell 参数信息模型
    /// </summary>
    public class PsParameterInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "object";
        public string DefaultValue { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> ValidateSet { get; set; } = new List<string>();
    }

    /// <summary>
    /// PowerShell 函数符号模型
    /// </summary>
    public class PsFunctionInfo
    {
        public string Name { get; set; } = string.Empty;
        public int LineNumber { get; set; } = 1;
        public string Synopsis { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<PsParameterInfo> Parameters { get; set; } = new List<PsParameterInfo>();
        public List<string> Examples { get; set; } = new List<string>();
        public string RawSignature { get; set; } = string.Empty;
    }

    /// <summary>
    /// PowerShell 脚本安全与环境检测模型
    /// </summary>
    public class PsSecurityNotice
    {
        public string Level { get; set; } = "info"; // info | warning | danger
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public int LineNumber { get; set; }
    }

    /// <summary>
    /// PowerShell 脚本全景分析结果模型
    /// </summary>
    public class PsScriptAnalysisResult
    {
        public string ScriptName { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Requires { get; set; } = new List<string>();
        public List<PsParameterInfo> TopLevelParameters { get; set; } = new List<PsParameterInfo>();
        public List<PsFunctionInfo> Functions { get; set; } = new List<PsFunctionInfo>();
        public List<string> Examples { get; set; } = new List<string>();
        public List<PsSecurityNotice> SecurityNotices { get; set; } = new List<PsSecurityNotice>();
        public int TotalLines { get; set; }
        public int CodeLines { get; set; }
        public int CommentLines { get; set; }
        public string SyntaxSynopsis { get; set; } = string.Empty;
    }

    /// <summary>
    /// PowerShell 脚本结构解析引擎与文档分析器
    /// 支持提取基于注释的帮助文档 (Comment-Based Help)、顶层输入参数、函数大纲树与关键环境/安全审计信息
    /// </summary>
    public static class PowerShellScriptAnalyzer
    {
        // 匹配 #Requires 指令
        private static readonly Regex RxRequires = new Regex(@"^\s*#Requires\s+(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // 匹配基于注释的帮助块: <# ... #>
        private static readonly Regex RxCommentHelpBlock = new Regex(@"<#([\s\S]*?)#>", RegexOptions.Compiled);

        // 匹配帮助块内的各节指令: .SYNOPSIS, .DESCRIPTION, .PARAMETER name, .EXAMPLE, .OUTPUTS, .NOTES
        private static readonly Regex RxHelpSections = new Regex(@"^\s*\.([a-zA-Z]+)(?:\s+([a-zA-Z0-9_\-]+))?\s*$", RegexOptions.Multiline | RegexOptions.Compiled);

        // 匹配函数定义: function Verb-Noun { ... } 或 filter Verb-Noun { ... }
        private static readonly Regex RxFunction = new Regex(@"(?:^|\n)\s*(?:function|filter)\s+([a-zA-Z0-9_\-\:]+)\s*(?:\(([^)]*)\))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // 匹配顶层或函数内 param(...) 块
        private static readonly Regex RxParamBlock = new Regex(@"(?:^|\s)param\s*\(([\s\S]*?)\)(?:\s*(?:\r?\n|;|\{))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// 分析 PowerShell 脚本源代码并生成结构化分析模型
        /// </summary>
        public static PsScriptAnalysisResult Analyze(string scriptContent, string scriptFileName = "script.ps1")
        {
            var res = new PsScriptAnalysisResult
            {
                ScriptName = string.IsNullOrWhiteSpace(scriptFileName) ? "script.ps1" : Path.GetFileName(scriptFileName)
            };

            if (string.IsNullOrEmpty(scriptContent))
                return res;

            string[] lines = scriptContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            res.TotalLines = lines.Length;

            // 1. 统计代码行与注释行，同时检测 #Requires 与安全敏感命令
            bool inBlockComment = false;
            for (int i = 0; i < lines.Length; i++)
            {
                int lineNo = i + 1;
                string line = lines[i];
                string trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                if (trimmed.StartsWith("<#")) inBlockComment = true;

                if (inBlockComment)
                {
                    res.CommentLines++;
                    if (trimmed.EndsWith("#>") || trimmed.Contains("#>")) inBlockComment = false;
                    continue;
                }

                if (trimmed.StartsWith("#"))
                {
                    res.CommentLines++;
                    // 检查 #Requires
                    var reqMatch = RxRequires.Match(line);
                    if (reqMatch.Success)
                    {
                        string reqVal = reqMatch.Groups[1].Value.Trim();
                        if (!res.Requires.Contains(reqVal))
                            res.Requires.Add(reqVal);
                    }
                    continue;
                }

                res.CodeLines++;

                // 安全与破坏性指令扫描
                ScanSecurityCommand(line, lineNo, res.SecurityNotices);
            }

            // 2. 提取脚本顶层的帮助注释或头部说明
            ExtractTopLevelHelp(scriptContent, lines, res);

            // 3. 提取顶层 param(...) 参数定义
            ExtractTopLevelParameters(scriptContent, res);

            // 4. 提取函数与子命令列表
            ExtractFunctions(scriptContent, lines, res);

            // 5. 生成语法调用简要签名 (Syntax Synopsis)
            BuildSyntaxSynopsis(res);

            return res;
        }

        /// <summary>
        /// 扫描单行中的潜在危险或破坏性指令
        /// </summary>
        private static void ScanSecurityCommand(string line, int lineNo, List<PsSecurityNotice> notices)
        {
            if (Regex.IsMatch(line, @"\bRemove-Item\b.*-(?:Recurse|Force)\b", RegexOptions.IgnoreCase))
            {
                notices.Add(new PsSecurityNotice
                {
                    Level = "warning",
                    Title = "递归级联删除",
                    Detail = "包含 Remove-Item 并带有 -Recurse / -Force 参数",
                    LineNumber = lineNo
                });
            }
            else if (Regex.IsMatch(line, @"\b(?:Format-Volume|Clear-Disk|Stop-Computer|Restart-Computer)\b", RegexOptions.IgnoreCase))
            {
                notices.Add(new PsSecurityNotice
                {
                    Level = "danger",
                    Title = "系统高危指令",
                    Detail = "包含可能影响系统磁盘或计算机电源状态的高危 Cmdlet",
                    LineNumber = lineNo
                });
            }
            else if (Regex.IsMatch(line, @"\b(?:Invoke-Expression|iex)\b", RegexOptions.IgnoreCase))
            {
                notices.Add(new PsSecurityNotice
                {
                    Level = "warning",
                    Title = "动态执行脚本",
                    Detail = "使用了 Invoke-Expression (iex) 动态求值，请确保输入来源受信任",
                    LineNumber = lineNo
                });
            }
            else if (Regex.IsMatch(line, @"\bSet-ExecutionPolicy\b", RegexOptions.IgnoreCase))
            {
                notices.Add(new PsSecurityNotice
                {
                    Level = "info",
                    Title = "修改执行策略",
                    Detail = "脚本正在配置 PowerShell 执行策略 (Set-ExecutionPolicy)",
                    LineNumber = lineNo
                });
            }
            else if (Regex.IsMatch(line, @"\bConvertTo-SecureString\b.*-AsPlainText\b", RegexOptions.IgnoreCase))
            {
                notices.Add(new PsSecurityNotice
                {
                    Level = "warning",
                    Title = "明文凭据转换",
                    Detail = "检测到明文转为安全字符串操作，需警惕明文密码硬编码",
                    LineNumber = lineNo
                });
            }
        }

        /// <summary>
        /// 提取顶层的帮助文档注释 (Comment-Based Help 或连续 # 注释)
        /// </summary>
        private static void ExtractTopLevelHelp(string content, string[] lines, PsScriptAnalysisResult res)
        {
            // 优先检查是否有标准的 <# ... #> 帮助块
            Match match = RxCommentHelpBlock.Match(content);
            if (match.Success && match.Index < 3000) // 必须位于脚本前半部分
            {
                ParseHelpBlock(match.Groups[1].Value, out string syn, out string desc, out var paramDocs, out var exList);
                res.Synopsis = syn;
                res.Description = desc;
                res.Examples.AddRange(exList);
                return;
            }

            // 若无 <# ... #>，尝试收集脚本前部的连续单行 # 注释作为说明
            var sbHeader = new StringBuilder();
            int collected = 0;
            for (int i = 0; i < Math.Min(lines.Length, 40); i++)
            {
                string tr = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(tr)) continue;
                if (tr.StartsWith("#"))
                {
                    string comment = tr.TrimStart('#', ' ', '\t');
                    // 过滤装饰线
                    if (comment.StartsWith("===") || comment.StartsWith("---") || comment.StartsWith("***"))
                        continue;
                    if (comment.StartsWith("Requires", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (sbHeader.Length > 0) sbHeader.AppendLine();
                    sbHeader.Append(comment);
                    collected++;
                }
                else
                {
                    // 遇到第一行非注释代码即停止收集头部说明
                    break;
                }
            }

            if (collected > 0)
            {
                string headerText = sbHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(headerText))
                {
                    // 取第一句作为 Synopsis，其余作为 Description
                    int firstDot = headerText.IndexOfAny(new[] { '。', '.', '\n' });
                    if (firstDot > 0 && firstDot < 120)
                    {
                        res.Synopsis = headerText.Substring(0, firstDot + 1).Trim();
                        res.Description = headerText.Substring(firstDot + 1).Trim();
                    }
                    else
                    {
                        res.Synopsis = headerText;
                    }
                }
            }
        }

        /// <summary>
        /// 解析 <# ... #> 注释块中的各个指令节
        /// </summary>
        private static void ParseHelpBlock(string blockContent, out string synopsis, out string description, out Dictionary<string, string> paramDocs, out List<string> examples)
        {
            synopsis = string.Empty;
            description = string.Empty;
            paramDocs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            examples = new List<string>();

            if (string.IsNullOrWhiteSpace(blockContent)) return;

            var matches = RxHelpSections.Matches(blockContent);
            if (matches.Count == 0)
            {
                // 无标准指令节，作为整段描述
                description = blockContent.Trim();
                return;
            }

            for (int i = 0; i < matches.Count; i++)
            {
                Match curr = matches[i];
                string sectionName = curr.Groups[1].Value.ToUpperInvariant();
                string sectionArg = curr.Groups[2].Success ? curr.Groups[2].Value.Trim() : string.Empty;

                int contentStart = curr.Index + curr.Length;
                int contentEnd = (i + 1 < matches.Count) ? matches[i + 1].Index : blockContent.Length;
                string secBody = blockContent.Substring(contentStart, contentEnd - contentStart).Trim();

                switch (sectionName)
                {
                    case "SYNOPSIS":
                        if (string.IsNullOrEmpty(synopsis)) synopsis = secBody;
                        break;
                    case "DESCRIPTION":
                        if (string.IsNullOrEmpty(description)) description = secBody;
                        break;
                    case "PARAMETER":
                        if (!string.IsNullOrEmpty(sectionArg))
                            paramDocs[sectionArg] = secBody;
                        break;
                    case "EXAMPLE":
                        if (!string.IsNullOrEmpty(secBody))
                            examples.Add(secBody);
                        break;
                }
            }
        }

        /// <summary>
        /// 提取顶层 param(...) 块参数
        /// </summary>
        private static void ExtractTopLevelParameters(string content, PsScriptAnalysisResult res)
        {
            // 查找脚本顶部的 param 块（通常在脚本开头部，忽略函数内部的 param）
            Match paramMatch = RxParamBlock.Match(content);
            if (paramMatch.Success)
            {
                // 确保该 param 块在第一个 function 出现之前
                int firstFuncIndex = content.IndexOf("function ", StringComparison.OrdinalIgnoreCase);
                if (firstFuncIndex < 0 || paramMatch.Index < firstFuncIndex)
                {
                    string rawParams = paramMatch.Groups[1].Value;
                    res.TopLevelParameters = ParseParameterList(rawParams);
                }
            }
        }

        /// <summary>
        /// 解析具体的参数列表文本，支持 [Parameter()], [ValidateSet()], 类型修饰与默认值
        /// </summary>
        public static List<PsParameterInfo> ParseParameterList(string rawParams)
        {
            var list = new List<PsParameterInfo>();
            if (string.IsNullOrWhiteSpace(rawParams)) return list;

            // 按照顶层逗号分割参数声明 (需规避 ValidateSet("a,b") 等内部逗号)
            List<string> paramChunks = SplitParameters(rawParams);

            foreach (string chunk in paramChunks)
            {
                string trimmed = chunk.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                var p = new PsParameterInfo();

                // 1. 检查 Mandatory
                if (Regex.IsMatch(trimmed, @"Mandatory\s*=\s*\$true\b", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(trimmed, @"\[Parameter\([^)]*Mandatory[^)]*\)\]", RegexOptions.IgnoreCase))
                {
                    p.IsMandatory = true;
                }

                // 2. 检查 ValidateSet
                Match vsMatch = Regex.Match(trimmed, @"\[ValidateSet\(([^)]+)\)\]", RegexOptions.IgnoreCase);
                if (vsMatch.Success)
                {
                    string inside = vsMatch.Groups[1].Value;
                    string[] items = inside.Split(',');
                    foreach (var item in items)
                    {
                        string cleanItem = item.Trim().Trim('"', '\'');
                        if (!string.IsNullOrEmpty(cleanItem)) p.ValidateSet.Add(cleanItem);
                    }
                }

                // 3. 提取类型: 例如 [string], [switch], [int], [System.IO.FileInfo]
                // 排除 [Parameter] 和 [ValidateSet]
                MatchCollection typeMatches = Regex.Matches(trimmed, @"\[([a-zA-Z0-9_\.]+)\]");
                foreach (Match tm in typeMatches)
                {
                    string tName = tm.Groups[1].Value;
                    if (!tName.Equals("Parameter", StringComparison.OrdinalIgnoreCase) &&
                        !tName.Equals("ValidateSet", StringComparison.OrdinalIgnoreCase) &&
                        !tName.Equals("ValidateNotNullOrEmpty", StringComparison.OrdinalIgnoreCase) &&
                        !tName.Equals("Alias", StringComparison.OrdinalIgnoreCase))
                    {
                        p.Type = tName;
                    }
                }

                // 如果显式标记为 [switch]，类型设为 switch
                if (Regex.IsMatch(trimmed, @"\[switch\]", RegexOptions.IgnoreCase))
                {
                    p.Type = "switch";
                }

                // 4. 提取变量名与默认值: $ParamName = DefaultVal
                Match nameMatch = Regex.Match(trimmed, @"\$([a-zA-Z0-9_]+)(?:\s*=\s*([\s\S]+))?");
                if (nameMatch.Success)
                {
                    p.Name = nameMatch.Groups[1].Value;
                    if (nameMatch.Groups[2].Success)
                    {
                        p.DefaultValue = nameMatch.Groups[2].Value.Trim().TrimEnd(',', ';');
                    }
                    list.Add(p);
                }
            }

            return list;
        }

        /// <summary>
        /// 顶层逗号智能分割参数声明（考虑括号嵌套）
        /// </summary>
        private static List<string> SplitParameters(string text)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            int parenDepth = 0;
            int bracketDepth = 0;
            bool inQuote = false;
            char quoteChar = '\0';

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (inQuote)
                {
                    current.Append(c);
                    if (c == quoteChar && (i == 0 || text[i - 1] != '`'))
                    {
                        inQuote = false;
                    }
                    continue;
                }

                if (c == '"' || c == '\'')
                {
                    inQuote = true;
                    quoteChar = c;
                    current.Append(c);
                    continue;
                }

                if (c == '(') parenDepth++;
                else if (c == ')') { if (parenDepth > 0) parenDepth--; }
                else if (c == '[') bracketDepth++;
                else if (c == ']') { if (bracketDepth > 0) bracketDepth--; }
                else if (c == ',' && parenDepth == 0 && bracketDepth == 0)
                {
                    result.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(c);
            }

            if (current.Length > 0)
                result.Add(current.ToString());

            return result;
        }

        /// <summary>
        /// 提取函数列表及其内部文档和参数
        /// </summary>
        private static void ExtractFunctions(string content, string[] lines, PsScriptAnalysisResult res)
        {
            MatchCollection funcMatches = RxFunction.Matches(content);
            foreach (Match m in funcMatches)
            {
                string funcName = m.Groups[1].Value;
                int charIndex = m.Index;

                // 计算对应的 1-based 行号
                int lineNo = 1;
                for (int i = 0; i < content.Length && i < charIndex; i++)
                {
                    if (content[i] == '\n') lineNo++;
                }

                var f = new PsFunctionInfo
                {
                    Name = funcName,
                    LineNumber = lineNo
                };

                // 提取函数内局部帮助块或签名参数
                int searchLen = Math.Min(content.Length - charIndex, 2500);
                string funcSnippet = content.Substring(charIndex, searchLen);

                // 检查函数内部是否有 <# ... #>
                Match helpM = RxCommentHelpBlock.Match(funcSnippet);
                if (helpM.Success && helpM.Index < 300)
                {
                    ParseHelpBlock(helpM.Groups[1].Value, out string fSyn, out string fDesc, out var fParamDocs, out var fExs);
                    f.Synopsis = fSyn;
                    f.Description = fDesc;
                    f.Examples.AddRange(fExs);
                }

                // 检查函数参数: (param1, param2) 或内部 param(...)
                if (m.Groups[2].Success && !string.IsNullOrWhiteSpace(m.Groups[2].Value))
                {
                    f.Parameters = ParseParameterList(m.Groups[2].Value);
                }
                else
                {
                    Match internalParam = RxParamBlock.Match(funcSnippet);
                    if (internalParam.Success && internalParam.Index < 500)
                    {
                        f.Parameters = ParseParameterList(internalParam.Groups[1].Value);
                    }
                }

                res.Functions.Add(f);
            }
        }

        /// <summary>
        /// 根据提取的参数自动构建命令行调用原型 (Syntax Synopsis)
        /// 例如: .\deploy.ps1 [-TargetFolder <string>] [-EnableRetinaExport]
        /// </summary>
        private static void BuildSyntaxSynopsis(PsScriptAnalysisResult res)
        {
            var sb = new StringBuilder();
            sb.Append(".\\").Append(res.ScriptName);

            if (res.TopLevelParameters.Count == 0)
            {
                sb.Append(" [<常规参数>]");
                res.SyntaxSynopsis = sb.ToString();
                return;
            }

            foreach (var p in res.TopLevelParameters)
            {
                sb.Append(" ");
                if (!p.IsMandatory) sb.Append("[");

                sb.Append("-").Append(p.Name);
                if (!p.Type.Equals("switch", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" <").Append(p.Type).Append(">");
                }

                if (!p.IsMandatory) sb.Append("]");
            }

            res.SyntaxSynopsis = sb.ToString();
        }
    }
}
