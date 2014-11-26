namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    internal static class WindowExtensions
    {
        internal static void HideMinimizeButton(this Window window)
        {
            NativeMethods.HideMinimizeButton(window);
        }

        internal static void HideMaximizeButton(this Window window)
        {
            NativeMethods.HideMaximizeButton(window);
        }

        internal static void HideMinimizeAndMaximizeButtons(this Window window)
        {
            NativeMethods.HideMinimizeAndMaximizeButtons(window);
        }

        private static class NativeMethods
        {
            // from winuser.h
            private const int GWL_STYLE = -16;
            private const int WS_MAXIMIZEBOX = 0x10000;
            private const int WS_MINIMIZEBOX = 0x20000;

            [DllImport("user32.dll")]
            private static extern int GetWindowLong(IntPtr hwnd, int index);

            [DllImport("user32.dll")]
            private static extern int SetWindowLong(IntPtr hwnd, int index, int value);

            internal static void HideMinimizeButton(Window window)
            {
                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd == IntPtr.Zero)
                    return;

                var currentStyle = GetWindowLong(hwnd, GWL_STYLE);
                const int Mask = ~WS_MINIMIZEBOX;
                SetWindowLong(hwnd, GWL_STYLE, currentStyle & Mask);
            }

            internal static void HideMaximizeButton(Window window)
            {
                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd == IntPtr.Zero)
                    return;

                var currentStyle = GetWindowLong(hwnd, GWL_STYLE);
                const int Mask = ~WS_MAXIMIZEBOX;
                SetWindowLong(hwnd, GWL_STYLE, currentStyle & Mask);
            }

            internal static void HideMinimizeAndMaximizeButtons(Window window)
            {
                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd == IntPtr.Zero)
                    return;

                var currentStyle = GetWindowLong(hwnd, GWL_STYLE);
                const int Mask = ~(WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
                SetWindowLong(hwnd, GWL_STYLE, currentStyle & Mask);
            }
        }
    }
}
