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
            API.RegisterCommand("sv", new Action<int, List<object>>((src, args) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && Enum.TryParse(argList[0], true, out VehicleHash vehicle))
                {
                    VehicleUtil.SpawnVehicle(vehicle);
                }
            }
            ), false);

            API.RegisterCommand("fix", new Action<int>((src) =>
            {
                if (Game.PlayerPed.IsInVehicle())
                    Game.Player.LastVehicle.Repair();
            }
            ), false);

            API.RegisterCommand("impound", new Action<int>((src) =>
            {
                Vector3 pos = Game.PlayerPed.Position;
                int vehInt = API.GetClosestVehicle(pos.X, pos.Y, pos.Z, 3f, 0, 70);
                Entity entity = Entity.FromHandle(vehInt);
                if (entity == null)
                {
                    ChatUtil.SendMessageToClient("null ", "null", 255, 255, 255);
                    
                } else
                {
                    int number = API.GetVehicleNumberOfPassengers(vehInt);
                    if(!API.IsVehicleSeatFree(vehInt, -1))
                    {
                        number++;
                    }
                    API.TaskEveryoneLeaveVehicle(vehInt);
                    
                    ChatUtil.SendMessageToClient("Closest ", entity.ToString() + ", " + number, 255, 255, 255);

                    entity.Delete();
                }

            }
            ), false);
        }
    }
}
