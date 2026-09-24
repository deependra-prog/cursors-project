using System;
using System.Runtime.InteropServices;

namespace CursorSwitcher
{
    /// <summary>
    /// Thin wrapper around the Win32 call that tells the shell to reload
    /// the cursor scheme from the registry immediately, with no logoff
    /// or restart required.
    /// </summary>
    internal static class NativeMethods
    {
        private const uint SPI_SETCURSORS = 0x0057;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDCHANGE = 0x02;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        public static void BroadcastCursorChange()
        {
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }
    }
}
