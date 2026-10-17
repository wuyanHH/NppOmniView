using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace NppPlugin.OmniView.Core
{
    public class DocumentContext
    {
        public IntPtr BufferId { get; set; }
        public string FullPath { get; set; }
        public string Extension { get; set; }
        public bool IsSavedOnDisk { get; set; }
        public string Content { get; set; }
        public int Length { get; set; }
        public bool IsLargeFile { get; set; }
    }

    public static class BufferReader
    {
        public const int MaxLivePreviewBytes = 5 * 1024 * 1024; // 5 MB threshold
        public static bool IsFormatting { get; private set; } = false;

        public static unsafe DocumentContext GetCurrentDocument()
        {
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();

            StringBuilder pathBuilder = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFULLCURRENTPATH, Win32.MAX_PATH, pathBuilder);
            string fullPath = pathBuilder.ToString();

            IntPtr bufferId = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTBUFFERID, 0, 0);
            bool isSaved = !string.IsNullOrWhiteSpace(fullPath) && File.Exists(fullPath);
            string ext = isSaved ? Path.GetExtension(fullPath).ToLowerInvariant() : string.Empty;

            int length = (int)Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_GETLENGTH, 0, 0);
            string content = string.Empty;
            bool isLarge = length > MaxLivePreviewBytes;

            if (length > 0)
            {
                // 采用 SCI_GETTEXT (2182) 安全读取 Scintilla 内存缓冲。
                // 严禁使用 SCI_GETCHARACTERPOINTER，因为其仅返回 Gap Buffer 前半段，若用户编辑过会导致文本截断与内存冲突。
                // 对于超大文件仅读取前 512KB 进行预览，避免在 LOH (大对象堆) 中分配过大数组。
                int readBytes = isLarge ? Math.Min(length, 512 * 1024) : length;
                byte[] buffer = new byte[readBytes + 1];
                fixed (byte* pBuf = buffer)
                {
                    Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_GETTEXT, readBytes + 1, (IntPtr)pBuf);
                }

                // 探测 Scintilla 代码页编码 (SC_CP_UTF8 = 65001)
                int codePage = (int)Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_GETCODEPAGE, 0, 0);
                Encoding enc = (codePage == 65001 || codePage == 0) ? Encoding.UTF8 : Encoding.Default;

                content = enc.GetString(buffer, 0, readBytes);
            }

            return new DocumentContext
            {
                BufferId = bufferId,
                FullPath = fullPath,
                Extension = ext,
                IsSavedOnDisk = isSaved,
                Content = content,
                Length = length,
                IsLargeFile = isLarge
            };
        }

        public static unsafe void ReplaceCurrentDocumentText(string newContent, bool resetHScroll = false)
        {
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();
            if (curScintilla == IntPtr.Zero) return;

            int codePage = (int)Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_GETCODEPAGE, 0, 0);
            Encoding enc = (codePage == 65001 || codePage == 0) ? Encoding.UTF8 : Encoding.Default;
            byte[] bytes = enc.GetBytes(newContent ?? string.Empty);

            int caretPos = (int)Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_GETCURRENTPOS, 0, 0);
            int firstLine = (int)Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_GETFIRSTVISIBLELINE, 0, 0);

            IsFormatting = true;
            Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_BEGINUNDOACTION, 0, 0);
            try
            {
                Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_CLEARALL, 0, 0);
                if (bytes.Length > 0)
                {
                    fixed (byte* pBytes = bytes)
                    {
                        Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_ADDTEXT, bytes.Length, (IntPtr)pBytes);
                    }
                }

                // Restore caret and viewport
                int newLen = bytes.Length;
                int targetCaret = Math.Min(caretPos, newLen);
                Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_SETSEL, targetCaret, targetCaret);
                Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_SETFIRSTVISIBLELINE, firstLine, 0);

                if (resetHScroll)
                {
                    ResetHorizontalScroll();
                }
            }
            finally
            {
                Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_ENDUNDOACTION, 0, 0);
                IsFormatting = false;
            }
        }

        public static void ResetHorizontalScroll()
        {
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();
            Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_SETXOFFSET, 0, 0);
            Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_LINESCROLL, -10000, 0);
        }

        public static void GetScrollLineInfo(out int firstVisibleDocLine, out int totalDocLines)
        {
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();
            int firstDisplayLine = (int)Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_GETFIRSTVISIBLELINE, 0, 0);
            firstVisibleDocLine = (int)Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_DOCLINEFROMVISIBLE, firstDisplayLine, 0);
            totalDocLines = (int)Win32.SendMessage(curScintilla, (uint)SciMsg.SCI_GETLINECOUNT, 0, 0);
        }
    }
}
