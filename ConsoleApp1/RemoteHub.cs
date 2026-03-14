using Microsoft.AspNetCore.SignalR;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;

namespace RemoteControl.Server
{
    public class RemoteHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendControl(string json)
        {
            try
            {
                ControlEvent? ev = JsonSerializer.Deserialize<ControlEvent>(json);
                if (ev == null)
                {
                    Console.WriteLine("ERROR: Failed to parse ControlEvent");
                    return;
                }

                if (ev.Type.StartsWith("mouse") || ev.Type.StartsWith("key"))
                {
                    HandleControlEvent(ev);
                }
                else
                {
                    await Clients.Others.SendAsync("ReceiveControl", json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in SendControl: " + ex);
            }
        }

        private void HandleControlEvent(ControlEvent ev)
        {
            try
            {
                string keyString = ev.GetKeyString();
                bool isTypingCharacter = (keyString.Length == 1 && !string.IsNullOrEmpty(keyString)) || keyString == " ";

                if (ev.Type == "key-down" && isTypingCharacter && !ev.Ctrl && !ev.Alt)
                {
                    char charToType = keyString == " " ? ' ' : keyString[0];
                    WindowsInputNative.TypeCharacter(charToType);
                    return;
                }

                if (ev.Type == "key-up" && isTypingCharacter && !ev.Ctrl && !ev.Alt)
                {
                    return;
                }

                switch (ev.Type)
                {
                    case "mouse-move": 
                        WindowsInputNative.MoveMouse((int)(ev.X.Value * Screen.PrimaryScreen.Bounds.Width), (int)(ev.Y.Value * Screen.PrimaryScreen.Bounds.Height)); 
                        break;
                    case "mouse-down":
                        if (ev.Button == 0) WindowsInputNative.LeftDown();
                        else if (ev.Button == 2) WindowsInputNative.RightDown();
                        break;
                    case "mouse-up":
                        if (ev.Button == 0) WindowsInputNative.LeftUp();
                        else if (ev.Button == 2) WindowsInputNative.RightUp();
                        break;
                    case "mouse-wheel": 
                        if (ev.DeltaY.HasValue) WindowsInputNative.MouseWheel((int)(-ev.DeltaY.Value)); 
                        break;
                    case "mouse-rightclick": 
                        WindowsInputNative.RightDown(); 
                        WindowsInputNative.RightUp(); 
                        break;
                    case "key-down": 
                        KeyDown(ev); 
                        break;
                    case "key-up": 
                        KeyUp(ev); 
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR handling event '" + ev.Type + "': " + ex);
            }
        }

        private void KeyDown(ControlEvent ev)
        {
            var (vk, shift, ctrl, alt) = ev.GetVirtualKeyWithModifiers();
            if (vk != 0)
            {
                WindowsInputNative.KeyDown(vk, shift, ctrl, alt);
            }
        }

        private void KeyUp(ControlEvent ev)
        {
            var (vk, shift, ctrl, alt) = ev.GetVirtualKeyWithModifiers();
            if (vk != 0)
            {
                WindowsInputNative.KeyUp(vk, shift, ctrl, alt);
            }
        }
    }

    public class ControlEvent
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "";
        [JsonPropertyName("x")] public double? X { get; set; }
        [JsonPropertyName("y")] public double? Y { get; set; }
        [JsonPropertyName("button")] public int? Button { get; set; }
        [JsonPropertyName("deltaX")] public double? DeltaX { get; set; }
        [JsonPropertyName("deltaY")] public double? DeltaY { get; set; }
        [JsonPropertyName("key")] public JsonElement? Key { get; set; }
        [JsonPropertyName("code")] public string Code { get; set; } = "";
        [JsonPropertyName("ctrl")] public bool Ctrl { get; set; }
        [JsonPropertyName("shift")] public bool Shift { get; set; }
        [JsonPropertyName("alt")] public bool Alt { get; set; }
        [JsonPropertyName("meta")] public bool Meta { get; set; }

        public static readonly Dictionary<string, ushort> PunctuationMap = new Dictionary<string, ushort>
        {
            ["1"] = (ushort)Keys.D1, ["2"] = (ushort)Keys.D2, ["3"] = (ushort)Keys.D3,
            ["4"] = (ushort)Keys.D4, ["5"] = (ushort)Keys.D5, ["6"] = (ushort)Keys.D6,
            ["7"] = (ushort)Keys.D7, ["8"] = (ushort)Keys.D8, ["9"] = (ushort)Keys.D9,
            ["0"] = (ushort)Keys.D0, ["!"] = (ushort)Keys.D1, ["@"] = (ushort)Keys.D2,
            ["#"] = (ushort)Keys.D3, ["$"] = (ushort)Keys.D4, ["%"] = (ushort)Keys.D5,
            ["^"] = (ushort)Keys.D6, ["&"] = (ushort)Keys.D7, ["*"] = (ushort)Keys.D8,
            ["("] = (ushort)Keys.D9, [")"] = (ushort)Keys.D0, ["-"] = (ushort)Keys.OemMinus,
            ["_"] = (ushort)Keys.OemMinus, ["="] = 0xBB, ["+"] = 0xBB,
            [","] = (ushort)Keys.Oemcomma, ["<"] = (ushort)Keys.Oemcomma,
            ["."] = (ushort)Keys.OemPeriod, [">"] = (ushort)Keys.OemPeriod,
            ["/"] = 0xBF, ["?"] = 0xBF, ["`"] = (ushort)Keys.Oemtilde,
            ["~"] = (ushort)Keys.Oemtilde, ["["] = (ushort)Keys.OemOpenBrackets,
            ["{"] = (ushort)Keys.OemOpenBrackets, ["]"] = (ushort)Keys.OemCloseBrackets,
            ["}"] = (ushort)Keys.OemCloseBrackets, ["\\"] = (ushort)Keys.OemBackslash,
            ["|"] = (ushort)Keys.OemBackslash, [";"] = 0xBA, [":"] = 0xBA,
            ["'"] = 0xDE, ["\""] = 0xDE,
        };

        public (ushort vk, bool shift, bool ctrl, bool alt) GetVirtualKeyWithModifiers()
        {
            ushort vk = 0;
            bool shift = Shift;
            bool ctrl = Ctrl;
            bool alt = Alt;
            string key = GetKeyString();

            if (!string.IsNullOrEmpty(Code))
            {
                vk = CodeToVirtualKey(Code);
            }
            else
            {
                vk = KeyStringToVirtualKey(key);
            }

            if (Ctrl || Alt)
            {
                if (char.IsLetter(key, 0) && key.Length == 1)
                {
                    if (Enum.TryParse<Keys>(key.ToUpper(), out var resultKey))
                    {
                        vk = (ushort)(int)resultKey;
                    }
                }
                else if (PunctuationMap.TryGetValue(key, out ushort punctuationVk))
                {
                    vk = punctuationVk;
                }
            }

            if (!(Ctrl || Alt) && (key.Length == 1 || key == " "))
            {
                shift = false;
            }

            return (vk, shift, ctrl, alt);
        }

        public string GetKeyString()
        {
            if (Key == null) return "";
            if (Key.Value.ValueKind == JsonValueKind.String)
                return Key.Value.GetString() ?? "";
            return Key.Value.GetRawText().Trim('"');
        }

        private ushort KeyStringToVirtualKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return 0;

            return key switch
            {
                "Enter" => (ushort)Keys.Return,
                "Tab" => (ushort)Keys.Tab,
                "Escape" => (ushort)Keys.Escape,
                "Backspace" => (ushort)Keys.Back,
                _ => 0
            };
        }

        private ushort CodeToVirtualKey(string code)
        {
            if (code.StartsWith("Key") && code.Length == 4)
            {
                char c = code[3];
                if (char.IsLetter(c))
                {
                    if (Enum.TryParse<Keys>(c.ToString().ToUpper(), out var resultKey))
                    {
                        return (ushort)(int)resultKey;
                    }
                }
            }

            return code switch
            {
                "ShiftLeft" or "ShiftRight" => (ushort)Keys.ShiftKey,
                "ControlLeft" or "ControlRight" => (ushort)Keys.ControlKey,
                "AltLeft" or "AltRight" => (ushort)Keys.Menu,
                "Enter" => (ushort)Keys.Return,
                "Tab" => (ushort)Keys.Tab,
                "Delete" => (ushort)Keys.Delete,
                "Backspace" => (ushort)Keys.Back,
                "ArrowUp" => (ushort)Keys.Up,
                "ArrowDown" => (ushort)Keys.Down,
                "ArrowLeft" => (ushort)Keys.Left,
                "ArrowRight" => (ushort)Keys.Right,
                "F1" => (ushort)Keys.F1, "F2" => (ushort)Keys.F2, "F3" => (ushort)Keys.F3,
                "F4" => (ushort)Keys.F4, "F5" => (ushort)Keys.F5, "F6" => (ushort)Keys.F6,
                "F7" => (ushort)Keys.F7, "F8" => (ushort)Keys.F8, "F9" => (ushort)Keys.F9,
                "F10" => (ushort)Keys.F10, "F11" => (ushort)Keys.F11, "F12" => (ushort)Keys.F12,
                "Insert" => (ushort)Keys.Insert,
                "PrintScreen" => (ushort)Keys.PrintScreen,
                "Home" => (ushort)Keys.Home,
                "End" => (ushort)Keys.End,
                "PageUp" => (ushort)Keys.PageUp,
                "PageDown" => (ushort)Keys.Next,
                _ => 0
            };
        }
    }

    public static class WindowsInputNative
    {
        [DllImport("user32.dll")]
        static extern uint SendInput(uint nInputs, INPUT[] inputs, int size);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint LD = 0x0002, LU = 0x0004, RD = 0x0008, RU = 0x0010, WHEEL = 0x0800;

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT { public int type; public InputData Data; }
        [StructLayout(LayoutKind.Explicit)]
        public struct InputData { [FieldOffset(0)] public MOUSEINPUT mi; [FieldOffset(0)] public KEYBDINPUT ki; [FieldOffset(0)] public HARDWAREINPUT hi; }
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT { public int dx; public int dy; public uint mouseData; public uint dwFlags; public uint time; public IntPtr dwExtraInfo; }
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT { public ushort wVk; public ushort wScan; public uint dwFlags; public uint time; public IntPtr dwExtraInfo; }
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT { public uint uMsg; public ushort wParamL; public ushort wParamH; }

        public static void MoveMouse(int x, int y) => SetCursorPos(x, y);

        private static void SendMouseEvent(uint flags, uint data = 0)
        {
            INPUT input = new INPUT
            {
                type = INPUT_MOUSE,
                Data = new InputData { mi = new MOUSEINPUT { dwFlags = flags, mouseData = data, time = 0, dwExtraInfo = IntPtr.Zero } }
            };
            SendInput(1, new[] { input }, Marshal.SizeOf<INPUT>());
        }

        public static void LeftDown() => SendMouseEvent(LD);
        public static void LeftUp() => SendMouseEvent(LU);
        public static void RightDown() => SendMouseEvent(RD);
        public static void RightUp() => SendMouseEvent(RU);
        public static void MouseWheel(int delta) => SendMouseEvent(WHEEL, (uint)delta);

        private static INPUT CreateVkInput(ushort vk, bool keyUp) =>
            new INPUT
            {
                type = INPUT_KEYBOARD,
                Data = new InputData
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = vk,
                        wScan = 0,
                        dwFlags = keyUp ? KEYEVENTF_KEYUP : 0,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

        private static INPUT CreateUnicodeInput(char character, bool keyUp) =>
            new INPUT
            {
                type = INPUT_KEYBOARD,
                Data = new InputData
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = character,
                        dwFlags = (keyUp ? KEYEVENTF_KEYUP : 0) | KEYEVENTF_UNICODE,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

        public static void TypeCharacter(char character)
        {
            INPUT inputDown = CreateUnicodeInput(character, false);
            INPUT inputUp = CreateUnicodeInput(character, true);
            INPUT[] inputs = new INPUT[] { inputDown, inputUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>());
            Thread.Sleep(20);
        }

        public static void KeyDown(ushort vk, bool shift, bool ctrl, bool alt)
        {
            var inputs = new List<INPUT>();
            if (shift) inputs.Add(CreateVkInput((ushort)Keys.ShiftKey, false));
            if (ctrl) inputs.Add(CreateVkInput((ushort)Keys.ControlKey, false));
            if (alt) inputs.Add(CreateVkInput((ushort)Keys.Menu, false));
            inputs.Add(CreateVkInput(vk, false));
            SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf<INPUT>());
        }

        public static void KeyUp(ushort vk, bool shift, bool ctrl, bool alt)
        {
            var inputs = new List<INPUT>();
            inputs.Add(CreateVkInput(vk, true));
            if (alt) inputs.Add(CreateVkInput((ushort)Keys.Menu, true));
            if (ctrl) inputs.Add(CreateVkInput((ushort)Keys.ControlKey, true));
            if (shift) inputs.Add(CreateVkInput((ushort)Keys.ShiftKey, true));
            SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf<INPUT>());
        }
    }
}
