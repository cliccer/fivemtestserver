using ActualServer.serverevents;
using CitizenFX.Core;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActualServer
{
    public class Main : BaseScript
    {
        private bool firstTick = false;
        public Main()
        {
            ServerEvents serverEvents = new ServerEvents();
            Tick += OnTick;
            
        }

        private async Task OnTick()
        {
            if(!firstTick)
            {
                firstTick = true;
            }
        }
    }
}
