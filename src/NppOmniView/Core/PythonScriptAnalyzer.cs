using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NppPlugin.OmniView.Core
{
    /// <summary>
    /// Python 符号结构类型
    /// </summary>
    public enum PySymbolType
    {
        Class,      // 类定义 (class)
        Function,   // 顶级函数 (def)
        Method      // 类成员方法 (def within class)
    }

    /// <summary>
    /// Python 符号大纲条目模型
    /// </summary>
    public class PySymbolInfo
    {
        /// <summary>
        /// 符号名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 符号类型
        /// </summary>
        public PySymbolType Type { get; set; } = PySymbolType.Function;

        /// <summary>
        /// 所在行号 (1-based)
        /// </summary>
        public int LineNumber { get; set; } = 1;

        /// <summary>
        /// 参数列表签名 (如 (self, name, age=18))
        /// </summary>
        public string Parameters { get; set; } = string.Empty;

        /// <summary>
        /// 注释或 Docstring 说明
        /// </summary>
        public string Docstring { get; set; } = string.Empty;

        /// <summary>
        /// 所属父类名称 (若为成员方法)
        /// </summary>
        public string ParentClass { get; set; } = string.Empty;
    }

    /// <summary>
    /// Python 脚本全景分析结果模型
    /// </summary>
    public class PythonScriptAnalysisResult
    {
        public string ScriptName { get; set; } = string.Empty;
        public int TotalLines { get; set; }
        public List<PySymbolInfo> Symbols { get; set; } = new List<PySymbolInfo>();
        public int ClassCount { get; set; }
        public int FunctionCount { get; set; }
    }

    /// <summary>
    /// 专属于 Python 脚本文件的轻量函数与类结构分析器
    /// </summary>
    public static class PythonScriptAnalyzer
    {
        // 匹配类定义: class ClassName(Base):
        private static readonly Regex RxClass = new Regex(@"^([ \t]*)class\s+([a-zA-Z_][a-zA-Z0-9_]*)(?:\s*\((.*?)\))?\s*:", RegexOptions.Compiled);

        // 匹配函数定义: def func_name(params):
        private static readonly Regex RxDef = new Regex(@"^([ \t]*)def\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*\((.*?)\)\s*(?:->.*?)?:", RegexOptions.Compiled);

        // 匹配注释
        private static readonly Regex RxComment = new Regex(@"^\s*#\s*(.*)$", RegexOptions.Compiled);

        public static PythonScriptAnalysisResult Analyze(string content, string fullPath)
        {
            var result = new PythonScriptAnalysisResult
            {
                ScriptName = !string.IsNullOrEmpty(fullPath) ? Path.GetFileName(fullPath) : "script.py"
            };

            if (string.IsNullOrEmpty(content)) return result;

            string[] lines = content.Split('\n');
            result.TotalLines = lines.Length;

            string currentClassName = "";
            int currentClassIndent = -1;
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

                // 检查注释
                var cmMatch = RxComment.Match(trimmed);
                if (cmMatch.Success)
                {
                    string comment = cmMatch.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(comment))
                    {
                        pendingComment = comment;
                    }
                    continue;
                }

                // 计算缩进层级
                int indent = 0;
                for (int c = 0; c < rawLine.Length; c++)
                {
                    if (rawLine[c] == ' ') indent++;
                    else if (rawLine[c] == '\t') indent += 4;
                    else break;
                }

                if (currentClassIndent >= 0 && indent <= currentClassIndent && !string.IsNullOrEmpty(trimmed))
                {
                    currentClassName = "";
                    currentClassIndent = -1;
                }

                // 1. 匹配类定义
                var classMatch = RxClass.Match(rawLine);
                if (classMatch.Success)
                {
                    string clsName = classMatch.Groups[2].Value;
                    string bases = classMatch.Groups[3].Success ? classMatch.Groups[3].Value.Trim() : "";

                    result.Symbols.Add(new PySymbolInfo
                    {
                        Name = "class " + clsName + (!string.IsNullOrEmpty(bases) ? "(" + bases + ")" : ""),
                        Type = PySymbolType.Class,
                        LineNumber = lineNo,
                        Docstring = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "类结构定义"
                    });
                    result.ClassCount++;

                    currentClassName = clsName;
                    currentClassIndent = indent;
                    pendingComment = "";
                    continue;
                }

                // 2. 匹配函数或方法
                var defMatch = RxDef.Match(rawLine);
                if (defMatch.Success)
                {
                    string fnName = defMatch.Groups[2].Value;
                    string parameters = defMatch.Groups[3].Success ? defMatch.Groups[3].Value.Trim() : "";

                    bool isMethod = !string.IsNullOrEmpty(currentClassName) && indent > currentClassIndent;

                    result.Symbols.Add(new PySymbolInfo
                    {
                        Name = fnName + "()",
                        Type = isMethod ? PySymbolType.Method : PySymbolType.Function,
                        LineNumber = lineNo,
                        Parameters = parameters,
                        ParentClass = isMethod ? currentClassName : "",
                        Docstring = !string.IsNullOrEmpty(pendingComment) ? pendingComment : (isMethod ? currentClassName + " 成员方法" : "顶级独立函数")
                    });
                    result.FunctionCount++;
                    pendingComment = "";
                    continue;
                }

                pendingComment = "";
            }

            return result;
        }
    }
}
