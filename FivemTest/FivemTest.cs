using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.actions;
using FivemTest.chatcommands;
using FivemTest.menus;
using FivemTest.tickactions;
using System;
using System.Threading.Tasks;

namespace FivemTest
{
    public class FivemTest : BaseScript
    {

        private bool firstTick = false;
        public FivemTest()
        {
            Tick += OnTick;
        }

        public async Task OnTick()
        {
            if(!firstTick)
            {
                firstTick = true;
                InitStartUpSettings();
                ChatCommandsMain.InitAllChatCommands();
                MenuInitializer.InitMenu();
            }

            OnTickEvents.Execute();
            
        }

        private void InitStartUpSettings()
        {            
            //Disable AI cops
            API.SetCreateRandomCops(false);
            API.SetMaxWantedLevel(0);


            //Disable auto respawn
            EventHandlers["onClientMapStart"] += new Action<string>(res => {
                Exports["spawnmanager"].setAutoSpawn(false);
                Exports["spawnmanager"].spawnPlayer();
            });
            
            
        }
    }


}
