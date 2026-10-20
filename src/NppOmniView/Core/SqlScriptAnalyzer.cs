using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NppPlugin.OmniView.Core
{
    /// <summary>
    /// SQL 结构元素类型
    /// </summary>
    public enum SqlItemType
    {
        Table,          // 数据表 (CREATE / ALTER / DROP TABLE)
        View,           // 视图 (CREATE VIEW)
        Procedure,      // 存储过程 (CREATE PROCEDURE)
        Function,       // 自定义函数 (CREATE FUNCTION)
        Index,          // 索引 (CREATE INDEX)
        Trigger,        // 触发器 (CREATE TRIGGER)
        Query,          // 独立查询 (SELECT / WITH CTE)
        Dml,            // 数据变更 (INSERT / UPDATE / DELETE / MERGE)
        Section         // 业务分区注释
    }

    /// <summary>
    /// SQL 结构大纲条目模型
    /// </summary>
    public class SqlItemInfo
    {
        /// <summary>
        /// 对象或片段名称 (如表名、过程名、查询摘要)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 结构分类类型
        /// </summary>
        public SqlItemType Type { get; set; } = SqlItemType.Query;

        /// <summary>
        /// 所在代码行号 (1-based)
        /// </summary>
        public int LineNumber { get; set; } = 1;

        /// <summary>
        /// 伴随说明与前置注释
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 代码原型或语句摘要
        /// </summary>
        public string Snippet { get; set; } = string.Empty;
    }

    /// <summary>
    /// SQL 脚本全景分析结果模型
    /// </summary>
    public class SqlScriptAnalysisResult
    {
        /// <summary>
        /// 脚本文件名
        /// </summary>
        public string ScriptName { get; set; } = string.Empty;

        /// <summary>
        /// 文档总行数
        /// </summary>
        public int TotalLines { get; set; }

        /// <summary>
        /// 数据库方言推测提示 (如 MySQL / PostgreSQL / SQL Server / SQLite / Oracle)
        /// </summary>
        public string DialectHint { get; set; } = "标准 SQL";

        /// <summary>
        /// 提取出的所有结构与导航项
        /// </summary>
        public List<SqlItemInfo> Items { get; set; } = new List<SqlItemInfo>();

        public int TableCount { get; set; }
        public int ViewCount { get; set; }
        public int ProcedureCount { get; set; }
        public int FunctionCount { get; set; }
        public int IndexCount { get; set; }
        public int QueryCount { get; set; }
        public int DmlCount { get; set; }
    }

    /// <summary>
    /// 专属于 SQL 脚本文件的快速轻量级结构分析器 (AST/正则启发式分词)
    /// </summary>
    public static class SqlScriptAnalyzer
    {
        // 匹配 DDL 数据对象
        private static readonly Regex RxCreateTable = new Regex(@"\bcreate\s+(?:or\s+replace\s+)?(?:temporary\s+|temp\s+)?table\s+(?:if\s+not\s+exists\s+)?([`""\[\]a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxAlterTable = new Regex(@"\balter\s+table\s+([`""\[\]a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxDropTable = new Regex(@"\bdrop\s+table\s+(?:if\s+exists\s+)?([`""\[\]a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxCreateView = new Regex(@"\bcreate\s+(?:or\s+replace\s+)?(?:materialized\s+)?view\s+([`""\[\]a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxCreateProc = new Regex(@"\bcreate\s+(?:or\s+replace\s+)?(?:procedure|proc)\s+([`""\[\]a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxCreateFunc = new Regex(@"\bcreate\s+(?:or\s+replace\s+)?function\s+([`""\[\]a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxCreateIndex = new Regex(@"\bcreate\s+(?:unique\s+)?index\s+([`""\[\]a-zA-Z0-9_\.]+)(?:\s+on\s+([`""\[\]a-zA-Z0-9_\.]+))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxCreateTrigger = new Regex(@"\bcreate\s+(?:or\s+replace\s+)?trigger\s+([`""\[\]a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // 匹配关键 DML 与查询
        private static readonly Regex RxSelectQuery = new Regex(@"^\s*(?:with\s+[a-zA-Z0-9_]+\s+as\s*\(|select\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxInsert = new Regex(@"\binsert\s+into\s+([`""\[\]a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxUpdate = new Regex(@"\bupdate\s+([`""\[\]a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxDelete = new Regex(@"\bdelete\s+from\s+([`""\[\]a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // 匹配注释与分区
        private static readonly Regex RxSingleLineComment = new Regex(@"^\s*--\s*(.*)$", RegexOptions.Compiled);
        private static readonly Regex RxSectionBanner = new Regex(@"^\s*--\s*[-=]{3,}\s*(.+?)\s*[-=]{3,}\s*$", RegexOptions.Compiled);

        // 数据库方言特征词
        private static readonly Regex RxMySql = new Regex(@"\b(engine\s*=|auto_increment|utf8mb4|tinyint\(1\))\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxPgSql = new Regex(@"\b(serial|bigserial|now\(\)|timestamptz|returning|citext)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxSqlServer = new Regex(@"\b(nvarchar|identity\(|getdate\(\)|sysobjects|syscolumns)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxOracle = new Regex(@"\b(varchar2|number\(|sysdate|dual|rownum)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxSqlite = new Regex(@"\b(autoincrement|pragma\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static SqlScriptAnalysisResult Analyze(string content, string fullPath)
        {
            var result = new SqlScriptAnalysisResult
            {
                ScriptName = !string.IsNullOrEmpty(fullPath) ? Path.GetFileName(fullPath) : "未命名脚本.sql"
            };

            if (string.IsNullOrEmpty(content)) return result;

            string[] lines = content.Split('\n');
            result.TotalLines = lines.Length;

            // 方言启发式推断
            if (RxMySql.IsMatch(content)) result.DialectHint = "MySQL 方言";
            else if (RxPgSql.IsMatch(content)) result.DialectHint = "PostgreSQL 方言";
            else if (RxSqlServer.IsMatch(content)) result.DialectHint = "SQL Server (T-SQL)";
            else if (RxOracle.IsMatch(content)) result.DialectHint = "Oracle (PL/SQL)";
            else if (RxSqlite.IsMatch(content)) result.DialectHint = "SQLite";

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

                // 1. 检查是否为注释行或业务分区
                var secMatch = RxSectionBanner.Match(trimmed);
                if (secMatch.Success)
                {
                    string secTitle = secMatch.Groups[1].Value.Trim();
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = secTitle,
                        Type = SqlItemType.Section,
                        LineNumber = lineNo,
                        Description = "业务分区分割线",
                        Snippet = trimmed
                    });
                    pendingComment = "";
                    continue;
                }

                var commentMatch = RxSingleLineComment.Match(trimmed);
                if (commentMatch.Success)
                {
                    string cText = commentMatch.Groups[1].Value.Trim(' ', '\t', '-', '=');
                    if (!string.IsNullOrEmpty(cText))
                    {
                        pendingComment = cText;
                    }
                    continue;
                }

                // 2. 匹配 CREATE / ALTER / DROP TABLE
                var ctMatch = RxCreateTable.Match(trimmed);
                if (ctMatch.Success)
                {
                    string tblName = CleanObjectName(ctMatch.Groups[1].Value);
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = tblName,
                        Type = SqlItemType.Table,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "新建数据表 (CREATE TABLE)",
                        Snippet = TrimSnippet(trimmed)
                    });
                    result.TableCount++;
                    pendingComment = "";
                    continue;
                }

                var atMatch = RxAlterTable.Match(trimmed);
                if (atMatch.Success)
                {
                    string tblName = CleanObjectName(atMatch.Groups[1].Value);
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = "修改表: " + tblName,
                        Type = SqlItemType.Table,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "结构变更 (ALTER TABLE)",
                        Snippet = TrimSnippet(trimmed)
                    });
                    pendingComment = "";
                    continue;
                }

                // 3. 匹配 CREATE VIEW
                var cvMatch = RxCreateView.Match(trimmed);
                if (cvMatch.Success)
                {
                    string viewName = CleanObjectName(cvMatch.Groups[1].Value);
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = viewName,
                        Type = SqlItemType.View,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "视图定义 (CREATE VIEW)",
                        Snippet = TrimSnippet(trimmed)
                    });
                    result.ViewCount++;
                    pendingComment = "";
                    continue;
                }

                // 4. 匹配 存储过程 与 函数
                var cpMatch = RxCreateProc.Match(trimmed);
                if (cpMatch.Success)
                {
                    string procName = CleanObjectName(cpMatch.Groups[1].Value);
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = procName,
                        Type = SqlItemType.Procedure,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "存储过程 (CREATE PROCEDURE)",
                        Snippet = TrimSnippet(trimmed)
                    });
                    result.ProcedureCount++;
                    pendingComment = "";
                    continue;
                }

                var cfMatch = RxCreateFunc.Match(trimmed);
                if (cfMatch.Success)
                {
                    string funcName = CleanObjectName(cfMatch.Groups[1].Value);
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = funcName,
                        Type = SqlItemType.Function,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "自定义函数 (CREATE FUNCTION)",
                        Snippet = TrimSnippet(trimmed)
                    });
                    result.FunctionCount++;
                    pendingComment = "";
                    continue;
                }

                // 5. 匹配 索引 与 触发器
                var idxMatch = RxCreateIndex.Match(trimmed);
                if (idxMatch.Success)
                {
                    string idxName = CleanObjectName(idxMatch.Groups[1].Value);
                    string onTable = idxMatch.Groups[2].Success ? CleanObjectName(idxMatch.Groups[2].Value) : "";
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = idxName + (!string.IsNullOrEmpty(onTable) ? " ON " + onTable : ""),
                        Type = SqlItemType.Index,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "索引定义 (CREATE INDEX)",
                        Snippet = TrimSnippet(trimmed)
                    });
                    result.IndexCount++;
                    pendingComment = "";
                    continue;
                }

                var trgMatch = RxCreateTrigger.Match(trimmed);
                if (trgMatch.Success)
                {
                    string trgName = CleanObjectName(trgMatch.Groups[1].Value);
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = trgName,
                        Type = SqlItemType.Trigger,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "触发器 (CREATE TRIGGER)",
                        Snippet = TrimSnippet(trimmed)
                    });
                    pendingComment = "";
                    continue;
                }

                // 6. 匹配 DML 数据变更
                var insMatch = RxInsert.Match(trimmed);
                if (insMatch.Success)
                {
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = "INSERT INTO " + CleanObjectName(insMatch.Groups[1].Value),
                        Type = SqlItemType.Dml,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "数据插入语句",
                        Snippet = TrimSnippet(trimmed)
                    });
                    result.DmlCount++;
                    pendingComment = "";
                    continue;
                }

                var updMatch = RxUpdate.Match(trimmed);
                if (updMatch.Success)
                {
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = "UPDATE " + CleanObjectName(updMatch.Groups[1].Value),
                        Type = SqlItemType.Dml,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "数据更新语句",
                        Snippet = TrimSnippet(trimmed)
                    });
                    result.DmlCount++;
                    pendingComment = "";
                    continue;
                }

                var delMatch = RxDelete.Match(trimmed);
                if (delMatch.Success)
                {
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = "DELETE FROM " + CleanObjectName(delMatch.Groups[1].Value),
                        Type = SqlItemType.Dml,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "数据删除语句",
                        Snippet = TrimSnippet(trimmed)
                    });
                    result.DmlCount++;
                    pendingComment = "";
                    continue;
                }

                // 7. 匹配 SELECT 查询
                if (RxSelectQuery.IsMatch(trimmed))
                {
                    string querySnippet = TrimSnippet(trimmed);
                    result.Items.Add(new SqlItemInfo
                    {
                        Name = querySnippet,
                        Type = SqlItemType.Query,
                        LineNumber = lineNo,
                        Description = !string.IsNullOrEmpty(pendingComment) ? pendingComment : "数据查询 (SELECT 语句)",
                        Snippet = querySnippet
                    });
                    result.QueryCount++;
                    pendingComment = "";
                    continue;
                }

                pendingComment = "";
            }

            return result;
        }

        private static string CleanObjectName(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "";
            return raw.Trim('`', '"', '[', ']', ' ', '\t');
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
