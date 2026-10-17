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
        public const string PluginVersion = "1.0.0";

        private static PreviewDockForm _previewForm = null;
        private static Timer _debounceTimer;
        private static readonly Dictionary<IntPtr, DocFormat> _forcedOverrides = new Dictionary<IntPtr, DocFormat>();
        private static IntPtr _ptrPluginName = IntPtr.Zero;
        private static IntPtr _ptrFuncItems = IntPtr.Zero;
        private static NppFuncItemDelegate _toggleDelegate;
        private static NppFuncItemDelegate _formatDelegate;
        private static NppFuncItemDelegate _aboutDelegate;
        private static IntPtr _ptrTbIcons = IntPtr.Zero;
        private static bool _resolverRegistered = false;

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
            nbF = 3;
            if (_ptrFuncItems == IntPtr.Zero)
            {
                _toggleDelegate = TogglePreviewPanel;
                _formatDelegate = SmartFormatCurrentBuffer;
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
                    _itemName = "\u5173\u4E8E NppOmniView (About)...",
                    _pFunc = Marshal.GetFunctionPointerForDelegate(_aboutDelegate),
                    _cmdID = 2,
                    _init2Check = false,
                    _pShKey = IntPtr.Zero
                };

                int itemSize = Marshal.SizeOf(typeof(FuncItem));
                _ptrFuncItems = Marshal.AllocHGlobal(itemSize * 3);
                Marshal.StructureToPtr(item0, _ptrFuncItems, false);
                Marshal.StructureToPtr(item1, new IntPtr(_ptrFuncItems.ToInt64() + itemSize), false);
                Marshal.StructureToPtr(item2, new IntPtr(_ptrFuncItems.ToInt64() + itemSize * 2), false);
            }
            return _ptrFuncItems;
        }

        public static void beNotified(IntPtr notifyCode)
        {
            // P0: Prevent crash on null pointer dereference
            if (notifyCode == IntPtr.Zero) return;

            ScNotification notification = (ScNotification)Marshal.PtrToStructure(notifyCode, typeof(ScNotification));
            uint code = notification.Header.code;

            if (code == (uint)NppMsg.NPPN_TBMODIFICATION)
            {
                RegisterToolbarIcon();
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

            if (_previewForm == null || !_previewForm.Visible) return;

            if (code == (uint)NppMsg.NPPN_BUFFERACTIVATED || code == (uint)NppMsg.NPPN_FILESAVED)
            {
                _previewForm?.NotifyLoadingStarted("正在读取并加载文档视图...");
                RefreshPreview();
            }
            else if (code == (uint)SciMsg.SCN_MODIFIED)
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
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, _previewForm.Handle);
                }
                else
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, _previewForm.Handle);
                    RefreshPreview();
                }
            }
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
