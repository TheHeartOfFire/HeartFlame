using Discord.WebSocket;
using HeartFlame.GuildControl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Discord;
using HeartFlame.Misc;
using Discord.Rest;

namespace HeartFlame.ChannelDirector
{
    public class ChannelCreation
    {
        /// <summary>
        /// Creates a channel in the specified Category in the specified Category with the specified name and posts a message if the following format:
        /// {PersistentData.BotName} has created this channel for {Purpose}
        /// </summary>
        /// <param name="Guild">The SocketGuild that the channel should be created in.</param>
        /// <param name="Name">The name of the Channel</param>
        /// <param name="Category">The name of the Channel Category this channel could fall under.</param>
        /// <param name="Purpose">The purpose of this channel to be used in the declaration message.</param>
        public static RestTextChannel CreateChannel(SocketGuild Guild, string Name, string Category, string Purpose)
        {
            ICategoryChannel DiscordCategory = null;

            if (Guild.CategoryChannels.ToList().Exists(x => x.Name.Equals(Category, StringComparison.OrdinalIgnoreCase)))
                DiscordCategory = Guild.CategoryChannels.ToList().Find(x => x.Name.Equals(Category, StringComparison.OrdinalIgnoreCase));
            else
                DiscordCategory = Guild.CreateCategoryChannelAsync(Category).Result;

            var Properties = new TextChannelProperties()
            {
                CategoryId = DiscordCategory.Id
            };

            var Channel = Guild.CreateTextChannelAsync(Name, Channel => Channel.CategoryId = DiscordCategory.Id).Result;

            Channel.SendMessageAsync($"{PersistentData.BotName} has created this channel for {Purpose}.");

            return Channel;
        }

        public static void CreateAndAssignChannel(ChannelLocations Location, SocketGuild Guild)
        {
            var BotGuild = GuildManager.GetGuild(Guild);

            switch (Location)
            {
                case ChannelLocations.BOTLOG:
                    BotGuild.Configuration.LogChannel = 
                        CreateChannel(Guild, 
                        "bot-logging", 
                        "Heart Flame", 
                        "logging actions taken by the bot")
                        .Id;
                    break;

                case ChannelLocations.CHAT:
                    BotGuild.Configuration.ChatChannel =
                        CreateChannel(Guild, 
                        "level-banners", 
                        "Heart Flame",
                        "the bot to post level up announcements and chat banners")
                        .Id;
                    break;

                case ChannelLocations.JOIN:
                    BotGuild.Configuration.Logging.JoinChannel =
                        CreateChannel(Guild,
                        "new-members",
                        "Heart Flame",
                        "the bot to post notifications when new members join the server")
                        .Id;
                    break;

                case ChannelLocations.LEAVE:
                    BotGuild.Configuration.Logging.LeaveChannel =
                        CreateChannel(Guild,
                        "ex-members",
                        "Heart Flame",
                        "the bot to post notifications when members leave the server")
                        .Id;
                    break;

                case ChannelLocations.SERVERLOG:
                    BotGuild.Configuration.Logging.ServerLoggingChannel =
                        CreateChannel(Guild,
                        "server-logging",
                        "Heart Flame",
                        "logging server changes")
                        .Id;
                    break;

                case ChannelLocations.SELFASSIGN:
                        CreateChannel(Guild,
                        "self-assign",
                        "Heart Flame",
                        "self assign modules");
                    break;

                case ChannelLocations.BOTUPDATES:
                    CreateChannel(Guild,
                    "status-updates",
                    "Heart Flame",
                    "subscribing to status updates for this bot. Please contact the bot owner to set this up if you are unable to");
                    break;

                case ChannelLocations.CHANGELOG:
                    CreateChannel(Guild,
                    "changelog",
                    "Heart Flame",
                    "subscribing to the changelog for this bot. Please contact the bot owner to set this up if you are unable to");
                    break;
            }

            PersistentData.SaveChangesToJson();
        }

        private static List<ChannelLocations> GetRequiredChannels(GuildData Guild, bool ValueIndependant)
        {
            var Modules = new List<ChannelLocations>();

            if (Guild.ModuleControl.IncludeChat && Guild.Configuration.UseChatChannel)
                if (ValueIndependant || Guild.Configuration.ChatChannel == 0)
                    Modules.Add(ChannelLocations.CHAT);

            if (Guild.ModuleControl.IncludeLogging)
            {
                if (ValueIndependant || Guild.Configuration.LogChannel == 0)
                    Modules.Add(ChannelLocations.BOTLOG);

                if (Guild.Configuration.Logging.SplitJoinLeave)
                {
                    if (ValueIndependant || Guild.Configuration.Logging.JoinChannel == 0)
                        Modules.Add(ChannelLocations.JOIN);

                    if (ValueIndependant || Guild.Configuration.Logging.LeaveChannel == 0)
                        Modules.Add(ChannelLocations.LEAVE);
                }

                if (Guild.Configuration.Logging.SplitServerBotLogging)
                    if (ValueIndependant || Guild.Configuration.Logging.ServerLoggingChannel == 0)
                        Modules.Add(ChannelLocations.SERVERLOG);
            }

            return Modules;
        }

        public static void CreateRequiredChannels(SocketGuild Guild, bool ValueIndependant)
        {
            var Modules = GetRequiredChannels(GuildManager.GetGuild(Guild), ValueIndependant);

            foreach(var Module in Modules)
            {
                CreateAndAssignChannel(Module, Guild);
            }
        }

        public static Embed RequiredChannelsEmbed(SocketGuild Guild, string CreateCommand, bool ValueIndependant = false)
        {
            var Modules = GetRequiredChannels(GuildManager.GetGuild(Guild), ValueIndependant);

            var Embed = new EmbedBuilder();
            string Disclaimer = string.Empty;

            if (ValueIndependant)
                Disclaimer = " Because you asked for all required channels some of these may already be assigned.";
            Embed.WithDescription($"The following channels are required for the modules you have enabled.{Disclaimer}");

            foreach(var Module in Modules)
            {
                var (Name, Purpose) = ChannelInfo(Module);
                Embed.AddField(Name, Purpose);
            }

            Embed.WithFooter($"If you would like me to create these channels and set them up for you please type `{CreateCommand}`");

            return Embed.Build();
        }

        private static (string Name, string Purpose) ChannelInfo(ChannelLocations Location)
        {
            switch (Location)
            {
                case ChannelLocations.BOTLOG:
                    return ("Bot Log", "For logging actions taken by the bot.");

                case ChannelLocations.CHAT:
                    return ("Chat", "For the bot to post level up announcements and chat banners.");

                case ChannelLocations.JOIN:
                    return ("Join", "For the bot to post notifications when new members join the server.");

                case ChannelLocations.LEAVE:
                    return ("Leave", "For the bot to post notifications when members leave the server.");

                case ChannelLocations.SERVERLOG:
                    return ("Server Log", "For logging server changes.");

                default:
                    throw new InvalidOperationException();
            }
        }

    }
}
