using System.Runtime.InteropServices;

namespace Mindsweeper
{
    public static class Clicker
    {
        static Clicker()
        {
            var res = SetProcessDPIAware();
        }

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;

        public enum ClickType
        {
            Left,
            Right
        }

        public static void ClickAt(int x, int y, ClickType clickType)
        {
            // Move the cursor to the specified location
            SetCursorPos(x, y);

            int downEvent, upEvent;

            if (clickType == ClickType.Left)
            {
                downEvent = MOUSEEVENTF_LEFTDOWN;
                upEvent = MOUSEEVENTF_LEFTUP;
            }
            else // Right click
            {
                downEvent = MOUSEEVENTF_RIGHTDOWN;
                upEvent = MOUSEEVENTF_RIGHTUP;
            }

            mouse_event(downEvent, x, y, 0, 0);
            mouse_event(upEvent, x, y, 0, 0);
        }

        public static void LeftClickAt(int x, int y)
        {
            ClickAt(x, y, ClickType.Left);
        }

        public static void RightClickAt(int x, int y)
        {
            ClickAt(x, y, ClickType.Right);
        }
    }
}
