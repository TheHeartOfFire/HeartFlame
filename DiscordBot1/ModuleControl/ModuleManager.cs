using Discord;
using Discord.WebSocket;
using HeartFlame.ChannelDirector;
using HeartFlame.ChatLevels;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Misc;
using HeartFlame.Moderation;
using HeartFlame.Reporting;

namespace HeartFlame.ModuleControl
{
    public static class ModuleManager
    {
        public static void MessageTunnel(SocketMessage arg)
        {
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if(((SocketGuildChannel)arg.Channel).Guild.Id == Guild.GuildID)
                {
                    if(Guild.ModuleControl.IncludeChat) ChatModuleIntegrator.OnMessagePosted(arg);
                }
            }
        }

        public static void OnReactionAdded(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if (((SocketGuildChannel)Channel).Guild.Id == Guild.GuildID)
                {
                    if(Guild.ModuleControl.IncludeSelfAssign)
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
            }
        }

        public static Modules? NormalizeModule(string Module)
        {
            Module = Module.ToLowerInvariant();

            if (Module.Equals("permissions") || Module.Equals("perms"))
                return Modules.PERMISSIONS;
            if (Module.Equals("logging"))
                return Modules.LOGGING;
            if (Module.Equals("chat"))
                return Modules.CHAT;
            if (Module.Equals("selfassign") || Module.Equals("sa") || Module.Equals("self assign") || Module.Equals("s a"))
                return Modules.SELFASSIGN;
            if (Module.Equals("compendium") || Module.Equals("usernames") || Module.Equals("username"))
                return Modules.COMPENDIUM;
            if (Module.Equals("moderation") || Module.Equals("mod"))
                return Modules.MODERATION;
            if (Module.Equals("serverlogging") || Module.Equals("server logging") || Module.Equals("sl") || Module.Equals("s l"))
                return Modules.SERVERLOGGING;
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
            PersistentData.SaveChangesToJson();
        }
    }
}