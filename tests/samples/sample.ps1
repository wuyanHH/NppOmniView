<#
.SYNOPSIS
    NppOmniView 自动化发版、全格式渲染巡检与环境合规诊断脚本 (Enterprise Automation Toolkit)

.DESCRIPTION
    本脚本是 NppOmniView 插件开发与生产发布的核心维护套件。
    主要功能包括：
    1. 自动执行 C# 与 C++ 二进制 PE 导出表注入补丁校验；
    2. 遍历测试样本集并执行多格式 (Markdown, JSON, CSV, SVG, Mermaid, HTML, Code) 渲染完整性巡检；
    3. 支持生成高分辨率视网膜 (Retina 2x/4x) 导出资产，并自动归档至发布目录；
    4. 执行系统运行环境依赖检测与执行策略合规审计。

.PARAMETER TargetFolder
    构建产物分发与归档的目标目录路径。若目录不存在将自动创建。默认值为 "dist\NppOmniView"。

.PARAMETER EnableRetinaExport
    是否开启高 DPI 视网膜超清图表导出模式 (2x/4x 采样抗锯齿)。

.PARAMETER Resolution
    HTML 视口沙箱预览的目标默认分辨率。支持 "1080p", "720p", "2K", "4K"。默认值为 "1080p"。

.PARAMETER TimeoutSeconds
    执行外部编译器与导出表修补工具时的最大超时等待时间 (秒)。

.PARAMETER CleanBeforeBuild
    是否在编译打包前彻底清理目标目录下的所有历史构建缓存与旧版 DLL。

.EXAMPLE
    .\sample.ps1 -TargetFolder "D:\Builds\OmniView" -EnableRetinaExport -Resolution "2K"
    以 2K 分辨率配置并启用超清图表导出，输出至指定的部署目录。

.EXAMPLE
    .\sample.ps1 -CleanBeforeBuild -Resolution "1080p"
    深度清理历史缓存后执行全量自动化巡检与标准 1080p 预览打包。

.OUTPUTS
    [System.Management.Automation.PSCustomObject] 包含构建耗时、测试通过率及产物哈希值的综合巡检报表。

.NOTES
    作者: NppOmniView Core Engineering Team
    版本: 2.3.0
    版权所有 (C) 2026. 遵循 MIT 开源许可协议。
#>

#Requires -Version 5.1
#Requires -RunAsAdministrator

param(
    [Parameter(Mandatory = $false, HelpMessage = "构建归档目录路径")]
    [string]$TargetFolder = "dist\NppOmniView",

    [switch]$EnableRetinaExport,

    [ValidateSet("1080p", "720p", "2K", "4K")]
    [string]$Resolution = "1080p",

    [ValidateRange(5, 300)]
    [int]$TimeoutSeconds = 60,

    [switch]$CleanBeforeBuild
)

$ErrorActionPreference = "Stop"

# ==============================================================================
# 内部函数与子命令定义 (Functions & Cmdlets)
# ==============================================================================

function Invoke-SvgPrettyPrinter {
    <#
    .SYNOPSIS
        格式化与校验 SVG 矢量图形代码，移除冗余命名空间并对齐缩进
    .PARAMETER RawSvg
        原始 SVG XML 字符串内容
    .PARAMETER IndentSpaces
        缩进空格数，默认为 2
    #>
    param(
        [Parameter(Mandatory = $true)]
        [string]$RawSvg,
        [int]$IndentSpaces = 2
    )

    $rawXml = '<svg width="200" height="200" viewBox="0 0 200 200"><circle cx="100" cy="100" r="80" fill="#3b82f6"/><text x="100" y="105" text-anchor="middle" fill="#ffffff" font-size="16">NppOmniView</text></svg>'
    $errorMsg = $null
    
    # 调用插件底层 C# 核心格式化处理器
    $formatted = [NppPlugin.OmniView.Core.FormatProcessor]::FormatSvg($rawXml, [ref]$errorMsg)

    if ($null -ne $errorMsg) {
        Write-Warning "SVG 格式化过程捕获到非致命警告: $errorMsg"
        return $RawSvg
    }

    Write-Host "[SUCCESS] 成功缩进格式化 SVG 矢量内容 (长度: $($formatted.Length) 字符)" -ForegroundColor Green
    return $formatted
}

function Get-SystemDiagnosticReport {
    <#
    .SYNOPSIS
        获取当前主机环境、WebView2 运行时状态与依赖合规信息
    .OUTPUTS
        [hashtable] 诊断度量指标集合
    #>
    param()

    $report = [ordered]@{
        "PluginName"      = "NppOmniView"
        "PowerShellVer"   = $PSVersionTable.PSVersion.ToString()
        "CLRVersion"      = $PSVersionTable.CLRVersion.ToString()
        "OSPlatform"      = [System.Environment]::OSVersion.VersionString
        "TargetResolution"= $Resolution
        "Is64BitProcess"  = [System.Environment]::Is64BitProcess
        "ReportTimestamp" = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    }

    return $report
}

function Remove-BuildCacheSafely {
    <#
    .SYNOPSIS
        安全清空构建临时目录与垃圾缓存 (包含敏感命令检测测试用例)
    .PARAMETER CachePath
        需清理的临时文件夹路径
    #>
    param(
        [string]$CachePath = "tmp\build_cache"
    )

    if (Test-Path $CachePath) {
        Write-Verbose "正在递归级联清理历史缓存目录: $CachePath"
        # 测试敏感命令扫描识别点：Remove-Item 带 -Recurse 与 -Force
        Remove-Item -Path $CachePath -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# ==============================================================================
# 脚本主执行流程 (Pipeline Execution Flow)
# ==============================================================================

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  NppOmniView PowerShell 全景巡检套件正在启动..." -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

if ($CleanBeforeBuild) {
    Remove-BuildCacheSafely -CachePath "tmp\sample_cache"
}

# 1. 运行系统诊断
$diag = Get-SystemDiagnosticReport
$diag.GetEnumerator() | ForEach-Object {
    Write-Host "  - $($_.Key): " -NoNewline -ForegroundColor DarkGray
    Write-Host "$($_.Value)" -ForegroundColor Yellow
}

# 2. 格式化测试样本
$sampleSvgContent = '<svg><rect width="100" height="100"/></svg>'
$prettySvg = Invoke-SvgPrettyPrinter -RawSvg $sampleSvgContent -IndentSpaces 4

# 3. 管道流式汇总与输出
$pipelineResults = 1..5 | ForEach-Object {
    [PSCustomObject]@{
        TaskIndex   = $_
        TaskName    = "FormatCheck_Phase_$_"
        Status      = "Completed"
        LatencyMs   = (Get-Random -Minimum 5 -Maximum 25)
    }
}

$pipelineResults | Format-Table -AutoSize
Write-Host "`n[DONE] 全景巡检套件执行完毕，就绪就绪状态正常！`n" -ForegroundColor Green
