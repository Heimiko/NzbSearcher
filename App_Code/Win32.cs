using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace NzbSearcher
{
    public class Win32
    {
        public static int WM_SETREDRAW = 0x000B;
        public static int WM_USER = 0x400;
        public static int EM_GETEVENTMASK = (WM_USER + 59);
        public static int EM_SETEVENTMASK = (WM_USER + 69);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);
    }

    public class LockWindowUpdate : IDisposable
    {
        IntPtr _eventMask = IntPtr.Zero;
        IntPtr _hWnd;

        public LockWindowUpdate(IntPtr hWnd)
        {
            _hWnd = hWnd;

            // Stop redrawing:
            //Win32.SendMessage(_hWnd, Win32.WM_SETREDRAW, 0, IntPtr.Zero);
            // Stop sending of events:
            //_eventMask = Win32.SendMessage(_hWnd, Win32.EM_GETEVENTMASK, 0, IntPtr.Zero);

            Win32.LockWindowUpdate(hWnd);
        }

        public void Dispose()
        {
            Win32.LockWindowUpdate(IntPtr.Zero);

            // turn on events
            //Win32.SendMessage(_hWnd, Win32.EM_SETEVENTMASK, 0, _eventMask);
            // turn on redrawing
            //Win32.SendMessage(_hWnd, Win32.WM_SETREDRAW, 1, IntPtr.Zero);
        }
    }
}
