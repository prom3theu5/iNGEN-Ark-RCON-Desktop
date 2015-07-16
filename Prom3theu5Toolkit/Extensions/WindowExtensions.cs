using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace PTK.Extensions
{
    public static class WindowExtensions
    {

        private const uint FLASHW_STOP = 0; //Stop flashing. The system restores the window to its original state.        private const UInt32 FLASHW_CAPTION = 1; //Flash the window caption.        
        private const uint FLASHW_TRAY = 2; //Flash the taskbar button.        
        private const uint FLASHW_ALL = 3; //Flash both the window caption and taskbar button.        
        private const uint FLASHW_TIMER = 4; //Flash continuously, until the FLASHW_STOP flag is set.        
        private const uint FLASHW_TIMERNOFG = 12; //Flash continuously until the window comes to the foreground.  

        private struct FLASHWINFO
        {   
            //The size of the structure in bytes.  
            public uint cbSize;

            //A Handle to the Window to be Flashed. The window can be either opened or minimized.
            public IntPtr hwnd;

            //The Flash Status.   
            public uint dwFlags;

            // number of times to flash the window    
            public uint uCount;

            //The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.    
            public uint dwTimeout;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        public static void FlashWindow(this Window win, uint count = uint.MaxValue)
        {
            if(win.IsActive)
                return;

            WindowInteropHelper h = new WindowInteropHelper(win);
            FLASHWINFO info = new FLASHWINFO
            {
                hwnd = h.Handle,
                dwFlags = FLASHW_ALL | FLASHW_TIMER,
                uCount = count,
                cbSize = (sizeof(uint) * 4) + (uint)IntPtr.Size,
                dwTimeout = 0
            };

            FlashWindowEx(ref info);
        }

        public static void StopFlashing(this Window win)
        {
            WindowInteropHelper h = new WindowInteropHelper(win);
            FLASHWINFO info = new FLASHWINFO();
            info.hwnd = h.Handle;
            info.dwFlags = FLASHW_STOP;
            info.uCount = uint.MaxValue;
            info.dwTimeout = 0;
            info.cbSize = (sizeof(uint) * 4) + (uint)IntPtr.Size;
            FlashWindowEx(ref info);
        }
    }
}



