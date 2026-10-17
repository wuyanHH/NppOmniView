using System;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;

namespace NppPlugin.OmniView.Core
{
    public static class HtmlTemplateBuilder
    {
        private static readonly MarkdownPipeline MdPipeline = new MarkdownPipelineBuilder()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
            .UseAdvancedExtensions()
            .UsePragmaLines()
            .Build();

        private static readonly JavaScriptSerializer JsonSerializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };

        public static string ConvertMarkdownToHtml(string md) => Markdown.ToHtml(md ?? string.Empty, MdPipeline);

        private static string SafeEmbedJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return "\"\"";
            return json.Replace("</", "<\\/").Replace("<!--", "<\\!--");
        }

        // ==================== VECTOR SVG ICONS (Crisp, Lucide / Feather Style) ====================
        private const string IconChevronDown = "<svg class='chevron-icon' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'><polyline points='6 9 12 15 18 9'/></svg>";
        private const string IconJson = "<svg width='15' height='15' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M8 3H7a2 2 0 0 0-2 2v5a2 2 0 0 1-2 2 2 2 0 0 1 2 2v5c0 1.1.9 2 2 2h1'/><path d='M16 21h1a2 2 0 0 0 2-2v-5c0-1.1.9-2 2-2a2 2 0 0 1-2-2V5a2 2 0 0 0-2-2h-1'/></svg>";
        private const string IconCsv = "<svg width='15' height='15' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><rect width='18' height='18' x='3' y='3' rx='2'/><path d='M3 9h18'/><path d='M3 15h18'/><path d='M9 3v18'/><path d='M15 3v18'/></svg>";
        private const string IconMarkdown = "<svg width='15' height='15' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z'/><polyline points='14 2 14 8 20 8'/><line x1='16' y1='13' x2='8' y2='13'/><line x1='16' y1='17' x2='8' y2='17'/><polyline points='10 9 9 9 8 9'/></svg>";
        private const string IconHtml = "<svg width='15' height='15' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><circle cx='12' cy='12' r='10'/><line x1='2' y1='12' x2='22' y2='12'/><path d='M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z'/></svg>";
        private const string IconCode = "<svg width='15' height='15' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polyline points='16 18 22 12 16 6'/><polyline points='8 6 2 12 8 18'/></svg>";
        private const string IconSparkles = "<svg class='btn-icon' width='13' height='13' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='m12 3-1.9 5.8a2 2 0 0 1-1.3 1.3L3 12l5.8 1.9a2 2 0 0 1 1.3 1.3L12 21l1.9-5.8a2 2 0 0 1 1.3-1.3L21 12l-5.8-1.9a2 2 0 0 1-1.3-1.3Z'/></svg>";
        private const string IconCompress = "<svg class='btn-icon' width='13' height='13' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M4 14h6v6'/><path d='M20 10h-6V4'/><path d='m14 10 7-7'/><path d='m3 21 7-7'/></svg>";
        private const string IconAlign = "<svg class='btn-icon' width='13' height='13' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><line x1='21' x2='3' y1='6' y2='6'/><line x1='21' x2='3' y1='12' y2='12'/><line x1='21' x2='3' y1='18' y2='18'/></svg>";
        private const string IconExport = "<svg class='btn-icon' width='13' height='13' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4'/><polyline points='7 10 12 15 17 10'/><line x1='12' x2='12' y1='15' y2='3'/></svg>";
        private const string IconEmptyDoc = "<svg width='48' height='48' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='1.5' stroke-linecap='round' stroke-linejoin='round'><path d='M14.5 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7.5L14.5 2z'/><polyline points='14 2 14 8 20 8'/><line x1='9' y1='13' x2='15' y2='13'/><line x1='9' y1='17' x2='13' y2='17'/></svg>";
        private const string IconTag = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M12 2H2v10l9.29 9.29c.94.94 2.48.94 3.42 0l6.58-6.58c.94-.94.94-2.48 0-3.42L12 2Z'/><path d='M7 7h.01'/></svg>";
        private const string IconExpand = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polyline points='15 3 21 3 21 9'/><polyline points='9 21 3 21 3 15'/><line x1='21' x2='14' y1='3' y2='10'/><line x1='3' x2='10' y1='21' y2='14'/></svg>";
        private const string IconCollapse = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polyline points='4 14 10 14 10 20'/><polyline points='20 10 14 10 14 4'/><line x1='14' x2='21' y1='10' y2='3'/><line x1='3' x2='10' y1='21' y2='14'/></svg>";
        private const string IconCopy = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><rect x='9' y='9' width='13' height='13' rx='2'></rect><path d='M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1'></path></svg>";
        private const string IconSave = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.2' stroke-linecap='round' stroke-linejoin='round'><path d='M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z'/><polyline points='17 21 17 13 7 13 7 21'/><polyline points='7 3 7 8 15 8'/></svg>";
        private const string IconSvg = "<svg width='15' height='15' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M12 19l7-7 3 3-7 7-3-3z'/><path d='M18 13l-1.5-7.5L2 2l3.5 14.5L13 18l5-5z'/><path d='M2 2l7.586 7.586'/><circle cx='11' cy='11' r='2'/></svg>";
        private const string IconYaml = "<svg width='15' height='15' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><line x1='4' x2='20' y1='21' y2='21'/><line x1='4' x2='20' y1='3' y2='3'/><line x1='4' x2='20' y1='12' y2='12'/><line x1='12' x2='12' y1='3' y2='21'/></svg>";
        private const string IconMermaid = "<svg width='15' height='15' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><circle cx='18' cy='5' r='3'/><circle cx='6' cy='12' r='3'/><circle cx='18' cy='19' r='3'/><line x1='8.59' x2='15.42' y1='13.51' y2='17.49'/><line x1='15.41' x2='8.59' y1='6.51' y2='10.49'/></svg>";
        private const string IconZoomIn = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><circle cx='11' cy='11' r='8'/><line x1='21' x2='16.65' y1='21' y2='16.65'/><line x1='11' x2='11' y1='8' y2='14'/><line x1='8' x2='14' y1='11' y2='11'/></svg>";
        private const string IconZoomOut = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><circle cx='11' cy='11' r='8'/><line x1='21' x2='16.65' y1='21' y2='16.65'/><line x1='8' x2='14' y1='11' y2='11'/></svg>";
        private const string IconReset = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8'/><path d='M3 3v5h5'/></svg>";
        private const string IconGrid = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><rect width='7' height='7' x='3' y='3' rx='1'/><rect width='7' height='7' x='14' y='3' rx='1'/><rect width='7' height='7' x='14' y='14' rx='1'/><rect width='7' height='7' x='3' y='14' rx='1'/></svg>";
        private const string IconDeviceDesktop = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><rect width='20' height='14' x='2' y='3' rx='2'/><line x1='8' x2='16' y1='21' y2='21'/><line x1='12' x2='12' y1='17' y2='21'/></svg>";
        private const string IconDeviceTablet = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><rect width='16' height='20' x='4' y='2' rx='2'/><line x1='12' x2='12.01' y1='18' y2='18'/></svg>";
        private const string IconDeviceMobile = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><rect width='14' height='20' x='5' y='2' rx='2'/><line x1='12' x2='12.01' y1='18' y2='18'/></svg>";
        private const string IconEdit = "<svg class='btn-icon' width='13' height='13' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5Z'/><path d='m15 5 4 4'/></svg>";
        private const string IconPlus = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.2' stroke-linecap='round' stroke-linejoin='round'><line x1='12' y1='5' x2='12' y2='19'/><line x1='5' y1='12' x2='19' y2='12'/></svg>";
        private const string IconTrash = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polyline points='3 6 5 6 21 6'/><path d='M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2'/></svg>";
        private const string IconCheck = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'><polyline points='20 6 9 17 4 12'/></svg>";
        private const string IconClose = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'><line x1='18' y1='6' x2='6' y2='18'/><line x1='6' y1='6' x2='18' y2='18'/></svg>";
        private const string IconReplace = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='m17 2 4 4-4 4'/><path d='M3 11v-1a4 4 0 0 1 4-4h14'/><path d='m7 22-4-4 4-4'/><path d='M21 13v1a4 4 0 0 1-4 4H3'/></svg>";
        private const string IconRotate = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M21 12a9 9 0 0 0-9-9 9.75 9.75 0 0 0-6.74 2.74L3 8'/><path d='M3 3v5h5'/><path d='M3 12a9 9 0 0 0 9 9 9.75 9.75 0 0 0 6.74-2.74L21 16'/><path d='M16 16h5v5'/></svg>";
        private const string IconMaximize = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M8 3H5a2 2 0 0 0-2 2v3m18 0V5a2 2 0 0 0-2-2h-3m0 18h3a2 2 0 0 0 2-2v-3M3 16v3a2 2 0 0 0 2 2h3'/></svg>";
        private const string IconMinimize = "<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M8 3v3a2 2 0 0 1-2 2H3m18 0h-3a2 2 0 0 1-2-2V3m0 18v-3a2 2 0 0 1 2-2h3M3 16h3a2 2 0 0 1 2 2v3'/></svg>";
        private const string IconSearch = "<svg class='search-icon' width='13' height='13' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><circle cx='11' cy='11' r='8'/><path d='m21 21-4.35-4.35'/></svg>";
        private const string IconBook = "<svg class='btn-icon' width='13' height='13' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M4 19.5A2.5 2.5 0 0 1 6.5 17H20'/><path d='M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z'/></svg>";
        private const string IconList = "<svg class='btn-icon' width='13' height='13' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><line x1='8' y1='6' x2='21' y2='6'/><line x1='8' y1='12' x2='21' y2='12'/><line x1='8' y1='18' x2='21' y2='18'/><line x1='3' y1='6' x2='3.01' y2='6'/><line x1='3' y1='12' x2='3.01' y2='12'/><line x1='3' y1='18' x2='3.01' y2='18'/></svg>";
        private const string IconShield = "<svg class='btn-icon' width='13' height='13' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z'/></svg>";
        private const string IconTerminal = "<svg class='btn-icon' width='13' height='13' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polyline points='4 17 10 11 4 5'/><line x1='12' y1='19' x2='20' y2='19'/></svg>";
        private const string IconLightbulb = "<svg class='tip-svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M15 14c.2-1 .7-1.7 1.5-2.5 1-.9 1.5-2.2 1.5-3.5A6 6 0 0 0 6 8c0 1 .2 2.2 1.5 3.5.7.7 1.3 1.5 1.5 2.5'/><path d='M9 18h6'/><path d='M10 22h4'/></svg>";

        private static string BuildFullscreenButton()
        {
            return @"<button class='btn btn-zen-fullscreen' id='btn-zen-fullscreen' onclick='toggleZenFullscreen()' title='全屏沉浸展示 (快捷键 F11 / Alt+Enter)'>" + IconMaximize + @"<span>全屏</span></button>";
        }

        private static string BuildEditButton(DocFormat fmt)
        {
            string label = "编辑源码";
            if (fmt == DocFormat.Markdown) label = "编辑文档";
            else if (fmt == DocFormat.Csv) label = "编辑表格";
            else if (fmt == DocFormat.Code) label = "编辑代码";
            return @"<button class='btn btn-edit-toggle primary' id='btn-enter-edit' onclick='enterEditMode()' title='在当前视口直接编辑 (快捷键 F2)'>" + IconEdit + @"<span>" + label + @"</span></button>";
        }


        private static string BuildDropdownItem(string act, string iconCls, string iconSvg, string title, string subtitle, string badgeText)
        {
            return @"
                <button class='dropdown-item' onclick=""postAct('" + act + @"')"">
                    <span class='item-icon " + iconCls + @"'>" + iconSvg + @"</span>
                    <div class='item-content'>
                        <span class='item-title'>" + title + @"</span>
                        <span class='item-subtitle'>" + subtitle + @"</span>
                    </div>
                    <span class='item-action-tag'>" + badgeText + @"</span>
                </button>";
        }

        private static string BuildDropdownHeader(string title) => "<div class='dropdown-header'>" + title + "</div>";
        private const string DropdownDivider = "<div class='dropdown-divider'></div>";

        private static string BuildPalettePopoverHtml(string prefix)
        {
            string defaultColor = prefix == "svg" ? "none" : "#ffffff";
            string defaultName = prefix == "svg" ? "不显示" : "白色";
            string initialSwatchCls = prefix == "svg" ? "color-swatch-badge swatch-none" : "color-swatch-badge";
            string initialSwatchStyle = prefix == "svg" ? "" : "style='background:#ffffff;'";

            return @"
                <div class='palette-dropdown-wrap' id='" + prefix + @"-palette-wrap'>
                    <button class='btn-pill' id='btn-" + prefix + @"-bg' onclick='togglePalettePopover(""" + prefix + @""", event)' title='打开调色盘选择画布背景'>
                        <span class='" + initialSwatchCls + @"' id='" + prefix + @"-bg-indicator' " + initialSwatchStyle + @"></span>
                        <span id='" + prefix + @"-bg-text'>背景: " + defaultName + @"</span>
                        " + IconChevronDown + @"
                    </button>
                    <div class='palette-popover' id='" + prefix + @"-palette-popover' onclick='event.stopPropagation()'>
                        <div class='palette-header-row'>
                            <span class='palette-header'>预设背景颜色</span>
                            <button class='btn-palette-close' onclick='closeAllPalettePopovers()' title='关闭调色盘 (Esc)'>" + IconClose + @"</button>
                        </div>
                        <div class='palette-grid'>
                            <button class='palette-chip' data-color='none' onclick='setCanvasBg(""" + prefix + @""", ""none"", ""不显示"", true)' title='透明无底色 (棋盘格)'>
                                <span class='color-swatch-badge swatch-none'></span>
                                <span>不显示</span>
                            </button>
                            <button class='palette-chip' data-color='#ffffff' onclick='setCanvasBg(""" + prefix + @""", ""#ffffff"", ""白色"", true)' title='纯白 (#ffffff)'>
                                <span class='color-swatch-badge' style='background:#ffffff;'></span>
                                <span>白色</span>
                            </button>
                            <button class='palette-chip' data-color='#f8fafc' onclick='setCanvasBg(""" + prefix + @""", ""#f8fafc"", ""浅灰"", true)' title='柔和浅灰 (#f8fafc)'>
                                <span class='color-swatch-badge' style='background:#f8fafc;'></span>
                                <span>浅灰</span>
                            </button>
                            <button class='palette-chip' data-color='#faf8f5' onclick='setCanvasBg(""" + prefix + @""", ""#faf8f5"", ""暖米"", true)' title='温润纸张 (#faf8f5)'>
                                <span class='color-swatch-badge' style='background:#faf8f5;'></span>
                                <span>暖米</span>
                            </button>
                            <button class='palette-chip' data-color='#1e293b' onclick='setCanvasBg(""" + prefix + @""", ""#1e293b"", ""蓝灰"", true)' title='深蓝灰 (#1e293b)'>
                                <span class='color-swatch-badge' style='background:#1e293b;'></span>
                                <span>蓝灰</span>
                            </button>
                            <button class='palette-chip' data-color='#121214' onclick='setCanvasBg(""" + prefix + @""", ""#121214"", ""暗黑"", true)' title='暗夜黑 (#121214)'>
                                <span class='color-swatch-badge' style='background:#121214;'></span>
                                <span>暗黑</span>
                            </button>
                        </div>
                        <div class='palette-divider'></div>
                        <div class='palette-custom-row'>
                            <span class='palette-custom-label'>自定义调色盘:</span>
                            <div class='palette-custom-pickers'>
                                <div class='native-color-wrap' title='点击打开系统调色盘'>
                                    <input type='color' id='" + prefix + @"-custom-color' value='#ffffff' oninput='onCanvasCustomColorInput(""" + prefix + @""", this.value)' onchange='onCanvasCustomColorChange(""" + prefix + @""", this.value)' />
                                </div>
                                <input type='text' id='" + prefix + @"-custom-hex' class='hex-input' value='#FFFFFF' maxlength='7' onchange='onCanvasHexChange(""" + prefix + @""", this.value)' placeholder='#HEX' title='输入 HEX 颜色代码' />
                            </div>
                        </div>
                    </div>
                </div>";
        }

        private static string BuildUnifiedViewSwitchGroup(DocFormat currentFmt)
        {
            var sb = new StringBuilder();
            sb.Append(BuildDropdownHeader("视图模式切换"));
            if (currentFmt != DocFormat.Markdown)
                sb.Append(BuildDropdownItem("FORCE_MD", "icon-md", IconMarkdown, "切换为 Markdown 预览", "解析富文本排版与嵌入图表", "视图"));
            if (currentFmt != DocFormat.Json)
                sb.Append(BuildDropdownItem("FORCE_JSON", "icon-json", IconJson, "切换为 JSON 预览", "交互式折叠语法树与类型标记", "视图"));
            if (currentFmt != DocFormat.Csv)
                sb.Append(BuildDropdownItem("FORCE_CSV", "icon-csv", IconCsv, "切换为 CSV 预览", "二维数据网格与单元格编辑", "视图"));
            if (currentFmt != DocFormat.Svg)
                sb.Append(BuildDropdownItem("FORCE_SVG", "icon-svg", IconSvg, "切换为 SVG 预览", "矢量图形渲染、缩放与转图", "视图"));
            if (currentFmt != DocFormat.Yaml)
                sb.Append(BuildDropdownItem("FORCE_YAML", "icon-yaml", IconYaml, "切换为 YAML 预览", "结构化配置大纲树与语法校验", "视图"));
            if (currentFmt != DocFormat.Mermaid)
                sb.Append(BuildDropdownItem("FORCE_MERMAID", "icon-mermaid", IconMermaid, "切换为 Mermaid 预览", "架构图、流程图与时序图渲染", "视图"));
            if (currentFmt != DocFormat.Html)
                sb.Append(BuildDropdownItem("FORCE_HTML", "icon-html", IconHtml, "切换为 HTML 预览", "网页与响应式视口沙箱预览", "视图"));
            if (currentFmt != DocFormat.Xml)
                sb.Append(BuildDropdownItem("FORCE_XML", "icon-xml", IconCode, "切换为 XML 树状预览", "交互式 XML DOM 节点树与属性大纲", "视图"));
            if (currentFmt != DocFormat.Code)
                sb.Append(BuildDropdownItem("FORCE_CODE", "icon-code", IconCode, "切换为 源码视图", "代码语法高亮与行号对齐", "视图"));
            return sb.ToString();
        }

        /// <summary>
        /// 构建专属于 PowerShell 脚本的全景富文档手册、代码浏览与结构大纲双模视图
        /// </summary>
        private static void BuildPowerShellRichView(DocumentContext doc, out string toolbarButtons, out string bodyContent)
        {
            var ps = PowerShellScriptAnalyzer.Analyze(doc.Content ?? "", doc.FullPath);
            string rawCodeLiteral = SafeEmbedJson(JsonSerializer.Serialize(doc.Content ?? ""));
            string safeCode = WebUtility.HtmlEncode(doc.Content ?? "");

            toolbarButtons = BuildEditButton(DocFormat.Code) + @"
                <button class='btn' onclick=""copyEntireCode()"">" + IconCopy + @"<span>复制代码</span></button>
                <div class='dropdown'>
                    <button class='btn btn-dropdown'><span>视图模式</span> " + IconChevronDown + @"</button>
                    <div class='dropdown-menu'>
                        " + BuildUnifiedViewSwitchGroup(DocFormat.Code) + @"
                    </div>
                </div>";

            var sb = new StringBuilder(16384);

            // 1. 顶部副工具栏 (Sub-Toolbar): 模式切换与关键指标徽章
            sb.Append(@"
                <div class='view-toolbar ps-toolbar'>
                    <div class='toolbar-group'>
                        <span class='format-pill ps-format-pill'>POWERSHELL</span>
                        <div class='ps-segmented-group'>
                            <button class='btn-pill active' id='btn-ps-mode-doc' onclick='switchPsViewMode(""doc"")' title='基于注释的帮助手册与语法说明'>" + IconBook + @"<span>帮助手册</span></button>
                            <button class='btn-pill' id='btn-ps-mode-code' onclick='switchPsViewMode(""code"")' title='精准行号与完整语法高亮源码'>" + IconCode + @"<span>源码浏览</span></button>
                        </div>
                        <button class='btn-pill' id='btn-ps-outline-toggle' onclick='togglePsOutlineDrawer()' title='展开/收起脚本函数与参数大纲导航'>" + IconList + @"<span>大纲 (" + ps.Functions.Count + @")</span></button>
                    </div>
                    <div class='toolbar-group ps-stats-group' style='justify-content: flex-end;'>");

            if (ps.Requires.Count > 0)
            {
                sb.Append("<span class='stats-tag ps-req-badge' title='运行环境依赖'>" + WebUtility.HtmlEncode(ps.Requires[0]) + "</span>");
            }

            if (ps.SecurityNotices.Count > 0)
            {
                sb.Append("<span class='stats-tag ps-warn-badge' onclick='jumpToPsSecurity()' style='cursor:pointer;' title='点击查看安全审计详情'>⚠️ " + ps.SecurityNotices.Count + " 项审计</span>");
            }

            sb.Append(@"
                        <span class='stats-tag'>" + ps.TotalLines + @" 行</span>
                        <span class='stats-tag'>" + ps.Functions.Count + @" 函数</span>
                        <span class='stats-tag'>" + ps.TopLevelParameters.Count + @" 参数</span>
                    </div>
                </div>");

            // 2. 模式 A: 帮助手册视图 (#ps-doc-view)
            sb.Append(@"
                <div class='ps-doc-container' id='ps-doc-view'>
                    <!-- Hero Header -->
                    <div class='ps-hero-card' id='ps-hero-section'>
                        <div class='ps-hero-header'>
                            <div class='ps-title-wrap'>
                                <span class='ps-hero-icon'>" + IconTerminal + @"</span>
                                <div>
                                    <h1 class='ps-script-title'>" + WebUtility.HtmlEncode(ps.ScriptName) + @"</h1>
                                    <div class='ps-hero-badges'>
                                        <span class='ps-badge'>PowerShell 脚本</span>");

            foreach (var req in ps.Requires)
            {
                sb.Append("<span class='ps-badge ps-badge-req'>" + WebUtility.HtmlEncode(req) + "</span>");
            }

            if (ps.SecurityNotices.Count > 0)
            {
                sb.Append("<span class='ps-badge ps-badge-warn' onclick='jumpToPsSecurity()' style='cursor:pointer;'>⚠️ 包含 " + ps.SecurityNotices.Count + " 处敏感/高危指令</span>");
            }

            sb.Append(@"
                                    </div>
                                </div>
                            </div>
                            <div class='ps-hero-stats'>
                                <div class='ps-stat-box'><span class='stat-num'>" + ps.TotalLines + @"</span><span class='stat-label'>总代码行</span></div>
                                <div class='ps-stat-box'><span class='stat-num'>" + ps.Functions.Count + @"</span><span class='stat-label'>自定义函数</span></div>
                                <div class='ps-stat-box'><span class='stat-num'>" + ps.TopLevelParameters.Count + @"</span><span class='stat-label'>输入参数</span></div>
                            </div>
                        </div>");

            if (!string.IsNullOrWhiteSpace(ps.Synopsis))
            {
                sb.Append(@"
                        <div class='ps-synopsis-callout'>
                            <div class='callout-tag'>SYNOPSIS 摘要</div>
                            <div class='callout-text'>" + WebUtility.HtmlEncode(ps.Synopsis) + @"</div>
                        </div>");
            }

            if (!string.IsNullOrWhiteSpace(ps.Description))
            {
                sb.Append(@"
                        <div class='ps-description-body'>
                            <div class='desc-tag'>DESCRIPTION 说明</div>
                            <div class='desc-text'>" + WebUtility.HtmlEncode(ps.Description).Replace("\r\n", "<br/>").Replace("\n", "<br/>") + @"</div>
                        </div>");
            }

            sb.Append("</div>"); // End ps-hero-card

            // 语法图解 (Syntax)
            sb.Append(@"
                    <div class='ps-section' id='ps-syntax-section'>
                        <div class='ps-section-header'>
                            <h3 class='ps-section-title'>" + IconTerminal + @"<span>命令行调用语法 (Syntax)</span></h3>
                            <button class='btn-pill btn-xs' onclick='copyPsSyntax()' title='复制调用命令行'>" + IconCopy + @"<span>复制语法</span></button>
                        </div>
                        <div class='ps-syntax-box'>
                            <code id='ps-syntax-code'>" + WebUtility.HtmlEncode(ps.SyntaxSynopsis) + @"</code>
                        </div>
                    </div>");

            // 参数列表 (Parameters)
            if (ps.TopLevelParameters.Count > 0)
            {
                sb.Append(@"
                    <div class='ps-section' id='ps-params-section'>
                        <div class='ps-section-header'>
                            <h3 class='ps-section-title'>" + IconSparkles + @"<span>输入参数规格 (Parameters)</span></h3>
                            <span class='ps-badge-counter'>" + ps.TopLevelParameters.Count + @" 个参数</span>
                        </div>
                        <div class='ps-param-table-wrapper'>
                            <table class='ps-param-table'>
                                <thead>
                                    <tr>
                                        <th style='width: 22%;'>参数名称</th>
                                        <th style='width: 14%;'>数据类型</th>
                                        <th style='width: 12%;'>必需性</th>
                                        <th style='width: 22%;'>默认值</th>
                                        <th style='width: 30%;'>可选范围 (ValidateSet) / 描述</th>
                                    </tr>
                                </thead>
                                <tbody>");

                foreach (var param in ps.TopLevelParameters)
                {
                    string reqBadge = param.IsMandatory
                        ? "<span class='ps-param-req'>必需 Mandatory</span>"
                        : "<span class='ps-param-opt'>可选 Optional</span>";

                    string valSets = "";
                    if (param.ValidateSet.Count > 0)
                    {
                        valSets = "<div class='ps-valid-set'>" + string.Join(" ", param.ValidateSet.ConvertAll(v => "<span class='val-chip'>" + WebUtility.HtmlEncode(v) + "</span>")) + "</div>";
                    }

                    string descText = !string.IsNullOrEmpty(param.Description) ? ("<div class='param-desc'>" + WebUtility.HtmlEncode(param.Description) + "</div>") : "";
                    string defVal = !string.IsNullOrEmpty(param.DefaultValue) ? ("<code>" + WebUtility.HtmlEncode(param.DefaultValue) + "</code>") : "<span class='text-muted'>$null</span>";

                    sb.Append(@"
                                    <tr>
                                        <td><span class='param-name'>-$" + WebUtility.HtmlEncode(param.Name) + @"</span></td>
                                        <td><span class='param-type'>[" + WebUtility.HtmlEncode(param.Type) + @"]</span></td>
                                        <td>" + reqBadge + @"</td>
                                        <td>" + defVal + @"</td>
                                        <td>" + valSets + descText + @"</td>
                                    </tr>");
                }

                sb.Append(@"
                                </tbody>
                            </table>
                        </div>
                    </div>");
            }

            // 示例 (Examples)
            if (ps.Examples.Count > 0)
            {
                sb.Append(@"
                    <div class='ps-section' id='ps-examples-section'>
                        <div class='ps-section-header'>
                            <h3 class='ps-section-title'>" + IconSparkles + @"<span>调用示例 (Examples)</span></h3>
                            <span class='ps-badge-counter'>" + ps.Examples.Count + @" 个示例</span>
                        </div>
                        <div class='ps-examples-list'>");

                for (int i = 0; i < ps.Examples.Count; i++)
                {
                    int exIdx = i + 1;
                    string exCode = ps.Examples[i];
                    sb.Append(@"
                            <div class='ps-example-card'>
                                <div class='example-header'>
                                    <span class='example-num'>示例 " + exIdx + @"</span>
                                    <button class='btn-pill btn-xs' onclick=""copyTextToClipboard(" + SafeEmbedJson(JsonSerializer.Serialize(exCode)) + @", '已复制示例代码')"" title='复制代码'>" + IconCopy + @"<span>复制示例</span></button>
                                </div>
                                <pre class='example-pre'><code>" + WebUtility.HtmlEncode(exCode) + @"</code></pre>
                            </div>");
                }

                sb.Append("</div></div>");
            }

            // 自定义函数清单 (Defined Functions)
            if (ps.Functions.Count > 0)
            {
                sb.Append(@"
                    <div class='ps-section' id='ps-functions-section'>
                        <div class='ps-section-header'>
                            <h3 class='ps-section-title'>" + IconList + @"<span>内部函数与子命令 (Functions)</span></h3>
                            <span class='ps-badge-counter'>" + ps.Functions.Count + @" 个函数</span>
                        </div>
                        <div class='ps-functions-grid'>");

                foreach (var f in ps.Functions)
                {
                    string fSyn = !string.IsNullOrEmpty(f.Synopsis) ? WebUtility.HtmlEncode(f.Synopsis) : "自定义脚本函数";
                    string fParamSummary = f.Parameters.Count > 0
                        ? string.Join(" ", f.Parameters.ConvertAll(p => "[-" + p.Name + " <" + p.Type + ">]"))
                        : "无参数";

                    sb.Append(@"
                            <div class='ps-func-card' id='ps-func-card-" + f.LineNumber + @"'>
                                <div class='func-card-header'>
                                    <div class='func-title-group'>
                                        <span class='func-badge'>function</span>
                                        <span class='func-name'>" + WebUtility.HtmlEncode(f.Name) + @"</span>
                                        <span class='func-line' title='定义所在行'>第 " + f.LineNumber + @" 行</span>
                                    </div>
                                    <button class='btn-pill btn-xs' onclick='jumpToPsFunctionInCode(" + f.LineNumber + @", """ + WebUtility.HtmlEncode(f.Name) + @""")' title='在源码中定位此函数'>" + IconCode + @"<span>查看源码</span></button>
                                </div>
                                <div class='func-synopsis'>" + fSyn + @"</div>
                                <div class='func-syntax-chip'><code>" + WebUtility.HtmlEncode(f.Name) + @" " + WebUtility.HtmlEncode(fParamSummary) + @"</code></div>
                            </div>");
                }

                sb.Append("</div></div>");
            }

            // 安全审计警示 (Security Notices)
            if (ps.SecurityNotices.Count > 0)
            {
                sb.Append(@"
                    <div class='ps-section' id='ps-security-section'>
                        <div class='ps-section-header'>
                            <h3 class='ps-section-title' style='color: #ef4444;'>" + IconShield + @"<span>运维与安全审计警示</span></h3>
                            <span class='ps-badge-warn'>" + ps.SecurityNotices.Count + @" 项提示</span>
                        </div>
                        <div class='ps-security-list'>");

                foreach (var sn in ps.SecurityNotices)
                {
                    sb.Append(@"
                            <div class='ps-security-item " + sn.Level + @"'>
                                <div class='sec-header'>
                                    <span class='sec-badge " + sn.Level + @"'>" + WebUtility.HtmlEncode(sn.Title) + @"</span>
                                    <span class='sec-line' onclick='jumpToPsFunctionInCode(" + sn.LineNumber + @")' style='cursor:pointer;'>第 " + sn.LineNumber + @" 行 &rarr;</span>
                                </div>
                                <div class='sec-detail'>" + WebUtility.HtmlEncode(sn.Detail) + @"</div>
                            </div>");
                }

                sb.Append("</div></div>");
            }

            // 若无 Comment-Based Help，输出温馨建议卡片
            if (string.IsNullOrEmpty(ps.Synopsis) && ps.Examples.Count == 0 && ps.TopLevelParameters.Count == 0)
            {
                sb.Append(@"
                    <div class='ps-template-hint-card'>
                        <div class='hint-icon'>" + IconBook + @"</div>
                        <div class='hint-content'>
                            <div class='hint-title'>" + IconLightbulb + @"<span>开启全景帮助手册体验</span></div>
                            <div class='hint-desc'>当前脚本暂未编写 Comment-Based Help (<# .SYNOPSIS ... #>) 注释，点击右侧即可一键复制官方标准帮助注释模板，将其置于脚本头部即可自动生成全景手册。</div>
                        </div>
                        <button class='btn-pill primary' onclick='copyPsCommentTemplate()'>" + IconCopy + @"<span>复制标准注释模板</span></button>
                    </div>");
            }

            sb.Append("</div>"); // End ps-doc-container

            // 3. 模式 B: 源代码高亮视图 (#ps-code-view)
            sb.Append(@"
                <div class='code-viewer-container' id='code-viewer-root' style='display:none;'>
                    <pre><code class='language-powershell'>" + safeCode + @"</code></pre>
                </div>");

            // 4. 侧边大纲抽屉 (#ps-outline-drawer)
            sb.Append(@"
                <div id='ps-outline-drawer' class='ps-outline-drawer'>
                    <div class='drawer-head'>
                        <div class='drawer-title'>" + IconList + @"<span>脚本结构大纲</span></div>
                        <button class='btn-close-drawer' onclick='togglePsOutlineDrawer(false)'>" + IconClose + @"</button>
                    </div>
                    <div class='drawer-search'>
                        <input type='text' id='ps-outline-search' placeholder='筛选函数或参数...' oninput='filterPsOutline(this.value)' />
                    </div>
                    <div class='drawer-body' id='ps-outline-tree'>");

            if (ps.TopLevelParameters.Count > 0)
            {
                sb.Append(@"
                        <div class='outline-group' data-group='params'>
                            <div class='group-title'>" + IconSparkles + @"<span>输入参数 (" + ps.TopLevelParameters.Count + @")</span></div>");
                foreach (var p in ps.TopLevelParameters)
                {
                    sb.Append(@"
                            <div class='outline-item param-item' onclick='jumpToPsSymbol(""ps-params-section"")' data-name='" + WebUtility.HtmlEncode(p.Name.ToLowerInvariant()) + @"'>
                                <span class='item-kind'>param</span>
                                <span class='item-name'>-$" + WebUtility.HtmlEncode(p.Name) + @"</span>
                                <span class='item-type'>[" + WebUtility.HtmlEncode(p.Type) + @"]</span>
                            </div>");
                }
                sb.Append("</div>");
            }

            if (ps.Functions.Count > 0)
            {
                sb.Append(@"
                        <div class='outline-group' data-group='funcs'>
                            <div class='group-title'>" + IconList + @"<span>函数清单 (" + ps.Functions.Count + @")</span></div>");
                foreach (var f in ps.Functions)
                {
                    sb.Append(@"
                            <div class='outline-item func-item' onclick='jumpToPsSymbol(""ps-func-card-" + f.LineNumber + @""", " + f.LineNumber + @")' data-name='" + WebUtility.HtmlEncode(f.Name.ToLowerInvariant()) + @"'>
                                <span class='item-kind'>func</span>
                                <span class='item-name'>" + WebUtility.HtmlEncode(f.Name) + @"</span>
                                <span class='item-line'>L" + f.LineNumber + @"</span>
                            </div>");
                }
                sb.Append("</div>");
            }

            if (ps.SecurityNotices.Count > 0)
            {
                sb.Append(@"
                        <div class='outline-group' data-group='security'>
                            <div class='group-title'>" + IconShield + @"<span>安全审计 (" + ps.SecurityNotices.Count + @")</span></div>");
                foreach (var sn in ps.SecurityNotices)
                {
                    sb.Append(@"
                            <div class='outline-item sec-item " + sn.Level + @"' onclick='jumpToPsSymbol(""ps-security-section"", " + sn.LineNumber + @")' data-name='" + WebUtility.HtmlEncode(sn.Title.ToLowerInvariant()) + @"'>
                                <span class='item-kind'>audit</span>
                                <span class='item-name'>" + WebUtility.HtmlEncode(sn.Title) + @"</span>
                                <span class='item-line'>L" + sn.LineNumber + @"</span>
                            </div>");
                }
                sb.Append("</div>");
            }

            sb.Append(@"
                    </div>
                </div>
                <div id='ps-outline-backdrop' class='ps-outline-backdrop' onclick='togglePsOutlineDrawer(false)'></div>");

            // 5. 挂载数据与脚本
            sb.Append(@"
                <script>
                    window.__RAW_CODE__ = " + rawCodeLiteral + @";
                    window.__CODE_LANG__ = 'powershell';
                    window.__PS_SYNTAX__ = " + SafeEmbedJson(JsonSerializer.Serialize(ps.SyntaxSynopsis)) + @";
                    window.__PS_SCRIPT_NAME__ = " + SafeEmbedJson(JsonSerializer.Serialize(ps.ScriptName)) + @";
                </script>");

            bodyContent = sb.ToString();
        }

        public static string BuildOmniViewHtml(DocumentContext doc, DetectionResult det, string baseHref = "", bool isDarkMode = false)
        {
            doc = doc ?? new DocumentContext { Content = string.Empty, FullPath = string.Empty, Extension = string.Empty };
            det = det ?? new DetectionResult { Format = DocFormat.Unknown, Reason = "未知格式" };
            string baseTag = string.IsNullOrEmpty(baseHref) ? "" : "<base href=\"" + baseHref + "\" />";
            string statusTag = doc.IsSavedOnDisk ? "已保存" : "未保存缓冲区";
            string safeReason = WebUtility.HtmlEncode(det.Reason ?? "");
            string darkModeClass = isDarkMode ? "dark-mode" : "";
            string formatClass = "fmt-" + det.Format.ToString().ToLowerInvariant();

            string bodyContent = "";
            string toolbarButtons = "";
            string fmtBadge = det.Format.ToString().ToUpperInvariant();
            string badgeClass = "badge-" + det.Format.ToString().ToLowerInvariant();
            if (det.Format == DocFormat.Unknown)
            {
                fmtBadge = "纯文本 (未识别)";
                badgeClass = "badge-unknown";
            }
            else if (det.Format == DocFormat.Code)
            {
                string lang = !string.IsNullOrEmpty(det.DetectedLanguage) ? det.DetectedLanguage : MultiFormatDetector.GetLanguageForExtension(doc.Extension);
                fmtBadge = "CODE: " + lang.ToUpperInvariant();
                if (lang.Equals("powershell", StringComparison.OrdinalIgnoreCase))
                {
                    formatClass = "fmt-code fmt-powershell";
                }
            }

            string loadingTitle = "正在智能解析与渲染文档视图...";
            string loadingSub = "全能视图 · 0ms 内存桥接 · 毫秒级流体渲染";
            switch (det.Format)
            {
                case DocFormat.Json:
                    loadingTitle = "正在解析并构建 JSON 交互树...";
                    loadingSub = "交互折叠大纲 · 键值就地编辑 · 智能类型徽章";
                    break;
                case DocFormat.Csv:
                    loadingTitle = "正在解析并排版电子表格网格...";
                    loadingSub = "多分隔符自适应 · 单元格双向编辑 · 列宽对齐";
                    break;
                case DocFormat.Markdown:
                    loadingTitle = "正在排版 Markdown 富文本与公式...";
                    loadingSub = "GFM 规范 · GitHub 警告块 · KaTeX 引擎 · TOC 平滑导航";
                    break;
                case DocFormat.Svg:
                    loadingTitle = "正在初始化 SVG 矢量画布与图形...";
                    loadingSub = "无损平移缩放 · 矢量形状检视 · 统一调色盘";
                    break;
                case DocFormat.Yaml:
                    loadingTitle = "正在解析并构建 YAML 配置树...";
                    loadingSub = "js-yaml 结构解析 · 规范美化 · 树状行内编辑";
                    break;
                case DocFormat.Mermaid:
                    loadingTitle = "正在计算并排版矢量架构图...";
                    loadingSub = "1:1 矢量抗锯齿 · 自适应视口等比缩放 · 节点编辑";
                    break;
                case DocFormat.Html:
                    loadingTitle = "正在准备超清视口沙箱环境...";
                    loadingSub = "多分辨率预设 · 原生所见即所得 · 移动端外框";
                    break;
                case DocFormat.Xml:
                    loadingTitle = "正在解析并构建 XML 交互树...";
                    loadingSub = "DOM 节点折叠 · 属性胶囊检视 · 规范美化";
                    break;
                case DocFormat.Code:
                    loadingTitle = "正在读取源码元数据与环境...";
                    loadingSub = "轻量代码概览 · 统计分析 · 按需全屏展演";
                    break;
            }

            if (det.Format == DocFormat.Markdown)
            {
                toolbarButtons = BuildEditButton(DocFormat.Markdown) + @"
                    <button class='btn' onclick=""postAct('MD_TO_HTML')"">" + IconExport + @"<span>导出 HTML</span></button>
                    <div class='dropdown'>
                        <button class='btn btn-dropdown'><span>操作</span> " + IconChevronDown + @"</button>
                        <div class='dropdown-menu'>
                            <div class='dropdown-group'>
                                " + BuildDropdownHeader("文档数据转换") + @"
                                " + BuildDropdownItem("MD_TO_HTML", "icon-html", IconHtml, "导出为 HTML 源码", "解析为 HTML 标记并写入文档", "写入") + @"
                            </div>
                            " + DropdownDivider + @"
                            " + BuildUnifiedViewSwitchGroup(DocFormat.Markdown) + @"
                        </div>
                    </div>";
                bodyContent = @"
                    <div class='view-toolbar md-toolbar'>
                        <div class='toolbar-group'>
                            <span class='format-pill'>GFM 富文本</span>
                            <button class='btn-pill' onclick=""postAct('MD_TO_HTML')"" title='导出为 HTML 源码'>" + IconExport + @"<span>导出 HTML</span></button>
                            <button class='btn-pill' onclick='enterEditMode()' title='在当前视口直接编辑 (快捷键 F2)'>" + IconEdit + @"<span>编辑文档</span></button>
                        </div>
                        <div class='search-box-wrap' style='justify-content: flex-end;'>
                            <span class='edit-tip-badge' title='双击任意段落、标题、代码块或列表项即可就地编辑 (按 Ctrl+S 保存)'>" + IconLightbulb + @"<span>双击就地编辑</span></span>
                        </div>
                    </div>
                    <div id='md-container' class='md-body'>" + ConvertMarkdownToHtml(doc.Content) + @"</div>";
            }
            else if (det.Format == DocFormat.Json)
            {
                toolbarButtons = BuildEditButton(DocFormat.Json) + @"
                    <button class='btn' onclick=""postAct('JSON_BEAUTIFY')"">" + IconSparkles + @"<span>美化缩进</span></button>
                    <button class='btn' onclick=""postAct('JSON_MINIFY')"">" + IconCompress + @"<span>压缩单行</span></button>
                    <div class='dropdown'>
                        <button class='btn btn-dropdown'><span>转换</span> " + IconChevronDown + @"</button>
                        <div class='dropdown-menu'>
                            <div class='dropdown-group'>
                                " + BuildDropdownHeader("文档数据转换") + @"
                                " + BuildDropdownItem("JSON_TO_CSV", "icon-csv", IconCsv, "转换 JSON &rarr; CSV 表格", "将扁平对象数组转为标准逗号表格", "写入") + @"
                                " + BuildDropdownItem("JSON_TO_MD", "icon-md", IconMarkdown, "转换 JSON &rarr; Markdown 表格", "将数组数据转为 MD 管道语法表格", "写入") + @"
                            </div>
                            " + DropdownDivider + @"
                            " + BuildUnifiedViewSwitchGroup(DocFormat.Json) + @"
                        </div>
                    </div>";
                string rawJsonLiteral = SafeEmbedJson(JsonSerializer.Serialize(doc.Content ?? ""));
                bodyContent = @"
                    <div class='view-toolbar json-toolbar'>
                        <div class='toolbar-group'>
                            <button class='btn-pill active' id='btn-toggle-types' onclick='toggleJsonTypes()'>
                                " + IconTag + @"<span>数据类型</span>
                            </button>
                            <button class='btn-pill' onclick='expandAllJson()'>" + IconExpand + @"<span>展开</span></button>
                            <button class='btn-pill' onclick='collapseAllJson()'>" + IconCollapse + @"<span>折叠</span></button>
                            <button class='btn-pill' onclick='copyFormattedJson()'>" + IconCopy + @"<span>复制 JSON</span></button>
                        </div>
                        <div class='search-box-wrap'>
                            <span class='edit-tip-badge' title='双击任意键名、属性值或数值即可就地修改 (按 Ctrl+S 保存)'>" + IconLightbulb + @"<span>双击就地编辑</span></span>
                            <input id='json-filter' type='text' placeholder='输入关键词搜索...' oninput='filterJsonTree(this.value)' />
                            <span id='json-stats' class='stats-tag'></span>
                        </div>
                    </div>
                    <div id='json-root' class='json-container'></div>
                    <script>window.__RAW_JSON__ = " + rawJsonLiteral + @";</script>";
            }
            else if (det.Format == DocFormat.Csv)
            {
                toolbarButtons = BuildEditButton(DocFormat.Csv) + @"
                    <button class='btn' onclick=""postAct('CSV_ALIGN')"">" + IconAlign + @"<span>列宽对齐</span></button>
                    <button class='btn' onclick=""postAct('CSV_MINIFY')"">" + IconCompress + @"<span>紧凑压缩</span></button>
                    <div class='dropdown'>
                        <button class='btn btn-dropdown'><span>转换</span> " + IconChevronDown + @"</button>
                        <div class='dropdown-menu'>
                            <div class='dropdown-group'>
                                " + BuildDropdownHeader("替换分隔符") + @"
                                " + BuildDropdownItem("CSV_REPLACE_DELIM:,", "icon-csv", IconCsv, "转为逗号 (,) 分隔", "标准逗号表格 CSV 格式", "转换") + @"
                                " + BuildDropdownItem("CSV_REPLACE_DELIM:\\t", "icon-csv", IconCsv, "转为制表符 (Tab) 分隔", "制表符分隔 TSV 格式", "转换") + @"
                                " + BuildDropdownItem("CSV_REPLACE_DELIM:;", "icon-csv", IconCsv, "转为分号 (;) 分隔", "欧洲区域常用表格格式", "转换") + @"
                                " + BuildDropdownItem("CSV_REPLACE_DELIM:|", "icon-csv", IconCsv, "转为竖线 (|) 分隔", "管道字符表格格式", "转换") + @"
                            </div>
                            " + DropdownDivider + @"
                            <div class='dropdown-group'>
                                " + BuildDropdownHeader("文档数据转换") + @"
                                " + BuildDropdownItem("CSV_TO_JSON", "icon-json", IconJson, "转换 CSV &rarr; JSON 数据", "将表格序列化为结构化对象数组", "写入") + @"
                                " + BuildDropdownItem("CSV_TO_MD", "icon-md", IconMarkdown, "转换 CSV &rarr; Markdown 表格", "转为标准 GitHub 风格管道表格", "写入") + @"
                            </div>
                            " + DropdownDivider + @"
                            " + BuildUnifiedViewSwitchGroup(DocFormat.Csv) + @"
                        </div>
                    </div>";
                var rows = FormatProcessor.ParseCsv(doc.Content);
                char delim = FormatProcessor.DetectCsvDelimiter(doc.Content);
                string delimName;
                switch (delim)
                {
                    case '\t': delimName = "TSV (制表符分隔)"; break;
                    case ';': delimName = "DSV (分号分隔)"; break;
                    case '|': delimName = "DSV (竖线分隔)"; break;
                    case ',': delimName = "CSV (逗号分隔)"; break;
                    default: delimName = "DSV ('" + delim + "' 分隔)"; break;
                }
                string rowsLiteral = SafeEmbedJson(JsonSerializer.Serialize(rows));
                bodyContent = @"
                    <div class='view-toolbar csv-toolbar' id='csv-toolbar'>
                        <div class='toolbar-group'>
                            <button id='csv-delim-pill' class='format-pill delim-pill-btn' onclick='openCsvDelimModal()' title='当前分隔符：点击快捷更换分隔符'>
                                <span class='pill-prefix'>分隔符</span>
                                <span class='pill-val'>" + delimName + @"</span>
                            </button>
                            <button class='btn-pill' onclick='openCsvDelimModal()' title='替换表格分隔符 (逗号/制表符/分号/竖线等)'>" + IconReplace + @"<span>更换分隔符</span></button>
                            <button class='btn-pill' onclick='addCsvRow()' title='在表格末尾追加空白数据行'>" + IconPlus + @"<span>添加新行</span></button>
                            <button class='btn-pill' onclick='copyCsvAsTable()' title='复制整表制表符数据 (可直接粘贴至 Excel/WPS)'>" + IconCopy + @"<span>复制数据</span></button>
                        </div>
                        <div class='search-box-wrap csv-search-wrap'>
                            <span class='edit-tip-badge' title='双击任意单元格即可原地修改 (Enter/Tab 切换单元格，Esc 取消)'>" + IconLightbulb + @"<span>双击就地编辑</span></span>
                            <div class='search-input-inner'>
                                " + IconSearch + @"
                                <input id='csv-filter' type='text' placeholder='搜索表格数据... (ESC 清空)' oninput='filterCsvTable(this.value)' onkeydown='handleCsvFilterKey(event)' />
                                <button id='csv-clear-search' class='btn-clear-search' onclick='clearCsvFilter()' title='清空过滤关键词' style='display:none;'>" + IconClose + @"</button>
                            </div>
                            <span id='csv-stats' class='stats-tag csv-stats-badge'></span>
                        </div>
                    </div>
                    <div id='csv-root' class='csv-container'></div>
                    <script>window.__CSV_ROWS__ = " + rowsLiteral + @"; window.__CSV_DELIM__ = " + JsonSerializer.Serialize(delim.ToString()) + @";</script>";
            }
            else if (det.Format == DocFormat.Svg)
            {
                toolbarButtons = BuildEditButton(DocFormat.Svg) + @"
                    <button class='btn' onclick=""postAct('SVG_BEAUTIFY')"">" + IconSparkles + @"<span>格式化 SVG</span></button>
                    <button class='btn' onclick=""copySvgDataUri()"">" + IconCopy + @"<span>复制 Data URI</span></button>
                    <button class='btn' onclick=""exportSvgAsPng()"">" + IconExport + @"<span>导出 PNG</span></button>
                    <div class='dropdown'>
                        <button class='btn btn-dropdown'><span>操作</span> " + IconChevronDown + @"</button>
                        <div class='dropdown-menu'>
                            <div class='dropdown-group'>
                                " + BuildDropdownHeader("矢量图形工具") + @"
                                " + BuildDropdownItem("SVG_BEAUTIFY", "icon-svg", IconSvg, "规范化格式化 SVG", "自动缩进与标签换行排版", "写入") + @"
                            </div>
                            " + DropdownDivider + @"
                            " + BuildUnifiedViewSwitchGroup(DocFormat.Svg) + @"
                        </div>
                    </div>";
                string rawSvgLiteral = SafeEmbedJson(JsonSerializer.Serialize(doc.Content ?? ""));
                bodyContent = @"
                    <div class='view-toolbar svg-toolbar'>
                        <div class='toolbar-group'>
                            " + BuildPalettePopoverHtml("svg") + @"
                            <button class='btn-pill' onclick='zoomSvg(1.2)' title='放大 (滚轮向上)'>" + IconZoomIn + @"<span>放大</span></button>
                            <button class='btn-pill' onclick='zoomSvg(0.8)' title='缩小 (滚轮向下)'>" + IconZoomOut + @"<span>缩小</span></button>
                            <button class='btn-pill' onclick='fitSvgToViewport()' title='自适应窗口大小'>" + IconExpand + @"<span>适应窗口</span></button>
                            <span id='svg-zoom-tag' class='zoom-badge' title='当前缩放比例'>100%</span>
                        </div>
                        <div class='search-box-wrap' style='justify-content: flex-end;'>
                            <span class='edit-tip-badge' title='双击画布文本即可快速就地编辑 (按 Ctrl+S 保存)'>" + IconLightbulb + @"<span>双击就地编辑</span></span>
                            <span id='svg-dim-tag' class='stats-tag'>尺寸: 自适应</span>
                        </div>
                    </div>
                    <div class='svg-viewport bg-none' id='svg-viewport' onwheel='handleSvgWheel(event)' onmousedown='startSvgPan(event)'>
                        <div id='svg-canvas' class='svg-canvas'>" + (doc.Content ?? "") + @"</div>
                    </div>
                    <script>window.__RAW_SVG__ = " + rawSvgLiteral + @";</script>";
            }
            else if (det.Format == DocFormat.Yaml)
            {
                toolbarButtons = BuildEditButton(DocFormat.Yaml) + @"
                    <button class='btn' onclick=""beautifyYaml()"">" + IconSparkles + @"<span>美化缩进</span></button>
                    <div class='dropdown'>
                        <button class='btn btn-dropdown'><span>转换</span> " + IconChevronDown + @"</button>
                        <div class='dropdown-menu'>
                            <div class='dropdown-group'>
                                " + BuildDropdownHeader("文档数据转换") + @"
                                " + BuildDropdownItem("YAML_TO_JSON", "icon-json", IconJson, "转换 YAML &rarr; JSON", "将配置数据转为标准 JSON 树", "写入") + @"
                                " + BuildDropdownItem("YAML_TO_CSV", "icon-csv", IconCsv, "转换 YAML &rarr; CSV", "将对象数组转为标准逗号表格", "写入") + @"
                            </div>
                            " + DropdownDivider + @"
                            " + BuildUnifiedViewSwitchGroup(DocFormat.Yaml) + @"
                        </div>
                    </div>";
                string rawYamlLiteral = SafeEmbedJson(JsonSerializer.Serialize(doc.Content ?? ""));
                bodyContent = @"
                    <div class='view-toolbar yaml-toolbar'>
                        <div class='toolbar-group'>
                            <button class='btn-pill active' id='btn-toggle-types' onclick='toggleJsonTypes()'>
                                " + IconTag + @"<span>数据类型</span>
                            </button>
                            <button class='btn-pill' onclick='expandAllJson()'>" + IconExpand + @"<span>全部展开</span></button>
                            <button class='btn-pill' onclick='collapseAllJson()'>" + IconCollapse + @"<span>全部折叠</span></button>
                            <button class='btn-pill' onclick='copyFormattedYamlJson()'>" + IconCopy + @"<span>复制为 JSON</span></button>
                        </div>
                        <div class='search-box-wrap'>
                            <span class='edit-tip-badge' title='双击任意配置项键名或属性值即可就地编辑 (按 Ctrl+S 保存)'>" + IconLightbulb + @"<span>双击就地编辑</span></span>
                            <input id='json-filter' type='text' placeholder='输入关键词搜索...' oninput='filterJsonTree(this.value)' />
                            <span id='json-stats' class='stats-tag'></span>
                        </div>
                    </div>
                    <div id='yaml-root' class='json-container'></div>
                    <script>window.__RAW_YAML__ = " + rawYamlLiteral + @";</script>";
            }
            else if (det.Format == DocFormat.Mermaid)
            {
                toolbarButtons = BuildEditButton(DocFormat.Mermaid) + @"
                    <button class='btn' onclick=""copyMermaidSvg()"">" + IconCopy + @"<span>复制矢量 SVG</span></button>
                    <button class='btn' onclick=""exportMermaidPng()"">" + IconExport + @"<span>导出 PNG</span></button>
                    <div class='dropdown'>
                        <button class='btn btn-dropdown'><span>操作</span> " + IconChevronDown + @"</button>
                        <div class='dropdown-menu'>
                            " + BuildUnifiedViewSwitchGroup(DocFormat.Mermaid) + @"
                        </div>
                    </div>";
                string rawMermaidLiteral = SafeEmbedJson(JsonSerializer.Serialize(doc.Content ?? ""));
                bodyContent = @"
                    <div class='view-toolbar mermaid-toolbar'>
                        <div class='toolbar-group'>
                            " + BuildPalettePopoverHtml("mermaid") + @"
                            <button class='btn-pill' onclick='zoomMermaid(1.2)' title='放大 (滚轮向上)'>" + IconZoomIn + @"<span>放大</span></button>
                            <button class='btn-pill' onclick='zoomMermaid(0.8)' title='缩小 (滚轮向下)'>" + IconZoomOut + @"<span>缩小</span></button>
                            <button class='btn-pill' onclick='fitMermaidToViewport()' title='自适应窗口大小'>" + IconExpand + @"<span>适应窗口</span></button>
                            <span id='mermaid-zoom-tag' class='zoom-badge' title='当前缩放比例'>100%</span>
                        </div>
                        <div class='search-box-wrap' style='justify-content: flex-end;'>
                            <span class='edit-tip-badge' title='双击图表中的节点即可快速原地编辑文本 (按 Ctrl+S 保存)'>" + IconLightbulb + @"<span>双击就地编辑</span></span>
                        </div>
                    </div>
                    <div class='svg-viewport mermaid-viewport' id='mermaid-viewport' onwheel='handleMermaidWheel(event)' onmousedown='startMermaidPan(event)'>
                        <div id='mermaid-loading-state' class='omni-loading-placeholder'>
                            <div class='loading-radar-ring'></div>
                            <div class='loading-title'>正在智能解析与渲染矢量架构图...</div>
                            <div class='loading-sub'>基于 Mermaid 矢量引擎自动排版与自适应缩放</div>
                        </div>
                        <div id='mermaid-canvas' class='mermaid-canvas'>
                            <div class='mermaid' id='mermaid-root'>" + WebUtility.HtmlEncode(doc.Content ?? "") + @"</div>
                        </div>
                    </div>
                    <script>window.__RAW_MERMAID__ = " + rawMermaidLiteral + @";</script>";
            }
            else if (det.Format == DocFormat.Html)
            {
                toolbarButtons = BuildEditButton(DocFormat.Html) + @"
                    <div class='dropdown' id='html-res-dropdown'>
                        <button class='btn btn-dropdown' id='btn-html-res-select' title='切换视口分辨率与设备规格'>
                            " + IconDeviceDesktop + @"<span id='html-res-select-text'>1080p (1920&times;1080)</span> " + IconChevronDown + @"
                        </button>
                        <div class='dropdown-menu'>
                            <div class='dropdown-group'>
                                " + BuildDropdownHeader("标准显示分辨率") + @"
                                <button class='dropdown-item' onclick=""setHtmlResolution(1920, 1080, '1080p', false)"">
                                    <span class='item-icon icon-html'>" + IconDeviceDesktop + @"</span>
                                    <div class='item-content'>
                                        <span class='item-title'>1080p (推荐默认)</span>
                                        <span class='item-subtitle'>1920 &times; 1080 · 全高清 FHD</span>
                                    </div>
                                    <span class='item-action-tag'>16:9</span>
                                </button>
                                <button class='dropdown-item' onclick=""setHtmlResolution(1280, 720, '720p', false)"">
                                    <span class='item-icon icon-html'>" + IconDeviceDesktop + @"</span>
                                    <div class='item-content'>
                                        <span class='item-title'>720p</span>
                                        <span class='item-subtitle'>1280 &times; 720 · 高清 HD</span>
                                    </div>
                                    <span class='item-action-tag'>16:9</span>
                                </button>
                                <button class='dropdown-item' onclick=""setHtmlResolution(2560, 1440, '2K', false)"">
                                    <span class='item-icon icon-html'>" + IconDeviceDesktop + @"</span>
                                    <div class='item-content'>
                                        <span class='item-title'>2K (QHD)</span>
                                        <span class='item-subtitle'>2560 &times; 1440 · 视网膜 2K</span>
                                    </div>
                                    <span class='item-action-tag'>16:9</span>
                                </button>
                                <button class='dropdown-item' onclick=""setHtmlResolution(3840, 2160, '4K', false)"">
                                    <span class='item-icon icon-html'>" + IconDeviceDesktop + @"</span>
                                    <div class='item-content'>
                                        <span class='item-title'>4K (UHD)</span>
                                        <span class='item-subtitle'>3840 &times; 2160 · 4K 超高清</span>
                                    </div>
                                    <span class='item-action-tag'>16:9</span>
                                </button>
                            </div>
                            " + DropdownDivider + @"
                            <div class='dropdown-group'>
                                " + BuildDropdownHeader("移动与平板设备") + @"
                                <button class='dropdown-item' onclick=""setHtmlResolution(390, 844, '手机', true)"">
                                    <span class='item-icon icon-html'>" + IconDeviceMobile + @"</span>
                                    <div class='item-content'>
                                        <span class='item-title'>手机移动端 (支持横竖屏)</span>
                                        <span class='item-subtitle'>390 &times; 844 · 主流智能手机</span>
                                    </div>
                                    <span class='item-action-tag'>手机</span>
                                </button>
                                <button class='dropdown-item' onclick=""setHtmlResolution(768, 1024, '平板', true)"">
                                    <span class='item-icon icon-html'>" + IconDeviceTablet + @"</span>
                                    <div class='item-content'>
                                        <span class='item-title'>iPad / 平板视口</span>
                                        <span class='item-subtitle'>768 &times; 1024 · 平板电脑</span>
                                    </div>
                                    <span class='item-action-tag'>平板</span>
                                </button>
                            </div>
                            " + DropdownDivider + @"
                            <div class='dropdown-group'>
                                " + BuildDropdownHeader("全宽与自定义") + @"
                                <button class='dropdown-item' onclick=""setHtmlResolution(0, 0, '全宽', false)"">
                                    <span class='item-icon icon-html'>" + IconMaximize + @"</span>
                                    <div class='item-content'>
                                        <span class='item-title'>桌面全宽视口</span>
                                        <span class='item-subtitle'>100% 充满视口沙箱</span>
                                    </div>
                                    <span class='item-action-tag'>100%</span>
                                </button>
                                <button class='dropdown-item' onclick=""openHtmlCustomResModal()"">
                                    <span class='item-icon icon-html'>" + IconSparkles + @"</span>
                                    <div class='item-content'>
                                        <span class='item-title'>自定义分辨率...</span>
                                        <span class='item-subtitle'>自由设定任意宽 &times; 高像素</span>
                                    </div>
                                    <span class='item-action-tag'>自定义</span>
                                </button>
                            </div>
                        </div>
                    </div>
                    <button class='btn' id='btn-html-rotate' onclick='toggleHtmlOrientation()' title='横竖屏方向切换 (互换宽与高)'>
                        " + IconRotate + @"<span id='btn-rotate-text'>横竖屏</span>
                    </button>
                    <button class='btn' id='btn-html-fit' onclick='toggleHtmlAutoFit()' title='切换等比缩放自适应与 1:1 真实像素'>
                        " + IconMaximize + @"<span id='btn-fit-text'>等比适应</span>
                    </button>
                    <div class='dropdown'>
                        <button class='btn btn-dropdown'><span>视图模式</span> " + IconChevronDown + @"</button>
                        <div class='dropdown-menu'>
                            " + BuildUnifiedViewSwitchGroup(DocFormat.Html) + @"
                        </div>
                    </div>";
                string rawHtmlLiteral = SafeEmbedJson(JsonSerializer.Serialize(doc.Content ?? ""));
                bodyContent = @"
                    <div class='view-toolbar html-toolbar'>
                        <div class='toolbar-group'>
                            <span class='format-pill' id='html-mode-pill'>1080p (1920&times;1080)</span>
                            <span class='format-pill' id='html-orient-pill'>横屏 Landscape</span>
                            <button class='btn-pill' onclick='reloadHtmlPreview()' title='重新加载 HTML'>" + IconReset + @"<span>刷新</span></button>
                            <button class='btn-pill' onclick='openHtmlCustomResModal()' title='自定义视口宽高'>" + IconSparkles + @"<span>自定义</span></button>
                        </div>
                        <div class='search-box-wrap' style='justify-content: flex-end;'>
                            <span class='edit-tip-badge' title='双击视口文本可快速就地编辑，亦可点击顶部“编辑源码”/按 F2'>" + IconLightbulb + @"<span>双击就地编辑</span></span>
                            <span id='html-dim-tag' class='stats-tag'>1920 &times; 1080 px</span>
                            <span id='html-zoom-tag' class='zoom-badge' title='当前视口等比自适应缩放比例'>100%</span>
                        </div>
                    </div>
                    <div class='html-viewport-wrapper' id='html-viewport-wrapper'>
                        <div class='html-viewport-stage' id='html-viewport-stage'>
                            <div id='html-device-frame' class='html-device-frame'>
                                <iframe id='html-preview-frame' sandbox='allow-scripts allow-same-origin'></iframe>
                            </div>
                        </div>
                    </div>
                    <script>window.__RAW_HTML__ = " + rawHtmlLiteral + @";</script>";
            }
            else if (det.Format == DocFormat.Xml)
            {
                toolbarButtons = BuildEditButton(DocFormat.Xml) + @"
                    <button class='btn' onclick=""postAct('XML_BEAUTIFY')"">" + IconSparkles + @"<span>美化 XML</span></button>
                    <div class='dropdown'>
                        <button class='btn btn-dropdown'><span>操作</span> " + IconChevronDown + @"</button>
                        <div class='dropdown-menu'>
                            <div class='dropdown-group'>
                                " + BuildDropdownHeader("XML 结构操作") + @"
                                " + BuildDropdownItem("XML_BEAUTIFY", "icon-xml", IconSparkles, "美化 XML 缩进", "规范化 2 空格缩进并对齐节点", "格式化") + @"
                            </div>
                            " + DropdownDivider + @"
                            <div class='dropdown-group'>
                                " + BuildUnifiedViewSwitchGroup(DocFormat.Xml) + @"
                            </div>
                        </div>
                    </div>";
                string rawXmlLiteral = SafeEmbedJson(JsonSerializer.Serialize(doc.Content ?? ""));
                bodyContent = @"
                    <div class='view-toolbar xml-toolbar'>
                        <div class='toolbar-group'>
                            <button class='btn-pill' id='btn-toggle-types' onclick='toggleXmlAttributes()'>" + IconTag + @"<span>属性徽章</span></button>
                            <button class='btn-pill' onclick='expandAllXml()'>" + IconExpand + @"<span>全部展开</span></button>
                            <button class='btn-pill' onclick='collapseAllXml()'>" + IconCollapse + @"<span>全部折叠</span></button>
                            <button class='btn-pill' onclick='copyFormattedXml()'>" + IconCopy + @"<span>复制 XML</span></button>
                        </div>
                        <div class='search-box-wrap'>
                            <span class='edit-tip-badge' title='双击节点或文本即可查看/就地编辑 (按 Ctrl+S 保存)'>" + IconLightbulb + @"<span>双击就地编辑</span></span>
                            <input id='xml-filter' type='text' placeholder='筛选 XML 节点或属性...' oninput='filterXmlTree(this.value)' />
                            <span id='xml-stats' class='stats-tag'></span>
                        </div>
                    </div>
                    <div id='xml-root' class='json-container xml-container'></div>
                    <script>window.__RAW_XML__ = " + rawXmlLiteral + @";</script>";
            }
            else if (det.Format == DocFormat.Code)
            {
                string lang = !string.IsNullOrEmpty(det.DetectedLanguage) ? det.DetectedLanguage : MultiFormatDetector.GetLanguageForExtension(doc.Extension);
                if (lang.Equals("powershell", StringComparison.OrdinalIgnoreCase))
                {
                    BuildPowerShellRichView(doc, out toolbarButtons, out bodyContent);
                }
                else
                {
                    int lineCount = (doc.Content ?? "").Split('\n').Length;
                    int charCount = (doc.Content ?? "").Length;
                    double sizeKb = Math.Round((double)charCount / 1024.0, 1);

                    toolbarButtons = BuildEditButton(DocFormat.Code) + @"
                        <button class='btn' onclick=""copyEntireCode()"">" + IconCopy + @"<span>复制代码</span></button>
                        <div class='dropdown'>
                            <button class='btn btn-dropdown'><span>视图模式</span> " + IconChevronDown + @"</button>
                            <div class='dropdown-menu'>
                                " + BuildUnifiedViewSwitchGroup(DocFormat.Code) + @"
                            </div>
                        </div>";
                    string rawCodeLiteral = SafeEmbedJson(JsonSerializer.Serialize(doc.Content ?? ""));
                    string safeCode = WebUtility.HtmlEncode(doc.Content ?? "");
                    bodyContent = @"
                        <div class='code-overview-wrapper'>
                            <div class='code-overview-card'>
                                <div class='code-lang-badge-large'>
                                    <span class='code-icon'>&lt;/&gt;</span>
                                    <span class='code-lang-title'>" + lang.ToUpperInvariant() + @" 源码文档</span>
                                </div>
                                <div class='code-stats-grid'>
                                    <div class='code-stat-item'>
                                        <span class='stat-val' id='code-stat-lines'>" + lineCount + @"</span>
                                        <span class='stat-label'>代码行数</span>
                                    </div>
                                    <div class='code-stat-item'>
                                        <span class='stat-val' id='code-stat-chars'>" + charCount + @"</span>
                                        <span class='stat-label'>字符数量</span>
                                    </div>
                                    <div class='code-stat-item'>
                                        <span class='stat-val' id='code-stat-size'>" + (sizeKb > 0 ? sizeKb + " KB" : charCount + " B") + @"</span>
                                        <span class='stat-label'>文件大小</span>
                                    </div>
                                    <div class='code-stat-item'>
                                        <span class='stat-val'>编辑中</span>
                                        <span class='stat-label'>Scintilla 状态</span>
                                    </div>
                                </div>
                                <div class='code-tip-box'>
                                    <p>⚡ <strong>智能免渲染休眠</strong>：Notepad++ 原生编辑器已内置高速代码语法高亮与编辑能力。全能视图已主动休眠侧边栏 DOM 代码镜像，实现零额外 CPU 与内存消耗，避免左右冗余对比；全能视图核心算力专注于 Markdown、JSON、CSV、SVG、YAML、Mermaid、HTML、XML 等富格式与结构化数据的深度渲染与双向就地编辑。</p>
                                </div>
                                <div class='code-action-row'>
                                    <button class='btn primary' onclick='toggleZenFullscreen(true)'>" + IconMaximize + @"<span>全屏沉浸展演 (F11)</span></button>
                                    <button class='btn' id='btn-toggle-code-mirror' onclick='toggleCodeMirrorDisplay()'>" + IconCode + @"<span>按需展开代码镜像</span></button>
                                </div>
                            </div>
                        </div>
                        <div class='code-viewer-container' id='code-viewer-root' style='display:none;'>
                            <pre><code class='language-" + lang + @"'>" + safeCode + @"</code></pre>
                        </div>
                        <script>
                            window.__RAW_CODE__ = " + rawCodeLiteral + @";
                            window.__CODE_LANG__ = " + JsonSerializer.Serialize(lang) + @";
                        </script>";
                }
            }
            else
            {
                toolbarButtons = BuildEditButton(DocFormat.Unknown) + @"
                    <div class='dropdown'>
                        <button class='btn primary btn-dropdown'><span>指定预览模式</span> " + IconChevronDown + @"</button>
                        <div class='dropdown-menu'>
                            " + BuildUnifiedViewSwitchGroup(DocFormat.Unknown) + @"
                        </div>
                    </div>";
                bodyContent = @"
                    <div class='notice-card'>
                        <div class='notice-icon'>" + IconEmptyDoc + @"</div>
                        <h3 class='notice-title'>未识别到特定格式</h3>
                        <p class='notice-desc'>" + safeReason + @"</p>
                        <p class='notice-hint'>您可以在编辑器中编辑 Markdown / JSON / CSV / SVG / YAML / Mermaid / HTML 文本，或点击上方按钮指定预览模式。</p>
                    </div>";
            }

            string editToolbarButtons;
            if (det.Format == DocFormat.Csv)
            {
                editToolbarButtons = @"
                    <button class='btn btn-sm' onclick='addCsvRow()' title='在表格末尾添加新行'>" + IconPlus + @"<span>添加新行</span></button>
                    <button class='btn btn-sm' onclick='openCsvDelimModal()' title='替换分隔符'>" + IconReplace + @"<span>替换分隔符</span></button>
                    <button class='btn primary btn-sm' id='btn-save-edit' onclick='saveAndExitEditMode()' title='保存修改并写回源文件 (Ctrl+S)'>" + IconCheck + @"<span>保存 (Ctrl+S)</span></button>
                    <button class='btn btn-sm' onclick='cancelEditMode()' title='放弃修改并返回预览 (Esc)'>" + IconClose + @"<span>取消</span></button>";
            }
            else
            {
                editToolbarButtons = @"
                    <button class='btn primary btn-sm' id='btn-save-edit' onclick='saveAndExitEditMode()' title='保存修改并写回源文件 (Ctrl+S)'>" + IconCheck + @"<span>保存 (Ctrl+S)</span></button>
                    <button class='btn btn-sm' onclick='cancelEditMode()' title='放弃修改并返回预览 (Esc)'>" + IconClose + @"<span>取消</span></button>";
            }

            var sb = new StringBuilder(16384);
            sb.Append(@"<!DOCTYPE html>
<html lang='zh-CN'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    ").Append(baseTag).Append(@"
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/katex@0.16.11/dist/katex.min.css' onerror='this.onerror=null;'>
    <script defer src='https://cdn.jsdelivr.net/npm/katex@0.16.11/dist/katex.min.js'></script>
    <script defer src='https://cdn.jsdelivr.net/npm/mermaid@10.9.1/dist/mermaid.min.js'></script>
    <script defer src='https://cdn.jsdelivr.net/npm/js-yaml@4.1.0/dist/js-yaml.min.js'></script>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/gh/highlightjs/cdn-release@11.9.0/build/styles/github.min.css' onerror='this.onerror=null;'>
    <script defer src='https://cdn.jsdelivr.net/gh/highlightjs/cdn-release@11.9.0/build/highlight.min.js'></script>
    <script defer src='https://cdn.jsdelivr.net/gh/highlightjs/cdn-release@11.9.0/build/languages/powershell.min.js'></script>
    <style>
        :root {
            --bg-body: #ffffff;
            --bg-topbar: #f8fafc;
            --bg-card: #f8fafc;
            --bg-code: #f1f5f9;
            --bg-table-th: #f1f5f9;
            --bg-table-stripe: #f8fafc;
            --border: #e2e8f0;
            --border-hover: #cbd5e1;
            --text-main: #1e293b;
            --text-muted: #64748b;
            --btn-bg: #ffffff;
            --btn-text: #334155;
            --btn-border: #cbd5e1;
            --btn-hover: #f1f5f9;
            --btn-primary-bg: #2563eb;
            --btn-primary-hover: #1d4ed8;
            --btn-accent-bg: #059669;
            --btn-accent-hover: #047857;
            --json-key: #0284c7;
            --json-colon: #64748b;
            --json-str: #16a34a;
            --json-num: #d97706;
            --json-bool: #7c3aed;
            --json-null: #64748b;
            --json-bracket: #0284c7;
            --json-tree-line: #e2e8f0;
            --json-tree-line-hover: #3b82f6;
            --badge-bg: #e0f2fe;
            --badge-text: #0369a1;
            --toast-bg: rgba(15, 23, 42, 0.88);
            --toast-text: #ffffff;
            --type-str-bg: #dcfce7;
            --type-str-text: #15803d;
            --type-str-border: #bbf7d0;
            --type-num-bg: #fef3c7;
            --type-num-text: #b45309;
            --type-num-border: #fde68a;
            --type-bool-bg: #f3e8ff;
            --type-bool-text: #7e22ce;
            --type-bool-border: #e9d5ff;
            --type-null-bg: #f1f5f9;
            --type-null-text: #475569;
            --type-null-border: #cbd5e1;
            --type-obj-bg: #e0f2fe;
            --type-obj-text: #0369a1;
            --type-obj-border: #bae6fd;
            --type-arr-bg: #ffedd5;
            --type-arr-text: #c2410c;
            --type-arr-border: #fed7aa;
            --scrollbar-thumb: rgba(148, 163, 184, 0.4);
            --scrollbar-thumb-hover: rgba(100, 116, 139, 0.75);
        }
        body.dark-mode {
            --bg-body: #1e1e1e;
            --bg-topbar: #252526;
            --bg-card: #252526;
            --bg-code: #2d2d30;
            --bg-table-th: #2d2d30;
            --bg-table-stripe: #252526;
            --border: #3e3e42;
            --border-hover: #55555a;
            --text-main: #d4d4d4;
            --text-muted: #858585;
            --btn-bg: #2d2d30;
            --btn-text: #cccccc;
            --btn-border: #3e3e42;
            --btn-hover: #3e3e40;
            --btn-primary-bg: #0e639c;
            --btn-primary-hover: #1177bb;
            --btn-accent-bg: #107c41;
            --btn-accent-hover: #158f4c;
            --json-key: #38bdf8;
            --json-colon: #94a3b8;
            --json-str: #4ade80;
            --json-num: #fbbf24;
            --json-bool: #c084fc;
            --json-null: #94a3b8;
            --json-bracket: #38bdf8;
            --json-tree-line: #334155;
            --json-tree-line-hover: #38bdf8;
            --badge-bg: #0c4a6e;
            --badge-text: #e0f2fe;
            --toast-bg: rgba(255, 255, 255, 0.92);
            --toast-text: #0f172a;
            --type-str-bg: rgba(74, 222, 128, 0.15);
            --type-str-text: #86efac;
            --type-str-border: rgba(74, 222, 128, 0.3);
            --type-num-bg: rgba(251, 191, 36, 0.15);
            --type-num-text: #fde68a;
            --type-num-border: rgba(251, 191, 36, 0.3);
            --type-bool-bg: rgba(192, 132, 252, 0.15);
            --type-bool-text: #e9d5ff;
            --type-bool-border: rgba(192, 132, 252, 0.3);
            --type-null-bg: rgba(148, 163, 184, 0.15);
            --type-null-text: #cbd5e1;
            --type-null-border: rgba(148, 163, 184, 0.3);
            --type-obj-bg: rgba(56, 189, 248, 0.15);
            --type-obj-text: #7dd3fc;
            --type-obj-border: rgba(56, 189, 248, 0.3);
            --type-arr-bg: rgba(251, 146, 60, 0.15);
            --type-arr-text: #fed7aa;
            --type-arr-border: rgba(251, 146, 60, 0.3);
            --scrollbar-thumb: rgba(255, 255, 255, 0.22);
            --scrollbar-thumb-hover: rgba(255, 255, 255, 0.5);
        }
        * { box-sizing: border-box; }

        /* Floating Overlay Scrollbar (悬浮滚动条 - 零占位与平滑自隐) */
        html {
            overflow-y: overlay;
            overflow-x: hidden;
            scrollbar-gutter: auto;
        }

        ::-webkit-scrollbar {
            width: 7px;
            height: 7px;
            background: transparent;
        }
        ::-webkit-scrollbar-track {
            background: transparent;
        }
        ::-webkit-scrollbar-thumb {
            background: transparent;
            border-radius: 999px;
            transition: background 0.25s ease;
        }

        /* 处于活跃滚动、鼠标靠近边缘或悬停在滚动条上时优雅浮现 */
        html.scrolling ::-webkit-scrollbar-thumb,
        body.scrolling ::-webkit-scrollbar-thumb,
        pre:hover::-webkit-scrollbar-thumb,
        table:hover::-webkit-scrollbar-thumb,
        .csv-container:hover::-webkit-scrollbar-thumb,
        ::-webkit-scrollbar-thumb:hover {
            background: var(--scrollbar-thumb);
        }

        ::-webkit-scrollbar-thumb:hover {
            background: var(--scrollbar-thumb-hover) !important;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'PingFang SC', 'Microsoft YaHei', sans-serif;
            margin: 0; padding: 46px 16px 20px 16px;
            background: var(--bg-body); color: var(--text-main); line-height: 1.6;
            -webkit-font-smoothing: antialiased;
            box-sizing: border-box;
        }

        /* 针对不同文档类型定制精准的“滚动到末尾行后”与视口策略 */
        body.fmt-markdown {
            /* Markdown: 启用优雅舒适的“滚动到末尾行后”特性，方便阅读底部内容与双向编辑 */
            padding-bottom: 35vh;
        }

        /* 智能就地编辑功能提示徽章 (Edit Tip Badge) */
        .edit-tip-badge {
            display: inline-flex; align-items: center; gap: 4px;
            font-size: 11px; font-weight: 500;
            padding: 3px 9px; border-radius: 12px;
            background: rgba(37, 99, 235, 0.08);
            color: #2563eb;
            border: 1px solid rgba(37, 99, 235, 0.22);
            cursor: help; user-select: none;
            white-space: nowrap; flex-shrink: 0;
            transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
        }
        body.dark-mode .edit-tip-badge {
            background: rgba(56, 189, 248, 0.12);
            color: #38bdf8;
            border-color: rgba(56, 189, 248, 0.28);
        }
        .edit-tip-badge:hover {
            background: rgba(37, 99, 235, 0.14);
            border-color: rgba(37, 99, 235, 0.45);
            transform: translateY(-0.5px);
        }
        .edit-tip-badge .tip-icon { font-size: 11px; }

        /* 智能图表与语法分析加载占位骨架屏 (Loading State Placeholder) */
        .omni-loading-placeholder {
            position: absolute;
            top: 0; left: 0; right: 0; bottom: 0;
            display: flex; flex-direction: column;
            align-items: center; justify-content: center;
            background: var(--bg-body);
            z-index: 40;
            pointer-events: none;
            transition: opacity 0.3s ease, visibility 0.3s ease;
        }
        .omni-loading-placeholder.loaded {
            opacity: 0;
            visibility: hidden;
        }
        .loading-radar-ring {
            width: 32px; height: 32px; border-radius: 50%;
            border: 2.5px solid rgba(59, 130, 246, 0.2);
            border-top-color: #3b82f6;
            animation: radarSpin 0.9s cubic-bezier(0.55, 0.15, 0.45, 0.85) infinite;
            margin-bottom: 10px;
        }
        body.dark-mode .loading-radar-ring {
            border-color: rgba(56, 189, 248, 0.2);
            border-top-color: #38bdf8;
        }
        @keyframes radarSpin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        .loading-title {
            font-size: 12.5px; font-weight: 600; color: var(--text-main);
            letter-spacing: 0.3px; margin-bottom: 4px;
        }
        .loading-sub {
            font-size: 11px; color: var(--text-muted);
        }

        /* 全格式通用视口中心加载与渲染遮罩屏 (Universal OmniView Loading Screen) */
        .omni-global-loading {
            position: fixed; top: 40px; left: 0; right: 0; bottom: 0;
            display: flex; align-items: center; justify-content: center;
            background: rgba(255, 255, 255, 0.88);
            backdrop-filter: blur(8px);
            -webkit-backdrop-filter: blur(8px);
            z-index: 9999;
            opacity: 1;
            visibility: visible;
            transition: opacity 0.26s cubic-bezier(0.16, 1, 0.3, 1), visibility 0.26s;
            pointer-events: auto;
        }
        body.dark-mode .omni-global-loading {
            background: rgba(24, 24, 27, 0.90);
        }
        body.zen-fullscreen .omni-global-loading {
            top: 0;
        }
        .omni-global-loading.hidden {
            opacity: 0;
            visibility: hidden;
            pointer-events: none;
        }
        .omni-global-loading .loading-box {
            display: flex; flex-direction: column; align-items: center; justify-content: center;
            padding: 24px 32px;
            background: rgba(255, 255, 255, 0.96);
            border: 1px solid rgba(226, 232, 240, 0.85);
            border-radius: 16px;
            box-shadow: 0 16px 40px -4px rgba(0, 0, 0, 0.12), 0 4px 14px -2px rgba(0, 0, 0, 0.05);
            transform: scale(1);
            transition: transform 0.24s cubic-bezier(0.16, 1, 0.3, 1);
        }
        body.dark-mode .omni-global-loading .loading-box {
            background: rgba(30, 41, 59, 0.94);
            border-color: rgba(255, 255, 255, 0.12);
            box-shadow: 0 20px 45px -4px rgba(0, 0, 0, 0.5), 0 6px 18px -2px rgba(0, 0, 0, 0.3);
        }
        .omni-global-loading.hidden .loading-box {
            transform: scale(0.94);
        }
        .loading-spinner-orbit {
            position: relative; width: 48px; height: 48px; margin-bottom: 14px;
            display: flex; align-items: center; justify-content: center;
        }
        .spinner-ring {
            position: absolute; width: 100%; height: 100%; border-radius: 50%;
            border: 3px solid rgba(14, 165, 233, 0.18);
            border-top-color: #0ea5e9; border-right-color: #3b82f6;
            animation: globalSpin 0.9s cubic-bezier(0.45, 0.05, 0.55, 0.95) infinite;
        }
        body.dark-mode .spinner-ring {
            border-color: rgba(56, 189, 248, 0.2);
            border-top-color: #38bdf8; border-right-color: #818cf8;
        }
        .spinner-ring-inner {
            position: absolute; width: 28px; height: 28px; border-radius: 50%;
            border: 2.5px solid transparent;
            border-bottom-color: #10b981; border-left-color: #06b6d4;
            animation: globalSpinRev 0.75s linear infinite;
        }
        body.dark-mode .spinner-ring-inner {
            border-bottom-color: #34d399; border-left-color: #22d3ee;
        }
        .spinner-dot {
            width: 8px; height: 8px; border-radius: 50%;
            background: #3b82f6;
            box-shadow: 0 0 10px #3b82f6;
            animation: spinnerPulse 1.2s ease-in-out infinite;
        }
        body.dark-mode .spinner-dot {
            background: #38bdf8;
            box-shadow: 0 0 12px #38bdf8;
        }
        @keyframes globalSpin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        @keyframes globalSpinRev {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(-360deg); }
        }
        @keyframes spinnerPulse {
            0%, 100% { transform: scale(0.8); opacity: 0.7; }
            50% { transform: scale(1.2); opacity: 1; }
        }
        .global-loading-title {
            font-size: 13.5px; font-weight: 600; color: var(--text-main);
            letter-spacing: 0.3px; margin-bottom: 4px; text-align: center;
        }
        .global-loading-sub {
            font-size: 11.5px; color: var(--text-muted); text-align: center;
        }

        /* 统一所有文档格式的面板与视口布局（对齐 HTML 标准贴顶全宽规格） */
        body.fmt-html, body.fmt-svg, body.fmt-mermaid, body.fmt-csv, body.fmt-markdown, body.fmt-json, body.fmt-yaml, body.fmt-xml, body.fmt-powershell {
            padding: 82px 0 0 0 !important;
            margin: 0 !important;
            box-sizing: border-box !important;
        }

        body.fmt-html, body.fmt-svg, body.fmt-mermaid, body.fmt-csv {
            overflow: hidden !important;
            height: 100vh !important;
            display: flex !important;
            flex-direction: column !important;
        }

        body.fmt-html .html-viewport-wrapper,
        body.fmt-svg .svg-viewport,
        body.fmt-mermaid .mermaid-viewport,
        body.fmt-csv .csv-container {
            flex: 1 !important;
            height: calc(100vh - 82px) !important;
            min-height: 0 !important;
            margin: 0 !important;
            border: none !important;
            border-radius: 0 !important;
            box-sizing: border-box !important;
        }

        body.fmt-csv .csv-container {
            overflow: auto !important;
        }

        body.fmt-markdown .md-body {
            padding: 16px 20px 35vh 20px !important;
        }

        body.fmt-json .json-container,
        body.fmt-yaml .json-container,
        body.fmt-xml .xml-container {
            padding: 12px 16px 35vh 16px !important;
        }

        body.fmt-powershell .ps-doc-container {
            padding: 16px 20px 35vh 20px !important;
        }

        body.fmt-code:not(.fmt-powershell) {
            padding: 46px 14px 20px 14px;
        }

        body.fmt-unknown {
            /* 提示卡片：固定居中无多余滚动条 */
            padding: 46px 16px 0 16px;
            overflow: hidden;
            height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        body.fmt-unknown .notice-card {
            margin: 0 auto;
        }

        /* Topbar Header */
        .topbar {
            position: fixed; top: 0; left: 0; right: 0; height: 42px;
            background: var(--bg-topbar); border-bottom: 1px solid var(--border);
            display: flex; align-items: center; justify-content: space-between;
            padding: 0 12px; z-index: 1000; box-shadow: 0 1px 3px rgba(0,0,0,0.04);
            position: fixed;
        }

        /* High-Performance Top Progress Bar (Laser Neon Streaming Style) */
        .top-loader {
            position: absolute; bottom: 0; left: 0; right: 0; height: 2.5px;
            overflow: hidden; pointer-events: none; opacity: 0;
            transition: opacity 0.2s ease;
            z-index: 1001;
        }
        .top-loader.active {
            opacity: 1;
        }
        .top-loader-bar {
            height: 100%; width: 100%;
            background: linear-gradient(90deg, #3b82f6, #06b6d4, #10b981, #a855f7, #3b82f6);
            background-size: 200% 100%;
            transform: translateX(-100%);
        }
        .top-loader.active .top-loader-bar {
            animation: loaderStream 1.1s cubic-bezier(0.4, 0, 0.2, 1) infinite;
        }
        @keyframes loaderStream {
            0% { transform: translateX(-100%); background-position: 0% 50%; }
            50% { transform: translateX(0%); background-position: 100% 50%; }
            100% { transform: translateX(100%); background-position: 0% 50%; }
        }
        @keyframes loaderProgress {
            0% { transform: translateX(-100%); }
            50% { transform: translateX(0%); }
            100% { transform: translateX(100%); }
        }

        /* Micro-Pulse Breathing Status Dot & Radar Wave */
        .pulse-dot {
            width: 7px; height: 7px; border-radius: 50%;
            background: #22c55e; display: inline-flex; align-items: center; justify-content: center;
            flex-shrink: 0; position: relative;
            box-shadow: 0 0 0 0 rgba(34, 197, 94, 0.6);
            transition: all 0.25s ease;
        }
        body.dark-mode .pulse-dot { background: #4ade80; box-shadow: 0 0 0 0 rgba(74, 222, 128, 0.6); }
        .pulse-dot.busy {
            background: #f59e0b !important;
            animation: pulseBusy 1s cubic-bezier(0.66, 0, 0, 1) infinite !important;
        }
        .pulse-dot .radar-ring {
            position: absolute; top: 50%; left: 50%;
            width: 100%; height: 100%; border-radius: 50%;
            transform: translate(-50%, -50%);
            pointer-events: none; opacity: 0;
        }
        .pulse-dot.busy .radar-ring {
            animation: radarWave 1.2s cubic-bezier(0, 0.2, 0.8, 1) infinite;
            border: 1.5px solid #f59e0b;
        }
        @keyframes radarWave {
            0% { width: 100%; height: 100%; opacity: 0.9; }
            100% { width: 320%; height: 320%; opacity: 0; }
        }
        @keyframes pulseBusy {
            0% { box-shadow: 0 0 0 0 rgba(245, 158, 11, 0.7); }
            70% { box-shadow: 0 0 0 6px rgba(245, 158, 11, 0); }
            100% { box-shadow: 0 0 0 0 rgba(245, 158, 11, 0); }
        }

        /* Badge Morphing Spring Animation */
        .badge-spring-animate {
            animation: badgeSpringPulse 0.36s cubic-bezier(0.175, 0.885, 0.32, 1.275);
        }
        @keyframes badgeSpringPulse {
            0% { transform: scale(0.92); }
            50% { transform: scale(1.08); }
            100% { transform: scale(1); }
        }

        /* Topbar Unknown Badge & Recognition HUD Overhaul */
        .badge.badge-unknown {
            background: rgba(148, 163, 184, 0.15) !important;
            color: var(--text-muted) !important;
            border: 1px solid rgba(148, 163, 184, 0.25) !important;
        }
        .recognition-hud {
            display: none !important;
        }
        .hud-radar-halo {
            position: relative; width: 14px; height: 14px;
            display: flex; align-items: center; justify-content: center;
        }
        .hud-radar-core {
            width: 6px; height: 6px; background: #38bdf8;
            border-radius: 50%; box-shadow: 0 0 6px #38bdf8;
        }
        .hud-radar-ring {
            position: absolute; width: 100%; height: 100%; border-radius: 50%;
            border: 1.5px solid #38bdf8;
            animation: hudRadarPulse 1.2s cubic-bezier(0, 0.2, 0.8, 1) infinite;
        }
        @keyframes hudRadarPulse {
            0% { transform: scale(0.5); opacity: 1; }
            100% { transform: scale(2.2); opacity: 0; }
        }
        .hud-text { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', system-ui, sans-serif; letter-spacing: 0.3px; }

        /* ==================== ZEN FULLSCREEN (全屏沉浸展示) ==================== */
        body.zen-fullscreen {
            padding: 0 !important;
            margin: 0 !important;
            overflow: hidden !important;
        }
        body.zen-fullscreen .topbar {
            top: -60px;
            left: 50%;
            transform: translateX(-50%);
            width: auto;
            min-width: 320px;
            max-width: 90vw;
            height: 38px;
            border-radius: 20px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.35);
            padding: 0 16px;
            border: 1px solid var(--border);
            background: var(--bg-topbar);
            backdrop-filter: blur(12px);
            -webkit-backdrop-filter: blur(12px);
            opacity: 0;
            pointer-events: none;
            transition: top 0.26s cubic-bezier(0.16, 1, 0.3, 1), opacity 0.26s ease;
            z-index: 9999;
        }
        body.zen-fullscreen .topbar.zen-revealed,
        body.zen-fullscreen .topbar:hover {
            top: 12px;
            opacity: 1;
            pointer-events: auto;
        }
        .zen-exit-pill {
            display: none;
        }
        body.zen-fullscreen .zen-exit-pill {
            display: inline-flex;
            position: fixed;
            top: 12px;
            right: 14px;
            z-index: 10000;
            align-items: center;
            gap: 6px;
            padding: 5px 12px;
            border-radius: 16px;
            font-size: 11.5px;
            font-weight: 600;
            color: var(--btn-text);
            background: var(--bg-card);
            border: 1px solid var(--border);
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.16);
            cursor: pointer;
            opacity: 0.45;
            transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
            user-select: none;
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
        }
        body.zen-fullscreen .zen-exit-pill:hover {
            opacity: 1;
            transform: scale(1.04);
            box-shadow: 0 6px 20px rgba(0, 0, 0, 0.28);
            border-color: var(--border-hover);
        }
        body.zen-fullscreen .md-body {
            max-width: 100% !important;
            padding: 24px 32px 32px 32px !important;
            height: 100vh;
            overflow-y: auto;
            margin: 0;
        }
        body.zen-fullscreen.fmt-csv {
            padding: 0 !important;
        }
        body.zen-fullscreen.fmt-csv .view-toolbar.csv-toolbar {
            top: 0 !important;
            margin: 0 !important;
            border-radius: 0 !important;
            border-left: none !important;
            border-right: none !important;
            border-top: none !important;
            border-bottom: 1px solid var(--border) !important;
            height: 40px !important;
        }
        body.zen-fullscreen .csv-container {
            top: 0 !important;
            height: calc(100vh - 40px) !important;
            border-radius: 0 !important;
            border: none !important;
        }
        body.zen-fullscreen .json-container {
            padding-top: 24px !important;
            height: 100vh !important;
        }
        body.zen-fullscreen.fmt-svg, body.zen-fullscreen.fmt-mermaid {
            padding: 0 !important;
        }
        body.zen-fullscreen .view-toolbar.svg-toolbar,
        body.zen-fullscreen .view-toolbar.mermaid-toolbar,
        body.zen-fullscreen .view-toolbar.html-toolbar {
            display: none !important;
        }
        body.zen-fullscreen .svg-viewport {
            top: 0 !important;
            height: 100vh !important;
            border-radius: 0 !important;
            border: none !important;
        }
        body.zen-fullscreen .mermaid-viewport,
        body.zen-fullscreen.fmt-mermaid .mermaid-viewport {
            top: 0 !important;
            height: 100vh !important;
            border-radius: 0 !important;
            border: none !important;
        }
        body.zen-fullscreen.fmt-html {
            padding: 0 !important;
        }
        body.zen-fullscreen .html-viewport-wrapper {
            top: 0 !important;
            height: 100vh !important;
            border-radius: 0 !important;
            border: none !important;
        }
        body.zen-fullscreen .code-viewer-container {
            top: 0 !important;
            height: 100vh !important;
            border-radius: 0 !important;
            border: none !important;
        }
        body.zen-fullscreen .omni-viewport-editor {
            top: 0 !important;
            height: 100vh !important;
            border-radius: 0 !important;
            border: none !important;
        }

        .badge {
            font-size: 11px; font-weight: 700; background: var(--badge-bg); color: var(--badge-text);
            padding: 3px 8px; border-radius: 4px; white-space: nowrap; letter-spacing: 0.5px;
            display: flex; align-items: center; gap: 6px;
        }
        .badge-markdown { background: #dbeafe; color: #1e40af; }
        .badge-json { background: #dcfce7; color: #166534; }
        .badge-csv { background: #ffedd5; color: #9a3412; }
        .badge-svg { background: #fce7f3; color: #9d174d; }
        .badge-yaml { background: #e0e7ff; color: #3730a3; }
        .badge-mermaid { background: #ccfbf1; color: #115e59; }
        .badge-html { background: #ffedd5; color: #c2410c; }
        .badge-code { background: #e0f2fe; color: #0369a1; }
        body.dark-mode .badge-markdown { background: #1e3a8a; color: #bfdbfe; }
        body.dark-mode .badge-json { background: #14532d; color: #bbf7d0; }
        body.dark-mode .badge-csv { background: #7c2d12; color: #fed7aa; }
        body.dark-mode .badge-svg { background: #831843; color: #fbcfe8; }
        body.dark-mode .badge-yaml { background: #312e81; color: #c7d2fe; }
        body.dark-mode .badge-mermaid { background: #134e4a; color: #99f6e4; }
        body.dark-mode .badge-html { background: #7c2d12; color: #fed7aa; }
        body.dark-mode .badge-code { background: #082f49; color: #38bdf8; }

        /* Smooth Container Fade & Micro Transitions */
        .md-body, .json-container, .csv-container, .svg-viewport, .html-viewport-wrapper, .notice-card, .code-viewer-container {
            animation: omniFadeIn 0.22s cubic-bezier(0.16, 1, 0.3, 1) forwards;
        }
        @keyframes omniFadeIn {
            from { opacity: 0; transform: translateY(4px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .actions { display: flex; gap: 6px; align-items: center; }
        .btn {
            background: var(--btn-bg); border: 1px solid var(--btn-border); color: var(--btn-text);
            padding: 4px 10px; border-radius: 5px; font-size: 12px; cursor: pointer; white-space: nowrap;
            transition: all 0.15s ease; font-weight: 500;
            display: inline-flex; align-items: center; gap: 5px;
        }
        .btn:active, .btn-pill:active {
            transform: scale(0.97);
        }
        .btn:hover { background: var(--btn-hover); border-color: var(--border-hover); }
        .btn.primary { background: var(--btn-primary-bg); color: #fff; border-color: var(--btn-primary-bg); }
        .btn.primary:hover { background: var(--btn-primary-hover); }
        .btn.accent { background: var(--btn-accent-bg); color: #fff; border-color: var(--btn-accent-bg); }
        .btn.accent:hover { background: var(--btn-accent-hover); }
        .btn-icon { flex-shrink: 0; }

        /* Modern Hover Dropdown Menu (转换悬浮下拉框) */
        .dropdown { position: relative; display: inline-block; }
        .btn-dropdown { display: inline-flex; align-items: center; gap: 5px; }
        .chevron-icon {
            transition: transform 0.2s cubic-bezier(0.4, 0, 0.2, 1);
            display: inline-block; flex-shrink: 0;
        }
        .dropdown:hover .chevron-icon { transform: rotate(180deg); }
        .dropdown::after {
            content: ''; position: absolute; top: 100%; left: 0; right: 0; height: 8px;
        }
        .dropdown-menu {
            display: none; position: absolute; top: calc(100% + 4px); right: 0;
            min-width: 260px; background: var(--bg-card); border: 1px solid var(--border);
            border-radius: 8px; box-shadow: 0 10px 25px -5px rgba(0,0,0,0.12), 0 8px 10px -6px rgba(0,0,0,0.08);
            padding: 5px; z-index: 2000;
        }
        body.dark-mode .dropdown-menu {
            box-shadow: 0 12px 30px rgba(0,0,0,0.55), 0 0 0 1px rgba(255,255,255,0.06);
        }
        .dropdown:hover .dropdown-menu { display: block; animation: dropFade 0.18s cubic-bezier(0.16, 1, 0.3, 1); }
        @keyframes dropFade {
            from { opacity: 0; transform: translateY(-5px) scale(0.98); }
            to { opacity: 1; transform: translateY(0) scale(1); }
        }
        .dropdown-group { display: flex; flex-direction: column; gap: 2px; }
        .dropdown-header {
            font-size: 10.5px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.6px;
            color: var(--text-muted); padding: 5px 9px 3px 9px; user-select: none;
        }
        .dropdown-divider { height: 1px; background: var(--border); margin: 4px 6px; }
        .dropdown-item {
            display: flex; align-items: center; gap: 9px; width: 100%; padding: 6px 8px;
            border: none; background: transparent; color: var(--text-main); font-size: 12px;
            text-align: left; border-radius: 6px; cursor: pointer; white-space: nowrap;
            transition: all 0.12s ease;
        }
        .dropdown-item:hover { background: var(--btn-hover); color: var(--btn-primary-bg); }
        body.dark-mode .dropdown-item:hover { background: #333338; color: #38bdf8; }

        .item-icon {
            width: 26px; height: 26px; display: inline-flex; align-items: center; justify-content: center;
            border-radius: 5px; background: var(--bg-code); color: var(--text-muted);
            flex-shrink: 0; transition: transform 0.15s ease;
        }
        .item-icon.icon-json { color: #0284c7; background: #e0f2fe; }
        .item-icon.icon-csv { color: #d97706; background: #fef3c7; }
        .item-icon.icon-md { color: #2563eb; background: #dbeafe; }
        .item-icon.icon-html { color: #ea580c; background: #ffedd5; }
        .item-icon.icon-svg { color: #db2777; background: #fce7f3; }
        .item-icon.icon-yaml { color: #4f46e5; background: #e0e7ff; }
        .item-icon.icon-mermaid { color: #0d9488; background: #ccfbf1; }
        .item-icon.icon-code { color: #0284c7; background: #e0f2fe; }
        body.dark-mode .item-icon.icon-json { color: #38bdf8; background: #082f49; }
        body.dark-mode .item-icon.icon-csv { color: #f59e0b; background: #451a03; }
        body.dark-mode .item-icon.icon-md { color: #60a5fa; background: #172554; }
        body.dark-mode .item-icon.icon-html { color: #fb923c; background: #431407; }
        body.dark-mode .item-icon.icon-svg { color: #f472b6; background: #500724; }
        body.dark-mode .item-icon.icon-yaml { color: #818cf8; background: #1e1b4b; }
        body.dark-mode .item-icon.icon-mermaid { color: #2dd4bf; background: #042f2e; }
        body.dark-mode .item-icon.icon-code { color: #38bdf8; background: #082f49; }

        /* Interactive SVG & Mermaid Viewport */
        .svg-viewport {
            width: 100%; height: 100%; min-height: 0;
            position: relative; overflow: hidden; border: none; border-radius: 0;
            display: flex; align-items: center; justify-content: center;
            background-color: var(--bg-card);
            user-select: none;
            cursor: grab;
            transition: background-color 0.2s ease;
        }
        .svg-viewport:active { cursor: grabbing; }

        /* 通用无底色棋盘格图案 (SVG 与 Mermaid 共享) */
        .svg-viewport.bg-none, .mermaid-viewport.bg-none {
            background-color: var(--bg-card) !important;
            background-image: linear-gradient(45deg, rgba(0,0,0,0.06) 25%, transparent 25%),
                              linear-gradient(-45deg, rgba(0,0,0,0.06) 25%, transparent 25%),
                              linear-gradient(45deg, transparent 75%, rgba(0,0,0,0.06) 75%),
                              linear-gradient(-45deg, transparent 75%, rgba(0,0,0,0.06) 75%) !important;
            background-size: 20px 20px !important;
            background-position: 0 0, 0 10px, 10px -10px, -10px 0px !important;
        }
        body.dark-mode .svg-viewport.bg-none, body.dark-mode .mermaid-viewport.bg-none {
            background-image: linear-gradient(45deg, rgba(255,255,255,0.05) 25%, transparent 25%),
                              linear-gradient(-45deg, rgba(255,255,255,0.05) 25%, transparent 25%),
                              linear-gradient(45deg, transparent 75%, rgba(255,255,255,0.05) 75%),
                              linear-gradient(-45deg, transparent 75%, rgba(255,255,255,0.05) 75%) !important;
        }

        /* Mermaid Viewport 背景定制与舒适可视对齐 */
        .mermaid-viewport {
            background-color: #ffffff;
            background-image: none;
            cursor: grab;
            transition: background-color 0.2s ease;
            align-items: flex-start !important;
            justify-content: center !important;
            padding: 0 !important;
        }
        body.dark-mode .mermaid-viewport {
            background-color: #121214;
            background-image: none;
        }

        /* Mermaid Canvas: 消除 GPU 栅格化模糊与强制重采样，保持原生矢量高清晰度与顶部居中缩放锚点 */
        .mermaid-canvas {
            display: inline-flex; align-items: center; justify-content: center;
            transform-origin: top center;
            pointer-events: none;
            transition: transform 0.1s cubic-bezier(0.2, 0.9, 0.3, 1), opacity 0.25s ease;
            opacity: 0; /* 默认未渲染完成时隐藏，避免先显示原始代码文本 */
        }
        .mermaid-canvas.rendered {
            opacity: 1 !important;
        }
        .mermaid-canvas.is-panning {
            transition: none !important;
        }
        .mermaid-canvas svg {
            display: block; pointer-events: auto;
            shape-rendering: geometricPrecision;
            text-rendering: geometricPrecision;
            overflow: visible !important;
        }
        .mermaid-canvas foreignObject {
            overflow: visible !important;
        }
        .mermaid-canvas foreignObject div {
            overflow: visible !important;
            word-break: break-word !important;
            line-height: 1.35 !important;
        }
        .mermaid-canvas .node .label,
        .mermaid-canvas .node text,
        .mermaid-canvas .nodeLabel,
        .mermaid-canvas .cluster text,
        .mermaid-canvas .actor text,
        .mermaid-canvas text {
            font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Microsoft YaHei"", ""PingFang SC"", sans-serif !important;
            overflow: visible !important;
        }
        .mermaid-canvas .node, .mermaid-canvas .actor {
            cursor: pointer;
        }
        .mermaid-canvas .node:hover rect,
        .mermaid-canvas .node:hover circle,
        .mermaid-canvas .node:hover polygon,
        .mermaid-canvas .node:hover path {
            filter: brightness(1.08);
            stroke-width: 2px !important;
        }
        #mermaid-root {
            display: inline-block;
            padding: 24px;
            background: transparent !important;
        }
        #mermaid-root:not([data-processed]) {
            visibility: hidden !important;
        }

        .svg-canvas {
            display: inline-flex; align-items: center; justify-content: center;
            transform-origin: center center;
            pointer-events: none;
            will-change: transform;
            backface-visibility: hidden;
            -webkit-backface-visibility: hidden;
            transition: transform 0.1s cubic-bezier(0.2, 0.9, 0.3, 1);
        }
        .svg-canvas.is-panning {
            transition: none !important;
        }
        .svg-canvas svg {
            display: block; max-width: none; max-height: none; pointer-events: auto;
            shape-rendering: geometricPrecision;
            text-rendering: geometricPrecision;
        }

        /* 调色盘与颜色徽章组件 */
        .palette-dropdown-wrap {
            position: relative;
            display: inline-flex;
            align-items: center;
            z-index: 1050;
        }
        .color-swatch-badge {
            width: 12px; height: 12px; border-radius: 50%;
            border: 1px solid rgba(0,0,0,0.25);
            display: inline-block; flex-shrink: 0; box-sizing: border-box;
            vertical-align: middle;
        }
        body.dark-mode .color-swatch-badge {
            border-color: rgba(255,255,255,0.35);
        }
        .color-swatch-badge.swatch-none {
            background: linear-gradient(45deg, #bbb 25%, transparent 25%),
                        linear-gradient(-45deg, #bbb 25%, transparent 25%),
                        linear-gradient(45deg, transparent 75%, #bbb 75%),
                        linear-gradient(-45deg, transparent 75%, #bbb 75%);
            background-size: 6px 6px;
            background-position: 0 0, 0 3px, 3px -3px, -3px 0;
            background-color: #fff;
        }
        .palette-popover {
            position: absolute; top: calc(100% + 6px); left: 0;
            min-width: 240px; background: var(--bg-card);
            border: 1px solid var(--border); border-radius: 8px;
            box-shadow: 0 10px 25px -3px rgba(0, 0, 0, 0.25), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
            z-index: 9999 !important; padding: 10px; display: none; user-select: none;
            backdrop-filter: blur(12px); -webkit-backdrop-filter: blur(12px);
            animation: popoverFadeIn 0.15s cubic-bezier(0.16, 1, 0.3, 1);
        }
        body.dark-mode .palette-popover {
            background: #1c1c20; border-color: #3f3f46;
            box-shadow: 0 14px 32px rgba(0, 0, 0, 0.6);
        }
        @keyframes popoverFadeIn {
            from { opacity: 0; transform: translateY(-4px); }
            to { opacity: 1; transform: translateY(0); }
        }
        .palette-popover.open { display: block; }
        .palette-header-row {
            display: flex; align-items: center; justify-content: space-between;
            margin-bottom: 8px;
        }
        .palette-header {
            font-size: 11px; font-weight: 600; color: var(--text-muted);
            text-transform: uppercase; letter-spacing: 0.5px;
        }
        .btn-palette-close {
            background: none; border: none; padding: 2px;
            cursor: pointer; color: var(--text-muted);
            border-radius: 4px; display: inline-flex;
            align-items: center; justify-content: center;
            line-height: 1; transition: all 0.12s ease;
        }
        .btn-palette-close:hover {
            color: var(--text-main); background: var(--btn-hover);
        }
        .palette-grid {
            display: grid; grid-template-columns: repeat(3, 1fr); gap: 6px;
            margin-bottom: 8px;
        }
        .palette-chip {
            display: flex; align-items: center; gap: 6px; padding: 5px 6px;
            border-radius: 5px; border: 1px solid var(--border);
            background: var(--btn-bg); cursor: pointer; font-size: 11px;
            color: var(--text-main); transition: all 0.12s ease;
        }
        .palette-chip:hover {
            background: var(--btn-hover); border-color: var(--border-hover);
        }
        .palette-chip.active {
            border-color: var(--btn-primary-bg); background: var(--badge-bg);
            color: var(--badge-text); font-weight: 600;
        }
        .palette-divider {
            height: 1px; background: var(--border); margin: 8px 0;
        }
        .palette-custom-row {
            display: flex; align-items: center; justify-content: space-between; gap: 8px;
        }
        .palette-custom-label {
            font-size: 11.5px; color: var(--text-main); font-weight: 500;
        }
        .palette-custom-pickers {
            display: flex; align-items: center; gap: 6px;
        }
        .native-color-wrap {
            position: relative; width: 24px; height: 24px; border-radius: 5px;
            overflow: hidden; border: 1px solid var(--border); cursor: pointer;
            box-shadow: 0 1px 2px rgba(0,0,0,0.06); background: #ffffff;
        }
        .native-color-wrap input[type='color'] {
            position: absolute; top: -6px; left: -6px; width: 36px; height: 36px;
            border: none; cursor: pointer; background: none;
        }
        .hex-input {
            width: 70px; padding: 3px 6px; font-size: 11px; font-family: monospace;
            border: 1px solid var(--border); border-radius: 4px; color: var(--text-main);
            background: var(--btn-bg); text-transform: uppercase;
        }
        .hex-input:focus {
            outline: none; border-color: var(--btn-primary-bg);
        }

        /* HTML Responsive Viewport & Resolution Engine */
        .html-viewport-wrapper {
            width: 100%;
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 16px;
            overflow: auto;
            box-sizing: border-box;
            background: var(--bg-body);
        }
        .html-viewport-stage {
            position: relative;
            display: block;
            margin: auto;
            flex-shrink: 0;
            transition: width 0.2s cubic-bezier(0.16, 1, 0.3, 1), height 0.2s cubic-bezier(0.16, 1, 0.3, 1);
        }
        #html-device-frame {
            border: 1px solid var(--border);
            border-radius: 8px;
            background: #ffffff;
            box-shadow: 0 4px 20px rgba(0,0,0,0.08);
            overflow: hidden;
            transform-origin: 0 0;
            transition: transform 0.2s cubic-bezier(0.16, 1, 0.3, 1), box-shadow 0.2s ease;
        }
        body.dark-mode #html-device-frame { box-shadow: 0 6px 24px rgba(0,0,0,0.45); }
        #html-device-frame.is-mobile {
            border: 9px solid #1e293b;
            border-radius: 36px;
            box-shadow: 0 8px 32px rgba(0,0,0,0.28);
        }
        body.dark-mode #html-device-frame.is-mobile {
            border-color: #334155;
            box-shadow: 0 8px 32px rgba(0,0,0,0.65);
        }
        #html-preview-frame {
            width: 100%; height: 100%; border: none; display: block; background: #ffffff;
        }

        .dropdown-item:hover .item-icon { transform: scale(1.06); }
        .item-content { display: flex; flex-direction: column; gap: 1px; flex: 1; }
        .item-title { font-size: 12px; font-weight: 500; line-height: 1.3; color: var(--text-main); white-space: nowrap; }
        .item-subtitle { font-size: 10.5px; color: var(--text-muted); line-height: 1.2; white-space: nowrap; }
        .dropdown-item:hover .item-title { color: var(--btn-primary-bg); }
        body.dark-mode .dropdown-item:hover .item-title { color: #38bdf8; }

        .item-action-tag {
            font-size: 10px; font-weight: 600; padding: 2px 6px; border-radius: 4px;
            background: var(--bg-code); color: var(--text-muted); letter-spacing: 0.3px;
            margin-left: 8px; flex-shrink: 0;
        }
        .dropdown-item:hover .item-action-tag { background: var(--badge-bg); color: var(--btn-primary-bg); }

        /* Secondary Sub-toolbar (统一所有格式的绝对置顶次级工具栏规范，完全对齐 HTML 视图) */
        .view-toolbar {
            position: fixed !important;
            top: 42px !important;
            left: 0 !important;
            right: 0 !important;
            z-index: 990 !important;
            margin: 0 !important;
            padding: 5px 12px !important;
            height: 40px !important;
            border-radius: 0 !important;
            border-left: none !important;
            border-right: none !important;
            border-top: none !important;
            border-bottom: 1px solid var(--border) !important;
            background: var(--bg-card) !important;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.03) !important;
            display: flex !important;
            align-items: center !important;
            justify-content: space-between !important;
            flex-wrap: nowrap !important;
            box-sizing: border-box !important;
            flex-shrink: 0 !important;
            overflow-x: auto !important;
            overflow-y: hidden !important;
        }
        .view-toolbar::-webkit-scrollbar { display: none; }
        body.dark-mode .view-toolbar {
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.25) !important;
        }
        .toolbar-group {
            display: flex !important;
            align-items: center !important;
            gap: 6px !important;
            flex-wrap: nowrap !important;
            white-space: nowrap !important;
            overflow: visible !important;
            flex-shrink: 0 !important;
        }
        .toolbar-group::-webkit-scrollbar { display: none; }
        .btn-pill {
            background: var(--btn-bg); border: 1px solid var(--btn-border); color: var(--btn-text);
            padding: 3px 8px; border-radius: 4px; font-size: 11px; cursor: pointer;
            transition: all 0.15s ease; display: inline-flex; align-items: center; gap: 4px;
            white-space: nowrap; flex-shrink: 0; user-select: none;
        }
        .btn-pill:hover { background: var(--btn-hover); }
        .btn-pill.active {
            background: var(--btn-primary-bg); color: #fff; border-color: var(--btn-primary-bg);
        }
        .pill-dot { width: 6px; height: 6px; border-radius: 50%; background: #22c55e; }
        .format-pill {
            font-size: 11px; font-weight: 600; padding: 3px 8px; border-radius: 4px;
            background: var(--badge-bg); color: var(--badge-text);
            white-space: nowrap; flex-shrink: 0;
        }
        .zoom-badge {
            font-size: 11px; font-weight: 700; padding: 3px 8px; border-radius: 4px;
            background: var(--badge-bg); color: var(--badge-text);
            display: inline-flex; align-items: center; justify-content: center;
            white-space: nowrap; min-width: 44px; letter-spacing: 0.3px; flex-shrink: 0;
        }
        .search-box-wrap {
            display: flex; align-items: center; gap: 6px;
            flex: 1 1 160px; min-width: 140px; max-width: 380px;
        }
        .search-box-wrap input {
            flex: 1; min-width: 80px; padding: 4px 8px; border: 1px solid var(--border); border-radius: 4px;
            background: var(--bg-body); color: var(--text-main); font-size: 12px;
            outline: none; transition: border-color 0.15s ease;
        }
        .search-box-wrap input:focus { border-color: var(--btn-primary-bg); }
        .stats-tag { font-size: 11px; color: var(--text-muted); white-space: nowrap; flex-shrink: 0; }

        /* CSV 专属分隔符交互徽章与按钮组 */
        .delim-pill-btn {
            background: var(--btn-bg);
            border: 1px solid var(--btn-border);
            color: var(--text-main);
            padding: 3px 9px;
            border-radius: 5px;
            font-size: 11.5px;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 5px;
            transition: all 0.15s ease;
            user-select: none;
            box-shadow: 0 1px 2px rgba(0, 0, 0, 0.02);
        }
        .delim-pill-btn:hover {
            border-color: var(--btn-primary-bg);
            background: rgba(37, 99, 235, 0.08);
            color: var(--btn-primary-bg);
        }
        .delim-pill-btn .pill-prefix {
            font-weight: 500;
            opacity: 0.7;
            font-size: 10.5px;
        }
        .delim-pill-btn .pill-val {
            font-weight: 700;
            letter-spacing: 0.2px;
        }

        /* CSV 专属搜索框组与一键清空按钮 */
        .csv-search-wrap {
            display: flex;
            align-items: center;
            gap: 8px;
            flex: 0 1 340px;
            min-width: 130px;
        }
        .search-input-inner {
            position: relative;
            display: flex;
            align-items: center;
            flex: 1;
            min-width: 100px;
        }
        .search-input-inner .search-icon {
            position: absolute;
            left: 8px;
            color: var(--text-muted);
            pointer-events: none;
            opacity: 0.7;
            transition: color 0.15s ease;
        }
        .search-input-inner input {
            width: 100%;
            height: 28px;
            box-sizing: border-box;
            padding: 3px 26px 3px 26px;
            border: 1px solid var(--border);
            border-radius: 5px;
            background: var(--bg-body);
            color: var(--text-main);
            font-size: 12px;
            outline: none;
            transition: all 0.15s ease;
        }
        .search-input-inner input:focus {
            border-color: var(--btn-primary-bg);
            box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.15);
        }
        .search-input-inner:focus-within .search-icon {
            color: var(--btn-primary-bg);
            opacity: 1;
        }
        .btn-clear-search {
            position: absolute;
            right: 5px;
            width: 18px;
            height: 18px;
            border-radius: 50%;
            border: none;
            background: transparent;
            color: var(--text-muted);
            display: inline-flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            padding: 0;
            transition: all 0.15s ease;
        }
        .btn-clear-search:hover {
            background: var(--border);
            color: var(--text-main);
        }
        .csv-stats-badge {
            font-size: 11px;
            color: var(--text-muted);
            white-space: nowrap;
            background: var(--badge-bg);
            padding: 3px 8px;
            border-radius: 4px;
            font-weight: 500;
            letter-spacing: 0.2px;
        }
        .csv-stats-badge .stats-filtered {
            color: var(--btn-primary-bg);
            font-weight: 700;
        }

        /* Markdown Body Styling */
        /* Markdown Body Styling */
        .md-body { font-size: 14px; line-height: 1.7; }
        .md-body h1, .md-body h2, .md-body h3, .md-body h4, .md-body h5, .md-body h6, .md-body [id] {
            scroll-margin-top: 56px;
        }
        .md-body h1, .md-body h2, .md-body h3, .md-body h4 {
            color: var(--text-main); font-weight: 600; margin-top: 1.2em; margin-bottom: 0.6em;
            border-bottom: 1px solid var(--border); padding-bottom: 0.3em;
            transition: background 0.3s ease;
        }
        .md-body h1 { font-size: 1.8em; }
        .md-body h2 { font-size: 1.4em; }
        .md-body h3 { font-size: 1.15em; border-bottom: none; }
        .md-body p { margin: 0.8em 0; }
        .md-body blockquote {
            margin: 14px 0; padding: 6px 16px; color: var(--text-muted);
            border-left: 4px solid var(--btn-primary-bg); background: var(--bg-card);
            border-radius: 0 6px 6px 0;
        }
        .md-body ul, .md-body ol { padding-left: 24px; margin: 0.6em 0; }
        .md-body li { margin: 0.3em 0; }
        .md-body hr { border: none; border-top: 1px solid var(--border); margin: 24px 0; }

        /* Links & Table of Contents (TOC) Navigation Styling */
        .md-body a {
            color: var(--btn-primary-bg); text-decoration: none;
            border-bottom: 1px dashed transparent; transition: all 0.15s ease;
            padding: 1px 3px; border-radius: 3px;
        }
        .md-body a:hover {
            color: var(--btn-primary-hover);
            background: rgba(37, 99, 235, 0.08);
            border-bottom-color: var(--btn-primary-hover);
        }
        body.dark-mode .md-body a { color: #58a6ff; }
        body.dark-mode .md-body a:hover { color: #79c0ff; background: rgba(56, 189, 248, 0.14); }

        /* TOC List & Anchor Tree Enhancements */
        .md-body a[href^='#'] {
            display: inline-flex; align-items: center; gap: 4px;
            font-weight: 500; font-size: 13.5px;
        }
        .md-body a[href^='#']::before {
            content: ''; display: inline-block; width: 6px; height: 6px;
            border-radius: 50%; background: var(--border-hover);
            margin-right: 3px; transition: all 0.18s ease; flex-shrink: 0;
        }
        .md-body a[href^='#']:hover::before {
            background: var(--btn-primary-bg); transform: scale(1.35);
        }
        body.dark-mode .md-body a[href^='#']:hover::before {
            background: #58a6ff;
        }

        /* Target Anchor Pulse Highlight */
        .anchor-target-highlight {
            animation: anchorPulse 1.8s cubic-bezier(0.16, 1, 0.3, 1) forwards !important;
        }
        @keyframes anchorPulse {
            0% { background: rgba(37, 99, 235, 0.22); border-radius: 4px; box-shadow: 0 0 0 6px rgba(37, 99, 235, 0.22); }
            60% { background: rgba(37, 99, 235, 0.10); border-radius: 4px; box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.10); }
            100% { background: transparent; box-shadow: none; }
        }
        body.dark-mode .anchor-target-highlight {
            animation: anchorPulseDark 1.8s cubic-bezier(0.16, 1, 0.3, 1) forwards !important;
        }
        @keyframes anchorPulseDark {
            0% { background: rgba(56, 189, 248, 0.26); border-radius: 4px; box-shadow: 0 0 0 6px rgba(56, 189, 248, 0.26); }
            60% { background: rgba(56, 189, 248, 0.12); border-radius: 4px; box-shadow: 0 0 0 3px rgba(56, 189, 248, 0.12); }
            100% { background: transparent; box-shadow: none; }
        }

        /* GitHub-Style 5-Color Alert Callouts (markdown-alert) */
        .markdown-alert {
            padding: 12px 16px; margin: 16px 0; border-radius: 6px;
            border-left: 4.5px solid var(--alert-border, #0969da);
            background: var(--alert-bg, rgba(9, 105, 218, 0.08));
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.02);
            transition: background 0.2s ease, border-color 0.2s ease;
        }
        .markdown-alert-title {
            display: flex; align-items: center; gap: 7px;
            font-size: 13px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.3px;
            color: var(--alert-title, #0969da); margin: 0 0 6px 0; user-select: none;
        }
        .markdown-alert-title svg {
            width: 16px; height: 16px; fill: currentColor; flex-shrink: 0;
            display: inline-block; vertical-align: text-bottom;
        }
        .markdown-alert p {
            margin: 0.35em 0; color: var(--text-main); font-size: 13.5px; line-height: 1.6;
        }
        .markdown-alert > :first-child { margin-top: 0; }
        .markdown-alert > :last-child { margin-bottom: 0; }

        /* Specific Alert Types */
        .markdown-alert-note {
            --alert-border: #0969da;
            --alert-title: #0969da;
            --alert-bg: rgba(9, 105, 218, 0.07);
        }
        .markdown-alert-tip {
            --alert-border: #1a7f37;
            --alert-title: #1a7f37;
            --alert-bg: rgba(26, 127, 55, 0.07);
        }
        .markdown-alert-important {
            --alert-border: #8250df;
            --alert-title: #8250df;
            --alert-bg: rgba(130, 80, 223, 0.07);
        }
        .markdown-alert-warning {
            --alert-border: #9a6700;
            --alert-title: #9a6700;
            --alert-bg: rgba(154, 103, 0, 0.08);
        }
        .markdown-alert-caution {
            --alert-border: #cf222e;
            --alert-title: #cf222e;
            --alert-bg: rgba(207, 34, 46, 0.07);
        }

        /* Dark Mode Alert Theme Overrides */
        body.dark-mode .markdown-alert-note {
            --alert-border: #2f81f7;
            --alert-title: #2f81f7;
            --alert-bg: rgba(56, 139, 253, 0.12);
        }
        body.dark-mode .markdown-alert-tip {
            --alert-border: #3fb950;
            --alert-title: #3fb950;
            --alert-bg: rgba(46, 160, 67, 0.12);
        }
        body.dark-mode .markdown-alert-important {
            --alert-border: #a371f7;
            --alert-title: #a371f7;
            --alert-bg: rgba(163, 113, 247, 0.12);
        }
        body.dark-mode .markdown-alert-warning {
            --alert-border: #d29922;
            --alert-title: #d29922;
            --alert-bg: rgba(187, 128, 9, 0.12);
        }
        body.dark-mode .markdown-alert-caution {
            --alert-border: #f85149;
            --alert-title: #f85149;
            --alert-bg: rgba(248, 81, 73, 0.12);
        }

        /* ==================== POWERSHELL RICH VIEWPORT & DOCUMENTATION STYLES ==================== */
        body.fmt-powershell {
            background-color: var(--bg-body);
        }
        .ps-format-pill {
            background: rgba(0, 120, 212, 0.12) !important;
            color: #0078d4 !important;
            font-weight: 700 !important;
            border: 1px solid rgba(0, 120, 212, 0.25) !important;
        }
        body.dark-mode .ps-format-pill {
            background: rgba(56, 189, 248, 0.15) !important;
            color: #38bdf8 !important;
            border-color: rgba(56, 189, 248, 0.3) !important;
        }
        .ps-segmented-group {
            display: inline-flex; align-items: center;
            background: var(--bg-body); border: 1px solid var(--border);
            border-radius: 6px; padding: 2px; gap: 2px;
        }
        .ps-segmented-group .btn-pill {
            border: none; background: transparent; border-radius: 4px; padding: 3px 10px;
            font-size: 11.5px; font-weight: 500; color: var(--text-muted);
        }
        .ps-segmented-group .btn-pill:hover {
            color: var(--text-main); background: rgba(0,0,0,0.04);
        }
        body.dark-mode .ps-segmented-group .btn-pill:hover {
            background: rgba(255,255,255,0.06);
        }
        .ps-segmented-group .btn-pill.active {
            background: var(--btn-primary-bg) !important; color: #ffffff !important;
            box-shadow: 0 1px 3px rgba(0,0,0,0.12);
        }
        .ps-req-badge {
            background: rgba(99, 102, 241, 0.1); color: #6366f1;
            border: 1px solid rgba(99, 102, 241, 0.2); border-radius: 4px; padding: 2px 6px; font-size: 11px;
        }
        body.dark-mode .ps-req-badge {
            background: rgba(129, 140, 248, 0.15); color: #a5b4fc; border-color: rgba(129, 140, 248, 0.3);
        }
        .ps-warn-badge {
            background: rgba(239, 68, 68, 0.12); color: #ef4444; font-weight: 600;
            border: 1px solid rgba(239, 68, 68, 0.25); border-radius: 4px; padding: 2px 7px; font-size: 11px;
            animation: badgePulse 2s infinite ease-in-out;
        }
        @keyframes badgePulse { 0%, 100% { opacity: 1; } 50% { opacity: 0.7; } }

        /* Document Mode Layout */
        .ps-doc-container {
            width: 100%; max-width: 1100px; margin: 0 auto;
            padding: 0 4px 60px 4px; box-sizing: border-box;
            display: flex; flex-direction: column; gap: 20px;
        }
        .ps-hero-card {
            background: var(--bg-card); border: 1px solid var(--border);
            border-radius: 10px; padding: 20px 22px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.03);
            display: flex; flex-direction: column; gap: 14px;
        }
        body.dark-mode .ps-hero-card { box-shadow: 0 4px 16px rgba(0,0,0,0.25); }
        .ps-hero-header {
            display: flex; align-items: flex-start; justify-content: space-between; gap: 16px; flex-wrap: wrap;
        }
        .ps-title-wrap {
            display: flex; align-items: flex-start; gap: 12px; flex: 1 1 300px;
        }
        .ps-hero-icon {
            width: 36px; height: 36px; border-radius: 8px; flex-shrink: 0;
            background: linear-gradient(135deg, #0284c7, #0369a1); color: #ffffff;
            display: flex; align-items: center; justify-content: center;
            box-shadow: 0 3px 8px rgba(2, 132, 199, 0.3);
        }
        .ps-hero-icon svg { width: 20px; height: 20px; }
        .ps-script-title {
            margin: 0; font-size: 20px; font-weight: 700; color: var(--text-main); line-height: 1.25;
            word-break: break-all;
        }
        .ps-hero-badges {
            display: flex; align-items: center; gap: 6px; flex-wrap: wrap; margin-top: 6px;
        }
        .ps-badge {
            font-size: 11px; font-weight: 500; padding: 2px 7px; border-radius: 4px;
            background: var(--badge-bg); color: var(--text-muted);
        }
        .ps-badge-req {
            background: rgba(99, 102, 241, 0.1); color: #6366f1; border: 1px solid rgba(99, 102, 241, 0.2);
        }
        .ps-badge-warn {
            background: rgba(239, 68, 68, 0.12); color: #ef4444; border: 1px solid rgba(239, 68, 68, 0.25);
        }
        .ps-hero-stats {
            display: flex; align-items: center; gap: 12px; flex-shrink: 0;
        }
        .ps-stat-box {
            display: flex; flex-direction: column; align-items: center; justify-content: center;
            background: var(--bg-body); border: 1px solid var(--border); border-radius: 6px;
            padding: 6px 12px; min-width: 58px;
        }
        .ps-stat-box .stat-num { font-size: 17px; font-weight: 700; color: var(--text-main); line-height: 1.1; }
        .ps-stat-box .stat-label { font-size: 10px; color: var(--text-muted); margin-top: 2px; }

        .ps-synopsis-callout {
            border-left: 3px solid #0284c7; background: rgba(2, 132, 199, 0.05);
            padding: 10px 14px; border-radius: 0 6px 6px 0;
        }
        .callout-tag { font-size: 10.5px; font-weight: 700; color: #0284c7; letter-spacing: 0.5px; margin-bottom: 3px; }
        .callout-text { font-size: 13.5px; color: var(--text-main); line-height: 1.6; }
        .ps-description-body {
            font-size: 13px; line-height: 1.65; color: var(--text-main);
            border-top: 1px dashed var(--border); padding-top: 10px;
        }
        .desc-tag { font-size: 10.5px; font-weight: 700; color: var(--text-muted); letter-spacing: 0.5px; margin-bottom: 4px; }
        .desc-text { color: var(--text-main); }

        /* Section Cards */
        .ps-section {
            background: var(--bg-card); border: 1px solid var(--border);
            border-radius: 8px; padding: 16px 18px; box-shadow: 0 1px 6px rgba(0,0,0,0.02);
            display: flex; flex-direction: column; gap: 12px;
        }
        .ps-section-header {
            display: flex; align-items: center; justify-content: space-between; gap: 10px;
            border-bottom: 1px solid var(--border); padding-bottom: 10px;
        }
        .ps-section-title {
            margin: 0; font-size: 14px; font-weight: 600; color: var(--text-main);
            display: flex; align-items: center; gap: 6px;
        }
        .ps-badge-counter {
            font-size: 11px; color: var(--text-muted); background: var(--badge-bg);
            padding: 2px 7px; border-radius: 4px; font-weight: 500;
        }

        /* Syntax Box */
        .ps-syntax-box {
            background: #0f172a; color: #f8fafc; border-radius: 6px; padding: 12px 14px;
            overflow-x: auto; font-family: Consolas, 'Cascadia Code', monospace; font-size: 12.5px;
            line-height: 1.5; box-shadow: inset 0 2px 4px rgba(0,0,0,0.3);
        }
        body.dark-mode .ps-syntax-box { background: #090d16; border: 1px solid #1e293b; }
        .ps-syntax-box code { color: #38bdf8; font-family: inherit; }

        /* Parameters Table */
        .ps-param-table-wrapper { width: 100%; overflow-x: auto; }
        .ps-param-table {
            width: 100%; border-collapse: collapse; font-size: 12px; text-align: left;
        }
        .ps-param-table th {
            background: var(--bg-body); color: var(--text-muted); font-weight: 600;
            padding: 8px 10px; border-bottom: 1px solid var(--border); font-size: 11.5px;
        }
        .ps-param-table td {
            padding: 9px 10px; border-bottom: 1px solid var(--border); vertical-align: top;
            color: var(--text-main); line-height: 1.45;
        }
        .ps-param-table tr:hover td { background: rgba(0,0,0,0.015); }
        body.dark-mode .ps-param-table tr:hover td { background: rgba(255,255,255,0.02); }
        .param-name { font-weight: 700; color: #0284c7; font-family: Consolas, monospace; }
        body.dark-mode .param-name { color: #38bdf8; }
        .param-type { color: var(--text-muted); font-family: Consolas, monospace; font-size: 11px; }
        .ps-param-req {
            font-size: 10.5px; font-weight: 600; padding: 1px 5px; border-radius: 3px;
            background: rgba(239, 68, 68, 0.12); color: #ef4444; border: 1px solid rgba(239, 68, 68, 0.2);
        }
        .ps-param-opt {
            font-size: 10.5px; padding: 1px 5px; border-radius: 3px;
            background: var(--badge-bg); color: var(--text-muted);
        }
        .ps-valid-set { display: flex; gap: 4px; flex-wrap: wrap; margin-bottom: 4px; }
        .val-chip {
            font-size: 10px; font-family: Consolas, monospace; background: var(--bg-body);
            border: 1px solid var(--border); padding: 1px 4px; border-radius: 3px; color: var(--text-main);
        }
        .param-desc { font-size: 11.5px; color: var(--text-muted); margin-top: 2px; }

        /* Examples */
        .ps-examples-list { display: flex; flex-direction: column; gap: 12px; }
        .ps-example-card {
            border: 1px solid var(--border); border-radius: 6px; overflow: hidden; background: var(--bg-body);
        }
        .example-header {
            display: flex; align-items: center; justify-content: space-between;
            padding: 6px 12px; background: rgba(0,0,0,0.03); border-bottom: 1px solid var(--border);
        }
        body.dark-mode .example-header { background: rgba(255,255,255,0.03); }
        .example-num { font-size: 11px; font-weight: 600; color: var(--text-muted); }
        .example-pre {
            margin: 0; padding: 10px 14px; font-family: Consolas, monospace; font-size: 12px;
            background: transparent; color: var(--text-main); overflow-x: auto;
        }

        /* Functions Grid */
        .ps-functions-grid {
            display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 12px;
        }
        .ps-func-card {
            border: 1px solid var(--border); border-radius: 6px; padding: 12px;
            background: var(--bg-body); display: flex; flex-direction: column; gap: 6px;
            transition: all 0.15s ease;
        }
        .ps-func-card:hover {
            border-color: var(--btn-primary-bg); transform: translateY(-1px);
            box-shadow: 0 3px 10px rgba(0,0,0,0.04);
        }
        .func-card-header {
            display: flex; align-items: center; justify-content: space-between; gap: 8px;
        }
        .func-title-group { display: flex; align-items: center; gap: 6px; flex-wrap: wrap; }
        .func-badge {
            font-size: 9.5px; font-weight: 700; text-transform: uppercase;
            padding: 1px 4px; border-radius: 3px; background: rgba(16, 185, 129, 0.12);
            color: #10b981; border: 1px solid rgba(16, 185, 129, 0.25);
        }
        .func-name { font-weight: 600; font-size: 13px; color: var(--text-main); font-family: Consolas, monospace; }
        .func-line { font-size: 10.5px; color: var(--text-muted); }
        .func-synopsis { font-size: 12px; color: var(--text-muted); line-height: 1.4; }
        .func-syntax-chip {
            background: var(--bg-card); border: 1px solid var(--border); border-radius: 4px;
            padding: 4px 8px; font-family: Consolas, monospace; font-size: 11px;
            color: #0284c7; overflow-x: auto; white-space: nowrap; margin-top: 4px;
        }
        body.dark-mode .func-syntax-chip { color: #38bdf8; }

        /* Security Notices List */
        .ps-security-list { display: flex; flex-direction: column; gap: 8px; }
        .ps-security-item {
            border: 1px solid rgba(239, 68, 68, 0.25); background: rgba(239, 68, 68, 0.05);
            border-radius: 6px; padding: 10px 12px; display: flex; flex-direction: column; gap: 4px;
        }
        .ps-security-item.warning {
            border-color: rgba(245, 158, 11, 0.25); background: rgba(245, 158, 11, 0.05);
        }
        .ps-security-item.info {
            border-color: rgba(59, 130, 246, 0.25); background: rgba(59, 130, 246, 0.05);
        }
        .sec-header { display: flex; align-items: center; justify-content: space-between; gap: 8px; }
        .sec-badge { font-size: 11px; font-weight: 700; color: #ef4444; }
        .sec-badge.warning { color: #d97706; }
        .sec-badge.info { color: #2563eb; }
        .sec-line { font-size: 11px; color: var(--text-muted); font-family: Consolas, monospace; }
        .sec-line:hover { color: var(--btn-primary-bg); text-decoration: underline; }
        .sec-detail { font-size: 12px; color: var(--text-main); line-height: 1.4; }

        /* Template Hint Card */
        .ps-template-hint-card {
            border: 1px dashed var(--border); border-radius: 8px; padding: 14px 18px;
            background: var(--bg-card); display: flex; align-items: center; justify-content: space-between;
            gap: 16px; flex-wrap: wrap; margin-top: 10px;
        }
        .hint-icon { width: 32px; height: 32px; color: #0284c7; flex-shrink: 0; }
        .hint-icon svg { width: 100%; height: 100%; }
        .hint-content { flex: 1 1 280px; }
        .hint-title { font-size: 13px; font-weight: 600; color: var(--text-main); margin-bottom: 2px; }
        .hint-desc { font-size: 12px; color: var(--text-muted); line-height: 1.45; }

        /* Outline Drawer */
        .ps-outline-drawer {
            position: fixed; top: 0; right: -360px; width: 340px; height: 100vh;
            background: var(--bg-card); border-left: 1px solid var(--border);
            box-shadow: -4px 0 20px rgba(0,0,0,0.1); z-index: 1000;
            display: flex; flex-direction: column; transition: right 0.25s cubic-bezier(0.16, 1, 0.3, 1);
        }
        .ps-outline-drawer.active { right: 0; }
        .ps-outline-backdrop {
            position: fixed; top: 0; left: 0; width: 100vw; height: 100vh;
            background: rgba(0,0,0,0.3); z-index: 999; display: none;
            backdrop-filter: blur(1px);
        }
        .ps-outline-backdrop.active { display: block; }
        .drawer-head {
            display: flex; align-items: center; justify-content: space-between;
            padding: 14px 16px; border-bottom: 1px solid var(--border);
        }
        .drawer-title {
            font-size: 13.5px; font-weight: 600; color: var(--text-main);
            display: flex; align-items: center; gap: 6px;
        }
        .btn-close-drawer {
            border: none; background: transparent; color: var(--text-muted);
            cursor: pointer; padding: 4px; border-radius: 4px; display: inline-flex;
        }
        .btn-close-drawer:hover { background: var(--border); color: var(--text-main); }
        .drawer-search { padding: 10px 14px; border-bottom: 1px solid var(--border); }
        .drawer-search input {
            width: 100%; box-sizing: border-box; padding: 6px 10px; font-size: 12px;
            border: 1px solid var(--border); border-radius: 5px; background: var(--bg-body);
            color: var(--text-main); outline: none;
        }
        .drawer-search input:focus { border-color: var(--btn-primary-bg); }
        .drawer-body { flex: 1; overflow-y: auto; padding: 12px 14px; display: flex; flex-direction: column; gap: 14px; }
        .outline-group { display: flex; flex-direction: column; gap: 5px; }
        .group-title {
            font-size: 11px; font-weight: 700; color: var(--text-muted); text-transform: uppercase;
            letter-spacing: 0.5px; margin-bottom: 4px; display: flex; align-items: center; gap: 5px;
        }
        .outline-item {
            display: flex; align-items: center; justify-content: space-between;
            padding: 6px 10px; border-radius: 5px; cursor: pointer; font-size: 12px;
            background: var(--bg-body); border: 1px solid transparent; transition: all 0.12s ease;
        }
        .outline-item:hover {
            border-color: var(--btn-primary-bg); background: rgba(37, 99, 235, 0.05);
        }
        .item-kind {
            font-size: 9.5px; font-weight: 700; text-transform: uppercase; padding: 1px 4px;
            border-radius: 3px; background: var(--badge-bg); color: var(--text-muted); margin-right: 6px;
        }
        .param-item .item-kind { background: rgba(2, 132, 199, 0.12); color: #0284c7; }
        .func-item .item-kind { background: rgba(16, 185, 129, 0.12); color: #10b981; }
        .sec-item .item-kind { background: rgba(239, 68, 68, 0.12); color: #ef4444; }
        .item-name { flex: 1; font-family: Consolas, monospace; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .item-type, .item-line { font-size: 11px; color: var(--text-muted); margin-left: 6px; }

        .btn-xs { padding: 2px 6px !important; font-size: 10.5px !important; }

        /* Code Overview & Lightweight Metadata Card Styles */
        .code-overview-wrapper {
            display: flex; align-items: center; justify-content: center;
            padding: 30px 16px; width: 100%; box-sizing: border-box;
        }
        .code-overview-card {
            background: var(--bg-card); border: 1px solid var(--border);
            border-radius: 14px; padding: 24px 28px; width: 100%; max-width: 520px;
            box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.06); text-align: center;
            display: flex; flex-direction: column; align-items: center; gap: 18px;
        }
        .code-lang-badge-large {
            display: inline-flex; align-items: center; gap: 8px;
            padding: 6px 16px; border-radius: 20px;
            background: rgba(37, 99, 235, 0.08); border: 1px solid rgba(37, 99, 235, 0.2);
            color: var(--btn-primary-bg); font-weight: 700; font-size: 13.5px;
        }
        .code-stats-grid {
            display: grid; grid-template-columns: repeat(2, 1fr); gap: 12px; width: 100%;
        }
        .code-stat-item {
            background: var(--bg-body); border: 1px solid var(--border);
            border-radius: 10px; padding: 12px; display: flex; flex-direction: column; gap: 4px;
        }
        .code-stat-item .stat-val {
            font-size: 16px; font-weight: 700; color: var(--text-main); font-family: Consolas, monospace;
        }
        .code-stat-item .stat-label {
            font-size: 11px; color: var(--text-muted);
        }
        .code-tip-box {
            background: rgba(14, 165, 233, 0.05); border: 1px dashed rgba(14, 165, 233, 0.25);
            border-radius: 8px; padding: 10px 14px; width: 100%; box-sizing: border-box;
        }
        .code-tip-box p {
            font-size: 11.5px; color: var(--text-muted); line-height: 1.5; margin: 0;
        }
        .code-action-row {
            display: flex; align-items: center; gap: 10px; width: 100%; justify-content: center;
        }

        /* XML Interactive Tree Styles (Modern Chrome/VSCode DevTools Level) */
        .xml-container {
            font-family: Consolas, 'Cascadia Code', 'Fira Code', Monaco, monospace;
            font-size: 13px; line-height: 1.6; padding: 12px 18px 60px 18px;
            color: var(--text-main); user-select: text;
        }
        .xml-prolog {
            padding: 3px 8px; margin-bottom: 8px; color: #8b5cf6;
            font-style: italic; background: rgba(139, 92, 246, 0.08);
            border: 1px solid rgba(139, 92, 246, 0.2);
            border-radius: 4px; display: inline-block; font-size: 12px;
        }
        body.dark-mode .xml-prolog { color: #a78bfa; background: rgba(139, 92, 246, 0.15); border-color: rgba(139, 92, 246, 0.3); }

        .xml-node {
            margin: 1px 0; position: relative;
        }
        .xml-summary, .xml-leaf-row, .xml-close-tag-row, .xml-cdata-row, .xml-comment-row {
            display: flex; align-items: center; flex-wrap: wrap;
            padding: 2px 6px; border-radius: 4px; min-height: 24px;
            position: relative; transition: background 0.15s ease;
        }
        .xml-summary { cursor: pointer; list-style: none; }
        .xml-summary::-webkit-details-marker { display: none; }
        .xml-summary:hover, .xml-leaf-row:hover, .xml-close-tag-row:hover, .xml-cdata-row:hover, .xml-comment-row:hover {
            background: rgba(0, 0, 0, 0.035);
        }
        body.dark-mode .xml-summary:hover,
        body.dark-mode .xml-leaf-row:hover,
        body.dark-mode .xml-close-tag-row:hover,
        body.dark-mode .xml-cdata-row:hover,
        body.dark-mode .xml-comment-row:hover {
            background: rgba(255, 255, 255, 0.05);
        }

        /* Guide Line */
        .xml-node-children {
            margin-left: 10px; padding-left: 14px;
            border-left: 1px dashed var(--border);
            position: relative;
        }
        .xml-node:hover > .xml-node-children {
            border-left-color: rgba(59, 130, 246, 0.4);
        }

        /* Chevron indicator */
        .xml-chevron {
            width: 14px; height: 14px; margin-right: 4px;
            color: var(--text-muted); flex-shrink: 0;
            transition: transform 0.15s ease;
        }
        details[open] > .xml-summary .xml-chevron {
            transform: rotate(90deg);
        }
        .xml-spacer { width: 18px; height: 14px; flex-shrink: 0; }

        /* Syntax Elements */
        .xml-bracket {
            color: #64748b; font-weight: 500; user-select: none;
        }
        body.dark-mode .xml-bracket { color: #94a3b8; }

        .xml-tag-name {
            color: #0284c7; font-weight: 600; cursor: pointer;
            padding: 0 2px; border-radius: 3px;
            transition: background 0.15s ease, color 0.15s ease;
        }
        .xml-tag-name:hover {
            background: rgba(2, 132, 199, 0.12);
            text-decoration: underline; text-underline-offset: 2px;
        }
        body.dark-mode .xml-tag-name { color: #38bdf8; }
        body.dark-mode .xml-tag-name:hover { background: rgba(56, 189, 248, 0.2); }

        .xml-attr-list { display: inline-flex; flex-wrap: wrap; align-items: center; }
        .xml-attr {
            margin: 0 3px; font-size: 12px; display: inline-flex; align-items: center;
            border-radius: 3px; padding: 0 2px;
        }
        .xml-attr-name {
            color: #d97706; font-weight: 500; cursor: pointer;
            padding: 0 2px; border-radius: 2px;
        }
        .xml-attr-name:hover {
            background: rgba(217, 119, 6, 0.15); text-decoration: underline;
        }
        body.dark-mode .xml-attr-name { color: #fbbf24; }
        body.dark-mode .xml-attr-name:hover { background: rgba(251, 191, 36, 0.2); }

        .xml-eq { color: var(--text-muted); margin: 0 1px; user-select: none; }
        .xml-quote { color: #16a34a; user-select: none; }
        body.dark-mode .xml-quote { color: #4ade80; }

        .xml-attr-val {
            color: #16a34a; cursor: pointer; padding: 0 2px;
            border-radius: 2px; word-break: break-all;
        }
        .xml-attr-val:hover {
            background: rgba(22, 163, 74, 0.15); text-decoration: underline;
        }
        body.dark-mode .xml-attr-val { color: #4ade80; }
        body.dark-mode .xml-attr-val:hover { background: rgba(74, 222, 128, 0.2); }

        /* Badge Attr View Mode */
        .xml-container.badge-attrs .xml-attr {
            background: rgba(0, 0, 0, 0.04); border: 1px solid var(--border);
            padding: 1px 5px; margin: 1px 3px; border-radius: 4px;
        }
        body.dark-mode .xml-container.badge-attrs .xml-attr {
            background: rgba(255, 255, 255, 0.06);
        }
        .xml-container.hide-attrs .xml-attr-list {
            display: none !important;
        }

        /* Text Content */
        .xml-text-val {
            color: var(--text-main); margin: 0 4px; cursor: pointer;
            background: rgba(0, 0, 0, 0.02); padding: 0 4px; border-radius: 3px;
            border: 1px dashed transparent;
        }
        .xml-text-val:hover {
            background: rgba(59, 130, 246, 0.08);
            border-color: rgba(59, 130, 246, 0.3);
        }
        body.dark-mode .xml-text-val {
            background: rgba(255, 255, 255, 0.04);
        }
        body.dark-mode .xml-text-val:hover {
            background: rgba(59, 130, 246, 0.15);
            border-color: rgba(59, 130, 246, 0.4);
        }

        /* CDATA & Comment */
        .xml-cdata-row { margin: 2px 0; }
        .xml-cdata-badge {
            font-size: 10px; font-weight: 700; background: rgba(6, 182, 212, 0.15);
            color: #0891b2; padding: 1px 5px; border-radius: 3px; margin-right: 6px;
        }
        body.dark-mode .xml-cdata-badge { background: rgba(6, 182, 212, 0.25); color: #22d3ee; }
        .xml-cdata-content {
            background: rgba(6, 182, 212, 0.06); border-radius: 3px;
            padding: 1px 6px; color: #0e7490; cursor: pointer;
        }
        body.dark-mode .xml-cdata-content { background: rgba(6, 182, 212, 0.12); color: #67e8f9; }

        .xml-comment {
            color: #94a3b8; font-style: italic; cursor: pointer;
            padding: 1px 4px; border-radius: 3px;
        }
        .xml-comment:hover { background: rgba(0, 0, 0, 0.04); }
        body.dark-mode .xml-comment { color: #64748b; }

        /* Collapsed Preview */
        details:not([open]) > .xml-summary .xml-collapsed-preview {
            display: inline-flex; align-items: center;
        }
        details[open] > .xml-summary .xml-collapsed-preview {
            display: none;
        }
        details[open] > .xml-summary .xml-open-bracket {
            display: inline;
        }
        details:not([open]) > .xml-summary .xml-open-bracket {
            display: none;
        }
        .xml-ellipsis {
            background: rgba(0, 0, 0, 0.06); color: var(--text-muted);
            padding: 0 4px; border-radius: 3px; margin: 0 2px; font-size: 11px;
        }
        body.dark-mode .xml-ellipsis { background: rgba(255, 255, 255, 0.1); }

        /* Badge count */
        .xml-badge {
            font-size: 11px; padding: 1px 6px; border-radius: 10px;
            background: rgba(59, 130, 246, 0.12); color: #2563eb;
            margin-left: 8px; font-weight: 500; user-select: none;
        }
        body.dark-mode .xml-badge { background: rgba(59, 130, 246, 0.25); color: #60a5fa; }

        /* Floating tool buttons on row hover */
        .xml-row-tools {
            margin-left: auto; display: none; align-items: center; gap: 4px;
        }
        .xml-summary:hover > .xml-row-tools,
        .xml-leaf-row:hover > .xml-row-tools,
        .xml-close-tag-row:hover > .xml-row-tools {
            display: inline-flex;
        }
        .xml-tool-btn {
            background: var(--bg-hover); border: 1px solid var(--border);
            color: var(--text-muted); padding: 1px 6px; font-size: 11px;
            border-radius: 4px; cursor: pointer; transition: all 0.15s ease;
        }
        .xml-tool-btn:hover {
            color: var(--text-main); border-color: var(--accent);
            background: rgba(59, 130, 246, 0.1);
        }

        /* In-place input */
        .xml-inline-input {
            font-family: inherit; font-size: inherit; color: var(--text-main);
            background: var(--bg-card); border: 1px solid #3b82f6;
            border-radius: 3px; padding: 1px 4px; outline: none;
            box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.25); min-width: 60px;
        }

        /* Edit flash animation */
        @keyframes xmlEditFlash {
            0% { background: rgba(34, 197, 94, 0.35); }
            100% { background: transparent; }
        }
        .xml-just-edited {
            animation: xmlEditFlash 1.5s ease-out;
            border-radius: 3px;
        }

        /* Search highlight */
        .xml-match-hl {
            background: #fde047; color: #854d0e; padding: 0 2px;
            border-radius: 2px; font-weight: 600;
        }
        body.dark-mode .xml-match-hl {
            background: #ca8a04; color: #fef08a;
        }

        /* Modern Multi-Language Code Block Styles */
        .code-viewer-container { width: 100%; margin: 0; padding: 0; }
        .code-viewer-container .code-block-wrapper { margin: 0 !important; }
        .code-block-wrapper {
            position: relative; margin: 18px 0; border: 1px solid var(--border);
            border-radius: 8px; overflow: hidden; background: var(--bg-code);
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
            transition: border-color 0.2s ease, box-shadow 0.2s ease;
        }
        .code-block-wrapper:hover {
            border-color: var(--border-hover);
            box-shadow: 0 4px 14px rgba(0, 0, 0, 0.08);
        }

        /* Top Language Accent Bar */
        .code-accent-bar {
            height: 3px; width: 100%;
            background: var(--block-accent, #64748b);
            transition: background 0.2s ease;
        }

        /* Mac-Style Header */
        .code-block-header {
            display: flex; align-items: center; justify-content: space-between;
            padding: 6px 12px; background: rgba(0, 0, 0, 0.025); border-bottom: 1px solid var(--border);
            font-size: 12px; user-select: none;
        }
        body.dark-mode .code-block-header { background: rgba(255, 255, 255, 0.035); }

        .code-header-left {
            display: flex; align-items: center; gap: 10px;
        }

        /* Mac Traffic Light Control Dots */
        .mac-dots {
            display: flex; align-items: center; gap: 6px; padding-right: 2px;
        }
        .mac-dot {
            width: 10px; height: 10px; border-radius: 50%; display: inline-block;
            transition: transform 0.15s ease;
        }
        .mac-dot:hover { transform: scale(1.15); }
        .dot-red    { background: #ff5f56; box-shadow: 0 0 2px rgba(255, 95, 86, 0.5); }
        .dot-yellow { background: #ffbd2e; box-shadow: 0 0 2px rgba(255, 189, 46, 0.5); }
        .dot-green  { background: #27c93f; box-shadow: 0 0 2px rgba(39, 201, 63, 0.5); }

        /* Distinct Language Pill Badge */
        .lang-badge {
            display: inline-flex; align-items: center; gap: 5px;
            padding: 2px 8px; border-radius: 12px; font-size: 11px;
            font-weight: 700; letter-spacing: 0.3px; border: 1px solid transparent;
            transition: all 0.2s ease; box-shadow: 0 1px 3px rgba(0, 0, 0, 0.03);
        }
        .lang-icon { font-size: 12px; line-height: 1; }

        /* Right Action Controls */
        .code-header-right {
            display: flex; align-items: center; gap: 8px;
        }
        .code-meta-lines {
            font-size: 11px; color: var(--text-muted); font-family: Consolas, monospace;
        }
        .code-action-btn {
            background: transparent; border: 1px solid transparent; border-radius: 4px;
            color: var(--text-muted); cursor: pointer; padding: 2px 6px; font-size: 11px;
            display: inline-flex; align-items: center; gap: 4px; transition: all 0.15s ease;
        }
        .code-action-btn:hover { background: var(--btn-hover); color: var(--text-main); border-color: var(--border); }

        .code-copy-btn {
            background: transparent; border: 1px solid transparent; border-radius: 4px;
            color: var(--text-muted); cursor: pointer; padding: 2px 6px; font-size: 11px;
            display: inline-flex; align-items: center; gap: 4px; transition: all 0.15s ease;
        }
        .code-copy-btn:hover { background: var(--btn-hover); color: var(--text-main); border-color: var(--border); }
        .code-copy-btn.copied { color: #10b981; border-color: rgba(16, 185, 129, 0.4); background: rgba(16, 185, 129, 0.08); }

        /* Code Body & Table-Based Zero-Misalignment Line Numbers */
        .code-body-container {
            overflow-x: auto; position: relative;
            background: var(--bg-code); transition: max-height 0.25s ease;
        }
        .code-body-container.collapsed {
            max-height: 0 !important; overflow: hidden;
        }

        .code-table {
            border-collapse: collapse; width: 100%; margin: 0 !important; border: none !important;
            font-family: Consolas, 'Cascadia Code', 'Fira Code', Menlo, monospace;
            font-size: 13px; line-height: 1.6; background: transparent;
        }
        .code-table tbody { border: none !important; }

        .code-tr {
            background: transparent !important; border: none !important;
            transition: background 0.1s ease;
        }
        .code-tr:hover {
            background: rgba(0, 0, 0, 0.035) !important;
        }
        body.dark-mode .code-tr:hover {
            background: rgba(255, 255, 255, 0.04) !important;
        }

        /* Gutter Cell (Line Number) */
        .code-gutter {
            width: 1%; min-width: 44px; padding: 0 10px 0 12px !important;
            text-align: right; user-select: none; vertical-align: top;
            color: var(--text-muted); opacity: 0.55; border: none !important;
            border-right: 1px solid var(--border) !important;
            background: rgba(0, 0, 0, 0.012); white-space: nowrap;
            font-size: 12px; line-height: inherit;
        }
        body.dark-mode .code-gutter { background: rgba(255, 255, 255, 0.012); }

        /* Code Line Cell */
        .code-line {
            padding: 0 14px !important; border: none !important;
            white-space: pre; vertical-align: top; line-height: inherit;
            font-family: inherit; font-size: inherit; color: var(--text-main);
            background: transparent !important;
        }
        .code-line code {
            padding: 0 !important; background: transparent !important;
            font-family: inherit; font-size: inherit; line-height: inherit;
            white-space: pre; tab-size: 4; border: none !important;
        }

        /* Symmetric vertical paddings for first and last line */
        .code-tr:first-child .code-gutter,
        .code-tr:first-child .code-line {
            padding-top: 10px !important;
        }
        .code-tr:last-child .code-gutter,
        .code-tr:last-child .code-line {
            padding-bottom: 10px !important;
        }

        /* Terminal & Shell Styling (Respects Theme Palette) */
        .code-block-wrapper.is-terminal {
            /* Keep unified theme card styling */
        }
        body.dark-mode .code-block-wrapper.is-terminal {
            background: #181c24; border-color: #2b3544;
        }

        /* Specialized Git Diff Tr Highlighting */
        .code-tr.diff-add-tr {
            background: rgba(34, 197, 94, 0.12) !important;
        }
        .code-tr.diff-add-tr .code-line {
            color: #15803d; font-weight: 500;
        }
        .code-tr.diff-del-tr {
            background: rgba(239, 68, 68, 0.12) !important;
        }
        .code-tr.diff-del-tr .code-line {
            color: #b91c1c;
        }
        .code-tr.diff-meta-tr .code-line {
            color: #7c3aed; font-weight: 600;
        }
        body.dark-mode .code-tr.diff-add-tr {
            background: rgba(34, 197, 94, 0.2) !important;
        }
        body.dark-mode .code-tr.diff-add-tr .code-line {
            color: #4ade80;
        }
        body.dark-mode .code-tr.diff-del-tr {
            background: rgba(239, 68, 68, 0.2) !important;
        }
        body.dark-mode .code-tr.diff-del-tr .code-line {
            color: #f87171;
        }
        body.dark-mode .code-tr.diff-meta-tr .code-line {
            color: #c084fc;
        }

        /* Built-in Syntax Highlighting Colors (GitHub / One Dark Palette) */
        .hljs-keyword, .hljs-selector-tag, .hljs-subst { color: #cf222e; font-weight: 600; }
        .hljs-string, .hljs-title, .hljs-section, .hljs-attribute, .hljs-literal, .hljs-template-tag, .hljs-template-variable, .hljs-type { color: #0a3069; }
        .hljs-comment, .hljs-quote, .hljs-deletion, .hljs-meta { color: #6e7781; font-style: italic; }
        .hljs-number, .hljs-regexp, .hljs-link { color: #0550ae; }
        .hljs-variable, .hljs-bullet { color: #953800; }
        .hljs-built_in, .hljs-symbol { color: #8250df; }

        body.dark-mode .hljs-keyword, body.dark-mode .hljs-selector-tag, body.dark-mode .hljs-subst { color: #ff7b72; font-weight: 600; }
        body.dark-mode .hljs-string, body.dark-mode .hljs-title, body.dark-mode .hljs-section, body.dark-mode .hljs-attribute, body.dark-mode .hljs-literal, body.dark-mode .hljs-template-tag, body.dark-mode .hljs-template-variable, body.dark-mode .hljs-type { color: #a5d6ff; }
        body.dark-mode .hljs-comment, body.dark-mode .hljs-quote, body.dark-mode .hljs-deletion, body.dark-mode .hljs-meta { color: #8b949e; font-style: italic; }
        body.dark-mode .hljs-number, body.dark-mode .hljs-regexp, body.dark-mode .hljs-link { color: #79c0ff; }
        body.dark-mode .hljs-variable, body.dark-mode .hljs-bullet { color: #ffa657; }
        body.dark-mode .hljs-built_in, body.dark-mode .hljs-symbol { color: #d2a8ff; }

        /* Inline Code with Click-To-Copy */
        .inline-code-pill {
            background: var(--bg-code); border: 1px solid var(--border);
            padding: 2px 6px; border-radius: 4px; font-family: Consolas, monospace;
            font-size: 0.9em; color: var(--text-main); cursor: pointer;
            display: inline-flex; align-items: center; gap: 4px; transition: all 0.15s ease;
        }
        .inline-code-pill:hover { border-color: var(--btn-primary-bg); background: var(--bg-card); }

        /* Tables (Markdown & CSV) */
        .csv-container { overflow: auto; max-width: 100%; }
        table { border-collapse: collapse; width: 100%; font-size: 13px; margin: 0; }
        th, td { border: 1px solid var(--border); padding: 7px 12px; text-align: left; }
        th {
            background: var(--bg-table-th); cursor: pointer; user-select: none;
            position: sticky; top: 0; font-weight: 600; z-index: 10;
            box-shadow: 0 1px 0 var(--border), 0 3px 6px -2px rgba(0, 0, 0, 0.08);
            transition: background 0.15s ease;
        }
        th:hover { background: var(--border); }
        tr:nth-child(even) { background: var(--bg-table-stripe); }
        tr:hover { background: rgba(37, 99, 235, 0.05); }
        td[contenteditable='true']:focus { outline: 2px solid var(--btn-primary-bg); background: var(--bg-body); }

        /* High-Definition Hierarchical JSON Tree View (优雅内联类型展示与树状引导线) */
        .json-container {
            font-family: Consolas, 'Fira Code', 'Cascadia Code', Menlo, monospace;
            font-size: 13px;
            line-height: 1.6;
            padding: 4px 0;
            user-select: text;
        }
        .json-row, .json-container summary {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 2px 6px;
            border-radius: 4px;
            margin: 1px 0;
            min-height: 24px;
            transition: background 0.12s ease;
        }
        .json-row:hover, .json-container summary:hover {
            background: rgba(37, 99, 235, 0.05);
        }
        body.dark-mode .json-row:hover, body.dark-mode .json-container summary:hover {
            background: rgba(255, 255, 255, 0.05);
        }
        .json-main {
            display: inline-flex;
            align-items: center;
            flex-wrap: wrap;
            gap: 2px;
            word-break: break-all;
            flex: 1;
        }
        .json-key { color: var(--json-key); font-weight: 600; }
        .json-colon { color: var(--json-colon); margin-right: 4px; user-select: none; }
        .json-idx { color: #8b5cf6; font-weight: 600; }
        .json-str { color: var(--json-str); }
        .json-num { color: var(--json-num); font-weight: 600; }
        .json-bool { color: var(--json-bool); font-weight: bold; }
        .json-null { color: var(--json-null); font-style: italic; }
        .json-bracket { color: var(--json-bracket); font-weight: 600; }
        .json-summary-tag {
            font-size: 11px;
            color: var(--text-muted);
            background: var(--bg-code);
            padding: 0 6px;
            border-radius: 10px;
            margin-left: 6px;
            user-select: none;
            display: inline-flex;
            align-items: center;
            height: 17px;
        }

        /* Inline JSON Type Badges (内联展示于值后方，跟随节点紧凑呈现) */
        .type-tag {
            font-size: 10px;
            font-weight: 600;
            line-height: 14px;
            height: 16px;
            padding: 0 5px;
            border-radius: 3px;
            user-select: none;
            letter-spacing: 0.2px;
            white-space: nowrap;
            flex-shrink: 0;
            display: inline-flex;
            align-items: center;
            border: 1px solid transparent;
            margin-left: 6px;
            vertical-align: middle;
        }
        .type-str { background: var(--type-str-bg); color: var(--type-str-text); border-color: var(--type-str-border); }
        .type-num { background: var(--type-num-bg); color: var(--type-num-text); border-color: var(--type-num-border); }
        .type-bool { background: var(--type-bool-bg); color: var(--type-bool-text); border-color: var(--type-bool-border); }
        .type-null { background: var(--type-null-bg); color: var(--type-null-text); border-color: var(--type-null-border); }
        .type-obj { background: var(--type-obj-bg); color: var(--type-obj-text); border-color: var(--type-obj-border); }
        .type-arr { background: var(--type-arr-bg); color: var(--type-arr-text); border-color: var(--type-arr-border); }
        .hide-types .type-tag { display: none !important; }

        /* Tree Guides & Details Structure */
        .json-container details {
            position: relative;
        }
        .json-container details > summary::-webkit-details-marker,
        .json-container details > summary::marker {
            display: none !important;
            content: '' !important;
        }
        .json-container details > summary {
            list-style: none;
            cursor: pointer;
            outline: none;
        }
        .tree-arrow {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 14px;
            height: 14px;
            color: var(--text-muted);
            transition: transform 0.15s ease, color 0.15s ease;
            flex-shrink: 0;
            margin-right: 4px;
        }
        .json-container details[open] > summary .tree-arrow {
            transform: rotate(90deg);
        }
        .json-container summary:hover .tree-arrow {
            color: var(--btn-primary-bg);
        }
        .tree-spacer {
            display: inline-block;
            width: 14px;
            height: 14px;
            flex-shrink: 0;
            margin-right: 4px;
        }
        .json-node-children {
            position: relative;
            margin-left: 6px;
            padding-left: 14px;
            border-left: 1px dashed var(--json-tree-line);
            transition: border-color 0.2s ease;
        }
        .json-node-children:hover {
            border-left-color: var(--json-tree-line-hover);
        }

        /* Hover Action Buttons */
        .json-actions {
            display: inline-flex;
            align-items: center;
            opacity: 0;
            transition: opacity 0.15s ease;
            margin-left: 12px;
            flex-shrink: 0;
        }
        .json-row:hover .json-actions,
        .json-container summary:hover .json-actions {
            opacity: 1;
        }
        .json-copy-btn {
            background: var(--btn-bg);
            border: 1px solid var(--btn-border);
            color: var(--text-muted);
            border-radius: 4px;
            padding: 1px 6px;
            font-size: 11px;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 3px;
            height: 20px;
            transition: all 0.15s ease;
        }
        .json-copy-btn:hover {
            background: var(--btn-hover);
            color: var(--btn-primary-bg);
            border-color: var(--btn-primary-bg);
        }

        .json-err {
            background: #fef2f2; border: 1px solid #f87171; color: #991b1b;
            padding: 14px; border-radius: 6px; font-family: Consolas, monospace;
        }
        body.dark-mode .json-err { background: #450a0a; border-color: #991b1b; color: #fca5a5; }

        /* Notice Card */
        .notice-card {
            max-width: 440px; margin: 60px auto; text-align: center; padding: 32px 24px;
            border: 1px solid var(--border); border-radius: 10px; background: var(--bg-card);
        }
        .notice-icon { font-size: 42px; margin-bottom: 10px; }
        .notice-title { margin: 0 0 8px 0; font-size: 18px; color: var(--text-main); }
        .notice-desc { color: var(--text-muted); font-size: 13px; margin: 0 0 12px 0; }
        .notice-hint { font-size: 12px; color: var(--text-muted); opacity: 0.8; margin: 0; }

        /* Toast Notification */
        .toast-bubble {
            position: fixed; bottom: 24px; left: 50%; transform: translateX(-50%) translateY(20px);
            background: var(--toast-bg); color: var(--toast-text); padding: 7px 16px;
            border-radius: 20px; font-size: 12px; font-weight: 500; opacity: 0;
            pointer-events: none; transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
            z-index: 9999; box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        }
        .toast-bubble.show { opacity: 1; transform: translateX(-50%) translateY(0); }

        /* Universal In-Place Viewport Editor & Mode Switcher */
        .preview-actions { display: flex; align-items: center; gap: 6px; }
        .edit-actions { display: none; align-items: center; gap: 6px; }
        body.is-editing-mode .preview-actions { display: none !important; }
        body.is-editing-mode .edit-actions { display: flex !important; }

        .omni-viewport-editor {
            position: fixed; top: 38px; left: 0; right: 0; bottom: 0;
            background: var(--bg-body); z-index: 100;
            display: none; flex-direction: column; overflow: hidden;
        }
        .omni-viewport-editor.active {
            display: flex;
        }
        .editor-subbar {
            padding: 6px 12px; background: var(--bg-card); border-bottom: 1px solid var(--border);
            display: flex; justify-content: space-between; align-items: center; gap: 8px; flex-shrink: 0;
        }
        .editor-subbar-tools { display: flex; align-items: center; gap: 6px; flex-wrap: wrap; }
        .editor-tool-badge {
            font-size: 11px; font-weight: 600; color: var(--text-muted); background: var(--bg-code);
            padding: 2px 7px; border-radius: 4px; margin-right: 4px;
        }
        .editor-subbar-info { font-size: 11px; color: var(--text-muted); white-space: nowrap; }
        .editor-main-area {
            flex: 1; display: flex; overflow: hidden; background: var(--bg-body); position: relative;
        }
        .editor-gutter {
            width: 44px; padding: 12px 6px 12px 0; background: var(--bg-card);
            border-right: 1px solid var(--border); color: var(--text-muted);
            font-family: 'Consolas', 'Fira Code', 'Cascadia Code', monospace; font-size: 12.5px;
            line-height: 1.55; text-align: right; user-select: none; overflow: hidden; flex-shrink: 0;
            white-space: pre;
        }
        .omni-editor-textarea {
            flex: 1; height: 100%; box-sizing: border-box; border: none; outline: none;
            padding: 12px 14px; resize: none; background: var(--bg-body); color: var(--text-main);
            font-family: 'Consolas', 'Fira Code', 'Cascadia Code', monospace; font-size: 12.5px;
            line-height: 1.55; tab-size: 2; white-space: pre; overflow: auto;
        }
        .omni-editor-textarea:focus { outline: none; }
        .editor-status-bar {
            padding: 4px 12px; background: var(--bg-card); border-top: 1px solid var(--border);
            display: flex; justify-content: space-between; align-items: center; font-size: 11px;
            color: var(--text-muted); flex-shrink: 0;
        }
        .editor-validation-tag {
            display: inline-flex; align-items: center; gap: 4px; padding: 2px 6px; border-radius: 3px; font-size: 11px; font-weight: 500;
        }
        .editor-validation-tag.valid {
            color: #10b981; background: rgba(16, 185, 129, 0.12);
        }
        .editor-validation-tag.invalid {
            color: #ef4444; background: rgba(239, 68, 68, 0.12);
        }

        /* CSV In-Place Grid Editing Styles */
        .csv-container:not(.editing-mode) tbody td:not(.csv-num-col) {
            cursor: cell;
            transition: background 0.12s ease, outline-color 0.12s ease;
        }
        .csv-container:not(.editing-mode) tbody td:not(.csv-num-col):hover {
            background: rgba(37, 99, 235, 0.08);
            outline: 1px dashed rgba(37, 99, 235, 0.45);
            outline-offset: -1px;
        }
        td.csv-cell-in-editing {
            outline: none !important;
            background: var(--bg-body) !important;
            box-shadow: inset 0 0 0 2px var(--btn-primary-bg), 0 2px 10px rgba(37, 99, 235, 0.25) !important;
            z-index: 5;
            position: relative;
            cursor: text !important;
        }
        .csv-container.editing-mode td[contenteditable='true'] {
            outline: none; cursor: text;
        }
        .csv-container.editing-mode td[contenteditable='true']:hover {
            background: var(--btn-hover);
        }
        .csv-container.editing-mode td[contenteditable='true']:focus {
            background: var(--bg-body) !important;
            box-shadow: inset 0 0 0 2px var(--btn-primary-bg) !important;
        }
        .csv-container.editing-mode .csv-row-del-btn {
            display: inline-flex !important;
        }

        /* JSON & YAML In-Place Inline Input */
        .json-inline-input {
            background: var(--bg-body); color: var(--text-main);
            border: 1px solid var(--btn-primary-bg); border-radius: 3px;
            padding: 1px 5px; font-size: 12px; font-family: inherit;
            outline: none; box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.2);
            margin: -2px 0;
        }

        /* Markdown Block-Level In-Place Editing */
        .md-block-editing {
            position: relative;
            background: var(--bg-card) !important;
            border-radius: 6px;
            padding: 8px 10px !important;
            box-shadow: 0 0 0 2px var(--btn-primary-bg), 0 4px 12px rgba(0,0,0,0.1);
        }
        .md-block-editor-wrap {
            display: flex; flex-direction: column; gap: 6px; width: 100%; box-sizing: border-box;
        }
        .md-block-textarea {
            width: 100%; min-height: 48px; border: none; outline: none; resize: vertical;
            background: transparent; color: var(--text-main); font-family: 'Consolas', 'Fira Code', monospace;
            font-size: 13px; line-height: 1.5; box-sizing: border-box;
        }
        .md-block-editor-bar {
            display: flex; justify-content: space-between; align-items: center; gap: 8px;
            padding-top: 4px; border-top: 1px solid var(--border); font-size: 11px; color: var(--text-muted);
        }
        .md-block-tip { font-size: 11px; opacity: 0.8; }
        .md-block-actions { display: flex; align-items: center; gap: 4px; }
        .btn-mini { padding: 2px 8px !important; font-size: 11px !important; height: 22px !important; }

        /* Split-Drawer In-Place Live Viewport Editor */
        .omni-viewport-editor.split-drawer {
            top: 48% !important;
            border-top: 2px solid var(--border);
            box-shadow: 0 -8px 24px rgba(0,0,0,0.15);
        }

        /* SVG In-Place Canvas Editing & Inspector */
        .svg-canvas.svg-editing-active {
            outline: 2px dashed rgba(37, 99, 235, 0.5);
            outline-offset: 4px;
        }
        .svg-inline-text-editor {
            position: fixed; background: var(--bg-body); color: var(--text-main);
            border: 2px solid var(--btn-primary-bg); border-radius: 4px;
            padding: 2px 8px; outline: none; box-shadow: 0 4px 16px rgba(0,0,0,0.35);
            z-index: 1000; box-sizing: border-box; line-height: normal; height: 28px;
        }
        .svg-shape-inspector {
            position: fixed; background: var(--bg-card); color: var(--text-main);
            border: 1px solid var(--border); border-radius: 6px; padding: 6px 10px;
            display: flex; align-items: center; gap: 8px; z-index: 1000;
            box-shadow: 0 6px 18px rgba(0,0,0,0.2); font-size: 12px;
        }
        .svg-shape-inspector label { display: flex; align-items: center; gap: 4px; font-size: 11px; }
        .svg-shape-inspector input[type='color'] {
            width: 24px; height: 22px; border: 1px solid var(--border); border-radius: 3px; cursor: pointer; padding: 0; background: none;
        }
        .svg-shape-inspector input[type='number'] {
            width: 44px; height: 22px; border: 1px solid var(--border); border-radius: 3px; padding: 0 4px; font-size: 11px;
            background: var(--bg-body); color: var(--text-main);
        }

        /* Code In-Place Active Editing */
        .code-viewer-container code.code-in-place-active {
            outline: 2px solid var(--btn-primary-bg) !important;
            outline-offset: 2px;
            cursor: text !important;
            user-select: text !important;
            min-height: 100px;
            display: block;
        }

        /* Mermaid Node In-Place Editor */
        .mermaid-node-editor {
            position: fixed; background: var(--bg-body); color: var(--text-main);
            border: 2px solid var(--btn-primary-bg); border-radius: 4px;
            padding: 3px 8px; outline: none; box-shadow: 0 4px 14px rgba(0,0,0,0.25);
            font-size: 12px; z-index: 1000;
        }

        /* CSV Row Action Tools & Number Column (双向冻结行号列) */
        th.csv-num-col, td.csv-num-col {
            width: 48px; min-width: 48px; max-width: 52px;
            text-align: center; vertical-align: middle;
            background: var(--bg-table-th);
            color: var(--text-muted); font-size: 11px;
            font-variant-numeric: tabular-nums;
            user-select: none; padding: 6px 4px;
            position: sticky; left: 0;
            box-shadow: 1px 0 0 var(--border);
        }
        th.csv-num-col {
            z-index: 25 !important;
            box-shadow: 1px 0 0 var(--border), 0 1px 0 var(--border), 0 3px 6px -2px rgba(0, 0, 0, 0.08);
        }
        td.csv-num-col {
            z-index: 5;
        }
        tr:hover td.csv-num-col {
            background: var(--border);
        }
        .csv-row-num-wrap {
            position: relative; display: flex; align-items: center; justify-content: center;
            width: 100%; height: 100%; min-height: 20px;
        }
        .csv-row-idx {
            display: inline-block; text-align: center; font-weight: 500;
            transition: opacity 0.15s ease;
        }
        .csv-row-del-btn {
            position: absolute; right: -2px; top: 50%; transform: translateY(-50%);
            background: var(--bg-card); border: 1px solid var(--border);
            color: var(--text-muted); cursor: pointer;
            padding: 2px; border-radius: 3px; display: none;
            box-shadow: 0 1px 2px rgba(0,0,0,0.08); line-height: 1;
        }
        tr:hover .csv-row-del-btn { display: inline-flex; }
        .csv-row-del-btn:hover { color: #ef4444; border-color: #ef4444; background: rgba(239, 68, 68, 0.1); }

        /* Universal Modal Backdrop & Dialog */
        .modal-backdrop {
            position: fixed; top: 0; left: 0; right: 0; bottom: 0;
            background: rgba(15, 23, 42, 0.45); backdrop-filter: blur(4px);
            display: flex; align-items: center; justify-content: center;
            z-index: 3000; animation: modalFadeIn 0.18s cubic-bezier(0.16, 1, 0.3, 1);
        }
        body.dark-mode .modal-backdrop {
            background: rgba(0, 0, 0, 0.7);
        }
        @keyframes modalFadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }
        .modal-dialog {
            background: var(--bg-card); border: 1px solid var(--border); border-radius: 10px;
            box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.15), 0 10px 10px -5px rgba(0, 0, 0, 0.08);
            width: 440px; max-width: 92vw; display: flex; flex-direction: column; overflow: hidden;
            animation: modalScaleUp 0.18s cubic-bezier(0.16, 1, 0.3, 1);
        }
        @keyframes modalScaleUp {
            from { transform: scale(0.95); opacity: 0; }
            to { transform: scale(1); opacity: 1; }
        }
        .modal-header {
            display: flex; align-items: center; justify-content: space-between;
            padding: 12px 16px; border-bottom: 1px solid var(--border); background: var(--bg-topbar);
        }
        .modal-title-group { display: flex; align-items: center; gap: 8px; }
        .modal-icon { color: var(--btn-primary-bg); display: flex; align-items: center; }
        .modal-title { font-size: 13px; font-weight: 600; color: var(--text-main); }
        .btn-modal-close {
            background: none; border: none; color: var(--text-muted); cursor: pointer;
            padding: 4px; border-radius: 4px; display: flex; align-items: center; transition: all 0.15s ease;
        }
        .btn-modal-close:hover { color: var(--text-main); background: var(--btn-hover); }
        .modal-body { padding: 14px 16px; display: flex; flex-direction: column; gap: 10px; }
        .modal-desc { font-size: 12px; color: var(--text-muted); line-height: 1.5; }

        /* Delimiter Status Bar */
        .delim-status-bar {
            display: flex; align-items: center; justify-content: space-between;
            background: var(--bg-body); border: 1px solid var(--border);
            padding: 8px 12px; border-radius: 6px; font-size: 12px;
        }
        .delim-status-label { color: var(--text-muted); font-size: 11.5px; }
        .delim-current-badge {
            background: var(--badge-bg); color: var(--badge-text);
            padding: 2px 8px; border-radius: 4px; font-weight: 600; font-size: 11.5px;
            font-family: Consolas, monospace;
        }

        /* Delimiter Option Cards - Vertical Stack for 100% Unclipped Readability */
        .delim-options-list, .delim-options-grid {
            display: flex; flex-direction: column; gap: 7px;
        }
        .delim-card {
            display: flex; align-items: center; gap: 11px; padding: 8px 12px;
            background: var(--bg-body); border: 1.5px solid var(--border); border-radius: 7px;
            cursor: pointer; transition: all 0.18s ease; user-select: none; text-align: left;
            position: relative;
        }
        .delim-card:hover { border-color: var(--border-hover); background: var(--btn-hover); }
        .delim-card.active {
            border-color: var(--btn-primary-bg); background: var(--badge-bg);
            box-shadow: 0 0 0 1px var(--btn-primary-bg);
        }
        .delim-radio-dot {
            width: 14px; height: 14px; border-radius: 50%;
            border: 1.5px solid var(--border-hover); background: var(--bg-card);
            flex-shrink: 0; display: inline-flex; align-items: center; justify-content: center;
            transition: all 0.18s ease;
        }
        .delim-card.active .delim-radio-dot {
            border-color: var(--btn-primary-bg); background: var(--btn-primary-bg);
            box-shadow: inset 0 0 0 3px var(--bg-card);
        }
        .delim-char {
            font-family: Consolas, monospace; font-size: 13px; font-weight: 700;
            width: 24px; height: 24px; display: inline-flex; align-items: center; justify-content: center;
            background: var(--bg-code); border: 1px solid var(--border); border-radius: 4px;
            color: var(--text-main); flex-shrink: 0;
        }
        .delim-card.active .delim-char {
            border-color: var(--btn-primary-bg); color: var(--badge-text);
        }
        .delim-card-text {
            display: flex; flex-direction: column; flex: 1; min-width: 0;
        }
        .delim-name {
            font-size: 12px; font-weight: 600; color: var(--text-main); line-height: 1.3;
        }
        .delim-hint {
            font-size: 11px; color: var(--text-muted); line-height: 1.35; margin-top: 1px;
            word-break: break-word; white-space: normal;
        }
        .delim-custom-card {
            display: flex; align-items: center; gap: 11px; padding: 7px 12px;
        }
        .delim-custom-input {
            width: 48px; height: 28px; padding: 2px 6px;
            border: 1.5px solid var(--border); border-radius: 5px;
            background: var(--bg-card); color: var(--text-main); font-size: 14px; font-weight: 700;
            text-align: center; outline: none; font-family: Consolas, monospace;
            transition: all 0.18s ease; flex-shrink: 0; margin-left: 8px;
        }
        .delim-custom-input:focus {
            border-color: var(--btn-primary-bg); box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.2);
        }
        .delim-card.active .delim-custom-input {
            border-color: var(--btn-primary-bg);
        }
        .modal-footer {
            display: flex; align-items: center; justify-content: flex-end; gap: 8px;
            padding: 10px 16px; border-top: 1px solid var(--border); background: var(--bg-topbar);
        }
        /* Custom Resolution Modal Styling */
        .res-input-row {
            display: flex; align-items: center; gap: 12px;
        }
        .res-input-group {
            flex: 1; display: flex; flex-direction: column; gap: 5px;
        }
        .res-input-group label {
            font-size: 11.5px; font-weight: 500; color: var(--text-muted);
        }
        .res-input-box {
            display: flex; align-items: center; border: 1.5px solid var(--border);
            border-radius: 6px; background: var(--bg-body); overflow: hidden;
            transition: all 0.18s ease;
        }
        .res-input-box:focus-within {
            border-color: var(--btn-primary-bg); box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.2);
        }
        .res-input-box input {
            flex: 1; border: none; background: transparent; padding: 7px 10px;
            font-size: 13px; font-weight: 600; color: var(--text-main);
            font-family: Consolas, monospace; outline: none; width: 100%;
        }
        .res-input-unit {
            padding: 0 10px; font-size: 11px; color: var(--text-muted); font-weight: 500;
            background: var(--btn-hover); border-left: 1px solid var(--border); line-height: 32px;
        }
        .res-preset-chips {
            display: flex; flex-wrap: wrap; gap: 6px; margin-top: 4px;
        }
        .res-chip {
            padding: 4px 10px; border-radius: 12px; border: 1px solid var(--border);
            background: var(--bg-body); color: var(--text-main); font-size: 11px;
            cursor: pointer; transition: all 0.15s ease;
        }
        .res-chip:hover {
            border-color: var(--btn-primary-bg); color: var(--btn-primary-bg);
            background: var(--btn-hover);
        }
        .res-checkbox-row {
            display: flex; align-items: center; gap: 8px; margin-top: 4px; cursor: pointer; user-select: none;
        }
        .res-checkbox-row input {
            cursor: pointer; width: 15px; height: 15px;
        }
        .res-checkbox-row span {
            font-size: 12px; color: var(--text-main);
        }
    </style>
</head>
<body class='").Append(darkModeClass).Append(" ").Append(formatClass).Append(@"'>
    <div class='topbar' id='omni-topbar'>
        <div id='top-loader' class='top-loader'><div class='top-loader-bar'></div></div>
        <span class='badge ").Append(badgeClass).Append(@"' id='topbar-badge'><span class='pulse-dot' id='pulse-dot'><span class='radar-ring'></span></span><span id='badge-text'>").Append(fmtBadge).Append(" | ").Append(statusTag).Append(@"</span></span>
        <div class='actions'>
            <div class='preview-actions' id='preview-actions'>").Append(toolbarButtons).Append(BuildFullscreenButton()).Append(@"</div>
            <div class='edit-actions' id='edit-actions'>").Append(editToolbarButtons).Append(@"</div>
        </div>
    </div>
    <div id='zen-exit-pill' class='zen-exit-pill' onclick='toggleZenFullscreen(false)' title='退出全屏 (Esc / F11)'>
        ").Append(IconMinimize).Append(@"<span>退出全屏</span>
    </div>
    <div id='recognition-hud' class='recognition-hud'>
        <div class='hud-radar-halo'><span class='hud-radar-core'></span><span class='hud-radar-ring'></span></div>
        <span id='recognition-hud-text' class='hud-text'>智能识别数据中...</span>
    </div>
    <div id='omni-global-loading' class='omni-global-loading'>
        <div class='loading-box'>
            <div class='loading-spinner-orbit'>
                <div class='spinner-ring'></div>
                <div class='spinner-ring-inner'></div>
                <div class='spinner-dot'></div>
            </div>
            <div class='global-loading-title' id='global-loading-title'>").Append(loadingTitle).Append(@"</div>
            <div class='global-loading-sub' id='global-loading-sub'>").Append(loadingSub).Append(@"</div>
        </div>
    </div>
    ").Append(bodyContent).Append(@"
    <div id='toast-container' class='toast-bubble'>已复制到剪贴板</div>
    <div id='csv-delim-modal' class='modal-backdrop' style='display:none;' onclick='if(event.target===this)closeCsvDelimModal()'>
        <div class='modal-dialog delim-modal-dialog'>
            <div class='modal-header'>
                <div class='modal-title-group'>
                    <span class='modal-icon'>").Append(IconReplace).Append(@"</span>
                    <span class='modal-title'>替换表格分隔符</span>
                </div>
                <button class='btn-modal-close' onclick='closeCsvDelimModal()' title='关闭 (Esc)'>").Append(IconClose).Append(@"</button>
            </div>
            <div class='modal-body'>
                <div class='delim-status-bar'>
                    <span class='delim-status-label'>当前检测到的分隔符：</span>
                    <span id='delim-current-badge' class='delim-current-badge'>逗号 ( , )</span>
                </div>
                <div class='delim-options-list delim-presets'>
                    <div class='delim-card' id='delim-card-comma' data-delim=',' onclick=""selectDelimPreset(',')"" role='button' tabindex='0'>
                        <span class='delim-radio-dot'></span>
                        <span class='delim-char'>,</span>
                        <div class='delim-card-text'>
                            <span class='delim-name'>逗号 (CSV)</span>
                            <span class='delim-hint'>标准逗号分隔 (Comma-Separated Values)</span>
                        </div>
                    </div>
                    <div class='delim-card' id='delim-card-tab' data-delim='\t' onclick=""selectDelimPreset('\t')"" role='button' tabindex='0'>
                        <span class='delim-radio-dot'></span>
                        <span class='delim-char'>⇥</span>
                        <div class='delim-card-text'>
                            <span class='delim-name'>制表符 (TSV)</span>
                            <span class='delim-hint'>Tab 分隔，便于与 Excel 表格互粘复制</span>
                        </div>
                    </div>
                    <div class='delim-card' id='delim-card-semi' data-delim=';' onclick=""selectDelimPreset(';')"" role='button' tabindex='0'>
                        <span class='delim-radio-dot'></span>
                        <span class='delim-char'>;</span>
                        <div class='delim-card-text'>
                            <span class='delim-name'>分号 (Semicolon)</span>
                            <span class='delim-hint'>欧洲常用格式，适用于字段值内含逗号时</span>
                        </div>
                    </div>
                    <div class='delim-card' id='delim-card-pipe' data-delim='|' onclick=""selectDelimPreset('|')"" role='button' tabindex='0'>
                        <span class='delim-radio-dot'></span>
                        <span class='delim-char'>|</span>
                        <div class='delim-card-text'>
                            <span class='delim-name'>竖线 (Pipe)</span>
                            <span class='delim-hint'>通用 DSV 格式，适用于 Markdown 表格与结构化日志</span>
                        </div>
                    </div>
                    <div class='delim-card delim-custom-card' id='delim-btn-custom' onclick='selectCustomDelimCard()' role='button' tabindex='0'>
                        <span class='delim-radio-dot'></span>
                        <span class='delim-char' id='delim-custom-char-preview'>?</span>
                        <div class='delim-card-text' style='flex:1;'>
                            <span class='delim-name'>自定义单字符分隔符</span>
                            <span class='delim-hint'>输入任意非空白字符（如 ^, ~, #, : 等）</span>
                        </div>
                        <input type='text' id='csv-custom-delim-input' class='delim-custom-input' maxlength='1' placeholder='输入' onfocus='selectCustomDelimCard()' oninput='onCustomDelimInput(this.value)' />
                    </div>
                </div>
            </div>
            <div class='modal-footer'>
                <button class='btn' onclick='closeCsvDelimModal()'>取消</button>
                <button class='btn primary' onclick='applyCsvDelimChange()'>").Append(IconCheck).Append(@"<span>确认替换</span></button>
            </div>
        </div>
    </div>
    <div id='html-custom-res-modal' class='modal-backdrop' style='display:none;' onclick='if(event.target===this)closeHtmlCustomResModal()'>
        <div class='modal-dialog' style='max-width: 440px;'>
            <div class='modal-header'>
                <div class='modal-title-group'>
                    <span class='modal-icon'>").Append(IconSparkles).Append(@"</span>
                    <span class='modal-title'>自定义视口分辨率</span>
                </div>
                <button class='btn-modal-close' onclick='closeHtmlCustomResModal()' title='关闭 (Esc)'>").Append(IconClose).Append(@"</button>
            </div>
            <div class='modal-body'>
                <div class='res-input-row'>
                    <div class='res-input-group'>
                        <label for='html-custom-w-input'>视口宽度 (Width)</label>
                        <div class='res-input-box'>
                            <input type='number' id='html-custom-w-input' min='100' max='8000' placeholder='1920' />
                            <span class='res-input-unit'>px</span>
                        </div>
                    </div>
                    <div class='res-input-group'>
                        <label for='html-custom-h-input'>视口高度 (Height)</label>
                        <div class='res-input-box'>
                            <input type='number' id='html-custom-h-input' min='100' max='8000' placeholder='1080' />
                            <span class='res-input-unit'>px</span>
                        </div>
                    </div>
                </div>
                <div style='margin-top: 6px;'>
                    <span class='delim-status-label' style='display:block; margin-bottom: 6px;'>常用规格快捷预设：</span>
                    <div class='res-preset-chips'>
                        <button type='button' class='res-chip' onclick='applyPresetCustomRes(1280, 720, false)'>720p (1280&times;720)</button>
                        <button type='button' class='res-chip' onclick='applyPresetCustomRes(1920, 1080, false)'>1080p (1920&times;1080)</button>
                        <button type='button' class='res-chip' onclick='applyPresetCustomRes(2560, 1440, false)'>2K (2560&times;1440)</button>
                        <button type='button' class='res-chip' onclick='applyPresetCustomRes(3840, 2160, false)'>4K (3840&times;2160)</button>
                        <button type='button' class='res-chip' onclick='applyPresetCustomRes(390, 844, true)'>iPhone 14/15 (390&times;844)</button>
                        <button type='button' class='res-chip' onclick='applyPresetCustomRes(768, 1024, true)'>iPad (768&times;1024)</button>
                    </div>
                </div>
                <label class='res-checkbox-row'>
                    <input type='checkbox' id='html-custom-mobile-cb' />
                    <span>启用移动端/真机边框外观展示</span>
                </label>
            </div>
            <div class='modal-footer'>
                <button class='btn' onclick='closeHtmlCustomResModal()'>取消</button>
                <button class='btn primary' onclick='confirmCustomResolution()'>").Append(IconCheck).Append(@"<span>确认应用</span></button>
            </div>
        </div>
    </div>
    <div id='omni-viewport-editor' class='omni-viewport-editor'>
        <div class='editor-subbar'>
            <div class='editor-subbar-tools' id='editor-subbar-tools'></div>
            <div class='editor-subbar-info' id='editor-subbar-info'>
                <button class='btn-pill btn-mini' id='btn-editor-layout-toggle' onclick='toggleEditorSplitLayout()' title='切换分栏联动与全屏视图'>全屏沉浸</button>
                <span id='editor-pos-info'>第 1 行, 第 1 列</span>
            </div>
        </div>
        <div class='editor-main-area'>
            <div class='editor-gutter' id='editor-gutter'>1</div>
            <textarea id='omni-editor-textarea' class='omni-editor-textarea' spellcheck='false' placeholder='在此编辑内容...' oninput='onEditorTextareaInput(this.value)' onscroll='syncEditorGutterScroll()' onkeydown='handleEditorKeydown(event)'></textarea>
        </div>
        <div class='editor-status-bar'>
            <div class='editor-status-left'>
                <span id='editor-char-count'>0 行, 0 字符</span>
            </div>
            <div class='editor-status-right'>
                <span id='editor-validation-tag' class='editor-validation-tag valid'>✓ 语法有效</span>
            </div>
        </div>
    </div>

    <script>
        window.__RAW_DOC_CONTENT__ = ").Append(SafeEmbedJson(JsonSerializer.Serialize(doc.Content ?? ""))).Append(@";
        window.__DOC_FORMAT__ = ").Append(JsonSerializer.Serialize(det.Format.ToString().ToLowerInvariant())).Append(@";
        function postAct(act) { window.chrome.webview.postMessage(act); }

        // ==================== UNIVERSAL IN-PLACE EDIT MODE STATE MACHINE ====================
        window.__isEditing = false;
        window.__ORIGINAL_DOC_CONTENT__ = '';
        window.__CSV_ROWS_BACKUP__ = null;

        window.toggleEditorSplitLayout = function() {
            const editorWrap = document.getElementById('omni-viewport-editor');
            const btn = document.getElementById('btn-editor-layout-toggle');
            if (!editorWrap) return;
            const isSplit = editorWrap.classList.toggle('split-drawer');
            if (btn) btn.textContent = isSplit ? '全屏沉浸' : '分栏联动';
            showToast(isSplit ? '已切换至分栏联动模式 (上方实时预览)' : '已切换至全屏沉浸编辑');
        };

        window.enterEditMode = function() {
            if (window.__isEditing) return;
            window.__isEditing = true;
            document.body.classList.add('is-editing-mode');

            const fmt = window.__DOC_FORMAT__;
            const badgeText = document.getElementById('badge-text');
            if (badgeText) badgeText.textContent = fmt.toUpperCase() + ' | 正在编辑';

            if (fmt === 'csv') {
                const root = document.getElementById('csv-root');
                if (root) root.classList.add('editing-mode');
                window.__CSV_ROWS_BACKUP__ = JSON.parse(JSON.stringify(window.__CSV_ROWS__ || []));
                renderCsvGrid();
                showToast('已进入表格编辑模式 (可直接点击编辑单元格，Ctrl+S 保存)');
                setTimeout(() => {
                    const firstCell = document.querySelector('#csv-root tbody td[contenteditable=""true""]');
                    if (firstCell) firstCell.focus();
                }, 50);
                return;
            }

            if (fmt === 'html') {
                const iframe = document.getElementById('html-preview-frame');
                if (iframe && iframe.contentDocument) {
                    window.__HTML_BACKUP__ = iframe.contentDocument.documentElement ? '<!DOCTYPE html>\n' + iframe.contentDocument.documentElement.outerHTML : (window.__RAW_HTML__ || '');
                    try {
                        iframe.contentDocument.designMode = 'on';
                        let editStyle = iframe.contentDocument.getElementById('__omni_edit_style');
                        if (!editStyle) {
                            editStyle = iframe.contentDocument.createElement('style');
                            editStyle.id = '__omni_edit_style';
                            editStyle.textContent = 'body { outline: 2px dashed rgba(37,99,235,0.45) !important; outline-offset: -2px; cursor: text !important; }';
                            iframe.contentDocument.head.appendChild(editStyle);
                        }
                        iframe.contentDocument.addEventListener('keydown', (e) => {
                            if ((e.ctrlKey || e.metaKey) && (e.key === 's' || e.key === 'S')) {
                                e.preventDefault(); window.saveAndExitEditMode();
                            } else if (e.key === 'Escape') {
                                e.preventDefault(); window.cancelEditMode();
                            }
                        });
                    } catch(e) {}
                    showToast('已进入 HTML 所见即所得编辑 (直接点击网页文本修改，Ctrl+S 保存)');
                    return;
                }
            }

            if (fmt === 'svg') {
                const canvas = document.getElementById('svg-canvas');
                if (canvas) {
                    window.__SVG_BACKUP__ = canvas.innerHTML;
                    canvas.classList.add('svg-editing-active');
                    showToast('已进入 SVG 画布直编 (双击文字就地编辑，点击图形改色，Ctrl+S 保存)');
                    return;
                }
            }

            if (fmt === 'code') {
                const codeEl = document.querySelector('#code-viewer-root code');
                if (codeEl) {
                    window.__CODE_BACKUP__ = window.__RAW_CODE__ || '';
                    codeEl.setAttribute('contenteditable', 'true');
                    codeEl.setAttribute('spellcheck', 'false');
                    codeEl.classList.add('code-in-place-active');
                    codeEl.focus();
                    showToast('已在代码高亮容器中就地直接编辑 (Ctrl+S 保存，Esc 取消)');
                    return;
                }
            }

            const editorWrap = document.getElementById('omni-viewport-editor');
            const textarea = document.getElementById('omni-editor-textarea');
            if (editorWrap && textarea) {
                editorWrap.classList.add('active');
                editorWrap.classList.add('split-drawer');
                const btn = document.getElementById('btn-editor-layout-toggle');
                if (btn) btn.textContent = '全屏沉浸';
                window.__ORIGINAL_DOC_CONTENT__ = window.__RAW_DOC_CONTENT__ || '';

                let contentToLoad = window.__RAW_DOC_CONTENT__ || '';
                if (fmt === 'json') {
                    try {
                        contentToLoad = JSON.stringify(JSON.parse(contentToLoad), null, 2);
                    } catch(e) {}
                } else if (fmt === 'xml') {
                    if (window.formatXmlString) {
                        contentToLoad = window.formatXmlString(contentToLoad);
                    }
                }
                textarea.value = contentToLoad;
                updateEditorSubbarTools(fmt);
                updateEditorGutter();
                validateEditorContent(fmt, contentToLoad);

                setTimeout(() => {
                    textarea.focus();
                    textarea.selectionStart = 0;
                    textarea.selectionEnd = 0;
                    updateEditorPosInfo();
                }, 50);

                showToast('已进入分栏就地编辑 (上方实时联动预览，Ctrl+S 保存，Esc 取消)');
            }
        };

        window.saveAndExitEditMode = function() {
            if (!window.__isEditing) return;

            const fmt = window.__DOC_FORMAT__;

            if (fmt === 'csv') {
                const serialized = window.serializeCsvRows();
                window.__RAW_DOC_CONTENT__ = serialized;
                postAct('UPDATE_BUFFER:' + serialized);

                const root = document.getElementById('csv-root');
                if (root) root.classList.remove('editing-mode');
                document.body.classList.remove('is-editing-mode');
                window.__isEditing = false;
                renderCsvGrid();
                showToast('已保存表格并同步至源文件');
                return;
            }

            if (fmt === 'html') {
                const iframe = document.getElementById('html-preview-frame');
                if (iframe && iframe.contentDocument) {
                    try {
                        const style = iframe.contentDocument.getElementById('__omni_edit_style');
                        if (style) style.remove();
                    } catch(e) {}
                    try { iframe.contentDocument.designMode = 'off'; } catch(e) {}
                    const newContent = '<!DOCTYPE html>\n' + (iframe.contentDocument.documentElement ? iframe.contentDocument.documentElement.outerHTML : '');
                    window.__RAW_HTML__ = newContent;
                    window.__RAW_DOC_CONTENT__ = newContent;
                    postAct('UPDATE_BUFFER:' + newContent);
                    document.body.classList.remove('is-editing-mode');
                    window.__isEditing = false;
                    showToast('已保存 HTML 并同步至源文件');
                    return;
                }
            }

            if (fmt === 'svg') {
                const canvas = document.getElementById('svg-canvas');
                if (canvas) {
                    canvas.classList.remove('svg-editing-active');
                    if (window.closeSvgShapeInspector) window.closeSvgShapeInspector();
                    const newContent = canvas.innerHTML.trim();
                    window.__RAW_SVG__ = newContent;
                    window.__RAW_DOC_CONTENT__ = newContent;
                    postAct('UPDATE_BUFFER:' + newContent);
                    document.body.classList.remove('is-editing-mode');
                    window.__isEditing = false;
                    showToast('已保存 SVG 矢量图并同步至源文件');
                    return;
                }
            }

            if (fmt === 'code') {
                const codeEl = document.querySelector('#code-viewer-root code');
                if (codeEl) {
                    codeEl.removeAttribute('contenteditable');
                    codeEl.classList.remove('code-in-place-active');
                    const newContent = codeEl.innerText || codeEl.textContent;
                    window.__RAW_CODE__ = newContent;
                    window.__RAW_DOC_CONTENT__ = newContent;
                    postAct('UPDATE_BUFFER:' + newContent);
                    document.body.classList.remove('is-editing-mode');
                    window.__isEditing = false;
                    if (window.updateCodeContent) {
                        window.updateCodeContent(newContent, null, window.__CODE_LANG__);
                    }
                    showToast('已保存代码并同步至源文件');
                    return;
                }
            }

            const textarea = document.getElementById('omni-editor-textarea');
            if (!textarea) return;
            const newContent = textarea.value;

            if (fmt === 'json') {
                try {
                    JSON.parse(newContent);
                } catch(err) {
                    showToast('JSON 语法错误，无法保存：' + err.message);
                    const tag = document.getElementById('editor-validation-tag');
                    if (tag) {
                        tag.className = 'editor-validation-tag invalid';
                        tag.textContent = '⚠ 语法错误: ' + err.message;
                    }
                    textarea.focus();
                    return;
                }
            } else if (fmt === 'yaml') {
                if (window.jsyaml && window.jsyaml.load) {
                    try {
                        window.jsyaml.load(newContent);
                    } catch(err) {
                        showToast('YAML 语法错误，无法保存：' + err.message);
                        const tag = document.getElementById('editor-validation-tag');
                        if (tag) {
                            tag.className = 'editor-validation-tag invalid';
                            tag.textContent = '⚠ 语法错误: ' + err.message;
                        }
                        textarea.focus();
                        return;
                    }
                }
            } else if (fmt === 'xml') {
                try {
                    const parser = new DOMParser();
                    const doc = parser.parseFromString(newContent, 'text/xml');
                    const errs = doc.getElementsByTagName('parsererror');
                    if (errs.length > 0) {
                        showToast('XML 语法错误，无法保存：' + (errs[0].textContent.split('\n')[0] || '格式不正确'));
                        const tag = document.getElementById('editor-validation-tag');
                        if (tag) {
                            tag.className = 'editor-validation-tag invalid';
                            tag.textContent = '⚠ XML 语法错误: ' + (errs[0].textContent.split('\n')[0] || '格式不正确');
                        }
                        textarea.focus();
                        return;
                    }
                } catch(err) {
                    showToast('XML 语法错误：' + err.message);
                    return;
                }
            }

            window.__RAW_DOC_CONTENT__ = newContent;
            postAct('UPDATE_BUFFER:' + newContent);

            const editorWrap = document.getElementById('omni-viewport-editor');
            if (editorWrap) {
                editorWrap.classList.remove('active');
                editorWrap.classList.remove('split-drawer');
            }
            document.body.classList.remove('is-editing-mode');
            window.__isEditing = false;

            if (fmt === 'markdown') {
                showToast('已保存 Markdown 并同步至源文件');
            } else if (fmt === 'json') {
                window.__RAW_JSON__ = newContent;
                initJsonView();
                showToast('已保存 JSON 并同步至源文件');
            } else if (fmt === 'yaml') {
                window.__RAW_YAML__ = newContent;
                initYamlView();
                showToast('已保存 YAML 并同步至源文件');
            } else if (fmt === 'xml') {
                window.__RAW_XML__ = newContent;
                initXmlView();
                showToast('已保存 XML 并刷新树视图');
            } else if (fmt === 'svg') {
                window.__RAW_SVG__ = newContent;
                const c = document.getElementById('svg-canvas');
                if (c) c.innerHTML = newContent;
                initSvgView();
                showToast('已保存 SVG 并重新渲染');
            } else if (fmt === 'mermaid') {
                window.__RAW_MERMAID__ = newContent;
                const root = document.getElementById('mermaid-root');
                if (root) {
                    root.removeAttribute('data-processed');
                    root.textContent = newContent;
                    initMermaidView(false);
                }
                showToast('已保存并重新渲染图表');
            } else if (fmt === 'html') {
                window.__RAW_HTML__ = newContent;
                const iframe = document.getElementById('html-preview-frame');
                if (iframe) iframe.srcdoc = newContent;
                showToast('已保存 HTML 并刷新预览');
            } else if (fmt === 'code') {
                window.__RAW_CODE__ = newContent;
                if (window.updateCodeContent) {
                    window.updateCodeContent(newContent, null, window.__CODE_LANG__);
                }
                showToast('已保存代码并同步至源文件');
            } else {
                showToast('已保存并同步至源文件');
            }
        };

        window.cancelEditMode = function() {
            if (!window.__isEditing) return;

            const fmt = window.__DOC_FORMAT__;
            if (fmt === 'csv') {
                if (window.__CSV_ROWS_BACKUP__) {
                    window.__CSV_ROWS__ = window.__CSV_ROWS_BACKUP__;
                }
                const root = document.getElementById('csv-root');
                if (root) root.classList.remove('editing-mode');
                document.body.classList.remove('is-editing-mode');
                window.__isEditing = false;
                renderCsvGrid();
                showToast('已取消编辑并还原数据');
                return;
            }

            if (fmt === 'html') {
                const iframe = document.getElementById('html-preview-frame');
                if (iframe && iframe.contentDocument) {
                    try {
                        const style = iframe.contentDocument.getElementById('__omni_edit_style');
                        if (style) style.remove();
                    } catch(e) {}
                    try { iframe.contentDocument.designMode = 'off'; } catch(e) {}
                    if (window.__HTML_BACKUP__) {
                        iframe.srcdoc = window.__HTML_BACKUP__;
                        window.__RAW_HTML__ = window.__HTML_BACKUP__;
                    }
                }
                document.body.classList.remove('is-editing-mode');
                window.__isEditing = false;
                showToast('已取消编辑并还原数据');
                return;
            }

            if (fmt === 'svg') {
                const canvas = document.getElementById('svg-canvas');
                if (canvas && window.__SVG_BACKUP__) {
                    canvas.innerHTML = window.__SVG_BACKUP__;
                    canvas.classList.remove('svg-editing-active');
                    if (window.closeSvgShapeInspector) window.closeSvgShapeInspector();
                    initSvgView();
                }
                document.body.classList.remove('is-editing-mode');
                window.__isEditing = false;
                showToast('已取消 SVG 编辑并还原');
                return;
            }

            if (fmt === 'code') {
                const codeEl = document.querySelector('#code-viewer-root code');
                if (codeEl) {
                    codeEl.removeAttribute('contenteditable');
                    codeEl.classList.remove('code-in-place-active');
                    if (window.__CODE_BACKUP__ !== undefined && window.updateCodeContent) {
                        window.updateCodeContent(window.__CODE_BACKUP__, null, window.__CODE_LANG__);
                    }
                }
                document.body.classList.remove('is-editing-mode');
                window.__isEditing = false;
                showToast('已取消代码编辑并还原');
                return;
            }

            const editorWrap = document.getElementById('omni-viewport-editor');
            if (editorWrap) {
                editorWrap.classList.remove('active');
                editorWrap.classList.remove('split-drawer');
            }
            document.body.classList.remove('is-editing-mode');
            window.__isEditing = false;

            const badgeText = document.getElementById('badge-text');
            if (badgeText) badgeText.textContent = fmt.toUpperCase() + ' | 已恢复';
            showToast('已取消编辑并返回预览');
        };

        window.toggleEditorDrawer = function(forceState) {
            if (forceState === false) {
                if (window.__isEditing) window.cancelEditMode();
            } else {
                if (window.__isEditing) window.saveAndExitEditMode();
                else window.enterEditMode();
            }
        };

        window.omniTriggerChange = function(newText, immediate) {
            window.__RAW_DOC_CONTENT__ = newText;
            postAct('UPDATE_BUFFER:' + (newText || ''));
            showToast('已同步至 Notepad++ (Ctrl+Z 撤销)');
        };

        window.omniFlushSync = function() {
            if (window.__isEditing) {
                window.saveAndExitEditMode();
            }
        };

        function updateEditorSubbarTools(fmt) {
            const tools = document.getElementById('editor-subbar-tools');
            if (!tools) return;
            let html = '';
            if (fmt === 'markdown') {
                html = `<span class='editor-tool-badge'>Markdown 文档</span>
                    <button class='btn-pill' onclick='insertMdSnippet(""bold"")' title='粗体 (Ctrl+B)'><b>B</b></button>
                    <button class='btn-pill' onclick='insertMdSnippet(""italic"")' title='斜体 (Ctrl+I)'><i>I</i></button>
                    <button class='btn-pill' onclick='insertMdSnippet(""h2"")' title='二级标题'>H2</button>
                    <button class='btn-pill' onclick='insertMdSnippet(""code"")' title='代码块'>&lt;/&gt;</button>
                    <button class='btn-pill' onclick='insertMdSnippet(""table"")' title='插入表格模板'>表格</button>
                    <button class='btn-pill' onclick='insertMdSnippet(""quote"")' title='引用'>引用</button>`;
            } else if (fmt === 'json') {
                html = `<span class='editor-tool-badge'>JSON 源码</span>
                    <button class='btn-pill' onclick='formatEditorJson()' title='按 2 空格美化缩进'>").Append(IconSparkles).Append(@"<span>美化</span></button>
                    <button class='btn-pill' onclick='minifyEditorJson()' title='压缩为单行'>").Append(IconCompress).Append(@"<span>压缩</span></button>`;
            } else if (fmt === 'yaml') {
                html = `<span class='editor-tool-badge'>YAML 源码</span>
                    <button class='btn-pill' onclick='beautifyEditorYaml()' title='美化 YAML 缩进'>").Append(IconSparkles).Append(@"<span>美化</span></button>`;
            } else if (fmt === 'svg') {
                html = `<span class='editor-tool-badge'>SVG 源码</span>
                    <button class='btn-pill' onclick='formatEditorSvg()' title='规范化格式化 XML'>").Append(IconSparkles).Append(@"<span>美化 XML</span></button>`;
            } else if (fmt === 'mermaid') {
                html = `<span class='editor-tool-badge'>Mermaid 源码</span>
                    <button class='btn-pill' onclick='insertMermaidSnippet(""flow"")' title='流程图模板'>流程图</button>
                    <button class='btn-pill' onclick='insertMermaidSnippet(""seq"")' title='时序图模板'>时序图</button>
                    <button class='btn-pill' onclick='insertMermaidSnippet(""class"")' title='类图模板'>类图</button>`;
            } else if (fmt === 'html') {
                html = `<span class='editor-tool-badge'>HTML 源码</span>
                    <button class='btn-pill' onclick='formatEditorHtml()' title='格式化 HTML 标签'>").Append(IconSparkles).Append(@"<span>美化</span></button>`;
            } else if (fmt === 'xml') {
                html = `<span class='editor-tool-badge'>XML 源码</span>
                    <button class='btn-pill' onclick='formatEditorXml()' title='按 2 空格美化 XML 缩进'>").Append(IconSparkles).Append(@"<span>美化</span></button>
                    <button class='btn-pill' onclick='copyEntireXml()' title='复制代码内容'>").Append(IconCopy).Append(@"<span>复制</span></button>`;
            } else if (fmt === 'code') {
                const langLabel = (window.__CODE_LANG__ || 'code').toUpperCase();
                html = `<span class='editor-tool-badge'>` + langLabel + ` 源码</span>
                    <button class='btn-pill' onclick='copyEntireCode()' title='复制代码内容'>").Append(IconCopy).Append(@"<span>复制代码</span></button>`;
            } else {
                html = `<span class='editor-tool-badge'>文档源码</span>`;
            }
            tools.innerHTML = html;
        }

        window.formatEditorXml = function() {
            const ta = document.getElementById('omni-editor-textarea');
            if (ta && window.formatXmlString) {
                ta.value = window.formatXmlString(ta.value);
                updateEditorGutter();
                validateEditorContent('xml', ta.value);
                showToast('已美化 XML 缩进');
            }
        };

        window.copyEntireXml = function() {
            const ta = document.getElementById('omni-editor-textarea');
            if (ta) {
                copyTextToClipboard(ta.value);
                showToast('已复制 XML 源码');
            }
        };

        function updateEditorGutter() {
            const textarea = document.getElementById('omni-editor-textarea');
            const gutter = document.getElementById('editor-gutter');
            const charCount = document.getElementById('editor-char-count');
            if (!textarea || !gutter) return;

            const text = textarea.value;
            const lines = text.split('\n').length;
            let gutterText = '';
            for (let i = 1; i <= lines; i++) {
                gutterText += i + '\n';
            }
            gutter.textContent = gutterText;
            if (charCount) charCount.textContent = `${lines} 行, ${text.length} 字符`;
        }

        function updateEditorPosInfo() {
            const textarea = document.getElementById('omni-editor-textarea');
            const pos = document.getElementById('editor-pos-info');
            if (!textarea || !pos) return;
            const start = textarea.selectionStart;
            const lines = textarea.value.substring(0, start).split('\n');
            const lineNum = lines.length;
            const colNum = lines[lines.length - 1].length + 1;
            pos.textContent = `第 ${lineNum} 行, 第 ${colNum} 列`;
        }

        window.syncEditorGutterScroll = function() {
            const textarea = document.getElementById('omni-editor-textarea');
            const gutter = document.getElementById('editor-gutter');
            if (textarea && gutter) {
                gutter.scrollTop = textarea.scrollTop;
            }
        };

        let livePreviewDebounce = null;
        window.onEditorTextareaInput = function(val) {
            updateEditorGutter();
            updateEditorPosInfo();
            const fmt = window.__DOC_FORMAT__;
            validateEditorContent(fmt, val);

            const editorWrap = document.getElementById('omni-viewport-editor');
            if (editorWrap && editorWrap.classList.contains('split-drawer')) {
                clearTimeout(livePreviewDebounce);
                livePreviewDebounce = setTimeout(() => {
                    if (fmt === 'mermaid') {
                        const root = document.getElementById('mermaid-root');
                        if (root) {
                            root.removeAttribute('data-processed');
                            root.textContent = val;
                            initMermaidView(false);
                        }
                    } else if (fmt === 'yaml') {
                        window.__RAW_YAML__ = val;
                        initYamlView();
                    } else if (fmt === 'json') {
                        try {
                            JSON.parse(val);
                            window.__RAW_JSON__ = val;
                            initJsonView();
                        } catch(e) {}
                    } else if (fmt === 'xml') {
                        try {
                            const parser = new DOMParser();
                            const doc = parser.parseFromString(val, 'text/xml');
                            if (doc.getElementsByTagName('parsererror').length === 0) {
                                window.__RAW_XML__ = val;
                                initXmlView();
                            }
                        } catch(e) {}
                    }
                }, 200);
            }
        };

        function validateEditorContent(fmt, val) {
            const tag = document.getElementById('editor-validation-tag');
            if (!tag) return;
            if (fmt === 'json') {
                try {
                    JSON.parse(val);
                    tag.className = 'editor-validation-tag valid';
                    tag.textContent = '✓ JSON 语法有效';
                } catch(e) {
                    tag.className = 'editor-validation-tag invalid';
                    tag.textContent = '⚠ 语法错误: ' + e.message;
                }
            } else if (fmt === 'yaml') {
                if (window.jsyaml && window.jsyaml.load) {
                    try {
                        window.jsyaml.load(val);
                        tag.className = 'editor-validation-tag valid';
                        tag.textContent = '✓ YAML 语法有效';
                    } catch(e) {
                        tag.className = 'editor-validation-tag invalid';
                        tag.textContent = '⚠ 语法错误: ' + e.message;
                    }
                } else {
                    tag.className = 'editor-validation-tag valid';
                    tag.textContent = 'YAML 文本模式';
                }
            } else if (fmt === 'xml') {
                try {
                    const parser = new DOMParser();
                    const doc = parser.parseFromString(val, 'text/xml');
                    const errs = doc.getElementsByTagName('parsererror');
                    if (errs.length > 0) {
                        tag.className = 'editor-validation-tag invalid';
                        tag.textContent = '⚠ XML 语法错误: ' + (errs[0].textContent.split('\n')[0] || '格式不正确');
                    } else {
                        tag.className = 'editor-validation-tag valid';
                        tag.textContent = '✓ XML 语法有效';
                    }
                } catch(e) {
                    tag.className = 'editor-validation-tag invalid';
                    tag.textContent = '⚠ XML 错误: ' + e.message;
                }
            } else if (fmt === 'svg') {
                if (val.includes('<svg') && val.includes('</svg>')) {
                    tag.className = 'editor-validation-tag valid';
                    tag.textContent = '✓ SVG 标签闭合';
                } else {
                    tag.className = 'editor-validation-tag invalid';
                    tag.textContent = '⚠ 缺少 <svg> 根标签';
                }
            } else if (fmt === 'code') {
                tag.className = 'editor-validation-tag valid';
                tag.textContent = '✓ 代码就绪';
            } else {
                tag.className = 'editor-validation-tag valid';
                tag.textContent = '就地编辑中';
            }
        }

        window.handleEditorKeydown = function(e) {
            const ta = e.target;
            if (e.key === 'Tab') {
                e.preventDefault();
                const start = ta.selectionStart;
                const end = ta.selectionEnd;
                if (e.shiftKey) {
                    const val = ta.value;
                    const lineStart = val.lastIndexOf('\n', start - 1) + 1;
                    if (val.substring(lineStart, lineStart + 2) === '  ') {
                        ta.value = val.substring(0, lineStart) + val.substring(lineStart + 2);
                        ta.selectionStart = Math.max(lineStart, start - 2);
                        ta.selectionEnd = Math.max(lineStart, end - 2);
                    }
                } else {
                    ta.value = ta.value.substring(0, start) + '  ' + ta.value.substring(end);
                    ta.selectionStart = ta.selectionEnd = start + 2;
                }
                window.onEditorTextareaInput(ta.value);
                return;
            }
            if (e.key === 'Enter') {
                const val = ta.value;
                const start = ta.selectionStart;
                const lineStart = val.lastIndexOf('\n', start - 1) + 1;
                const currentLine = val.substring(lineStart, start);
                const match = currentLine.match(/^(\s+)/);
                if (match) {
                    e.preventDefault();
                    const indent = match[1];
                    ta.value = val.substring(0, start) + '\n' + indent + val.substring(start);
                    ta.selectionStart = ta.selectionEnd = start + 1 + indent.length;
                    window.onEditorTextareaInput(ta.value);
                    return;
                }
            }
            if ((e.ctrlKey || e.metaKey) && (e.key === 's' || e.key === 'S')) {
                e.preventDefault();
                window.saveAndExitEditMode();
                return;
            }
            if (e.key === 'Escape') {
                e.preventDefault();
                window.cancelEditMode();
                return;
            }
            setTimeout(updateEditorPosInfo, 10);
        };

        window.formatEditorJson = function() {
            const ta = document.getElementById('omni-editor-textarea');
            if (!ta) return;

            try {
                const obj = JSON.parse(ta.value);
                ta.value = JSON.stringify(obj, null, 2);
                updateEditorGutter();
                validateEditorContent('json', ta.value);
                showToast('已格式化 JSON');
            } catch(err) {
                showToast('JSON 语法错误，无法格式化: ' + err.message);
            }
        };

        window.minifyEditorJson = function() {
            const ta = document.getElementById('omni-editor-textarea');
            if (!ta) return;
            try {
                const obj = JSON.parse(ta.value);
                ta.value = JSON.stringify(obj);
                updateEditorGutter();
                validateEditorContent('json', ta.value);
                showToast('已压缩 JSON');
            } catch(err) {
                showToast('JSON 语法错误，无法压缩: ' + err.message);
            }
        };

        window.beautifyEditorYaml = function() {
            const ta = document.getElementById('omni-editor-textarea');
            if (!ta) return;
            try {
                if (window.jsyaml && window.jsyaml.load && window.jsyaml.dump) {
                    const parsed = window.jsyaml.load(ta.value);
                    ta.value = window.jsyaml.dump(parsed, { indent: 2, lineWidth: -1 });
                    updateEditorGutter();
                    validateEditorContent('yaml', ta.value);
                    showToast('已美化 YAML 缩进');
                } else {
                    showToast('YAML 解析引擎初始化中，请稍候');
                }
            } catch(err) {
                showToast('YAML 语法错误，美化失败: ' + err.message);
            }
        };

        window.formatEditorSvg = function() {
            const ta = document.getElementById('omni-editor-textarea');
            if (!ta) return;
            let xml = ta.value.trim();
            let formatted = '';
            let pad = 0;
            const reg = /(>)(<)(\/*)/g;
            xml = xml.replace(reg, '$1\r\n$2$3');
            const lines = xml.split('\r\n');
            for (let i = 0; i < lines.length; i++) {
                let node = lines[i].trim();
                if (!node) continue;
                let indent = 0;
                if (node.match(/.+<\/\w[^>]*>$/)) {
                    indent = 0;
                } else if (node.match(/^<\/\w/) && pad > 0) {
                    pad--;
                } else if (node.match(/^<\w[^>]*[^\/]>.*$/)) {
                    indent = 1;
                } else {
                    indent = 0;
                }
                let padding = '';
                for (let j = 0; j < pad; j++) padding += '  ';
                formatted += padding + node + '\r\n';
                pad += indent;
            }
            ta.value = formatted.trim();
            updateEditorGutter();
            validateEditorContent('svg', ta.value);
            showToast('已格式化 XML');
        };

        window.formatEditorHtml = function() {
            window.formatEditorSvg();
        };

        window.insertMdSnippet = function(type) {
            const ta = document.getElementById('omni-editor-textarea');
            if (!ta) return;
            const start = ta.selectionStart;
            const end = ta.selectionEnd;
            const sel = ta.value.substring(start, end);
            let before = '', after = '', insert = '';
            if (type === 'bold') { before = '**'; after = '**'; insert = sel || '粗体文字'; }
            else if (type === 'italic') { before = '*'; after = '*'; insert = sel || '斜体文字'; }
            else if (type === 'h2') { before = '\n## '; after = '\n'; insert = sel || '标题'; }
            else if (type === 'code') { before = '\n```\n'; after = '\n```\n'; insert = sel || 'code here'; }
            else if (type === 'table') { insert = '\n| 列 1 | 列 2 | 列 3 |\n| --- | --- | --- |\n| 内容 A | 内容 B | 内容 C |\n'; }
            else if (type === 'quote') { before = '\n> '; after = '\n'; insert = sel || '引用内容'; }
            ta.value = ta.value.substring(0, start) + before + insert + after + ta.value.substring(end);
            ta.focus();
            ta.selectionStart = start + before.length;
            ta.selectionEnd = start + before.length + insert.length;
            updateEditorGutter();
        };

        window.insertMermaidSnippet = function(type) {
            const ta = document.getElementById('omni-editor-textarea');
            if (!ta) return;
            let snippet = '';
            if (type === 'flow') {
                snippet = '\nflowchart TD\n    A[开始] --> B{判断}\n    B -- 是 --> C[处理]\n    B -- 否 --> D[结束]\n    C --> D\n';
            } else if (type === 'seq') {
                snippet = '\nsequenceDiagram\n    autonumber\n    actor 用户\n    participant 系统\n    用户->>系统: 发送请求\n    系统-->>用户: 返回数据\n';
            } else if (type === 'class') {
                snippet = '\nclassDiagram\n    class Animal {\n        +String name\n        +move()\n    }\n';
            }
            const start = ta.selectionStart;
            ta.value = ta.value.substring(0, start) + snippet + ta.value.substring(ta.selectionEnd);
            ta.focus();
            ta.selectionStart = ta.selectionEnd = start + snippet.length;
            updateEditorGutter();
        };

        // ==================== IN-PLACE PREVIEW EDITING CONTROLLERS ====================
        window.initMarkdownInPlaceEditing = function() {
            const container = document.getElementById('md-container');
            if (!container) return;

            container.addEventListener('dblclick', function(e) {
                if (e.target.tagName && (e.target.tagName.toLowerCase() === 'input' || e.target.tagName.toLowerCase() === 'a')) return;
                const block = e.target.closest('p, h1, h2, h3, h4, h5, h6, blockquote, li, pre');
                if (!block || block.classList.contains('md-block-editing') || block.querySelector('.md-block-textarea')) return;
                e.stopPropagation();

                startEditMarkdownBlock(block);
            });
        };

        function startEditMarkdownBlock(block) {
            block.__ORIGINAL_HTML__ = block.innerHTML;
            block.classList.add('md-block-editing');

            let lineIdx = -1;
            let targetEl = block;
            while (targetEl && targetEl !== document.body) {
                let id = targetEl.id || '';
                let m = id.match(/^pragma-line-(\d+)$/);
                if (m) { lineIdx = parseInt(m[1], 10); break; }
                let a = targetEl.querySelector && targetEl.querySelector('a[id^=""pragma-line-""]');
                if (a) {
                    let am = a.id.match(/^pragma-line-(\d+)$/);
                    if (am) { lineIdx = parseInt(am[1], 10); break; }
                }
                targetEl = targetEl.parentElement;
            }

            let initialText = '';
            if (lineIdx >= 0 && window.__RAW_DOC_CONTENT__) {
                const lines = window.__RAW_DOC_CONTENT__.split('\n');
                initialText = lines[lineIdx] || block.innerText.trim();
            } else {
                let tag = block.tagName.toLowerCase();
                let prefix = '';
                if (tag === 'h1') prefix = '# ';
                else if (tag === 'h2') prefix = '## ';
                else if (tag === 'h3') prefix = '### ';
                else if (tag === 'h4') prefix = '#### ';
                else if (tag === 'h5') prefix = '##### ';
                else if (tag === 'h6') prefix = '###### ';
                else if (tag === 'blockquote') prefix = '> ';
                else if (tag === 'li') prefix = '- ';
                initialText = prefix + block.innerText.trim();
            }

            block.innerHTML = `
                <div class='md-block-editor-wrap'>
                    <textarea class='md-block-textarea' spellcheck='false'></textarea>
                    <div class='md-block-editor-bar'>
                        <span class='md-block-tip'>Ctrl+Enter 保存并编译 · Esc 取消</span>
                        <div class='md-block-actions'>
                            <button class='btn-pill btn-mini primary' type='button' id='btn-md-block-save'>保存</button>
                            <button class='btn-pill btn-mini' type='button' id='btn-md-block-cancel'>取消</button>
                        </div>
                    </div>
                </div>
            `;

            const ta = block.querySelector('.md-block-textarea');
            ta.value = initialText;
            ta.style.height = Math.max(ta.scrollHeight, 46) + 'px';
            setTimeout(() => { ta.focus(); ta.selectionStart = ta.selectionEnd = ta.value.length; }, 20);

            ta.addEventListener('input', () => {
                ta.style.height = 'auto';
                ta.style.height = Math.max(ta.scrollHeight, 46) + 'px';
            });

            function commit() {
                const newText = ta.value;
                block.classList.remove('md-block-editing');
                if (lineIdx >= 0 && window.__RAW_DOC_CONTENT__) {
                    const lines = window.__RAW_DOC_CONTENT__.split('\n');
                    lines[lineIdx] = newText;
                    const fullText = lines.join('\n');
                    window.__RAW_DOC_CONTENT__ = fullText;
                    window.omniTriggerChange(fullText, false);
                } else if (window.__RAW_DOC_CONTENT__) {
                    const oldTrimmed = block.innerText.trim();
                    if (oldTrimmed) {
                        const updated = window.__RAW_DOC_CONTENT__.replace(oldTrimmed, newText);
                        window.__RAW_DOC_CONTENT__ = updated;
                        window.omniTriggerChange(updated, false);
                    }
                }
                showToast('已更新段落并同步至源文件');
            }

            function cancel() {
                block.classList.remove('md-block-editing');
                block.innerHTML = block.__ORIGINAL_HTML__;
            }

            ta.addEventListener('keydown', (evt) => {
                if (evt.ctrlKey && evt.key === 'Enter') { evt.preventDefault(); commit(); }
                else if (evt.key === 'Escape') { evt.preventDefault(); cancel(); }
            });

            const btnSave = block.querySelector('#btn-md-block-save');
            const btnCancel = block.querySelector('#btn-md-block-cancel');
            if (btnSave) btnSave.onclick = commit;
            if (btnCancel) btnCancel.onclick = cancel;
        }

        let activeSvgShape = null;
        window.initSvgInPlaceEditing = function() {
            const canvas = document.getElementById('svg-canvas');
            if (!canvas) return;
            if (canvas.hasAttribute('data-inplace-bound')) return;
            canvas.setAttribute('data-inplace-bound', 'true');

            canvas.addEventListener('dblclick', function(e) {
                const textNode = e.target.closest('text, tspan');
                if (!textNode) return;
                e.preventDefault();
                e.stopPropagation();

                // 立即解除平移锁定，杜绝画布跟随鼠标晃动
                svgPanning = false;
                const canvasEl = document.getElementById('svg-canvas');
                if (canvasEl) canvasEl.classList.remove('is-panning');
                const vp = document.getElementById('svg-viewport');
                if (vp) vp.style.cursor = 'grab';

                // 清理可能已存在的其他输入框，防止重叠抖动
                const existingInput = document.querySelector('.svg-inline-text-editor');
                if (existingInput) existingInput.remove();

                const rect = textNode.getBoundingClientRect();
                const currentText = textNode.textContent.trim();
                if (!currentText) return;
                const computed = window.getComputedStyle(textNode);

                const input = document.createElement('input');
                input.className = 'svg-inline-text-editor';
                input.value = currentText;
                input.style.left = Math.max(10, Math.round(rect.left)) + 'px';
                input.style.top = Math.max(45, Math.round(rect.top + (rect.height - 28) / 2)) + 'px';
                input.style.width = Math.max(Math.round(rect.width) + 24, 90) + 'px';
                input.style.fontFamily = computed.fontFamily;
                input.style.fontSize = computed.fontSize;

                let committed = false;
                function commit() {
                    if (committed) return;
                    committed = true;
                    input.removeEventListener('blur', commit);
                    const newText = input.value.trim();
                    input.remove();

                    if (!newText || newText === currentText) return;

                    // 1. 立即更新 DOM 中文本，保证预览区即时生效
                    textNode.textContent = newText;

                    // 2. 在原始 SVG 文档中精准安全替换，严密保留 XML 声明、注释、样式与缩进排版
                    const oldRaw = window.__RAW_DOC_CONTENT__ || window.__RAW_SVG__ || '';
                    if (!oldRaw) return;

                    let newRaw = oldRaw;
                    if (oldRaw.includes(currentText)) {
                        newRaw = oldRaw.replace(currentText, newText);
                    } else {
                        const escCurrent = escapeHtml(currentText);
                        if (oldRaw.includes(escCurrent)) {
                            newRaw = oldRaw.replace(escCurrent, escapeHtml(newText));
                        } else {
                            const esc = currentText.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
                            newRaw = oldRaw.replace(new RegExp(esc), newText);
                        }
                    }

                    window.__RAW_SVG__ = newRaw;
                    window.__RAW_DOC_CONTENT__ = newRaw;
                    window.omniTriggerChange(newRaw, false);
                    showToast('已修改 SVG 文本并同步至 Notepad++');
                }

                input.addEventListener('keydown', (evt) => {
                    if (evt.key === 'Enter') {
                        evt.preventDefault();
                        commit();
                    } else if (evt.key === 'Escape') {
                        evt.preventDefault();
                        committed = true;
                        input.removeEventListener('blur', commit);
                        input.remove();
                    }
                });
                input.addEventListener('blur', commit);

                document.body.appendChild(input);
                setTimeout(() => {
                    input.focus({ preventScroll: true });
                    input.select();
                }, 20);
            });

            canvas.addEventListener('click', function(e) {
                const shape = e.target.closest('path, rect, circle, ellipse, polygon, line, polyline');
                if (!shape || shape.tagName.toLowerCase() === 'svg') return;
                if (!document.body.classList.contains('is-editing-mode')) return;
                e.stopPropagation();
                selectSvgShape(shape);
            });
        };

        function selectSvgShape(shape) {
            closeSvgShapeInspector();
            activeSvgShape = shape;

            const rect = shape.getBoundingClientRect();
            const comp = window.getComputedStyle(shape);

            const inspector = document.createElement('div');
            inspector.id = 'svg-shape-inspector';
            inspector.className = 'svg-shape-inspector';
            inspector.style.left = Math.min(window.innerWidth - 260, Math.max(10, rect.left)) + 'px';
            inspector.style.top = Math.max(45, rect.top - 40) + 'px';

            let curFill = shape.getAttribute('fill') || comp.fill || '#3b82f6';
            if (curFill === 'none') curFill = '#3b82f6';
            let curStroke = shape.getAttribute('stroke') || comp.stroke || '#1e293b';
            if (curStroke === 'none') curStroke = '#1e293b';
            let curWidth = parseFloat(shape.getAttribute('stroke-width') || comp.strokeWidth || '1');

            inspector.innerHTML = `
                <label>填充 <input type='color' id='svg-insp-fill' value='${curFill}' /></label>
                <label>描边 <input type='color' id='svg-insp-stroke' value='${curStroke}' /></label>
                <label>粗细 <input type='number' id='svg-insp-width' min='0' max='30' step='1' value='${curWidth}' /></label>
                <button class='btn-pill btn-mini' id='btn-svg-insp-close' type='button'>✕</button>
            `;

            inspector.querySelector('#svg-insp-fill').oninput = function() {
                shape.setAttribute('fill', this.value);
            };
            inspector.querySelector('#svg-insp-stroke').oninput = function() {
                shape.setAttribute('stroke', this.value);
            };
            inspector.querySelector('#svg-insp-width').oninput = function() {
                shape.setAttribute('stroke-width', this.value);
            };
            inspector.querySelector('#btn-svg-insp-close').onclick = closeSvgShapeInspector;

            document.body.appendChild(inspector);
        }

        window.closeSvgShapeInspector = function() {
            const insp = document.getElementById('svg-shape-inspector');
            if (insp) insp.remove();
            activeSvgShape = null;
        };

        window.initMermaidInPlaceEditing = function() {
            const canvas = document.getElementById('mermaid-canvas');
            if (!canvas) return;
            if (canvas.dataset.inplaceBound === 'true') return;
            canvas.dataset.inplaceBound = 'true';

            canvas.addEventListener('dblclick', function(e) {
                // 中断正在进行的平移拖拽，防止冲突
                mmPanning = false;
                canvas.classList.remove('is-panning');

                const target = e.target.closest('.node, .cluster, .actor, .label, g[id^=""flowchart-""], .title-text, .sectionTitle');
                if (!target) return;
                e.stopPropagation();

                // 优先以用户实际点击的文本元素为准进行坐标计算与文本获取
                const clickedTextEl = e.target.closest('text, tspan, .nodeLabel, span, p') || target.querySelector('.nodeLabel, text, span') || target;
                const currentText = clickedTextEl.textContent.trim();
                if (!currentText) return;

                // 获取真实点击的文本元素 rect，防止 cluster / subgraph 整个容器过大导致编辑框居中偏移
                const textRect = clickedTextEl.getBoundingClientRect();
                const targetRect = target.getBoundingClientRect();
                const useTextRect = (textRect.width > 0 && textRect.height > 0);
                const rect = useTextRect ? textRect : targetRect;

                const input = document.createElement('input');
                input.className = 'mermaid-node-editor';
                input.value = currentText;
                
                // 居中或紧贴在点击文本处
                const posX = Math.max(10, rect.left);
                const posY = Math.max(45, rect.top + (rect.height - 28) / 2);
                input.style.left = posX + 'px';
                input.style.top = posY + 'px';
                input.style.width = Math.max(rect.width + 30, 120) + 'px';

                let committed = false;
                function commit() {
                    if (committed) return;
                    committed = true;
                    input.removeEventListener('blur', commit);
                    const newText = input.value.trim();
                    input.remove();
                    if (newText && newText !== currentText && window.__RAW_MERMAID__) {
                        const oldContent = window.__RAW_MERMAID__;
                        const esc = currentText.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
                        const newContent = oldContent.replace(new RegExp(esc, 'g'), newText);
                        window.__RAW_MERMAID__ = newContent;
                        window.omniTriggerChange(newContent, true);
                        const root = document.getElementById('mermaid-root');
                        if (root) {
                            root.removeAttribute('data-processed');
                            root.textContent = newContent;
                            initMermaidView(false);
                        }
                        showToast('已修改节点并更新图表');
                    }
                }

                input.addEventListener('keydown', function(evt) {
                    if (evt.key === 'Enter') { evt.preventDefault(); commit(); }
                    else if (evt.key === 'Escape') { 
                        evt.preventDefault(); 
                        committed = true;
                        input.removeEventListener('blur', commit);
                        input.remove(); 
                    }
                });
                input.addEventListener('blur', commit);

                document.body.appendChild(input);
                setTimeout(() => { input.focus(); input.select(); }, 20);
            });
        };

        // Global shortcuts for editing & Zen Fullscreen
        window.addEventListener('keydown', (e) => {
            if ((e.ctrlKey || e.metaKey) && (e.key === 's' || e.key === 'S')) {
                if (window.__isEditing) {
                    e.preventDefault();
                    window.saveAndExitEditMode();
                    return;
                }
            }
            if (e.key === 'F2') {
                e.preventDefault();
                if (window.__isEditing) window.saveAndExitEditMode();
                else window.enterEditMode();
                return;
            }
            if (e.key === 'F11' || (e.altKey && e.key === 'Enter')) {
                e.preventDefault();
                window.toggleZenFullscreen();
                return;
            }
            if (e.key === 'Escape') {
                const modal = document.getElementById('csv-delim-modal');
                if (modal && modal.style.display !== 'none') {
                    e.preventDefault();
                    window.closeCsvDelimModal();
                    return;
                }
                if (window.__isEditing) {
                    e.preventDefault();
                    window.cancelEditMode();
                    return;
                }
                if (document.body.classList.contains('zen-fullscreen')) {
                    e.preventDefault();
                    window.toggleZenFullscreen(false);
                    return;
                }
            }
            if (e.key === 'Enter') {
                const modal = document.getElementById('csv-delim-modal');
                if (modal && modal.style.display !== 'none') {
                    e.preventDefault();
                    window.applyCsvDelimChange();
                    return;
                }
            }
        });

        // ==================== ZEN FULLSCREEN CONTROLLER ====================
        const ICON_ZEN_MAXIMIZE = `<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M8 3H5a2 2 0 0 0-2 2v3m18 0V5a2 2 0 0 0-2-2h-3m0 18h3a2 2 0 0 0 2-2v-3M3 16v3a2 2 0 0 0 2 2h3'/></svg>`;
        const ICON_ZEN_MINIMIZE = `<svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M8 3v3a2 2 0 0 1-2 2H3m18 0h-3a2 2 0 0 1-2-2V3m0 18v-3a2 2 0 0 1 2-2h3M3 16h3a2 2 0 0 1 2 2v3'/></svg>`;

        window.syncFullScreenState = function(isFull) {
            if (isFull) {
                document.body.classList.add('zen-fullscreen');
                const btn = document.getElementById('btn-zen-fullscreen');
                if (btn) {
                    btn.innerHTML = ICON_ZEN_MINIMIZE + '<span>还原</span>';
                    btn.title = '退出全屏 (快捷键 Esc / F11)';
                }
                showToast('已进入全屏展示 (按 Esc 或 F11 退出)');
            } else {
                document.body.classList.remove('zen-fullscreen');
                const topbar = document.getElementById('omni-topbar');
                if (topbar) topbar.classList.remove('zen-revealed');
                const btn = document.getElementById('btn-zen-fullscreen');
                if (btn) {
                    btn.innerHTML = ICON_ZEN_MAXIMIZE + '<span>全屏</span>';
                    btn.title = '全屏沉浸展示 (快捷键 F11 / Alt+Enter)';
                }
                showToast('已退出全屏展示');
            }
            setTimeout(() => {
                window.dispatchEvent(new Event('resize'));
            }, 60);
        };

        window.toggleZenFullscreen = function(force) {
            const isZen = document.body.classList.contains('zen-fullscreen');
            const target = (typeof force === 'boolean') ? force : !isZen;
            window.syncFullScreenState(target);
            postAct(target ? 'ENTER_FULLSCREEN' : 'EXIT_FULLSCREEN');
        };

        let zenHideTimer = null;
        window.addEventListener('mousemove', (e) => {
            if (!document.body.classList.contains('zen-fullscreen')) return;
            const topbar = document.getElementById('omni-topbar');
            if (!topbar) return;
            if (e.clientY <= 55) {
                topbar.classList.add('zen-revealed');
                if (zenHideTimer) clearTimeout(zenHideTimer);
            } else if (e.clientY > 100) {
                if (!zenHideTimer) {
                    zenHideTimer = setTimeout(() => {
                        topbar.classList.remove('zen-revealed');
                        zenHideTimer = null;
                    }, 300);
                }
            }
        });

        // ==================== SMOOTH LOADER & RECOGNITION CONTROLLER ====================
        let loaderTimer = null;
        let globalLoadingTimer = null;

        window.showGlobalLoading = function(title, sub) {
            const el = document.getElementById('omni-global-loading');
            if (el) {
                const tEl = document.getElementById('global-loading-title');
                const sEl = document.getElementById('global-loading-sub');
                if (tEl && title) tEl.textContent = title;
                if (sEl && sub) sEl.textContent = sub;
                el.classList.remove('hidden');
            }
            const l = document.getElementById('top-loader');
            const d = document.getElementById('pulse-dot');
            if (l) l.classList.add('active');
            if (d) d.classList.add('busy');
            if (globalLoadingTimer) clearTimeout(globalLoadingTimer);
            // 5 秒超时防死锁保护
            globalLoadingTimer = setTimeout(() => { window.hideGlobalLoading(); }, 5000);
        };

        window.hideGlobalLoading = function() {
            if (globalLoadingTimer) {
                clearTimeout(globalLoadingTimer);
                globalLoadingTimer = null;
            }
            const el = document.getElementById('omni-global-loading');
            if (el) {
                requestAnimationFrame(() => {
                    setTimeout(() => {
                        el.classList.add('hidden');
                    }, 120);
                });
            }
            const l = document.getElementById('top-loader');
            const d = document.getElementById('pulse-dot');
            if (l) setTimeout(() => { l.classList.remove('active'); }, 180);
            if (d) setTimeout(() => { d.classList.remove('busy'); }, 180);
        };

        window.startLoader = function(hint) {
            window.showGlobalLoading(hint || '正在智能解析与渲染文档视图...');
            const b = document.getElementById('badge-text');
            const d = document.getElementById('pulse-dot');
            if (b) {
                if (!b.getAttribute('data-origin-text')) b.setAttribute('data-origin-text', b.textContent);
                b.textContent = '识别中...';
            }
            if (d) d.classList.add('busy');

            if (loaderTimer) clearTimeout(loaderTimer);
            loaderTimer = setTimeout(() => { window.finishLoader(); }, 4000);
        };

        window.finishLoader = function(fmtLabel) {
            window.hideGlobalLoading();
            if (loaderTimer) clearTimeout(loaderTimer);

            const d = document.getElementById('pulse-dot');
            if (d) d.classList.remove('busy');

            const b = document.getElementById('badge-text');
            if (b) {
                if (fmtLabel) {
                    b.textContent = fmtLabel.includes('|') ? fmtLabel : (fmtLabel + ' | 已就绪');
                } else {
                    const orig = b.getAttribute('data-origin-text');
                    if (orig && b.textContent.includes('...')) b.textContent = orig;
                }
            }

            const badge = document.getElementById('topbar-badge');
            if (badge) {
                badge.classList.remove('badge-spring-animate');
                void badge.offsetWidth;
                badge.classList.add('badge-spring-animate');
            }
        };

        window.startRecognition = function(hint) {
            window.startLoader(hint || '智能识别数据结构中...');
        };

        window.finishRecognition = function(fmtLabel) {
            window.finishLoader(fmtLabel);
        };

        function setBadgeStatus(statusText) {
            const b = document.getElementById('badge-text');
            if (b && statusText) b.textContent = statusText;
            else {
                const legacy = document.getElementById('topbar-badge');
                if (legacy && statusText && !b) legacy.textContent = statusText;
            }
        }

        function showToast(msg) {
            const t = document.getElementById('toast-container');
            if (!t) return;
            t.textContent = msg || '已复制到剪贴板';
            t.classList.add('show');
            setTimeout(() => t.classList.remove('show'), 1600);
        }

        function copyTextToClipboard(text) {
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(text).then(() => showToast()).catch(() => fallbackCopy(text));
            } else {
                fallbackCopy(text);
            }
        }

        function fallbackCopy(text) {
            const ta = document.createElement('textarea');
            ta.value = text;
            ta.style.position = 'fixed'; ta.style.left = '-9999px';
            document.body.appendChild(ta);
            ta.select();
            try { document.execCommand('copy'); showToast(); } catch(e){}
            document.body.removeChild(ta);
        }

        function escapeHtml(str) {
            return String(str).replace(/&/g, '&amp;')
                              .replace(/</g, '&lt;')
                              .replace(/>/g, '&gt;')
                              .replace(/""/g, '&quot;')
                              .replace(/'/g, '&#39;');
        }

        // ==================== JSON VIEW LOGIC (专业树状层级与内联类型) ====================
        const JSON_CHEVRON_SVG = `<svg class='tree-arrow' viewBox='0 0 24 24' width='13' height='13' fill='none' stroke='currentColor' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'><polyline points='9 18 15 12 9 6'></polyline></svg>`;
        const JSON_SPACER_HTML = `<span class='tree-spacer'></span>`;
        const JSON_COPY_SVG = `<svg width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><rect x='9' y='9' width='13' height='13' rx='2'></rect><path d='M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1'></path></svg>`;

        function makeJsonCopyBtn(val) {
            let serialized = '';
            try {
                serialized = typeof val === 'string' ? val : JSON.stringify(val);
            } catch(e) {
                serialized = String(val);
            }
            const safe = encodeURIComponent(serialized);
            return `<div class='json-actions'><button class='json-copy-btn' data-copy='${safe}' onclick='copyJsonNodeVal(event, this)' title='复制此项内容'>${JSON_COPY_SVG}<span>复制</span></button></div>`;
        }

        window.copyJsonNodeVal = function(e, btn) {
            e.stopPropagation();
            e.preventDefault();
            const raw = btn.getAttribute('data-copy');
            if (raw !== null) {
                let text = decodeURIComponent(raw);
                try {
                    if (text.startsWith('{') || text.startsWith('[')) {
                        text = JSON.stringify(JSON.parse(text), null, 2);
                    }
                } catch(err) {}
                copyTextToClipboard(text);
                showToast('已复制节点内容');
            }
        };

        function buildJsonTreeHtml(val, keyName, isArrayChild, path = []) {
            let keyPrefix = '';
            const pathStr = encodeURIComponent(JSON.stringify(path));
            const parentPathStr = encodeURIComponent(JSON.stringify(path.slice(0, -1)));

            if (isArrayChild) {
                keyPrefix = `<span class='json-idx'>[${keyName}]</span><span class='json-colon'>:</span>`;
            } else if (keyName !== undefined) {
                keyPrefix = `<span class='json-key' data-key='${escapeHtml(keyName)}' data-parent-path='${parentPathStr}' ondblclick='startEditJsonKey(this)' title='双击修改键名'>""${escapeHtml(keyName)}""</span><span class='json-colon'>:</span>`;
            }

            if (val === null) {
                return `<div class='json-row'><div class='json-main'>${JSON_SPACER_HTML}${keyPrefix}<span class='json-null' data-path='${pathStr}' ondblclick='startEditJsonVal(this)' title='双击修改数值'>null</span><span class='type-tag type-null'>null</span></div>${makeJsonCopyBtn('null')}</div>`;
            }
            if (typeof val === 'boolean') {
                return `<div class='json-row'><div class='json-main'>${JSON_SPACER_HTML}${keyPrefix}<span class='json-bool' data-path='${pathStr}' ondblclick='startEditJsonVal(this)' title='双击修改数值'>${val}</span><span class='type-tag type-bool'>bool</span></div>${makeJsonCopyBtn(String(val))}</div>`;
            }
            if (typeof val === 'number') {
                return `<div class='json-row'><div class='json-main'>${JSON_SPACER_HTML}${keyPrefix}<span class='json-num' data-path='${pathStr}' ondblclick='startEditJsonVal(this)' title='双击修改数值'>${val}</span><span class='type-tag type-num'>number</span></div>${makeJsonCopyBtn(String(val))}</div>`;
            }
            if (typeof val === 'string') {
                return `<div class='json-row'><div class='json-main'>${JSON_SPACER_HTML}${keyPrefix}<span class='json-str' data-path='${pathStr}' ondblclick='startEditJsonVal(this)' title='双击修改文本'>""${escapeHtml(val)}""</span><span class='type-tag type-str'>string</span></div>${makeJsonCopyBtn(val)}</div>`;
            }
            if (Array.isArray(val)) {
                if (val.length === 0) {
                    return `<div class='json-row'><div class='json-main'>${JSON_SPACER_HTML}${keyPrefix}<span class='json-bracket'>[ ]</span><span class='type-tag type-arr'>array[0]</span></div>${makeJsonCopyBtn('[]')}</div>`;
                }
                let items = val.map((item, idx) => buildJsonTreeHtml(item, idx, true, path.concat([idx]))).join('');
                return `<details open><summary><div class='json-main'>${JSON_CHEVRON_SVG}${keyPrefix}<span class='json-bracket'>[ ]</span><span class='json-summary-tag'>${val.length} 项</span><span class='type-tag type-arr'>array</span></div>${makeJsonCopyBtn(val)}</summary><div class='json-node-children'>${items}</div></details>`;
            }
            if (typeof val === 'object') {
                const keys = Object.keys(val);
                if (keys.length === 0) {
                    return `<div class='json-row'><div class='json-main'>${JSON_SPACER_HTML}${keyPrefix}<span class='json-bracket'>{ }</span><span class='type-tag type-obj'>object{0}</span></div>${makeJsonCopyBtn('{}')}</div>`;
                }
                let items = keys.map(k => buildJsonTreeHtml(val[k], k, false, path.concat([k]))).join('');
                return `<details open><summary><div class='json-main'>${JSON_CHEVRON_SVG}${keyPrefix}<span class='json-bracket'>{ }</span><span class='json-summary-tag'>${keys.length} 属性</span><span class='type-tag type-obj'>object</span></div>${makeJsonCopyBtn(val)}</summary><div class='json-node-children'>${items}</div></details>`;
            }
            return '';
        }

        // ==================== IN-PLACE JSON & YAML EDITING ====================
        function getJsonValByPath(path) {
            let target = window.__PARSED_JSON__;
            for (let i = 0; i < path.length; i++) {
                if (target == null) return null;
                target = target[path[i]];
            }
            return target;
        }

        function setJsonValByPath(path, newVal) {
            if (!path || path.length === 0) {
                window.__PARSED_JSON__ = newVal;
                return;
            }
            let target = window.__PARSED_JSON__;
            for (let i = 0; i < path.length - 1; i++) {
                target = target[path[i]];
            }
            target[path[path.length - 1]] = newVal;
        }

        function syncJsonOrYamlAfterEdit() {
            if (window.__DOC_FORMAT__ === 'yaml') {
                if (window.jsyaml && window.jsyaml.dump) {
                    const dumped = window.jsyaml.dump(window.__PARSED_JSON__, { indent: 2, lineWidth: -1 });
                    window.__RAW_YAML__ = dumped;
                    window.omniTriggerChange(dumped, false);
                }
                initYamlView();
            } else {
                const formatted = JSON.stringify(window.__PARSED_JSON__, null, 2);
                window.__RAW_JSON__ = formatted;
                window.omniTriggerChange(formatted, false);
                initJsonView();
            }
        }

        function initJsonOrYamlView() {
            if (window.__DOC_FORMAT__ === 'yaml') initYamlView();
            else initJsonView();
        }

        window.startEditJsonVal = function(spanEl) {
            if (spanEl.querySelector('input')) return;
            const pathStr = spanEl.getAttribute('data-path');
            if (!pathStr) return;
            const path = JSON.parse(decodeURIComponent(pathStr));
            let currentVal = getJsonValByPath(path);
            const input = document.createElement('input');
            input.type = 'text';
            input.className = 'json-inline-input';
            input.value = currentVal === null ? 'null' : String(currentVal);
            spanEl.textContent = '';
            spanEl.appendChild(input);
            input.focus();
            input.select();

            let committed = false;
            function commit() {
                if (committed) return;
                committed = true;
                const rawVal = input.value.trim();
                let parsedVal = rawVal;
                if (rawVal === 'null') parsedVal = null;
                else if (rawVal === 'true') parsedVal = true;
                else if (rawVal === 'false') parsedVal = false;
                else if (!isNaN(Number(rawVal)) && rawVal !== '') parsedVal = Number(rawVal);

                setJsonValByPath(path, parsedVal);
                syncJsonOrYamlAfterEdit();
            }

            input.addEventListener('keydown', (e) => {
                if (e.key === 'Enter') { e.preventDefault(); commit(); }
                if (e.key === 'Escape') { e.preventDefault(); committed = true; initJsonOrYamlView(); }
            });
            input.addEventListener('blur', commit);
        };

        window.startEditJsonKey = function(spanEl) {
            if (spanEl.querySelector('input')) return;
            const parentPathStr = spanEl.getAttribute('data-parent-path');
            const oldKey = spanEl.getAttribute('data-key');
            if (parentPathStr === null || oldKey === null) return;
            const parentPath = JSON.parse(decodeURIComponent(parentPathStr));

            const input = document.createElement('input');
            input.type = 'text';
            input.className = 'json-inline-input';
            input.value = oldKey;
            spanEl.textContent = '';
            spanEl.appendChild(input);
            input.focus();
            input.select();

            let committed = false;
            function commit() {
                if (committed) return;
                committed = true;
                const newKey = input.value.trim();
                if (newKey && newKey !== oldKey) {
                    let parentObj = getJsonValByPath(parentPath);
                    if (parentObj && typeof parentObj === 'object' && !Array.isArray(parentObj)) {
                        const newObj = {};
                        for (let k in parentObj) {
                            if (k === oldKey) newObj[newKey] = parentObj[k];
                            else newObj[k] = parentObj[k];
                        }
                        if (parentPath.length === 0) window.__PARSED_JSON__ = newObj;
                        else setJsonValByPath(parentPath, newObj);
                        syncJsonOrYamlAfterEdit();
                        return;
                    }
                }
                initJsonOrYamlView();
            }

            input.addEventListener('keydown', (e) => {
                if (e.key === 'Enter') { e.preventDefault(); commit(); }
                if (e.key === 'Escape') { e.preventDefault(); committed = true; initJsonOrYamlView(); }
            });
            input.addEventListener('blur', commit);
        };

        function initJsonView() {
            const root = document.getElementById('json-root');
            if (!root || window.__RAW_JSON__ === undefined) return;
            try {
                window.__PARSED_JSON__ = JSON.parse(window.__RAW_JSON__);
                root.innerHTML = buildJsonTreeHtml(window.__PARSED_JSON__, undefined, false, []);
                updateJsonNodeCount();
            } catch (err) {
                root.innerHTML = `<div class='json-err'><strong>JSON 语法解析失败：</strong><br/>${escapeHtml(err.message)}</div>`;
            }
        }

        window.toggleJsonTypes = function() {
            const root = document.getElementById('json-root') || document.getElementById('yaml-root');
            const btn = document.getElementById('btn-toggle-types');
            if (!root || !btn) return;
            if (root.classList.contains('hide-types')) {
                root.classList.remove('hide-types');
                btn.classList.add('active');
                showToast('已显示数据类型');
            } else {
                root.classList.add('hide-types');
                btn.classList.remove('active');
                showToast('已隐藏数据类型');
            }
        };

        window.expandAllJson = function() {
            const details = document.querySelectorAll('#json-root details, #yaml-root details');
            details.forEach(d => d.open = true);
            showToast('已展开全部分支 (' + details.length + ' 个)');
        };

        window.collapseAllJson = function() {
            const details = document.querySelectorAll('#json-root details, #yaml-root details');
            details.forEach(d => d.open = false);
            const first = document.querySelector('#json-root details, #yaml-root details');
            if (first) first.open = true;
            showToast('已折叠全部分支');
        };

        window.copyFormattedJson = function() {
            if (window.__RAW_JSON__ !== undefined) {
                try {
                    const obj = JSON.parse(window.__RAW_JSON__);
                    copyTextToClipboard(JSON.stringify(obj, null, 2));
                    showToast('已复制格式化 JSON');
                } catch(e) {
                    copyTextToClipboard(window.__RAW_JSON__);
                    showToast('已复制原始 JSON 文本');
                }
            } else {
                showToast('无可用 JSON 数据');
            }
        };

        window.filterJsonTree = function(query) {
            const q = (query || '').toLowerCase().trim();
            const root = document.getElementById('json-root') || document.getElementById('yaml-root');
            if (!root) return;
            if (!q) {
                root.querySelectorAll('.json-row, details').forEach(r => r.style.display = '');
                updateJsonNodeCount();
                return;
            }
            let matches = 0;
            root.querySelectorAll('.json-row').forEach(r => r.style.display = 'none');
            root.querySelectorAll('details').forEach(d => d.style.display = 'none');

            root.querySelectorAll('.json-row, summary').forEach(el => {
                const txt = el.textContent.toLowerCase();
                if (txt.includes(q)) {
                    matches++;
                    if (el.classList.contains('json-row')) {
                        el.style.display = '';
                    }
                    let p = el.parentElement;
                    while (p && p.id !== 'json-root' && p.id !== 'yaml-root') {
                        if (p.tagName.toLowerCase() === 'details') {
                            p.style.display = '';
                            p.open = true;
                        }
                        p = p.parentElement;
                    }
                }
            });
            const stats = document.getElementById('json-stats');
            if (stats) stats.textContent = `匹配 ${matches} 项`;
        };

        function updateJsonNodeCount() {
            const root = document.getElementById('json-root') || document.getElementById('yaml-root');
            const stats = document.getElementById('json-stats');
            if (root && stats) {
                const totalRows = root.querySelectorAll('.json-row').length;
                const totalDetails = root.querySelectorAll('details').length;
                stats.textContent = `共 ${totalRows + totalDetails} 个节点`;
            }
        }

        // ==================== XML INTERACTIVE TREE VIEW LOGIC ====================
        function initXmlView(targetFlashPath) {
            const root = document.getElementById('xml-root');
            if (!root || window.__RAW_XML__ === undefined) return;

            // 记住当前已展开节点的路径
            const openPaths = new Set();
            root.querySelectorAll('details.xml-node[open]').forEach(d => {
                const p = d.getAttribute('data-xml-path');
                if (p) openPaths.add(p);
            });
            const hasPrevState = root.querySelectorAll('details.xml-node').length > 0;

            try {
                const parser = new DOMParser();
                const xmlDoc = parser.parseFromString(window.__RAW_XML__, 'text/xml');
                const parseErr = xmlDoc.getElementsByTagName('parsererror');
                if (parseErr.length > 0) {
                    root.innerHTML = `<div class='json-err'><strong>XML 语法解析失败：</strong><br/>${escapeHtml(parseErr[0].textContent)}</div>`;
                    return;
                }
                window.__XML_DOC__ = xmlDoc;

                let prologHtml = '';
                const rawTrimmed = (window.__RAW_XML__ || '').trim();
                const prologMatch = rawTrimmed.match(/^<\?xml[^>]*\?>/i);
                if (prologMatch) {
                    prologHtml = `<div class='xml-prolog'>${escapeHtml(prologMatch[0])}</div>`;
                }

                const treeHtml = buildXmlNodeHtml(xmlDoc.documentElement, [], '/' + xmlDoc.documentElement.tagName, openPaths, hasPrevState);
                root.innerHTML = prologHtml + treeHtml;
                updateXmlNodeCount();

                if (targetFlashPath) {
                    const targetEl = root.querySelector(`[data-xml-path='${targetFlashPath}']`);
                    if (targetEl) {
                        targetEl.classList.add('xml-just-edited');
                        setTimeout(() => targetEl.classList.remove('xml-just-edited'), 1600);
                    }
                }
            } catch (err) {
                root.innerHTML = `<div class='json-err'><strong>XML 解析异常：</strong><br/>${escapeHtml(err.message)}</div>`;
            }
        }

        function getXmlMeaningfulChildren(node) {
            const list = [];
            if (!node || !node.childNodes) return list;
            for (let i = 0; i < node.childNodes.length; i++) {
                const c = node.childNodes[i];
                if (c.nodeType === Node.ELEMENT_NODE ||
                    c.nodeType === Node.CDATA_SECTION_NODE ||
                    c.nodeType === Node.COMMENT_NODE) {
                    list.push(c);
                } else if (c.nodeType === Node.TEXT_NODE) {
                    if (c.nodeValue && c.nodeValue.trim().length > 0) {
                        list.push(c);
                    }
                }
            }
            return list;
        }

        function getXmlNodeByPath(pathArr) {
            if (!window.__XML_DOC__) return null;
            let curr = window.__XML_DOC__.documentElement;
            if (!curr) return null;
            for (let i = 0; i < pathArr.length; i++) {
                const idx = pathArr[i];
                const children = getXmlMeaningfulChildren(curr);
                if (idx < 0 || idx >= children.length) return null;
                curr = children[idx];
            }
            return curr;
        }

        function buildXmlNodeHtml(node, pathArr, xpathStr, openPaths, hasPrevState) {
            if (!node) return '';
            const pathStr = pathArr.join(',');

            if (node.nodeType === Node.COMMENT_NODE) {
                return `<div class='xml-comment-row' data-xml-path='${pathStr}'>
                    <span class='xml-spacer'></span>
                    <span class='xml-comment' ondblclick='startEditXmlComment(this, event)' title='双击就地修改注释'>&lt;!-- ${escapeHtml(node.nodeValue)} --&gt;</span>
                </div>`;
            }
            if (node.nodeType === Node.CDATA_SECTION_NODE) {
                return `<div class='xml-cdata-row' data-xml-path='${pathStr}'>
                    <span class='xml-spacer'></span>
                    <span class='xml-cdata-badge'>CDATA</span>
                    <span class='xml-cdata-content' ondblclick='startEditXmlCData(this, event)' title='双击就地修改 CDATA 内容'>${escapeHtml(node.nodeValue)}</span>
                </div>`;
            }
            if (node.nodeType === Node.TEXT_NODE) {
                const txt = node.nodeValue.trim();
                if (!txt) return '';
                return `<div class='xml-leaf-row' data-xml-path='${pathStr}'>
                    <span class='xml-spacer'></span>
                    <span class='xml-text-val' ondblclick='startEditXmlText(this, event)' title='双击就地修改文本'>${escapeHtml(txt)}</span>
                </div>`;
            }
            if (node.nodeType !== Node.ELEMENT_NODE) return '';

            const tag = node.tagName;
            const hasAttrs = node.attributes && node.attributes.length > 0;
            let attrHtml = '';
            if (hasAttrs) {
                attrHtml = '<span class=\'xml-attr-list\'>';
                for (let i = 0; i < node.attributes.length; i++) {
                    const a = node.attributes[i];
                    attrHtml += ` <span class='xml-attr' data-attr-name='${escapeHtml(a.name)}'>` +
                        `<span class='xml-attr-name' ondblclick='startEditXmlAttrName(this, event)' title='双击修改属性名'>${escapeHtml(a.name)}</span>` +
                        `<span class='xml-eq'>=</span>` +
                        `<span class='xml-quote'>&quot;</span>` +
                        `<span class='xml-attr-val' ondblclick='startEditXmlAttrVal(this, event)' title='双击修改属性值'>${escapeHtml(a.value)}</span>` +
                        `<span class='xml-quote'>&quot;</span>` +
                        `</span>`;
                }
                attrHtml += '</span>';
            }

            const children = getXmlMeaningfulChildren(node);

            // 自闭合叶子节点 (无任何子节点)
            if (children.length === 0) {
                return `<div class='xml-leaf-row' data-xml-path='${pathStr}'>
                    <span class='xml-spacer'></span>
                    <span class='xml-bracket'>&lt;</span><span class='xml-tag-name' ondblclick='startEditXmlTagName(this, event)' title='双击修改标签名'>${escapeHtml(tag)}</span>${attrHtml}<span class='xml-bracket'> /&gt;</span>
                    <div class='xml-row-tools'>
                        <button class='xml-tool-btn' data-xpath='${escapeHtml(xpathStr)}' onclick='copyXmlXPath(this, event)' title='复制当前节点 XPath 路径'>XPath</button>
                        <button class='xml-tool-btn' data-path='${pathStr}' onclick='copyXmlNodeXml(this, event)' title='复制此节点完整 XML'>复制</button>
                    </div>
                </div>`;
            }

            // 单行纯文本/CDATA 叶子节点 (仅 1 个文本或 CDATA 子项)
            if (children.length === 1 && (children[0].nodeType === Node.TEXT_NODE || children[0].nodeType === Node.CDATA_SECTION_NODE)) {
                const onlyChild = children[0];
                const isCData = onlyChild.nodeType === Node.CDATA_SECTION_NODE;
                const childPathStr = (pathArr.concat([0])).join(',');
                const valHtml = isCData
                    ? `<span class='xml-cdata-badge' style='margin-left:4px;'>CDATA</span><span class='xml-cdata-content' data-xml-path='${childPathStr}' ondblclick='startEditXmlCData(this, event)' title='双击修改 CDATA 内容'>${escapeHtml(onlyChild.nodeValue)}</span>`
                    : `<span class='xml-text-val' data-xml-path='${childPathStr}' ondblclick='startEditXmlText(this, event)' title='双击就地修改文本'>${escapeHtml(onlyChild.nodeValue.trim())}</span>`;

                return `<div class='xml-leaf-row' data-xml-path='${pathStr}'>
                    <span class='xml-spacer'></span>
                    <span class='xml-bracket'>&lt;</span><span class='xml-tag-name' ondblclick='startEditXmlTagName(this, event)' title='双击修改标签名'>${escapeHtml(tag)}</span>${attrHtml}<span class='xml-bracket'>&gt;</span>` +
                    valHtml +
                    `<span class='xml-bracket'>&lt;/</span><span class='xml-tag-name' ondblclick='startEditXmlTagName(this, event)' title='双击修改标签名'>${escapeHtml(tag)}</span><span class='xml-bracket'>&gt;</span>
                    <div class='xml-row-tools'>
                        <button class='xml-tool-btn' data-xpath='${escapeHtml(xpathStr)}' onclick='copyXmlXPath(this, event)' title='复制当前节点 XPath 路径'>XPath</button>
                        <button class='xml-tool-btn' data-path='${pathStr}' onclick='copyXmlNodeXml(this, event)' title='复制此节点完整 XML'>复制</button>
                    </div>
                </div>`;
            }

            // 复合父节点：展开/折叠结构
            const isOpen = hasPrevState ? openPaths.has(pathStr) : true;
            let childrenHtml = '';
            const tagCounts = {};
            for (let i = 0; i < children.length; i++) {
                const ch = children[i];
                let subXpath = xpathStr;
                if (ch.nodeType === Node.ELEMENT_NODE) {
                    const cTag = ch.tagName;
                    tagCounts[cTag] = (tagCounts[cTag] || 0) + 1;
                    subXpath = `${xpathStr}/${cTag}[${tagCounts[cTag]}]`;
                }
                childrenHtml += buildXmlNodeHtml(ch, pathArr.concat([i]), subXpath, openPaths, hasPrevState);
            }

            return `<details class='xml-node' data-xml-path='${pathStr}' ${isOpen ? 'open' : ''}>
                <summary class='xml-summary'>
                    <svg class='xml-chevron' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.2'><polyline points='9 18 15 12 9 6'></polyline></svg>
                    <span class='xml-bracket'>&lt;</span><span class='xml-tag-name' ondblclick='startEditXmlTagName(this, event)' title='双击修改标签名'>${escapeHtml(tag)}</span>${attrHtml}<span class='xml-bracket xml-open-bracket'>&gt;</span>
                    <span class='xml-collapsed-preview'><span class='xml-bracket'>&gt;</span><span class='xml-ellipsis'>...</span><span class='xml-bracket'>&lt;/</span><span class='xml-tag-name'>${escapeHtml(tag)}</span><span class='xml-bracket'>&gt;</span></span>
                    <span class='xml-badge'>${children.length} 个子项</span>
                    <div class='xml-row-tools'>
                        <button class='xml-tool-btn' data-xpath='${escapeHtml(xpathStr)}' onclick='copyXmlXPath(this, event)' title='复制当前节点 XPath 路径'>XPath</button>
                        <button class='xml-tool-btn' data-path='${pathStr}' onclick='copyXmlNodeXml(this, event)' title='复制此节点完整 XML'>复制</button>
                    </div>
                </summary>
                <div class='xml-node-children'>
                    ${childrenHtml}
                </div>
                <div class='xml-close-tag-row'>
                    <span class='xml-spacer'></span>
                    <span class='xml-bracket'>&lt;/</span><span class='xml-tag-name' ondblclick='startEditXmlTagName(this, event)' title='双击修改标签名'>${escapeHtml(tag)}</span><span class='xml-bracket'>&gt;</span>
                </div>
            </details>`;
        }

        // ==================== 就地双击编辑引擎 (In-Place XML Editors) ====================
        window.startEditXmlAttrVal = function(spanEl, e) {
            if (e) { e.preventDefault(); e.stopPropagation(); }
            if (spanEl.querySelector('input')) return;
            const rowEl = spanEl.closest('[data-xml-path]');
            const attrEl = spanEl.closest('.xml-attr');
            if (!rowEl || !attrEl) return;
            const pathArr = rowEl.getAttribute('data-xml-path').split(',').map(Number);
            const attrName = attrEl.getAttribute('data-attr-name');
            const oldVal = spanEl.textContent;

            const input = document.createElement('input');
            input.type = 'text';
            input.className = 'xml-inline-input';
            input.value = oldVal;
            spanEl.textContent = '';
            spanEl.appendChild(input);
            input.focus();
            input.select();

            let committed = false;
            function commit() {
                if (committed) return;
                committed = true;
                const newVal = input.value;
                if (newVal !== oldVal) {
                    const targetNode = getXmlNodeByPath(pathArr);
                    if (targetNode && targetNode.setAttribute) {
                        targetNode.setAttribute(attrName, newVal);
                        syncXmlAfterEdit(rowEl.getAttribute('data-xml-path'));
                        showToast(`已更新属性 [${attrName}]`);
                        return;
                    }
                }
                spanEl.textContent = oldVal;
            }

            input.addEventListener('keydown', (evt) => {
                if (evt.key === 'Enter') { evt.preventDefault(); commit(); }
                if (evt.key === 'Escape') { evt.preventDefault(); committed = true; spanEl.textContent = oldVal; }
            });
            input.addEventListener('blur', commit);
        };

        window.startEditXmlAttrName = function(spanEl, e) {
            if (e) { e.preventDefault(); e.stopPropagation(); }
            if (spanEl.querySelector('input')) return;
            const rowEl = spanEl.closest('[data-xml-path]');
            const attrEl = spanEl.closest('.xml-attr');
            if (!rowEl || !attrEl) return;
            const pathArr = rowEl.getAttribute('data-xml-path').split(',').map(Number);
            const oldAttrName = attrEl.getAttribute('data-attr-name');

            const input = document.createElement('input');
            input.type = 'text';
            input.className = 'xml-inline-input';
            input.value = oldAttrName;
            spanEl.textContent = '';
            spanEl.appendChild(input);
            input.focus();
            input.select();

            let committed = false;
            function commit() {
                if (committed) return;
                committed = true;
                const newAttrName = input.value.trim();
                if (newAttrName && newAttrName !== oldAttrName) {
                    const targetNode = getXmlNodeByPath(pathArr);
                    if (targetNode && targetNode.hasAttribute && targetNode.hasAttribute(oldAttrName)) {
                        const val = targetNode.getAttribute(oldAttrName);
                        targetNode.removeAttribute(oldAttrName);
                        targetNode.setAttribute(newAttrName, val);
                        syncXmlAfterEdit(rowEl.getAttribute('data-xml-path'));
                        showToast(`已将属性名重命名为 [${newAttrName}]`);
                        return;
                    }
                }
                spanEl.textContent = oldAttrName;
            }

            input.addEventListener('keydown', (evt) => {
                if (evt.key === 'Enter') { evt.preventDefault(); commit(); }
                if (evt.key === 'Escape') { evt.preventDefault(); committed = true; spanEl.textContent = oldAttrName; }
            });
            input.addEventListener('blur', commit);
        };

        window.startEditXmlText = function(spanEl, e) {
            if (e) { e.preventDefault(); e.stopPropagation(); }
            if (spanEl.querySelector('input')) return;
            const pathAttr = spanEl.getAttribute('data-xml-path') || spanEl.closest('[data-xml-path]')?.getAttribute('data-xml-path');
            if (!pathAttr) return;
            const pathArr = pathAttr.split(',').map(Number);
            const oldText = spanEl.textContent;

            const input = document.createElement('input');
            input.type = 'text';
            input.className = 'xml-inline-input';
            input.value = oldText;
            spanEl.textContent = '';
            spanEl.appendChild(input);
            input.focus();
            input.select();

            let committed = false;
            function commit() {
                if (committed) return;
                committed = true;
                const newText = input.value;
                if (newText !== oldText) {
                    const targetNode = getXmlNodeByPath(pathArr);
                    if (targetNode) {
                        if (targetNode.nodeType === Node.TEXT_NODE) {
                            targetNode.nodeValue = newText;
                        } else if (targetNode.nodeType === Node.ELEMENT_NODE) {
                            targetNode.textContent = newText;
                        }
                        syncXmlAfterEdit(pathAttr);
                        showToast('已更新节点文本内容');
                        return;
                    }
                }
                spanEl.textContent = oldText;
            }

            input.addEventListener('keydown', (evt) => {
                if (evt.key === 'Enter') { evt.preventDefault(); commit(); }
                if (evt.key === 'Escape') { evt.preventDefault(); committed = true; spanEl.textContent = oldText; }
            });
            input.addEventListener('blur', commit);
        };

        window.startEditXmlCData = function(spanEl, e) {
            if (e) { e.preventDefault(); e.stopPropagation(); }
            if (spanEl.querySelector('input')) return;
            const pathAttr = spanEl.getAttribute('data-xml-path') || spanEl.closest('[data-xml-path]')?.getAttribute('data-xml-path');
            if (!pathAttr) return;
            const pathArr = pathAttr.split(',').map(Number);
            const oldText = spanEl.textContent;

            const input = document.createElement('input');
            input.type = 'text';
            input.className = 'xml-inline-input';
            input.value = oldText;
            spanEl.textContent = '';
            spanEl.appendChild(input);
            input.focus();
            input.select();

            let committed = false;
            function commit() {
                if (committed) return;
                committed = true;
                const newText = input.value;
                if (newText !== oldText) {
                    const targetNode = getXmlNodeByPath(pathArr);
                    if (targetNode && (targetNode.nodeType === Node.CDATA_SECTION_NODE || targetNode.data !== undefined)) {
                        targetNode.data = newText;
                        syncXmlAfterEdit(pathAttr);
                        showToast('已更新 CDATA 区块内容');
                        return;
                    }
                }
                spanEl.textContent = oldText;
            }

            input.addEventListener('keydown', (evt) => {
                if (evt.key === 'Enter') { evt.preventDefault(); commit(); }
                if (evt.key === 'Escape') { evt.preventDefault(); committed = true; spanEl.textContent = oldText; }
            });
            input.addEventListener('blur', commit);
        };

        window.startEditXmlComment = function(spanEl, e) {
            if (e) { e.preventDefault(); e.stopPropagation(); }
            if (spanEl.querySelector('input')) return;
            const rowEl = spanEl.closest('[data-xml-path]');
            if (!rowEl) return;
            const pathArr = rowEl.getAttribute('data-xml-path').split(',').map(Number);
            const rawComment = spanEl.textContent.replace(/^<!--\s*/, '').replace(/\s*-->$/, '');

            const input = document.createElement('input');
            input.type = 'text';
            input.className = 'xml-inline-input';
            input.value = rawComment;
            spanEl.textContent = '';
            spanEl.appendChild(input);
            input.focus();
            input.select();

            let committed = false;
            function commit() {
                if (committed) return;
                committed = true;
                const newText = input.value;
                if (newText !== rawComment) {
                    const targetNode = getXmlNodeByPath(pathArr);
                    if (targetNode && targetNode.nodeType === Node.COMMENT_NODE) {
                        targetNode.nodeValue = newText;
                        syncXmlAfterEdit(rowEl.getAttribute('data-xml-path'));
                        showToast('已更新 XML 注释');
                        return;
                    }
                }
                spanEl.textContent = `<!-- ${rawComment} -->`;
            }

            input.addEventListener('keydown', (evt) => {
                if (evt.key === 'Enter') { evt.preventDefault(); commit(); }
                if (evt.key === 'Escape') { evt.preventDefault(); committed = true; spanEl.textContent = `<!-- ${rawComment} -->`; }
            });
            input.addEventListener('blur', commit);
        };

        window.startEditXmlTagName = function(spanEl, e) {
            if (e) { e.preventDefault(); e.stopPropagation(); }
            if (spanEl.querySelector('input')) return;
            const rowEl = spanEl.closest('[data-xml-path]');
            if (!rowEl) return;
            const pathArr = rowEl.getAttribute('data-xml-path').split(',').map(Number);
            const oldTag = spanEl.textContent;

            const input = document.createElement('input');
            input.type = 'text';
            input.className = 'xml-inline-input';
            input.value = oldTag;
            spanEl.textContent = '';
            spanEl.appendChild(input);
            input.focus();
            input.select();

            let committed = false;
            function commit() {
                if (committed) return;
                committed = true;
                const newTag = input.value.trim();
                if (newTag && newTag !== oldTag && /^[a-zA-Z_:][a-zA-Z0-9.\-_:]*$/.test(newTag)) {
                    const targetNode = getXmlNodeByPath(pathArr);
                    if (targetNode && targetNode.nodeType === Node.ELEMENT_NODE) {
                        const newElement = window.__XML_DOC__.createElement(newTag);
                        // 复制属性
                        for (let i = 0; i < targetNode.attributes.length; i++) {
                            const attr = targetNode.attributes[i];
                            newElement.setAttribute(attr.name, attr.value);
                        }
                        // 移动子节点
                        while (targetNode.firstChild) {
                            newElement.appendChild(targetNode.firstChild);
                        }
                        if (targetNode.parentNode) {
                            targetNode.parentNode.replaceChild(newElement, targetNode);
                        }
                        syncXmlAfterEdit(rowEl.getAttribute('data-xml-path'));
                        showToast(`已将标签重命名为 <${newTag}>`);
                        return;
                    }
                }
                spanEl.textContent = oldTag;
            }

            input.addEventListener('keydown', (evt) => {
                if (evt.key === 'Enter') { evt.preventDefault(); commit(); }
                if (evt.key === 'Escape') { evt.preventDefault(); committed = true; spanEl.textContent = oldTag; }
            });
            input.addEventListener('blur', commit);
        };

        function syncXmlAfterEdit(targetFlashPath) {
            if (!window.__XML_DOC__) return;
            const serializer = new XMLSerializer();
            const rawXml = serializer.serializeToString(window.__XML_DOC__);
            const beautified = formatXmlString(rawXml);
            window.__RAW_XML__ = beautified;
            window.__RAW_DOC_CONTENT__ = beautified;
            window.omniTriggerChange(beautified, true);
            initXmlView(targetFlashPath);
        }

        window.formatXmlString = function(xml) {
            if (!xml) return '';
            let formatted = '';
            let pad = 0;
            const tab = '  ';
            xml = xml.replace(/(>)(<)(\/*)/g, '$1\r\n$2$3');
            const lines = xml.split(/\r?\n/);
            for (let i = 0; i < lines.length; i++) {
                let line = lines[i].trim();
                if (!line) continue;
                if (line.match(/^<\/\w/)) {
                    pad = Math.max(0, pad - 1);
                }
                formatted += tab.repeat(pad) + line + '\r\n';
                if (line.match(/^<[a-zA-Z0-9_\-:]+[^>]*[^\/]>.*$/) && !line.includes('</') && !line.startsWith('<?') && !line.startsWith('<!')) {
                    pad++;
                }
            }
            return formatted.trim();
        };

        window.copyXmlXPath = function(btnOrXpath, e) {
            if (e) { e.preventDefault(); e.stopPropagation(); }
            const xpath = typeof btnOrXpath === 'string' ? btnOrXpath : (btnOrXpath.getAttribute('data-xpath') || '');
            if (xpath) {
                copyTextToClipboard(xpath);
                showToast('已复制 XPath: ' + xpath);
            }
        };

        window.copyXmlNodeXml = function(btnOrPath, e) {
            if (e) { e.preventDefault(); e.stopPropagation(); }
            const pathStr = typeof btnOrPath === 'string' ? btnOrPath : (btnOrPath.getAttribute('data-path') || '');
            if (!pathStr) return;
            const pathArr = pathStr.split(',').map(Number);
            const targetNode = getXmlNodeByPath(pathArr);
            if (targetNode) {
                const serializer = new XMLSerializer();
                const nodeXml = formatXmlString(serializer.serializeToString(targetNode));
                copyTextToClipboard(nodeXml);
                showToast('已复制该节点 XML 源码');
            }
        };

        window.toggleXmlAttributes = function() {
            const root = document.getElementById('xml-root');
            const btn = document.getElementById('btn-toggle-types');
            if (!root) return;
            if (root.classList.contains('badge-attrs')) {
                root.classList.remove('badge-attrs');
                root.classList.add('hide-attrs');
                if (btn) {
                    btn.classList.remove('active');
                    btn.innerHTML = `${ICON_TAG_SVG}<span>隐藏属性</span>`;
                }
                showToast('已隐藏属性列表');
            } else if (root.classList.contains('hide-attrs')) {
                root.classList.remove('hide-attrs');
                if (btn) {
                    btn.classList.remove('active');
                    btn.innerHTML = `${ICON_TAG_SVG}<span>内联属性</span>`;
                }
                showToast('已切换至标准内联属性');
            } else {
                root.classList.add('badge-attrs');
                if (btn) {
                    btn.classList.add('active');
                    btn.innerHTML = `${ICON_TAG_SVG}<span>属性徽章</span>`;
                }
                showToast('已开启属性独立徽章视图');
            }
        };

        window.expandAllXml = function() {
            const root = document.getElementById('xml-root');
            if (root) {
                root.querySelectorAll('details.xml-node').forEach(d => d.open = true);
                showToast('已全部展开 XML 节点');
            }
        };

        window.collapseAllXml = function() {
            const root = document.getElementById('xml-root');
            if (root) {
                root.querySelectorAll('details.xml-node').forEach(d => d.open = false);
                showToast('已全部折叠 XML 节点');
            }
        };

        window.copyFormattedXml = function() {
            if (window.__RAW_XML__ !== undefined) {
                const formatted = formatXmlString(window.__RAW_XML__);
                copyTextToClipboard(formatted);
                showToast('已复制格式化后的 XML 文本');
            }
        };

        window.filterXmlTree = function(query) {
            const q = (query || '').toLowerCase().trim();
            const root = document.getElementById('xml-root');
            if (!root) return;
            if (!q) {
                root.querySelectorAll('.xml-node, .xml-leaf-row, .xml-cdata-row, .xml-comment-row').forEach(r => r.style.display = '');
                updateXmlNodeCount();
                return;
            }
            let matches = 0;
            root.querySelectorAll('.xml-leaf-row, .xml-cdata-row, .xml-comment-row').forEach(r => r.style.display = 'none');
            root.querySelectorAll('details.xml-node').forEach(d => d.style.display = 'none');

            root.querySelectorAll('.xml-summary, .xml-leaf-row, .xml-cdata-row, .xml-comment-row').forEach(el => {
                const txt = el.textContent.toLowerCase();
                if (txt.includes(q)) {
                    matches++;
                    el.style.display = '';
                    let p = el.parentElement;
                    while (p && p.id !== 'xml-root') {
                        if (p.tagName.toLowerCase() === 'details') {
                            p.style.display = '';
                            p.open = true;
                        }
                        p = p.parentElement;
                    }
                }
            });
            const stats = document.getElementById('xml-stats');
            if (stats) stats.textContent = `匹配 ${matches} 项`;
        };

        function updateXmlNodeCount() {
            const root = document.getElementById('xml-root');
            const stats = document.getElementById('xml-stats');
            if (root && stats) {
                const totalNodes = root.querySelectorAll('.xml-tag-name').length;
                stats.textContent = `共 ${Math.ceil(totalNodes / 2)} 个节点`;
            }
        }

        // ==================== CODE OVERVIEW TOGGLE LOGIC ====================
        window.toggleCodeMirrorDisplay = function() {
            const root = document.getElementById('code-viewer-root');
            const btn = document.getElementById('btn-toggle-code-mirror');
            if (!root) return;
            const isHidden = root.style.display === 'none';
            if (isHidden) {
                root.style.display = 'block';
                if (btn) btn.innerHTML = `<span>收起代码预览</span>`;
            } else {
                root.style.display = 'none';
                if (btn) btn.innerHTML = `<span>展开代码着色预览</span>`;
            }
        };

        // ==================== CSV VIEW LOGIC (支持就地单元格编辑与同步) ====================
        let csvSortCol = -1, csvSortAsc = true;
        let csvEdited = false;

        function renderCsvGrid(filterText = '') {
            const root = document.getElementById('csv-root');
            const rows = window.__CSV_ROWS__;
            if (!root || !rows || rows.length === 0) return;
            const headers = rows[0];
            let bodyRows = rows.slice(1);
            if (filterText) {
                const q = filterText.toLowerCase();
                bodyRows = bodyRows.filter(r => r.some(c => (c || '').toLowerCase().includes(q)));
            }
            if (csvSortCol >= 0) {
                bodyRows.sort((a, b) => {
                    const va = (a[csvSortCol] || '').trim(), vb = (b[csvSortCol] || '').trim();
                    const na = parseFloat(va), nb = parseFloat(vb);
                    if (!isNaN(na) && !isNaN(nb)) return csvSortAsc ? na - nb : nb - na;
                    return csvSortAsc ? va.localeCompare(vb) : vb.localeCompare(va);
                });
            }
            const statsEl = document.getElementById('csv-stats');
            const totalRowsCount = rows.length > 0 ? rows.length - 1 : 0;
            const colCount = headers.length;
            if (statsEl) {
                if (filterText && filterText.trim().length > 0) {
                    statsEl.innerHTML = `<span class='stats-filtered'>筛选出 ${bodyRows.length} / ${totalRowsCount} 行</span> · ${colCount} 列`;
                } else {
                    statsEl.innerHTML = `共 ${totalRowsCount} 行 · ${colCount} 列`;
                }
            }
            const clearBtn = document.getElementById('csv-clear-search');
            if (clearBtn) {
                clearBtn.style.display = filterText && filterText.length > 0 ? 'inline-flex' : 'none';
            }
            let html = '<table><thead><tr><th class=""csv-num-col"">#</th>';
            headers.forEach((h, idx) => {
                const arrow = csvSortCol === idx ? (csvSortAsc ? ' ▲' : ' ▼') : '';
                html += '<th onclick=""sortCsvBy(' + idx + ')"">' + escapeHtml(h || ('Col ' + idx)) + arrow + '</th>';
            });
            html += '</tr></thead><tbody>';
            const isEditing = window.__isEditing === true;
            const CSV_DEL_SVG = `<svg width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><line x1='18' y1='6' x2='6' y2='18'/><line x1='6' y1='6' x2='18' y2='18'/></svg>`;
            bodyRows.forEach((r, rIdx) => {
                const realRowIdx = rIdx + 1;
                const delBtn = isEditing ? `<button class='csv-row-del-btn' onclick='deleteCsvRow(${realRowIdx})' title='删除第 ${realRowIdx} 行'>${CSV_DEL_SVG}</button>` : '';
                html += `<tr><td class='csv-num-col'><div class='csv-row-num-wrap'><span class='csv-row-idx'>${realRowIdx}</span>${delBtn}</div></td>`;
                headers.forEach((_, cIdx) => {
                    const cellVal = r[cIdx] || '';
                    const isNum = !isNaN(parseFloat(cellVal)) && isFinite(cellVal);
                    const align = isNum ? 'text-align:right;' : '';
                    const editAttr = isEditing ? 'contenteditable=""true""' : 'contenteditable=""false""';
                    html += '<td ' + editAttr + ' style=""' + align + '"" ondblclick=""startEditCsvCell(this, ' + realRowIdx + ', ' + cIdx + ', event)"" onblur=""onCsvCellEdited(' + realRowIdx + ', ' + cIdx + ', this.textContent)"" onkeydown=""handleCsvCellKey(event, ' + realRowIdx + ', ' + cIdx + ')"">' + escapeHtml(cellVal) + '</td>';
                });
                html += '</tr>';
            });
            html += '</tbody></table>';
            root.innerHTML = html;
        }

        window.startEditCsvCell = function(tdEl, rIdx, cIdx, evt) {
            if (evt) {
                evt.preventDefault();
                evt.stopPropagation();
            }
            if (!tdEl || tdEl.classList.contains('csv-cell-in-editing')) return;
            if (window.__isEditing) {
                tdEl.focus();
                return;
            }

            const oldText = tdEl.textContent;
            tdEl.setAttribute('data-original-val', oldText);
            tdEl.contentEditable = 'true';
            tdEl.classList.add('csv-cell-in-editing');
            tdEl.focus();

            try {
                const range = document.createRange();
                range.selectNodeContents(tdEl);
                const sel = window.getSelection();
                sel.removeAllRanges();
                sel.addRange(range);
            } catch (err) {}

            let committed = false;
            function finish(save) {
                if (committed) return;
                committed = true;
                tdEl.contentEditable = 'false';
                tdEl.classList.remove('csv-cell-in-editing');
                tdEl.removeEventListener('blur', onBlur);
                tdEl.removeEventListener('keydown', onKeyDown);

                if (!save) {
                    tdEl.textContent = oldText;
                    return;
                }
                const newText = tdEl.textContent;
                if (newText !== oldText) {
                    if (window.__CSV_ROWS__ && window.__CSV_ROWS__[rIdx]) {
                        window.__CSV_ROWS__[rIdx][cIdx] = newText;
                        const newCsv = window.serializeCsvRows();
                        window.__RAW_DOC_CONTENT__ = newCsv;
                        window.omniTriggerChange(newCsv, false);
                        showToast('已更新第 ' + rIdx + ' 行第 ' + (cIdx + 1) + ' 列数据');
                    }
                }
            }

            function onBlur() {
                finish(true);
            }

            function onKeyDown(e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    finish(true);
                    const nextRowCell = document.querySelector('tbody tr:nth-child(' + (rIdx + 1) + ') td:nth-child(' + (cIdx + 2) + ')');
                    if (nextRowCell) {
                        setTimeout(() => window.startEditCsvCell(nextRowCell, rIdx + 1, cIdx), 30);
                    }
                } else if (e.key === 'Tab') {
                    e.preventDefault();
                    finish(true);
                    const nextCell = document.querySelector('tbody tr:nth-child(' + rIdx + ') td:nth-child(' + (cIdx + 3) + ')');
                    if (nextCell) {
                        setTimeout(() => window.startEditCsvCell(nextCell, rIdx, cIdx + 1), 30);
                    } else {
                        const nextRowFirstCell = document.querySelector('tbody tr:nth-child(' + (rIdx + 1) + ') td:nth-child(2)');
                        if (nextRowFirstCell) {
                            setTimeout(() => window.startEditCsvCell(nextRowFirstCell, rIdx + 1, 0), 30);
                        }
                    }
                } else if (e.key === 'Escape') {
                    e.preventDefault();
                    finish(false);
                }
            }

            tdEl.addEventListener('blur', onBlur);
            tdEl.addEventListener('keydown', onKeyDown);
        };

        window.handleCsvCellKey = function(e, rIdx, cIdx) {
            if (e.key === 'Enter') {
                e.preventDefault();
                e.target.blur();
                const nextRowCell = document.querySelector(`tbody tr:nth-child(${rIdx + 1}) td:nth-child(${cIdx + 2})`);
                if (nextRowCell) nextRowCell.focus();
            } else if (e.key === 'Tab') {
                const nextCell = document.querySelector(`tbody tr:nth-child(${rIdx}) td:nth-child(${cIdx + 3})`);
                if (nextCell) {
                    e.preventDefault();
                    e.target.blur();
                    nextCell.focus();
                } else {
                    const nextRowFirstCell = document.querySelector(`tbody tr:nth-child(${rIdx + 1}) td:nth-child(2)`);
                    if (nextRowFirstCell) {
                        e.preventDefault();
                        e.target.blur();
                        nextRowFirstCell.focus();
                    }
                }
            }
        };

        window.serializeCsvRows = function() {
            const rows = window.__CSV_ROWS__;
            const delim = window.__CSV_DELIM__ || ',';
            if (!rows) return '';
            const q = String.fromCharCode(34);
            const qq = q + q;
            return rows.map(r => r.map(c => {
                const s = String(c == null ? '' : c);
                if (s.indexOf(delim) >= 0 || s.indexOf(q) >= 0 || s.indexOf('\n') >= 0 || s.indexOf('\r') >= 0) {
                    return q + s.split(q).join(qq) + q;
                }
                return s;
            }).join(delim)).join('\r\n');
        };

        window.onCsvCellEdited = function(rowIdx, colIdx, newText) {
            const rows = window.__CSV_ROWS__;
            if (rows && rows[rowIdx]) {
                const oldText = rows[rowIdx][colIdx] || '';
                if (oldText !== newText) {
                    rows[rowIdx][colIdx] = newText;
                }
            }
        };

        window.addCsvRow = function() {
            const rows = window.__CSV_ROWS__;
            if (!rows || rows.length === 0) return;
            const colCount = rows[0].length;
            const newRow = new Array(colCount).fill('');
            rows.push(newRow);
            renderCsvGrid();
            showToast('已在表格末尾新增一行 (点击保存写回)');
        };

        window.deleteCsvRow = function(rowIdx) {
            const rows = window.__CSV_ROWS__;
            if (!rows || rows.length <= 1) {
                showToast('无法删除最后一行');
                return;
            }
            rows.splice(rowIdx, 1);
            renderCsvGrid();
            showToast('已删除第 ' + rowIdx + ' 行 (点击保存写回)');
        };

        function parseCsvFallback(text) {
            const delim = window.__CSV_DELIM__ || ',';
            const lines = (text || '').split(/\r?\n/);
            return lines.filter(l => l.length > 0).map(line => {
                let row = [];
                let inQuotes = false;
                let cur = '';
                for (let i = 0; i < line.length; i++) {
                    const ch = line[i];
                    if (ch === '\x22') {
                        if (inQuotes && line[i+1] === '\x22') { cur += '\x22'; i++; }
                        else inQuotes = !inQuotes;
                    } else if (ch === delim && !inQuotes) {
                        row.push(cur);
                        cur = '';
                    } else {
                        cur += ch;
                    }
                }
                row.push(cur);
                return row;
            });
        }

        window.syncCsvToEditor = function() {
            window.omniFlushSync();
        };

        window.sortCsvBy = function(colIdx) {
            if (csvSortCol === colIdx) csvSortAsc = !csvSortAsc;
            else { csvSortCol = colIdx; csvSortAsc = true; }
            const filterEl = document.getElementById('csv-filter');
            renderCsvGrid(filterEl ? filterEl.value : '');
        };

        window.filterCsvTable = function(val) { renderCsvGrid(val); };

        window.clearCsvFilter = function() {
            const el = document.getElementById('csv-filter');
            if (el) {
                el.value = '';
                renderCsvGrid('');
                el.focus();
            }
        };

        window.handleCsvFilterKey = function(e) {
            if (e.key === 'Escape') {
                e.preventDefault();
                window.clearCsvFilter();
            }
        };

        window.copyCsvAsTable = function() {
            const rows = window.__CSV_ROWS__;
            if (!rows) return;
            const text = rows.map(r => r.join('\t')).join('\n');
            copyTextToClipboard(text);
            showToast('已复制表格数据 (可直接粘贴至 Excel)');
        };

        // CSV 分隔符更换模态框逻辑
        let currentSelectedDelim = window.__CSV_DELIM__ || ',';

        function getDelimDisplayName(d) {
            if (d === '\t') return '制表符 ( \\t )';
            if (d === ';') return '分号 ( ; )';
            if (d === '|') return '竖线 ( | )';
            if (d === ',') return '逗号 ( , )';
            return `'${d}' 分隔符`;
        }

        window.openCsvDelimModal = function() {
            const modal = document.getElementById('csv-delim-modal');
            if (!modal) return;
            const curDelim = window.__CSV_DELIM__ || ',';
            currentSelectedDelim = curDelim;

            const badge = document.getElementById('delim-current-badge');
            if (badge) badge.textContent = getDelimDisplayName(curDelim);

            const customInput = document.getElementById('csv-custom-delim-input');
            const customPreview = document.getElementById('delim-custom-char-preview');
            const customCard = document.getElementById('delim-btn-custom');

            const presets = {
                ',': document.getElementById('delim-card-comma'),
                '\t': document.getElementById('delim-card-tab'),
                ';': document.getElementById('delim-card-semi'),
                '|': document.getElementById('delim-card-pipe')
            };

            document.querySelectorAll('.delim-card').forEach(c => c.classList.remove('active'));

            if (presets[curDelim]) {
                presets[curDelim].classList.add('active');
                if (customInput) customInput.value = '';
                if (customPreview) customPreview.textContent = '?';
            } else {
                if (customCard) customCard.classList.add('active');
                if (customInput) customInput.value = curDelim;
                if (customPreview) customPreview.textContent = curDelim;
            }

            modal.style.display = 'flex';
        };

        window.closeCsvDelimModal = function() {
            const modal = document.getElementById('csv-delim-modal');
            if (modal) modal.style.display = 'none';
        };

        window.selectDelimPreset = function(delim) {
            document.querySelectorAll('.delim-card').forEach(b => b.classList.remove('active'));
            let cardId = 'delim-card-comma';
            if (delim === '\t') cardId = 'delim-card-tab';
            else if (delim === ';') cardId = 'delim-card-semi';
            else if (delim === '|') cardId = 'delim-card-pipe';

            const card = document.getElementById(cardId);
            if (card) card.classList.add('active');

            const customInput = document.getElementById('csv-custom-delim-input');
            const customPreview = document.getElementById('delim-custom-char-preview');
            if (customInput) customInput.value = '';
            if (customPreview) customPreview.textContent = '?';

            currentSelectedDelim = delim;
        };

        window.selectCustomDelimCard = function() {
            document.querySelectorAll('.delim-card').forEach(b => b.classList.remove('active'));
            const customCard = document.getElementById('delim-btn-custom');
            if (customCard) customCard.classList.add('active');

            const customInput = document.getElementById('csv-custom-delim-input');
            if (customInput && customInput.value) {
                currentSelectedDelim = customInput.value.charAt(0);
            }
        };

        window.onCustomDelimInput = function(val) {
            selectCustomDelimCard();
            const preview = document.getElementById('delim-custom-char-preview');
            if (val && val.length > 0) {
                const ch = val.charAt(0);
                currentSelectedDelim = ch;
                if (preview) preview.textContent = ch;
            } else {
                currentSelectedDelim = '';
                if (preview) preview.textContent = '?';
            }
        };

        window.applyCsvDelimChange = function() {
            const cur = window.__CSV_DELIM__ || ',';
            const customCard = document.getElementById('delim-btn-custom');
            const isCustomActive = customCard && customCard.classList.contains('active');

            if (isCustomActive) {
                const customInput = document.getElementById('csv-custom-delim-input');
                const val = customInput ? customInput.value.trim() : '';
                if (!val) {
                    showToast('请输入有效的自定义分隔字符');
                    if (customInput) customInput.focus();
                    return;
                }
                currentSelectedDelim = val.charAt(0);
            }

            if (!currentSelectedDelim) currentSelectedDelim = ',';
            if (currentSelectedDelim === cur) {
                showToast('当前已是该分隔符');
                closeCsvDelimModal();
                return;
            }
            const target = currentSelectedDelim === '\t' ? '\\t' : currentSelectedDelim;
            window.__CSV_DELIM__ = currentSelectedDelim;
            postAct('CSV_REPLACE_DELIM:' + target);
            closeCsvDelimModal();
            showToast('已替换分隔符并同步更新');
        };

        // ==================== MULTI-LANGUAGE CODE BLOCK ENHANCEMENT ====================
        const LANG_ICONS = {
            terminal: `<svg width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5'><polyline points='4 17 10 11 4 5'></polyline><line x1='12' y1='19' x2='20' y2='19'></line></svg>`,
            code:     `<svg width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5'><polyline points='16 18 22 12 16 6'></polyline><polyline points='8 6 2 12 8 18'></polyline></svg>`,
            database: `<svg width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><ellipse cx='12' cy='5' rx='9' ry='3'></ellipse><path d='M21 12c0 1.66-4 3-9 3s-9-1.34-9-3'></path><path d='M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5'></path></svg>`,
            box:      `<svg width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><path d='M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z'></path><polyline points='3.27 6.96 12 12.01 20.73 6.96'></polyline><line x1='12' y1='22.08' x2='12' y2='12'></line></svg>`,
            git:      `<svg width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><line x1='6' y1='3' x2='6' y2='15'></line><circle cx='18' cy='6' r='3'></circle><circle cx='6' cy='18' r='3'></circle><path d='M18 9a9 9 0 0 1-9 9'></path></svg>`,
            file:     `<svg width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><path d='M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z'></path><polyline points='13 2 13 9 20 9'></polyline></svg>`,
            gear:     `<svg width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><circle cx='12' cy='12' r='3'></circle><path d='M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z'></path></svg>`
        };

        const LANG_THEMES = {
            python:     { name: 'Python',     iconType: 'code',     color: '#3776ab', bgLight: '#eef6fc', bgDark: '#172738' },
            javascript: { name: 'JavaScript', iconType: 'code',     color: '#eab308', bgLight: '#fefce8', bgDark: '#332704' },
            typescript: { name: 'TypeScript', iconType: 'code',     color: '#3178c6', bgLight: '#eff6ff', bgDark: '#142742' },
            csharp:     { name: 'C#',         iconType: 'code',     color: '#9333ea', bgLight: '#faf5ff', bgDark: '#29143d' },
            cpp:        { name: 'C++',        iconType: 'gear',     color: '#0284c7', bgLight: '#f0f9ff', bgDark: '#112a3d' },
            c:          { name: 'C',          iconType: 'gear',     color: '#0284c7', bgLight: '#f0f9ff', bgDark: '#112a3d' },
            java:       { name: 'Java',       iconType: 'code',     color: '#ea580c', bgLight: '#fff7ed', bgDark: '#381c10' },
            rust:       { name: 'Rust',       iconType: 'gear',     color: '#e11d48', bgLight: '#fff1f2', bgDark: '#3b1219' },
            go:         { name: 'Go',         iconType: 'code',     color: '#00add8', bgLight: '#ecfeff', bgDark: '#0e2b34' },
            swift:      { name: 'Swift',      iconType: 'code',     color: '#f97316', bgLight: '#fff7ed', bgDark: '#381c10' },
            kotlin:     { name: 'Kotlin',     iconType: 'code',     color: '#8b5cf6', bgLight: '#f5f3ff', bgDark: '#261947' },
            php:        { name: 'PHP',        iconType: 'code',     color: '#777bb4', bgLight: '#f3f4f9', bgDark: '#212234' },
            ruby:       { name: 'Ruby',       iconType: 'code',     color: '#cc342d', bgLight: '#fef2f2', bgDark: '#351616' },

            html:       { name: 'HTML',       iconType: 'code',     color: '#e34f26', bgLight: '#fff1ee', bgDark: '#3a1912' },
            xml:        { name: 'XML',        iconType: 'file',     color: '#e34f26', bgLight: '#fff1ee', bgDark: '#3a1912' },
            svg:        { name: 'SVG',        iconType: 'code',     color: '#ffb13b', bgLight: '#fffbeb', bgDark: '#382b0e' },
            css:        { name: 'CSS',        iconType: 'code',     color: '#2563eb', bgLight: '#eff6ff', bgDark: '#132442' },
            scss:       { name: 'SCSS',       iconType: 'code',     color: '#c66597', bgLight: '#fdf2f8', bgDark: '#331323' },
            less:       { name: 'LESS',       iconType: 'code',     color: '#1d365d', bgLight: '#f0f4f9', bgDark: '#151e2b' },

            json:       { name: 'JSON',       iconType: 'box',      color: '#059669', bgLight: '#ecfdf5', bgDark: '#0f2f24' },
            yaml:       { name: 'YAML',       iconType: 'file',     color: '#dc2626', bgLight: '#fef2f2', bgDark: '#361414' },
            toml:       { name: 'TOML',       iconType: 'gear',     color: '#64748b', bgLight: '#f1f5f9', bgDark: '#1e293b' },
            ini:        { name: 'INI',        iconType: 'gear',     color: '#64748b', bgLight: '#f1f5f9', bgDark: '#1e293b' },
            sql:        { name: 'SQL',        iconType: 'database', color: '#0891b2', bgLight: '#ecfeff', bgDark: '#0e2b34' },

            bash:       { name: 'Bash',       iconType: 'terminal', color: '#16a34a', bgLight: '#f0fdf4', bgDark: '#0f2918', isTerminal: true },
            sh:         { name: 'Shell',      iconType: 'terminal', color: '#16a34a', bgLight: '#f0fdf4', bgDark: '#0f2918', isTerminal: true },
            zsh:        { name: 'Zsh',        iconType: 'terminal', color: '#16a34a', bgLight: '#f0fdf4', bgDark: '#0f2918', isTerminal: true },
            powershell: { name: 'PowerShell', iconType: 'terminal', color: '#0284c7', bgLight: '#eff6ff', bgDark: '#0c2742', isTerminal: true },
            batch:      { name: 'CMD',        iconType: 'terminal', color: '#64748b', bgLight: '#f8fafc', bgDark: '#1e293b', isTerminal: true },

            diff:       { name: 'Diff',       iconType: 'git',      color: '#10b981', bgLight: '#f0fdf4', bgDark: '#0d281a', isDiff: true },
            dockerfile: { name: 'Docker',     iconType: 'box',      color: '#0284c7', bgLight: '#f0f9ff', bgDark: '#122c42' },
            markdown:   { name: 'Markdown',   iconType: 'file',     color: '#4338ca', bgLight: '#eef2ff', bgDark: '#191c3d' }
        };

        const LANG_ALIASES = {
            py: 'python',
            js: 'javascript', mjs: 'javascript', cjs: 'javascript',
            ts: 'typescript',
            cs: 'csharp', 'c#': 'csharp',
            'c++': 'cpp', cc: 'cpp', cxx: 'cpp',
            golang: 'go', rs: 'rust', rb: 'ruby', kt: 'kotlin',
            ps: 'powershell', ps1: 'powershell', posh: 'powershell', pwsh: 'powershell',
            cmd: 'batch', bat: 'batch', shell: 'sh',
            yml: 'yaml', htm: 'html', md: 'markdown', docker: 'dockerfile'
        };

        function getLanguageInfo(rawLang) {
            const raw = (rawLang || '').toLowerCase().trim();
            const alias = LANG_ALIASES[raw] || raw;
            if (LANG_THEMES[alias]) {
                return Object.assign({ key: alias }, LANG_THEMES[alias]);
            }
            if (alias) {
                return {
                    key: alias,
                    name: alias.toUpperCase(),
                    iconType: 'code',
                    color: '#64748b',
                    bgLight: '#f1f5f9',
                    bgDark: '#1e293b'
                };
            }
            return {
                key: 'text',
                name: 'Plain Text',
                iconType: 'file',
                color: '#64748b',
                bgLight: '#f1f5f9',
                bgDark: '#1e293b'
            };
        }

        // Built-in PowerShell syntax highlighter fallback
        function highlightPowerShellFallback(rawCode) {
            const lines = rawCode.split('\n');
            return lines.map(line => {
                let esc = escapeHtml(line);
                if (/^\s*#/.test(line)) {
                    return `<span class='hljs-comment'>${esc}</span>`;
                }
                esc = esc.replace(/(&quot;.*?&quot;|&#39;.*?&#39;)/g, `<span class='hljs-string'>$1</span>`);
                esc = esc.replace(/(\$[a-zA-Z0-9_:]+)/g, `<span class='hljs-variable'>$1</span>`);
                esc = esc.replace(/\b(function|param|if|else|elseif|switch|foreach|for|while|try|catch|finally|return|break|continue|throw)\b/gi, `<span class='hljs-keyword'>$1</span>`);
                esc = esc.replace(/\b(Get|Set|New|Remove|Write|Start|Stop|Restart|Test|Find|Install|Update|Select|Where|Sort|Group|Measure|Out|Export|Import)-[a-zA-Z0-9]+\b/g, `<span class='hljs-built_in'>$&</span>`);
                return esc;
            }).join('\n');
        }

        // Table-based Zero-Misalignment Code Formatter
        function buildCodeTableHtml(rawCode, highlightedHtml, isDiff) {
            const rawLines = (highlightedHtml || '').split('\n');
            if (rawLines.length > 1 && rawLines[rawLines.length - 1] === '') {
                rawLines.pop();
            }
            const lineCount = Math.max(1, rawLines.length);
            let openTags = [];
            let rowsHtml = '';

            for (let i = 0; i < lineCount; i++) {
                const line = rawLines[i] !== undefined ? rawLines[i] : '';
                const prefix = openTags.join('');

                const tagRegex = /<\/?span[^>]*>/gi;
                let match;
                while ((match = tagRegex.exec(line)) !== null) {
                    const t = match[0];
                    if (t.startsWith('</')) {
                        openTags.pop();
                    } else {
                        openTags.push(t);
                    }
                }

                const suffix = openTags.map(() => '</span>').join('');
                const lineContent = line ? (prefix + line + suffix) : '&nbsp;';
                const lineNum = i + 1;

                let diffTrClass = '';
                if (isDiff) {
                    const textContent = line.replace(/<[^>]+>/g, '').trim();
                    if (textContent.startsWith('+') && !textContent.startsWith('+++')) {
                        diffTrClass = ' diff-add-tr';
                    } else if (textContent.startsWith('-') && !textContent.startsWith('---')) {
                        diffTrClass = ' diff-del-tr';
                    } else if (textContent.startsWith('@@')) {
                        diffTrClass = ' diff-meta-tr';
                    }
                }

                rowsHtml += `<tr class='code-tr${diffTrClass}'><td class='code-gutter' data-line='${lineNum}'>${lineNum}</td><td class='code-line'><code>${lineContent}</code></td></tr>`;
            }

            return {
                tableHtml: `<table class='code-table'><tbody>${rowsHtml}</tbody></table>`,
                lineCount: lineCount
            };
        }

        function enhanceCodeBlocks() {
            const isDark = document.body.classList.contains('dark-mode');

            document.querySelectorAll('.md-body pre > code, .code-viewer-container pre > code').forEach(codeEl => {
                const pre = codeEl.parentElement;
                if (!pre || (pre.parentElement && pre.parentElement.classList.contains('code-block-wrapper'))) return;

                // Skip Mermaid code blocks to let Mermaid renderer handle them
                if (codeEl.classList.contains('language-mermaid')) return;

                let rawLang = '';
                for (const cls of codeEl.classList) {
                    if (cls.startsWith('language-')) {
                        rawLang = cls.replace('language-', '');
                        break;
                    }
                }

                const info = getLanguageInfo(rawLang);
                const rawCode = codeEl.textContent || '';

                // Syntax Highlighting
                let highlightedHtml = '';
                if (info.isDiff) {
                    highlightedHtml = escapeHtml(rawCode);
                } else if (info.key === 'powershell') {
                    if (window.hljs && window.hljs.getLanguage && window.hljs.getLanguage('powershell')) {
                        try {
                            highlightedHtml = window.hljs.highlight(rawCode, { language: 'powershell', ignoreIllegals: true }).value;
                        } catch(e) {
                            highlightedHtml = highlightPowerShellFallback(rawCode);
                        }
                    } else {
                        highlightedHtml = highlightPowerShellFallback(rawCode);
                    }
                } else if (window.hljs) {
                    try {
                        const targetLang = (info.key !== 'text' && window.hljs.getLanguage && window.hljs.getLanguage(info.key)) ? info.key : undefined;
                        if (targetLang) {
                            highlightedHtml = window.hljs.highlight(rawCode, { language: targetLang, ignoreIllegals: true }).value;
                        } else {
                            window.hljs.highlightElement(codeEl);
                            highlightedHtml = codeEl.innerHTML;
                        }
                    } catch(e) {
                        highlightedHtml = escapeHtml(rawCode);
                    }
                } else {
                    highlightedHtml = escapeHtml(rawCode);
                }

                const tableRes = buildCodeTableHtml(rawCode, highlightedHtml, info.isDiff);
                const lineCount = tableRes.lineCount;

                const wrapper = document.createElement('div');
                wrapper.className = 'code-block-wrapper' + (info.isTerminal ? ' is-terminal' : '') + (info.isDiff ? ' is-diff' : '');
                wrapper.style.setProperty('--block-accent', info.color);
                wrapper.setAttribute('data-raw-code', rawCode);

                const accentBar = document.createElement('div');
                accentBar.className = 'code-accent-bar';

                const header = document.createElement('div');
                header.className = 'code-block-header';

                const badgeBg = isDark ? info.bgDark : info.bgLight;
                const badgeBorder = info.color + '40';

                const iconSvg = LANG_ICONS[info.iconType] || LANG_ICONS.code;
                const headerLeft = `
                    <div class='code-header-left'>
                        <div class='mac-dots' aria-hidden='true'>
                            <span class='mac-dot dot-red'></span>
                            <span class='mac-dot dot-yellow'></span>
                            <span class='mac-dot dot-green'></span>
                        </div>
                        <span class='lang-badge' style='background:${badgeBg}; color:${info.color}; border-color:${badgeBorder};'>
                            <span class='lang-icon'>${iconSvg}</span>
                            <span class='lang-name'>${escapeHtml(info.name)}</span>
                        </span>
                    </div>`;

                const collapseBtn = lineCount >= 8 ? `<button class='code-action-btn code-collapse-btn' onclick='toggleCodeBlock(this)'>收起 ▴</button>` : '';

                const headerRight = `
                    <div class='code-header-right'>
                        <span class='code-meta-lines'>${lineCount} 行</span>
                        ${collapseBtn}
                        <button class='code-copy-btn' onclick='copyBlockCode(this)'>
                            <svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><rect x='9' y='9' width='13' height='13' rx='2'></rect><path d='M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1'></path></svg>
                            <span>复制</span>
                        </button>
                    </div>`;

                header.innerHTML = headerLeft + headerRight;

                const bodyContainer = document.createElement('div');
                bodyContainer.className = 'code-body-container';
                bodyContainer.innerHTML = tableRes.tableHtml;

                pre.parentNode.insertBefore(wrapper, pre);
                wrapper.appendChild(accentBar);
                wrapper.appendChild(header);
                wrapper.appendChild(bodyContainer);
                pre.remove();
            });

            // Inline Code Click-To-Copy
            document.querySelectorAll('.md-body code').forEach(codeEl => {
                if (codeEl.parentElement && codeEl.parentElement.tagName.toLowerCase() === 'pre') return;
                if (codeEl.classList.contains('inline-code-pill')) return;

                codeEl.classList.add('inline-code-pill');
                codeEl.title = '点击复制此代码';
                codeEl.addEventListener('click', () => {
                    copyTextToClipboard(codeEl.textContent);
                });
            });
        }

        window.toggleCodeBlock = function(btn) {
            const wrapper = btn.closest('.code-block-wrapper');
            if (!wrapper) return;
            const body = wrapper.querySelector('.code-body-container');
            if (!body) return;
            if (body.classList.contains('collapsed')) {
                body.classList.remove('collapsed');
                btn.textContent = '收起 ▴';
            } else {
                body.classList.add('collapsed');
                btn.textContent = '展开 ▾';
            }
        };

        window.copyBlockCode = function(btn) {
            const wrapper = btn.closest('.code-block-wrapper');
            if (!wrapper) return;
            const rawCode = wrapper.getAttribute('data-raw-code');
            let textToCopy = rawCode;
            if (textToCopy === null || textToCopy === undefined) {
                const pre = wrapper.querySelector('pre');
                textToCopy = pre ? pre.textContent : '';
            }
            copyTextToClipboard(textToCopy);
            btn.classList.add('copied');
            const span = btn.querySelector('span');
            if (span) span.textContent = '已复制!';
            setTimeout(() => {
                btn.classList.remove('copied');
                if (span) span.textContent = '复制';
            }, 1800);
        };

        function enhanceTaskLists() {
            const container = document.getElementById('md-container');
            if (!container) return;
            const checkboxes = container.querySelectorAll('li input[type=\x27checkbox\x27], .task-list-item input[type=\x27checkbox\x27]');
            checkboxes.forEach((chk, idx) => {
                chk.removeAttribute('disabled');
                chk.style.cursor = 'pointer';
                chk.onchange = function() {
                    toggleMarkdownTaskItem(chk, idx);
                };
            });
        }

        function toggleMarkdownTaskItem(chk, taskIdx) {
            let raw = window.__RAW_DOC_CONTENT__ || '';
            const regex = /^(\s*[-*+]\s+\[)([ xX])(\]\s+)/gm;
            let match;
            let currentIdx = 0;
            let targetMatch = null;
            while ((match = regex.exec(raw)) !== null) {
                if (currentIdx === taskIdx) {
                    targetMatch = match;
                    break;
                }
                currentIdx++;
            }
            if (targetMatch) {
                const replaceChar = chk.checked ? 'x' : ' ';
                const start = targetMatch.index + targetMatch[1].length;
                const newRaw = raw.substring(0, start) + replaceChar + raw.substring(start + 1);
                window.__RAW_DOC_CONTENT__ = newRaw;
                window.omniTriggerChange(newRaw, true);
                showToast(chk.checked ? '已勾选任务项并同步' : '已取消勾选任务项并同步');
            }
        }

        function enhanceMarkdownAlerts() {
            const container = document.getElementById('md-container');
            if (!container) return;
            const ALERT_TYPES = {
                'note':      { name: 'Note',      svg: '<svg viewBox=\x270 0 16 16\x27 width=\x2716\x27 height=\x2716\x27><path d=\x27M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z\x27></path></svg>' },
                'tip':       { name: 'Tip',       svg: '<svg viewBox=\x270 0 16 16\x27 width=\x2716\x27 height=\x2716\x27><path d=\x27M8 1.5c-2.363 0-4 1.69-4 3.75 0 .761.22 1.5.591 2.05-.138.2-.24.444-.24.7 0 .55.45 1 1 1h5.3c.55 0 1-.45 1-1 0-.256-.102-.5-.24-.7.371-.55.59-1.289.59-2.05 0-2.06-1.637-3.75-4-3.75ZM6 11.25a.75.75 0 0 1 .75-.75h2.5a.75.75 0 0 1 0 1.5h-2.5a.75.75 0 0 1-.75-.75Zm.75 2.25a.75.75 0 0 0 0 1.5h1.5a.75.75 0 0 0 0-1.5h-1.5Z\x27></path></svg>' },
                'important': { name: 'Important', svg: '<svg viewBox=\x270 0 16 16\x27 width=\x2716\x27 height=\x2716\x27><path d=\x27M0 1.75C0 .784.784 0 1.75 0h12.5C15.216 0 16 .784 16 1.75v9.5A1.75 1.75 0 0 1 14.25 13H9.06l-2.573 2.573A1.458 1.458 0 0 1 4 14.543V13H1.75A1.75 1.75 0 0 1 0 11.25Zm1.75-.25a.25.25 0 0 0-.25.25v9.5c0 .138.112.25.25.25h2.5a.75.75 0 0 1 .75.75v2.19l2.72-2.72a.749.749 0 0 1 .53-.22h6.5a.25.25 0 0 0 .25-.25v-9.5a.25.25 0 0 0-.25-.25Zm7 2.25v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 9a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z\x27></path></svg>' },
                'warning':   { name: 'Warning',   svg: '<svg viewBox=\x270 0 16 16\x27 width=\x2716\x27 height=\x2716\x27><path d=\x27M6.457 1.047c.659-1.234 2.427-1.234 3.086 0l6.082 11.378A1.75 1.75 0 0 1 14.082 15H1.918a1.75 1.75 0 0 1-1.543-2.575Zm1.763.707a.25.25 0 0 0-.44 0L1.698 13.132a.25.25 0 0 0 .22.368h12.164a.25.25 0 0 0 .22-.368Zm.53 3.996v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 11a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z\x27></path></svg>' },
                'caution':   { name: 'Caution',   svg: '<svg viewBox=\x270 0 16 16\x27 width=\x2716\x27 height=\x2716\x27><path d=\x27M4.47.22A.749.749 0 0 1 5 0h6c.199 0 .389.079.53.22l4.25 4.25c.141.14.22.331.22.53v6a.749.749 0 0 1-.22.53l-4.25 4.25A.749.749 0 0 1 11 16H5a.749.749 0 0 1-.53-.22L.22 11.53A.749.749 0 0 1 0 11V5c0-.199.079-.389.22-.53Zm.84 1.28L1.5 5.31v5.38l3.81 3.81h5.38l3.81-3.81V5.31L10.69 1.5ZM8 4a.75.75 0 0 1 .75.75v3.5a.75.75 0 0 1-1.5 0v-3.5A.75.75 0 0 1 8 4Zm0 8a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z\x27></path></svg>' }
            };

            // Fallback for regular blockquotes starting with [!TYPE]
            container.querySelectorAll('blockquote').forEach(bq => {
                const firstP = bq.querySelector('p') || bq;
                if (!firstP) return;
                const match = firstP.textContent.trim().match(/^\[!(NOTE|TIP|IMPORTANT|WARNING|CAUTION)\]/i);
                if (match) {
                    const type = match[1].toLowerCase();
                    const conf = ALERT_TYPES[type] || ALERT_TYPES['note'];
                    const alertDiv = document.createElement('div');
                    alertDiv.className = 'markdown-alert markdown-alert-' + type;
                    const titleP = document.createElement('p');
                    titleP.className = 'markdown-alert-title';
                    titleP.innerHTML = conf.svg + conf.name;
                    alertDiv.appendChild(titleP);

                    firstP.innerHTML = firstP.innerHTML.replace(/^\[!(NOTE|TIP|IMPORTANT|WARNING|CAUTION)\]\s*(<br\s*\/?>)?/i, '').trim();
                    if (!firstP.textContent.trim() && !firstP.children.length) firstP.remove();

                    while (bq.firstChild) alertDiv.appendChild(bq.firstChild);
                    bq.replaceWith(alertDiv);
                }
            });
        }

        // ==================== MARKDOWN ANCHOR SMOOTH SCROLL & TOLERANT NAVIGATION ====================
        document.addEventListener('click', (e) => {
            const link = e.target.closest('a');
            if (!link) return;
            const href = link.getAttribute('href');
            if (!href) return;

            // 1. In-page anchor navigation (e.g. #section or https://local.doc/#section)
            let hash = '';
            if (href.startsWith('#')) {
                hash = href.substring(1);
            } else {
                const hashIdx = href.indexOf('#');
                if (hashIdx >= 0) {
                    const urlPart = href.substring(0, hashIdx);
                    if (urlPart === '' || urlPart === window.location.href.split('#')[0] || urlPart.indexOf('local.doc') >= 0) {
                        hash = href.substring(hashIdx + 1);
                    }
                }
            }

            if (hash) {
                e.preventDefault();
                navigateToMarkdownAnchor(hash);
                return;
            }

            // 2. External links: open in system default browser via shell
            if (href.startsWith('http://') || href.startsWith('https://')) {
                if (href.indexOf('local.doc') < 0) {
                    e.preventDefault();
                    postAct('OPEN_EXTERNAL_URL:' + href);
                }
            }
        });

        function navigateToMarkdownAnchor(rawHash) {
            if (!rawHash) return;
            const decodedHash = decodeURIComponent(rawHash).trim();

            // Try exact ID match
            let target = document.getElementById(decodedHash) ||
                         document.getElementById(rawHash);
            if (!target && window.CSS && CSS.escape) {
                try {
                    target = document.querySelector('[name=\x27' + CSS.escape(decodedHash) + '\x27]') ||
                             document.querySelector('[name=\x27' + CSS.escape(rawHash) + '\x27]');
                } catch(e) {}
            }

            // Fallback 1: case-insensitive ID match
            if (!target) {
                const lowerDecoded = decodedHash.toLowerCase();
                target = Array.from(document.querySelectorAll('[id]')).find(el => el.id && el.id.toLowerCase() === lowerDecoded);
            }

            // Fallback 2: Heading text content fuzzy match
            if (!target) {
                const cleanHash = decodedHash.replace(/^[0-9\.\-\s]+/, '').replace(/[\-\s]+/g, '').toLowerCase();
                const headings = Array.from(document.querySelectorAll('h1, h2, h3, h4, h5, h6'));
                target = headings.find(h => {
                    const hClean = h.textContent.replace(/^[0-9\.\-\s]+/, '').replace(/[\-\s]+/g, '').toLowerCase();
                    return hClean === cleanHash || (cleanHash.length > 2 && (hClean.indexOf(cleanHash) >= 0 || cleanHash.indexOf(hClean) >= 0));
                });
            }

            if (target) {
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
                target.classList.remove('anchor-target-highlight');
                void target.offsetWidth;
                target.classList.add('anchor-target-highlight');
                setTimeout(() => { target.classList.remove('anchor-target-highlight'); }, 2000);
            }
        }

        function renderEnhancements() {
            enhanceCodeBlocks();
            enhanceTaskLists();
            enhanceMarkdownAlerts();
            if (typeof katex !== 'undefined') {
                document.querySelectorAll('.math').forEach(n => {
                    let tex = n.textContent.trim();
                    const isBlk = n.tagName.toLowerCase() === 'div' || tex.startsWith('\\[');
                    if (tex.startsWith('\\(') || tex.startsWith('\\[')) tex = tex.slice(2, -2);
                    try { katex.render(tex, n, { displayMode: isBlk, throwOnError: false }); } catch(e){}
                });
            }
            if (typeof mermaid !== 'undefined') {
                try {
                    mermaid.initialize({
                        startOnLoad: false,
                        securityLevel: 'loose',
                        fontFamily: '-apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Microsoft YaHei"", ""PingFang SC"", sans-serif',
                        flowchart: { padding: 18, htmlLabels: true }
                    });
                    document.querySelectorAll('pre > code.language-mermaid').forEach(c => {
                        const d = document.createElement('div'); d.className = 'mermaid'; d.textContent = c.textContent;
                        const w = c.closest('.code-block-wrapper') || c.parentElement;
                        w.replaceWith(d);
                    });
                    mermaid.run();
                } catch(e) {}
            }
        }

        // ==================== SVG VIEW LOGIC (支持交互缩放、平移与格式转换) ====================
        let svgScale = 1, svgPanX = 0, svgPanY = 0;
        let svgPanning = false, svgStartX = 0, svgStartY = 0;
        let svgBgColor = 'none'; // 'none', '#ffffff', or any hex
        let svgBgName = '不显示';
        let svgNaturalWidth = 0, svgNaturalHeight = 0;

        function applySvgTransform() {
            const canvas = document.getElementById('svg-canvas');
            if (canvas) {
                const px = Math.round(svgPanX);
                const py = Math.round(svgPanY);
                canvas.style.transform = `translate3d(${px}px, ${py}px, 0) scale(${svgScale})`;
            }
            const zoomTag = document.getElementById('svg-zoom-tag');
            if (zoomTag) zoomTag.textContent = Math.round(svgScale * 100) + '%';
        }

        window.zoomSvg = function(factor) {
            svgScale = Math.min(10, Math.max(0.1, svgScale * factor));
            applySvgTransform();
        };

        window.resetSvgZoom = function() {
            svgScale = 1; svgPanX = 0; svgPanY = 0;
            applySvgTransform();
        };

        function calcSvgInitialScale() {
            const vp = document.getElementById('svg-viewport');
            if (!vp || svgNaturalWidth <= 0 || svgNaturalHeight <= 0) return 1.0;
            const padX = 40;
            const padY = 40;
            const availW = Math.max(100, vp.clientWidth - padX);
            const availH = Math.max(100, vp.clientHeight - padY);
            const scaleX = availW / svgNaturalWidth;
            const scaleY = availH / svgNaturalHeight;
            const fitScale = Math.min(scaleX, scaleY);
            const rounded = Math.round(fitScale * 100) / 100;
            return Math.max(0.1, Math.min(5.0, rounded));
        }

        window.fitSvgToViewport = function() {
            svgScale = calcSvgInitialScale();
            svgPanX = 0; svgPanY = 0;
            applySvgTransform();
        };

        window.handleSvgWheel = function(e) {
            e.preventDefault();
            const factor = e.deltaY < 0 ? 1.15 : 0.87;
            svgScale = Math.min(10, Math.max(0.1, svgScale * factor));
            applySvgTransform();
        };

        window.startSvgPan = function(e) {
            if (e.button !== 0) return;
            if (e.target.closest && e.target.closest('.palette-dropdown-wrap')) return;
            svgPanning = true;
            svgStartX = e.clientX - svgPanX;
            svgStartY = e.clientY - svgPanY;
            const vp = document.getElementById('svg-viewport');
            if (vp) vp.style.cursor = 'grabbing';
            const canvas = document.getElementById('svg-canvas');
            if (canvas) canvas.classList.add('is-panning');
        };

        window.toggleSvgBg = function() {
            window.togglePalettePopover('svg');
        };

        window.copySvgDataUri = function() {
            const svg = window.__RAW_SVG__ || '';
            if (!svg) { showToast('无可用 SVG 数据'); return; }
            try {
                const encoded = 'data:image/svg+xml;base64,' + btoa(unescape(encodeURIComponent(svg)));
                copyTextToClipboard(encoded);
                showToast('已复制 SVG Data URI');
            } catch(e) {
                showToast('复制 Data URI 失败: ' + e.message);
            }
        };

        window.exportSvgAsPng = function() {
            let svg = window.__RAW_SVG__ || '';
            if (!svg) { showToast('无可用 SVG 数据'); return; }
            if (!svg.includes('xmlns=')) {
                svg = svg.replace('<svg ', '<svg xmlns=\'http://www.w3.org/2000/svg\' ');
            }
            const img = new Image();
            const svgBlob = new Blob([svg], { type: 'image/svg+xml;charset=utf-8' });
            const url = URL.createObjectURL(svgBlob);
            img.onload = function() {
                const canvas = document.createElement('canvas');
                const naturalW = svgNaturalWidth > 0 ? svgNaturalWidth : (img.naturalWidth || img.width || 800);
                const naturalH = svgNaturalHeight > 0 ? svgNaturalHeight : (img.naturalHeight || img.height || 600);
                const scaleFactor = 2; // 2x Retina 超清导出
                canvas.width = Math.round(naturalW * scaleFactor);
                canvas.height = Math.round(naturalH * scaleFactor);
                const ctx = canvas.getContext('2d');
                ctx.scale(scaleFactor, scaleFactor);
                if (svgBgColor === 'none') {
                    ctx.clearRect(0, 0, naturalW, naturalH);
                } else {
                    ctx.fillStyle = svgBgColor;
                    ctx.fillRect(0, 0, naturalW, naturalH);
                }
                ctx.drawImage(img, 0, 0, naturalW, naturalH);
                URL.revokeObjectURL(url);

                const base64Data = canvas.toDataURL('image/png');
                if (window.chrome && window.chrome.webview) {
                    window.chrome.webview.postMessage('EXPORT_IMAGE:' + JSON.stringify({ filename: 'vector_export.png', data: base64Data }));
                } else {
                    const a = document.createElement('a');
                    a.download = 'vector_export.png';
                    a.href = base64Data;
                    a.click();
                    showToast('已导出 2x 超清 PNG 图片');
                }
            };
            img.onerror = function() {
                URL.revokeObjectURL(url);
                showToast('导出图片失败，请检查 SVG 语法');
            };
            img.src = url;
        };

        let svgResizeTimer = null;
        window.addEventListener('resize', () => {
            if (window.__DOC_FORMAT__ === 'svg') {
                if (svgResizeTimer) clearTimeout(svgResizeTimer);
                svgResizeTimer = setTimeout(() => {
                    if (svgPanX === 0 && svgPanY === 0) {
                        window.fitSvgToViewport();
                    }
                }, 100);
            }
        });

        function initSvgView(preserveView) {
            const canvas = document.getElementById('svg-canvas');
            if (!canvas) return;
            const svgEl = canvas.querySelector('svg');
            const dimTag = document.getElementById('svg-dim-tag');
            if (svgEl) {
                let w = 0, h = 0;
                const vb = svgEl.viewBox ? svgEl.viewBox.baseVal : null;
                if (vb && vb.width > 0 && vb.height > 0) {
                    w = vb.width;
                    h = vb.height;
                } else {
                    const vbAttr = svgEl.getAttribute('viewBox');
                    if (vbAttr) {
                        const p = vbAttr.trim().split(/[\s,]+/).map(Number);
                        if (p.length === 4) { w = p[2]; h = p[3]; }
                    }
                }
                if (w <= 0 || h <= 0) {
                    w = parseFloat(svgEl.getAttribute('width')) || 0;
                    h = parseFloat(svgEl.getAttribute('height')) || 0;
                }
                if (w <= 0 || h <= 0) {
                    try {
                        const bbox = svgEl.getBBox();
                        w = bbox.width;
                        h = bbox.height;
                    } catch(e){}
                }
                if (w > 0 && h > 0) {
                    svgNaturalWidth = w;
                    svgNaturalHeight = h;
                    if (dimTag) dimTag.textContent = `尺寸: ${Math.round(w)} × ${Math.round(h)}`;
                } else if (dimTag) {
                    dimTag.textContent = '尺寸: 自适应';
                }
                if (!preserveView) {
                    window.fitSvgToViewport();
                } else {
                    applySvgTransform();
                }
            }
        }

        // ==================== YAML VIEW LOGIC (复用 JSON 树状折叠与转换) ====================
        function parseYamlFallback(yaml) {
            const lines = (yaml || '').split('\n');
            const obj = {};
            for (const line of lines) {
                const trimmed = line.trim();
                if (!trimmed || trimmed.startsWith('#')) continue;
                const idx = trimmed.indexOf(':');
                if (idx > 0) {
                    const k = trimmed.substring(0, idx).trim();
                    const v = trimmed.substring(idx + 1).trim();
                    if (v === 'true') obj[k] = true;
                    else if (v === 'false') obj[k] = false;
                    else if (v === 'null' || v === '~') obj[k] = null;
                    else if (!isNaN(Number(v)) && v !== '') obj[k] = Number(v);
                    else obj[k] = v.replace(/^['""]|['""]$/g, '');
                }
            }
            return obj;
        }

        function initYamlView() {
            const root = document.getElementById('yaml-root');
            if (!root || window.__RAW_YAML__ === undefined) return;
            try {
                let parsed = null;
                if (window.jsyaml && window.jsyaml.load) {
                    parsed = window.jsyaml.load(window.__RAW_YAML__);
                } else {
                    parsed = parseYamlFallback(window.__RAW_YAML__);
                }
                window.__PARSED_JSON__ = parsed;
                root.innerHTML = buildJsonTreeHtml(parsed, undefined, false, []);
                updateJsonNodeCount();
            } catch (err) {
                root.innerHTML = `<div class='json-err'><strong>YAML 语法解析错误：</strong><br/>${escapeHtml(err.message)}</div>`;
            }
        }

        window.copyFormattedYamlJson = function() {
            if (window.__RAW_YAML__ === undefined) return;
            try {
                const parsed = window.jsyaml ? window.jsyaml.load(window.__RAW_YAML__) : parseYamlFallback(window.__RAW_YAML__);
                copyTextToClipboard(JSON.stringify(parsed, null, 2));
                showToast('已复制解析后的 JSON');
            } catch(e) {
                showToast('YAML 解析失败，无法转换: ' + e.message);
            }
        };

        window.beautifyYaml = function() {
            if (!window.__RAW_YAML__) return;
            try {
                if (window.jsyaml && window.jsyaml.load && window.jsyaml.dump) {
                    const parsed = window.jsyaml.load(window.__RAW_YAML__);
                    const dumped = window.jsyaml.dump(parsed, { indent: 2, lineWidth: -1 });
                    postAct('UPDATE_BUFFER:' + dumped);
                    showToast('已美化 YAML 缩进并同步至文档');
                } else {
                    showToast('YAML 解析引擎初始化中，请稍候');
                }
            } catch(e) {
                showToast('YAML 语法错误，美化失败: ' + e.message);
            }
        };

        // ==================== MERMAID VIEW LOGIC (时序图/流程图高精渲染与调色盘) ====================
        let mmScale = 1, mmPanX = 0, mmPanY = 0;
        let mmPanning = false, mmStartX = 0, mmStartY = 0;
        let mmBgColor = '#ffffff'; // 'none', '#ffffff', or any hex
        let mmBgName = '白色';
        let mmNaturalWidth = 0, mmNaturalHeight = 0;

        function isDarkColor(colorStr) {
            if (!colorStr || colorStr === 'none') {
                return document.body.classList.contains('dark-mode');
            }
            let r = 255, g = 255, b = 255;
            if (colorStr.startsWith('#')) {
                let hex = colorStr.slice(1);
                if (hex.length === 3) hex = hex.split('').map(c => c + c).join('');
                if (hex.length >= 6) {
                    r = parseInt(hex.substr(0, 2), 16) || 0;
                    g = parseInt(hex.substr(2, 2), 16) || 0;
                    b = parseInt(hex.substr(4, 2), 16) || 0;
                }
            }
            const yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;
            return yiq < 130;
        }

        function applyMermaidTransform() {
            const canvas = document.getElementById('mermaid-canvas');
            if (canvas) {
                const px = Math.round(mmPanX);
                const py = Math.round(mmPanY);
                canvas.style.transform = `translate(${px}px, ${py}px) scale(${mmScale})`;
            }
            const zoomTag = document.getElementById('mermaid-zoom-tag');
            if (zoomTag) zoomTag.textContent = Math.round(mmScale * 100) + '%';
        }

        window.zoomMermaid = function(factor) {
            mmScale = Math.min(10, Math.max(0.1, mmScale * factor));
            applyMermaidTransform();
        };

        window.resetMermaidZoom = function() {
            mmScale = 1; mmPanX = 0; mmPanY = 0;
            applyMermaidTransform();
        };

        window.fitMermaidToViewport = function() {
            mmScale = calcMermaidInitialScale();
            mmPanX = 0; mmPanY = 0;
            applyMermaidTransform();
        };

        window.handleMermaidWheel = function(e) {
            e.preventDefault();
            const factor = e.deltaY < 0 ? 1.15 : 0.87;
            mmScale = Math.min(10, Math.max(0.1, mmScale * factor));
            applyMermaidTransform();
        };

        window.startMermaidPan = function(e) {
            if (e.button !== 0) return;
            if (e.target.closest && e.target.closest('.palette-dropdown-wrap')) return;
            mmPanning = true;
            mmStartX = e.clientX - mmPanX;
            mmStartY = e.clientY - mmPanY;
            const vp = document.getElementById('mermaid-viewport');
            if (vp) vp.style.cursor = 'grabbing';
            const canvas = document.getElementById('mermaid-canvas');
            if (canvas) canvas.classList.add('is-panning');
        };

        function calcMermaidInitialScale() {
            const vp = document.getElementById('mermaid-viewport');
            if (!vp || mmNaturalWidth <= 0 || mmNaturalHeight <= 0) return 1.0;
            const padX = 40;
            const padY = 40;
            const availW = Math.max(100, vp.clientWidth - padX);
            const availH = Math.max(100, vp.clientHeight - padY);
            const scaleX = availW / mmNaturalWidth;
            const scaleY = availH / mmNaturalHeight;
            const fitScale = Math.min(scaleX, scaleY);
            const rounded = Math.round(fitScale * 100) / 100;
            return Math.max(0.15, Math.min(2.0, rounded));
        }

        let mmResizeTimer = null;
        window.addEventListener('resize', () => {
            if (window.__DOC_FORMAT__ === 'mermaid') {
                if (mmResizeTimer) clearTimeout(mmResizeTimer);
                mmResizeTimer = setTimeout(() => {
                    if (mmPanX === 0 && mmPanY === 0) {
                        window.fitMermaidToViewport();
                    }
                }, 100);
            }
        });

        // 根治 mmd 模糊与微型显示：重置 SVG 为其真实的原始矢量尺寸，默认以清晰可读比例居中展现
        function normalizeMermaidSvg(isInitial) {
            const svg = document.querySelector('#mermaid-canvas svg');
            if (!svg) return false;
            let w = 0, h = 0;
            const vb = svg.viewBox ? svg.viewBox.baseVal : null;
            if (vb && vb.width > 0 && vb.height > 0) {
                w = vb.width;
                h = vb.height;
            } else {
                const vbAttr = svg.getAttribute('viewBox');
                if (vbAttr) {
                    const p = vbAttr.trim().split(/[\s,]+/).map(Number);
                    if (p.length === 4) { w = p[2]; h = p[3]; }
                }
            }
            if (w > 0 && h > 0) {
                mmNaturalWidth = w;
                mmNaturalHeight = h;
                svg.style.width = w + 'px';
                svg.style.height = h + 'px';
                svg.style.minWidth = w + 'px';
                svg.style.minHeight = h + 'px';
                svg.style.maxWidth = 'none';
                svg.style.maxHeight = 'none';
                svg.removeAttribute('width');
                svg.removeAttribute('height');

                if (isInitial) {
                    mmScale = calcMermaidInitialScale();
                    mmPanX = 0;
                    mmPanY = 0;
                    applyMermaidTransform();
                } else {
                    applyMermaidTransform();
                }

                const canvas = document.getElementById('mermaid-canvas');
                if (canvas) canvas.classList.add('rendered');
                const loader = document.getElementById('mermaid-loading-state');
                if (loader) {
                    loader.classList.add('loaded');
                    setTimeout(() => { loader.style.display = 'none'; }, 320);
                }
                return true;
            }
            return false;
        }

        // ==================== UNIFIED COLOR PALETTE LOGIC (SVG & MERMAID 通用调色盘) ====================
        window.togglePalettePopover = function(target, e) {
            if (e) {
                e.preventDefault();
                e.stopPropagation();
            }
            const pop = document.getElementById(target + '-palette-popover');
            if (!pop) return;
            const isOpen = pop.classList.contains('open');
            window.closeAllPalettePopovers();
            if (!isOpen) pop.classList.add('open');
        };

        window.closeAllPalettePopovers = function() {
            document.querySelectorAll('.palette-popover.open').forEach(p => p.classList.remove('open'));
        };

        document.addEventListener('click', (e) => {
            if (!e.target.closest || !e.target.closest('.palette-dropdown-wrap')) {
                window.closeAllPalettePopovers();
            }
        });

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                window.closeAllPalettePopovers();
            }
        });

        window.setCanvasBg = function(target, color, name, shouldClosePopover = false) {
            const vp = document.getElementById(target + '-viewport');
            const ind = document.getElementById(target + '-bg-indicator');
            const txt = document.getElementById(target + '-bg-text');
            const hexInp = document.getElementById(target + '-custom-hex');
            const colInp = document.getElementById(target + '-custom-color');
            if (!vp) return;

            let oldIsDark = false;
            let bgName = name || (color === 'none' ? '不显示' : color.toUpperCase());

            if (target === 'svg') {
                svgBgColor = color;
                svgBgName = bgName;
            } else if (target === 'mermaid') {
                oldIsDark = isDarkColor(mmBgColor);
                mmBgColor = color;
                mmBgName = bgName;
            }

            if (color === 'none') {
                vp.classList.add('bg-none');
                vp.style.removeProperty('background-color');
                vp.style.removeProperty('background-image');
                if (ind) {
                    ind.className = 'color-swatch-badge swatch-none';
                    ind.style.backgroundColor = '';
                }
            } else {
                vp.classList.remove('bg-none');
                vp.style.setProperty('background-color', color, 'important');
                vp.style.setProperty('background-image', 'none', 'important');
                if (ind) {
                    ind.className = 'color-swatch-badge';
                    ind.style.backgroundColor = color;
                }
                if (hexInp) hexInp.value = color.toUpperCase();
                if (colInp && color.startsWith('#')) colInp.value = color;
            }

            if (txt) txt.textContent = '背景: ' + bgName;

            // 更新预设 chip 的 active 状态
            document.querySelectorAll('#' + target + '-palette-popover .palette-chip').forEach(chip => {
                const c = chip.getAttribute('data-color');
                if (c && c.toLowerCase() === color.toLowerCase()) {
                    chip.classList.add('active');
                } else {
                    chip.classList.remove('active');
                }
            });

            try {
                localStorage.setItem('npp_omniview_' + target + '_bg_color', color);
                localStorage.setItem('npp_omniview_' + target + '_bg_name', bgName);
            } catch(e){}

            if (target === 'mermaid') {
                const newIsDark = isDarkColor(color);
                if (oldIsDark !== newIsDark) {
                    initMermaidView(false);
                }
            }

            if (shouldClosePopover) {
                window.closeAllPalettePopovers();
                showToast('已切换背景: ' + bgName);
            }
        };

        window.onCanvasCustomColorInput = function(target, val) {
            const hexInp = document.getElementById(target + '-custom-hex');
            if (hexInp) hexInp.value = val.toUpperCase();
            window.setCanvasBg(target, val, '自定义 (' + val.toUpperCase() + ')', false);
        };

        window.onCanvasCustomColorChange = function(target, val) {
            window.setCanvasBg(target, val, '自定义 (' + val.toUpperCase() + ')', true);
        };

        window.onCanvasHexChange = function(target, val) {
            let hex = (val || '').trim();
            if (!hex.startsWith('#')) hex = '#' + hex;
            if (/^#[0-9A-Fa-f]{6}$/.test(hex) || /^#[0-9A-Fa-f]{3}$/.test(hex)) {
                window.setCanvasBg(target, hex, '自定义 (' + hex.toUpperCase() + ')', true);
            } else {
                showToast('请输入有效的 HEX 颜色代码，例如 #FFFFFF');
            }
        };

        // 向下兼容别名函数 (Mermaid & SVG)
        window.toggleMermaidPalette = function(e) { window.togglePalettePopover('mermaid', e); };
        window.closeMermaidPalette = function() { window.closeAllPalettePopovers(); };
        window.setMermaidBg = function(c, n) { window.setCanvasBg('mermaid', c, n); };
        window.onMermaidCustomColorInput = function(v) { window.onCanvasCustomColorInput('mermaid', v); };
        window.onMermaidCustomColorChange = function(v) { window.onCanvasCustomColorChange('mermaid', v); };
        window.onMermaidHexChange = function(v) { window.onCanvasHexChange('mermaid', v); };

        window.toggleSvgPalette = function(e) { window.togglePalettePopover('svg', e); };
        window.closeSvgPalette = function() { window.closeAllPalettePopovers(); };
        window.setSvgBg = function(c, n) { window.setCanvasBg('svg', c, n); };
        window.onSvgCustomColorInput = function(v) { window.onCanvasCustomColorInput('svg', v); };
        window.onSvgCustomColorChange = function(v) { window.onCanvasCustomColorChange('svg', v); };
        window.onSvgHexChange = function(v) { window.onCanvasHexChange('svg', v); };

        function initMermaidView(isInitial = true) {
            const root = document.getElementById('mermaid-root');
            if (!root || !window.__RAW_MERMAID__) return;
            if (typeof mermaid !== 'undefined') {
                try {
                    const isDark = isDarkColor(mmBgColor);
                    mermaid.initialize({
                        startOnLoad: false,
                        theme: isDark ? 'dark' : 'default',
                        securityLevel: 'loose',
                        fontFamily: '-apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Microsoft YaHei"", ""PingFang SC"", sans-serif',
                        flowchart: {
                            padding: 18,
                            htmlLabels: true,
                            useMaxWidth: false
                        },
                        sequence: {
                            boxMargin: 12,
                            useMaxWidth: false
                        }
                    });
                    root.removeAttribute('data-processed');
                    root.textContent = window.__RAW_MERMAID__;
                    const p = mermaid.run({ nodes: [root] });
                    if (p && typeof p.then === 'function') {
                        p.then(() => { normalizeMermaidSvg(isInitial); })
                         .catch(err => {
                             const loader = document.getElementById('mermaid-loading-state');
                             if (loader) loader.style.display = 'none';
                             const canvas = document.getElementById('mermaid-canvas');
                             if (canvas) canvas.classList.add('rendered');
                             root.innerHTML = `<div class='json-err'><strong>Mermaid 语法错误：</strong><br/>${escapeHtml(err.message)}</div>`;
                         });
                    } else {
                        setTimeout(() => { normalizeMermaidSvg(isInitial); }, 50);
                    }
                } catch(e) {
                    const loader = document.getElementById('mermaid-loading-state');
                    if (loader) loader.style.display = 'none';
                    const canvas = document.getElementById('mermaid-canvas');
                    if (canvas) canvas.classList.add('rendered');
                    root.innerHTML = `<div class='json-err'><strong>Mermaid 语法错误：</strong><br/>${escapeHtml(e.message)}</div>`;
                }
            }
        }

        window.copyMermaidSvg = function() {
            const svg = document.querySelector('#mermaid-canvas svg');
            if (svg) {
                copyTextToClipboard(svg.outerHTML);
                showToast('已复制 Mermaid 矢量 SVG');
            } else {
                showToast('未检测到生成的矢量图');
            }
        };

        window.exportMermaidPng = function() {
            const svg = document.querySelector('#mermaid-canvas svg');
            if (!svg) { showToast('未检测到生成的矢量图'); return; }
            
            try {
                // 克隆 SVG 并注入独立渲染所需的命名空间与样式
                const clonedSvg = svg.cloneNode(true);
                if (!clonedSvg.getAttribute('xmlns')) {
                    clonedSvg.setAttribute('xmlns', 'http://www.w3.org/2000/svg');
                }
                if (!clonedSvg.getAttribute('xmlns:xlink')) {
                    clonedSvg.setAttribute('xmlns:xlink', 'http://www.w3.org/1999/xlink');
                }

                // 提取 Mermaid 全局动态样式并嵌入 defs/style
                let styleText = '';
                document.querySelectorAll('style').forEach(st => {
                    const text = st.textContent || '';
                    if ((st.id && st.id.includes('mermaid')) || text.includes('.node') || text.includes('.cluster')) {
                        styleText += text + '\n';
                    }
                });
                if (styleText) {
                    let defs = clonedSvg.querySelector('defs');
                    if (!defs) {
                        defs = document.createElementNS('http://www.w3.org/2000/svg', 'defs');
                        clonedSvg.insertBefore(defs, clonedSvg.firstChild);
                    }
                    const styleEl = document.createElementNS('http://www.w3.org/2000/svg', 'style');
                    styleEl.setAttribute('type', 'text/css');
                    styleEl.textContent = styleText;
                    defs.appendChild(styleEl);
                }

                const svgStr = new XMLSerializer().serializeToString(clonedSvg);
                const svgBlob = new Blob([svgStr], { type: 'image/svg+xml;charset=utf-8' });
                const url = URL.createObjectURL(svgBlob);
                const img = new Image();

                img.onload = function() {
                    try {
                        const canvas = document.createElement('canvas');
                        const naturalW = mmNaturalWidth > 0 ? mmNaturalWidth : (clonedSvg.viewBox?.baseVal?.width || img.naturalWidth || img.width || 1200);
                        const naturalH = mmNaturalHeight > 0 ? mmNaturalHeight : (clonedSvg.viewBox?.baseVal?.height || img.naturalHeight || img.height || 800);
                        const scaleFactor = 2; // 2x Retina 超清导出
                        canvas.width = Math.round(naturalW * scaleFactor);
                        canvas.height = Math.round(naturalH * scaleFactor);
                        const ctx = canvas.getContext('2d');
                        ctx.scale(scaleFactor, scaleFactor);

                        if (mmBgColor === 'none') {
                            ctx.clearRect(0, 0, naturalW, naturalH);
                        } else {
                            ctx.fillStyle = mmBgColor;
                            ctx.fillRect(0, 0, naturalW, naturalH);
                        }
                        ctx.drawImage(img, 0, 0, naturalW, naturalH);
                        URL.revokeObjectURL(url);

                        const base64Data = canvas.toDataURL('image/png');
                        if (window.chrome && window.chrome.webview) {
                            window.chrome.webview.postMessage('EXPORT_IMAGE:' + JSON.stringify({ filename: 'mermaid_diagram.png', data: base64Data }));
                        } else {
                            const a = document.createElement('a');
                            a.download = 'mermaid_diagram.png';
                            a.href = base64Data;
                            a.click();
                            showToast('已导出 2x 超清 PNG 图片');
                        }
                    } catch(drawErr) {
                        URL.revokeObjectURL(url);
                        // 若因安全沙箱或 foreignObject 限制无法直接绘入 canvas，优雅降级为复制 SVG
                        if (window.copyMermaidSvg) window.copyMermaidSvg();
                        showToast('因含富文本节点已转为复制 SVG 矢量代码到剪贴板');
                    }
                };

                img.onerror = function() {
                    URL.revokeObjectURL(url);
                    if (window.copyMermaidSvg) window.copyMermaidSvg();
                    showToast('当前图表已自动将超清矢量 SVG 复制到剪贴板');
                };

                img.src = url;
            } catch(ex) {
                if (window.copyMermaidSvg) window.copyMermaidSvg();
                showToast('导出异常，已转为复制矢量 SVG 代码');
            }
        };

        // ==================== HTML VIEWPORT & RESOLUTION ENGINE ====================
        let htmlTargetW = 1920;
        let htmlTargetH = 1080;
        let htmlResName = '1080p';
        let htmlIsMobile = false;
        let htmlOrientation = 'landscape';
        let htmlAutoFit = true;

        function initHtmlView() {
            const iframe = document.getElementById('html-preview-frame');
            if (iframe && window.__RAW_HTML__ !== undefined) {
                iframe.srcdoc = window.__RAW_HTML__;
            }
            try {
                const saved = localStorage.getItem('npp_omniview_html_viewport');
                if (saved) {
                    const cfg = JSON.parse(saved);
                    if (cfg && typeof cfg.w === 'number') {
                        htmlTargetW = cfg.w;
                        htmlTargetH = cfg.h;
                        htmlResName = cfg.name || '1080p';
                        htmlIsMobile = !!cfg.isMobile;
                        htmlOrientation = cfg.orient || 'landscape';
                        if (typeof cfg.autoFit === 'boolean') htmlAutoFit = cfg.autoFit;
                    }
                }
            } catch(e) {}
            applyHtmlViewportStyles();
        }

        window.setHtmlResolution = function(w, h, name, isMobile) {
            if (w === 0 && h === 0) {
                htmlTargetW = 0;
                htmlTargetH = 0;
                htmlResName = '全宽';
                htmlIsMobile = false;
                htmlOrientation = 'landscape';
            } else {
                htmlTargetW = w;
                htmlTargetH = h;
                htmlResName = name || (w + '×' + h);
                htmlIsMobile = !!isMobile;
                if (htmlIsMobile && htmlTargetW > htmlTargetH) {
                    const tmp = htmlTargetW; htmlTargetW = htmlTargetH; htmlTargetH = tmp;
                    htmlOrientation = 'portrait';
                } else if (!htmlIsMobile && htmlTargetW < htmlTargetH) {
                    htmlOrientation = 'portrait';
                } else {
                    htmlOrientation = 'landscape';
                }
            }
            saveHtmlViewportPreferences();
            applyHtmlViewportStyles();
            showToast('已切换分辨率: ' + htmlResName);
        };

        window.toggleHtmlOrientation = function() {
            if (htmlTargetW === 0 && htmlTargetH === 0) {
                showToast('全宽模式无需切换横竖屏');
                return;
            }
            const tmp = htmlTargetW;
            htmlTargetW = htmlTargetH;
            htmlTargetH = tmp;
            htmlOrientation = (htmlOrientation === 'landscape') ? 'portrait' : 'landscape';
            saveHtmlViewportPreferences();
            applyHtmlViewportStyles();
            showToast('已切换为: ' + (htmlOrientation === 'landscape' ? '横屏 Landscape' : '竖屏 Portrait'));
        };

        window.toggleHtmlAutoFit = function() {
            htmlAutoFit = !htmlAutoFit;
            saveHtmlViewportPreferences();
            applyHtmlViewportStyles();
            showToast(htmlAutoFit ? '已启用等比缩放适应' : '已切换为 1:1 原始物理像素');
        };

        function saveHtmlViewportPreferences() {
            try {
                localStorage.setItem('npp_omniview_html_viewport', JSON.stringify({
                    w: htmlTargetW,
                    h: htmlTargetH,
                    name: htmlResName,
                    isMobile: htmlIsMobile,
                    orient: htmlOrientation,
                    autoFit: htmlAutoFit
                }));
            } catch(e) {}
        }

        function applyHtmlViewportStyles() {
            const frame = document.getElementById('html-device-frame');
            const stage = document.getElementById('html-viewport-stage');
            const wrapper = document.getElementById('html-viewport-wrapper');
            const pill = document.getElementById('html-mode-pill');
            const orientPill = document.getElementById('html-orient-pill');
            const dimTag = document.getElementById('html-dim-tag');
            const zoomTag = document.getElementById('html-zoom-tag');
            const selectText = document.getElementById('html-res-select-text');
            const btnFitText = document.getElementById('btn-fit-text');

            if (!frame || !stage || !wrapper) return;

            if (htmlIsMobile) {
                frame.classList.add('is-mobile');
            } else {
                frame.classList.remove('is-mobile');
            }

            if (htmlTargetW === 0 && htmlTargetH === 0) {
                stage.style.width = '100%';
                stage.style.height = '100%';
                stage.style.margin = '0';
                frame.style.position = 'relative';
                frame.style.top = '0';
                frame.style.left = '0';
                frame.style.width = '100%';
                frame.style.height = '100%';
                frame.style.transform = 'none';
                if (pill) pill.textContent = '桌面端全宽沙箱';
                if (orientPill) orientPill.textContent = '自适应 100%';
                if (dimTag) dimTag.textContent = '100% 视口';
                if (zoomTag) zoomTag.textContent = '100%';
                if (selectText) selectText.innerHTML = '桌面全宽 (100%)';
                if (btnFitText) btnFitText.textContent = '等比适应';
                return;
            }

            // 计算等比自适应缩放比例
            let scale = 1;
            if (htmlAutoFit) {
                const availW = Math.max(wrapper.clientWidth - 32, 100);
                const availH = Math.max(wrapper.clientHeight - 32, 100);
                const scaleX = availW / htmlTargetW;
                const scaleY = availH / htmlTargetH;
                scale = Math.min(scaleX, scaleY, 1);
            }

            const scaledW = Math.round(htmlTargetW * scale);
            const scaledH = Math.round(htmlTargetH * scale);

            stage.style.width = scaledW + 'px';
            stage.style.height = scaledH + 'px';

            // 尺寸小于视口时绝对水平垂直居中；超出视口（如1:1原尺寸）时左上贴齐支持完整滚动
            if (scaledW > wrapper.clientWidth || scaledH > wrapper.clientHeight) {
                stage.style.margin = '0 auto';
                if (scaledW > wrapper.clientWidth) stage.style.marginLeft = '0';
                if (scaledH > wrapper.clientHeight) stage.style.marginTop = '0';
            } else {
                stage.style.margin = 'auto';
            }

            frame.style.position = 'absolute';
            frame.style.top = '0';
            frame.style.left = '0';
            frame.style.width = htmlTargetW + 'px';
            frame.style.height = htmlTargetH + 'px';
            frame.style.transformOrigin = '0 0';
            frame.style.transform = (scale < 1) ? 'scale(' + scale.toFixed(4) + ')' : 'none';

            const pct = Math.round(scale * 100);
            if (zoomTag) zoomTag.textContent = pct + '%';
            if (btnFitText) btnFitText.textContent = htmlAutoFit ? '等比适应 (' + pct + '%)' : '1:1 原尺寸';

            const orientLabel = (htmlOrientation === 'landscape') ? '横屏 Landscape' : '竖屏 Portrait';
            if (pill) pill.textContent = htmlResName + ' (' + htmlTargetW + '×' + htmlTargetH + ')';
            if (orientPill) orientPill.textContent = orientLabel;
            if (dimTag) dimTag.textContent = htmlTargetW + ' × ' + htmlTargetH + ' px';
            if (selectText) selectText.innerHTML = htmlResName + ' (' + htmlTargetW + '&times;' + htmlTargetH + ')';
        }

        window.addEventListener('resize', () => {
            if (document.getElementById('html-preview-frame')) {
                applyHtmlViewportStyles();
            }
        });

        window.setHtmlViewport = function(mode) {
            if (mode === 'desktop') {
                setHtmlResolution(1920, 1080, '1080p', false);
            } else if (mode === 'tablet') {
                setHtmlResolution(768, 1024, '平板', true);
            } else if (mode === 'mobile') {
                setHtmlResolution(390, 844, '手机', true);
            }
        };

        window.openHtmlCustomResModal = function() {
            const modal = document.getElementById('html-custom-res-modal');
            const wInput = document.getElementById('html-custom-w-input');
            const hInput = document.getElementById('html-custom-h-input');
            const mobileCb = document.getElementById('html-custom-mobile-cb');
            if (!modal) return;
            if (wInput) wInput.value = htmlTargetW > 0 ? htmlTargetW : 1920;
            if (hInput) hInput.value = htmlTargetH > 0 ? htmlTargetH : 1080;
            if (mobileCb) mobileCb.checked = htmlIsMobile;
            modal.style.display = 'flex';
        };

        window.closeHtmlCustomResModal = function() {
            const modal = document.getElementById('html-custom-res-modal');
            if (modal) modal.style.display = 'none';
        };

        window.applyPresetCustomRes = function(w, h, isMobile) {
            const wInput = document.getElementById('html-custom-w-input');
            const hInput = document.getElementById('html-custom-h-input');
            const mobileCb = document.getElementById('html-custom-mobile-cb');
            if (wInput) wInput.value = w;
            if (hInput) hInput.value = h;
            if (mobileCb && isMobile !== undefined) mobileCb.checked = !!isMobile;
        };

        window.confirmCustomResolution = function() {
            const wInput = document.getElementById('html-custom-w-input');
            const hInput = document.getElementById('html-custom-h-input');
            const mobileCb = document.getElementById('html-custom-mobile-cb');
            let w = parseInt(wInput ? wInput.value : '', 10);
            let h = parseInt(hInput ? hInput.value : '', 10);
            if (isNaN(w) || w < 100 || w > 8000) {
                showToast('请输入有效的宽度 (100 - 8000 px)');
                return;
            }
            if (isNaN(h) || h < 100 || h > 8000) {
                showToast('请输入有效的高度 (100 - 8000 px)');
                return;
            }
            const isMobile = mobileCb ? mobileCb.checked : false;
            setHtmlResolution(w, h, '自定义 ' + w + '×' + h, isMobile);
            closeHtmlCustomResModal();
        };

        window.reloadHtmlPreview = function() {
            const iframe = document.getElementById('html-preview-frame');
            if (iframe) iframe.srcdoc = window.__RAW_HTML__ || '';
            showToast('已刷新页面');
        };

        let panRafPending = false;
        window.addEventListener('mousemove', (e) => {
            if (svgPanning) {
                svgPanX = e.clientX - svgStartX;
                svgPanY = e.clientY - svgStartY;
            }
            if (mmPanning) {
                mmPanX = e.clientX - mmStartX;
                mmPanY = e.clientY - mmStartY;
            }
            if ((svgPanning || mmPanning) && !panRafPending) {
                panRafPending = true;
                requestAnimationFrame(() => {
                    if (svgPanning) applySvgTransform();
                    if (mmPanning) applyMermaidTransform();
                    panRafPending = false;
                });
            }
        });

        window.addEventListener('mouseup', () => {
            if (svgPanning) {
                svgPanning = false;
                const vp = document.getElementById('svg-viewport');
                if (vp) vp.style.cursor = 'grab';
                const canvas = document.getElementById('svg-canvas');
                if (canvas) canvas.classList.remove('is-panning');
            }
            if (mmPanning) {
                mmPanning = false;
                const vp = document.getElementById('mermaid-viewport');
                if (vp) vp.style.cursor = 'grab';
                const canvas = document.getElementById('mermaid-canvas');
                if (canvas) canvas.classList.remove('is-panning');
            }
        });

        // ==================== INCREMENTAL UPDATE APIS ====================
        window.updateMarkdownContent = function(newHtml, statusText, rawDoc) {
            if (rawDoc !== undefined) window.__RAW_DOC_CONTENT__ = rawDoc;
            if (window.__isEditing) return;
            const c = document.getElementById('md-container');
            if (c) {
                c.innerHTML = newHtml;
                renderEnhancements();
                if (window.initMarkdownInPlaceEditing) window.initMarkdownInPlaceEditing();
            }
            if (statusText) setBadgeStatus(statusText);
            window.finishLoader(statusText);
        };

        window.updateJsonContent = function(rawJsonStr, statusText) {
            window.__RAW_JSON__ = rawJsonStr;
            window.__RAW_DOC_CONTENT__ = rawJsonStr;
            if (window.__isEditing) return;
            initJsonView();
            if (statusText) setBadgeStatus(statusText);
            window.finishLoader(statusText);
        };

        window.updateCsvContent = function(rows, statusText, rawCsv, delim) {
            window.__CSV_ROWS__ = rows;
            if (rawCsv !== undefined) window.__RAW_DOC_CONTENT__ = rawCsv;
            if (delim !== undefined && delim !== null) {
                window.__CSV_DELIM__ = delim;
                const pill = document.getElementById('csv-delim-pill');
                if (pill) {
                    let dName = 'CSV (逗号分隔)';
                    if (delim === '\t') dName = 'TSV (制表符分隔)';
                    else if (delim === ';') dName = 'DSV (分号分隔)';
                    else if (delim === '|') dName = 'DSV (竖线分隔)';
                    else if (delim !== ',') dName = 'DSV (\'' + delim + '\' 分隔)';
                    const valEl = pill.querySelector('.pill-val');
                    if (valEl) {
                        valEl.textContent = dName;
                    } else {
                        pill.textContent = dName;
                    }
                }
            }
            if (window.__isEditing) return;
            const filterEl = document.getElementById('csv-filter');
            renderCsvGrid(filterEl ? filterEl.value : '');
            if (statusText) setBadgeStatus(statusText);
            window.finishLoader(statusText);
        };

        window.updateSvgContent = function(svgStr, statusText) {
            window.__RAW_SVG__ = svgStr;
            window.__RAW_DOC_CONTENT__ = svgStr;
            if (window.__isEditing) return;
            const c = document.getElementById('svg-canvas');
            if (c) c.innerHTML = svgStr;
            initSvgView(true);
            if (window.initSvgInPlaceEditing) window.initSvgInPlaceEditing();
            if (statusText) setBadgeStatus(statusText);
            window.finishLoader(statusText);
        };

        window.updateYamlContent = function(yamlStr, statusText) {
            window.__RAW_YAML__ = yamlStr;
            window.__RAW_DOC_CONTENT__ = yamlStr;
            if (window.__isEditing) return;
            initYamlView();
            if (statusText) setBadgeStatus(statusText);
            window.finishLoader(statusText);
        };

        window.updateMermaidContent = function(mmStr, statusText) {
            window.__RAW_MERMAID__ = mmStr;
            window.__RAW_DOC_CONTENT__ = mmStr;
            if (window.__isEditing) return;
            const root = document.getElementById('mermaid-root');
            if (root) {
                root.removeAttribute('data-processed');
                root.textContent = mmStr;
                initMermaidView(false);
                if (window.initMermaidInPlaceEditing) window.initMermaidInPlaceEditing();
            }
            if (statusText) setBadgeStatus(statusText);
            window.finishLoader(statusText);
        };

        window.updateHtmlContent = function(htmlStr, statusText) {
            window.__RAW_HTML__ = htmlStr;
            window.__RAW_DOC_CONTENT__ = htmlStr;
            if (window.__isEditing) return;
            const iframe = document.getElementById('html-preview-frame');
            if (iframe) iframe.srcdoc = htmlStr;
            if (statusText) setBadgeStatus(statusText);
            window.finishLoader(statusText);
        };

        window.updateXmlContent = function(rawXmlStr, statusText) {
            window.__RAW_XML__ = rawXmlStr;
            window.__RAW_DOC_CONTENT__ = rawXmlStr;
            if (window.__isEditing) return;
            initXmlView();
            if (statusText) setBadgeStatus(statusText);
            window.finishLoader(statusText);
        };

        window.updateCodeContent = function(rawCodeStr, statusText, lang) {
            window.__RAW_CODE__ = rawCodeStr;
            window.__RAW_DOC_CONTENT__ = rawCodeStr;
            if (lang) window.__CODE_LANG__ = lang;
            if (window.__isEditing) return;

            const linesEl = document.getElementById('code-stat-lines');
            const charsEl = document.getElementById('code-stat-chars');
            const sizeEl = document.getElementById('code-stat-size');
            if (linesEl) linesEl.textContent = (rawCodeStr || '').split('\n').length;
            if (charsEl) charsEl.textContent = (rawCodeStr || '').length;
            if (sizeEl) {
                const kb = Math.round((rawCodeStr || '').length / 1024.0 * 10) / 10;
                sizeEl.textContent = kb > 0 ? kb + ' KB' : (rawCodeStr || '').length + ' B';
            }

            const container = document.getElementById('code-viewer-root');
            if (container && container.style.display !== 'none') {
                const effectiveLang = window.__CODE_LANG__ || 'plaintext';
                const pre = document.createElement('pre');
                const code = document.createElement('code');
                code.className = 'language-' + effectiveLang;
                code.textContent = rawCodeStr;
                pre.appendChild(code);
                container.innerHTML = '';
                container.appendChild(pre);
                renderEnhancements();
            }
            if (statusText) setBadgeStatus(statusText);
            window.finishLoader(statusText);
        };

        window.copyEntireCode = function() {
            const codeToCopy = window.__RAW_DOC_CONTENT__ || window.__RAW_CODE__ || '';
            copyTextToClipboard(codeToCopy);
        };

        window.syncScrollToLine = function(targetLine, totalLines) {
            if (targetLine <= 1) {
                window.scrollTo({ top: 0, behavior: 'auto' });
                return;
            }
            const codeEl = document.querySelector(`.code-gutter[data-line='${targetLine}']`);
            if (codeEl) {
                codeEl.scrollIntoView({ behavior: 'auto', block: 'center' });
                return;
            }
            const md = document.getElementById('md-container');
            if (md) {
                let targetEl = null;
                for (let l = targetLine; l >= Math.max(1, targetLine - 15); l--) {
                    targetEl = md.querySelector(`[data-sourcepos^='${l}:']`);
                    if (targetEl) break;
                }
                if (targetEl) {
                    targetEl.scrollIntoView({ behavior: 'auto', block: 'center' });
                    return;
                }
            }
            const ratio = targetLine / Math.max(1, totalLines);
            window.scrollTo(0, (document.documentElement.scrollHeight - window.innerHeight) * ratio);
        };

        // ==================== AUTO-HIDING FLOATING SCROLLBAR ====================
        let scrollHideTimer = null;
        function triggerScrollbarVisible() {
            document.documentElement.classList.add('scrolling');
            if (scrollHideTimer) clearTimeout(scrollHideTimer);
            scrollHideTimer = setTimeout(() => {
                document.documentElement.classList.remove('scrolling');
            }, 1000);
        }

        window.addEventListener('scroll', triggerScrollbarVisible, { passive: true });
        window.addEventListener('mousemove', (e) => {
            if (window.innerWidth - e.clientX <= 35) {
                triggerScrollbarVisible();
            }
        }, { passive: true });

        // ==================== POWERSHELL RICH VIEW CONTROLLER ====================
        function initPowerShellView() {
            if (!document.getElementById('ps-doc-view')) return;

            let savedMode = 'doc';
            try {
                savedMode = localStorage.getItem('npp_omniview_ps_view_mode') || 'doc';
            } catch(e) {}
            switchPsViewMode(savedMode, true);
        }

        window.switchPsViewMode = function(mode, skipToast) {
            const docView = document.getElementById('ps-doc-view');
            const codeView = document.getElementById('code-viewer-root');
            const btnDoc = document.getElementById('btn-ps-mode-doc');
            const btnCode = document.getElementById('btn-ps-mode-code');

            if (!docView || !codeView) return;

            if (mode === 'code') {
                docView.style.display = 'none';
                codeView.style.display = 'block';
                if (btnDoc) btnDoc.classList.remove('active');
                if (btnCode) btnCode.classList.add('active');
                if (!skipToast) showToast('已切换至: 源码浏览模式');
                if (window.enhanceCodeBlocks) enhanceCodeBlocks();
            } else {
                docView.style.display = 'flex';
                codeView.style.display = 'none';
                if (btnDoc) btnDoc.classList.add('active');
                if (btnCode) btnCode.classList.remove('active');
                if (!skipToast) showToast('已切换至: 帮助手册模式');
            }

            try {
                localStorage.setItem('npp_omniview_ps_view_mode', mode);
            } catch(e) {}
        };

        window.togglePsOutlineDrawer = function(force) {
            const drawer = document.getElementById('ps-outline-drawer');
            const backdrop = document.getElementById('ps-outline-backdrop');
            if (!drawer || !backdrop) return;

            const isOpen = drawer.classList.contains('active');
            const target = (typeof force === 'boolean') ? force : !isOpen;

            if (target) {
                drawer.classList.add('active');
                backdrop.classList.add('active');
                const searchInput = document.getElementById('ps-outline-search');
                if (searchInput) {
                    searchInput.value = '';
                    filterPsOutline('');
                    setTimeout(() => searchInput.focus(), 150);
                }
            } else {
                drawer.classList.remove('active');
                backdrop.classList.remove('active');
            }
        };

        window.filterPsOutline = function(query) {
            query = (query || '').toLowerCase().trim();
            const items = document.querySelectorAll('.outline-item');
            items.forEach(el => {
                const name = el.getAttribute('data-name') || '';
                if (!query || name.includes(query)) {
                    el.style.display = 'flex';
                } else {
                    el.style.display = 'none';
                }
            });
        };

        window.jumpToPsSymbol = function(targetId, lineNo) {
            togglePsOutlineDrawer(false);

            const docView = document.getElementById('ps-doc-view');
            const isDocVisible = (docView && docView.style.display !== 'none');

            if (isDocVisible) {
                const targetEl = document.getElementById(targetId);
                if (targetEl) {
                    targetEl.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    targetEl.classList.remove('anchor-highlight-pulse');
                    void targetEl.offsetWidth;
                    targetEl.classList.add('anchor-highlight-pulse');
                    return;
                }
            }

            if (lineNo) {
                jumpToPsFunctionInCode(lineNo);
            }
        };

        window.jumpToPsFunctionInCode = function(lineNo, funcName) {
            switchPsViewMode('code', true);
            togglePsOutlineDrawer(false);

            setTimeout(() => {
                const pre = document.querySelector('.code-viewer-container pre');
                if (!pre) return;
                const totalLines = (window.__RAW_CODE__ || '').split('\n').length || 1;
                const ratio = Math.max(0, Math.min(1, (lineNo - 1) / totalLines));
                pre.scrollTop = pre.scrollHeight * ratio;
                showToast('已定位至第 ' + lineNo + ' 行: ' + (funcName || ''));
            }, 80);
        };

        window.jumpToPsSecurity = function() {
            switchPsViewMode('doc', true);
            const sec = document.getElementById('ps-security-section');
            if (sec) {
                sec.scrollIntoView({ behavior: 'smooth', block: 'center' });
                sec.classList.remove('anchor-highlight-pulse');
                void sec.offsetWidth;
                sec.classList.add('anchor-highlight-pulse');
            }
        };

        window.copyPsSyntax = function() {
            const syntax = window.__PS_SYNTAX__ || '';
            if (syntax) {
                copyTextToClipboard(syntax, '已复制命令行语法');
            }
        };

        window.copyPsCommentTemplate = function() {
            const scriptName = window.__PS_SCRIPT_NAME__ || 'script.ps1';
            const tmpl = '<#\\n.SYNOPSIS\\n    在此处输入简短的脚本功能摘要\\n\\n.DESCRIPTION\\n    在此处详细说明脚本的业务逻辑、前置条件与执行流程\\n\\n.PARAMETER TargetPath\\n    目标路径或文件夹地址\\n\\n.PARAMETER Force\\n    是否强制执行并跳过确认提示\\n\\n.EXAMPLE\\n    .\\' + scriptName + ' -TargetPath C:\\\\Data -Force\\n    执行示例说明与预期效果\\n\\n.OUTPUTS\\n    [System.Boolean] 执行状态返回值\\n#>';
            copyTextToClipboard(tmpl, '已复制标准 Comment-Based Help 注释模板');
        };

        window.addEventListener('DOMContentLoaded', () => {
            initJsonView();
            initXmlView();
            renderCsvGrid();
            let savedSvgColor = null, savedSvgName = null;
            try {
                savedSvgColor = localStorage.getItem('npp_omniview_svg_bg_color');
                savedSvgName = localStorage.getItem('npp_omniview_svg_bg_name');
            } catch(e){}
            if (savedSvgColor !== null) {
                setCanvasBg('svg', savedSvgColor, savedSvgName);
            } else {
                setCanvasBg('svg', 'none', '不显示');
            }
            initSvgView();
            initYamlView();
            let savedMmColor = null, savedMmName = null;
            try {
                savedMmColor = localStorage.getItem('npp_omniview_mermaid_bg_color');
                savedMmName = localStorage.getItem('npp_omniview_mermaid_bg_name');
            } catch(e){}
            if (savedMmColor !== null) {
                setCanvasBg('mermaid', savedMmColor, savedMmName);
            } else {
                const isDark = document.body.classList.contains('dark-mode');
                setCanvasBg('mermaid', isDark ? '#121214' : '#ffffff', isDark ? '暗黑' : '白色');
            }
            initMermaidView(true);
            initHtmlView();
            initPowerShellView();
            if (window.initMarkdownInPlaceEditing) window.initMarkdownInPlaceEditing();
            if (window.initSvgInPlaceEditing) window.initSvgInPlaceEditing();
            if (window.initMermaidInPlaceEditing) window.initMermaidInPlaceEditing();
            renderEnhancements();
            window.finishLoader();
        });
    </script>
</body>
</html>");

            return sb.ToString();
        }
    }
}
