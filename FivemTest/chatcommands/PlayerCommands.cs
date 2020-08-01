﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.actions;
using FivemTest.entities;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FivemTest.chatcommands
{
    public class PlayerCommands : BaseScript
    {
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
                    Game.PlayerPed.Resurrect();
                }
            }), false);

            API.RegisterKeyMapping("shuffleSeat", "Shuffle", "keyboard", "h");

            API.RegisterCommand("shuffleSeat", (new Action<int>(src =>
            {
                if(Game.PlayerPed.IsInVehicle() 
                    && (API.GetPedInVehicleSeat(Game.PlayerPed.CurrentVehicle.Handle, 0) ==  Game.PlayerPed.Handle || API.GetPedInVehicleSeat(Game.PlayerPed.CurrentVehicle.Handle, 1) == Game.PlayerPed.Handle)
                    && !PedValues.shuffleSeat)
                {
                Thread thread = new Thread(PedActions.ShuffleSeatAction);
                thread.Start();
                }
            })), false);

            API.RegisterCommand("drag", (new Action<int, List<object>>((src, args) =>
            {
                List<string> argList = args.Select(o => o.ToString()).ToList();
                if(argList.Any() && argList[0].Equals("reset"))
                {
                    Debug.WriteLine("Resetting drag");
                    PedValues.attachedEntity = 0;
                    return;
                }
                if (argList.Any() && argList[0].Equals("check"))
                {
                    Debug.WriteLine("attachedEntity = " + PedValues.attachedEntity);
                    return;
                }
                if (argList.Any() && argList[0].Equals("pos"))
                {
                    Vector3 pos = API.GetEntityCoords(PedValues.attachedEntity, true);
                    Debug.WriteLine("attachedEntityPosition = x " + pos.X + " y " + pos.Y + " z " + pos.Z);
                    return;
                }
                Ped playerPed = Game.PlayerPed;
                if (PedValues.attachedEntity != 0)
                {
                    Debug.WriteLine("Detaching entity " + PedValues.attachedEntity);
                    API.DetachEntity(PedValues.attachedEntity, true, true);
                    PedValues.attachedEntity = 0;
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
                    API.AttachEntityToEntity(ped, playerPed.Handle, 4103, 0, 1, 0, 0f, 0f, 0f, true, false, false, false, 2, true);
                    PedValues.attachedEntity = ped;

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
        }

    }

}
