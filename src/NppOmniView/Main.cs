using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NppPlugin.OmniView.Core;
using NppPlugin.OmniView.Forms;

namespace NppPlugin.OmniView
{
    public static class Main
    {
        public const string PluginName = "NppOmniView";
        /// <summary>
        /// NppOmniView 官方正式发布版本号 (符合语义化版本 SemVer 规范与 nppPluginList 标准)
        /// </summary>
        public const string PluginVersion = "1.1.0";

        private static PreviewDockForm _previewForm = null;
        private static Timer _debounceTimer;
        private static DateTime _lastPreviewDrivenScrollTime = DateTime.MinValue;
        private static readonly Dictionary<IntPtr, DocFormat> _forcedOverrides = new Dictionary<IntPtr, DocFormat>();
        private static IntPtr _ptrPluginName = IntPtr.Zero;
        private static IntPtr _ptrFuncItems = IntPtr.Zero;
        private static NppFuncItemDelegate _toggleDelegate;
        private static NppFuncItemDelegate _formatDelegate;
        private static NppFuncItemDelegate _autoCollapseDelegate;
        private static NppFuncItemDelegate _aboutDelegate;
        private static IntPtr _ptrTbIcons = IntPtr.Zero;
        private static bool _resolverRegistered = false;
        private static bool _autoCollapseEnabled = true;
        private static bool _autoCollapsedByPlugin = false;
        private static bool _userExplicitlyClosed = false;

        private static string GetConfigFilePath()
        {
            try
            {
                string nppAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Notepad++", "plugins", "config");
                if (Directory.Exists(nppAppData))
                {
                    return Path.Combine(nppAppData, "NppOmniView.ini");
                }
                string localFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(localFolder ?? ".", "NppOmniView.ini");
            }
            catch
            {
                return "NppOmniView.ini";
            }
        }

        private static void LoadConfig()
        {
            try
            {
                string cfgPath = GetConfigFilePath();
                if (File.Exists(cfgPath))
                {
                    var sb = new System.Text.StringBuilder(64);
                    Win32.GetPrivateProfileString("Settings", "AutoCollapse", "1", sb, sb.Capacity, cfgPath);
                    _autoCollapseEnabled = sb.ToString().Trim() != "0";
                }
                else
                {
                    _autoCollapseEnabled = true;
                }
            }
            catch
            {
                _autoCollapseEnabled = true;
            }
        }

        private static void SaveConfig()
        {
            try
            {
                string cfgPath = GetConfigFilePath();
                string dir = Path.GetDirectoryName(cfgPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                Win32.WritePrivateProfileString("Settings", "AutoCollapse", _autoCollapseEnabled ? "1" : "0", cfgPath);
            }
            catch { }
        }

        private static void UpdateMenuCheckState()
        {
            if (PluginBase.nppData._nppHandle != IntPtr.Zero)
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, 2, _autoCollapseEnabled ? 1 : 0);
            }
        }

        private static void EnsureAssemblyResolver()
        {
            if (_resolverRegistered) return;
            _resolverRegistered = true;
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string asmName = new AssemblyName(args.Name).Name + ".dll";
                string pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string candidate = Path.Combine(pluginFolder, asmName);
                return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
            };
        }

        public static bool isUnicode()
        {
            EnsureAssemblyResolver();
            return true;
        }

        public static void setInfo(NppData notepadPlusData)
        {
            EnsureAssemblyResolver();
            PluginBase.nppData = notepadPlusData;
            LoadConfig();
            _debounceTimer = new Timer { Interval = 280 };
            _debounceTimer.Tick += (s, e) => { _debounceTimer.Stop(); RefreshPreview(); };
        }

        public static IntPtr getName()
        {
            if (_ptrPluginName == IntPtr.Zero)
                _ptrPluginName = Marshal.StringToHGlobalUni(PluginName);
            return _ptrPluginName;
        }

        public static IntPtr getFuncsArray(ref int nbF)
        {
            nbF = 4;
            if (_ptrFuncItems == IntPtr.Zero)
            {
                LoadConfig();
                _toggleDelegate = TogglePreviewPanel;
                _formatDelegate = SmartFormatCurrentBuffer;
                _autoCollapseDelegate = ToggleAutoCollapse;
                _aboutDelegate = ShowAboutDialog;

                ShortcutKey skToggle = new ShortcutKey { _isCtrl = 1, _isAlt = 1, _isShift = 0, _key = (byte)Keys.P };
                IntPtr ptrSkToggle = Marshal.AllocHGlobal(Marshal.SizeOf(skToggle));
                Marshal.StructureToPtr(skToggle, ptrSkToggle, false);

                ShortcutKey skFormat = new ShortcutKey { _isCtrl = 1, _isAlt = 1, _isShift = 0, _key = (byte)Keys.F };
                IntPtr ptrSkFormat = Marshal.AllocHGlobal(Marshal.SizeOf(skFormat));
                Marshal.StructureToPtr(skFormat, ptrSkFormat, false);

                FuncItem item0 = new FuncItem
                {
                    _itemName = "\u6253\u5F00/\u5173\u95ED NppOmniView \u5168\u80FD\u89C6\u56FE",
                    _pFunc = Marshal.GetFunctionPointerForDelegate(_toggleDelegate),
                    _cmdID = 0,
                    _init2Check = false,
                    _pShKey = ptrSkToggle
                };

                FuncItem item1 = new FuncItem
                {
                    _itemName = "\u4E00\u952E\u7F8E\u5316/\u683C\u5F0F\u5316\u5F53\u524D\u6587\u6863",
                    _pFunc = Marshal.GetFunctionPointerForDelegate(_formatDelegate),
                    _cmdID = 1,
                    _init2Check = false,
                    _pShKey = ptrSkFormat
                };

                FuncItem item2 = new FuncItem
                {
                    _itemName = "\u26A1 \u667A\u80FD\u81EA\u9002\u5E94\u6536\u6298\u9762\u677F (Auto-Collapse)",
                    _pFunc = Marshal.GetFunctionPointerForDelegate(_autoCollapseDelegate),
                    _cmdID = 2,
                    _init2Check = _autoCollapseEnabled,
                    _pShKey = IntPtr.Zero
                };

                FuncItem item3 = new FuncItem
                {
                    _itemName = "\u5173\u4E8E NppOmniView (About)...",
                    _pFunc = Marshal.GetFunctionPointerForDelegate(_aboutDelegate),
                    _cmdID = 3,
                    _init2Check = false,
                    _pShKey = IntPtr.Zero
                };

                int itemSize = Marshal.SizeOf(typeof(FuncItem));
                _ptrFuncItems = Marshal.AllocHGlobal(itemSize * 4);
                Marshal.StructureToPtr(item0, _ptrFuncItems, false);
                Marshal.StructureToPtr(item1, new IntPtr(_ptrFuncItems.ToInt64() + itemSize), false);
                Marshal.StructureToPtr(item2, new IntPtr(_ptrFuncItems.ToInt64() + itemSize * 2), false);
                Marshal.StructureToPtr(item3, new IntPtr(_ptrFuncItems.ToInt64() + itemSize * 3), false);
            }
            return _ptrFuncItems;
        }

        public static void beNotified(IntPtr notifyCode)
        {
            // P0: Prevent crash on null pointer dereference
            if (notifyCode == IntPtr.Zero) return;

            ScNotification notification = (ScNotification)Marshal.PtrToStructure(notifyCode, typeof(ScNotification));
            uint code = notification.Header.code;

            if (code == (uint)NppMsg.NPPN_READY)
            {
                UpdateMenuCheckState();
                return;
            }
            else if (code == (uint)NppMsg.NPPN_TBMODIFICATION)
            {
                RegisterToolbarIcon();
                UpdateMenuCheckState();
                return;
            }
            else if (code == (uint)NppMsg.NPPN_DARKMODECHANGED)
            {
                _previewForm?.UpdateTheme(PluginBase.IsDarkModeEnabled());
                return;
            }
            else if (code == (uint)NppMsg.NPPN_SHUTDOWN)
            {
                _debounceTimer?.Stop();
                _debounceTimer?.Dispose();
                _debounceTimer = null;
                _previewForm?.Dispose();
                _previewForm = null;
                return;
            }

            // Suppress cascading updates caused by plugin's own programmatic edits
            if (BufferReader.IsFormatting) return;

            if (code == (uint)NppMsg.NPPN_BUFFERACTIVATED || code == (uint)NppMsg.NPPN_FILESAVED)
            {
                DocumentContext doc = BufferReader.GetCurrentDocument();
                if (_autoCollapseEnabled)
                {
                    DetectionResult det = MultiFormatDetector.Evaluate(doc, _forcedOverrides);
                    bool isRich = IsRichViewDocument(doc, det);

                    if (!isRich)
                    {
                        // 非富文档：若当前面板可见，则自动收拢面板让出满宽编辑区
                        if (_previewForm != null && _previewForm.Visible)
                        {
                            _autoCollapsedByPlugin = true;
                            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, _previewForm.Handle);
                            return;
                        }
                    }
                    else
                    {
                        // 富文档：若此前是由插件自动收折的，且用户未显式主动关闭，则自动恢复弹开
                        if (_autoCollapsedByPlugin && !_userExplicitlyClosed)
                        {
                            _autoCollapsedByPlugin = false;
                            if (_previewForm != null)
                            {
                                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, _previewForm.Handle);
                            }
                        }
                    }
                }

                if (_previewForm == null || !_previewForm.Visible) return;

                _previewForm?.NotifyLoadingStarted("正在读取并加载文档视图...");
                RefreshPreview();
                return;
            }

            if (_previewForm == null || !_previewForm.Visible) return;

            if (code == (uint)SciMsg.SCN_MODIFIED)
            {
                if ((notification.ModificationType & 0x03) != 0)
                {
                    _previewForm?.NotifyTypingPending();
                    _debounceTimer?.Stop();
                    _debounceTimer?.Start();
                }
            }
            else if (code == (uint)SciMsg.SCN_UPDATEUI)
            {
                if ((notification.Updated & 0x08) != 0)
                {
                    // 若在 220ms 内由预览视口反向驱动滚动，则屏蔽向预览的回发，避免死循环震荡
                    if ((DateTime.UtcNow - _lastPreviewDrivenScrollTime).TotalMilliseconds < 220)
                        return;

                    BufferReader.GetScrollLineInfo(out int firstLine, out int totalLines);
                    _previewForm?.SyncScrollToLine(firstLine, totalLines);
                }
            }
        }

        public static IntPtr messageProc(uint Message, IntPtr wParam, IntPtr lParam) => new IntPtr(1);

        private static void RegisterToolbarIcon()
        {
            try
            {
                string pluginDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string bmpPath = Path.Combine(pluginDir, "toolbar_16x16.bmp");
                string icoLightPath = Path.Combine(pluginDir, "toolbar_light.ico");
                string icoDarkPath = Path.Combine(pluginDir, "toolbar_dark.ico");

                if (!File.Exists(bmpPath)) return;

                IntPtr hBmp = Win32.LoadImage(IntPtr.Zero, bmpPath, Win32.IMAGE_BITMAP, 16, 16, Win32.LR_LOADFROMFILE);
                IntPtr hIcoLight = File.Exists(icoLightPath) ? Win32.LoadImage(IntPtr.Zero, icoLightPath, Win32.IMAGE_ICON, 32, 32, Win32.LR_LOADFROMFILE) : IntPtr.Zero;
                IntPtr hIcoDark = File.Exists(icoDarkPath) ? Win32.LoadImage(IntPtr.Zero, icoDarkPath, Win32.IMAGE_ICON, 32, 32, Win32.LR_LOADFROMFILE) : hIcoLight;

                FuncItem firstItem = (FuncItem)Marshal.PtrToStructure(_ptrFuncItems, typeof(FuncItem));

                ToolbarIconsWithDarkMode tbIcons = new ToolbarIconsWithDarkMode
                {
                    hToolbarBmp = hBmp,
                    hToolbarIcon = hIcoLight,
                    hToolbarIconDarkMode = hIcoDark
                };

                // Notepad++ holds onto the toolbarIconsWithDarkMode pointer for dark mode toggles.
                // It must NOT be freed while Notepad++ is running.
                if (_ptrTbIcons == IntPtr.Zero)
                    _ptrTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));

                Marshal.StructureToPtr(tbIcons, _ptrTbIcons, false);
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON_FORDARKMODE, firstItem._cmdID, _ptrTbIcons);
            }
            catch { }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void TogglePreviewPanel()
        {
            EnsureAssemblyResolver();
            if (_previewForm == null)
            {
                _userExplicitlyClosed = false;
                _autoCollapsedByPlugin = false;
                _previewForm = new PreviewDockForm(HandleWebViewAction);

                NppTbData tbData = new NppTbData
                {
                    hClient = _previewForm.Handle,
                    pszName = "NppOmniView \u5168\u80FD\u89C6\u56FE",
                    dlgID = 0,
                    uMask = NppTbMsg.DWS_DF_CONT_RIGHT,
                    pszModuleName = PluginName + ".dll"
                };

                IntPtr ptrTb = Marshal.AllocHGlobal(Marshal.SizeOf(tbData));
                Marshal.StructureToPtr(tbData, ptrTb, false);
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, ptrTb);
                RefreshPreview();
            }
            else
            {
                // Synchronize strictly with the form's real visible state
                if (_previewForm.Visible)
                {
                    _userExplicitlyClosed = true;
                    _autoCollapsedByPlugin = false;
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, _previewForm.Handle);
                }
                else
                {
                    _userExplicitlyClosed = false;
                    _autoCollapsedByPlugin = false;
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, _previewForm.Handle);
                    RefreshPreview();
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ToggleAutoCollapse()
        {
            EnsureAssemblyResolver();
            _autoCollapseEnabled = !_autoCollapseEnabled;
            SaveConfig();
            UpdateMenuCheckState();

            string tip = _autoCollapseEnabled 
                ? "⚡ 已开启智能自适应收折：纯文本/无富预览文档将自动收起面板，富文档自动弹开" 
                : "⚡ 已关闭智能自适应收折：面板将保持当前停靠状态不变";

            if (_previewForm != null && _previewForm.Visible)
            {
                _previewForm.ExecuteScript("if(window.showToast) window.showToast('" + tip.Replace("'", "\\'") + "');");
            }

            DocumentContext doc = BufferReader.GetCurrentDocument();
            DetectionResult det = MultiFormatDetector.Evaluate(doc, _forcedOverrides);
            bool isRich = IsRichViewDocument(doc, det);

            if (_autoCollapseEnabled)
            {
                if (!isRich && _previewForm != null && _previewForm.Visible)
                {
                    _autoCollapsedByPlugin = true;
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, _previewForm.Handle);
                }
                else if (isRich && _autoCollapsedByPlugin)
                {
                    _autoCollapsedByPlugin = false;
                    if (_previewForm != null)
                    {
                        Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, _previewForm.Handle);
                        RefreshPreview();
                    }
                }
            }
            else
            {
                if (_autoCollapsedByPlugin && _previewForm != null)
                {
                    _autoCollapsedByPlugin = false;
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, _previewForm.Handle);
                    RefreshPreview();
                }
            }
        }

        /// <summary>
        /// 判断当前文档是否属于支持 OmniView 专属结构化/可视化富视图的文档类型
        /// </summary>
        public static bool IsRichViewDocument(DocumentContext doc, DetectionResult det)
        {
            if (doc == null || det == null) return false;

            // 1. 核心8大富格式
            if (det.Format == DocFormat.Markdown ||
                det.Format == DocFormat.Json ||
                det.Format == DocFormat.Csv ||
                det.Format == DocFormat.Svg ||
                det.Format == DocFormat.Yaml ||
                det.Format == DocFormat.Mermaid ||
                det.Format == DocFormat.Html ||
                det.Format == DocFormat.Xml)
            {
                return true;
            }

            // 2. 具备结构化大纲与可视化分析的代码类型
            if (det.Format == DocFormat.Code)
            {
                string lang = !string.IsNullOrEmpty(det.DetectedLanguage) ? det.DetectedLanguage : MultiFormatDetector.GetLanguageForExtension(doc.Extension);
                if (lang.Equals("powershell", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("batch", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("sql", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("bash", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("shell", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("sh", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("zsh", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("python", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("ini", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("toml", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("cfg", StringComparison.OrdinalIgnoreCase) ||
                    lang.Equals("conf", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SmartFormatCurrentBuffer()
        {
            EnsureAssemblyResolver();
            DocumentContext doc = BufferReader.GetCurrentDocument();
            DetectionResult det = MultiFormatDetector.Evaluate(doc, _forcedOverrides);
            if (det.Format == DocFormat.Json)
            {
                string formatted = FormatProcessor.BeautifyJson(doc.Content, out string err);
                if (err == null) BufferReader.ReplaceCurrentDocumentText(formatted);
            }
            else if (det.Format == DocFormat.Csv)
            {
                BufferReader.ReplaceCurrentDocumentText(FormatProcessor.AlignCsvColumns(doc.Content));
            }
            else if (det.Format == DocFormat.Svg)
            {
                string formatted = FormatProcessor.FormatSvg(doc.Content, out string err);
                if (err == null) BufferReader.ReplaceCurrentDocumentText(formatted);
            }
            else if (det.Format == DocFormat.Xml)
            {
                string formatted = FormatProcessor.FormatXml(doc.Content, out string err);
                if (err == null) BufferReader.ReplaceCurrentDocumentText(formatted);
            }
            RefreshPreview();
        }

        /// <summary>
        /// 弹出关于对话框，展示插件版本号、架构特性与开源版权信息 (符合 Notepad++ 官方开源插件规范)
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ShowAboutDialog()
        {
            EnsureAssemblyResolver();
            string aboutMessage = $"{PluginName} (全能视图) v{PluginVersion}\r\n\r\n" +
                                  "现代工业级多格式实时渲染预览与双向就地编辑 Notepad++ 插件 (x64)\r\n\r\n" +
                                  "【核心架构】\r\n" +
                                  "• 渲染内核: Microsoft Edge WebView2 (Chromium)\r\n" +
                                  "• 8大富格式渲染: Markdown, JSON, CSV/TSV, SVG, YAML, Mermaid, HTML, XML\r\n" +
                                  "• 纯文本/源码策略: 智能免渲染休眠 (保留 Notepad++ 原生极速编辑与零额外消耗)\r\n" +
                                  "• 交互特性: 0ms 击键即时反馈、双向就地编辑、暗黑模式零闪烁、跨格式清洗转换\r\n\r\n" +
                                  "【开源规范】\r\n" +
                                  "• 开源协议: Apache License 2.0\r\n" +
                                  "• 项目主页: https://github.com/npp-plugins/NppOmniView\r\n\r\n" +
                                  "Copyright © 2026 NppOmniView Contributors. All rights reserved.";

            MessageBox.Show(aboutMessage, $"关于 {PluginName} v{PluginVersion}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void HandleWebViewAction(string action)
        {
            if (action == "TOGGLE_AUTO_COLLAPSE")
            {
                ToggleAutoCollapse();
                return;
            }

            if (action.StartsWith("JUMP_TO_LINE:"))
            {
                if (int.TryParse(action.Substring("JUMP_TO_LINE:".Length), out int line))
                {
                    BufferReader.JumpToDocumentLine(line);
                }
                return;
            }

            if (action.StartsWith("SYNC_SCROLL_TO_NPP:"))
            {
                if (int.TryParse(action.Substring("SYNC_SCROLL_TO_NPP:".Length), out int line))
                {
                    _lastPreviewDrivenScrollTime = DateTime.UtcNow;
                    BufferReader.ScrollToDocumentLine(line);
                }
                return;
            }

            if (action == "TRIGGER_SYNC_SCROLL")
            {
                BufferReader.GetScrollLineInfo(out int firstLine, out int totalLines);
                _previewForm?.SyncScrollToLine(firstLine, totalLines);
                return;
            }

            if (action.StartsWith("UPDATE_BUFFER:"))
            {
                string newText = action.Substring("UPDATE_BUFFER:".Length);
                BufferReader.ReplaceCurrentDocumentText(newText);
                RefreshPreview();
                return;
            }

            DocumentContext doc = BufferReader.GetCurrentDocument();
            if (action.StartsWith("CSV_REPLACE_DELIM:"))
            {
                string param = action.Substring("CSV_REPLACE_DELIM:".Length);
                char targetDelim = ',';
                if (param == "\\t" || param.Equals("TAB", StringComparison.OrdinalIgnoreCase)) targetDelim = '\t';
                else if (param.Length > 0) targetDelim = param[0];

                char oldDelim = FormatProcessor.DetectCsvDelimiter(doc.Content);
                string newContent = FormatProcessor.ConvertCsvDelimiter(doc.Content, targetDelim, oldDelim);
                BufferReader.ReplaceCurrentDocumentText(newContent, resetHScroll: true);
                RefreshPreview();
                return;
            }

            if (action.StartsWith("GOTO_LINE:"))
            {
                string lineStr = action.Substring("GOTO_LINE:".Length);
                if (int.TryParse(lineStr, out int targetLine))
                {
                    BufferReader.JumpToDocumentLine(targetLine - 1);
                }
                return;
            }

            if (action.StartsWith("RUN_BATCH:"))
            {
                string mode = action.Substring("RUN_BATCH:".Length).Trim();
                bool isAdmin = mode.Equals("admin", StringComparison.OrdinalIgnoreCase);

                if (!doc.IsSavedOnDisk || string.IsNullOrEmpty(doc.FullPath) || !File.Exists(doc.FullPath))
                {
                    _previewForm?.ExecuteScript("if(window.showToast) window.showToast('⚠️ 请先将当前批处理脚本保存到磁盘后再运行！');");
                    return;
                }

                try
                {
                    string docFolder = Path.GetDirectoryName(doc.FullPath);
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/k \"cd /d \"" + docFolder + "\" && \"" + doc.FullPath + "\"\"",
                        WorkingDirectory = docFolder,
                        UseShellExecute = true
                    };

                    if (isAdmin)
                    {
                        psi.Verb = "runas";
                    }

                    System.Diagnostics.Process.Start(psi);
                    string tip = isAdmin ? "已请求管理员提权并在控制台中启动运行！" : "已在当前脚本目录成功调起控制台运行！";
                    _previewForm?.ExecuteScript("if(window.showToast) window.showToast('" + tip + "');");
                }
                catch (System.ComponentModel.Win32Exception wEx)
                {
                    if (wEx.NativeErrorCode == 1223)
                    {
                        _previewForm?.ExecuteScript("if(window.showToast) window.showToast('已取消管理员提权启动。');");
                    }
                    else
                    {
                        string safeErr = wEx.Message.Replace("'", "\\'");
                        _previewForm?.ExecuteScript("if(window.showToast) window.showToast('启动批处理失败: " + safeErr + "');");
                    }
                }
                catch (Exception ex)
                {
                    string safeErr = ex.Message.Replace("'", "\\'");
                    _previewForm?.ExecuteScript("if(window.showToast) window.showToast('启动批处理失败: " + safeErr + "');");
                }
                return;
            }

            if (action.StartsWith("OPEN_EXTERNAL_URL:"))
            {
                string url = action.Substring("OPEN_EXTERNAL_URL:".Length);
                try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }); } catch { }
                return;
            }

            if (action.StartsWith("EXPORT_IMAGE:"))
            {
                string jsonPayload = action.Substring("EXPORT_IMAGE:".Length);
                try
                {
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                    var dict = serializer.Deserialize<Dictionary<string, object>>(jsonPayload);
                    string filename = (dict != null && dict.ContainsKey("filename")) ? dict["filename"]?.ToString() : "mermaid_diagram.png";
                    string dataUrl = (dict != null && dict.ContainsKey("data")) ? dict["data"]?.ToString() : "";

                    int commaIdx = dataUrl.IndexOf(',');
                    string base64 = commaIdx >= 0 ? dataUrl.Substring(commaIdx + 1) : dataUrl;
                    byte[] bytes = Convert.FromBase64String(base64);

                    if (_previewForm != null && !_previewForm.IsDisposed)
                    {
                        _previewForm.BeginInvoke((MethodInvoker)delegate
                        {
                            using (var sfd = new SaveFileDialog())
                            {
                                sfd.Title = "导出图片为 PNG";
                                sfd.Filter = "PNG 图片 (*.png)|*.png|所有文件 (*.*)|*.*";
                                sfd.FileName = filename;
                                if (sfd.ShowDialog(_previewForm) == DialogResult.OK)
                                {
                                    File.WriteAllBytes(sfd.FileName, bytes);
                                    string safeSavedName = Path.GetFileName(sfd.FileName).Replace("'", "\\'");
                                    _previewForm.ExecuteScript("if(window.showToast) window.showToast('已成功保存 PNG 到: " + safeSavedName + "');");
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    string safeErr = ex.Message.Replace("'", "\\'");
                    _previewForm?.ExecuteScript("if(window.showToast) window.showToast('导出图片失败: " + safeErr + "');");
                }
                return;
            }

            // ==================== 全屏与停靠切换动作处理 ====================
            if (action == "TOGGLE_FULLSCREEN")
            {
                _previewForm?.ToggleFullScreen();
                return;
            }
            if (action == "ENTER_FULLSCREEN")
            {
                _previewForm?.SetFullScreen(true);
                return;
            }
            if (action == "EXIT_FULLSCREEN")
            {
                _previewForm?.SetFullScreen(false);
                return;
            }

            string err;
            switch (action)
            {
                case "FORCE_MD": _forcedOverrides[doc.BufferId] = DocFormat.Markdown; break;
                case "FORCE_JSON": _forcedOverrides[doc.BufferId] = DocFormat.Json; break;
                case "FORCE_CSV": _forcedOverrides[doc.BufferId] = DocFormat.Csv; break;
                case "FORCE_SVG": _forcedOverrides[doc.BufferId] = DocFormat.Svg; break;
                case "FORCE_YAML": _forcedOverrides[doc.BufferId] = DocFormat.Yaml; break;
                case "FORCE_MERMAID": _forcedOverrides[doc.BufferId] = DocFormat.Mermaid; break;
                case "FORCE_HTML": _forcedOverrides[doc.BufferId] = DocFormat.Html; break;
                case "FORCE_XML": _forcedOverrides[doc.BufferId] = DocFormat.Xml; break;
                case "FORCE_CODE": _forcedOverrides[doc.BufferId] = DocFormat.Code; break;
                case "JSON_BEAUTIFY":
                    string bJson = FormatProcessor.BeautifyJson(doc.Content, out err);
                    if (err == null) BufferReader.ReplaceCurrentDocumentText(bJson, resetHScroll: true);
                    break;
                case "JSON_MINIFY":
                    string mJson = FormatProcessor.MinifyJson(doc.Content, out err);
                    if (err == null) BufferReader.ReplaceCurrentDocumentText(mJson, resetHScroll: true);
                    break;
                case "JSON_TO_CSV":
                    string csv = FormatProcessor.ConvertJsonToCsv(doc.Content, out err);
                    if (err == null) { _forcedOverrides[doc.BufferId] = DocFormat.Csv; BufferReader.ReplaceCurrentDocumentText(csv, resetHScroll: true); }
                    break;
                case "JSON_TO_MD":
                    string jMd = FormatProcessor.ConvertJsonToMarkdownTable(doc.Content, out err);
                    if (err == null) { _forcedOverrides[doc.BufferId] = DocFormat.Markdown; BufferReader.ReplaceCurrentDocumentText(jMd, resetHScroll: true); }
                    break;
                case "CSV_ALIGN":
                    BufferReader.ReplaceCurrentDocumentText(FormatProcessor.AlignCsvColumns(doc.Content), resetHScroll: true);
                    break;
                case "CSV_MINIFY":
                    BufferReader.ReplaceCurrentDocumentText(FormatProcessor.MinifyCsv(doc.Content), resetHScroll: true);
                    break;
                case "CSV_TO_JSON":
                    _forcedOverrides[doc.BufferId] = DocFormat.Json;
                    BufferReader.ReplaceCurrentDocumentText(FormatProcessor.ConvertCsvToJson(doc.Content), resetHScroll: true);
                    break;
                case "CSV_TO_MD":
                    _forcedOverrides[doc.BufferId] = DocFormat.Markdown;
                    BufferReader.ReplaceCurrentDocumentText(FormatProcessor.ConvertCsvToMarkdownTable(doc.Content), resetHScroll: true);
                    break;
                case "SVG_BEAUTIFY":
                    string fSvg = FormatProcessor.FormatSvg(doc.Content, out err);
                    if (err == null) BufferReader.ReplaceCurrentDocumentText(fSvg, resetHScroll: true);
                    break;
                case "XML_BEAUTIFY":
                    string fXml = FormatProcessor.FormatXml(doc.Content, out err);
                    if (err == null) BufferReader.ReplaceCurrentDocumentText(fXml, resetHScroll: true);
                    break;
                case "MD_TO_HTML":
                    BufferReader.ReplaceCurrentDocumentText(HtmlTemplateBuilder.ConvertMarkdownToHtml(doc.Content), resetHScroll: true);
                    break;
            }
            RefreshPreview();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RefreshPreview()
        {
            if (_previewForm == null || !_previewForm.Visible) return;
            DocumentContext doc = BufferReader.GetCurrentDocument();
            DetectionResult det = MultiFormatDetector.Evaluate(doc, _forcedOverrides);
            _previewForm.RenderDocument(doc, det);
        }
    }
}
