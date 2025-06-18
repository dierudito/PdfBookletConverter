using System.Runtime.InteropServices;

namespace PdfBookletConverter.Services;

/// <summary>
/// Provides access to native Windows API functions using P/Invoke.
/// </summary>
internal static partial class WindowsApi
{
    // Attaches the calling process to the console of the specified process.
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool AttachConsole(uint dwProcessId);

    // Allocates a new console for the calling process.
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool AllocConsole();

    // Detaches the calling process from its console.
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool FreeConsole();

    // Displays a modal dialog box that contains a system icon, a set of buttons, and a brief application-specific message.
    [LibraryImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

    // Constants for MessageBox uType parameter
    internal const uint MB_OK = 0x00000000;
    internal const uint MB_ICONERROR = 0x00000010;
}