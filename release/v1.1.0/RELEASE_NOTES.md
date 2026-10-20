# NppOmniView v1.1.0 正式版发布说明

## 🌟 版本概览 (Release Overview)

**NppOmniView (全能视图) v1.1.0** 正式发布！本次重大迭代带来了多脚本语言智能语法分析矩阵、Markdown 数学公式 (KaTeX) 高保真深度解析、双向高精度行对齐平滑同步、XML 三模合一交互矩阵以及工业级 WebView2 安全沙箱防护。全自动化测试回归门禁已扩充至 **252 项断言**，100% 绿色通行！

---

## 🚀 核心更新内容 (What's New)

### 1. 多脚本语言结构大纲与智能工作台
- 🐍 **Python 脚本大纲分析器 (`PythonScriptAnalyzer`)**：
  - 精准提取类、派生类、成员方法与独立函数；
  - 侧边栏集成符号列表，支持一键点击毫秒级定位跳转。
- 🗄️ **SQL 架构分析器 (`SqlScriptAnalyzer`)**：
  - 自动识别 MySQL、PostgreSQL、SQL Server、Oracle 等常用方言；
  - 提取数据表定义 (`CREATE TABLE`)、视图与核心 DDL/DML 结构，支持实时搜索与表级跳转。
- 🐚 **Shell / Bash 分析器 (`ShellScriptAnalyzer`)**：
  - 精准提取 Shebang (`#!/bin/bash` 等) 与脚本执行模式；
  - 严格检测 `set -euo pipefail` 模式，提取函数定义与核心脚本流程大纲。
- ⚡ **Batch / CMD 批处理分析器 (`BatchScriptAnalyzer`)**：
  - 提取批处理中的所有跳转标签 (`:label`) 与行内说明；
  - 识别 `@echo off`、`chcp 65001`、`setlocal` 等环境初始化指令；
  - 实时标记敏感高危指令（如 `del /f /q`）并提供安全预警提示。
- ⚙️ **INI 配置文件网格矩阵 (`IniConfigAnalyzer`)**：
  - 解析配置节 (`[Section]`) 与键值对，提供可视化分节表格视图与实时检索过滤。

### 2. Markdown 数学公式 (KaTeX) 高保真深度解析
- **公式块与行内公式全景支持**：支持多行方程对齐、分段函数、矩阵运算等复杂 LaTeX 数学公式块；
- **代码块语法隔离**：在包含公式符号的代码块 (` ``` `) 内进行严密隔离保护，杜绝代码被误转义破坏；
- **动态离线注入**：集成 KaTeX 离线渲染与安全挂载生命周期钩子，大文档渲染零错乱。

### 3. 双向高精度行对齐平滑同步 (Bi-directional Sync Scroll)
- **Scintilla 物理行号锚点**：自动注入 `pragma-line` 物理行号锚点并进行平滑插值，彻底解决长图文与编辑器错位问题；
- **反向同步通道 (`SYNC_SCROLL_TO_NPP`)**：在网页预览区平滑滚动时，可驱动 Notepad++ 编辑器窗口进行精准视口协同；
- **原生行跳转增强**：直接调用 Scintilla 原生消息，实现毫秒级平滑定位。

### 4. XML 交互矩阵三模合一与沙箱防御
- **三模态工作台**：交互式可折叠 DOM 树 (Tree)、实体数据网格 (Grid)、XML 转 JSON (JSON) 三种视图自由无缝切换；
- **实时 XPath 面包屑**：悬浮展示当前层级完整 XPath 路径并支持单击复制；
- **WebView2 安全沙箱防护**：注入 `SafeStorage` 代理，杜绝因 `localStorage` 受限引发的 `SecurityError`；
- **就地编辑遮罩保护**：增加带防抖时间锁的时序守卫，防止快速点击导致就地编辑器意外收起或状态竞争。

### 5. 质量工程与回归测试
- 回归测试断言全面扩充至 **252 项**，涵盖各脚本分析器、KaTeX 公式保护、双向滚动协议、INI 解析与安全沙箱，全量 100% 绿色通行！

---

## 📦 安装与升级指引 (Installation)

1. 下载下方发布资产中的 `NppOmniView-v1.1.0-x64.zip`；
2. 退出 Notepad++；
3. 将压缩包内的 `NppOmniView` 文件夹整体解压至 Notepad++ 的 `plugins/` 目录下（如 `C:\Program Files\Notepad++\plugins\` 或便携版的对应目录）；
4. 启动 Notepad++，在顶部菜单栏查看 `插件 (P) -> NppOmniView` 或点击工具栏图标开启体验。

---

## 🔒 资产完整性校验 (SHA-256)

请参见附件 `SHA256SUMS.txt` 校验下载包的完整性。
