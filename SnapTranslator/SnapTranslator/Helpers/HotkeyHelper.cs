using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SnapTranslator.Helpers.HotkeyHelper;

namespace SnapTranslator.Helpers
{
    public class HotkeyHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // Khai báo hàm UnregisterHotKey từ user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [Flags]
        public enum Modifiers
        {
            None = 0x0,
            Alt = 0x1,
            Control = 0x2,
            Shift = 0x4,
            Win = 0x8
        }

        public static bool Register(string fsModifiers, char vk, int keyId)
        {
            uint modifiers = 0;
            var parts = fsModifiers.Split('_');

            foreach (var part in parts)
            {
                switch (part.Trim().ToLower())
                {
                    case "alt":
                        modifiers |= (uint)Modifiers.Alt;
                        break;
                    case "ctrl":
                    case "control":
                        modifiers |= (uint)Modifiers.Control;
                        break;
                    case "shift":
                        modifiers |= (uint)Modifiers.Shift;
                        break;
                    case "win":
                    case "windows":
                        modifiers |= (uint)Modifiers.Win;
                        break;
                    default:
                        throw new ArgumentException($"Không hỗ trợ modifier '{part}'.");
                }
            }

            bool isRegistered = RegisterHotKey(IntPtr.Zero, keyId, modifiers, (uint)char.ToUpper(vk));

            return isRegistered;
        }
    }
}
