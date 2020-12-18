using Discord;
using Discord.WebSocket;
using HeartFlame.GuildControl;

namespace HeartFlame.Misc
{
    public class BotLogging
    {
        public static async void PrintLogMessage(string Source, string Action, string Message, ulong GuildID, SocketGuildUser User = null)
        {
            foreach(var Guild in GuildManager.Guilds)
            {
                if(Guild.GuildID == GuildID)
                {
                    EmbedBuilder Embed = new EmbedBuilder
                    {
                        Color = Color.DarkRed
                    };
                    Embed.WithAuthor("Log");
                    Embed.WithDescription($"Source: {Source}\n\nAction: {Action}");
                    string Actor = ModuleControl.BotName;
                    if (User != null)
                        Actor = User.Username;
                    Embed.AddField(Message, $"This action was taken by: {Actor}");

                    var IDs = Guild.Configuration.LogChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendMessageAsync("", false, Embed.Build());
                    }
                }
            }
            
            
        }
    }
}