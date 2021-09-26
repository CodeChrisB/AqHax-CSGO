using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AqHaxCSGO.Objects;
using AqHaxCSGO.Objects.Structs;
using static AqHaxCSGO.Objects.GlobalLists;
using AqHaxCSGO.Hacks.Features;
using System.Runtime.InteropServices;
using GameOverlay.Windows;
using GameOverlay.Drawing;

namespace AqHaxCSGO.Hacks
{
    static class WallHack
    {
        public struct RECT
        {
            public int left, top, right, bottom, width, height;
        }
        public static int i = 0;
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
                IsVisible = true,
                IsTopmost = true

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
        }
        public static void WallHackThread()
        {

            while (true)
            {
                graphics.BeginScene();
                graphics.ClearScene();
                DrawPlayerBox(10,500,400,800);
                DrawPlayerBox(600,200,50,40);


                if (!Globals.WallHackEnabled)
                {
                    Thread.Sleep(Globals.IdleWait);
                    continue;
                }
                if (!EngineDLL.InGame)
                {
                    Thread.Sleep(Globals.IdleWait);
                    continue;
                }

                int mp = EngineDLL.MaxPlayer;
                for (int i = 0; i < mp; i++)
                {
                    CBaseEntity baseEntity = entityList[i];
                    if (baseEntity == null) continue;
                    CCSPlayer entity = new CCSPlayer(baseEntity);

                    if (entity == null) continue;
                    if (entity.Dormant) continue;
                    if (entity.Health <= 0) continue;

                    if (entity.Team != CBasePlayer.Team)
                    {
                        GlowObject glowObject = entityList[i].GlowObject;
                        glowObject.r = Globals.WallHackEnemy.R / 255;
                        glowObject.g = Globals.WallHackEnemy.G / 255;
                        glowObject.b = Globals.WallHackEnemy.B / 255;
                        glowObject.a = 0.7f;
                        glowObject.m_bFullBloom = Globals.WallHackFullEnabled;
                        glowObject.BloomAmount = Globals.FullBloomAmount;
                        glowObject.m_nGlowStyle = Globals.WallHackGlowOnly ? 1 : 0;
                        glowObject.m_bRenderWhenOccluded = true;
                        glowObject.m_bRenderWhenUnoccluded = false;

                        entityList[i].GlowObject = glowObject;
                    }
                    else
                    {
                        GlowObject glowObject = entityList[i].GlowObject;
                        glowObject.r = Globals.WallHackTeammate.R / 255;
                        glowObject.g = Globals.WallHackTeammate.G / 255;
                        glowObject.b = Globals.WallHackTeammate.B / 255;
                        glowObject.a = 0.7f;
                        glowObject.m_bFullBloom = Globals.WallHackFullEnabled;
                        glowObject.BloomAmount = Globals.FullBloomAmount;
                        glowObject.m_nGlowStyle = Globals.WallHackGlowOnly ? 1 : 0;
                        glowObject.m_bRenderWhenOccluded = true;
                        glowObject.m_bRenderWhenUnoccluded = false;

                        entityList[i].GlowObject = glowObject;
                    }
                }

                graphics.EndScene();
                Thread.Sleep(Globals.UsageDelay);
            }
        }

        private static void DrawPlayerBox(int left, int top, int right, int bottom)
        {
            graphics.DrawBox2D(
                graphics.CreateSolidBrush(Color.Blue),
                graphics.CreateSolidBrush(Color.Transparent),
                new Rectangle(left,top,right,bottom), 
                5
             );
        }

        public static void RenderColorThread()
        {
            while (true)
            {
                if (!Globals.RenderEnabled)
                {
                    Thread.Sleep(Globals.IdleWait);
                    continue;
                }
                if (!EngineDLL.InGame)
                {
                    Thread.Sleep(Globals.IdleWait);
                    continue;
                }

                int mp = EngineDLL.MaxPlayer;
                for (int i = 0; i < mp; i++)
                {
                    CBaseEntity baseEntity = entityList[i];
                    if (baseEntity == null) continue;
                    CCSPlayer entity = new CCSPlayer(baseEntity);
                    if (entity == null) continue;
                    if (entity.Dormant) continue;
                    if (entity.Health <= 0) continue;

                    if (entity.Team != CBasePlayer.Team)
                    {
                        RenderColor rco = new RenderColor();
                        rco.r = Globals.RenderColor.R;
                        rco.g = Globals.RenderColor.G;
                        rco.b = Globals.RenderColor.B;
                        rco.a = 255;
                        entity.RenderColor = rco;
                    }

                    if (!Globals.RenderEnemyOnly) 
                    {
                        RenderColor rc = new RenderColor();
                        rc.r = Globals.RenderColor.R;
                        rc.g = Globals.RenderColor.G;
                        rc.b = Globals.RenderColor.B;
                        rc.a = 255;
                        entity.RenderColor = rc;
                    }

                    EngineDLL.ModelAmbientIntensity = Globals.RenderBrightness;
                }

                Thread.Sleep(Globals.UsageDelay);
            }
        }

        public static void RadarThread() 
        {
            while (true) 
            {
                if (!Globals.RadarEnabled)
                {
                    Thread.Sleep(Globals.IdleWait);
                    continue;
                }
                if (!EngineDLL.InGame)
                {
                    Thread.Sleep(Globals.IdleWait);
                    continue;
                }

                int mp = EngineDLL.MaxPlayer;
                for (int i = 0; i < mp; i++)
                {
                    CBaseEntity baseEntity = entityList[i];
                    if (baseEntity == null) continue;
                    CCSPlayer entity = new CCSPlayer(baseEntity);
                    if (entity == null) continue;
                    if (entity.Dormant) continue;
                    if (entity.Team == CBasePlayer.Team) continue;

                    if (!entity.Spotted) entity.Spotted = true;
                }

                Thread.Sleep(Globals.UsageDelay);
            }
        }
    }
}
