using CitizenFX.Core;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.eventhandlers
{
    class GeneralEventHandlers : BaseScript
    {

        public GeneralEventHandlers()
        {
            Init();
        }


        private void Init()
        {
            EventHandlers["errorMessage"] += new Action<string>(SendErrorMessage);
        }

        private void SendErrorMessage(string message)
        {
            ChatUtil.SendMessageToClient("ERROR", message, 255, 0, 0);
        }
    }

}
