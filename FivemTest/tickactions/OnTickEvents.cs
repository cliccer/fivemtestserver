using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.entities;
using FivemTest.hud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.tickactions
{
    class OnTickEvents
    {

        public static void Execute()
        {
            DisableVehicleAirControl();
            DisableVehicleSeatAutoShuffle();

            VehicleHud.UpdateVehicleHud();
        }

        private static void DisableVehicleAirControl()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;

                if (vehicle.IsInAir && !VehicleClass.Helicopters.Equals(vehicle.ClassType) 
                    && !VehicleClass.Planes.Equals(vehicle.ClassType) && !VehicleClass.Cycles.Equals(vehicle.ClassType))
                {
                    API.DisableControlAction(0, 59, true);
                    API.DisableControlAction(0, 60, true);
                    API.DisableControlAction(0, 61, true);
                    API.DisableControlAction(0, 62, true);
                    API.DisableControlAction(0, 63, true);
                }
                
            }
        }

        private static void DisableVehicleSeatAutoShuffle()
        {

            Ped ped = Game.PlayerPed;
            if (ped.IsInVehicle() && !PedValues.shuffleSeat)
            {
                Vehicle veh = ped.CurrentVehicle;
                if (API.GetLastPedInVehicleSeat(veh.Handle, 0) == ped.Handle && API.GetIsTaskActive(ped.Handle, 165))
                {
                    API.SetPedIntoVehicle(ped.Handle, veh.Handle, 0);
                }
            }
        }
    }
}
