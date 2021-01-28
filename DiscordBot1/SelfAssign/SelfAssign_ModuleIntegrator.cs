using Discord;
using Discord.Net;
using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.SelfAssign
{
    public class SelfAssign_ModuleIntegrator
    {
        public static async void OnReactionAdded(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (((SocketGuildUser)Reaction.User).IsBot)
                return;
            try
            {
                var Guild = GuildManager.GetGuild(Reaction.User.Value);

                if (await CheckForSelfAssignMessage(Guild.SelfAssign.Consoles, Reaction))
                    return;

                if (await CheckForSelfAssignMessage(Guild.SelfAssign.TimeZones, Reaction))
                    return;

                foreach (var Module in Guild.SelfAssign.Misc)
                    if (await CheckForSelfAssignMessage(Module, Reaction))
                        return;

            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.Forbidden)
                    await Channel.SendMessageAsync(Properties.Resources.BadHierarchyPosition);
            }
        }

        public static async void OnReactionRemoved(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (((SocketGuildUser)Reaction.User).IsBot)
                return;
            try
            {
                var Guild = GuildManager.GetGuild(Reaction.User.Value);

                if (await CheckForSelfAssignMessage(Guild.SelfAssign.Consoles, Reaction, false))
                    return;

                if (await CheckForSelfAssignMessage(Guild.SelfAssign.TimeZones, Reaction, false))
                    return;

                foreach(var Module in Guild.SelfAssign.Misc)
                    if (await CheckForSelfAssignMessage(Module, Reaction, false))
                        return;

            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.Forbidden)
                    await Channel.SendMessageAsync(Properties.Resources.BadHierarchyPosition);
            }
        }

        private static async Task<bool> CheckForSelfAssignMessage(RoleCategory Module, SocketReaction Reaction, bool Add = true)
        {

            var user = (SocketGuildUser)Reaction.User;
            var guild = user.Guild;

            if (Reaction.MessageId == Module.MsgID)
            {
                foreach (var role in Module.Roles)
                {
                    if (role.Emoji.Contains(Reaction.Emote.Name))
                    {
                        var Role = guild.GetRole(role.RoleID);
                        if (Add)
                        {
                            await user.AddRoleAsync(Role);
                            if (Module.DividerRoleID > 0)
                            {
                                Role = guild.GetRole(Module.DividerRoleID);
                                await user.AddRoleAsync(Role);
                            }
                        }else
                        await user.RemoveRoleAsync(Role);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}