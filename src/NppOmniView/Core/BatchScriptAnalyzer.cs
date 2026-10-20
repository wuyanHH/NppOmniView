using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NppPlugin.OmniView.Core
{
    /// <summary>
    /// 批处理标签符号模型 (用于流程大纲导航)
    /// </summary>
    public class BatchLabelInfo
    {
        /// <summary>
        /// 标签名称 (包含前缀冒号，如 :init)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 所在行号 (1-based)
        /// </summary>
        public int LineNumber { get; set; } = 1;

        /// <summary>
        /// 伴随说明与意图注释 (从行尾或相邻行提取)
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// 批处理安全与环境检测模型
    /// </summary>
    public class BatchSecurityNotice
    {
        /// <summary>
        /// 警告级别: "info" | "warning" | "danger"
        /// </summary>
        public string Level { get; set; } = "info";

        /// <summary>
        /// 审查标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 详细规避建议与原因说明
        /// </summary>
        public string Detail { get; set; } = string.Empty;

        /// <summary>
        /// 触发所在行号 (1-based)
        /// </summary>
        public int LineNumber { get; set; } = 1;

        /// <summary>
        /// 触发的代码片段
        /// </summary>
        public string Snippet { get; set; } = string.Empty;
    }

    /// <summary>
    /// 批处理脚本全景分析结果模型
    /// </summary>
    public class BatchScriptAnalysisResult
    {
        public string ScriptName { get; set; } = string.Empty;
        public int TotalLines { get; set; }
        public List<BatchLabelInfo> Labels { get; set; } = new List<BatchLabelInfo>();
        public List<BatchSecurityNotice> SecurityNotices { get; set; } = new List<BatchSecurityNotice>();
        public bool HasEchoOff { get; set; }
        public bool HasDelayedExpansion { get; set; }
        public bool HasUtf8CodePage { get; set; }
    }

    /// <summary>
    /// 专属于 Windows Batch (.bat / .cmd) 脚本的结构、流程与安全全景分析器
    /// </summary>
    public static class BatchScriptAnalyzer
    {
        // 匹配普通标签 (排除 :: 注释标签)
        private static readonly Regex RxLabel = new Regex(@"^\s*:([a-zA-Z0-9_\-\.\$]+)(.*)$", RegexOptions.Compiled);

        // 匹配注释行
        private static readonly Regex RxCommentLine = new Regex(@"^\s*(::|REM\b)(.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // 安全审计高危/敏感指令正则
        private static readonly Regex RxDelForce = new Regex(@"\b(del|erase)\b(?=.*\/[fq])(?=.*\/[fq])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxRmDir = new Regex(@"\b(rmdir|rd)\b(?=.*\/s)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxFormat = new Regex(@"\bformat\b\s+[a-zA-Z]:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxTaskKill = new Regex(@"\btaskkill\b(?=.*\/f)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxRegDelete = new Regex(@"\breg\s+delete\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxDiskpart = new Regex(@"\bdiskpart\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // 环境与上下文配置检测
        private static readonly Regex RxEchoOff = new Regex(@"^\s*@?echo\s+off\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex RxDelayedExp = new Regex(@"setlocal\b(?=.*enabledelayedexpansion)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex RxChcpUtf8 = new Regex(@"chcp\s+65001\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static BatchScriptAnalysisResult Analyze(string content, string fullPath)
        {
            var result = new BatchScriptAnalysisResult
            {
                ScriptName = !string.IsNullOrEmpty(fullPath) ? Path.GetFileName(fullPath) : "未命名脚本.bat"
            };

            if (string.IsNullOrEmpty(content)) return result;

            string[] lines = content.Split('\n');
            result.TotalLines = lines.Length;

            result.HasEchoOff = RxEchoOff.IsMatch(content);
            result.HasDelayedExpansion = RxDelayedExp.IsMatch(content);
            result.HasUtf8CodePage = RxChcpUtf8.IsMatch(content);

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

                // 1. 检查是否为前置注释
                var cmMatch = RxCommentLine.Match(trimmed);
                if (cmMatch.Success)
                {
                    string cText = cmMatch.Groups[2].Value.Trim(' ', '\t', ':', '-');
                    if (!string.IsNullOrEmpty(cText))
                    {
                        pendingComment = cText;
                    }
                    continue;
                }

                // 2. 检查是否为跳转标签 (:label)
                if (trimmed.StartsWith(":") && !trimmed.StartsWith("::"))
                {
                    var labelMatch = RxLabel.Match(trimmed);
                    if (labelMatch.Success)
                    {
                        string labelName = ":" + labelMatch.Groups[1].Value.Trim();
                        string inlineComment = labelMatch.Groups[2].Value.Trim();

                        string desc = "";
                        if (!string.IsNullOrEmpty(inlineComment))
                        {
                            // 去掉可能跟随的 :: 或 rem
                            desc = inlineComment.TrimStart(' ', '\t', ':', '-');
                            if (desc.StartsWith("rem", StringComparison.OrdinalIgnoreCase))
                                desc = desc.Substring(3).TrimStart(' ', '\t', ':');
                        }
                        if (string.IsNullOrEmpty(desc) && !string.IsNullOrEmpty(pendingComment))
                        {
                            desc = pendingComment;
                        }
                        if (string.IsNullOrEmpty(desc))
                        {
                            desc = "脚本流程跳转锚点";
                        }

                        result.Labels.Add(new BatchLabelInfo
                        {
                            Name = labelName,
                            LineNumber = lineNo,
                            Description = desc
                        });
                        pendingComment = "";
                    }
                }
                else
                {
                    pendingComment = "";
                }

                // 3. 安全与危险命令模式审查
                CheckSecurityNotice(trimmed, lineNo, result.SecurityNotices);
            }

            // 4. 环境与全局安全提示补全
            if (!result.HasEchoOff && result.TotalLines > 5)
            {
                result.SecurityNotices.Add(new BatchSecurityNotice
                {
                    Level = "info",
                    Title = "建议在文件开头关闭命令回显",
                    Detail = "未检测到 @echo off。建议在首行添加 @echo off，保持终端输出整洁专业。",
                    LineNumber = 1,
                    Snippet = "@echo off"
                });
            }

            if (!result.HasUtf8CodePage && (content.Contains("汉字") || Regex.IsMatch(content, @"[\u4e00-\u9fa5]")))
            {
                result.SecurityNotices.Add(new BatchSecurityNotice
                {
                    Level = "info",
                    Title = "中文字符可能面临控制台代码页乱码",
                    Detail = "检测到脚本中包含中文字符但未声明 chcp 65001。若在纯英文 Windows 环境执行，建议在开头执行 chcp 65001 确保 UTF-8 正常渲染。",
                    LineNumber = 1,
                    Snippet = "chcp 65001 >nul"
                });
            }

            return result;
        }

        private static void CheckSecurityNotice(string line, int lineNo, List<BatchSecurityNotice> notices)
        {
            if (line.StartsWith("::") || line.StartsWith("REM", StringComparison.OrdinalIgnoreCase)) return;

            // 检查高危磁盘格式化
            if (RxFormat.IsMatch(line))
            {
                notices.Add(new BatchSecurityNotice
                {
                    Level = "danger",
                    Title = "发现高危磁盘格式化命令 (format)",
                    Detail = "代码含有 format 指令，此操作具有不可逆的破坏性，可能导致整盘数据永久丢失！",
                    LineNumber = lineNo,
                    Snippet = line
                });
                return;
            }

            // 检查注册表静默删除
            if (RxRegDelete.IsMatch(line))
            {
                notices.Add(new BatchSecurityNotice
                {
                    Level = "danger",
                    Title = "发现注册表删除命令 (reg delete)",
                    Detail = "修改或删除系统注册表键值可能导致系统或软件运行异常，请仔细核对目标注册表路径。",
                    LineNumber = lineNo,
                    Snippet = line
                });
                return;
            }

            // 检查 Diskpart 磁盘分区操作
            if (RxDiskpart.IsMatch(line))
            {
                notices.Add(new BatchSecurityNotice
                {
                    Level = "danger",
                    Title = "发现底层磁盘分区命令 (diskpart)",
                    Detail = "diskpart 将直接操作物理存储卷与分区表，极度危险，请谨慎确认！",
                    LineNumber = lineNo,
                    Snippet = line
                });
                return;
            }

            // 检查强制删除命令
            if (RxDelForce.IsMatch(line))
            {
                notices.Add(new BatchSecurityNotice
                {
                    Level = "warning",
                    Title = "发现强制静默删除指令 (del /f /q)",
                    Detail = "del /f /q 将跳过确认提示直接删除目标文件。若路径变量为空或未断言，可能误删意外目录！",
                    LineNumber = lineNo,
                    Snippet = line
                });
            }

            // 检查递归删除目录命令
            if (RxRmDir.IsMatch(line))
            {
                notices.Add(new BatchSecurityNotice
                {
                    Level = "warning",
                    Title = "发现递归删除目录指令 (rd /s /q)",
                    Detail = "rd /s /q 将无提示强制清除整棵目录树，请确保目标路径合法且非根目录。",
                    LineNumber = lineNo,
                    Snippet = line
                });
            }

            // 检查强制杀死进程
            if (RxTaskKill.IsMatch(line))
            {
                notices.Add(new BatchSecurityNotice
                {
                    Level = "warning",
                    Title = "发现强制终止进程指令 (taskkill /f)",
                    Detail = "taskkill /f 将跳过应用退出保存逻辑直接杀进程，可能造成被杀应用未保存的数据损坏。",
                    LineNumber = lineNo,
                    Snippet = line
                });
            }
        }
    }
}
