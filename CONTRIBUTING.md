# 贡献指南 (Contributing Guide)

感谢您对 **NppOmniView (全能视图)** 项目的关注与支持！我们欢迎社区提交 Issue 缺陷报告、新功能建议以及 Pull Request 代码贡献。

为了保证插件在 Notepad++ 宿主环境下的高稳定度、高性能与工业级品质，请在参与贡献时遵循以下开发规约。

---

## 🛠️ 开发环境要求

1. **操作系统**：Windows 10 / Windows 11 (x64)
2. **IDE / 工具链**：
   - Visual Studio 2022 (或 VS Code + C# Dev Kit)
   - .NET Framework 4.8 Developer Pack
   - .NET SDK (`dotnet` CLI v6.0+)
3. **宿主软件**：Notepad++ x64 (推荐 v8.5 及以上最新版本)
4. **运行时引擎**：Microsoft Edge WebView2 Runtime (Windows 11 自带，Windows 10 推荐安装最新 Evergreen 运行时)

---

## 🏗️ 架构与编译流程

NppOmniView 采用纯 C# 编写，但通过配套的 `ExportPatcher` 工具向托管 DLL 注入原生的 Win32 C 导出函数，使其能够被 Notepad++ 的插件管理器正常加载。

```
[C# 源码 (net48)] ──> dotnet build ──> [NppOmniView.dll]
                                              │
                                              ▼
[ExportPatcher.exe (dnlib)] ──注入 PE 导出表──┘
                                              │
                                              ▼
                           [可被 Notepad++ 加载的原生插件 DLL]
```

### 一键构建与打包
在仓库根目录下执行 PowerShell 脚本：

```powershell
powershell -ExecutionPolicy Bypass -File .\pack_for_agent.ps1
```

该脚本将自动执行：
1. 编译 `ExportPatcher.exe`
2. 完整构建 `NppOmniView.csproj`
3. 运行 `ExportPatcher.exe` 注入原生 PE 导出表 (`isUnicode`, `setInfo`, `getName`, `getFuncsArray`, `beNotified`, `messageProc`)
4. 运行全量 197 项自动化回归测试
5. 将最终生产工件与依赖资源输出至 `dist/NppOmniView/` 目录

---

## 🧪 自动化测试规约

在提交任何 Pull Request 之前，**必须**确保所有自动化测试用例 100% 通过：

```powershell
powershell -ExecutionPolicy Bypass -File .\tests\run_tests.ps1
```

如果您添加了新的文件格式嗅探规则、新的格式转换器或新的编辑命令，必须在 `tests/run_tests.ps1` 与 `tests/TEST_SPEC.md` 中补充对应的断言用例，并确保无任何回归缺陷。

---

## 📝 代码与注释风格

1. **代码注释规范**：
   - 只要文件格式支持（UTF-8），所有新增或修改的公共类、接口、核心算法及复杂状态逻辑，**必须附带详尽、高质量的中文注释**（包括 XML 文档注释 `///`）。
2. **编码与换行符**：
   - 源代码文件统一保存为 **UTF-8 with BOM / UTF-8 无 BOM**。
   - 行尾符在 Windows 开发环境下规范为 `CRLF`。
3. **内存与性能**：
   - 严禁在 Scintilla 内存读取热路径上发生无谓的字符串大对象分配。
   - 尽量复用缓冲区，避免频繁触发 GC LOH 回收。

---

## 🤝 提交与分支规范

- **分支命名**：
  - 功能特性：`feat/<feature-name>`
  - 缺陷修复：`fix/<issue-name>`
  - 文档重构：`docs/<topic>`
- **Commit 提交信息**：
  - 推荐遵循 [Conventional Commits](https://www.conventionalcommits.org/zh-hans/) 规范，例如：
    - `feat: add yaml in-place editing controller`
    - `fix: prevent null pointer dereference on empty buffer notification`
    - `docs: update nppPluginList integration instructions`
