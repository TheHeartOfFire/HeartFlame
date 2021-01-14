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
            GlobalErrorLogging(Error.ToString(), Context);

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
                    $"Time: {DateTime.UtcNow.AddHours(-6)}\n" +
                    $"Location: {Context.Guild.Name}\n" +
                    $"Text: {Context.Message.Content}\n" +
                    $"Message: {Error}");
        }

        public static void GlobalErrorLogging(string Error, string Source)
        {
            if (PersistentData.Data.Config.Reporting.ErrorChannel > 0)
                PersistentData.Data.Config.Reporting.GlobalErrorChannel().SendMessageAsync(
                    $"Time: {DateTime.UtcNow.AddHours(-6)}\n" +
                    $"Location: {Source}\n" +
                    $"Message: {Error}");
        }
    }
}
