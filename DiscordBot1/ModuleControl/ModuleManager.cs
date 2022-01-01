using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using HeartFlame.ChannelDirector;
using HeartFlame.ChatLevels;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Misc;
using HeartFlame.Moderation;
using HeartFlame.Reporting;
using System;

namespace HeartFlame.ModuleControl
{
    public static class ModuleManager
    {
        public static void MessageTunnel(SocketMessage arg)
        {
            var Guild = GuildManager.GetGuild(((SocketGuildChannel)arg.Channel).Guild);

            if (Guild.ModuleControl.IncludeChat) ChatModuleIntegrator.OnMessagePosted(arg);

            if (Guild.ModuleControl.IncludeTime && arg.Content.Contains("what time", StringComparison.OrdinalIgnoreCase))
            {
                arg.Channel.SendMessageAsync($"Would you like to know the time? Try `!hf Time`");
            }
             
        }

        public static async void CommandReceived(SocketUserMessage Message, SocketCommandContext Context, int argpos)
        {
            var Guild = GuildManager.GetGuild(((SocketGuildChannel)Message.Channel).Guild);

            await ModerationManager.OnMessageReceived(Message, argpos, Context);
            if (Guild.ModuleControl.IncludeCustomCommands && Guild.Commands != null)
                await Guild.Commands.Client_MessageReceived(Message, argpos);

        }

        public static void OnReactionAdded(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (((SocketGuildChannel)Channel).Guild.Id == Guild.GuildID)
                {
                    if (Guild.ModuleControl.IncludeSelfAssign)
                        SelfAssign.SelfAssign_ModuleIntegrator.OnReactionAdded(Cache, Channel, Reaction);
                }
            }


            ReportingManager.OnReactionAdded(Cache, Channel, Reaction);
        }

        public static void OnReactionRemoved(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (((SocketGuildChannel)Channel).Guild.Id == Guild.GuildID)
                {
                    if (Guild.ModuleControl.IncludeSelfAssign)
                        SelfAssign.SelfAssign_ModuleIntegrator.OnReactionRemoved(Cache, Channel, Reaction);
                }
            }
            ReportingManager.OnReactionRemoved(Cache, Channel, Reaction);
        }

        internal static void InitializeModules()
        {
            ChatModuleIntegrator.OnpreProcessing();
        }

        public static void OnUserJoined(SocketGuildUser User)
        {
            var Guild = GuildManager.GetGuild(User);
            Guild.AddUser(User);
            PersistentData.SaveChangesToJson();

            if (User.IsPending is null)
                ModerationManager.GiveJoinRole(Guild, User);

            if (Guild.ModuleControl.IncludeServerLogging)
                ServerLogging.UserJoined(User);

            if (Program.BetaActive)
                GuildManager.SetBetaTester(Guild.GetUser(User));
        }

        public static void OnUserLeft(SocketGuildUser User)
        {
            var Guild = GuildManager.GetGuild(User);
            Guild.RemoveUser(User);
            PersistentData.SaveChangesToJson();

            if (Guild.ModuleControl.IncludeServerLogging)
                ServerLogging.UserLeft(User);
        }

        public static void OnBotJoinGuild(SocketGuild Guild)
        {
            GuildManager.AddGuild(Guild);
            Guild.DefaultChannel.SendMessageAsync("", false, ChannelCreation.RequiredChannelsEmbed(Guild, $"`{PersistentData.Data.Config.CommandPrefix}Requirements Create`"));
        }

        public static void OnBotLeaveGuild(SocketGuild Guild)
        {
            GuildManager.RemoveGuild(Guild.Id);

        }

        public static void OnMessageDeleted(Cacheable<IMessage, ulong> CachedMessage, ISocketMessageChannel Channel)
        {

        }
        
        public static void SetModule(GuildData Guild, Modules Module, bool Active = true)
        {
            switch (Module)
            {
                case Modules.PERMISSIONS:
                    Guild.ModuleControl.IncludePermissions = Active;
                    break;
                case Modules.LOGGING:
                    Guild.ModuleControl.IncludeLogging = Active;
                    break;
                case Modules.CHAT:
                    Guild.ModuleControl.IncludeChat = Active;
                    break;
                case Modules.SELFASSIGN:
                    Guild.ModuleControl.IncludeSelfAssign = Active;
                    break;
                case Modules.COMPENDIUM:
                    Guild.ModuleControl.IncludeCompendium = Active;
                    break;
                case Modules.MODERATION:
                    Guild.ModuleControl.IncludeModeration = Active;
                    break;
                case Modules.SERVERLOGGING:
                    Guild.ModuleControl.IncludeServerLogging = Active;
                    break;
                case Modules.TIME:
                    Guild.ModuleControl.IncludeServerLogging = Active;
                    break;
                case Modules.COMMANDS:
                    Guild.ModuleControl.IncludeCustomCommands = Active;
                    break;
                case Modules.JOINMESSAGES:
                    Guild.ModuleControl.IncludeJoinMessages = Active;
                    break;
                case Modules.PATCHNOTES:
                    Guild.ModuleControl.IncludePatchNotes = Active;
                    break;
                case Modules.COMP:
                    Guild.ModuleControl.IncludeComp = Active;
                    break;
            }
        }

        public static Modules? NormalizeModule(string Module)
        {
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module, 
                "permissions", "perms"))
                return Modules.PERMISSIONS;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module, 
                "logging", "bot logging", "bl"))
                return Modules.LOGGING;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module, 
                "chat"))
                return Modules.CHAT;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module, 
                "selfassign", "sa", "self assign", "s a"))
                return Modules.SELFASSIGN;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module, 
                "compendium", "usernames", "username"))
                return Modules.COMPENDIUM;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module, 
                "moderation", "mod"))
                return Modules.MODERATION;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module, 
                "serverlogging", "server logging", "sl", "s l"))
                return Modules.SERVERLOGGING;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module,
                "time"))
                return Modules.TIME;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module,
                "commands", "custom commands", "echo"))
                return Modules.COMMANDS;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module,
                "join", "join messages", "greet", "greetings"))
                return Modules.JOINMESSAGES;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module,
                "PatchNotes", "Patch Notes", "patch", "patches"))
                return Modules.PATCHNOTES;
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, Module,
                "Comp"))
                return Modules.COMP;
            return null;
        }

        /// <summary>
        /// This method automatically calls SaveChangesToJason()
        /// </summary>
        /// <param name="Guild"></param>
        /// <param name="Module"></param>
        /// <param name="Active"></param>
        /// <returns>true if bad module</returns>
        public static bool UpdateModules(GuildData Guild, string Module, bool Active = true)
        {
            var EModule = NormalizeModule(Module);
            if (EModule is null)
                return false;

            SetModule(Guild, (Modules)EModule, Active);
            PersistentData.SaveChangesToJson();
            return true;
        }

        public static void UpdateAllModules(GuildData Guild, bool Active)
        {
            SetModule(Guild, Modules.CHAT, Active);
            SetModule(Guild, Modules.COMPENDIUM, Active);
            SetModule(Guild, Modules.LOGGING, Active);
            SetModule(Guild, Modules.MODERATION, Active);
            SetModule(Guild, Modules.PERMISSIONS, Active);
            SetModule(Guild, Modules.SELFASSIGN, Active);
            SetModule(Guild, Modules.SERVERLOGGING, Active);
            SetModule(Guild, Modules.TIME, Active);
            SetModule(Guild, Modules.COMMANDS, Active);
            SetModule(Guild, Modules.JOINMESSAGES, Active);
            SetModule(Guild, Modules.PATCHNOTES, Active);
            SetModule(Guild, Modules.COMP, Active);
            PersistentData.SaveChangesToJson();
        }
    }
}