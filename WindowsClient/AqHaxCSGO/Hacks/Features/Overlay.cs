using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AqHaxCSGO.Hacks.Features
{
    static class Overlay
    {
        public struct RECT
        {
            public int left, top, right, bottom, width, height;
        }
        public static RECT rect;

        public static string WINDOW_NAME = "Counter-Strike: Global Offensive";
        public static IntPtr handle = FindWindow(null, WINDOW_NAME);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]

        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        public static OverlayWindow window;
        public static Graphics graphics;

        public static void SetUp()
        {
            GetWindowRect(handle, out rect);
            rect.width = rect.right - rect.left;
            rect.height = rect.bottom - rect.top;

            window = new OverlayWindow(rect.left, rect.top, rect.width, rect.height)
            {
                IsTopmost = true,
                IsVisible = true
            };

            graphics = new Graphics(IntPtr.Zero, window.Width, window.Height)
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                VSync = true,
            };

            window.Create();
            graphics.WindowHandle = window.Handle;
            graphics.Setup();


            graphics.BeginScene();
            graphics.ClearScene();
            for(int i =0;i<1000;i++)
                graphics.DrawLine(graphics.CreateSolidBrush(Color.Blue),0,0,i,i*2,5);
        }
    }
}
