﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.chatcommands
{
    static class ChatCommandsMain
    {
        public static void InitAllChatCommands()
        {
            PlayerPedCommands.InitPlayerPedCommands();
            VehicleCommands.InitVehicleCommands();
            WorldCommands.InitWorldCommands();
        }
    }
}
