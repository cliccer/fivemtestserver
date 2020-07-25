using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.utils
{
    public class ChatUtil : BaseScript
    {
        public static void SendMessageToClient(string title, string message, int r, int g, int b)
        {
            var msg = new Dictionary<string, object>
            {
                ["color"] = new[] { r, g, b },
                ["args"] = new[] { title, message }
            };
            TriggerEvent("chat:addMessage", msg);
        }
    }
}
