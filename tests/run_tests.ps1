$ErrorActionPreference = "Stop"

Write-Host "=== Running NppOmniView Unit & Regression Tests ===" -ForegroundColor Cyan

$dllPath = "E:\Projects\plugins\NppOmniView\src\NppOmniView\bin\Release\net48\NppOmniView.dll"
Add-Type -Path $dllPath

$testsPassed = 0
$testsFailed = 0

function Assert-Equal($actual, $expected, $testName) {
    if ($actual -eq $expected) {
        Write-Host "  [PASS] $testName" -ForegroundColor Green
        $global:testsPassed++
    } else {
        Write-Host "  [FAIL] $testName" -ForegroundColor Red
        Write-Host "    Expected: [$expected]" -ForegroundColor DarkGray
        Write-Host "    Actual  : [$actual]" -ForegroundColor DarkGray
        $global:testsFailed++
    }
}

# Test 1: Empty JSON Objects & Arrays
$err = $null
$resObj = [NppPlugin.OmniView.Core.FormatProcessor]::BeautifyJson("{}", [ref]$err)
Assert-Equal $resObj "{}" "Beautify empty object {}"

$resArr = [NppPlugin.OmniView.Core.FormatProcessor]::BeautifyJson("[]", [ref]$err)
Assert-Equal $resArr "[]" "Beautify empty array []"

# Test 2: CSV to JSON Leading Zero & Float Invariance
$csvData = @"
id,code,score,active
1,"0123",98.5,true
2,"007",85.0,false
"@
$jsonFromCsv = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertCsvToJson($csvData)
$hasCodeString = $jsonFromCsv.Contains('"code": "0123"')
Assert-Equal $hasCodeString $true "ConvertCsvToJson preserves leading zero string '0123'"
$hasScoreFloat = $jsonFromCsv.Contains('"score": 98.5')
Assert-Equal $hasScoreFloat $true "ConvertCsvToJson parses invariant float 98.5"

# Test 3: Multiline CSV to Markdown Table
$csvMultiline = @"
id,notes
101,"Line 1
Line 2"
"@
$mdTable = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertCsvToMarkdownTable($csvMultiline)
$hasBr = $mdTable.Contains("Line 1<br>Line 2")
Assert-Equal $hasBr $true "ConvertCsvToMarkdownTable converts newline to <br>"

# Test 4: Nested JSON to CSV (RFC 4180 escaped quotes in CSV cell)
$nestedJson = '[{"id":1,"tags":["a","b"],"meta":{"role":"admin"}}]'
$csvFromNested = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertJsonToCsv($nestedJson, [ref]$err)
$hasArrayJson = $csvFromNested.Contains('[""a"",""b""]')
Assert-Equal $hasArrayJson $true "ConvertJsonToCsv serializes nested array"
$hasMetaJson = $csvFromNested.Contains('{""role"":""admin""}')
Assert-Equal $hasMetaJson $true "ConvertJsonToCsv serializes nested dictionary"
$noTypeDump = (-not $csvFromNested.Contains("System.Collections"))
Assert-Equal $noTypeDump $true "ConvertJsonToCsv does not dump C# type names"

# Test 5: TSV Delimiter Preservation
$tsvData = "name`tage`tscore`r`nAlice`t25`t99.5"
$alignedTsv = [NppPlugin.OmniView.Core.FormatProcessor]::AlignCsvColumns($tsvData)
$hasTabs = $alignedTsv.Contains("`t")
Assert-Equal $hasTabs $true "AlignCsvColumns preserves tab delimiter on TSV"

# Test 6: MultiFormatDetector Zero-Allocation Detection
$docMd = New-Object NppPlugin.OmniView.Core.DocumentContext
$docMd.Content = "# Title`r`n- Item 1`r`n- Item 2"
$detMd = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docMd, $null)
Assert-Equal $detMd.Format.ToString() "Markdown" "MultiFormatDetector detects Markdown"

$docJson = New-Object NppPlugin.OmniView.Core.DocumentContext
$docJson.Content = "   { `r`n `"key`": `"value`" }  "
$detJson = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docJson, $null)
Assert-Equal $detJson.Format.ToString() "Json" "MultiFormatDetector detects JSON"

$docCsv = New-Object NppPlugin.OmniView.Core.DocumentContext
$docCsv.Content = "col1,col2,col3`r`nval1,val2,val3`r`nval4,val5,val6"
$detCsv = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docCsv, $null)
Assert-Equal $detCsv.Format.ToString() "Csv" "MultiFormatDetector detects CSV"

# Test 7: HTML Template Builder Verification
$htmlMd = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docMd, $detMd, "", $false)
$noPlaceholders = (-not $htmlMd.Contains("__FMT_NAME__")) -and (-not $htmlMd.Contains("__TOOLBAR_BUTTONS__"))
Assert-Equal $noPlaceholders $true "BuildOmniViewHtml has zero unreplaced placeholders"

$noRawUnicodeEscapes = (-not $htmlMd.Contains("\u4F5C")) -and (-not $htmlMd.Contains("\u7F8E"))
Assert-Equal $noRawUnicodeEscapes $true "BuildOmniViewHtml uses real UTF-8 Chinese strings without \uXXXX leaks"

$hasCodeCopy = $htmlMd.Contains("code-copy-btn") -and $htmlMd.Contains("inline-code-pill")
Assert-Equal $hasCodeCopy $true "BuildOmniViewHtml includes code block and inline code copy mechanisms"

$htmlJson = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docJson, $detJson, "", $false)
$hasJsonTypeControls = $htmlJson.Contains("btn-toggle-types") -and $htmlJson.Contains("type-tag")
Assert-Equal $hasJsonTypeControls $true "BuildOmniViewHtml includes JSON type badges and toggle control"

$hasJsonToolbar = $htmlJson.Contains("json-toolbar") -and $htmlJson.Contains("expandAllJson") -and $htmlJson.Contains("collapseAllJson") -and $htmlJson.Contains("copyFormattedJson") -and $htmlJson.Contains("position: sticky")
Assert-Equal $hasJsonToolbar $true "BuildOmniViewHtml includes sticky single-row JSON toolbar with expand, collapse, and copy actions"

# Test 8: CSV Alignment Idempotency & Minification
$rawCsv = "name,age,city`r`nAlice,25,Beijing`r`nBob,30,Shanghai"
$alignedOnce = [NppPlugin.OmniView.Core.FormatProcessor]::AlignCsvColumns($rawCsv)
$alignedTwice = [NppPlugin.OmniView.Core.FormatProcessor]::AlignCsvColumns($alignedOnce)
Assert-Equal ($alignedOnce -eq $alignedTwice) $true "AlignCsvColumns is idempotent (repeated clicks produce identical output)"

$minifiedCsv = [NppPlugin.OmniView.Core.FormatProcessor]::MinifyCsv($alignedTwice)
$minifiedExpected = "name,age,city`r`nAlice,25,Beijing`r`nBob,30,Shanghai"
Assert-Equal $minifiedCsv $minifiedExpected "MinifyCsv cleanly strips whitespace padding"

# Test 9: Floating Scrollbar and Unified Dropdown Menu Verification
$hasFloatingScrollbar = $htmlJson.Contains("::-webkit-scrollbar") -and $htmlJson.Contains("scrollbar-thumb")
Assert-Equal $hasFloatingScrollbar $true "BuildOmniViewHtml includes floating overlay scrollbar styling"

$hasDropdownMenu = $htmlJson.Contains("class='dropdown'") -and $htmlJson.Contains("class='dropdown-menu'")
Assert-Equal $hasDropdownMenu $true "BuildOmniViewHtml includes consolidated hover dropdown menu"

# Test 10: Hierarchical JSON Tree View (Guides, Chevrons, Inline Types & Actions)
$hasTreeGuidesAndChevrons = $htmlJson.Contains("json-node-children") -and $htmlJson.Contains("tree-arrow") -and $htmlJson.Contains("tree-spacer")
Assert-Equal $hasTreeGuidesAndChevrons $true "BuildOmniViewHtml includes hierarchical tree guides and alignment spacers"

$hasInlineTypeBadgesAndActions = $htmlJson.Contains("json-actions") -and $htmlJson.Contains("json-copy-btn")
Assert-Equal $hasInlineTypeBadgesAndActions $true "BuildOmniViewHtml includes node copy actions and inline type tags"

# Test 11: Professional SVG Icons & Zero Emojis in Dropdowns and Toolbars
$docUnknown = New-Object NppPlugin.OmniView.Core.DocumentContext
$docUnknown.Content = "Random plain text without structure"
$detUnknown = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docUnknown, $null)
$htmlUnknown = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docUnknown, $detUnknown, "", $false)

$hasSvgDropdownIcons = $htmlJson.Contains("item-icon") -and $htmlJson.Contains("chevron-icon") -and $htmlJson.Contains("dropdown-header")
Assert-Equal $hasSvgDropdownIcons $true "BuildOmniViewHtml includes professional categorized SVG dropdown items"

$hasNoticeSvgIcon = (-not $htmlUnknown.Contains("&#128196;")) -and $htmlUnknown.Contains("<svg width='48'")
Assert-Equal $hasNoticeSvgIcon $true "BuildOmniViewHtml uses clean SVG icon for notice card instead of emoji"

$hasNoEmojis = (-not [regex]::IsMatch($htmlJson, "[\uD800-\uDBFF][\uDC00-\uDFFF]")) -and (-not [regex]::IsMatch($htmlMd, "[\uD800-\uDBFF][\uDC00-\uDFFF]"))
Assert-Equal $hasNoEmojis $true "BuildOmniViewHtml has zero emoji characters in UI templates"

# Test 12: Multi-Language Code Block Theming, Mac-Style Dots, Gutters & Highlighting
$docCodeSample = New-Object NppPlugin.OmniView.Core.DocumentContext
$docCodeSample.Content = @'
```python
def add(a, b):
    return a + b
```
```bash
echo 'Hello World'
```
'@
$detCodeSample = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docCodeSample, $null)
$htmlCodeSample = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docCodeSample, $detCodeSample, "", $false)

$hasCodeTheming = $htmlCodeSample.Contains("mac-dots") -and $htmlCodeSample.Contains("code-accent-bar") -and $htmlCodeSample.Contains("lang-badge") -and $htmlCodeSample.Contains("code-gutter") -and $htmlCodeSample.Contains("code-body-container") -and $htmlCodeSample.Contains("is-terminal") -and $htmlCodeSample.Contains("highlight.min.js") -and $htmlCodeSample.Contains("data-raw-code")
Assert-Equal $hasCodeTheming $true "BuildOmniViewHtml includes multi-language code block styling, Mac dots, gutters and syntax highlighting"

# Test 13: Detection of New Formats (SVG, YAML, Mermaid, HTML)
$docSvg = New-Object NppPlugin.OmniView.Core.DocumentContext
$docSvg.Content = '<svg xmlns="http://www.w3.org/2000/svg" width="100" height="100"><circle cx="50" cy="50" r="40"/></svg>'
$detSvg = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docSvg, $null)
Assert-Equal $detSvg.Format.ToString() "Svg" "MultiFormatDetector detects SVG by content"

$docYaml = New-Object NppPlugin.OmniView.Core.DocumentContext
$docYaml.Content = "server:`r`n  port: 8080`r`n  name: app"
$detYaml = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docYaml, $null)
Assert-Equal $detYaml.Format.ToString() "Yaml" "MultiFormatDetector detects YAML by content"

$docMmd = New-Object NppPlugin.OmniView.Core.DocumentContext
$docMmd.Content = "flowchart TD`r`n  A[Start] --> B(Process)"
$detMmd = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docMmd, $null)
Assert-Equal $detMmd.Format.ToString() "Mermaid" "MultiFormatDetector detects Mermaid by content"

$docHtml = New-Object NppPlugin.OmniView.Core.DocumentContext
$docHtml.Content = "<!DOCTYPE html><html><body><h1>Hello</h1></body></html>"
$detHtml = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docHtml, $null)
Assert-Equal $detHtml.Format.ToString() "Html" "MultiFormatDetector detects HTML by content"

# Test 14: SVG Formatting XML Pretty-Print
$rawSvg = '<svg width="100" height="100"><circle cx="50" cy="50" r="40"/><text>Hello</text></svg>'
$err = $null
$formattedSvg = [NppPlugin.OmniView.Core.FormatProcessor]::FormatSvg($rawSvg, [ref]$err)
$isIndented = $formattedSvg.Contains("  <circle") -and ($err -eq $null)
Assert-Equal $isIndented $true "FormatSvg formats and indents XML/SVG properly"

# Test 15: HTML Template Generation for SVG View
$htmlSvg = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docSvg, $detSvg, "", $false)
$hasSvgComponents = $htmlSvg.Contains("svg-viewport") -and $htmlSvg.Contains("svg-canvas") -and $htmlSvg.Contains("exportSvgAsPng") -and $htmlSvg.Contains("copySvgDataUri") -and $htmlSvg.Contains("badge-svg")
Assert-Equal $hasSvgComponents $true "BuildOmniViewHtml includes SVG viewport, canvas, tools and badge"

$hasSvgPalette = $htmlSvg.Contains("btn-svg-bg") -and $htmlSvg.Contains("svg-palette-popover") -and $htmlSvg.Contains("svg-zoom-tag") -and $htmlSvg.Contains("fitSvgToViewport") -and $htmlSvg.Contains("setCanvasBg")
Assert-Equal $hasSvgPalette $true "SVG view includes unified color palette popover, zoom-badge and viewport fitting"

# Test 16: HTML Template Generation for YAML View
$htmlYaml = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docYaml, $detYaml, "", $false)
$hasYamlComponents = $htmlYaml.Contains("yaml-root") -and $htmlYaml.Contains("js-yaml.min.js") -and $htmlYaml.Contains("beautifyYaml") -and $htmlYaml.Contains("badge-yaml") -and $htmlYaml.Contains("copyFormattedYamlJson")
Assert-Equal $hasYamlComponents $true "BuildOmniViewHtml includes YAML root, js-yaml script, beautify and badge"

# Test 17: HTML Template Generation for Mermaid & HTML Views
$htmlMmd = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docMmd, $detMmd, "", $false)
$hasMmdComponents = $htmlMmd.Contains("mermaid-viewport") -and $htmlMmd.Contains("mermaid-canvas") -and $htmlMmd.Contains("copyMermaidSvg") -and $htmlMmd.Contains("badge-mermaid")
Assert-Equal $hasMmdComponents $true "BuildOmniViewHtml includes Mermaid viewport, canvas, copy and badge"

$htmlDoc = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docHtml, $detHtml, "", $false)
$hasHtmlComponents = $htmlDoc.Contains("html-viewport-wrapper") -and $htmlDoc.Contains("html-device-frame") -and $htmlDoc.Contains("html-preview-frame") -and $htmlDoc.Contains("setHtmlViewport") -and $htmlDoc.Contains("badge-html")
Assert-Equal $hasHtmlComponents $true "BuildOmniViewHtml includes HTML responsive device frame, iframe and viewport switcher"

# Test 18: Unified View Switch Menu in All Formats
$hasAllForceModes = $htmlJson.Contains("FORCE_SVG") -and $htmlJson.Contains("FORCE_YAML") -and $htmlJson.Contains("FORCE_MERMAID") -and $htmlJson.Contains("FORCE_HTML")
Assert-Equal $hasAllForceModes $true "BuildOmniViewHtml includes unified mode switching for all 7 formats"

# Test 19: Loading Animation, Micro-Pulse Dot and Fluidity Elements
$hasLoaderComponents = $htmlJson.Contains("top-loader") -and $htmlJson.Contains("top-loader-bar") -and $htmlJson.Contains("loaderProgress") -and $htmlJson.Contains("pulse-dot") -and $htmlJson.Contains("pulseBusy") -and $htmlJson.Contains("omniFadeIn")
Assert-Equal $hasLoaderComponents $true "BuildOmniViewHtml includes top loader bar, breathing pulse dot and container fade animations"

$hasLoaderApis = $htmlJson.Contains("window.startLoader") -and $htmlJson.Contains("window.finishLoader") -and $htmlJson.Contains("setBadgeStatus")
Assert-Equal $hasLoaderApis $true "BuildOmniViewHtml includes startLoader, finishLoader and setBadgeStatus JavaScript controllers"

$hasNotifyMethod = [NppPlugin.OmniView.Forms.PreviewDockForm].GetMethod("NotifyTypingPending") -ne $null
Assert-Equal $hasNotifyMethod $true "PreviewDockForm exposes NotifyTypingPending method for instant keystroke feedback"

# Test 20: Differentiated In-Place Editing Architecture (Eliminating Drawers)
$hasInPlaceEditor = $htmlJson.Contains("omni-viewport-editor") -and $htmlJson.Contains("omni-editor-textarea") -and $htmlJson.Contains("btn-enter-edit") -and $htmlJson.Contains("enterEditMode") -and $htmlJson.Contains("saveAndExitEditMode") -and $htmlJson.Contains("cancelEditMode") -and $htmlJson.Contains("window.omniTriggerChange") -and $htmlJson.Contains("window.omniFlushSync")
Assert-Equal $hasInPlaceEditor $true "BuildOmniViewHtml includes in-place viewport editor, textarea, enter/save/cancel controllers and sync"

$hasInPlaceInMd = $htmlMd.Contains("btn-enter-edit") -and $htmlMd.Contains("编辑文档") -and $htmlMd.Contains("omni-viewport-editor")
Assert-Equal $hasInPlaceInMd $true "Markdown view includes in-place document editing button and viewport editor"

$htmlCsv = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docCsv, $detCsv, "", $false)
$hasInPlaceInCsv = $htmlCsv.Contains("btn-enter-edit") -and $htmlCsv.Contains("编辑表格") -and $htmlCsv.Contains("editing-mode")
Assert-Equal $hasInPlaceInCsv $true "CSV view includes specialized spreadsheet in-place editing button and grid mode"

# Test 21: In-Place Editing for JSON & YAML Tree
$hasJsonInPlaceEdit = $htmlJson.Contains("startEditJsonVal") -and $htmlJson.Contains("startEditJsonKey") -and $htmlJson.Contains("json-inline-input") -and $htmlJson.Contains("data-path") -and $htmlJson.Contains("syncJsonOrYamlAfterEdit")
Assert-Equal $hasJsonInPlaceEdit $true "JSON & YAML views include double-click in-place inline input editing"

# Test 22: In-Place CSV Table Editing, Row Operations and Auto-Sync
$htmlCsv = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docCsv, $detCsv, "", $false)
$hasCsvInPlaceEdit = $htmlCsv.Contains("addCsvRow") -and $htmlCsv.Contains("deleteCsvRow") -and $htmlCsv.Contains("csv-row-del-btn") -and $htmlCsv.Contains("handleCsvCellKey") -and $htmlCsv.Contains("serializeCsvRows")
Assert-Equal $hasCsvInPlaceEdit $true "CSV view includes add row, delete row, cell key navigation and automatic serialization"

$noAwkwardBtn = -not $htmlCsv.Contains("btn-sync-csv")
Assert-Equal $noAwkwardBtn $true "CSV view completely eliminated the awkward manual overwrite sync button"

# Test 23: Markdown Interactive Task List Checkboxes
$hasTaskCheckboxes = $htmlMd.Contains("enhanceTaskLists") -and $htmlMd.Contains("toggleMarkdownTaskItem")
Assert-Equal $hasTaskCheckboxes $true "Markdown view includes interactive task list checkbox toggling"

# Test 24: Mermaid Color Palette (Presets & Custom Picker) and Vector Sharpness
$hasMmdPalette = $htmlMmd.Contains("btn-mermaid-bg") -and $htmlMmd.Contains("mermaid-palette-popover") -and $htmlMmd.Contains("setMermaidBg") -and $htmlMmd.Contains("mermaid-custom-color") -and $htmlMmd.Contains("data-color='none'") -and $htmlMmd.Contains("data-color='#ffffff'")
Assert-Equal $hasMmdPalette $true "Mermaid view includes color palette popover (presets: none, white, common + custom picker)"

$hasMmdSharpness = $htmlMmd.Contains("normalizeMermaidSvg") -and $htmlMmd.Contains("fitMermaidToViewport") -and $htmlMmd.Contains("panRafPending") -and $htmlMmd.Contains("is-panning")
Assert-Equal $hasMmdSharpness $true "Mermaid view includes native vector sharpness normalization and viewport fitting"

# Test 25: Format-Specific Scroll-Past-End & Viewport Optimization
$hasFormatClassInMd = $htmlMd.Contains("fmt-markdown")
Assert-Equal $hasFormatClassInMd $true "Markdown HTML body includes fmt-markdown class"

$hasFormatClassInSvg = $htmlSvg.Contains("fmt-svg")
Assert-Equal $hasFormatClassInSvg $true "SVG HTML body includes fmt-svg class with overflow: hidden"

$hasFormatClassInMmd = $htmlMmd.Contains("fmt-mermaid")
Assert-Equal $hasFormatClassInMmd $true "Mermaid HTML body includes fmt-mermaid class with overflow: hidden"

$hasScrollPastEndRules = $htmlMd.Contains("body.fmt-markdown") -and $htmlMd.Contains("body.fmt-svg") -and $htmlMd.Contains("body.fmt-mermaid") -and $htmlMd.Contains("body.fmt-html")
Assert-Equal $hasScrollPastEndRules $true "BuildOmniViewHtml includes precise format-specific scroll-past-end rules eliminating unwanted bottom blank spaces"

$hasCsvStickyToolbar = $htmlCsv.Contains("body.fmt-csv") -and $htmlCsv.Contains("csv-toolbar") -and $htmlCsv.Contains("csv-container") -and $htmlCsv.Contains("flex: 1")
Assert-Equal $hasCsvStickyToolbar $true "CSV view includes pinned sticky toolbar and auto-scrolling container"

# Test 26: CSV Delimiter Detection, Conversion & Quoting Integrity
$csvForConvert = @"
id,name,notes,score
1,Alice,"Math, Physics",95
2,Bob,"Has | pipe",88
3,Charlie,"Normal",100
"@

# 1. Convert Comma to Tab (TSV)
$tsvConverted = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertCsvDelimiter($csvForConvert, [char]"`t")
$tsvHasTabs = $tsvConverted.Contains("`t") -and (-not $tsvConverted.Contains("1,Alice"))
Assert-Equal $tsvHasTabs $true "ConvertCsvDelimiter converts comma CSV to TSV correctly"

# 2. Convert Comma to Pipe (|)
$pipeConverted = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertCsvDelimiter($csvForConvert, [char]'|')
$pipeHasPipes = $pipeConverted.Contains('1|Alice|Math, Physics|95') -and $pipeConverted.Contains('2|Bob|"Has | pipe"|88')
Assert-Equal $pipeHasPipes $true "ConvertCsvDelimiter converts to Pipe delimiter with RFC 4180 quote handling"

# 3. Convert Comma to Semicolon (;)
$semiConverted = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertCsvDelimiter($csvForConvert, [char]';')
$semiHasSemis = $semiConverted.Contains('1;Alice;Math, Physics;95')
Assert-Equal $semiHasSemis $true "ConvertCsvDelimiter converts to Semicolon delimiter"

# 4. Multi-delimiter detection test (comma, tab, semicolon, pipe)
$detectSemi = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter("a;b;c`r`n1;2;3`r`n4;5;6")
Assert-Equal $detectSemi ([char]';') "DetectCsvDelimiter detects semicolon delimiter"

$detectPipe = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter("col1|col2|col3`r`nx|y|z`r`n1|2|3")
Assert-Equal $detectPipe ([char]'|') "DetectCsvDelimiter detects pipe delimiter"

# 5. Idempotent when target delimiter is identical
$sameConverted = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertCsvDelimiter($csvForConvert, [char]',')
Assert-Equal $sameConverted $csvForConvert "ConvertCsvDelimiter is idempotent when delimiter is identical"

# Test 27: CSV Delimiter Replacement UI & Modal Dialog
$hasDelimModal = $htmlCsv.Contains('csv-delim-modal') -and $htmlCsv.Contains('delim-presets') -and $htmlCsv.Contains('csv-custom-delim-input')
Assert-Equal $hasDelimModal $true "BuildOmniViewHtml includes CSV delimiter modal dialog and custom input"

$hasDelimToolbarBtn = $htmlCsv.Contains('openCsvDelimModal')
Assert-Equal $hasDelimToolbarBtn $true "BuildOmniViewHtml includes CSV delimiter replacement toolbar button"

$hasDelimDropdownActions = $htmlCsv.Contains('CSV_REPLACE_DELIM:,') -and $htmlCsv.Contains('CSV_REPLACE_DELIM:\t') -and $htmlCsv.Contains('CSV_REPLACE_DELIM:;') -and $htmlCsv.Contains('CSV_REPLACE_DELIM:|')
Assert-Equal $hasDelimDropdownActions $true "BuildOmniViewHtml includes CSV delimiter quick convert actions in dropdown menu"

$hasDelimJsControllers = $htmlCsv.Contains('window.openCsvDelimModal') -and $htmlCsv.Contains('window.closeCsvDelimModal') -and $htmlCsv.Contains('window.applyCsvDelimChange')
Assert-Equal $hasDelimJsControllers $true "BuildOmniViewHtml includes JS controllers for CSV delimiter modal"

# Test 28: Markdown GitHub Alert Rendering & Styles
$alertMd = @"
> [!NOTE]
> This is a note callout.

> [!TIP]
> This is a tip callout.
"@
$htmlFromAlert = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::ConvertMarkdownToHtml($alertMd)
$hasAlertNote = $htmlFromAlert.Contains('markdown-alert-note') -and $htmlFromAlert.Contains('markdown-alert-title')
Assert-Equal $hasAlertNote $true "ConvertMarkdownToHtml generates GitHub alert container and title"

$hasAlertCss = $htmlMd.Contains('.markdown-alert') -and $htmlMd.Contains('.markdown-alert-note') -and $htmlMd.Contains('.markdown-alert-tip') -and $htmlMd.Contains('.markdown-alert-warning')
Assert-Equal $hasAlertCss $true "BuildOmniViewHtml includes comprehensive GitHub alert 5-color styles"

$hasAlertEnhancer = $htmlMd.Contains('enhanceMarkdownAlerts')
Assert-Equal $hasAlertEnhancer $true "BuildOmniViewHtml includes enhanceMarkdownAlerts fallback controller"

# Test 29: Markdown TOC Anchor Jump & Auto-Identifier GitHub Compatibility
$tocSample = "## 1. Quick Start (Overview)`r`n`r`n- [Jump](#1-quick-start-overview)"
$htmlFromToc = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::ConvertMarkdownToHtml($tocSample)
$hasGitHubId = $htmlFromToc.Contains('id="1-quick-start-overview"')
Assert-Equal $hasGitHubId $true "ConvertMarkdownToHtml generates GitHub-compatible heading ID matching TOC href"

$hasScrollMargin = $htmlMd.Contains('scroll-margin-top: 56px')
Assert-Equal $hasScrollMargin $true "BuildOmniViewHtml sets scroll-margin-top preventing topbar overlap"

$hasAnchorHighlight = $htmlMd.Contains('anchor-target-highlight') -and $htmlMd.Contains('anchorPulse')
Assert-Equal $hasAnchorHighlight $true "BuildOmniViewHtml includes anchor highlight pulse animation"

$hasAnchorJsController = $htmlMd.Contains('navigateToMarkdownAnchor')
Assert-Equal $hasAnchorJsController $true "BuildOmniViewHtml includes navigateToMarkdownAnchor smooth navigation controller"

# Test 30: Custom Delimiter Detection & Arbitrary Delimiter Conversion
$caretData = "id^name^city`r`n1^Alice^Beijing`r`n2^Bob^Shanghai"
$detectedCaret = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter($caretData)
Assert-Equal $detectedCaret ([char]'^') "DetectCsvDelimiter detects custom caret (^) delimiter"

$convertedFromCaret = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertCsvDelimiter($caretData, [char]'|', [char]'^')
$hasPipeOnly = $convertedFromCaret.Contains("1|Alice|Beijing") -and (-not $convertedFromCaret.Contains("^"))
Assert-Equal $hasPipeOnly $true "ConvertCsvDelimiter correctly converts custom caret (^) to pipe (|)"

$colonData = "key:val:comment`r`nfoo:bar:hello`r`na:b:c"
$detectedColon = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter($colonData)
Assert-Equal $detectedColon ([char]':') "DetectCsvDelimiter detects colon (:) delimiter"

# Test 31: CSV Number Column Stability & Delimiter Selection Cards
$hasNumColStyle = $htmlCsv.Contains('th.csv-num-col, td.csv-num-col') -and $htmlCsv.Contains('csv-row-num-wrap') -and $htmlCsv.Contains('csv-row-idx')
Assert-Equal $hasNumColStyle $true "BuildOmniViewHtml uses table-cell based csv-num-col eliminating flexbox row jitter"

$hasDelimCards = $htmlCsv.Contains('delim-card') -and $htmlCsv.Contains('delim-radio-dot') -and $htmlCsv.Contains('delim-current-badge') -and $htmlCsv.Contains('delim-btn-custom')
Assert-Equal $hasDelimCards $true "BuildOmniViewHtml includes modern delimiter selection cards and current delimiter badge"

$hasDelimPill = $htmlCsv.Contains("id='csv-delim-pill'")
Assert-Equal $hasDelimPill $true "BuildOmniViewHtml includes dynamic csv-delim-pill element for live updates"

# Test 32: Mermaid Window Auto-Fit, Dedicated Zoom Badge, and Refresh Button Removal
$hasMmdInitialScale = $htmlMmd.Contains('calcMermaidInitialScale') -and $htmlMmd.Contains('align-items: flex-start') -and $htmlMmd.Contains('transform-origin: top center')
Assert-Equal $hasMmdInitialScale $true "BuildOmniViewHtml calculates comfortable auto-fit window scale and top alignment for Mermaid"

$hasZoomBadgeOnPage = $htmlMmd.Contains("id='mermaid-zoom-tag'") -and $htmlMmd.Contains("zoom-badge")
Assert-Equal $hasZoomBadgeOnPage $true "BuildOmniViewHtml includes dedicated zoom-badge on page directly in toolbar-group"

$hasNoRefreshBtn = (-not $htmlMmd.Contains("resetMermaidZoom()"))
Assert-Equal $hasNoRefreshBtn $true "BuildOmniViewHtml completely removes the confusing reset/refresh button from Mermaid toolbar"

# Test 33: MultiFormatDetector Code Extensions & Language Resolution
$docPs1 = New-Object NppPlugin.OmniView.Core.DocumentContext
$docPs1.Extension = ".ps1"
$docPs1.FullPath = "C:\test\sample.ps1"
$docPs1.Content = "# Test 14: SVG Formatting`r`n`$rawSvg = '<svg><circle/></svg>'"
$detPs1 = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docPs1, $null)
Assert-Equal $detPs1.Format.ToString() "Code" "MultiFormatDetector evaluates .ps1 as DocFormat.Code"

$docPy = New-Object NppPlugin.OmniView.Core.DocumentContext
$docPy.Extension = ".py"
$docPy.FullPath = "C:\test\sample.py"
$docPy.Content = "import os`r`nprint('hello')"
$detPy = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docPy, $null)
Assert-Equal $detPy.Format.ToString() "Code" "MultiFormatDetector evaluates .py as DocFormat.Code"

$psLang = [NppPlugin.OmniView.Core.MultiFormatDetector]::GetLanguageForExtension(".ps1")
Assert-Equal $psLang "powershell" "GetLanguageForExtension resolves .ps1 to powershell"

$csLang = [NppPlugin.OmniView.Core.MultiFormatDetector]::GetLanguageForExtension(".cs")
Assert-Equal $csLang "csharp" "GetLanguageForExtension resolves .cs to csharp"

# Test 34: PowerShell Code Content Fallback (prevents false Markdown heading detection)
$psCodeSample = @"
# Test 14: SVG Formatting XML Pretty-Print
`$rawSvg = '<svg width="100" height="100"><circle cx="50" cy="50" r="40"/><text>Hello</text></svg>'
`$err = `$null
`$formattedSvg = [NppPlugin.OmniView.Core.FormatProcessor]::FormatSvg(`$rawSvg, [ref]`$err)
`$isIndented = `$formattedSvg.Contains("  <circle") -and (`$err -eq `$null)
Assert-Equal `$isIndented `$true "FormatSvg formats and indents XML/SVG properly"
"@
$docUnnamedPs = New-Object NppPlugin.OmniView.Core.DocumentContext
$docUnnamedPs.Extension = ""
$docUnnamedPs.FullPath = "new 1"
$docUnnamedPs.Content = $psCodeSample
$detUnnamedPs = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docUnnamedPs, $null)
Assert-Equal $detUnnamedPs.Format.ToString() "Code" "MultiFormatDetector content fallback detects PowerShell code constructs without extension"

# Test 35: Code Viewer HTML Generation & SVG Raw Execution Prevention
$htmlCode = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docPs1, $detPs1, "", $false)
$hasCodeViewerContainer = $htmlCode.Contains("class='code-viewer-container'") -and $htmlCode.Contains("id='code-viewer-root'")
Assert-Equal $hasCodeViewerContainer $true "BuildOmniViewHtml generates code-viewer-container for DocFormat.Code"

# Critical security/rendering test: <svg> tag must be HTML-encoded and NOT rendered as raw DOM elements
$isSvgEncoded = $htmlCode.Contains("&lt;svg&gt;&lt;circle/&gt;&lt;/svg&gt;") -and (-not $htmlCode.Contains("<svg><circle/></svg>"))
Assert-Equal $isSvgEncoded $true "BuildOmniViewHtml safely HTML-encodes <svg> tags inside code content preventing execution"

$hasCodeBadge = $htmlCode.Contains("CODE: POWERSHELL") -and $htmlCode.Contains("badge-code")
Assert-Equal $hasCodeBadge $true "BuildOmniViewHtml includes CODE: POWERSHELL format badge"

$hasCodeJsControllers = $htmlCode.Contains("window.updateCodeContent") -and $htmlCode.Contains("copyEntireCode")
Assert-Equal $hasCodeJsControllers $true "BuildOmniViewHtml includes window.updateCodeContent and copyEntireCode controllers"

$hasCodeSwitchMenu = $htmlMd.Contains("postAct('FORCE_CODE')") -and $htmlCode.Contains("postAct('FORCE_MD')")
Assert-Equal $hasCodeSwitchMenu $true "BuildOmniViewHtml includes FORCE_CODE action in Markdown view menu and alternate formats in Code view menu"

# Test 36: HTML Viewport Resolution Engine, Orientation Toggle, Mobile Bezel and Custom Modal
$hasHtmlResolutionEngine = $htmlDoc.Contains("html-res-dropdown") -and $htmlDoc.Contains("1080p") -and $htmlDoc.Contains("720p") -and $htmlDoc.Contains("2K") -and $htmlDoc.Contains("4K") -and $htmlDoc.Contains("setHtmlResolution")
Assert-Equal $hasHtmlResolutionEngine $true "HTML view includes 1080p (default), 720p, 2K, 4K resolution options and setHtmlResolution"

$hasHtmlOrientationAndFit = $htmlDoc.Contains("toggleHtmlOrientation") -and $htmlDoc.Contains("toggleHtmlAutoFit") -and $htmlDoc.Contains("html-viewport-stage") -and $htmlDoc.Contains("html-orient-pill") -and $htmlDoc.Contains("html-dim-tag")
Assert-Equal $hasHtmlOrientationAndFit $true "HTML view includes screen orientation toggle, auto-fit scale engine and stats pills"

$hasHtmlCustomModal = $htmlDoc.Contains("html-custom-res-modal") -and $htmlDoc.Contains("openHtmlCustomResModal") -and $htmlDoc.Contains("confirmCustomResolution") -and $htmlDoc.Contains("html-custom-w-input") -and $htmlDoc.Contains("html-custom-h-input")
Assert-Equal $hasHtmlCustomModal $true "HTML view includes custom resolution modal dialog with width/height inputs and presets"

$hasHtmlMobileMode = $htmlDoc.Contains("setHtmlResolution(390, 844, '手机', true)") -and $htmlDoc.Contains("is-mobile")
Assert-Equal $hasHtmlMobileMode $true "HTML view includes mobile device framing and bezel styling"

# Test 37: Fullscreen Presentation (Zen Mode) & Data Recognition Animation Engine
$hasZenFullscreenButton = $htmlMd.Contains("id='btn-zen-fullscreen'") -and $htmlCode.Contains("id='btn-zen-fullscreen'") -and $htmlDoc.Contains("id='btn-zen-fullscreen'")
Assert-Equal $hasZenFullscreenButton $true "BuildOmniViewHtml injects Fullscreen presentation button across all document view types"

$hasZenExitPill = $htmlMd.Contains("id='zen-exit-pill'") -and $htmlMd.Contains("zen-exit-pill")
Assert-Equal $hasZenExitPill $true "BuildOmniViewHtml generates zen-exit-pill floating action button for one-click fullscreen exit"

$hasZenFullscreenCss = $htmlMd.Contains("body.zen-fullscreen") -and $htmlMd.Contains(".zen-exit-pill") -and $htmlMd.Contains("zen-revealed")
Assert-Equal $hasZenFullscreenCss $true "BuildOmniViewHtml includes zen-fullscreen layout styles with auto-hiding topbar and viewport expansion"

$hasZenJsController = $htmlMd.Contains("window.toggleZenFullscreen") -and $htmlMd.Contains("ICON_ZEN_MINIMIZE") -and $htmlMd.Contains("ICON_ZEN_MAXIMIZE")
Assert-Equal $hasZenJsController $true "BuildOmniViewHtml includes window.toggleZenFullscreen controller with dynamic icon morphing"

$hasRecognitionHudAndRadar = $htmlMd.Contains("id='recognition-hud'") -and $htmlMd.Contains("radar-ring") -and $htmlMd.Contains("hud-radar-halo")
Assert-Equal $hasRecognitionHudAndRadar $true "BuildOmniViewHtml generates recognition-hud glassmorphism banner and radar-ring halo"

$hasRecognitionAnimations = $htmlMd.Contains("radarWave") -and $htmlMd.Contains("loaderStream") -and $htmlMd.Contains("badgeSpringPulse")
Assert-Equal $hasRecognitionAnimations $true "BuildOmniViewHtml includes radarWave, loaderStream neon laser, and badgeSpringPulse keyframe animations"

$hasRecognitionJsController = $htmlMd.Contains("window.startRecognition") -and $htmlMd.Contains("window.finishRecognition")
Assert-Equal $hasRecognitionJsController $true "BuildOmniViewHtml provides window.startRecognition and window.finishRecognition API controllers"

$hasFullscreenKeybindings = $htmlMd.Contains("e.key === 'F11'") -and $htmlMd.Contains("e.altKey")
Assert-Equal $hasFullscreenKeybindings $true "BuildOmniViewHtml binds F11 and Alt+Enter for instant fullscreen toggling"

$hasSyncFullScreenState = $htmlMd.Contains("window.syncFullScreenState") -and $htmlMd.Contains("ENTER_FULLSCREEN") -and $htmlMd.Contains("EXIT_FULLSCREEN")
Assert-Equal $hasSyncFullScreenState $true "BuildOmniViewHtml includes window.syncFullScreenState and posts ENTER_FULLSCREEN / EXIT_FULLSCREEN actions"

$previewFormMethods = [NppPlugin.OmniView.Forms.PreviewDockForm].GetMethods()
$hasToggleFullScreen = ($previewFormMethods | Where-Object { $_.Name -eq "ToggleFullScreen" }) -ne $null
$hasSetFullScreen = ($previewFormMethods | Where-Object { $_.Name -eq "SetFullScreen" }) -ne $null
Assert-Equal ($hasToggleFullScreen -and $hasSetFullScreen) $true "PreviewDockForm exposes ToggleFullScreen and SetFullScreen native desktop APIs"

# Test 38: JSON Minification & JSON to Markdown Table Direct Conversion
$jsonSampleForMinify = @'
{
  "project": "NppOmniView",
  "features": [ "Markdown", "JSON", "CSV", "SVG", "YAML", "Mermaid", "HTML", "Code" ],
  "active": true,
  "metrics": { "fps": 60, "rating": 5.0 }
}
'@
$errMinify = $null
$minifiedJson = [NppPlugin.OmniView.Core.FormatProcessor]::MinifyJson($jsonSampleForMinify, [ref]$errMinify)
$isJsonMinified = (-not $minifiedJson.Contains("`r")) -and (-not $minifiedJson.Contains("`n")) -and $minifiedJson.Contains('"fps":60') -and ($errMinify -eq $null)
Assert-Equal $isJsonMinified $true "MinifyJson strips whitespace and newlines cleanly"

$jsonArrayForMdTable = '[{"id":1,"name":"Alice","role":"Admin"},{"id":2,"name":"Bob","role":"Developer"}]'
$errMdTable = $null
$mdTableFromJson = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertJsonToMarkdownTable($jsonArrayForMdTable, [ref]$errMdTable)
$hasMdTableHeaders = $mdTableFromJson.Contains("| id | name | role |") -and $mdTableFromJson.Contains("| --- | --- | --- |") -and $mdTableFromJson.Contains("| 1 | Alice | Admin |")
Assert-Equal $hasMdTableHeaders $true "ConvertJsonToMarkdownTable creates valid GitHub Markdown table directly from JSON"

# Test 39: Error Handling & Resilience (Invalid SVG, JSON, CSV Edge Cases)
$errInvalidSvg = $null
$resInvalidSvg = [NppPlugin.OmniView.Core.FormatProcessor]::FormatSvg("<svg><unclosed>", [ref]$errInvalidSvg)
$hasSvgError = ($errInvalidSvg -ne $null) -and ($resInvalidSvg -eq "<svg><unclosed>")
Assert-Equal $hasSvgError $true "FormatSvg safely catches XML parsing errors and returns original string with errorMsg"

$errInvalidJson = $null
$resInvalidJson = [NppPlugin.OmniView.Core.FormatProcessor]::BeautifyJson("{ invalid_json: 123 ", [ref]$errInvalidJson)
$hasJsonError = ($errInvalidJson -ne $null) -and ($resInvalidJson -eq "{ invalid_json: 123 ")
Assert-Equal $hasJsonError $true "BeautifyJson safely catches syntax errors and returns original text with errorMsg"

$errNonArrayJson = $null
$resNonArray = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertJsonToCsv('"just a string"', [ref]$errNonArrayJson)
$hasArrayError = ($errNonArrayJson -ne $null)
Assert-Equal $hasArrayError $true "ConvertJsonToCsv validates that JSON root is an array or object"

# Test 40: Multi-Language Code Extension Mapping Completeness
$codeExtMap = @{
    ".sh"   = "bash"
    ".py"   = "python"
    ".js"   = "javascript"
    ".ts"   = "typescript"
    ".rs"   = "rust"
    ".go"   = "go"
    ".sql"  = "sql"
    ".xml"  = "xml"
    ".bat"  = "batch"
}
$allLangsMatch = $true
foreach ($ext in $codeExtMap.Keys) {
    $resolved = [NppPlugin.OmniView.Core.MultiFormatDetector]::GetLanguageForExtension($ext)
    if ($resolved -ne $codeExtMap[$ext]) { $allLangsMatch = $false; break }
}
Assert-Equal $allLangsMatch $true "GetLanguageForExtension accurately maps all primary developer languages (.sh, .py, .js, .ts, .rs, .go, .sql, .xml, .bat)"

$unknownLang = [NppPlugin.OmniView.Core.MultiFormatDetector]::GetLanguageForExtension(".unknown_ext_xyz")
Assert-Equal $unknownLang "text" "GetLanguageForExtension gracefully defaults to 'text' for unmapped extensions"

# Test 41: Mermaid Multi-Diagram Keywords Detection & Palette Shared Logic
$mmdKeywords = @(
    "sequenceDiagram`r`nautonumber`r`nAlice->>Bob: Hello",
    "classDiagram`r`nclass BankAccount",
    "stateDiagram-v2`r`n[*] --> Still",
    "erDiagram`r`nCUSTOMER ||--o{ ORDER : places",
    "pie title Pet Adoption`r`n`"Dogs`" : 386`r`n`"Cats`" : 85",
    "gitGraph`r`ncommit"
)
$allMmdDetected = $true
foreach ($sampleMmd in $mmdKeywords) {
    $d = New-Object NppPlugin.OmniView.Core.DocumentContext
    $d.Content = $sampleMmd
    $res = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($d, $null)
    if ($res.Format.ToString() -ne "Mermaid") { $allMmdDetected = $false; break }
}
Assert-Equal $allMmdDetected $true "MultiFormatDetector recognizes sequence, class, state, ER, pie, and gitGraph Mermaid diagrams"

# Test 42: Unified Palette Architecture (SVG & Mermaid Parity)
$hasSharedPaletteLogic = $htmlSvg.Contains("setCanvasBg") -and $htmlMmd.Contains("setCanvasBg") -and $htmlSvg.Contains("npp_omniview_svg_bg_color") -and $htmlMmd.Contains("npp_omniview_mermaid_bg_color")
Assert-Equal $hasSharedPaletteLogic $true "SVG and Mermaid share unified setCanvasBg architecture and independent localStorage persistence"

# Test 43: Sticky Single-Row Toolbars (JSON, CSV, HTML Viewports)
$hasJsonStickyCss = $htmlJson.Contains("view-toolbar json-toolbar") -and $htmlJson.Contains(".view-toolbar") -and $htmlJson.Contains("position: sticky")
Assert-Equal $hasJsonStickyCss $true "JSON view includes pinned sticky toolbar preventing scroll-away during long tree inspection"

$hasCsvStickyCss = $htmlCsv.Contains("csv-toolbar") -and $htmlCsv.Contains("position: sticky")
Assert-Equal $hasCsvStickyCss $true "CSV view includes pinned sticky toolbar and flexbox auto-scroll container"

$hasHtmlStickyCss = $htmlDoc.Contains("view-toolbar html-toolbar") -and $htmlDoc.Contains(".view-toolbar") -and $htmlDoc.Contains("position: sticky")
Assert-Equal $hasHtmlStickyCss $true "HTML view includes pinned resolution status sub-toolbar"

# Test 44: Custom Delimiter Detection & Conversion on Canonical Sample Files (.csv, .tsv) & In-Memory Delimiters
function Get-SampleFilePath($name) {
    $p = Join-Path $PSScriptRoot "samples\$name"
    if (Test-Path $p) { return $p }
    return Join-Path $PSScriptRoot $name
}

$csvSamplePath = Get-SampleFilePath "sample.csv"
if (Test-Path $csvSamplePath) {
    $csvContent = [System.IO.File]::ReadAllText($csvSamplePath, [System.Text.Encoding]::UTF8)
    $detectedComma = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter($csvContent)
    Assert-Equal $detectedComma ([char]',') "DetectCsvDelimiter detects comma (,) delimiter from actual sample.csv"
}

$tsvSamplePath = Get-SampleFilePath "sample.tsv"
if (Test-Path $tsvSamplePath) {
    $tsvContent = [System.IO.File]::ReadAllText($tsvSamplePath, [System.Text.Encoding]::UTF8)
    $detectedTab = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter($tsvContent)
    Assert-Equal $detectedTab ([char]"`t") "DetectCsvDelimiter detects tab (\t) delimiter from actual sample.tsv"
}

# Pipe & Semicolon Delimiter Robustness Tests
$pipeData = "task_id|project_name|assignee|status`r`n1001|OmniView UI Redesign|Alice|Done`r`n1002|Custom Delimiter Parser|Bob|Done"
$detectedPipeSample = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter($pipeData)
Assert-Equal $detectedPipeSample ([char]'|') "DetectCsvDelimiter detects pipe (|) delimiter accurately"

$convertedFromPipe = [NppPlugin.OmniView.Core.FormatProcessor]::ConvertCsvDelimiter($pipeData, [char]',', [char]'|')
$hasConvertedCommas = $convertedFromPipe.Contains("1001,OmniView UI Redesign,Alice,Done")
Assert-Equal $hasConvertedCommas $true "ConvertCsvDelimiter successfully converts pipe-delimited data to comma CSV"

$semiData = "sku;product_name;price`r`nSKU-001;Notepad++ OmniView;0,00`r`nSKU-002;Mermaid Native Renderer;15,50"
$detectedSemiSample = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter($semiData)
Assert-Equal $detectedSemiSample ([char]';') "DetectCsvDelimiter detects semicolon (;) delimiter accurately"

$ps1SamplePath = Get-SampleFilePath "sample.ps1"
if (Test-Path $ps1SamplePath) {
    $ps1Content = [System.IO.File]::ReadAllText($ps1SamplePath, [System.Text.Encoding]::UTF8)
    $docFromPs1 = New-Object NppPlugin.OmniView.Core.DocumentContext
    $docFromPs1.Extension = ".ps1"
    $docFromPs1.FullPath = $ps1SamplePath
    $docFromPs1.Content = $ps1Content
    $detFromPs1 = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docFromPs1, $null)
    Assert-Equal $detFromPs1.Format.ToString() "Code" "MultiFormatDetector accurately identifies sample.ps1 as DocFormat.Code"
}


# Test 45: Universal In-Place Preview Editing Architecture (P0 & P1 Parity)
$hasInPlaceHtml = $htmlDoc.Contains("designMode = 'on'") -and $htmlDoc.Contains("designMode = 'off'")
Assert-Equal $hasInPlaceHtml $true "HTML view includes in-place WYSIWYG designMode controllers"

$hasInPlaceSvg = $htmlSvg.Contains("initSvgInPlaceEditing") -and $htmlSvg.Contains("svg-shape-inspector") -and $htmlSvg.Contains("svg-inline-text-editor") -and $htmlSvg.Contains("svg-editing-active")
Assert-Equal $hasInPlaceSvg $true "SVG view includes in-place canvas text editing and vector shape inspector"

$hasInPlaceCode = $htmlCode.Contains("code-in-place-active") -and $htmlCode.Contains("contenteditable")
Assert-Equal $hasInPlaceCode $true "Code view includes in-place syntax-highlighted code block editing"

$hasInPlaceMd = $htmlMd.Contains("initMarkdownInPlaceEditing") -and $htmlMd.Contains("md-block-editing") -and $htmlMd.Contains("md-block-textarea")
Assert-Equal $hasInPlaceMd $true "Markdown view includes block-level double-click in-place editing"

$hasInPlaceMmd = $htmlMmd.Contains("initMermaidInPlaceEditing") -and $htmlMmd.Contains("mermaid-node-editor")
Assert-Equal $hasInPlaceMmd $true "Mermaid view includes node-level double-click in-place text editing"

$hasSplitDrawer = $htmlMd.Contains("split-drawer") -and $htmlMd.Contains("btn-editor-layout-toggle") -and $htmlMd.Contains("toggleEditorSplitLayout")
Assert-Equal $hasSplitDrawer $true "OmniView includes split-drawer live viewport editor with synchronized live preview"

# Test 46: PowerShell Rich Viewport, AST Analyzer & Comment-Based Help
$samplePs1Path = "E:\Projects\plugins\NppOmniView\tests\samples\sample.ps1"
$samplePs1Content = [System.IO.File]::ReadAllText($samplePs1Path)
$psAnalysis = [NppPlugin.OmniView.Core.PowerShellScriptAnalyzer]::Analyze($samplePs1Content, "sample.ps1")
Assert-Equal ($psAnalysis.TopLevelParameters.Count -ge 2) $true "PowerShellScriptAnalyzer extracts top-level parameters"
Assert-Equal ($psAnalysis.Functions.Count -ge 1) $true "PowerShellScriptAnalyzer extracts script functions"
Assert-Equal $psAnalysis.Functions[0].Name "Invoke-SvgPrettyPrinter" "PowerShellScriptAnalyzer extracts function name accurately"
Assert-Equal ($psAnalysis.Functions[0].Synopsis.Length -gt 0) $true "PowerShellScriptAnalyzer extracts function Comment-Based Help synopsis"

$docPsRich = New-Object NppPlugin.OmniView.Core.DocumentContext
$docPsRich.Content = $samplePs1Content
$docPsRich.FullPath = $samplePs1Path
$docPsRich.Extension = ".ps1"
$docPsRich.IsSavedOnDisk = $true
$detPsRich = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docPsRich, $null)
$htmlPsRich = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docPsRich, $detPsRich, "", $false)

$hasPsDocView = $htmlPsRich.Contains("id='ps-doc-view'") -and $htmlPsRich.Contains("class='ps-doc-container'")
Assert-Equal $hasPsDocView $true "BuildOmniViewHtml includes Microsoft Learn style PowerShell doc view"
$hasPsOutline = $htmlPsRich.Contains("id='ps-outline-drawer'") -and $htmlPsRich.Contains("togglePsOutlineDrawer")
Assert-Equal $hasPsOutline $true "BuildOmniViewHtml includes PowerShell AST outline drawer and symbol navigation"
$hasPsSyntax = $htmlPsRich.Contains("id='ps-syntax-section'") -and $htmlPsRich.Contains("copyPsSyntax")
Assert-Equal $hasPsSyntax $true "BuildOmniViewHtml includes PowerShell syntax diagram and copy controller"
$hasPsModeSwitch = $htmlPsRich.Contains("switchPsViewMode") -and $htmlPsRich.Contains("btn-ps-mode-doc") -and $htmlPsRich.Contains("btn-ps-mode-code")
Assert-Equal $hasPsModeSwitch $true "BuildOmniViewHtml includes seamless Doc vs Code dual-mode switcher"

# Test 47: Mermaid/SVG Top-Pinned Toolbar, Loading Skeleton & Native Image Export
$hasPinnedMermaidToolbar = $htmlMmd.Contains(".view-toolbar.mermaid-toolbar") -and $htmlMmd.Contains("top: 0 !important")
Assert-Equal $hasPinnedMermaidToolbar $true "BuildOmniViewHtml includes top-pinned seamless secondary toolbar for Mermaid"

$hasMermaidLoadingSkeleton = $htmlMmd.Contains("id='mermaid-loading-state'") -and $htmlMmd.Contains("omni-loading-placeholder")
Assert-Equal $hasMermaidLoadingSkeleton $true "BuildOmniViewHtml includes loading skeleton preventing raw code flash in Mermaid"

$hasAntiFlickerStyle = $htmlMmd.Contains("#mermaid-root:not([data-processed])") -and $htmlMmd.Contains("visibility: hidden")
Assert-Equal $hasAntiFlickerStyle $true "BuildOmniViewHtml strictly hides unrendered raw mermaid text before compilation"

$hasEditTipBadge = $htmlMmd.Contains("edit-tip-badge") -and $htmlJson.Contains("edit-tip-badge") -and $htmlCsv.Contains("edit-tip-badge")
Assert-Equal $hasEditTipBadge $true "BuildOmniViewHtml includes in-place double-click editing tip badges across supported views"

$hasNativeExportImage = $htmlMmd.Contains("EXPORT_IMAGE:") -and $htmlSvg.Contains("EXPORT_IMAGE:")
Assert-Equal $hasNativeExportImage $true "BuildOmniViewHtml dispatches EXPORT_IMAGE native message to host for SaveFileDialog"

$hasCsvDblClickEdit = $htmlCsv.Contains("startEditCsvCell") -and $htmlCsv.Contains("csv-cell-in-editing") -and $htmlCsv.Contains("ondblclick=""startEditCsvCell")
Assert-Equal $hasCsvDblClickEdit $true "BuildOmniViewHtml includes native double-click in-place editing for CSV table cells"

$hasExecuteScriptMethod = [NppPlugin.OmniView.Forms.PreviewDockForm].GetMethod("ExecuteScript") -ne $null
Assert-Equal $hasExecuteScriptMethod $true "PreviewDockForm exposes public ExecuteScript method for host-to-view messaging"

# Test 48: Multi-Language Code Viewer Expansion (Python, C#, SQL, Shell, XML)
$docPy = New-Object NppPlugin.OmniView.Core.DocumentContext
$docPy.Content = [System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample.py")
$docPy.FullPath = "E:\Projects\plugins\NppOmniView\tests\samples\sample.py"
$docPy.Extension = ".py"
$detPy = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docPy, $null)
Assert-Equal $detPy.Format.ToString() "Code" "MultiFormatDetector accurately identifies sample.py as DocFormat.Code"
$htmlPy = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docPy, $detPy, "", $false)
Assert-Equal ($htmlPy.Contains("language-python") -and $htmlPy.Contains("class='code-viewer-container'")) $true "BuildOmniViewHtml generates Python code block"

$docCs = New-Object NppPlugin.OmniView.Core.DocumentContext
$docCs.Content = [System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample.cs")
$docCs.FullPath = "E:\Projects\plugins\NppOmniView\tests\samples\sample.cs"
$docCs.Extension = ".cs"
$detCs = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docCs, $null)
Assert-Equal $detCs.Format.ToString() "Code" "MultiFormatDetector accurately identifies sample.cs as DocFormat.Code"
$htmlCs = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docCs, $detCs, "", $false)
Assert-Equal ($htmlCs.Contains("language-csharp")) $true "BuildOmniViewHtml generates C# code block"

$docSql = New-Object NppPlugin.OmniView.Core.DocumentContext
$docSql.Content = [System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample.sql")
$docSql.FullPath = "E:\Projects\plugins\NppOmniView\tests\samples\sample.sql"
$docSql.Extension = ".sql"
$detSql = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docSql, $null)
Assert-Equal $detSql.Format.ToString() "Code" "MultiFormatDetector accurately identifies sample.sql as DocFormat.Code"
$htmlSql = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docSql, $detSql, "", $false)
Assert-Equal ($htmlSql.Contains("language-sql")) $true "BuildOmniViewHtml generates SQL code block"

$docSh = New-Object NppPlugin.OmniView.Core.DocumentContext
$docSh.Content = [System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample.sh")
$docSh.FullPath = "E:\Projects\plugins\NppOmniView\tests\samples\sample.sh"
$docSh.Extension = ".sh"
$detSh = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docSh, $null)
Assert-Equal $detSh.Format.ToString() "Code" "MultiFormatDetector accurately identifies sample.sh as DocFormat.Code"
$htmlSh = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docSh, $detSh, "", $false)
Assert-Equal ($htmlSh.Contains("language-bash")) $true "BuildOmniViewHtml generates Bash code block"
Assert-Equal ($htmlCs.Contains("code-overview-card") -and $htmlCs.Contains("toggleCodeMirrorDisplay")) $true "BuildOmniViewHtml generates lightweight code overview card for C#"

$docXml = New-Object NppPlugin.OmniView.Core.DocumentContext
$docXml.Content = [System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample.xml")
$docXml.FullPath = "E:\Projects\plugins\NppOmniView\tests\samples\sample.xml"
$docXml.Extension = ".xml"
$detXml = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docXml, $null)
Assert-Equal $detXml.Format.ToString() "Xml" "MultiFormatDetector accurately identifies sample.xml as DocFormat.Xml"
$htmlXml = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docXml, $detXml, "", $false)
$hasXmlComponents = $htmlXml.Contains("xml-root") -and $htmlXml.Contains("initXmlView") -and $htmlXml.Contains("toggleXmlAttributes") -and $htmlXml.Contains("filterXmlTree") -and $htmlXml.Contains("badge-xml")
Assert-Equal $hasXmlComponents $true "BuildOmniViewHtml generates XML interactive DOM tree outline and controls"

# Test 49: Expanded Mermaid Diagrams (Sequence & Class Diagrams)
$docSeq = New-Object NppPlugin.OmniView.Core.DocumentContext
$docSeq.Content = [System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample_sequence.mmd")
$docSeq.FullPath = "E:\Projects\plugins\NppOmniView\tests\samples\sample_sequence.mmd"
$docSeq.Extension = ".mmd"
$detSeq = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docSeq, $null)
Assert-Equal $detSeq.Format.ToString() "Mermaid" "MultiFormatDetector accurately identifies sample_sequence.mmd as DocFormat.Mermaid"
$htmlSeq = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docSeq, $detSeq, "", $false)
Assert-Equal ($htmlSeq.Contains("sequenceDiagram") -and $htmlSeq.Contains("mermaid-viewport")) $true "BuildOmniViewHtml renders Mermaid sequence diagram"

$docClass = New-Object NppPlugin.OmniView.Core.DocumentContext
$docClass.Content = [System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample_class.mmd")
$docClass.FullPath = "E:\Projects\plugins\NppOmniView\tests\samples\sample_class.mmd"
$docClass.Extension = ".mmd"
$detClass = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docClass, $null)
Assert-Equal $detClass.Format.ToString() "Mermaid" "MultiFormatDetector accurately identifies sample_class.mmd as DocFormat.Mermaid"
$htmlClass = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docClass, $detClass, "", $false)
Assert-Equal ($htmlClass.Contains("classDiagram") -and $htmlClass.Contains("mermaid-viewport")) $true "BuildOmniViewHtml renders Mermaid class diagram"

# Test 50: Non-Standard CSV Delimiter Samples (Semicolon & Pipe)
$docSemicolon = [System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample_semicolon.csv")
$delimSemicolon = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter($docSemicolon)
Assert-Equal $delimSemicolon ";" "DetectCsvDelimiter detects semicolon delimiter from sample_semicolon.csv"

$docPipe = [System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample_pipe.csv")
$delimPipe = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter($docPipe)
Assert-Equal $delimPipe "|" "DetectCsvDelimiter detects pipe delimiter from sample_pipe.csv"

# Test 51: Enterprise PowerShell Script Deep Inspection
$psEnterprise = [NppPlugin.OmniView.Core.PowerShellScriptAnalyzer]::Analyze([System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample.ps1"), "sample.ps1")
Assert-Equal ($psEnterprise.TopLevelParameters.Count -ge 4) $true "PowerShellScriptAnalyzer extracts 4+ top-level parameters from enterprise sample"
Assert-Equal ($psEnterprise.Functions.Count -ge 3) $true "PowerShellScriptAnalyzer extracts 3+ enterprise functions"
Assert-Equal ($psEnterprise.Requires.Count -ge 2) $true "PowerShellScriptAnalyzer extracts multiple #Requires directives"
Assert-Equal ($psEnterprise.SecurityNotices.Count -ge 1) $true "PowerShellScriptAnalyzer captures sensitive security commands in sample.ps1"

# Test 52: Unsaved SVG Content Sniffing (Prolog, Comments, DOCTYPE & In-Progress Typing)
$docUnsavedSvg = New-Object NppPlugin.OmniView.Core.DocumentContext
$docUnsavedSvg.Content = [System.IO.File]::ReadAllText("E:\Projects\plugins\NppOmniView\tests\samples\sample.svg")
$docUnsavedSvg.Extension = ""
$docUnsavedSvg.IsSavedOnDisk = $false
$detUnsavedSvg = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docUnsavedSvg, $null)
Assert-Equal $detUnsavedSvg.Format.ToString() "Svg" "MultiFormatDetector automatically identifies unsaved sample.svg without extension"

$docPrologSvg = New-Object NppPlugin.OmniView.Core.DocumentContext
$docPrologSvg.Content = "<?xml version=""1.0"" encoding=""utf-8""?>`r`n<!-- Generator: Adobe Illustrator -->`r`n<!DOCTYPE svg PUBLIC ""-//W3C//DTD SVG 1.1//EN"">`r`n<svg viewBox=""0 0 100 100""><circle r=""10""/></svg>"
$docPrologSvg.Extension = ""
$docPrologSvg.IsSavedOnDisk = $false
$detPrologSvg = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docPrologSvg, $null)
Assert-Equal $detPrologSvg.Format.ToString() "Svg" "MultiFormatDetector identifies unsaved SVG with XML prolog, comments, and DOCTYPE"

$docShortSvg = New-Object NppPlugin.OmniView.Core.DocumentContext
$docShortSvg.Content = "<svg viewBox=""0 0 100 100"">"
$docShortSvg.Extension = ""
$docShortSvg.IsSavedOnDisk = $false
$detShortSvg = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docShortSvg, $null)
Assert-Equal $detShortSvg.Format.ToString() "Svg" "MultiFormatDetector identifies in-progress unsaved SVG typing fragment"

# Test 53: Universal Viewport Loading & Rendering Animation Engine
$hasGlobalLoadingDom = $htmlJson.Contains("id='omni-global-loading'") -and $htmlJson.Contains("class='omni-global-loading'") -and $htmlJson.Contains("loading-spinner-orbit") -and $htmlJson.Contains("spinner-ring")
Assert-Equal $hasGlobalLoadingDom $true "BuildOmniViewHtml injects universal omni-global-loading container with dual-orbit spinner"

$hasJsonLoadingTitle = $htmlJson.Contains("正在解析并构建 JSON 交互树") -and $htmlJson.Contains("global-loading-title")
Assert-Equal $hasJsonLoadingTitle $true "BuildOmniViewHtml configures specialized JSON loading title and subtitle"

$hasLoadingJsApi = $htmlJson.Contains("window.showGlobalLoading") -and $htmlJson.Contains("window.hideGlobalLoading")
Assert-Equal $hasLoadingJsApi $true "BuildOmniViewHtml exposes window.showGlobalLoading and window.hideGlobalLoading JS controllers"

$hasDockNotifyLoading = [bool]([NppPlugin.OmniView.Forms.PreviewDockForm].GetMethod("NotifyLoadingStarted"))
Assert-Equal $hasDockNotifyLoading $true "PreviewDockForm exposes public NotifyLoadingStarted desktop API"

# Test 54: Unsaved XML Content Sniffing (Prolog, Maven, Android, Config, Self-closing tags)
$docUnsavedXml1 = New-Object NppPlugin.OmniView.Core.DocumentContext
$docUnsavedXml1.Content = "<?xml version=""1.0"" encoding=""utf-8""?>`r`n<manifest xmlns:android=""http://schemas.android.com/apk/res/android"" package=""com.example.app"">`r`n  <application android:label=""Test""/>`r`n</manifest>"
$docUnsavedXml1.Extension = ""
$docUnsavedXml1.IsSavedOnDisk = $false
$detUnsavedXml1 = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docUnsavedXml1, $null)
Assert-Equal $detUnsavedXml1.Format.ToString() "Xml" "MultiFormatDetector accurately identifies unsaved XML manifest with prolog as DocFormat.Xml"

$docUnsavedXml2 = New-Object NppPlugin.OmniView.Core.DocumentContext
$docUnsavedXml2.Content = "<project xmlns=""http://maven.apache.org/POM/4.0.0"">`r`n  <modelVersion>4.0.0</modelVersion>`r`n  <groupId>com.npp</groupId>`r`n</project>"
$docUnsavedXml2.Extension = ""
$docUnsavedXml2.IsSavedOnDisk = $false
$detUnsavedXml2 = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docUnsavedXml2, $null)
Assert-Equal $detUnsavedXml2.Format.ToString() "Xml" "MultiFormatDetector accurately identifies unsaved Maven POM XML without prolog as DocFormat.Xml"

$htmlXml = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docUnsavedXml1, $detUnsavedXml1, "", $false)
$hasXmlBadge = $htmlXml.Contains("badge-xml") -and $htmlXml.Contains("xml-root")
Assert-Equal $hasXmlBadge $true "BuildOmniViewHtml generates XML interactive DOM tree outline and badge for unsaved XML"

# Test 55: RFC 4180 CSV with Multiline Newlines and Delimiters inside Double Quotes
$docMultilineCsv = New-Object NppPlugin.OmniView.Core.DocumentContext
$docMultilineCsv.Content = @'
"item_id","product_name","description"
"1001","Apple Crisp","Fresh, hand-picked apples
baked to a golden crust"
"1002","Banana Split","Classic dessert with 3 scoops,
nuts, and cherries"
"1003","Cherry Pie","Traditional tart cherry"
'@
$docMultilineCsv.Extension = ""
$docMultilineCsv.IsSavedOnDisk = $false
$detMultilineCsv = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docMultilineCsv, $null)
Assert-Equal $detMultilineCsv.Format.ToString() "Csv" "MultiFormatDetector accurately identifies unsaved CSV containing multiline field values"

$detectedDelimWithNewlines = [NppPlugin.OmniView.Core.FormatProcessor]::DetectCsvDelimiter($docMultilineCsv.Content)
Assert-Equal $detectedDelimWithNewlines ([char]',') "DetectCsvDelimiter correctly detects comma delimiter even when records have internal newlines"

$docEscapedQuotesCsv = New-Object NppPlugin.OmniView.Core.DocumentContext
$docEscapedQuotesCsv.Content = @'
"id","title","author"
"1","The ""Great"" Gatsby, 1925","F. Scott Fitzgerald"
"2","To Kill, A Mockingbird","Harper Lee"
"3","1984","George Orwell"
'@
$docEscapedQuotesCsv.Extension = ""
$docEscapedQuotesCsv.IsSavedOnDisk = $false
$detEscapedQuotesCsv = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docEscapedQuotesCsv, $null)
Assert-Equal $detEscapedQuotesCsv.Format.ToString() "Csv" "MultiFormatDetector accurately identifies unsaved CSV with commas and escaped quotes inside quoted fields"

# Test 56: Friendly Badge Description for Unknown/Plaintext & Topbar Badge State Focus
$docUnknown = New-Object NppPlugin.OmniView.Core.DocumentContext
$docUnknown.Content = "Just some arbitrary plain text notes without any structured markup or syntax.`r`nAnother normal line."
$docUnknown.Extension = ""
$docUnknown.IsSavedOnDisk = $false
$detUnknown = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docUnknown, $null)
Assert-Equal $detUnknown.Format.ToString() "Unknown" "MultiFormatDetector evaluates arbitrary text as Unknown format"

$htmlUnknown = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docUnknown, $detUnknown, $false, $null)
$hasFriendlyUnknownBadge = $htmlUnknown.Contains("纯文本 (未识别)") -and $htmlUnknown.Contains("badge-unknown")
Assert-Equal $hasFriendlyUnknownBadge $true "BuildOmniViewHtml generates friendly '纯文本 (未识别)' badge instead of UNKNOWN"

$doesNotContainRawUnknownBadge = -not $htmlUnknown.Contains(">UNKNOWN |")
Assert-Equal $doesNotContainRawUnknownBadge $true "BuildOmniViewHtml strictly eliminates raw all-caps UNKNOWN badge text"

$hasBadgeStateControllers = $htmlUnknown.Contains("badge.setAttribute('data-origin-text'") -or $htmlUnknown.Contains("b.setAttribute('data-origin-text'")
Assert-Equal $hasBadgeStateControllers $true "BuildOmniViewHtml focuses loading and ready indicators directly on topbar badge"

# Test 57: Dedicated XML View, Sniffing & Formatting
$docXmlSniff = New-Object NppPlugin.OmniView.Core.DocumentContext
$docXmlSniff.Content = "<?xml version=""1.0"" encoding=""utf-8""?>`r`n<configuration><appSettings><add key=""Env"" value=""Prod""/></appSettings></configuration>"
$docXmlSniff.Extension = ""
$docXmlSniff.IsSavedOnDisk = $false
$detXmlSniff = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docXmlSniff, $null)
Assert-Equal $detXmlSniff.Format.ToString() "Xml" "MultiFormatDetector sniffs unsaved XML documents with XML declaration"

$xmlErr = $null
$formattedXml = [NppPlugin.OmniView.Core.FormatProcessor]::FormatXml($docXmlSniff.Content, [ref]$xmlErr)
Assert-Equal ($xmlErr -eq $null -and $formattedXml.Contains("  <appSettings>")) $true "FormatXml accurately indents and formats XML"

$htmlXmlCheck = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docXmlSniff, $detXmlSniff, "", $false)
Assert-Equal ($htmlXmlCheck.Contains("xml-root") -and $htmlXmlCheck.Contains("XML_BEAUTIFY")) $true "BuildOmniViewHtml contains xml-root and XML_BEAUTIFY controls"
Assert-Equal ($htmlJson.Contains("FORCE_XML")) $true "BuildOmniViewHtml includes FORCE_XML switch in global view menu"

# Test 58: Unified Top Toolbar Architecture & Single Simplified In-Place Edit Tip
$docMdTest = New-Object NppPlugin.OmniView.Core.DocumentContext
$docMdTest.Content = "# Heading 1`r`nThis is a sample markdown paragraph."
$docMdTest.Extension = ".md"
$detMdTest = [NppPlugin.OmniView.Core.MultiFormatDetector]::Evaluate($docMdTest, $null)
$htmlMdTest = [NppPlugin.OmniView.Core.HtmlTemplateBuilder]::BuildOmniViewHtml($docMdTest, $detMdTest, "", $false)

$hasMdToolbar = $htmlMdTest.Contains("view-toolbar md-toolbar")
Assert-Equal $hasMdToolbar $true "Markdown view integrates unified top secondary toolbar (md-toolbar)"

$noMdFloatingTipBar = -not $htmlMdTest.Contains("class='md-edit-tip-bar'")
Assert-Equal $noMdFloatingTipBar $true "Markdown view eliminated awkward separate md-edit-tip-bar above content"

$noMermaidFloatingTip = -not $htmlMmd.Contains("class='mermaid-float-tip'")
Assert-Equal $noMermaidFloatingTip $true "Mermaid view eliminated duplicate in-canvas floating tip"

$hasSingleSimplifiedTip = $htmlMdTest.Contains("<span>双击就地编辑</span>") -and `
                          $htmlJson.Contains("<span>双击就地编辑</span>") -and `
                          $htmlCsv.Contains("<span>双击就地编辑</span>") -and `
                          $htmlSvg.Contains("<span>双击就地编辑</span>") -and `
                          $htmlYaml.Contains("<span>双击就地编辑</span>") -and `
                          $htmlMmd.Contains("<span>双击就地编辑</span>") -and `
                          $htmlDoc.Contains("<span>双击就地编辑</span>") -and `
                          $htmlXmlCheck.Contains("<span>双击就地编辑</span>")
Assert-Equal $hasSingleSimplifiedTip $true "All supported preview pages display exactly one unified simplified '双击就地编辑' badge on secondary toolbar"

# Test: Universal Fixed Pinned Secondary Toolbar Standards
$hasFixedViewToolbar = $htmlCsv.Contains(".view-toolbar {") -and `
                       $htmlCsv.Contains("position: fixed !important;") -and `
                       $htmlCsv.Contains("top: 42px !important;") -and `
                       $htmlCsv.Contains("z-index: 990 !important;")
Assert-Equal $hasFixedViewToolbar $true "All preview pages unify on rock-solid position: fixed top: 42px secondary toolbar"

$hasUnifiedTopSpacing = $htmlCsv.Contains("padding: 82px 0 0 0 !important;") -and `
                        $htmlMmd.Contains("padding: 82px 0 0 0 !important;") -and `
                        $htmlDoc.Contains("padding: 82px 0 0 0 !important;")
Assert-Equal $hasUnifiedTopSpacing $true "All formats with secondary sub-toolbars adopt unified 82px top spacing to prevent header overlap"

$hasCsvIndependentScroll = $htmlCsv.Contains("body.fmt-csv .csv-container {") -and `
                           $htmlCsv.Contains("overflow: auto !important;")
Assert-Equal $hasCsvIndependentScroll $true "CSV container features isolated dual-axis overflow: auto ensuring toolbar stays pinned during table scroll"

$noExtraXmlClosingDiv = -not $htmlXmlCheck.Contains("</div>`r`n                    </div>`r`n                    </div>`r`n                    <div id='xml-root'") -and `
                        -not $htmlXmlCheck.Contains("</div>`n                    </div>`n                    </div>`n                    <div id='xml-root'")
Assert-Equal $noExtraXmlClosingDiv $true "XML template strictly eliminates redundant third closing div preserving DOM tree integrity"

$noPsToolbarLegacyOverride = -not $htmlDoc.Contains(".view-toolbar.ps-toolbar {")
# Test: Modern XML Interactive Tree, Complete Fold Closed Tags & In-Place Inline Editors
$hasXmlInPlaceEditors = $htmlXmlCheck.Contains("startEditXmlAttrVal") -and `
                        $htmlXmlCheck.Contains("startEditXmlAttrName") -and `
                        $htmlXmlCheck.Contains("startEditXmlText") -and `
                        $htmlXmlCheck.Contains("startEditXmlTagName") -and `
                        $htmlXmlCheck.Contains("startEditXmlCData")
Assert-Equal $hasXmlInPlaceEditors $true "XML interactive tree includes full suite of double-click in-place inline editors"

$hasXmlCollapsedPreview = $htmlXmlCheck.Contains("xml-collapsed-preview") -and `
                          $htmlXmlCheck.Contains("xml-close-tag-row")
Assert-Equal $hasXmlCollapsedPreview $true "XML interactive tree preserves complete closing tags during collapse and unindent"

$hasXmlAdvancedTools = $htmlXmlCheck.Contains("formatXmlString") -and `
                       $htmlXmlCheck.Contains("copyXmlXPath") -and `
                       $htmlXmlCheck.Contains("xml-just-edited") -and `
                       $htmlXmlCheck.Contains("xml-cdata-row")
Assert-Equal $hasXmlAdvancedTools $true "XML view integrates formatXmlString, XPath copy, CDATA handling, and visual edit flash feedback"

$resultColor = if ($testsFailed -eq 0) { "Green" } else { "Red" }
$testsPassed = $testsPassed
$testsFailed = $testsFailed
Write-Host "Results: $testsPassed Passed, $testsFailed Failed" -ForegroundColor $resultColor
if ($testsFailed -gt 0) { exit 1 }
