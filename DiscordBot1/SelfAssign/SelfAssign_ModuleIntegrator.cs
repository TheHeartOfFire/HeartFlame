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

            try
            {
                foreach (var Guild in PersistentData.Data.Guilds)
                {
                    if (Reaction.MessageId == Guild.SelfAssign.Consoles.MsgID)
                    {

                        foreach (var role in Guild.SelfAssign.Consoles.Roles)
                        {
                            if (role.Emoji.Contains(Reaction.Emote.Name))
                            {
                                var Role = guild.GetRole(role.RoleID);
                                await user.AddRoleAsync(Role);// Discord.Net.HttpException: 'The server responded with error 403: Forbidden'
                                if (Guild.SelfAssign.Consoles.DividerRoleID > 0)
                                {
                                    Role = guild.GetRole(Guild.SelfAssign.Consoles.DividerRoleID);
                                    await user.AddRoleAsync(Role);// Discord.Net.HttpException: 'The server responded with error 403: Forbidden'
                                }
                                return;
                            }
                        }

                    }
                    if (Reaction.MessageId == Guild.SelfAssign.TimeZones.MsgID)
                    {
                        foreach (var role in Guild.SelfAssign.TimeZones.Roles)
                        {
                            if (role.Emoji.Contains(Reaction.Emote.Name))
                            {
                                var Role = guild.GetRole(role.RoleID);
                                await user.AddRoleAsync(Role);
                                if (Guild.SelfAssign.TimeZones.DividerRoleID > 0)
                                {
                                    Role = guild.GetRole(Guild.SelfAssign.TimeZones.DividerRoleID);
                                    await user.AddRoleAsync(Role);
                                }
                                return;
                            }
                        }
                    }

                }

            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.Forbidden)
                    await Channel.SendMessageAsync(Properties.Resources.BadHierarchyPosition);
            }
        }

        public static async void OnReactionRemoved(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            SocketGuildUser user = (SocketGuildUser)Reaction.User;
            SocketGuild guild = user.Guild;

            if (user.IsBot)
                return;
            try
            {
                foreach (var Guild in PersistentData.Data.Guilds)
                {
                    if (Reaction.MessageId == Guild.SelfAssign.Consoles.MsgID)
                    {
                        foreach (var role in Guild.SelfAssign.Consoles.Roles)
                        {
                            if (role.Emoji.Contains(Reaction.Emote.Name))
                            {
                                var Role = guild.GetRole(role.RoleID);
                                await user.RemoveRoleAsync(Role);
                                return;
                            }
                        }
                    }
                    if (Reaction.MessageId == Guild.SelfAssign.TimeZones.MsgID)
                    {
                        foreach (var role in Guild.SelfAssign.TimeZones.Roles)
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
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.Forbidden)
                    await Channel.SendMessageAsync(Properties.Resources.BadHierarchyPosition);
            }
        }
    }
}