using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Misc
{
    public static class ModuleControl
    {
        public static readonly string BotName = "heartflame";
        public static readonly bool IncludePermissions = true;
        public static readonly bool IncludeLogging = true;
        public static readonly bool IncludeChat = true;
        public static readonly bool IncludeSelfAssign = true;

        public static void InitializeModules()
        {
            if (IncludePermissions) Permissions.Permissions.OnpreProcessing();
            if (IncludeChat) ChatLevels.ChatUsers.OnpreProcessing();
            if (IncludeSelfAssign) SelfAssign.SelfAssign.ConstructorAsync();
        }

        public static void MessageTunnel(SocketMessage arg)
        {
            if (IncludeChat) ChatLevels.ChatModuleIntegrator.OnMessagePosted(arg);
        }

        public static void OnReactionAdded(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            SelfAssign.SelfAssign_ModuleIntegrator.OnReactionAdded(Cache, Channel, Reaction);
        }

        public static void OnReactionRemoved(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            SelfAssign.SelfAssign_ModuleIntegrator.OnReactionRemoved(Cache, Channel, Reaction);
        }
    }
}