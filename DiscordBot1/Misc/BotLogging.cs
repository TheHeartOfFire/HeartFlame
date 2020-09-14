using Discord;
using Discord.WebSocket;

namespace DiscordBot1.Misc
{
    public class BotLogging
    {
        public static async void PrintLogMessage(string Source, string Action, string Message, SocketGuildUser User = null)
        {
            EmbedBuilder Embed = new EmbedBuilder
            {
                Color = Color.DarkRed
            };
            Embed.WithAuthor("Log");
            Embed.WithDescription($"Source: {Source}\n\nAction: {Action}");
            string Actor = "Celestial Imperium Bot";
            if (User != null)
                Actor = User.Username;
            Embed.AddField(Message, $"This action was taken by: {Actor}");

            var IDs = Configuration.Configuration.bot.LogChannel;
            foreach (var id in IDs)
            {
                await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendMessageAsync("", false, Embed.Build());
            }
        }
    }
}