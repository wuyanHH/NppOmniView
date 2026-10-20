using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NppPlugin.OmniView.Core
{
    /// <summary>
    /// Shell 脚本元素类型
    /// </summary>
    public enum ShellItemType
    {
        Function,   // 函数声明
        Export,     // 环境变量与常量导出
        Source,     // 外部依赖脚本导入
        Section     // 注释业务分区
    }

    /// <summary>
    /// Shell 大纲条目模型
    /// </summary>
    public class ShellItemInfo
    {
        /// <summary>
        /// 标识符或名称 (如函数名、变量名、依赖路径)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 条目类型
        /// </summary>
        public ShellItemType Type { get; set; } = ShellItemType.Function;

        /// <summary>
        /// 所在行号 (1-based)
        /// </summary>
        public int LineNumber { get; set; } = 1;

        /// <summary>
        /// 函数意图与注释说明
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 代码片段摘要
        /// </summary>
        public string Snippet { get; set; } = string.Empty;
    }

    /// <summary>
    /// Shell 脚本全景分析结果模型
    /// </summary>
    public class ShellScriptAnalysisResult
    {
        /// <summary>
        /// 脚本名称
        /// </summary>
        public string ScriptName { get; set; } = string.Empty;

        /// <summary>
        /// 文档总行数
        /// </summary>
        public int TotalLines { get; set; }

        /// <summary>
        /// 脚本解释器声明 (Shebang，如 #!/bin/bash)
        /// </summary>
        public string Shebang { get; set; } = string.Empty;

        /// <summary>
        /// 是否启用了严格错误检测 (如 set -e / set -euo pipefail)
        /// </summary>
        public bool HasStrictErrorCheck { get; set; }

        /// <summary>
        /// 大纲导航条目列表
        /// </summary>
        public List<ShellItemInfo> Items { get; set; } = new List<ShellItemInfo>();

        public int FunctionCount { get; set; }
        public int ExportCount { get; set; }
        public int SourceCount { get; set; }
    }

    /// <summary>
    /// 专属于 Shell / Bash / Zsh 脚本的结构、函数与依赖全景分析器
    /// </summary>
    public static class ShellScriptAnalyzer
    {
        // 匹配 Shebang 解释器声明
        private static readonly Regex RxShebang = new Regex(@"^#!\s*(.*)$", RegexOptions.Compiled);

        // 匹配函数定义: function foo() / function foo / foo()
        private static readonly Regex RxFunction = new Regex(@"^\s*(?:function\s+([a-zA-Z0-9_\-\.]+)(?:\s*\(\s*\))?|([a-zA-Z0-9_\-\.]+)\s*\(\s*\))\s*(?:\{|$)", RegexOptions.Compiled);

        // 匹配环境变量与常量导出
        private static readonly Regex RxExport = new Regex(@"^\s*(?:export|readonly)\s+([a-zA-Z_][a-zA-Z0-9_]*)(?:\s*=\s*(.*))?$", RegexOptions.Compiled);

        // 匹配外部脚本引入: source file.sh 或 . file.sh
        private static readonly Regex RxSource = new Regex(@"^\s*(?:source|\.)\s+([^\s#]+)", RegexOptions.Compiled);

        // 匹配严格模式 set -e / -u / -o pipefail
        private static readonly Regex RxStrict = new Regex(@"^\s*set\s+.*-(?:[a-zA-Z]*e|o\s+pipefail)", RegexOptions.Compiled);

        // 匹配注释与分区
        private static readonly Regex RxCommentLine = new Regex(@"^\s*#\s*(.*)$", RegexOptions.Compiled);
        private static readonly Regex RxSectionBanner = new Regex(@"^\s*#\s*[-=]{3,}\s*(.+?)\s*[-=]{3,}\s*$", RegexOptions.Compiled);

        public static ShellScriptAnalysisResult Analyze(string content, string fullPath)
        {
            var result = new ShellScriptAnalysisResult
            {
                ScriptName = !string.IsNullOrEmpty(fullPath) ? Path.GetFileName(fullPath) : "未命名脚本.sh"
            };

            if (string.IsNullOrEmpty(content)) return result;

            string[] lines = content.Split('\n');
            result.TotalLines = lines.Length;

            string pendingComment = "";

            for (int i = 0; i < lines.Length; i++)
            {
                int lineNo = i + 1;
                string rawLine = lines[i].TrimEnd('\r');
                string trimmed = rawLine.Trim();

                if (string.IsNullOrEmpty(trimmed))
                {
                    pendingComment = "";
                    continue;
                }

                // 1. 检查首行 Shebang
                if (i == 0 && trimmed.StartsWith("#!"))
                {
                    var sbMatch = RxShebang.Match(trimmed);
                    if (sbMatch.Success)
                    {
                        result.Shebang = sbMatch.Groups[1].Value.Trim();
                    }
                    continue;
                }

                // 2. 检查严格模式
                if (RxStrict.IsMatch(trimmed))
                {
                    result.HasStrictErrorCheck = true;
                }

                // 3. 检查注释行与分区条幅
                var secMatch = RxSectionBanner.Match(trimmed);
                if (secMatch.Success)
                {
                    string secTitle = secMatch.Groups[1].Value.Trim();
                    result.Items.Add(new ShellItemInfo
                    {
                        Name = secTitle,
                        Type = ShellItemType.Section,
                        LineNumber = lineNo,
                        Description = "脚本逻辑分区",
                        Snippet = trimmed
                    });
                    pendingComment = "";
                    continue;
                }

                var cmMatch = RxCommentLine.Match(trimmed);
                if (cmMatch.Success)
                {
                    string cText = cmMatch.Groups[1].Value.Trim(' ', '\t', '#', '-', '=');
                    if (!string.IsNullOrEmpty(cText))
                    {
                        pendingComment = cText;
                    }
                    continue;
                }

                // 4. 匹配函数定义
                var fnMatch = RxFunction.Match(trimmed);
                if (fnMatch.Success)
                {
                    string fnName = fnMatch.Groups[1].Success ? fnMatch.Groups[1].Value : fnMatch.Groups[2].Value;
                    if (!string.IsNullOrEmpty(fnName) && !fnName.Equals("if", StringComparison.OrdinalIgnoreCase) && !fnName.Equals("then", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Items.Add(new ShellItemInfo
                        {
                            Name = fnName + "()",
                            Type = ShellItemType.Function,
                            LineNumber = lineNo,
                            Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "Shell 内部函数定义",
                            Snippet = TrimSnippet(trimmed)
                        });
                        result.FunctionCount++;
                        pendingComment = "";
                        continue;
                    }
                }

                // 5. 匹配 source / . 外部引用
                var srcMatch = RxSource.Match(trimmed);
                if (srcMatch.Success)
                {
                    string srcTarget = srcMatch.Groups[1].Value.Trim('\'', '"');
                    result.Items.Add(new ShellItemInfo
                    {
                        Name = "source " + srcTarget,
                        Type = ShellItemType.Source,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "外部依赖脚本引入",
                        Snippet = TrimSnippet(trimmed)
                    });
                    result.SourceCount++;
                    pendingComment = "";
                    continue;
                }

                // 6. 匹配 export 环境变量
                var expMatch = RxExport.Match(trimmed);
                if (expMatch.Success)
                {
                    string varName = expMatch.Groups[1].Value;
                    string varVal = expMatch.Groups[2].Success ? expMatch.Groups[2].Value : "";
                    result.Items.Add(new ShellItemInfo
                    {
                        Name = "export " + varName,
                        Type = ShellItemType.Export,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "全局环境变量 / 常量",
                        Snippet = TrimSnippet(trimmed)
                    });
                    result.ExportCount++;
                    pendingComment = "";
                    continue;
                }

                pendingComment = "";
            }

            return result;
        }

        private static string TrimSnippet(string line)
        {
            if (string.IsNullOrEmpty(line)) return "";
            line = line.Replace('\t', ' ');
            if (line.Length > 60) return line.Substring(0, 57) + "...";
            return line;
        }
    }
}
