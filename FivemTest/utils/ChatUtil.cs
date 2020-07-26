using CitizenFX.Core;
using System.Collections.Generic;

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
