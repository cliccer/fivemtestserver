using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.chatcommands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FivemTest
{
    public class FivemTest : BaseScript
    {

        public FivemTest()
        {
            InitStartUpSettings();
            ChatCommandsMain.InitAllChatCommands();
        }

        

        private void InitStartUpSettings()
        {            
            //Disable AI cops
            API.SetCreateRandomCops(false);
            API.SetMaxWantedLevel(0);
        }
    }


}
