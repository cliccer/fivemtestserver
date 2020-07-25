using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    var model = new Model(vehicle);
                    var veh = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0f, 5f, 2f)), Game.PlayerPed.Heading);
                    veh.NeedsToBeHotwired = false;
                    veh.Mods.InstallModKit();
                    veh.Mods[VehicleModType.Engine].Index = veh.Mods[VehicleModType.Engine].ModCount - 1;
                    veh.Mods[VehicleModType.Brakes].Index = veh.Mods[VehicleModType.Brakes].ModCount - 1;
                    veh.Mods[VehicleModType.Transmission].Index = veh.Mods[VehicleModType.Transmission].ModCount - 1;

                    veh.Mods[VehicleToggleModType.Turbo].IsInstalled = true;
                    veh.Mods[VehicleToggleModType.XenonHeadlights].IsInstalled = true;


                    veh.Rotation = Game.PlayerPed.Rotation + new Vector3(0f, 0f, 90f);
                    ChatUtil.SendMessageToClient("[VehicleSpawner]", "You spawned a " + vehicle.ToString(), 255, 255, 255);
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
