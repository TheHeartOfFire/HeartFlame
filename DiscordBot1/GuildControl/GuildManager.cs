using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.Misc;

namespace HeartFlame.GuildControl
{
    public class GuildManager
    {
        public static void AddGuild(SocketGuild guild)
        {
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if(Guild.GuildID == guild.Id)
                    return;
            }

            PersistentData.Data.Guilds.Add(new GuildData(guild));
            PersistentData.SaveChangesToJson();
        }

        public static void RemoveGuild(ulong GuildID)
        {
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == GuildID)
                    PersistentData.Data.Guilds.Remove(Guild);
            }
            PersistentData.SaveChangesToJson();
        }

        public static GuildData GetGuild(ulong GuildID)
        {
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == GuildID)
                    return Guild;
            }
            return null;
        }

        public static GuildData GetGuild(SocketGuild Guild)
        {
            return GetGuild(Guild.Id);
        }

        public static GuildData GetGuild(SocketGuildUser User)
        {
            return GetGuild(User.Guild);
        }

        public static GuildData GetGuild(SocketUser User)
        {
            return GetGuild((SocketGuildUser)User);
        }

        public static void UpdateGuildName(SocketGuild Guild)
        {
            foreach(var BotGuild in PersistentData.Data.Guilds)
            {
                if (Guild.Id == BotGuild.GuildID)
                    if (!BotGuild.Name.Equals(Guild.Name))
                    {
                        BotGuild.Name = Guild.Name;
                        PersistentData.SaveChangesToJson();
                        return;
                    }
            }
        }
        public static IMessageChannel GetChatChannel(SocketCommandContext Context, GuildData Guild)
        {
            return GetChatChannel(Context.Channel, Guild);
        }

        public static IMessageChannel GetChatChannel(IMessageChannel MessageChannel, GuildData Guild)
        {
            if (Guild.Configuration.UseChatChannel)
                return Program.Client.GetChannel(Guild.Configuration.ChatChannel) as ISocketMessageChannel;
            return MessageChannel;
        }

    }
}
