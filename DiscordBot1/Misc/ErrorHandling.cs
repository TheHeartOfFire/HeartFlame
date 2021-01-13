using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Misc
{
    public class ErrorHandling
    {
        public static void DotNetCommandException(CommandError Error, SocketCommandContext Context)
        {
            switch (Error)
            {
                case CommandError.UnknownCommand:
                    Context.Channel.SendMessageAsync(":x: I'm not sure what that command is! Please check your spelling or use a help command and try again.");
                    return;

            }
        }

        public static void GlobalErrorLogging(string Error, SocketCommandContext Context)
        {
            if (PersistentData.Data.Config.Reporting.ErrorChannel > 0)
                PersistentData.Data.Config.Reporting.GlobalErrorChannel().SendMessageAsync(
                    $"{DateTime.Now} at Commands: Something went wrong while evecuting a command. Text: {Context.Message.Content} | Error: {Error}");
        }

        
    }
}
