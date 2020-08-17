using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.actions;
using FivemTest.entities;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FivemTest.chatcommands
{
    public class PlayerCommands : BaseScript
    {

        public PlayerCommands()
        {
            API.RegisterCommand("respawn", new Action<int>(src =>
            {
                Exports["spawnmanager"].spawnPlayer();
            }), false);
        }
        public static void InitPlayerPedCommands()
        {
            API.RegisterCommand("gw", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && Enum.TryParse(argList[0], true, out WeaponHash weapon))
                {
                    Game.PlayerPed.Weapons.Give(weapon, 250, true, false);
                }
                else
                {
                    ChatUtil.SendMessageToClient("Error", "Usage : /gw [WeaponName]", 255, 0, 0);
                }
            }


            ), false);


            API.RegisterCommand("heal", new Action<int, List<object>, string>((src, args, raw) =>
            {
                Game.PlayerPed.Armor = Game.Player.MaxArmor;
                Game.PlayerPed.Health = Game.PlayerPed.MaxHealth;
                Game.PlayerPed.ResetVisibleDamage();
            }
            ), false);

            API.RegisterCommand("killme", new Action<int, List<object>, string>((src, args, raw) =>
            {
                Game.PlayerPed.Kill();
            }
            ), false);


            API.RegisterCommand("tp", new Action<int, List<object>, string>((src, args, raw) =>
            {
                int waypoint = API.GetFirstBlipInfoId(8);

                Vector3 waypointCoords = API.GetBlipCoords(waypoint);

                Debug.WriteLine("Waypointcoords " + waypointCoords.X + waypointCoords.Y + waypointCoords.Z);

                float height = 10000f;

                API.GetGroundZFor_3dCoord(waypointCoords[0], waypointCoords[1], height, ref height, false);
                Game.PlayerPed.Position = new Vector3(waypointCoords[0], waypointCoords[1], height + 2f);

            }
            ), false);


            API.RegisterCommand("coords", new Action<int>(src =>
            {
                Vector3 playerLocation = Game.PlayerPed.Position;
                ChatUtil.SendMessageToClient("[Coords", "Your coordinates are x " + playerLocation.X + " y " + playerLocation.Y + " z " + playerLocation.Z, 255, 255, 255);
            }), false);

            API.RegisterCommand("interior", new Action<int>(src =>
            {
                Vector3 playerLocation = Game.PlayerPed.Position;
                int interiorId = API.GetInteriorAtCoords(playerLocation.X, playerLocation.Y, playerLocation.Z);

                ChatUtil.SendMessageToClient("[Interior]", "Current playerposition interiorID = " + interiorId, 255, 255, 255);
            }), false);

            API.RegisterCommand("revive", new Action<int>(src =>
            {
                if (Game.PlayerPed.IsDead)
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        Vehicle veh = Game.PlayerPed.CurrentVehicle;
                        API.ClearAllPedVehicleForcedSeatUsage(Game.Player.Handle);
                    }
                    Game.PlayerPed.Resurrect();
                }
            }), false);

            API.RegisterKeyMapping("shuffleSeat", "Shuffle", "keyboard", "h");

            API.RegisterCommand("shuffleSeat", (new Action<int>(async src =>
            {
                if(Game.PlayerPed.IsInVehicle() 
                    && (API.GetPedInVehicleSeat(Game.PlayerPed.CurrentVehicle.Handle, 0) ==  Game.PlayerPed.Handle || API.GetPedInVehicleSeat(Game.PlayerPed.CurrentVehicle.Handle, 1) == Game.PlayerPed.Handle)
                    && !PlayerValues.shuffleSeat)
                {
                    PlayerValues.shuffleSeat = true;
                    Debug.WriteLine("shuffleSeat = true");
                    await Delay(4000);
                    PlayerValues.shuffleSeat = false;
                    Debug.WriteLine("shuffleSeat = false");
                }
            })), false);

            API.RegisterCommand("drag", (new Action<int, List<object>>((src, args) =>
            {
                List<string> argList = args.Select(o => o.ToString()).ToList();
                if(argList.Any() && argList[0].Equals("reset"))
                {
                    Debug.WriteLine("Resetting drag");
                    PlayerValues.attachedEntity = 0;
                    return;
                }
                if (argList.Any() && argList[0].Equals("check"))
                {
                    Debug.WriteLine("attachedEntity = " + PlayerValues.attachedEntity);
                    return;
                }
                if (argList.Any() && argList[0].Equals("pos"))
                {
                    Vector3 pos = API.GetEntityCoords(PlayerValues.attachedEntity, true);
                    Debug.WriteLine("attachedEntityPosition = x " + pos.X + " y " + pos.Y + " z " + pos.Z);
                    return;
                }
                Ped playerPed = Game.PlayerPed;
                if (PlayerValues.attachedEntity != 0)
                {
                    Debug.WriteLine("Detaching entity " + PlayerValues.attachedEntity);
                    API.DetachEntity(PlayerValues.attachedEntity, true, true);
                    PlayerValues.attachedEntity = 0;
                    return;
                }
                Vector3 playerPos = playerPed.Position;
                Vector3 playerOffset = API.GetOffsetFromEntityGivenWorldCoords(playerPed.Handle, 0, 5f, 0);
                int rayHandle = API.StartShapeTestCapsule(playerPos.X, playerPos.Y, playerPos.Z, playerOffset.X, playerOffset.Y, playerOffset.Z, 1, 12, playerPed.Handle, 7);
                bool hit = false;
                Vector3 endPoint = playerPos;
                Vector3 surfaceNormal = playerPos;
                int ped = 0;
                API.GetShapeTestResult(rayHandle, ref hit, ref endPoint, ref surfaceNormal, ref ped);
                
                if (ped != 0)
                {
                    Debug.WriteLine("Would try to attach " + ped + " to " + playerPed.Handle);
                    API.AttachEntityToEntity(ped, playerPed.Handle, 4103, 0, 0.7f, 0, 0f, 0f, 0f, true, false, false, false, 2, true);
                    PlayerValues.attachedEntity = ped;
                } else
                {
                    Debug.WriteLine("shit");
                }

            })),false);

            API.RegisterCommand("e", new Action<int, List<object>>((src, args) =>
            {
                List<string> argList = args.Select(o => o.ToString()).ToList();
                if (Game.PlayerPed.IsInVehicle())
                {
                    ChatUtil.SendMessageToClient("[Error]", "You cannot perform an emote while in a vehicle.", 255, 255, 255);
                    return;
                } else
                {
                    string emote = Emote.Get(argList[0]);
                    if (emote != null)
                    {
                        API.TaskStartScenarioInPlace(Game.PlayerPed.Handle, emote, 0, true);
                    } else
                    {
                        ChatUtil.SendMessageToClient("[Error]", "Invalid emote specified", 255, 255, 255);
                    }
                }
            }), false);

            API.RegisterKeyMapping("cancelEmote", "Cancel emote", "keyboard", "space");

            API.RegisterCommand("cancelEmote", new Action<int>(src =>
            {
                if (API.IsPedActiveInScenario(Game.PlayerPed.Handle))
                {
                    API.ClearPedTasksImmediately(Game.PlayerPed.Handle);
                }
            }), false);

            API.RegisterKeyMapping("enterClosestVehicleDoor", "Enter the closest vehicle door", "keyboard", "f");

            API.RegisterCommand("enterClosestVehicleDoor", new Action<int>(src =>
            {
                if (Game.PlayerPed.IsInVehicle())
                {
                    return;
                }

                if (API.IsControlJustPressed(1, 23))
                {
                    Vector3 playerPos = Game.PlayerPed.Position;
                    int veh = API.GetClosestVehicle(playerPos.X, playerPos.Y, playerPos.Z, 3f, 0, 70);

                    if (veh != 0)
                    {
                        Vehicle vehicle = new Vehicle(veh);
                        API.SetVehicleEngineOn(veh, vehicle.IsEngineRunning, false, true);
                        if (!VehicleClass.Motorcycles.Equals(vehicle.ClassType) && !VehicleClass.Cycles.Equals(vehicle.ClassType))
                        {
                            int closestDoor = VehicleUtil.GetClosestVehicleDoor(veh, playerPos);
                            API.TaskEnterVehicle(Game.PlayerPed.Handle, veh, 10000, closestDoor, 2.0f, 1, 0);
                        }

                    }
                }
            }), false);

            API.RegisterCommand("piv", new Action<int>(async src =>
            {
                
                Vector3 playerPos = Game.PlayerPed.Position;
                int veh = API.GetClosestVehicle(playerPos.X, playerPos.Y, playerPos.Z, 2.5f, 0, 70);
                if(veh != 0)
                {
                    Vehicle vehicle = new Vehicle(veh);
                    if(!VehicleClass.Motorcycles.Equals(vehicle.ClassType) && !VehicleClass.Cycles.Equals(vehicle.ClassType))
                    {
                        int closestSeat = VehicleUtil.GetClosestVehicleDoor(veh, playerPos);
                        int closestDoor = closestSeat + 1;
                        int ped = API.GetPedInVehicleSeat(veh, closestSeat);

                        if(PlayerValues.attachedEntity == 0)
                        {
                            if(ped == 0)
                            {
                                return;
                            }
                            API.TaskLeaveVehicle(ped, veh, 16);
                            await Delay(200);
                            API.AttachEntityToEntity(ped, Game.PlayerPed.Handle, 4103, 0, 0.7f, 0, 0f, 0f, 0f, true, false, false, false, 2, true);
                            PlayerValues.attachedEntity = ped;
                        } else 
                        {
                            if(ped != 0)
                            {
                                return;
                            }
                            vehicle.Doors[VehicleUtil.GetVehicleDoorIndexFromSeatIndex(closestDoor)].Open();
                            await Delay(200);
                            API.DetachEntity(PlayerValues.attachedEntity, true, true);
                            API.SetPedIntoVehicle(PlayerValues.attachedEntity, veh, closestSeat);
                            PlayerValues.attachedEntity = 0;
                        }

                        await Delay(500);
                        vehicle.Doors[VehicleUtil.GetVehicleDoorIndexFromSeatIndex(closestDoor)].Close();
                    }
                }
            }), false);

            API.RegisterCommand("invis", new Action<int>(src =>
            {
                API.SetPlayerInvisibleLocally(Game.Player.Handle, true);
            }), false);

            
            API.RegisterCommand("character", new Action<int, List<object>>( (src, args) =>
            {
                List<string> argList = args.Select(o => o.ToString()).ToList();

                if (!argList.Any())
                {
                    ChatUtil.SendMessageToClient("ERROR", "No argument given, usage: /character [create, list, change]", 255, 0, 0);
                } else if (argList[0].Equals("create"))
                {
                    if(argList.Count >= 3)
                    {
                        if (argList[1].All(Char.IsLetter) && argList[2].All(Char.IsLetter))
                        {
                            Debug.WriteLine("Sending saveNewCharacter event to server");
                            TriggerServerEvent("saveNewCharacter", argList[1], argList[2]);
                            return;
                        }
                    }
                    ChatUtil.SendMessageToClient("ERROR", "Wrong input, usage: /character create [firstname(a-z)] [lastname(a-z)]", 255, 0, 0);
                } else if (argList[0].Equals("change"))
                {
                    if(argList.Count >= 2)
                    {
                        if (argList[1].All(Char.IsNumber))
                        {
                            TriggerServerEvent("changeCharacter", argList[1]);
                        }
                    } else
                    {
                        ChatUtil.SendMessageToClient("ERROR", "Wrong input, usage: /character change [id]", 255, 0, 0);
                    }
                } else if (argList[0].Equals("list"))
                {
                    TriggerServerEvent("listCharactersForAccount");
                } else if (argList[0].Equals("current"))
                {
                    TriggerServerEvent("currentCharacter");
                }
            }), false);
        }

    }

}
