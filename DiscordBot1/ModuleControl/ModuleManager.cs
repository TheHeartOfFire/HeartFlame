using Discord;
using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
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
    }
}