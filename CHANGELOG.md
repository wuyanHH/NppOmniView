# 更新日志 (Changelog)

本项目遵循 [Semantic Versioning (语义化版本 2.0.0)](https://semver.org/lang/zh-CN/) 规范。

---

## [1.1.0] - 2026-10-20

### 🚀 脚本智能分析与可视化架构全面跃升 (Script Intelligence & Outline Enhancements)

本次发布带来了全方位的新格式与多脚本语言语法分析矩阵、Markdown 数学公式深度增强、双向平滑行对齐同步及更高等级的工业级工程防御与测试覆盖。

#### 1. 新增脚本语言结构大纲与智能工作台 (Script Outline Analyzers)
- **Python 大纲分析 (`PythonScriptAnalyzer`)**：
  - 深度解析 Python 脚本中的顶层类、派生类、成员方法与独立函数；
  - 自动提取类继承关系与函数签名，侧边栏集成符号列表，支持一键点击定位到代码行。
- **SQL 架构分析 (`SqlScriptAnalyzer`)**：
  - 智能嗅探检测 MySQL、PostgreSQL、SQL Server、Oracle 等常用方言；
  - 提取数据表定义 (`CREATE TABLE`)、视图与 DDL/DML 结构，支持关键字实时检索与表级跳转。
- **Shell / Bash 分析 (`ShellScriptAnalyzer`)**：
  - 精准提取 Shebang (`#!/bin/bash` 等) 与脚本执行模式；
  - 检测 `set -euo pipefail` 严格防御模式，提取函数定义与核心脚本流程大纲。
- **Batch / CMD 脚本分析 (`BatchScriptAnalyzer`)**：
  - 提取批处理脚本中的所有跳转标签 (`:label`) 与行内说明；
  - 识别 `@echo off`、`chcp 65001`、`setlocal enabledelayedexpansion` 等环境初始化指令；
  - 内置关键危险指令（如 `del /f /q`）安全提示预警。
- **INI 配置文件网格矩阵 (`IniConfigAnalyzer`)**：
  - 解析配置节 (`[Section]`) 与键值对，提供可视化分节表格视图与实时检索过滤。

#### 2. Markdown 数学公式 (KaTeX) 高保真深度解析
- **公式块与行内公式全景支持**：完美支持多行方程对齐、分段函数、矩阵运算等复杂 LaTeX 数学公式块；
- **代码块语法隔离**：在包含公式符号（如 `$`）的代码块 (` ``` `) 内进行严密隔离，坚决避免代码被误转义破坏；
- **动态离线注入与 Hook**：提供 `window.renderMathBlocks` 与 KaTeX 页面挂载生命周期钩子，确保大文档加载时公式渲染零错乱。

#### 3. 双向高精度行对齐平滑同步 (Bi-Directional Precise Scroll Sync)
- **Scintilla 物理行号锚点**：自动基于行注入 `pragma-line` 物理行号锚点并进行视口平滑插值，彻底解决长图文预览与编辑器行错位的问题；
- **反向同步通道 (`SYNC_SCROLL_TO_NPP`)**：在网页预览区平滑滚动时，可驱动 Notepad++ 编辑器窗口进行精准视口协同；
- **原生行跳转增强**：`BufferReader` 提供 `ScrollToDocumentLine` 接口直接调用 Scintilla 原生消息，实现毫秒级平滑定位。

#### 4. XML 交互矩阵三模合一与沙箱防御
- **三模态工作台**：支持交互式可折叠 DOM 树 (Tree)、实体数据网格 (Grid)、XML 转 JSON (JSON) 三种视图自由无缝切换；
- **实时 XPath 面包屑**：悬浮展示当前层级的完整 XPath 路径并支持一键单击复制；
- **WebView2 安全沙箱防护**：注入 `SafeStorage` 代理，彻底消除在特殊受限环境下由于 `localStorage` 访问限制引发的 `SecurityError`；
- **就地编辑遮罩保护**：增加带防抖时间锁的时序守卫，防止快速点击导致就地编辑器意外收起或状态竞争。

#### 5. 质量工程与回归测试
- **自动化测试断言扩充**：测试套件新增对各脚本分析器、KaTeX 公式保护、双向滚动协议、INI 解析等全项覆盖，回归测试断言从 197 项提升至 **252 项**，100% 通过验证。

---

## [1.0.0] - 2026-10-17

### 🎉 首次正式开源发布 (Initial Official Open-Source Release)

**NppOmniView (全能视图)** 是专为 **Notepad++ (x64)** 打造的工业级、多格式实时渲染预览与双向就地编辑插件。底层深度集成 **Microsoft Edge WebView2 (Chromium)** 高性能渲染引擎，辅以纯 C# 原生 Scintilla 内存桥接与 PE 导出表注入，实现 **0ms 击键即时反馈、双向无感撤销流、全格式所见即所得 (WYSIWYG) 与暗黑模式零闪烁融合**。

### ✨ 核心特性全景 (Core Features)

#### 1. 多格式实时渲染矩阵
- **Markdown**：深度支持 GitHub Flavored Markdown (GFM)、GitHub 5 色警告呼出块 (`NOTE`, `TIP`, `IMPORTANT`, `WARNING`, `CAUTION`)、KaTeX 高保真数学公式渲染、Mermaid 原生图表以及平滑目录大纲导航。
- **JSON**：交互式可折叠语法树视图、数据类型胶囊标识 (`str`, `num`, `bool`, `arr`, `obj`)、键值双击就地编辑、单行粘性悬浮顶栏与语法校验。
- **CSV / TSV / DSV**：现代化电子表格网格、多分隔符智能自适应嗅探（逗号、制表符、分号、竖线等）、单元格双击行内就地修改、增删行与列等宽对齐。
- **SVG 矢量图**：无限视口平移与缩放 (Pan & Zoom)、统一调色盘背景切换、矢量形状检视器、复制 Data URI 与 2x 超清 PNG 导出。
- **YAML**：结构化层级配置树、数据类型胶囊、js-yaml 语法校验与规范 2 空格美化。
- **Mermaid 架构图**：独立 `.mmd` 架构图与时序图支持，1:1 抗锯齿矢量渲染、自适应视口、专用缩放徽标、调色盘与 2x PNG 导出。
- **HTML 视口**：1080p 超清默认沙箱、720p/2K/4K/全宽与自定义分辨率预设，真实移动端手机边框模拟与横竖屏一键旋转。
- **XML 交互树**：DOM 节点可折叠树形大纲、属性胶囊标识、节点与文本双击就地编辑、XPath 复制与标准规范格式化美化。
- **纯文本与源码智能休眠 (Zero-Overhead Policy)**：Notepad++ 原生编辑器已内置工业级极速代码语法高亮，全能视图坚决摒弃在侧边栏重复构建庞大 DOM 代码高亮镜像的做法，自动进入轻量休眠模式，实现零额外 CPU/内存开销；对于 PowerShell 专属提供 AST 结构大纲与帮助文档抽屉。

#### 2. 流体交互与视觉反馈 UX
- **0ms 击键即时反馈**：基于 Scintilla `SCN_MODIFIED`，在 280ms 防抖倒计时期间即时触发拟真毛玻璃雷达 HUD 光环与顶部流光进度条。
- **暗黑模式零白闪**：动态读取 Notepad++ 主题并实时同步 `WebView2.DefaultBackgroundColor`（深色 `#1e1e1e` 与浅色 `#ffffff`），彻底消除停靠面板伸缩及标签切换时的白屏闪烁。
- **双向就地编辑 (Bi-Directional In-Place Editing)**：预览窗口内双击即可就地改写，变更操作全量包装于 Notepad++ 原生 `SCI_BEGINUNDOACTION` / `SCI_ENDUNDOACTION` 事务堆栈，支持单步干净 `Ctrl+Z` 撤销。
- **全屏沉浸模式 (Zen Fullscreen)**：支持 `F11` / `Alt+Enter` 快速沉浸全屏，悬浮退出演示胶囊与自适应顶栏吸顶。

#### 3. 跨格式清洗与一键转换
- **JSON**：2 空格美化、单行压缩 (Minify)、一键转 CSV、一键转 Markdown 表格。
- **CSV**：列宽对齐、紧凑压缩、任意分隔符一键替换互转、一键转 JSON / Markdown 表格。
- **SVG**：XML 规范格式化、Data URI 提取、超清图片导出。
- **YAML**：规范美化、一键转 JSON / CSV。
- **Markdown**：一键导出为纯净 HTML 源码。

#### 4. 工程质量与自动化验证
- **自动化测试门禁**：内置 `tests/run_tests.ps1` 回归测试套件，全面覆盖 45 个核心大项、197 个断言，强制 100% 通过验证。
- **PE 导出表补丁**：配套开发基于 `dnlib` 的 `ExportPatcher.exe`，实现无损 x64 PE 导出表注入，完全符合 Notepad++ 官方二进制扩展协议。
