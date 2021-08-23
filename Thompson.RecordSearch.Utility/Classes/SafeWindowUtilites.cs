using System.Runtime.InteropServices;

namespace Thompson.RecordSearch.Utility.Classes
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1060:Move pinvokes to native methods class", Justification = "<Pending>")]
    internal static class SafeWindowUtilites
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.SysInt)]
        internal static extern int ShowWindow(int hwnd, int nCmdShow);
    }
}
