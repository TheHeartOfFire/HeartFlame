using Discord;
using Discord.WebSocket;
using HeartFlame.ChatLevels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.SelfAssign
{
    public class SelfAssign_ModuleIntegrator
    {
        public static async void OnReactionAdded(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            SocketGuildUser user = (SocketGuildUser)Reaction.User;
            SocketGuild guild = user.Guild;

            if (user.IsBot)
                return;

            if (Reaction.MessageId == SelfAssign.roles.Consoles.MsgID)
            {
                foreach (var role in SelfAssign.roles.Consoles.Roles)
                {
                    if (role.Emoji.Contains(Reaction.Emote.Name))
                    {
                        var Role = guild.GetRole(role.RoleID);
                        await user.AddRoleAsync(Role);
                        if (SelfAssign.roles.Consoles.DividerRoleID > 0)
                        {
                            Role = guild.GetRole(SelfAssign.roles.Consoles.DividerRoleID);
                            await user.AddRoleAsync(Role);
                        }
                        return;
                    }
                }
            }
            if (Reaction.MessageId == SelfAssign.roles.TimeZones.MsgID)
            {
                foreach (var role in SelfAssign.roles.TimeZones.Roles)
                {
                    if (role.Emoji.Contains(Reaction.Emote.Name))
                    {
                        var Role = guild.GetRole(role.RoleID);
                        await user.AddRoleAsync(Role);
                        if (SelfAssign.roles.TimeZones.DividerRoleID > 0)
                        {
                            Role = guild.GetRole(SelfAssign.roles.TimeZones.DividerRoleID);
                            await user.AddRoleAsync(Role);
                        }
                        return;
                    }
                }
            }
        }

        public static async void OnReactionRemoved(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            SocketGuildUser user = (SocketGuildUser)Reaction.User;
            SocketGuild guild = user.Guild;

            if (user.IsBot)
                return;

            if (Reaction.MessageId == SelfAssign.roles.Consoles.MsgID)
            {
                foreach (var role in SelfAssign.roles.Consoles.Roles)
                {
                    if (role.Emoji.Contains(Reaction.Emote.Name))
                    {
                        var Role = guild.GetRole(role.RoleID);
                        await user.RemoveRoleAsync(Role);
                        return;
                    }
                }
            }
            if (Reaction.MessageId == SelfAssign.roles.TimeZones.MsgID)
            {
                foreach (var role in SelfAssign.roles.TimeZones.Roles)
                {
                    if (role.Emoji.Contains(Reaction.Emote.Name))
                    {
                        var Role = guild.GetRole(role.RoleID);
                        await user.RemoveRoleAsync(Role);
                        return;
                    }
                }
            }
        }
    }
}