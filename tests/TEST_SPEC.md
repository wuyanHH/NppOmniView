# NppOmniView 全能视图自动化测试与质量规范体系 (TEST_SPEC.md)

> 本文档详细阐述 NppOmniView 插件全套自动化单元测试、回归测试用例、测试样本集与质量门禁规范，确保每一次版本迭代与功能增强均具备 100% 的回归覆盖率与稳定性。

---

## 1. 测试套件架构与运行机制

### 1.1 自动化测试执行器
- **测试脚本路径**：[`tests/run_tests.ps1`](file:///e:/Projects/plugins/NppOmniView/tests/run_tests.ps1)
- **依赖运行环境**：Windows PowerShell / PowerShell 7+，.NET Framework 4.8 运行时，已编译生成的 `src/NppOmniView/bin/Release/net48/NppOmniView.dll`。
- **一键运行命令**：
  ```powershell
  # 方式 1：独立执行全量单元与回归测试
  powershell -ExecutionPolicy Bypass -File .\tests\run_tests.ps1

  # 方式 2：全自动构建、PE 导出注入、测试并打包分发
  powershell -ExecutionPolicy Bypass -File .\pack_for_agent.ps1
  ```
- **质量门禁机制**：断言失败计数 `testsFailed > 0` 时立即返回非零退出码（Exit Code 1），阻断流水线打包。

---

## 2. 测试样本集 (Test Samples Index)

位于 [`tests/samples/`](file:///e:/Projects/plugins/NppOmniView/tests/samples/) 专属独立目录下，提供各格式真实工业级测试样本文件，专供在 Notepad++ 中进行人工预览验收：

| 样本文件 | 格式类型 | 核心评测要点 |
| :--- | :--- | :--- |
| [`sample.md`](file:///e:/Projects/plugins/NppOmniView/tests/samples/sample.md) | Markdown | 540行极客评测套件：GFM 规范、GitHub 5 色呼出警告块、交互式任务列表复选框、TOC 锚点平滑跳转、KaTeX 公式、全语言代码块 |
| [`sample.json`](file:///e:/Projects/plugins/NppOmniView/tests/samples/sample.json) | JSON | 包含深层嵌套对象、数组、科学计数法浮点数、大整数、UTF-8 中文、转义字符、空值、布尔值与置顶工具栏 |
| [`sample.csv`](file:///e:/Projects/plugins/NppOmniView/tests/samples/sample.csv) | CSV (逗号) | 包含保留前导零工号 (`00101`, `00007`)、字段内逗号、转义双引号 (`""`)、浮点数薪资、中文字符与布尔值 |
| [`sample.tsv`](file:///e:/Projects/plugins/NppOmniView/tests/samples/sample.tsv) | TSV (制表符) | 标准制表符分隔性能监控指标表，用于测试制表符保留与列宽对齐 |
| [`sample.svg`](file:///e:/Projects/plugins/NppOmniView/tests/samples/sample.svg) | SVG | 现代化暗色仪表盘矢量图形，包含渐变、阴影发光滤镜、裁剪蒙版、矢量路径、无损缩放与统一调色盘 |
| [`sample.yaml`](file:///e:/Projects/plugins/NppOmniView/tests/samples/sample.yaml) | YAML | 标量类型（布尔、空值、多进制数字）、字面量多行文本块 (`\|`)、折叠多行块 (`>`)、锚点与别名引用、复杂对象树与注释保留 |
| [`sample.mmd`](file:///e:/Projects/plugins/NppOmniView/tests/samples/sample.mmd) | Mermaid | 完整现代架构流程图，支持窗口自适应等比缩放、页面实时缩放比例标签、节点就地编辑与矢量抗锯齿 |
| [`sample.html`](file:///e:/Projects/plugins/NppOmniView/tests/samples/sample.html) | HTML | 响应式多视口交互式沙箱测试页面（默认 1080p 超清、720p/2K/4K 预设、手机拟真外框边框与横竖屏旋转） |
| [`sample.ps1`](file:///e:/Projects/plugins/NppOmniView/tests/samples/sample.ps1) | 源码代码 (Code) | PowerShell 自动化运维脚本，包含 Cmdlet、哈希表、管道以及内嵌 SVG 标签，验证代码安全高亮与防误判 |

---

## 3. 详细测试分类与条目列表 (Tests 1 ~ 45)

当前自动化套件共计 **45 个大项测试、197 个独立断言**，已全部通过。

### 一、JSON 核心处理与树状视图 (Tests 1, 4, 7, 10, 21, 38, 39)
- **Test 1**: 空 JSON 对象 `{}` 与空数组 `[]` 格式化美化（确保不产生多余换行与空格）。
- **Test 4**: 复杂嵌套 JSON 转 CSV，验证 RFC 4180 双引号转义，防止 C# 类型名称泄漏。
- **Test 7**: JSON 模板构建无未替换占位符、中文真实 UTF-8 编码、类型标签控制开关、置顶单行粘性工具栏 (`json-toolbar`, `position: sticky`)。
- **Test 10**: 树状导向线与对齐占位符 (`json-node-children`, `tree-arrow`, `tree-spacer`)、节点复制动作与行内类型胶囊。
- **Test 21**: JSON 树双击就地编辑 (`startEditJsonVal`, `startEditJsonKey`, `syncJsonOrYamlAfterEdit`)。
- **Test 38**: `MinifyJson` 紧凑压缩（去除多余空格与换行）与 `ConvertJsonToMarkdownTable` 直接转换 Markdown 表格。
- **Test 39**: 容错与异常处理：非合法 JSON 语法时安全捕获并返回原文本与 `errorMsg`，非数组/对象根节点校验。

### 二、CSV / TSV / DSV 多格式网格与分隔符体系 (Tests 2, 3, 5, 8, 22, 26, 27, 30, 31, 43, 44)
- **Test 2**: CSV 转 JSON 时的前导零字符串保留（如工号 `"0123"`, `"007"`）与跨区域浮点数不变量解析（`98.5`）。
- **Test 3**: 多行 CSV 单元格转 Markdown 表格换行符转 `<br>`。
- **Test 5**: TSV 制表符分隔符保留与对齐。
- **Test 8**: `AlignCsvColumns` 列宽等宽对齐幂等性（多次点击不发生宽度漂移）与 `MinifyCsv` 紧凑清洗。
- **Test 22**: 表格双击就地编辑、添加行 (`addCsvRow`)、删除行 (`deleteCsvRow`)、键盘光标导航与自动序列化写回。
- **Test 26**: 分隔符互转：逗号转 TSV 制表符、逗号转竖线 `|`（自动处理引号）、逗号转分号 `;`、同分隔符幂等性。
- **Test 27**: 分隔符替换模态弹窗 UI、预设卡片、工具栏按钮与下拉菜单动作绑定。
- **Test 30**: 自定义分隔符自动探测与转换（脱字号 `^`、冒号 `:` 等）。
- **Test 31**: 序号列稳定性（采用 `table-cell` 布局消除 Flexbox 行抖动）、现代分隔符单选卡片、动态 `csv-delim-pill` 标签。
- **Test 43**: CSV 顶栏粘性置顶与自适应滚动容器样式 (`csv-toolbar`, `position: sticky`, `flex: 1`)。
- **Test 44**: 真实样本文件校验：读取 `sample_pipe.dsv` 自动识别 `|` 并转标准逗号 CSV，读取 `sample_semicolon.csv` 识别 `;` 分号。

### 三、Markdown 排版、GitHub 警告块与平滑导航 (Tests 6, 7, 11, 12, 18, 20, 23, 25, 28, 29)
- **Test 6**: 内容驱动的零分配 Markdown 格式探测。
- **Test 7**: 代码块与行内代码胶囊点击快速复制机制。
- **Test 11**: 采用专业现代 SVG 图标，全面肃清 UI 模板中的系统 Emoji 字符。
- **Test 12**: 多语言代码块高亮、Mac 风格三色圆点、行号侧边栏、终端提示符与 Highlight.js 引擎。
- **Test 18**: 7 大格式通用一键视图切换菜单 (`FORCE_MD`, `FORCE_JSON`, `FORCE_CSV`, `FORCE_SVG`, `FORCE_YAML`, `FORCE_MERMAID`, `FORCE_HTML`, `FORCE_CODE`)。
- **Test 20**: 原生视口内源码就地编辑引擎 (`omni-viewport-editor`, `omni-editor-textarea`, 撤销堆栈保护)。
- **Test 23**: 交互式任务列表复选框打勾即写回 (`enhanceTaskLists`, `toggleMarkdownTaskItem`)。
- **Test 25**: 格式专属滚动触底空间优化 (`fmt-markdown`, `fmt-svg`, `fmt-mermaid`, `fmt-html`)。
- **Test 28**: GitHub 5 色呼出警告块样式与 Markdig 扩展增强器（NOTE, TIP, IMPORTANT, WARNING, CAUTION）。
- **Test 29**: TOC 目录锚点自动标识符生成（完全对齐 GitHub 规范）、`scroll-margin-top` 防止顶栏遮挡、波纹脉冲高亮与平滑跳转。

### 四、SVG 矢量图形与调色盘体系 (Tests 13, 14, 15, 39, 42)
- **Test 13**: SVG 内容自动嗅探（识别 `<svg>` 与 `<?xml` 复合标签）。
- **Test 14**: XML 标准 Pretty-Print 美化与缩进格式化 (`FormatSvg`)。
- **Test 15**: 矢量画布平移缩放、2x 超清 PNG 导出、Data URI 复制与状态徽标。
- **Test 39**: 破损 XML/SVG 容错机制：异常解析时返回原内容并抛出明细。
- **Test 42**: 统一调色盘体系：透明无色、纯白、深色预设与 Hex 颜色拾取器，`localStorage` 独立记忆持久化。

### 五、YAML 结构化大纲与配置处理 (Tests 13, 16, 21)
- **Test 13**: YAML 规范内容探测（识别键值对、列表结构）。
- **Test 16**: js-yaml 引擎集成、2 格标准缩进美化、格式徽标与 YAML 树转 JSON 复制。
- **Test 21**: YAML 树节点行内快速编辑与反向同步。

### 六、Mermaid 架构图与自适应视口 (Tests 13, 17, 24, 32, 41, 42)
- **Test 13**: 流程图关键词识别 (`flowchart`, `graph`)。
- **Test 17**: 独立 Mermaid 渲染视口、矢量 SVG 复制与 2x 导出。
- **Test 24**: 矢量高清归一化 (`normalizeMermaidSvg`) 与调色盘无缝集成。
- **Test 32**: 窗口等比自适应缩放 (`calcMermaidInitialScale`)、顶部居中对齐、页面专用实时缩放比例标签 (`mermaid-zoom-tag`)、彻底移除冗余刷新按钮。
- **Test 41**: 覆盖全部 Mermaid 图表类型识别（时序图 `sequenceDiagram`、类图 `classDiagram`、状态图 `stateDiagram-v2`、实体关系图 `erDiagram`、饼图 `pie`、Git 版本图 `gitGraph`）。
- **Test 42**: 与 SVG 统一共享 `setCanvasBg` 体系与背景预设。

### 七、HTML 多分辨率引擎与手机模式 (Tests 13, 17, 36, 43)
- **Test 13**: 网页 HTML 标签探测。
- **Test 17**: 视口沙箱与基础 iframe 容器。
- **Test 36**: 
  - **默认 1080p 分辨率**：`1920 × 1080` 高保真视口。
  - **标准分辨率档位**：720p, 1080p, 2K, 4K, 桌面全宽 (100%)。
  - **手机移动端与真机外框**：`390 × 844`，附加拟真手机物理边框阴影 (`is-mobile`)。
  - **横竖屏一键切换**：`toggleHtmlOrientation` 即时互换宽与高。
  - **等比缩放适应引擎**：`toggleHtmlAutoFit`、CSS 负外边距消除空白算法。
  - **自定义分辨率模态弹窗**：`html-custom-res-modal`，支持自由像素输入与快捷预设胶囊。
- **Test 43**: HTML 视口副工具栏与分辨率标签置顶粘性显示。

### 八、源码文档模式与智能休眠策略 (.ps1 / 纯代码 DocFormat.Code) (Tests 33, 34, 35, 40, 44)
- **Test 33**: 后缀探测：支持 `.ps1`, `.py`, `.cs`, `.js` 等常见编程语言后缀精准嗅探，自动进入免渲染休眠保护。
- **Test 34**: PowerShell 伪标题规避：未保存的无后缀临时文档中包含 `# Heading` 注释及 PowerShell 代码构件时，准确判定为代码文档，防止误触发 Markdown。
- **Test 35**: 智能休眠与安全防护：代码概览卡片呈现，杜绝侧边栏无意义 DOM 镜像消耗；代码中的 `<svg>`、`<script>` 等标签均被严格 HTML 转义，杜绝 DOM 注入与执行；专属 `CODE: <LANG>` 徽标与复制代码控制器。
- **Test 40**: 全面覆盖主流编程语言后缀映射映射（`.sh`, `.py`, `.js`, `.ts`, `.rs`, `.go`, `.sql`, `.xml`, `.bat` 等）。
- **Test 44**: 验证 `sample.ps1` 真实文件解析与语言识别。

### 九、禅模式全屏展示与雷达动效引擎 (Tests 19, 37)
- **Test 19**: 顶层流光进度条 (`top-loader`)、微呼吸状态点 (`pulse-dot`)、0ms 击键即时反馈 (`NotifyTypingPending`)。
- **Test 37**: 
  - 覆盖所有文档类型的全屏展演按钮 (`btn-zen-fullscreen`)。
  - 悬浮一键退出胶囊 (`zen-exit-pill`) 与自动隐藏顶栏。
  - 快捷键绑定（F11 与 Alt+Enter）。
  - 拟真毛玻璃雷达 HUD 光环 (`recognition-hud`, `radar-ring`)、激光流光与弹簧徽标动画。

### 十、全格式双向就地预览编辑与实时同步体系 (Tests 20, 21, 22, 45)
- **Test 20**: 视口内原地源码编辑引擎 (`omni-viewport-editor`, `omni-editor-textarea`, 撤销堆栈保护)。
- **Test 21**: JSON 与 YAML 树状视图双击键值就地行内编辑 (`json-inline-input`, `data-path`, `syncJsonOrYamlAfterEdit`)。
- **Test 22**: CSV 电子表格双击单元格就地编辑、添加行、删除行、键盘导航与自动同步写回。
- **Test 45**: 全格式双向就地编辑全景覆盖 (P0 & P1 Parity)：
  - **HTML 视口**：`designMode = 'on'` 原生所见即所得 (WYSIWYG) 实时编辑。
  - **SVG 矢量画布**：`initSvgInPlaceEditing` 画布文本原地双击编辑与矢量形状尺寸颜色检视器 (`svg-shape-inspector`)。
  - **Code 源码高亮视口**：`contenteditable` 语法着色代码块实时就地编辑。
  - **Markdown 富文本文档**：`initMarkdownInPlaceEditing` 块级双击原地编辑浮层 (`md-block-editing`, `md-block-textarea`)。
  - **Mermaid 架构流程图**：`initMermaidInPlaceEditing` 节点级双击原地文本编辑弹窗 (`mermaid-node-editor`)。
  - **分屏实时对照抽屉**：`split-drawer` 视口左侧源码/右侧实时预览对照视图 (`toggleEditorSplitLayout`)。

---

## 4. 历史用户反馈需求与回归测试映射表

| 用户反馈 / 历史问题 | 影响模块 | 解决实施方案 | 对应的回归自动化测试项 |
| :--- | :--- | :--- | :--- |
| **MMD 展示模糊、背景切换需支持调色盘与预设** | Mermaid | 引入原生矢量归一化，重构调色盘弹层，支持透明、纯白及 Hex 颜色 | **Test 24, Test 42** |
| **CSV 列表前面编号列抖动；替换分隔符选中状态与自定义保存异常** | CSV | 采用 table-cell 序号列消除 Flexbox 抖动；引入分隔符单选卡片与记忆状态 | **Test 27, Test 31** |
| **替换分隔符按钮在工具栏未完整显示** | CSV 界面 | 优化工具栏 Flex 弹性伸缩、防溢出包裹与视口空间分配 | **Test 27, Test 43** |
| **.ps1 格式中的代码与 SVG 标签显示异常** | Code / PS1 | 引入通用 Code Viewer，防止 `#` 注释被误当 Markdown，转义内嵌 SVG | **Test 33, 34, 35, 40, 44** |
| **YAML 美化格式导致注释丢失机制阐明与规范** | YAML | 规范 YAML 原生保留机制，在模板内直接内嵌原文本确保注释不丢失 | **Test 16, 21** |
| **MMD 默认大小适应窗口、去掉刷新按钮、显示缩放比例** | Mermaid | 实现 `calcMermaidInitialScale` 窗口自适应，页面直接放置 `mermaid-zoom-tag` | **Test 32** |
| **SVG 的背景与 MMD 统一共用一套调色盘逻辑** | SVG / MMD | 抽离统一的 `setCanvasBg` 体系，SVG 与 Mermaid 共享色板结构与交互 | **Test 15, 24, 42** |
| **JSON 预览页面顶部的工具栏要置顶展示** | JSON | 添加 `.view-toolbar.json-toolbar` 粘性置顶 CSS (`position: sticky; top: 42px`) | **Test 7, Test 43** |
| **CSV 预览页面顶部的工具栏也得置顶展示** | CSV | 添加 `.view-toolbar.csv-toolbar` 粘性置顶与容器独立滚动 | **Test 25, Test 43** |
| **HTML 优化分辨率功能（默认1080p, 720p/2K/4K/全宽/自定义/手机/横竖屏）** | HTML | 研发响应式视口引擎，默认 1080p，真机外框，横竖屏互换，等比缩放消除空白 | **Test 36, Test 43** |
| **全格式双向就地预览编辑与实时同步（WYSIWYG、分屏抽屉、节点编辑）** | 全部视图 | 构建全格式统一就地编辑体系，支持 HTML WYSIWYG、SVG/MMD 节点编辑与分屏对照 | **Test 20, 21, 22, 45** |

---

## 5. 持续集成与发布检查清单 (Checklist)

在执行发版或提交前，请严格遵照以下步骤验证：
- [x] 执行 `powershell -ExecutionPolicy Bypass -File .\tests\run_tests.ps1`，确保全部 **197 项断言 100% 通过 (0 Failed)**。
- [x] 执行 `powershell -ExecutionPolicy Bypass -File .\pack_for_agent.ps1`，验证编译无警告错误、PE 导出注入成功、测试通过并成功生成 `dist/NppOmniView` 产物。
- [x] 验证 `tests/samples/` 目录下的全量标准测试样本文件（`sample.md`, `sample.json`, `sample.csv`, `sample.tsv`, `sample.svg`, `sample.yaml`, `sample.mmd`, `sample.html`, `sample.xml`, `sample.ps1`, `sample.py`, `sample.cs`, `sample.sql`, `sample.sh`, `sample_class.mmd`, `sample_sequence.mmd`, `sample_pipe.csv`, `sample_semicolon.csv`）格式规范、语法有效。
