﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClassicUO.Configuration;

namespace ClassicUO.Game.Data
{
    [Flags]
    public enum NotorietyFlag : byte
    {
        Unknown = 0x00,
        Innocent = 0x01,
        Ally = 0x02,
        Gray = 0x03,
        Criminal = 0x04,
        Enemy = 0x05,
        Murderer = 0x06,
        Invulnerable = 0x07
    }

    static class Notoriety
    {
        public static Hue GetHue(NotorietyFlag flag)
        {
            Settings settings = Service.Get<Settings>();

            switch (flag)
            {
                case NotorietyFlag.Innocent:
                    return (ushort)settings.InnocentColor;
                case NotorietyFlag.Ally:
                    return (ushort)settings.FriendColor;
                case NotorietyFlag.Criminal:
                case NotorietyFlag.Gray:
                    return (ushort)settings.CriminalColor;
                case NotorietyFlag.Enemy:
                    return (ushort)settings.EnemyColor;
                case NotorietyFlag.Murderer:
                    return (ushort)settings.MurdererColor;
                case NotorietyFlag.Invulnerable:
                    return 0x0034;
                default:
                    return 0;
            }
        }

        public static string GetHTMLHue(NotorietyFlag flag)
        {
            switch (flag)
            {
                case NotorietyFlag.Innocent:
                    return "<basefont color=\"cyan\">";
                case NotorietyFlag.Ally:
                    return "<basefont color=\"lime\">";
                case NotorietyFlag.Criminal:
                case NotorietyFlag.Gray:
                    return "<basefont color=\"gray\">";
                case NotorietyFlag.Enemy:
                    return "<basefont color=\"orange\">";
                case NotorietyFlag.Murderer:
                    return "<basefont color=\"red\">";
                case NotorietyFlag.Invulnerable:
                    return "<basefont color=\"yellow\">";
                default:
                    return string.Empty;
            }
        }

    }

}
