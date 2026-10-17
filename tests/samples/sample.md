# NppOmniView 极客全功能 Markdown 预览评测套件

> [!NOTE]
> 本文档是专为 **NppOmniView** 打造的深度全特性测试套件，全面覆盖 GFM 语法、全语言专属代码块、KaTeX 数学公式、交互式任务列表、Mermaid 流程图与时序图，以及丰富的文本与表格排版。

> [!TIP]
> **实时双向编辑提示**：
> - 支持双向实时就地编辑：点击顶栏编辑按钮或双击内容区域直接修改，60 FPS 流式渲染，修改即时自动安全保存回写 Notepad++（支持 `Ctrl+Z` 撤销）。
> - 下方的**待办任务列表复选框**支持直接点击打勾/取消，即点即存！

---

## 目录索引 (Table of Contents)

- [1. 格式化排版与呼出警告块 (Typography & Callouts)](#1-格式化排版与呼出警告块-typography--callouts)
- [2. 交互式任务列表 (Interactive Task Lists - 支持双向保存)](#2-交互式任务列表-interactive-task-lists---支持双向保存)
- [3. 多语言全能代码块评测 (Multi-Language Code Blocks)](#3-多语言全能代码块评测-multi-language-code-blocks)
  - [3.1 C# (.NET 4.8 / 现代 C#)](#31-c-net-48--现代-c)
  - [3.2 Java (Spring Boot / 现代并发)](#32-java-spring-boot--现代并发)
  - [3.3 JavaScript (ES2024 / 异步流)](#33-javascript-es2024--异步流)
  - [3.4 TypeScript (类型系统与泛型)](#34-typescript-类型系统与泛型)
  - [3.5 Python (数据分析与现代语法)](#35-python-数据分析与现代语法)
  - [3.6 Go (Golang 并发 Goroutine)](#36-go-golang-并发-goroutine)
  - [3.7 Rust (安全内存与所有权)](#37-rust-安全内存与所有权)
  - [3.8 C++ / C (现代 C++ 模板与智能指针)](#38-c--c-现代-c-模板与智能指针)
  - [3.9 SQL (复杂分析与多表窗口函数)](#39-sql-复杂分析与多表窗口函数)
  - [3.10 Bash / Shell (自动化运维脚本)](#310-bash--shell-自动化运维脚本)
  - [3.11 PowerShell (Cmdlet 与系统管理)](#311-powershell-cmdlet-与系统管理)
  - [3.12 Git Diff (版本对比与增删补丁)](#312-git-diff-版本对比与增删补丁)
  - [3.13 自动化构建与分发脚本 (Packaging & Build)](#313-自动化构建与分发脚本-packaging--build)
  - [3.14 HTML & CSS / SCSS (前端样式)](#314-html--css--scss-前端样式)
  - [3.15 JSON & YAML 配置展示](#315-json--yaml-配置展示)
- [4. KaTeX 数学公式全景覆盖 (Mathematical Formulas)](#4-katex-数学公式全景覆盖-mathematical-formulas)
- [5. GFM 复杂数据对齐表格 (Tables)](#5-gfm-复杂数据对齐表格-tables)
- [6. 嵌入式 Mermaid 架构图 (Embedded Diagrams)](#6-嵌入式-mermaid-架构图-embedded-diagrams)

---

## 1. 格式化排版与呼出警告块 (Typography & Callouts)

### 1.1 字体行内修饰与按键
- **粗体强调**：`**精通 Notepad++ 插件开发**` $\rightarrow$ **精通 Notepad++ 插件开发**
- *斜体强调*：`*轻量而高效的渲染引擎*` $\rightarrow$ *轻量而高效的渲染引擎*
- ***粗斜体***：`***全能实时预览 OmniView***` $\rightarrow$ ***全能实时预览 OmniView***
- ~~删除线~~：`~~过时的旧版 MFC WebBrowser 方案~~` $\rightarrow$ ~~过时的旧版 MFC WebBrowser 方案~~
- 行内代码胶囊（支持悬停高亮与点击快速复制）：`SCI_GETCHARACTERPOINTER`、`WebView2`、`Utf8ToUtf16`
- 键盘按键与标记：请按 <kbd>F11</kbd> 进入全屏沉浸禅模式，按 <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>P</kbd> 打开停靠窗，或双击内容进行就地编辑。
- 上标与下标：化学式 $\text{H}_2\text{O}$，勾股定理 $a^2 + b^2 = c^2$。

### 1.2 GitHub 风格五色呼出警告块 (Alerts / Callouts)

> [!NOTE]
> **NOTE 提示块**：用于说明系统背景信息、技术架构细节或环境依赖。

> [!TIP]
> **TIP 技巧块**：使用快捷键 <kbd>Ctrl</kbd> + <kbd>滚轮</kbd> 可以平滑缩放视图，按顶部操作栏按钮可一键导出高分辨率图片。

> [!IMPORTANT]
> **IMPORTANT 重要通知**：NppOmniView 采用原生双向回写机制，所有的实时编辑均被纳入 Scintilla 撤销堆栈，按 <kbd>Ctrl</kbd> + <kbd>Z</kbd> 即可撤销。

> [!WARNING]
> **WARNING 警示块**：在大批量替换数据时，建议先对重要文件进行版本控制或建立快照。

> [!CAUTION]
> **CAUTION 危险警示**：请勿在运行中的生产系统上执行未经验证的 DROP 或 TRUNCATE 语句！

### 1.3 嵌套多级引用
> 一级引用：极简、现代、跨格式全能视图。
> > 二级嵌套引用：依托 Microsoft Edge WebView2 原生 Chromium 内核渲染，支持硬件加速。
> > > 三级嵌套引用：零网络依赖、纯本地极速离线解析。

---

## 2. 交互式任务列表 (Interactive Task Lists - 支持双向保存)

> 💡 **交互测试**：直接点击下方复选框，观察文档原始文本是否自动在 `- [ ]` 与 `- [x]` 之间切换，并实时无缝同步保存至 Notepad++ 缓冲区！

- [x] 核心多格式智能检测引擎架构 (Markdown, JSON, CSV, SVG, YAML, Mermaid, HTML, Code)
- [x] 现代化玻璃拟态顶栏与深浅色自适应主题
- [x] 代码块 Mac 风格三色圆点、语言徽标与独立对齐行号栏
- [x] 全格式实时双向就地编辑与分屏对照视口编辑器
- [x] CSV / TSV 交互式数据表格与自定义分隔符快捷转换
- [x] JSON / YAML 树状结构节点双击微编辑与自动类型推断
- [x] HTML 响应式视口沙箱（1080p 默认、720p/2K/4K、手机外框与横竖屏）
- [x] SVG 与 Mermaid 统一全彩调色盘与自适应等比缩放
- [ ] 持续探索更多前沿可视化与扩展特性

---

## 3. 多语言全能代码块评测 (Multi-Language Code Blocks)

### 3.1 C# (.NET 4.8 / 现代 C#)
```csharp
using System;
using System.Threading.Tasks;
using NppPlugin.OmniView.Core;

namespace NppPlugin.OmniView.Engine
{
    /// <summary>
    /// 全能文档格式流式调度器
    /// </summary>
    public sealed class OmniDispatcher : IDisposable
    {
        private readonly BufferReader _reader;
        public string ActiveDocumentName { get; private set; }

        public OmniDispatcher(IntPtr scintillaHandle)
        {
            _reader = new BufferReader(scintillaHandle);
        }

        public async Task<FormatResult> ProcessBufferAsync(string rawContent)
        {
            if (string.IsNullOrWhiteSpace(rawContent))
                return FormatResult.Empty;

            var detector = new MultiFormatDetector();
            return await Task.Run(() => detector.Evaluate(rawContent));
        }

        public void Dispose() => _reader?.Dispose();
    }
}
```

### 3.2 Java (Spring Boot / 现代并发)
```java
package com.notepad.plugin.omniview;

import java.util.concurrent.CompletableFuture;
import java.util.List;
import java.util.stream.Collectors;

public record PluginMetric(String format, long renderTimeMs, boolean success) {}

public class PreviewOptimizer {
    public CompletableFuture<List<PluginMetric>> benchmarkAsync(List<String> payloads) {
        return CompletableFuture.supplyAsync(() -> payloads.parallelStream()
            .map(payload -> {
                long start = System.currentTimeMillis();
                boolean ok = payload.length() > 0;
                return new PluginMetric("Markdown", System.currentTimeMillis() - start, ok);
            })
            .collect(Collectors.toList()));
    }
}
```

### 3.3 JavaScript (ES2024 / 异步流)
```javascript
// 现代客户端自适应事件分发总线
class OmniEventBus {
    #subscribers = new Map();

    on(event, handler) {
        if (!this.#subscribers.has(event)) {
            this.#subscribers.set(event, new Set());
        }
        this.#subscribers.get(event).add(handler);
        return () => this.#subscribers.get(event)?.delete(handler);
    }

    async emit(event, payload) {
        const handlers = Array.from(this.#subscribers.get(event) ?? []);
        await Promise.allSettled(handlers.map(fn => fn(payload)));
    }
}

const bus = new OmniEventBus();
bus.on('buffer:update', async ({ line, content }) => {
    console.log(`[OmniView] 实时渲染第 ${line} 行，字节数: ${content.length}`);
});
```

### 3.4 TypeScript (类型系统与泛型)
```typescript
interface DocumentMetadata<TFormat extends string> {
    readonly id: string;
    readonly format: TFormat;
    timestamp: number;
    encoding: 'utf-8' | 'utf-16' | 'gbk';
}

type SupportedFormat = 'markdown' | 'json' | 'csv' | 'svg' | 'yaml' | 'mermaid' | 'html';

export class TypeSafePreviewBridge<T extends SupportedFormat> {
    constructor(private readonly meta: DocumentMetadata<T>) {}

    public dispatchPayload(payload: string): Promise<boolean> {
        console.log(`[Bridge] 分发 ${this.meta.format} 格式载荷 (${payload.length} chars)`);
        return Promise.resolve(true);
    }
}
```

### 3.5 Python (数据分析与现代语法)
```python
import numpy as np
import pandas as pd
from typing import Dict, Any, List

def calculate_render_statistics(latency_samples: List[float]) -> Dict[str, Any]:
    """计算渲染引擎延迟四分位数与统计分布"""
    arr = np.array(latency_samples, dtype=np.float64)
    return {
        "mean_ms": round(float(np.mean(arr)), 2),
        "p50_ms": round(float(np.percentile(arr, 50)), 2),
        "p99_ms": round(float(np.percentile(arr, 99)), 2),
        "std_dev": round(float(np.std(arr)), 3)
    }

if __name__ == "__main__":
    samples = [12.4, 15.1, 8.9, 14.2, 38.6, 11.0, 9.5]
    metrics = calculate_render_statistics(samples)
    print(f"渲染性能指标: {metrics}")
```

### 3.6 Go (Golang 并发 Goroutine)
```go
package main

import (
	"context"
	"fmt"
	"sync"
	"time"
)

type WorkerResult struct {
	ID    int
	Lines int
	Cost  time.Duration
}

func parseWorker(ctx context.Context, id int, jobs <-chan string, results chan<- WorkerResult, wg *sync.WaitGroup) {
	defer wg.Done()
	for {
		select {
		case <-ctx.Done():
			return
		case job, ok := <-jobs:
			if !ok {
				return
			}
			start := time.Now()
			// 模拟快速分词解析
			time.Sleep(10 * time.Millisecond)
			results <- WorkerResult{ID: id, Lines: len(job), Cost: time.Since(start)}
		}
	}
}
```

### 3.7 Rust (安全内存与所有权)
```rust
use std::collections::HashMap;

#[derive(Debug, Clone, PartialEq, Eq)]
pub enum FileKind {
    Markdown,
    Json,
    Csv,
    Yaml,
}

pub struct FormatRegistry {
    extensions: HashMap<String, FileKind>,
}

impl FormatRegistry {
    pub fn new() -> Self {
        let mut extensions = HashMap::new();
        extensions.insert("md".into(), FileKind::Markdown);
        extensions.insert("json".into(), FileKind::Json);
        extensions.insert("csv".into(), FileKind::Csv);
        extensions.insert("yaml".into(), FileKind::Yaml);
        Self { extensions }
    }

    pub fn lookup(&self, ext: &str) -> Option<&FileKind> {
        self.extensions.get(ext)
    }
}
```

### 3.8 C++ / C (现代 C++ 模板与智能指针)
```cpp
#include <iostream>
#include <memory>
#include <string_view>
#include <vector>

template <typename TRenderer>
class ViewportHost {
private:
    std::unique_ptr<TRenderer> renderer_;
public:
    explicit ViewportHost(std::unique_ptr<TRenderer> r) : renderer_(std::move(r)) {}

    void RenderScene(std::string_view content) const {
        if (!content.empty() && renderer_) {
            renderer_->Draw(content);
        }
    }
};
```

### 3.9 SQL (复杂分析与多表窗口函数)
```sql
WITH daily_render_stats AS (
    SELECT 
        DATE(created_at) AS render_date,
        doc_format,
        render_duration_ms,
        ROW_NUMBER() OVER (
            PARTITION BY DATE(created_at), doc_format 
            ORDER BY render_duration_ms DESC
        ) AS rank_in_day
    FROM omni_render_logs
    WHERE created_at >= NOW() - INTERVAL '30 DAYS'
)
SELECT 
    render_date,
    doc_format,
    AVG(render_duration_ms) AS avg_duration,
    MAX(render_duration_ms) AS max_duration
FROM daily_render_stats
WHERE rank_in_day <= 50
GROUP BY render_date, doc_format
ORDER BY render_date DESC, avg_duration ASC;
```

### 3.10 Bash / Shell (自动化运维脚本)
```bash
#!/usr/bin/env bash
set -euo pipefail

DIST_DIR="dist/NppOmniView"
TARGET_DIR="/d/Software/Notepad++/plugins/NppOmniView"

echo "=== 开始同步插件包至 Notepad++ 目录 ==="
if [ ! -d "$DIST_DIR" ]; then
    echo "错误：分发目录不存在，请先执行 pack_for_agent.ps1 打包" >&2
    exit 1
fi

mkdir -p "$TARGET_DIR"
cp -rf "$DIST_DIR"/* "$TARGET_DIR"/
echo "同步完毕！当前安装版本哈希: $(sha256sum "$TARGET_DIR/NppOmniView.dll" | cut -d' ' -f1)"
```

### 3.11 PowerShell (Cmdlet 与系统管理)
```powershell
[CmdletBinding()]
param(
    [string]$PluginPath = "D:\Software\Notepad++\plugins\NppOmniView\NppOmniView.dll"
)

function Test-PluginBinaryIntegrity {
    if (Test-Path $PluginPath) {
        $fileInfo = Get-Item $PluginPath
        $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($PluginPath)
        [PSCustomObject]@{
            FileName     = $fileInfo.Name
            SizeKB       = [Math]::Round($fileInfo.Length / 1KB, 2)
            FileVersion  = $version.FileVersion
            ProductDesc  = $version.FileDescription
            LastModified = $fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
        }
    } else {
        Write-Error "未找到指定的插件动态链接库: $PluginPath"
    }
}

Test-PluginBinaryIntegrity | Format-Table -AutoSize
```

### 3.12 Git Diff (版本对比与增删补丁)
```diff
diff --git a/src/NppOmniView/Core/HtmlTemplateBuilder.cs b/src/NppOmniView/Core/HtmlTemplateBuilder.cs
index a1c4b20..e7f893d 100644
--- a/src/NppOmniView/Core/HtmlTemplateBuilder.cs
+++ b/src/NppOmniView/Core/HtmlTemplateBuilder.cs
@@ -2080,6 +2080,10 @@ namespace NppPlugin.OmniView.Core
-    // 旧版静态展示模式
+    // 新版通用视口就地编辑器与分屏双向联动引擎
+    <div id='omni-viewport-editor' class='omni-viewport-editor split-drawer'>
+        <textarea id='omni-editor-textarea' oninput='onEditorTextareaInput(this.value)'></textarea>
+    </div>
```

### 3.13 自动化构建与分发脚本 (Packaging & Build)
```powershell
# NppOmniView 自动化极速编译、PE导出表注入与部署流水线
param([switch]$DeployToNpp)

$ErrorActionPreference = "Stop"
Write-Host "1. 编译 ExportPatcher 与 NppOmniView.dll..." -ForegroundColor Yellow
dotnet build src\NppOmniView\NppOmniView.csproj -c Release -v minimal

Write-Host "2. 执行 dnlib 注入导出表 (isUnicode, setInfo, beNotified)..." -ForegroundColor Yellow
& src\ExportPatcher\bin\Release\net48\ExportPatcher.exe src\NppOmniView\bin\Release\net48\NppOmniView.dll

Write-Host "3. 运行全套自动化测试验证..." -ForegroundColor Green
& powershell -ExecutionPolicy Bypass -File tests\run_tests.ps1
```

### 3.14 HTML & CSS / SCSS (前端样式)
```html
<div class="omni-status-card glassmorphism">
    <div class="pulse-indicator online"></div>
    <span class="status-label">WebView2 Core Ready</span>
</div>
```

```scss
$primary-color: #3b82f6;
$border-radius: 8px;

.omni-status-card {
    display: inline-flex;
    align-items: center;
    padding: 6px 14px;
    border-radius: $border-radius;
    background: rgba(255, 255, 255, 0.85);
    backdrop-filter: blur(12px);
    border: 1px solid rgba(226, 232, 240, 0.8);

    .pulse-indicator.online {
        width: 8px;
        height: 8px;
        background-color: #10b981;
        border-radius: 50%;
        box-shadow: 0 0 8px rgba(16, 185, 129, 0.6);
    }
}
```

### 3.15 JSON & YAML 配置展示
```json
{
  "plugin": "NppOmniView",
  "version": "2.2.0",
  "autoSaveDebounceMs": 350,
  "hotkeys": {
    "toggleDrawer": "F2",
    "flushSave": "Ctrl+S"
  }
}
```

```yaml
plugin:
  name: NppOmniView
  active: true
  features:
    - realtime_sync: true
    - katex_math: true
    - mermaid_graph: true
```

---

## 4. KaTeX 数学公式全景覆盖 (Mathematical Formulas)

### 4.1 行内公式 (Inline Math)
- 爱因斯坦质能等价方程：$E = mc^2$
- 欧拉恒等式：$e^{i\pi} + 1 = 0$
- 狄拉克常数与普朗克关系：$E = \hbar \omega = h \nu$
- 高斯正态分布密度函数：$f(x) = \frac{1}{\sigma \sqrt{2\pi}} \exp\left( -\frac{(x - \mu)^2}{2\sigma^2} \right)$

### 4.2 高斯微积分与极限 (Calculus & Limits)
$$\int_{-\infty}^{+\infty} e^{-x^2} \, dx = \sqrt{\pi}$$

$$\lim_{x \to 0} \frac{\sin x}{x} = 1, \qquad \lim_{n \to \infty} \left(1 + \frac{1}{n}\right)^n = e$$

$$\sum_{k=1}^{\infty} \frac{1}{k^2} = \frac{\pi^2}{6} \quad (\text{巴塞尔问题})$$

### 4.3 线性代数与矩阵求逆 (Matrices & Linear Algebra)
$$\mathbf{A} = \begin{pmatrix} a_{11} & a_{12} & a_{13} \\ a_{21} & a_{22} & a_{23} \\ a_{31} & a_{32} & a_{33} \end{pmatrix}, \qquad \det(\mathbf{A}) = |\mathbf{A}|$$

二阶方阵求逆通式：
$$\begin{pmatrix} a & b \\ c & d \end{pmatrix}^{-1} = \frac{1}{ad - bc} \begin{pmatrix} d & -b \\ -c & a \end{pmatrix}$$

### 4.4 麦克斯韦电磁学方程组 (Maxwell's Equations - 多行对齐)
$$\begin{aligned}
\nabla \cdot \mathbf{E} &= \frac{\rho}{\varepsilon_0} && \text{(高斯电场定律)} \\
\nabla \cdot \mathbf{B} &= 0 && \text{(高斯磁场定律)} \\
\nabla \times \mathbf{E} &= -\frac{\partial \mathbf{B}}{\partial t} && \text{(法拉第电磁感应定律)} \\
\nabla \times \mathbf{B} &= \mu_0 \mathbf{J} + \mu_0 \varepsilon_0 \frac{\partial \mathbf{E}}{\partial t} && \text{(安培-麦克斯韦定律)}
\end{aligned}$$

### 4.5 分段函数与递推关系 (Piecewise Functions)
$$f(n) = \begin{cases} 
\frac{n}{2}, & \text{当 } n \text{ 为偶数} \\ 
3n + 1, & \text{当 } n \text{ 为奇数 (冰雹猜想/考拉兹假说)} 
\end{cases}$$

---

## 5. GFM 复杂数据对齐表格 (Tables)

| 格式名称 | 语法特征 | 实时微编辑机制 | 状态指示 | 渲染引擎 |
| :--- | :---: | :---: | :---: | ---: |
| **Markdown** | `.md`, `.markdown` | 任务列表 `- [ ]` 即点即存 | `Active` | Markdig + KaTeX |
| **JSON** | `.json` | 节点双击修改值/键名 | `Active` | 交互式折叠树 |
| **CSV / TSV** | `.csv`, `.tsv` | 单元格无感编辑、增删行 | `Active` | 二维高精度网格 |
| **SVG** | `.svg` | 滚轮无损缩放、平移画布 | `Active` | 纯原生矢量沙箱 |
| **YAML** | `.yaml`, `.yml` | 树状大纲、语法校验 | `Active` | js-yaml 大纲视图 |
| **Mermaid** | `.mmd`, `.mermaid` | 独立图表、PNG/SVG 导出 | `Active` | Mermaid.js 10+ |
| **HTML** | `.html`, `.htm` | 响应式三视口（桌面/平板/手机） | `Active` | WebView2 隔离沙箱 |

---

## 6. 嵌入式 Mermaid 架构图 (Embedded Diagrams)

```mermaid
flowchart TD
    subgraph Host[Notepad++ 宿主进程]
        Scintilla[Scintilla 编辑缓冲区]
        OmniBridge[NppOmniView 内存桥接层]
    end

    subgraph Preview[OmniView 高性能预览容器]
        Detector{智能格式探测器}
        Editor[通用视口就地编辑器 / 分屏联动]
        WebView[WebView2 硬件加速核心]
    end

    Scintilla -->|UTF-8 内容推流| OmniBridge
    OmniBridge --> Detector
    Detector -->|Markdown/Code| WebView
    Detector -->|JSON/YAML| WebView
    Detector -->|CSV/TSV| WebView
    Detector -->|SVG/HTML| WebView
    Detector -->|Mermaid| WebView
    WebView -.->|双击修改内容| Editor
    Editor -.->|单次撤销保护事务写回| Scintilla
```
