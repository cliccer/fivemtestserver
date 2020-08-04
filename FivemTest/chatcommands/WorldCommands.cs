using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FivemTest.chatcommands
{
    public class WorldCommands : BaseScript
    {
        public static void InitWorldCommands()
        {
            API.RegisterCommand("wanted", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && int.TryParse(argList[0], out int level))
                {
                    API.SetMaxWantedLevel(level);
                    ChatUtil.SendMessageToClient("[WantedLevel]", "Max wanted level set to " + level, 255, 255, 255);
                }
            }
            ), false);

            API.RegisterCommand("wanted2", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && int.TryParse(argList[0], out int level))
                {
                    Game.WantedMultiplier = 0;
                    ChatUtil.SendMessageToClient("[WantedLevel]", "Wanted multiplier set to " + level, 255, 255, 255);
                }
            }
            ), false);

            API.RegisterCommand("announce", new Action<int, List<object>>((src, args) =>
            {
                var argList = args.Select(o => o.ToString()).ToArray();
                Debug.WriteLine("1");
                string message = string.Join(" ", argList);
                Debug.WriteLine("2" + message);
                TriggerServerEvent("serverAnnouncement", message);
                Debug.WriteLine("3");
            }
            ), false);
        }
    }
}
