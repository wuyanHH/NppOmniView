using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NppPlugin.OmniView.Core
{
    /// <summary>
    /// INI / TOML / Conf 配置项条目模型
    /// </summary>
    public class IniPropertyItem
    {
        /// <summary>
        /// 配置键名 (Key)
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 配置值 (Value)
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 所在行号 (1-based)
        /// </summary>
        public int LineNumber { get; set; } = 1;

        /// <summary>
        /// 行内或前置注释说明
        /// </summary>
        public string Comment { get; set; } = string.Empty;
    }

    /// <summary>
    /// INI / TOML 配置分组 (Section) 模型
    /// </summary>
    public class IniSection
    {
        /// <summary>
        /// 节点分组名称 (如 [database], [server])
        /// </summary>
        public string Name { get; set; } = "全局通用设置 (Global)";

        /// <summary>
        /// Section 声明所在行号
        /// </summary>
        public int LineNumber { get; set; } = 1;

        /// <summary>
        /// 该节点下的配置项列表
        /// </summary>
        public List<IniPropertyItem> Properties { get; set; } = new List<IniPropertyItem>();
    }

    /// <summary>
    /// INI / TOML 配置文件分析结果模型
    /// </summary>
    public class IniConfigAnalysisResult
    {
        public string FileName { get; set; } = string.Empty;
        public int TotalLines { get; set; }
        public int TotalKeys { get; set; }
        public List<IniSection> Sections { get; set; } = new List<IniSection>();
    }

    /// <summary>
    /// 专属于 INI / TOML / CFG / Conf 配置文件的轻量结构解析器
    /// </summary>
    public static class IniConfigAnalyzer
    {
        // 匹配 Section 声明: [SectionName]
        private static readonly Regex RxSection = new Regex(@"^\s*\[([^\]]+)\]\s*(.*)$", RegexOptions.Compiled);

        // 匹配 Key = Value 或 Key: Value (排除注释)
        private static readonly Regex RxKeyValue = new Regex(@"^\s*([a-zA-Z0-9_\-\.\$@]+)\s*[:=]\s*(.*)$", RegexOptions.Compiled);

        // 匹配独立注释行: ; 注释 或 # 注释
        private static readonly Regex RxComment = new Regex(@"^\s*[;#]\s*(.*)$", RegexOptions.Compiled);

        public static IniConfigAnalysisResult Analyze(string content, string fullPath)
        {
            var result = new IniConfigAnalysisResult
            {
                FileName = !string.IsNullOrEmpty(fullPath) ? Path.GetFileName(fullPath) : "config.ini"
            };

            if (string.IsNullOrEmpty(content)) return result;

            string[] lines = content.Split('\n');
            result.TotalLines = lines.Length;

            var currentSection = new IniSection { Name = "常规配置 (Default)", LineNumber = 1 };
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

                // 1. 检查是否为注释行
                var cmMatch = RxComment.Match(trimmed);
                if (cmMatch.Success)
                {
                    string commentText = cmMatch.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(commentText))
                    {
                        pendingComment = commentText;
                    }
                    continue;
                }

                // 2. 检查是否为 Section 声明 [section]
                var secMatch = RxSection.Match(trimmed);
                if (secMatch.Success)
                {
                    // 若前一个 section 中有配置，则归档加入结果
                    if (currentSection.Properties.Count > 0 || currentSection.Name != "常规配置 (Default)")
                    {
                        result.Sections.Add(currentSection);
                    }

                    string secName = secMatch.Groups[1].Value.Trim();
                    currentSection = new IniSection
                    {
                        Name = secName,
                        LineNumber = lineNo
                    };
                    pendingComment = "";
                    continue;
                }

                // 3. 检查是否为 Key = Value 键值对
                var kvMatch = RxKeyValue.Match(trimmed);
                if (kvMatch.Success)
                {
                    string key = kvMatch.Groups[1].Value.Trim();
                    string rawVal = kvMatch.Groups[2].Value.Trim();

                    // 分离行尾注释 (; 或 #)
                    string val = rawVal;
                    string inlineComment = "";

                    int semiIdx = val.IndexOf(';');
                    int hashIdx = val.IndexOf('#');
                    int cIdx = -1;

                    if (semiIdx >= 0 && hashIdx >= 0) cIdx = Math.Min(semiIdx, hashIdx);
                    else if (semiIdx >= 0) cIdx = semiIdx;
                    else if (hashIdx >= 0) cIdx = hashIdx;

                    if (cIdx >= 0)
                    {
                        inlineComment = val.Substring(cIdx + 1).Trim();
                        val = val.Substring(0, cIdx).Trim();
                    }

                    string finalComment = !string.IsNullOrEmpty(inlineComment) ? inlineComment : pendingComment;

                    currentSection.Properties.Add(new IniPropertyItem
                    {
                        Key = key,
                        Value = val,
                        LineNumber = lineNo,
                        Comment = finalComment
                    });
                    result.TotalKeys++;
                    pendingComment = "";
                }
            }

            if (currentSection.Properties.Count > 0 || result.Sections.Count == 0)
            {
                result.Sections.Add(currentSection);
            }

            return result;
        }
    }
}
