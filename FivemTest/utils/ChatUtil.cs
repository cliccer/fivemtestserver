﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;

namespace FivemTest.utils
{
    public class ChatUtil : BaseScript
    {
        public static void SendMessageToClient(string title, string message, int r, int g, int b)
        {
            Dictionary<string, object> msg = new Dictionary<string, object>
            {
                ["color"] = new[] { r, g, b },
                ["args"] = new[] { title, message }
            };
            TriggerEvent("chat:addMessage", msg);
        }

        public static void RemoveAllChatSuggestions()
        {
            List<dynamic> cmds = API.GetRegisteredCommands();

            foreach(dynamic cmd in cmds) {
                TriggerEvent("chat:removeSuggestion", "/" + cmd.name);
            }
        }
    }
}
