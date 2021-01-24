using Discord;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Logging
{
    public class ServerLogging
    {
        public static async void UserJoined(SocketGuildUser User)
        {
            var Guild = GuildManager.GetGuild(User);
            var Channel = GetJoinChannel(Guild);

            await Channel.SendMessageAsync("", false, GetEmbed(User));
        }

        public static async void UserLeft(SocketGuildUser User)
        {
            var Guild = GuildManager.GetGuild(User);
            var Channel = GetLeaveChannel(Guild);

            await Channel.SendMessageAsync("", false, GetEmbed(User, false));
        }

        public static async void AuditLog(GuildData Guild, string Event, string Message)
        {
            if (Guild.Configuration.Logging.SplitServerBotLogging)
                if (Guild.Configuration.Logging.ServerLoggingChannel == 0)
                    return;

            if (Guild.Configuration.LogChannel == 0)
                return;

            await GetServerLoggingChannel(Guild).SendMessageAsync("", false, GetEmbed(Event, Message));
        }

        public static async void AuditLog(GuildData Guild, string Event, List<CompareResult> Results)
        {
            if (Guild.Configuration.Logging.SplitServerBotLogging)
                if (Guild.Configuration.Logging.ServerLoggingChannel == 0)
                    return;

            if (Guild.Configuration.LogChannel == 0)
                return;

            await GetServerLoggingChannel(Guild).SendMessageAsync("", false, GetEmbed(Event, Results));
        }

        public static ISocketMessageChannel GetJoinChannel(GuildData Guild)
        {
            var Client = Program.Client;
            if (Guild.Configuration.Logging.SplitJoinLeave)
                return Client.GetChannel(Guild.Configuration.Logging.JoinChannel) as ISocketMessageChannel;
            else
                return GetServerLoggingChannel(Guild);
        }

        public static ISocketMessageChannel GetLeaveChannel(GuildData Guild)
        {
            var Client = Program.Client;
            if (Guild.Configuration.Logging.SplitJoinLeave)
                return (SocketGuildChannel)Client.GetChannel(Guild.Configuration.Logging.LeaveChannel) as ISocketMessageChannel;
            else
                return GetServerLoggingChannel(Guild);
        }

        public static ISocketMessageChannel GetServerLoggingChannel(GuildData Guild)
        {
            var Client = Program.Client;
            if (Guild.Configuration.Logging.SplitServerBotLogging)
                return (SocketGuildChannel)Client.GetChannel(Guild.Configuration.Logging.ServerLoggingChannel) as ISocketMessageChannel;
            return (SocketGuildChannel)Client.GetChannel(Guild.Configuration.LogChannel) as ISocketMessageChannel;
        }

        public static Embed GetEmbed(SocketGuildUser User, bool Join = true)
        {
            var GUser = GuildManager.GetUser(User);
            var EColor = Color.Green;
            var Type = "joined";
            if (!Join)
            {
                Type = "left";
                EColor = Color.Red;
            }

            var Embed = new EmbedBuilder
            {
                Color = EColor,
                ImageUrl = User.GetAvatarUrl()
            };


            Embed.AddField(DateTime.UtcNow.ToString(), $"{GUser.Name} has {Type} the server.");
            return Embed.Build();
        }

        public static Embed GetEmbed(string Event, string Message)
        {
            var Embed = new EmbedBuilder
            {
                Color = Color.Orange
            };


            Embed.AddField(DateTime.UtcNow.ToString(), $"{Event}: {Message}");
            return Embed.Build();
        }

        public static Embed GetEmbed(string Event, List<CompareResult> Results)
        {
            var Embed = new EmbedBuilder
            {
                Color = Color.Orange
            };

            Embed.WithDescription(DateTime.UtcNow.ToString() + " : " + Event);
            foreach(var Result in Results)
            Embed.AddField(Result.Event, $"{Result.Message}");

            return Embed.Build();
        }

    }
}
