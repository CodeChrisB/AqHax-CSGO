﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using AqHaxCSGO.Objects;
using AqHaxCSGO.Objects.Structs;
using static System.Math;
using static AqHaxCSGO.Objects.GlobalLists;
using AqHaxCSGO.Hacks.Features;

namespace AqHaxCSGO.Hacks
{
    static class Aimbot
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int GetAsyncKeyState(
            int vKey
        );

        public static void TriggerThread()
        {
            while (true)
            {
                if (!Globals.TriggerEnabled && !Globals.AimShootOnCollide)
                {
                    Thread.Sleep(Globals.IdleWait);
                    continue;
                }
                if (!Engine.InGame)
                {
                    Thread.Sleep(Globals.IdleWait);
                    continue;
                }

                if (Globals.TriggerPressOnlyEnabled && !Globals.AimShootOnCollide)
                {
                    if ((GetAsyncKeyState(Globals.TriggerKey) & 0x8000) > 0)
                    {
                        if (LocalPlayer.CrosshairID > 0 && LocalPlayer.CrosshairID < Engine.MaxPlayer + 2)
                        {
                            Entity crossEntity = entityList[LocalPlayer.CrosshairID - 1];
                            if (crossEntity == null) continue; // TRIGGER BOT CRASH FIX
                            if (crossEntity != null && crossEntity.Team != LocalPlayer.Team)
                            {
                                Thread.Sleep(1);
                                Client.ForceAttack(true);
                                Thread.Sleep(5);
                                Client.ForceAttack(false);
                            }
                        }
                    }
                }
                else
                {
                    if (LocalPlayer.CrosshairID > 0 && LocalPlayer.CrosshairID < Engine.MaxPlayer + 2)
                    {
                        Entity crossEntity = entityList[LocalPlayer.CrosshairID - 1];
                        if (crossEntity == null) continue; // TRIGGER BOT CRASH FIX
                        if (crossEntity != null && crossEntity.Team != LocalPlayer.Team)
                        {
                            Thread.Sleep(1);
                            Client.ForceAttack(true);
                            Thread.Sleep(5);
                            Client.ForceAttack(false);
                        }
                    }
                }

                Thread.Sleep(2);
            }
        }

        public static void AimbotThread()
        {
            while (true)
            {
                if (!Globals.AimEnabled)
                {
                    Thread.Sleep(Globals.IdleWait);
                    continue;
                }
                if (!Engine.InGame)
                {
                    Thread.Sleep(Globals.IdleWait);
                    continue;
                }

                int mp = Engine.MaxPlayer;
                Rectangle screen = Misc.GetWindowRect();
                Vector2 screenOrigin = new Vector2(screen.Width / 2, screen.Height / 2);
                double latestDistance = screen.Width;
                Vector3 closestEntityPos = new Vector3(99999f, 0f, 0f);
                for (int i = 0; i < mp; i++)
                {
                    if (entityList[i] == null) continue;
                    if (entityList[i].Dormant) continue;
                    if (entityList[i].Health <= 0) continue;
                    if (entityList[i].Team == LocalPlayer.Team) continue;

                    Vector3 entSelectedPos = entityList[i].GetBonePosition((int)Globals.AimPosition);
                    Vector2 entPosOnScreen;
                    if (entSelectedPos.PointOnScreen(out entPosOnScreen))
                    {
                        if (entPosOnScreen.x > screen.Width || entPosOnScreen.x < 0 || entPosOnScreen.y > screen.Height || entPosOnScreen.y < 0)
                        {
                            continue;
                        }
                    }
                    else continue;

                    double dist = Sqrt(Pow(screenOrigin.x - entPosOnScreen.x, 2) + Pow(screenOrigin.y - entPosOnScreen.y, 2));
                    if (dist < latestDistance)
                    {
                        latestDistance = dist;
                        closestEntityPos = entSelectedPos;
                    }
                }

                if (closestEntityPos.x != 99999f && (GetAsyncKeyState(Globals.TriggerKey) & 0x8000) > 0)
                {
                    Angle AimAt = CalcAngle(LocalPlayer.VectorEyeLevel, closestEntityPos);

                    if (Globals.AimRecoil)
                    {
                        Angle Punch = LocalPlayer.ViewPunchAngle * 2.0f;
                        AimAt.x -= Punch.x;
                        AimAt.y -= Punch.y;
                    }

                    LocalPlayer.ViewAngle = AimAt;

                    if (!Globals.AimShootOnCollide)
                    {
                        if (weaponList.ActiveWeapon.IsSniper())
                        {
                            Client.ForceRightAttack(true);
                            Thread.Sleep(2);
                            Client.ForceAttack(true);
                            Thread.Sleep(5);
                            Client.ForceRightAttack(false);
                            Client.ForceAttack(false);
                        }
                        else
                        {
                            Thread.Sleep(1);
                            Client.ForceAttack(true);
                            Thread.Sleep(5);
                            Client.ForceAttack(false);
                        }
                    }
                }

                Thread.Sleep(Globals.UsageDelay);
            }
        }

        static Angle CalcAngle(Vector3 src, Vector3 dst)
        {

            Angle angles;
            Vector3 e = new Vector3(dst.x - src.x, dst.y - src.y, dst.z - src.z);
            float eh = (float)Sqrt(e.x * e.x + e.y * e.y);

            angles.x = (float)(Atan2(-e.z, eh) * 180 / PI);
            angles.y = (float)(Atan2(e.y, e.x) * 180 / PI);

            return angles;
        }
    }
}
