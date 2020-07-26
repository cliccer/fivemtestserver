using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FivemTest.chatcommands
{
    static class VehicleCommands
    {
        public static void InitVehicleCommands()
        {
            API.RegisterCommand("sv", new Action<int, List<object>, string>(async (src, args, raw) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && Enum.TryParse(argList[0], true, out VehicleHash vehicle))
                {
                    VehicleUtil.SpawnVehicle(vehicle);
                }
            }
            ), false);

            API.RegisterCommand("fix", new Action<int, List<object>, string>((src, args, raw) =>
            {
                if (Game.PlayerPed.IsInVehicle())
                    Game.Player.LastVehicle.Repair();
            }
            ), false);
        }
    }
}
