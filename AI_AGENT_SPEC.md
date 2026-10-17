# AI AGENT IMPLEMENTATION & MAINTENANCE SPEC: NppOmniView (Notepad++ Plugin)

## 1. Project Identity
- **Plugin Name (Menu & DLL)**: `NppOmniView` (`NppOmniView.dll`)
- **Chinese Display Name**: `NppOmniView 全能视图`
- **Root Namespace**: `NppPlugin.OmniView`
- **Target Architecture**: `x64` (.NET Framework 4.8)
- **Target Notepad++ Plugin Directory**: `D:\Software\Notepad++\plugins\NppOmniView\`

## 2. Core Capabilities
1. **Multi-Format Real-Time Preview**: Supports **Markdown** (GFM + GitHub 5-Color Alerts + KaTeX + Mermaid + Smooth TOC Anchor Navigation), **JSON** (Interactive collapsible syntax tree + validation + sticky single-row toolbar), **CSV/TSV/DSV** (Sortable, filterable, editable data table + custom delimiter modal + sticky toolbar), **SVG** (Interactive pan & zoom, unified color palette popover, PNG export, Data URI copy), **YAML** (Hierarchical tree, type badges, validation, 2-space beautify), **Mermaid** (Independent .mmd architecture and sequence diagrams with 1:1 crisp native vector rendering, auto-fit viewport, dedicated zoom badge, unified palette popover, 2x Retina PNG export), **HTML** (Responsive sandbox with 1080p default, 720p/2K/4K presets, custom resolution modal, mobile phone bezel framing, and screen orientation toggle), **XML** (Interactive collapsible DOM tree, attribute pills, Pretty-Print), and **Code / Text** (Intelligent resource-saving sleep mode avoiding redundant DOM mirroring, with PowerShell AST documentation outline analysis).
2. **Unsaved Buffer & Saved File Support**: Reads directly from Scintilla memory via `SCI_GETTEXT` (`BufferReader.GetCurrentDocument()`). Works on unsaved `new 1` tabs without requiring disk files.
3. **In-Place Formatting & Cross-Format Conversion**:
   - JSON: Beautify (2-space indent), Minify, Convert `JSON -> CSV`, Convert `JSON -> Markdown Table`.
   - CSV: Align columns, Minify, Replace Delimiter (Comma, Tab/TSV, Semicolon, Pipe, Custom), Convert `CSV -> JSON`, Convert `CSV -> Markdown Table`.
   - SVG: XML standard beautification & indenting via `FormatSvg()`, Copy Data URI.
   - XML: Standard Pretty-Print indentation and beautification via `FormatXml()`.
   - YAML: 2-space normalized beautify, Convert `YAML -> JSON`, Convert `YAML -> CSV`.
   - Markdown: Convert `Markdown -> HTML Source`.
   - All editor text replacements are wrapped in `SCI_BEGINUNDOACTION` (2078) and `SCI_ENDUNDOACTION` (2079) so `Ctrl+Z` cleanly undoes any formatting or conversion.
4. **Toolbar Icon & Shortcuts**:
   - Registers `toolbar_16x16.bmp`, `toolbar_light.ico`, and `toolbar_dark.ico` on `NPPN_TBMODIFICATION` using `NPPM_ADDTOOLBARICON_FORDARKMODE`.
   - `Ctrl+Alt+P`: Toggle NppOmniView Dock Panel.
   - `Ctrl+Alt+F`: One-key Smart Beautify/Format for current buffer.
5. **Fluidity & Visual Feedback UX**:
   - **0ms Keystroke Feedback**: Scintilla `SCN_MODIFIED` triggers immediate visual acknowledgement (`NotifyTypingPending`) before the 280ms debounce timer ends.
   - **Linear Top Progress Loader**: A glowing gradient progress bar (`.top-loader`) indicates rendering and conversion progress without layout shifts.
   - **Breathing Status Pulse**: The topbar format badge incorporates a breathing dot (`.pulse-dot`) that pulses amber while typing/rendering and turns steady green when synced.
   - **Dark-Mode Flash Elimination**: `WebView2.DefaultBackgroundColor` is dynamically bound to Notepad++'s dark mode state (`#1e1e1e` vs `#ffffff`), eliminating white flicker during initialization, dock resizing, and tab switches.

## 3. Directory Structure
- `assets/`: `icon.svg`, `toolbar_16x16.bmp`, `toolbar_light.ico`, `toolbar_dark.ico`
- `src/NppOmniView/`: Main plugin C# source project (`NppOmniView.csproj`)
- `src/ExportPatcher/`: `dnlib`-based PE export table injector (`ExportPatcher.csproj`)
- `tests/`: `run_tests.ps1`, `TEST_SPEC.md`, `samples/` (`sample.md`, `sample.json`, `sample.csv`, `sample.tsv`, `sample.svg`, `sample.yaml`, `sample.mmd`, `sample.html`, `sample.xml`, `sample.ps1`, `sample.py`, `sample.cs`, `sample.sql`, `sample.sh`, `sample_class.mmd`, `sample_sequence.mmd`, `sample_pipe.csv`, `sample_semicolon.csv`)
- `dist/NppOmniView/`: Ready-to-deploy compiled plugin folder

## 4. Build & Deployment Command
Run `powershell -ExecutionPolicy Bypass -File .\pack_for_agent.ps1` from the root directory, or build `src/NppOmniView/NppOmniView.csproj` in Release mode and run `ExportPatcher.exe` on `NppOmniView.dll`.
