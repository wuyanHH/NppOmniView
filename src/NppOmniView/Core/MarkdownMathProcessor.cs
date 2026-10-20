using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NppPlugin.OmniView.Core
{
    /// <summary>
    /// 数学公式项元数据容器
    /// </summary>
    public sealed class MathItem
    {
        /// <summary>
        /// 公式唯一标识 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 是否为独立块级公式 (Display Math)
        /// </summary>
        public bool IsBlock { get; set; }

        /// <summary>
        /// 提取出的原始 LaTeX 公式源码
        /// </summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// Markdown 科学计算与 LaTeX 数学公式智能隔离预处理器
    /// <para>
    /// 解决 Markdig 原生数学扩展对非标准栅栏换行 (如 $$\begin{aligned} 与 \end{aligned}$$)
    /// 误判定为普通段落导致 GenericAttributes 属性解析器吞噬大括号与破坏 LaTeX 语法的核心缺陷。
    /// 在进入 Markdig 管道前将数学块替换为安全令牌并严格保持行数 1:1 对齐，渲染为 HTML 后无缝还原为 KaTeX 容器。
    /// </para>
    /// </summary>
    public static class MarkdownMathProcessor
    {
        // 占位符前缀与后缀定义
        private const string BlockTokenPrefix = "OMNI_MATH_BLOCK_TOKEN_";
        private const string InlineTokenPrefix = "OMNI_MATH_INLINE_TOKEN_";
        private const string TokenSuffix = "_END";

        /// <summary>
        /// 预处理 Markdown 源码中的数学公式，将其替换为隔离令牌并精确保持源文本行号对齐
        /// </summary>
        /// <param name="markdown">原始 Markdown 源码</param>
        /// <param name="items">提取出的数学公式列表</param>
        /// <returns>安全隔离后的 Markdown 源码</returns>
        public static string Preprocess(string markdown, out List<MathItem> items)
        {
            items = new List<MathItem>();
            if (string.IsNullOrEmpty(markdown) || markdown.IndexOf('$') < 0)
            {
                return markdown ?? string.Empty;
            }

            // 按行拆分，兼容各种操作系统的换行规范 (\r\n, \n, \r)
            string[] rawLines = markdown.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            string newline = markdown.Contains("\r\n") ? "\r\n" : "\n";
            var resultSb = new StringBuilder(markdown.Length + 512);

            bool inCodeFence = false;
            char fenceChar = '\0';
            int fenceMinLength = 0;

            bool inMathBlock = false;
            var currentMathContent = new StringBuilder();
            int currentMathId = -1;

            for (int i = 0; i < rawLines.Length; i++)
            {
                string line = rawLines[i];
                string trimmed = line.TrimStart();

                // 1. 代码围栏 (Code Fence) 状态检测与穿透保护
                if (!inMathBlock)
                {
                    if (trimmed.Length >= 3 && (trimmed[0] == '`' || trimmed[0] == '~'))
                    {
                        char c = trimmed[0];
                        int count = 0;
                        while (count < trimmed.Length && trimmed[count] == c) count++;

                        if (count >= 3)
                        {
                            if (!inCodeFence)
                            {
                                // 进入代码围栏
                                inCodeFence = true;
                                fenceChar = c;
                                fenceMinLength = count;
                                resultSb.Append(line).Append(newline);
                                continue;
                            }
                            else if (c == fenceChar && count >= fenceMinLength && trimmed.Substring(count).Trim().Length == 0)
                            {
                                // 闭合代码围栏
                                inCodeFence = false;
                                fenceChar = '\0';
                                fenceMinLength = 0;
                                resultSb.Append(line).Append(newline);
                                continue;
                            }
                        }
                    }

                    if (inCodeFence)
                    {
                        // 处于代码块内部的所有行直接保留，不进行数学公式匹配
                        resultSb.Append(line).Append(newline);
                        continue;
                    }
                }

                // 2. 处于多行块级数学公式 (Multi-line Math Block) 内部
                if (inMathBlock)
                {
                    int closeIdx = FindUnescapedDollarDollar(line, 0);
                    if (closeIdx >= 0)
                    {
                        // 找到了多行公式的闭合 $$
                        currentMathContent.Append(line.Substring(0, closeIdx));
                        items.Add(new MathItem
                        {
                            Id = currentMathId,
                            IsBlock = true,
                            Content = currentMathContent.ToString()
                        });

                        inMathBlock = false;
                        currentMathContent.Clear();

                        string remainder = line.Substring(closeIdx + 2);
                        if (string.IsNullOrWhiteSpace(remainder))
                        {
                            // 保持行数绝对一致：这一行填充为空行
                            resultSb.Append(string.Empty).Append(newline);
                        }
                        else
                        {
                            // 若闭合 $$ 后紧跟其他文字，保留该剩余文字
                            resultSb.Append(remainder).Append(newline);
                        }
                    }
                    else
                    {
                        // 仍未闭合，将整行计入公式内容，当前输出填充为空行以严格保持行号不偏移
                        currentMathContent.AppendLine(line);
                        resultSb.Append(string.Empty).Append(newline);
                    }
                    continue;
                }

                // 3. 当前处于普通文本中，检查本行是否包含 $$
                int ddIdx = FindUnescapedDollarDollarOutsideInlineCode(line, 0);
                if (ddIdx >= 0)
                {
                    string prefix = line.Substring(0, ddIdx);
                    bool isLineStart = string.IsNullOrWhiteSpace(prefix);
                    int nextDdIdx = FindUnescapedDollarDollarOutsideInlineCode(line, ddIdx + 2);

                    if (nextDdIdx >= 0)
                    {
                        string suffix = line.Substring(nextDdIdx + 2);
                        bool isLineEnd = string.IsNullOrWhiteSpace(suffix);

                        if (isLineStart && isLineEnd)
                        {
                            // 独立行单行块级公式 (如 $$E = mc^2$$)
                            int id = items.Count;
                            string mathText = line.Substring(ddIdx + 2, nextDdIdx - ddIdx - 2);
                            items.Add(new MathItem
                            {
                                Id = id,
                                IsBlock = true,
                                Content = mathText
                            });

                            resultSb.Append(prefix)
                                    .Append(BlockTokenPrefix).Append(id).Append(TokenSuffix)
                                    .Append(newline);
                            continue;
                        }
                        else
                        {
                            // 行内包含双美元公式，或者同一行内有多个公式与普通文本混排
                            string processedLine = ProcessInlineDoubleDollarOnSingleLine(line, items);
                            resultSb.Append(processedLine).Append(newline);
                            continue;
                        }
                    }
                    else
                    {
                        // 跨行多行公式的起始行！(例如 $$\begin{aligned} 或 $$f(n) = \begin{cases})
                        int id = items.Count;
                        currentMathId = id;
                        inMathBlock = true;
                        currentMathContent.Clear();

                        string firstLineMath = line.Substring(ddIdx + 2);
                        if (!string.IsNullOrEmpty(firstLineMath))
                        {
                            currentMathContent.AppendLine(firstLineMath);
                        }

                        // 首行输出块级令牌，前置空白保留以保持缩进对齐
                        resultSb.Append(prefix)
                                .Append(BlockTokenPrefix).Append(id).Append(TokenSuffix)
                                .Append(newline);
                        continue;
                    }
                }

                // 普通文本行
                resultSb.Append(line).Append(newline);
            }

            // 4. 文档末尾容错处理：若多行公式未显式闭合，仍然保存现有内容
            if (inMathBlock)
            {
                items.Add(new MathItem
                {
                    Id = currentMathId,
                    IsBlock = true,
                    Content = currentMathContent.ToString()
                });
            }

            // 保持尾部空白结构与源文档一致
            string processed = resultSb.ToString();
            if (!markdown.EndsWith("\n") && !markdown.EndsWith("\r") && (processed.EndsWith("\n") || processed.EndsWith("\r")))
            {
                processed = processed.TrimEnd('\r', '\n');
            }

            return processed;
        }

        /// <summary>
        /// 将 Markdig 转换后 HTML 中的数学令牌无缝还原为符合 KaTeX 渲染规范的标准 HTML 结构
        /// </summary>
        /// <param name="html">Markdig 渲染出的 HTML</param>
        /// <param name="items">预处理阶段收集的数学公式项</param>
        /// <returns>还原后的完整富文本 HTML</returns>
        public static string Postprocess(string html, List<MathItem> items)
        {
            if (string.IsNullOrEmpty(html) || items == null || items.Count == 0)
            {
                return html ?? string.Empty;
            }

            string result = html;

            foreach (var item in items)
            {
                string encoded = WebUtility.HtmlEncode((item.Content ?? string.Empty).Trim());

                if (item.IsBlock)
                {
                    string token = BlockTokenPrefix + item.Id + TokenSuffix;
                    // 正则匹配被 <p ...> 包裹的单行块级令牌，提取行号 id="pragma-line-X" 等现有属性
                    var pRegex = new Regex(@"<p(?<attrs>[^>]*)>\s*" + Regex.Escape(token) + @"\s*</p>", RegexOptions.Singleline);

                    if (pRegex.IsMatch(result))
                    {
                        result = pRegex.Replace(result, m =>
                        {
                            string attrs = m.Groups["attrs"].Value;
                            if (attrs.IndexOf("class=\"", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                attrs = Regex.Replace(attrs, @"class=""([^""]*)""", "class=\"math $1\"");
                            }
                            else
                            {
                                attrs = " class=\"math\"" + attrs;
                            }

                            return "<div" + attrs + ">\n\\[\n" + encoded + "\n\\]\n</div>";
                        });
                    }
                    else
                    {
                        // 兜底：未被独立 <p> 标签包裹的情况 (如紧凑列表项内部)
                        string fallbackBlock = "<div class=\"math\">\n\\[\n" + encoded + "\n\\]\n</div>";
                        result = result.Replace(token, fallbackBlock);
                    }
                }
                else
                {
                    string token = InlineTokenPrefix + item.Id + TokenSuffix;
                    string inlineHtml = "<span class=\"math\">\\(" + encoded + "\\)</span>";
                    result = result.Replace(token, inlineHtml);
                }
            }

            return result;
        }

        /// <summary>
        /// 在单行中扫描并替换所有行内 $$...$$ 公式为内联令牌
        /// </summary>
        private static string ProcessInlineDoubleDollarOnSingleLine(string line, List<MathItem> items)
        {
            var sb = new StringBuilder(line.Length + 64);
            int cursor = 0;

            while (cursor < line.Length)
            {
                int ddIdx = FindUnescapedDollarDollarOutsideInlineCode(line, cursor);
                if (ddIdx < 0)
                {
                    sb.Append(line.Substring(cursor));
                    break;
                }

                sb.Append(line.Substring(cursor, ddIdx - cursor));
                int nextDdIdx = FindUnescapedDollarDollarOutsideInlineCode(line, ddIdx + 2);

                if (nextDdIdx < 0)
                {
                    // 无法匹配闭合 $$，保留剩余字符
                    sb.Append(line.Substring(ddIdx));
                    break;
                }

                int id = items.Count;
                string mathText = line.Substring(ddIdx + 2, nextDdIdx - ddIdx - 2);
                items.Add(new MathItem
                {
                    Id = id,
                    IsBlock = false,
                    Content = mathText
                });

                sb.Append(InlineTokenPrefix).Append(id).Append(TokenSuffix);
                cursor = nextDdIdx + 2;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 在指定字符串中跳过反斜杠转义，定位下一个 $$ 的索引
        /// </summary>
        private static int FindUnescapedDollarDollar(string text, int startIndex)
        {
            if (string.IsNullOrEmpty(text) || startIndex >= text.Length - 1) return -1;

            for (int i = startIndex; i < text.Length - 1; i++)
            {
                if (text[i] == '$' && text[i + 1] == '$')
                {
                    int slashCount = 0;
                    for (int j = i - 1; j >= 0 && text[j] == '\\'; j--)
                    {
                        slashCount++;
                    }

                    if (slashCount % 2 == 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// 在指定单行文本中寻找未处于行内代码 (`...`) 内且未被转义的 $$ 索引
        /// </summary>
        private static int FindUnescapedDollarDollarOutsideInlineCode(string text, int startIndex)
        {
            if (string.IsNullOrEmpty(text) || startIndex >= text.Length - 1) return -1;

            bool inBacktick = false;

            for (int i = startIndex; i < text.Length - 1; i++)
            {
                char c = text[i];
                if (c == '`')
                {
                    // 跳过转义反引号
                    int slashCount = 0;
                    for (int j = i - 1; j >= 0 && text[j] == '\\'; j--) slashCount++;
                    if (slashCount % 2 == 0)
                    {
                        inBacktick = !inBacktick;
                    }
                    continue;
                }

                if (!inBacktick && c == '$' && text[i + 1] == '$')
                {
                    int slashCount = 0;
                    for (int j = i - 1; j >= 0 && text[j] == '\\'; j--) slashCount++;
                    if (slashCount % 2 == 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}
