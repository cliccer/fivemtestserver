using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.entities;
using FivemTest.hud;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.tickactions
{
    class OnTickEvents : BaseScript
    {

        public static void Execute()
        {
            DisableVehicleAirControl();
            DisableVehicleSeatAutoShuffle();
            StopEnterVehicle();
            KeepEngineRunning();
            DisableAutoEngineStartInHeli();

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
                if (ped.IsInHeli && veh.GetPedOnSeat(VehicleSeat.RightFront) == ped && !API.GetIsTaskActive(ped.Handle, 164) && !API.GetIsTaskActive(ped.Handle, 2))
                {
                    API.SetPedIntoVehicle(ped.Handle, veh.Handle, 0);
                }
                if (API.GetLastPedInVehicleSeat(veh.Handle, 0) == ped.Handle && API.GetIsTaskActive(ped.Handle, 165))
                {
                    API.SetPedIntoVehicle(ped.Handle, veh.Handle, 0);
                } else if (API.GetLastPedInVehicleSeat(veh.Handle, 1) == ped.Handle && API.GetIsTaskActive(ped.Handle, 165))
                {
                    API.SetPedIntoVehicle(ped.Handle, veh.Handle, 1);
                }
            }
        }

        private static void StopEnterVehicle()
        {

            if(API.IsControlJustReleased(0, 33) || API.IsControlJustReleased(0, 35)
                || API.IsControlJustReleased(0, 32) || API.IsControlJustReleased(0, 34))
            {
                if (API.GetIsTaskActive(Game.PlayerPed.Handle, 162))
                {
                    API.ClearPedTasksImmediately(Game.PlayerPed.Handle);
                }
            }
        }

        private static async void KeepEngineRunning()
        {
            if (Game.PlayerPed.IsInVehicle() )
            {
                Vehicle vehicle = Game.PlayerPed.CurrentVehicle;
                if(vehicle.GetPedOnSeat(VehicleSeat.Driver) == Game.PlayerPed && vehicle.IsEngineRunning)
                {
                    await Delay(1000);
                    if(!Game.PlayerPed.IsInVehicle() && !vehicle.IsEngineRunning)
                    {
                        API.SetVehicleEngineOn(vehicle.Handle, true, true, true);
                    }
                    
                }
            }
        }

        private static void DisableAutoEngineStartInHeli()
        {
            if (Game.PlayerPed.IsInHeli && !Game.PlayerPed.CurrentVehicle.IsEngineRunning)
            {
                API.DisableControlAction(25, 87, true);
                API.DisableControlAction(25, 88, true);
                API.DisableControlAction(25, 89, true);
                API.DisableControlAction(25, 90, true);
            }
        }
    }
}
