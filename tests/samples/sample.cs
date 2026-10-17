// ==============================================================================
// NppOmniView C# 源码高亮与代码编辑测试套件
// 涵盖泛型接口、LINQ 流式表达式、模式匹配、Record 记录类型与异步 Task 模型
// ==============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NppOmniView.Testing.Samples
{
    /// <summary>
    /// 支持的文档格式枚举
    /// </summary>
    public enum SupportedFormat
    {
        Unknown,
        Markdown,
        Json,
        Csv,
        Svg,
        Yaml,
        Mermaid,
        Html,
        Code
    }

    /// <summary>
    /// 文档上下文不可变数据记录
    /// </summary>
    public record DocumentSnapshot(
        int DocumentId,
        string FileName,
        SupportedFormat Format,
        long ContentLength,
        DateTime LastModifiedUtc
    );

    /// <summary>
    /// 诊断统计报表模型
    /// </summary>
    public class DiagnosticSummary
    {
        public int TotalProcessed { get; set; }
        public int TotalSuccess { get; set; }
        public double AverageLatencyMs { get; set; }
        public Dictionary<SupportedFormat, int> FormatCounts { get; } = new();
    }

    /// <summary>
    /// 文档高亮与处理引擎接口规范
    /// </summary>
    public interface IDocumentProcessor<TResult>
    {
        Task<TResult> ProcessAsync(DocumentSnapshot snapshot, CancellationToken ct = default);
        bool CanHandle(SupportedFormat format);
    }

    /// <summary>
    /// 核心测试服务实现类
    /// </summary>
    public class OmniViewBenchmarkRunner : IDocumentProcessor<DiagnosticSummary>
    {
        private static readonly Regex SafeIdentifierRegex = new(@"^[a-zA-Z0-9_\-\.]+$", RegexOptions.Compiled);

        public bool CanHandle(SupportedFormat format) => format != SupportedFormat.Unknown;

        public async Task<DiagnosticSummary> ProcessAsync(DocumentSnapshot snapshot, CancellationToken ct = default)
        {
            if (snapshot is null)
                throw new ArgumentNullException(nameof(snapshot));

            // 模拟异步工作负荷
            await Task.Delay(10, ct).ConfigureAwait(false);

            var summary = new DiagnosticSummary
            {
                TotalProcessed = 1,
                TotalSuccess = 1,
                AverageLatencyMs = 12.5
            };

            summary.FormatCounts[snapshot.Format] = 1;
            return summary;
        }

        public static IEnumerable<DocumentSnapshot> GenerateSampleSnapshots()
        {
            var formats = new[] { SupportedFormat.Markdown, SupportedFormat.Json, SupportedFormat.Csv, SupportedFormat.Code };
            
            return formats.Select((fmt, idx) => new DocumentSnapshot(
                DocumentId: 1000 + idx,
                FileName: $"sample_{fmt.ToString().ToLowerInvariant()}.dat",
                Format: fmt,
                ContentLength: (idx + 1) * 1024,
                LastModifiedUtc: DateTime.UtcNow
            ));
        }

        public static void Main(string[] args)
        {
            var snapshots = GenerateSampleSnapshots().ToList();
            Console.WriteLine($"[Benchmark] 已加载 {snapshots.Count} 个测试文档快照。");

            foreach (var item in snapshots.Where(s => s.ContentLength > 1000))
            {
                string status = item switch
                {
                    { Format: SupportedFormat.Markdown } => "富文本解析模式",
                    { Format: SupportedFormat.Json }     => "DOM 语法树模式",
                    { Format: SupportedFormat.Csv }      => "表格二维网格模式",
                    _                                    => "源码代码查看模式"
                };

                Console.WriteLine($" -> {item.FileName} ({item.ContentLength} bytes): {status}");
            }
        }
    }
}
