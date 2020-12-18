using Discord;
using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.GuildControl;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Misc
{
    public static class ModuleControl
    {
        public static readonly string BotName = "heartflame";


        public static void MessageTunnel(SocketMessage arg)
        {
            foreach(var Guild in GuildManager.Guilds)
            {
                if(((SocketGuildChannel)arg.Channel).Guild.Id == Guild.GuildID)
                {
                    if(Guild.ModuleControl.IncludeChat) ChatLevels.ChatModuleIntegrator.OnMessagePosted(arg);
                }
            }
        }

        public static void OnReactionAdded(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            foreach(var Guild in GuildManager.Guilds)
            {
                if (((SocketGuildChannel)Channel).Guild.Id == Guild.GuildID)
                {
                    if(Guild.ModuleControl.IncludeSelfAssign)
                        SelfAssign.SelfAssign_ModuleIntegrator.OnReactionAdded(Cache, Channel, Reaction);
                }
            }


        }

        public static void OnReactionRemoved(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (((SocketGuildChannel)Channel).Guild.Id == Guild.GuildID)
                {
                    if (Guild.ModuleControl.IncludeSelfAssign)
                        SelfAssign.SelfAssign_ModuleIntegrator.OnReactionRemoved(Cache, Channel, Reaction);
                }
            }
        }

        internal static void InitializeModules()
        {
            ChatUsers.OnpreProcessing();
        }
    }
}