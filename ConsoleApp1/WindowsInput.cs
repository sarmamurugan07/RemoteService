
//using System;
//using System.Runtime.InteropServices;

//public static class WindowsInputNative
//{
//    // ---------------------------------------
//    // MOUSE API
//    // ---------------------------------------
//    [DllImport("user32.dll")]
//    static extern bool SetCursorPos(int X, int Y);

//    [DllImport("user32.dll")]
//    static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

//    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
//    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
//    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
//    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
//    private const uint MOUSEEVENTF_WHEEL = 0x0800;

//    public static void MoveMouse(int x, int y) => SetCursorPos(x, y);
//    public static void LeftDown() => mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
//    public static void LeftUp() => mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
//    public static void RightDown() => mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
//    public static void RightUp() => mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
//    public static void LeftClick() { LeftDown(); LeftUp(); }
//    public static void RightClick() { RightDown(); RightUp(); }
//    public static void ScrollMouse(int dx, int dy) => mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)dy, UIntPtr.Zero);

//    // ---------------------------------------
//    // KEYBOARD API
//    // ---------------------------------------
//    [DllImport("user32.dll")]
//    static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

//    private const int INPUT_KEYBOARD = 1;
//    private const uint KEYEVENTF_KEYUP = 0x0002;

//    [StructLayout(LayoutKind.Sequential)]
//    struct INPUT { public uint type; public InputUnion U; }
//    [StructLayout(LayoutKind.Explicit)]
//    struct InputUnion { [FieldOffset(0)] public KEYBDINPUT ki; }
//    [StructLayout(LayoutKind.Sequential)]
//    struct KEYBDINPUT { public ushort wVk; public ushort wScan; public uint dwFlags; public uint time; public UIntPtr dwExtraInfo; }

//    public static void KeyDown(ushort key)
//    {
//        INPUT[] inputs = { new INPUT { type = INPUT_KEYBOARD, U = new InputUnion { ki = new KEYBDINPUT { wVk = key, dwFlags = 0 } } } };
//        SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
//    }

//    public static void KeyUp(ushort key)
//    {
//        INPUT[] inputs = { new INPUT { type = INPUT_KEYBOARD, U = new InputUnion { ki = new KEYBDINPUT { wVk = key, dwFlags = KEYEVENTF_KEYUP } } } };
//        SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
//    }
//}
