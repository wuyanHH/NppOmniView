using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NppPlugin.OmniView.Core
{
    public static class Win32
    {
        public const int MAX_PATH = 260;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, out int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        public const uint IMAGE_BITMAP = 0;
        public const uint IMAGE_ICON = 1;
        public const uint LR_LOADFROMFILE = 0x00000010;

        // ==================== 窗口样式与全屏控制所需 Win32 常量与 API ====================
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int WS_CHILD = 0x40000000;
        public const int WS_POPUP = unchecked((int)0x80000000);
        public const int WS_VISIBLE = 0x10000000;

        public static readonly IntPtr HWND_TOP = IntPtr.Zero;
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_FRAMECHANGED = 0x0020;
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint WM_SIZE = 0x0005;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// 兼容 32 位与 64 位系统的窗口样式获取方法
        /// </summary>
        public static int GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return unchecked((int)GetWindowLongPtr64(hWnd, nIndex).ToInt64());
            return GetWindowLong32(hWnd, nIndex);
        }

        /// <summary>
        /// 兼容 32 位与 64 位系统的窗口样式设置方法
        /// </summary>
        public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 8)
                return unchecked((int)SetWindowLongPtr64(hWnd, nIndex, new IntPtr(dwNewLong)).ToInt64());
            return SetWindowLong32(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
    }

    public enum NppMsg : uint
    {
        NPPMSG = 2024,
        NPPM_GETCURRENTSCINTILLA = NPPMSG + 4,
        NPPM_ADDTOOLBARICON_DEPRECATED = NPPMSG + 28,
        NPPM_DMMSHOW = NPPMSG + 30,
        NPPM_DMMHIDE = NPPMSG + 31,
        NPPM_DMMREGASDCKDLG = NPPMSG + 33,
        NPPM_SETMENUITEMCHECK = NPPMSG + 38,
        NPPM_GETCURRENTBUFFERID = NPPMSG + 60,
        NPPM_ADDTOOLBARICON_FORDARKMODE = NPPMSG + 101,
        NPPM_ISDARKMODEENABLED = NPPMSG + 107,
        NPPM_GETFULLCURRENTPATH = 4025,
        NPPN_READY = 1001,
        NPPN_TBMODIFICATION = 1002,
        NPPN_FILESAVED = 1008,
        NPPN_SHUTDOWN = 1009,
        NPPN_BUFFERACTIVATED = 1010,
        NPPN_DARKMODECHANGED = 1027
    }

    public enum SciMsg : uint
    {
        SCI_ADDTEXT = 2001,
        SCI_CLEARALL = 2004,
        SCI_GETLENGTH = 2006,
        SCI_GETCURRENTPOS = 2008,
        SCI_GOTOLINE = 2024,
        SCI_BEGINUNDOACTION = 2078,
        SCI_ENDUNDOACTION = 2079,
        SCI_GETCODEPAGE = 2137,
        SCI_GETLINEENDPOSITION = 2136,
        SCI_SETXOFFSET = 2145,
        SCI_GETFIRSTVISIBLELINE = 2152,
        SCI_GETLINECOUNT = 2154,
        SCI_SETSEL = 2160,
        SCI_POSITIONFROMLINE = 2167,
        SCI_LINESCROLL = 2168,
        SCI_SCROLLCARET = 2169,
        SCI_GETTEXT = 2182,
        SCI_VISIBLEFROMDOCLINE = 2220,
        SCI_DOCLINEFROMVISIBLE = 2221,
        SCI_ENSUREVISIBLE = 2232,
        SCI_GETCHARACTERPOINTER = 2520,
        SCI_SETFIRSTVISIBLELINE = 2613,
        SCN_UPDATEUI = 2007,
        SCN_MODIFIED = 2008
    }

    public static class NppTbMsg
    {
        public const uint DWS_ICONTAB = 0x00000001;
        public const uint DWS_DF_CONT_RIGHT = 0x10000000;
    }

    /// <summary>
    /// Scintilla SCN_UPDATEUI 通知的 updated 掩码常量
    /// </summary>
    public static class SciUpdate
    {
        /// <summary>
        /// 内容发生变动
        /// </summary>
        public const int SC_UPDATE_CONTENT = 0x01;
        /// <summary>
        /// 选区或光标位置改变
        /// </summary>
        public const int SC_UPDATE_SELECTION = 0x02;
        /// <summary>
        /// 垂直视口滚动
        /// </summary>
        public const int SC_UPDATE_V_SCROLL = 0x04;
        /// <summary>
        /// 水平视口滚动
        /// </summary>
        public const int SC_UPDATE_H_SCROLL = 0x08;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NppData
    {
        public IntPtr _nppHandle;
        public IntPtr _scintillaMainHandle;
        public IntPtr _scintillaSecondHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ToolbarIconsWithDarkMode
    {
        public IntPtr hToolbarBmp;
        public IntPtr hToolbarIcon;
        public IntPtr hToolbarIconDarkMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ShortcutKey
    {
        public byte _isCtrl;
        public byte _isAlt;
        public byte _isShift;
        public byte _key;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NppFuncItemDelegate();

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FuncItem
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string _itemName;
        public IntPtr _pFunc;
        public int _cmdID;
        [MarshalAs(UnmanagedType.U1)]
        public bool _init2Check;
        public IntPtr _pShKey;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SciNotifyHeader
    {
        public IntPtr hwndFrom;
        public IntPtr idFrom;
        public uint code;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ScNotification
    {
        public SciNotifyHeader Header;
        public IntPtr Position;
        public int Ch;
        public int Modifiers;
        public int ModificationType;
        public IntPtr Text;
        public IntPtr Length;
        public IntPtr LinesAdded;
        public int Message;
        public IntPtr WParam;
        public IntPtr LParam;
        public IntPtr Line;
        public int FoldLevelNow;
        public int FoldLevelPrev;
        public int Margin;
        public int ListType;
        public int X;
        public int Y;
        public int Token;
        public IntPtr AnnotationLinesAdded;
        public int Updated;
        public int ListCompletionMethod;
        public int CharacterSource;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NppTbData
    {
        public IntPtr hClient;
        public string pszName;
        public int dlgID;
        public uint uMask;
        public IntPtr hIconTab;
        public string pszAddInfo;
        public RECT rcFloat;
        public int iPrevCont;
        public string pszModuleName;
    }

    public static class PluginBase
    {
        public static NppData nppData;

        public static IntPtr GetCurrentScintilla()
        {
            Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTSCINTILLA, 0, out int curScintilla);
            return curScintilla == 0 ? nppData._scintillaMainHandle : nppData._scintillaSecondHandle;
        }

        public static bool IsDarkModeEnabled()
        {
            if (nppData._nppHandle == IntPtr.Zero) return false;
            return Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_ISDARKMODEENABLED, 0, 0) != IntPtr.Zero;
        }
    }
}
