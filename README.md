# NppOmniView (全能视图) - 现代全格式实时预览与双向就地编辑 Notepad++ 插件

[![Release](https://img.shields.io/badge/Release-v1.1.0-blue.svg)](CHANGELOG.md)
[![License](https://img.shields.io/badge/License-Apache%202.0-green.svg)](LICENSE)
[![Notepad++](https://img.shields.io/badge/Notepad%2B%2B-v8.x%20(x64)-brightgreen.svg)](https://notepad-plus-plus.org/)
[![Platform](https://img.shields.io/badge/Platform-Windows%20x64-blue.svg)](https://github.com/wuyanHH/NppOmniView)
[![Runtime](https://img.shields.io/badge/.NET%20Framework-4.8-purple.svg)](https://dotnet.microsoft.com/)
[![Engine](https://img.shields.io/badge/Engine-Microsoft%20Edge%20WebView2-blue.svg)](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)
[![Tests](https://img.shields.io/badge/Tests-252%2F252%20Passing-success.svg)](tests/TEST_SPEC.md)

**NppOmniView** 是专为 **Notepad++ (x64)** 打造的工业级、多格式实时渲染预览与双向就地编辑插件。底层深度集成 **Microsoft Edge WebView2 (Chromium)** 高性能渲染引擎，辅以纯 C# 原生 Scintilla 内存桥接与 PE 导出表注入，实现 **0ms 击键即时反馈、双向无感撤销流、全格式所见即所得 (WYSIWYG) 与暗黑模式零闪烁融合**。

---

## 🌟 核心特性全景 (Core Capabilities)

```
┌────────────────────────────────────────────────────────────────────────┐
│                      NppOmniView 全能视图矩阵                         │
├───────────────┬────────────────────────────────────────────────────────┤
│ 📝 Markdown   │ GFM 规范、GitHub 5 色呼出块、KaTeX 数学公式、Mermaid 图表│
│ 🌳 JSON       │ 交互式折叠语法树、数据类型胶囊、键值双击就地编辑、粘性顶栏 │
│ 📊 CSV/TSV/DSV│ 电子表格网格、自定义任意分隔符、单元格双击编辑、增删行 │
│ 🎨 SVG 矢量   │ 无损视口平移缩放、统一调色盘、矢量形状检视器、2x 导出 │
│ 📑 YAML       │ 结构化配置大纲树、js-yaml 校验与 2 空格美化、双击编辑   │
│ 📐 Mermaid    │ 原生 1:1 矢量抗锯齿、流程图/时序图/类图、节点双击编辑    │
│ 🌐 HTML       │ 1080p 默认超清视口、720p/2K/4K/全宽/自定义/手机移动端外框│
│ 📄 XML        │ 交互式 DOM 语法树、属性胶囊、节点双击编辑、标准美化   │
│ ⚡ 源码/纯文本│ 智能免渲染休眠 (保留原生极速零开销)、PowerShell AST 大纲│
└───────────────┴────────────────────────────────────────────────────────┘
```

### 1. ⚡ 毫秒级流体交互体验
- **0ms 击键雷达 (Zero-Latency Feedback)**：监听 Scintilla `SCN_MODIFIED` 事件，在 280ms 防抖定时器倒计时期间，即时激活拟真毛玻璃雷达 HUD 光环与顶部流光进度条，杜绝传统插件的卡顿与失焦感。
- **暗黑模式零白闪 (Zero-Flicker)**：动态读取 Notepad++ 主题并实时同步 `WebView2.DefaultBackgroundColor`（深色 `#1e1e1e` 与浅色 `#ffffff`），彻底消除打开面板、调整停靠尺寸及切换标签页时的“白屏闪烁”。
- **增量局部更新 (DOM Differential Updating)**：当文档格式未发生改变时，触发局部容器热更新，完美保留用户当前的滚动位置与折叠展开状态。

### 2. ✏️ 全格式双向就地编辑 (Bi-Directional In-Place Editing)
- 支持直接在预览窗口内通过双击或快捷键进入编辑状态：
  - **HTML 视口**：原生 `designMode` 所见即所得 (WYSIWYG) 实时修改文字与排版。
  - **JSON / YAML**：双击任意键（Key）或值（Value）直接弹出内联输入框修改，自动类型转换并写回。
  - **CSV / TSV**：如同 Excel 般双击单元格就地改值、上下左右光标快速跳转、快捷键加行/删行。
  - **SVG 矢量画布**：双击修改文本内容，右键检视矢量图形尺寸与填充色。
  - **Mermaid 架构图**：双击节点直接弹出节点文本编辑器实时修改节点名称。
  - **分屏实时对照抽屉 (Split Drawer)**：支持左侧全功能源码编辑、右侧实时渲染视口双向联动。
- **撤销堆栈保护**：所有回写操作均被原生 `SCI_BEGINUNDOACTION` / `SCI_ENDUNDOACTION` 事务包裹，在 Notepad++ 主界面按 `Ctrl+Z` 可单步干净撤销。

### 3. 🔄 跨格式一键转换与清洗工具箱
- **JSON**：美化缩进 (2-space)、压缩单行 (Minify)、一键转 CSV、一键转 GitHub Markdown 表格。
- **CSV / TSV / DSV**：自动探测任意分隔符（逗号、制表符、分号、竖线、脱字号等）、列宽等宽对齐 (Align)、去除外围冗余空格、一键转 JSON / Markdown 表格。
- **SVG / XML**：XML 规范标准 Pretty-Print 格式化、复制 Data URI、导出 2x 超清 PNG。
- **YAML**：规范化 2 空格美化、转 JSON 树、转 CSV。
- **Markdown**：一键转 HTML 完整源码。

### 4. 💡 纯文本与源码：智能免渲染休眠哲学 (Zero-Overhead Policy)
- **原生极速优先**：Notepad++ 本身已具备业界公认的高速 Scintilla 语法着色引擎。全能视图插件坚决摒弃在侧边栏重复构建庞大 DOM 代码高亮镜像的做法，杜绝双重内存占用与卡顿。
- **智能资源休眠**：打开纯文本或源码文件（如 `.py`, `.cs`, `.sql`, `.sh` 等）时，插件自动休眠重型 DOM 渲染引擎，仅展示轻量统计元数据概览，将 100% 算力归还 Notepad++ 编辑器。
- **PowerShell 深度赋能**：对于 PowerShell 脚本，提供基于原生 AST 分析的文档视图与函数大纲抽屉，按需辅助专业运维开发。

---

## ⌨️ 常用快捷键 (Keybindings)

| 快捷键 | 功能描述 |
| :--- | :--- |
| **`Ctrl + Alt + P`** | 打开 / 关闭 NppOmniView 全能视图停靠面板 |
| **`Ctrl + Alt + F`** | 一键智能美化 / 格式化当前文档 (JSON / CSV / SVG / YAML / XML) |
| **`F11` / `Alt + Enter`** | 切换全屏沉浸禅模式 (Zen Fullscreen Presentation Mode) |
| **`F2`** | 进入 / 退出当前视口双向就地编辑模式 |
| **`Ctrl + S`** (编辑状态下) | 保存就地编辑内容并同步写回 Notepad++ 编辑器 |
| **`Esc`** (编辑状态下) | 放弃当前编辑修改并恢复原本预览 |

---

## 📁 目录架构 (Project Architecture)

```
NppOmniView/
├── assets/                     # 图标资产 (SVG, 16x16 BMP, 亮暗两套 ICO)
├── dist/NppOmniView/           # 生产级构建发布目录 (包含插件 DLL 与依赖)
├── src/
│   ├── ExportPatcher/          # 基于 dnlib 的 PE 导出表无损注入命令行工具
│   └── NppOmniView/            # 插件核心源码
│       ├── Core/
│       │   ├── BufferReader.cs         # Scintilla 内存零拷贝安全读取器与大文件 LOH 优化
│       │   ├── FormatProcessor.cs     # JSON/CSV/SVG/YAML/XML 算法处理、对齐与转换引擎
│       │   ├── HtmlTemplateBuilder.cs # 纯原生微前端视口模板、CSS 变量与交互控制器
│       │   ├── MultiFormatDetector.cs # 智能多格式内容特征嗅探器
│       │   └── NppInterop.cs          # Notepad++ 与 Scintilla Win32 P/Invoke 互操作层
│       ├── Forms/
│       │   └── PreviewDockForm.cs     # 停靠窗体与 Edge WebView2 通信生命周期管理
│       └── Main.cs                    # 插件入口点、C-Export 回调绑定与消息分发
├── tests/                      # 自动化测试套件与工业级样本集
│   ├── run_tests.ps1           # 45 个大项、197 项独立断言测试执行器
│   ├── TEST_SPEC.md            # 测试规范与历史需求映射规约
│   └── sample.*                # JSON, YAML, CSV, TSV, DSV, SVG, MD, MMD, HTML, XML, PS1 等样本
├── pack_for_agent.ps1          # 一键全自动编译、PE 打补丁、测试并打包分发脚本
└── README.md                   # 官方技术文档与用户指南
```

---

## 🛠️ 构建与部署指南 (Build & Packaging)

### 系统环境要求
- **操作系统**：Windows 10 / 11 (x64)
- **Notepad++**：Notepad++ x64 (推荐 v8.5 及以上)
- **构建环境**：.NET Framework 4.8 Developer Pack + .NET SDK (`dotnet` CLI)
- **运行时支持**：Microsoft Edge WebView2 Runtime (Windows 11 自带，Windows 10 通常已随 Edge 预装)

### 一键构建与打包
在项目根目录下打开 PowerShell 执行打包脚本：

```powershell
# 执行全自动编译、PE 导出表注入、运行 197 项回归测试并生成 dist 目录
powershell -ExecutionPolicy Bypass -File .\pack_for_agent.ps1

# 若本机已安装 Notepad++，可附加参数一键热部署至插件目录
powershell -ExecutionPolicy Bypass -File .\pack_for_agent.ps1 -DeployToNpp
```

### 手动安装部署
1. 关闭正在运行的 Notepad++ 实例。
2. 将构建生成的 `dist/NppOmniView/` 文件夹整体复制到 Notepad++ 安装目录下的 `plugins/` 目录中：
   ```text
   C:\Program Files\Notepad++\plugins\NppOmniView\
   ├── NppOmniView.dll                        # 注入 PE 导出表的主插件动态链接库
   ├── WebView2Loader.dll                     # Microsoft Edge WebView2 x64 原生加载器
   ├── Markdig.dll                            # Markdown 解析引擎
   ├── Microsoft.Web.WebView2.Core.dll        # WebView2 核心交互程序集
   ├── Microsoft.Web.WebView2.WinForms.dll    # WinForms 宿主容器控件
   ├── System.Buffers.dll                     # 内存缓冲区基础依赖
   ├── System.Memory.dll                      # Span/Memory 零拷贝内存操作依赖
   ├── System.Numerics.Vectors.dll            # 向量运算基础依赖
   ├── System.Runtime.CompilerServices.Unsafe.dll # 快速不安全内存访问支持
   ├── toolbar_16x16.bmp                      # Notepad++ 经典工具栏 16x16 位图图标
   ├── toolbar_light.ico                      # 浅色模式工具栏矢量图标
   ├── toolbar_dark.ico                       # 暗黑模式工具栏矢量图标
   └── icon.svg                               # 项目矢量高清 Logo
   ```
3. 重新启动 Notepad++，在顶部菜单栏 **插件 (Plugins)** -> **NppOmniView 全能视图** 即可看到控制菜单与工具栏图标。

---

## 🧪 自动化测试与质量门禁 (Verification)

本插件内置了严密的自动化单元与回归测试体系：

```powershell
powershell -ExecutionPolicy Bypass -File .\tests\run_tests.ps1
```

测试覆盖了包括空结构处理、RFC 4180 双引号转义、跨平台浮点数不变量、自定义符号识别、矢量图抗锯齿、双向就地编辑全景等 **45 个核心大项、197 个断言**，在任何构建发布前强制保证 **100% 通过 (0 Failed)**。详细规约请参阅 [`tests/TEST_SPEC.md`](tests/TEST_SPEC.md)。

---

## 🌐 官方插件生态兼容性 (nppPluginList Compatibility)

本项目完全遵循 Notepad++ 官方插件清单 ([`nppPluginList`](https://github.com/notepad-plus-plus/nppPluginList)) 开源规约：
- **插件标识**：文件夹目录 `NppOmniView` 与主程序集 `NppOmniView.dll` 严格一致。
- **目标架构**：x64 原生 PE 导出表注入，兼容 Notepad++ 8.0 及以上所有 x64 版本。
- **清单模板**：已在项目根目录下提供预置的标准元数据文件 [`nppPluginList.json`](nppPluginList.json)，便于向官方插件仓库发起自动化入驻请求。

---

## 📄 开源许可证

本项目基于 [Apache License 2.0](LICENSE) 协议分发。欢迎查阅与参与贡献。
