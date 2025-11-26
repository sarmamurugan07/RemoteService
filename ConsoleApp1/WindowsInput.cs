using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace RemoteControl.Server
{
    public static class WindowsInput
    {
        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public InputUnion U;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        const uint INPUT_MOUSE = 0;
        const uint INPUT_KEYBOARD = 1;

        const uint MOUSEEVENTF_MOVE = 0x0001;
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;

        const uint KEYEVENTF_KEYUP = 0x0002;

        public static void MoveMouse(int x, int y)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].U.mi.dx = x;
            inputs[0].U.mi.dy = y;
            inputs[0].U.mi.dwFlags = MOUSEEVENTF_MOVE;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void LeftClick()
        {
            Mouse(MOUSEEVENTF_LEFTDOWN); Thread.Sleep(40);
            Mouse(MOUSEEVENTF_LEFTUP);
        }

        public static void RightClick()
        {
            Mouse(MOUSEEVENTF_RIGHTDOWN); Thread.Sleep(40);
            Mouse(MOUSEEVENTF_RIGHTUP);
        }

        static void Mouse(uint flags)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].U.mi.dwFlags = flags;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        public static void TypeText(string text)
        {
            foreach (char c in text)
            {
                short key = VkKeyScan(c);
                KeyPress((ushort)(key & 0xFF));
            }
        }

        public static void KeyPress(ushort keyCode)
        {
            Keyboard(keyCode, false);
            Thread.Sleep(35);
            Keyboard(keyCode, true);
        }

        static void Keyboard(ushort keyCode, bool keyUp)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].U.ki.wVk = keyCode;
            inputs[0].U.ki.dwFlags = keyUp ? KEYEVENTF_KEYUP : 0;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
