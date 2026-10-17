# 更新日志 (Changelog)

本项目遵循 [Semantic Versioning (语义化版本 2.0.0)](https://semver.org/lang/zh-CN/) 规范。

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
