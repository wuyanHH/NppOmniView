using System;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using NppPlugin.OmniView.Core;

namespace NppPlugin.OmniView.Forms
{
    public class PreviewDockForm : Form
    {
        private WebView2 _webView;
        private bool _isWebViewReady = false;
        private string _currentMappedDocFolder = null;
        private DocFormat _lastRenderedFormat = DocFormat.Unknown;
        private readonly Action<string> _onWebActionCallback;
        private Action _pendingRenderAction = null;
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
        private int _lastSyncedLine = -1;
        private DateTime _lastSyncTime = DateTime.MinValue;

        // ==================== 全屏展示状态与停靠父句柄追踪 ====================
        private bool _isFullScreen = false;
        private IntPtr _originalDockParent = IntPtr.Zero;
        private int _originalStyle = 0;

        /// <summary>
        /// 当前是否处于独立屏幕全屏展示模式
        /// </summary>
        public bool IsFullScreen => _isFullScreen;

        public PreviewDockForm(Action<string> onWebActionCallback)
        {
            _onWebActionCallback = onWebActionCallback;
            this.Text = "NppOmniView 全能视图";
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            bool isDark = PluginBase.IsDarkModeEnabled();
            _webView = new WebView2
            {
                Dock = DockStyle.Fill,
                DefaultBackgroundColor = isDark ? System.Drawing.Color.FromArgb(30, 30, 30) : System.Drawing.Color.White
            };
            this.Controls.Add(_webView);
            InitializeWebViewAsync();
        }

        private async void InitializeWebViewAsync()
        {
            try
            {
                string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NppOmniView", "WebView2");
                if (!Directory.Exists(userDataFolder)) Directory.CreateDirectory(userDataFolder);
                var options = new CoreWebView2EnvironmentOptions(
                    "--enable-features=OverlayScrollbar,Windows11OverlayScrollbars",
                    null, null, false, null
                );
                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
                await _webView.EnsureCoreWebView2Async(env);
                _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;

                // 注册 WebResourceRequestedFilter 拦截所有 https://local.doc/* 本地资源请求，强制注入 CORS 响应头与 MIME 类型
                _webView.CoreWebView2.AddWebResourceRequestedFilter("https://local.doc/*", CoreWebView2WebResourceContext.All);
                _webView.CoreWebView2.WebResourceRequested += (s, args) =>
                {
                    try
                    {
                        string uri = args.Request.Uri;
                        string prefix = "https://local.doc/";
                        if (uri.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        {
                            string rel = uri.Substring(prefix.Length);
                            int q = rel.IndexOf('?');
                            if (q >= 0) rel = rel.Substring(0, q);
                            int hash = rel.IndexOf('#');
                            if (hash >= 0) rel = rel.Substring(0, hash);
                            rel = Uri.UnescapeDataString(rel).Replace('/', Path.DirectorySeparatorChar);

                            string targetFolder = _currentMappedDocFolder;
                            if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder))
                            {
                                string pluginDir = Path.GetDirectoryName(typeof(PreviewDockForm).Assembly.Location);
                                targetFolder = Path.Combine(pluginDir, "samples");
                            }

                            string filePath = null;
                            if (rel.StartsWith("__abs__" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                                rel.StartsWith("__abs__/", StringComparison.OrdinalIgnoreCase))
                            {
                                string absPart = rel.Substring(8);
                                int slashIdx = absPart.IndexOfAny(new char[] { '/', Path.DirectorySeparatorChar });
                                if (slashIdx > 0)
                                {
                                    string drive = absPart.Substring(0, slashIdx).TrimEnd(':');
                                    string sub = absPart.Substring(slashIdx + 1);
                                    filePath = drive + ":" + Path.DirectorySeparatorChar + sub.Replace('/', Path.DirectorySeparatorChar);
                                }
                            }
                            else
                            {
                                filePath = Path.Combine(targetFolder, rel);
                                if (!File.Exists(filePath))
                                {
                                    try
                                    {
                                        var parentDir = Directory.GetParent(targetFolder);
                                        if (parentDir != null && parentDir.Exists)
                                        {
                                            string pPath = Path.Combine(parentDir.FullName, rel);
                                            if (File.Exists(pPath)) filePath = pPath;
                                        }
                                    }
                                    catch { }
                                }
                                if (!File.Exists(filePath))
                                {
                                    try
                                    {
                                        string pluginDir = Path.GetDirectoryName(typeof(PreviewDockForm).Assembly.Location);
                                        string fallback = Path.Combine(pluginDir, "samples", rel);
                                        if (File.Exists(fallback)) filePath = fallback;
                                    }
                                    catch { }
                                }
                            }

                            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                            {
                                string ext = Path.GetExtension(filePath).ToLowerInvariant();
                                string mime = "application/octet-stream";
                                switch (ext)
                                {
                                    case ".svg": mime = "image/svg+xml"; break;
                                    case ".png": mime = "image/png"; break;
                                    case ".jpg":
                                    case ".jpeg": mime = "image/jpeg"; break;
                                    case ".gif": mime = "image/gif"; break;
                                    case ".webp": mime = "image/webp"; break;
                                    case ".bmp": mime = "image/bmp"; break;
                                    case ".ico": mime = "image/x-icon"; break;
                                    case ".css": mime = "text/css"; break;
                                    case ".js": mime = "text/javascript"; break;
                                    case ".json": mime = "application/json"; break;
                                    case ".xml": mime = "application/xml"; break;
                                }

                                var fs = File.OpenRead(filePath);
                                args.Response = _webView.CoreWebView2.Environment.CreateWebResourceResponse(
                                    fs,
                                    200,
                                    "OK",
                                    "Content-Type: " + mime + "\r\nAccess-Control-Allow-Origin: *\r\nAccess-Control-Allow-Methods: GET, OPTIONS\r\nCache-Control: no-cache\r\n"
                                );
                            }
                        }
                    }
                    catch { }
                };

                _webView.CoreWebView2.NavigationStarting += (s, e) =>
                {
                    if (string.IsNullOrEmpty(e.Uri)) return;
                    if (e.Uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        e.Uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!e.Uri.StartsWith("https://local.doc/", StringComparison.OrdinalIgnoreCase))
                        {
                            e.Cancel = true;
                            try { Process.Start(new ProcessStartInfo(e.Uri) { UseShellExecute = true }); } catch { }
                        }
                    }
                };
                _webView.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    string act = e.TryGetWebMessageAsString();
                    if (!string.IsNullOrEmpty(act)) _onWebActionCallback?.Invoke(act);
                };
                _isWebViewReady = true;
                _pendingRenderAction?.Invoke();
                _pendingRenderAction = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("WebView2 Init Error: " + ex.Message, "NppOmniView", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void RenderDocument(DocumentContext doc, DetectionResult det)
        {
            if (!_isWebViewReady) { _pendingRenderAction = () => RenderDocument(doc, det); return; }

            bool isDarkMode = PluginBase.IsDarkModeEnabled();
            string statusTag = doc.IsSavedOnDisk ? "已保存" : "未保存缓冲区";
            string badgeText = det.Format.ToString().ToUpperInvariant() + " | " + statusTag;

            // Handle large files gracefully
            if (doc.IsLargeFile)
            {
                badgeText += " (大文件预览模式)";
            }

            string docFolder = "";
            if (doc.IsSavedOnDisk && !string.IsNullOrWhiteSpace(doc.FullPath))
            {
                try { docFolder = Path.GetDirectoryName(doc.FullPath); } catch { }
            }
            if (string.IsNullOrEmpty(docFolder) || !Directory.Exists(docFolder))
            {
                try
                {
                    string pluginDir = Path.GetDirectoryName(typeof(PreviewDockForm).Assembly.Location);
                    string sampleDir = Path.Combine(pluginDir, "samples");
                    if (Directory.Exists(sampleDir)) docFolder = sampleDir;
                }
                catch { }
            }

            string baseHref = "https://local.doc/";
            if (!string.IsNullOrEmpty(docFolder) && Directory.Exists(docFolder))
            {
                if (!string.Equals(_currentMappedDocFolder, docFolder, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        _webView.CoreWebView2.SetVirtualHostNameToFolderMapping("local.doc", docFolder, CoreWebView2HostResourceAccessKind.Allow);
                    }
                    catch { }
                    _currentMappedDocFolder = docFolder;
                }
            }

            // Zero-flicker Incremental DOM Update
            // If format hasn't changed and is recognized, update inner container directly without destroying DOM/scroll
            if (_lastRenderedFormat == det.Format && det.Format != DocFormat.Unknown)
            {
                if (det.Format == DocFormat.Markdown)
                {
                    string innerHtml = HtmlTemplateBuilder.ConvertMarkdownToHtml(doc.Content);
                    string script = "if(window.updateMarkdownContent) window.updateMarkdownContent(" +
                                    Serializer.Serialize(innerHtml) + ", " +
                                    Serializer.Serialize(badgeText) + ", " +
                                    Serializer.Serialize(doc.Content ?? "") + ", " +
                                    Serializer.Serialize(baseHref) + "); if(window.finishRecognition) window.finishRecognition(" + Serializer.Serialize(badgeText) + "); else if(window.finishLoader) window.finishLoader();";
                    _webView.CoreWebView2.ExecuteScriptAsync(script);
                    return;
                }
                else if (det.Format == DocFormat.Json)
                {
                    string script = "if(window.updateJsonContent) window.updateJsonContent(" +
                                    Serializer.Serialize(doc.Content ?? "") + ", " +
                                    Serializer.Serialize(badgeText) + "); if(window.finishRecognition) window.finishRecognition(" + Serializer.Serialize(badgeText) + "); else if(window.finishLoader) window.finishLoader();";
                    _webView.CoreWebView2.ExecuteScriptAsync(script);
                    return;
                }
                else if (det.Format == DocFormat.Csv)
                {
                    char delim = FormatProcessor.DetectCsvDelimiter(doc.Content);
                    var rows = FormatProcessor.ParseCsv(doc.Content, delim);
                    string script = "if(window.updateCsvContent) window.updateCsvContent(" +
                                    Serializer.Serialize(rows) + ", " +
                                    Serializer.Serialize(badgeText) + ", " +
                                    Serializer.Serialize(doc.Content ?? "") + ", " +
                                    Serializer.Serialize(delim.ToString()) + "); if(window.finishRecognition) window.finishRecognition(" + Serializer.Serialize(badgeText) + "); else if(window.finishLoader) window.finishLoader();";
                    _webView.CoreWebView2.ExecuteScriptAsync(script);
                    return;
                }
                else if (det.Format == DocFormat.Svg)
                {
                    string script = "if(window.updateSvgContent) window.updateSvgContent(" +
                                    Serializer.Serialize(doc.Content ?? "") + ", " +
                                    Serializer.Serialize(badgeText) + "); if(window.finishRecognition) window.finishRecognition(" + Serializer.Serialize(badgeText) + "); else if(window.finishLoader) window.finishLoader();";
                    _webView.CoreWebView2.ExecuteScriptAsync(script);
                    return;
                }
                else if (det.Format == DocFormat.Yaml)
                {
                    string script = "if(window.updateYamlContent) window.updateYamlContent(" +
                                    Serializer.Serialize(doc.Content ?? "") + ", " +
                                    Serializer.Serialize(badgeText) + "); if(window.finishRecognition) window.finishRecognition(" + Serializer.Serialize(badgeText) + "); else if(window.finishLoader) window.finishLoader();";
                    _webView.CoreWebView2.ExecuteScriptAsync(script);
                    return;
                }
                else if (det.Format == DocFormat.Mermaid)
                {
                    string script = "if(window.updateMermaidContent) window.updateMermaidContent(" +
                                    Serializer.Serialize(doc.Content ?? "") + ", " +
                                    Serializer.Serialize(badgeText) + "); if(window.finishRecognition) window.finishRecognition(" + Serializer.Serialize(badgeText) + "); else if(window.finishLoader) window.finishLoader();";
                    _webView.CoreWebView2.ExecuteScriptAsync(script);
                    return;
                }
                else if (det.Format == DocFormat.Html)
                {
                    string script = "if(window.updateHtmlContent) window.updateHtmlContent(" +
                                    Serializer.Serialize(doc.Content ?? "") + ", " +
                                    Serializer.Serialize(badgeText) + "); if(window.finishRecognition) window.finishRecognition(" + Serializer.Serialize(badgeText) + "); else if(window.finishLoader) window.finishLoader();";
                    _webView.CoreWebView2.ExecuteScriptAsync(script);
                    return;
                }
                else if (det.Format == DocFormat.Xml)
                {
                    string script = "if(window.updateXmlContent) window.updateXmlContent(" +
                                    Serializer.Serialize(doc.Content ?? "") + ", " +
                                    Serializer.Serialize(badgeText) + "); if(window.finishRecognition) window.finishRecognition(" + Serializer.Serialize(badgeText) + "); else if(window.finishLoader) window.finishLoader();";
                    _webView.CoreWebView2.ExecuteScriptAsync(script);
                    return;
                }
                else if (det.Format == DocFormat.Code)
                {
                    string lang = MultiFormatDetector.GetLanguageForExtension(doc.Extension);
                    string script = "if(window.updateCodeContent) window.updateCodeContent(" +
                                    Serializer.Serialize(doc.Content ?? "") + ", " +
                                    Serializer.Serialize(badgeText) + ", " +
                                    Serializer.Serialize(lang) + "); if(window.finishRecognition) window.finishRecognition(" + Serializer.Serialize(badgeText) + "); else if(window.finishLoader) window.finishLoader();";
                    _webView.CoreWebView2.ExecuteScriptAsync(script);
                    return;
                }
            }

            // Full document build (format changed or initial load)
            _lastRenderedFormat = det.Format;
            string html = HtmlTemplateBuilder.BuildOmniViewHtml(doc, det, baseHref, isDarkMode);
            _webView.NavigateToString(html);
        }

        public void NotifyTypingPending()
        {
            if (!_isWebViewReady) return;
            _webView.CoreWebView2?.ExecuteScriptAsync("if(window.showGlobalLoading) window.showGlobalLoading('检测到内容输入，正在实时渲染...'); else if(window.startRecognition) window.startRecognition('检测到输入，智能解析中...'); else if(window.startLoader) window.startLoader();");
        }

        public void NotifyLoadingStarted(string hint = "正在读取与加载文档...")
        {
            if (!_isWebViewReady) return;
            string script = "if(window.showGlobalLoading) window.showGlobalLoading(" + Serializer.Serialize(hint) + "); else if(window.startLoader) window.startLoader();";
            _webView.CoreWebView2?.ExecuteScriptAsync(script);
        }

        public void UpdateTheme(bool isDarkMode)
        {
            _webView.DefaultBackgroundColor = isDarkMode ? System.Drawing.Color.FromArgb(30, 30, 30) : System.Drawing.Color.White;
            if (!_isWebViewReady) return;
            string script = "document.body.classList.toggle('dark-mode', " + (isDarkMode ? "true" : "false") + ");";
            _webView.CoreWebView2?.ExecuteScriptAsync(script);
        }

        public void SyncScrollToLine(int firstVisibleDocLine, int totalDocLines)
        {
            if (!_isWebViewReady) return;

            // Throttle scroll sync IPC messages
            DateTime now = DateTime.UtcNow;
            if (firstVisibleDocLine == _lastSyncedLine || (now - _lastSyncTime).TotalMilliseconds < 25)
                return;

            _lastSyncedLine = firstVisibleDocLine;
            _lastSyncTime = now;

            _webView.CoreWebView2.ExecuteScriptAsync("if(window.syncScrollToLine) window.syncScrollToLine(" + firstVisibleDocLine + ", " + totalDocLines + ");");
        }

        public void ExecuteScript(string script)
        {
            if (!_isWebViewReady || _webView == null || _webView.CoreWebView2 == null) return;
            _webView.CoreWebView2.ExecuteScriptAsync(script);
        }

        /// <summary>
        /// 切换物理屏幕全屏模式与停靠窗口模式
        /// </summary>
        public void ToggleFullScreen()
        {
            SetFullScreen(!_isFullScreen);
        }

        /// <summary>
        /// 设置物理屏幕真全屏展示模式
        /// </summary>
        public void SetFullScreen(bool fullScreen)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => SetFullScreen(fullScreen)));
                return;
            }

            if (_isFullScreen == fullScreen) return;

            if (fullScreen)
            {
                // 1. 记录原始停靠容器父句柄和样式
                IntPtr curParent = Win32.GetParent(this.Handle);
                if (curParent != IntPtr.Zero)
                {
                    _originalDockParent = curParent;
                }
                _originalStyle = Win32.GetWindowLong(this.Handle, Win32.GWL_STYLE);

                _isFullScreen = true;

                // 2. 脱离 Notepad++ 停靠面板，提升为桌面顶层无边框弹出窗口
                Win32.SetParent(this.Handle, IntPtr.Zero);

                int newStyle = _originalStyle;
                newStyle &= ~Win32.WS_CHILD;
                newStyle |= Win32.WS_POPUP;
                Win32.SetWindowLong(this.Handle, Win32.GWL_STYLE, newStyle);

                // 3. 获取 Notepad++ 主窗口或当前控件所在的物理显示器尺寸
                Screen screen = Screen.FromHandle(this.Handle);
                if (screen == null && PluginBase.nppData._nppHandle != IntPtr.Zero)
                {
                    screen = Screen.FromHandle(PluginBase.nppData._nppHandle);
                }
                if (screen == null) screen = Screen.PrimaryScreen;

                // 4. 铺满整个显示器物理屏幕并激活聚焦
                Win32.SetWindowPos(this.Handle, Win32.HWND_TOP,
                    screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height,
                    Win32.SWP_SHOWWINDOW | Win32.SWP_FRAMECHANGED);

                this.BringToFront();
                this.Activate();
                this.Focus();

                // 5. 通知 WebView2 重新适应全屏视口并同步 UI 状态
                _webView?.Invalidate();
                _webView?.CoreWebView2?.ExecuteScriptAsync("if(window.syncFullScreenState) window.syncFullScreenState(true);");
            }
            else
            {
                _isFullScreen = false;

                // 1. 恢复原始窗口样式
                Win32.SetWindowLong(this.Handle, Win32.GWL_STYLE, _originalStyle);

                // 2. 重新归属回 Notepad++ 原始停靠容器
                if (_originalDockParent != IntPtr.Zero)
                {
                    Win32.SetParent(this.Handle, _originalDockParent);

                    Win32.RECT rc;
                    if (Win32.GetClientRect(_originalDockParent, out rc))
                    {
                        Win32.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, rc.Right - rc.Left, rc.Bottom - rc.Top,
                            Win32.SWP_NOZORDER | Win32.SWP_SHOWWINDOW | Win32.SWP_FRAMECHANGED);
                    }
                    Win32.PostMessage(_originalDockParent, Win32.WM_SIZE, IntPtr.Zero, IntPtr.Zero);
                }

                // 3. 通知 WebView2 重新校准停靠视口并同步 UI 状态
                _webView?.Invalidate();
                _webView?.CoreWebView2?.ExecuteScriptAsync("if(window.syncFullScreenState) window.syncFullScreenState(false);");
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_isFullScreen)
            {
                if (keyData == Keys.Escape || keyData == Keys.F11 || keyData == (Keys.Alt | Keys.Enter))
                {
                    SetFullScreen(false);
                    return true;
                }
            }
            else
            {
                if (keyData == Keys.F11 || keyData == (Keys.Alt | Keys.Enter))
                {
                    SetFullScreen(true);
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isFullScreen)
            {
                SetFullScreen(false);
            }
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_isFullScreen)
                {
                    SetFullScreen(false);
                }
                _webView?.Dispose();
                _webView = null;
            }
            base.Dispose(disposing);
        }
    }
}
