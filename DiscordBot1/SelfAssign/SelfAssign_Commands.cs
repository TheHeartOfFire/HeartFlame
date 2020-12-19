﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.SelfAssign
{
    [Group("SelfAssign"), Alias("Self Assign", "Self", "sa", "s a")]
    public class SelfAssign_Commands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Group"), Remarks("SelfAssign_Help")]
        public async Task SelfAssignHelp()
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            if (!BotGuild.ModuleControl.IncludeSelfAssign)
            {
                await ReplyAsync(Properties.Resources.NotSelf);
                return;
            }

            var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Help", "SelfAssign_Help", 0);
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Group("Console"), Alias("Con")]
        public class SelfAssign_Console_Commands : ModuleBase<SocketCommandContext>
        {
            [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Console Group"), Remarks("SelfAssign_Console_Help")]
            public async Task SelfAssignHelp()
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                if (!BotGuild.ModuleControl.IncludeSelfAssign)
                {
                    await ReplyAsync(Properties.Resources.NotSelf);
                    return;
                }

                var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Console Help", "SelfAssign_Console_Help", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("Prefab"), Alias("default", "pre"), Summary("Generate the prefabricated Console Self Assign."), Priority(1)]
            public async Task SelfAssignConsoles()
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser((SocketGuildUser)Context.User);
                if (!BotGuild.ModuleControl.IncludeSelfAssign)
                {
                    await ReplyAsync(Properties.Resources.NotSelf);
                    return;
                }

                if (BotGuild.ModuleControl.IncludePermissions && !GUser.isMod())
                {
                    await ReplyAsync(Properties.Resources.NotMod);
                    return;
                }

                if (BotGuild.SelfAssign.Consoles.MsgID > 0)
                {
                    foreach (var chnl in Context.Guild.Channels)
                    {
                        if (chnl is IMessageChannel && await ((IMessageChannel)chnl).GetMessageAsync(BotGuild.SelfAssign.Consoles.MsgID) != null)
                            await ((IMessageChannel)chnl).DeleteMessageAsync(BotGuild.SelfAssign.Consoles.MsgID);
                    }
                }

                var Embed = await SelfAssign.PrefabConsoleAsync(Context.Guild.Id);

                var msgID = await Context.Channel.SendMessageAsync("", false, Embed);

                SelfAssign.SetConsoleMessageID(msgID.Id, Context.Guild.Id);

                foreach (var role in BotGuild.SelfAssign.Consoles.Roles)
                {
                    await msgID.AddReactionAsync(Emote.Parse(role.Emoji));
                }

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                    "SelfAssign.SelfAssign_Commands.SelfAssign_Console_Commands.SelfAssignConsoles()",
                    "Generated the prefabricated Console Self Assign.",
                    $"A Console Self Assign prefab was generated in {Context.Channel.Name}",
                        Context.Guild.Id,
                    (SocketGuildUser)Context.User);
            }
        }

        [Group("TimeZone"), Alias("Time")]
        public class SelfAssign_Time_Commands : ModuleBase<SocketCommandContext>
        {
            [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Time Group"), Remarks("SelfAssign_Time_Help")]
            public async Task SelfAssignHelp()
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                if (!BotGuild.ModuleControl.IncludeSelfAssign)
                {
                    await ReplyAsync(Properties.Resources.NotSelf);
                    return;
                }

                var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Time Help", "SelfAssign_Time_Help", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("Prefab"), Alias("default", "pre"), Summary("Generate the prefabricated TimeZone Self Assign."), Priority(1)]
            public async Task SelfAssignTime()
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser((SocketGuildUser)Context.User);
                if (!BotGuild.ModuleControl.IncludeSelfAssign)
                {
                    await ReplyAsync(Properties.Resources.NotSelf);
                    return;
                }

                if (BotGuild.ModuleControl.IncludePermissions && !GUser.isMod())
                {
                    await ReplyAsync(Properties.Resources.NotMod);
                    return;
                }

                if (BotGuild.SelfAssign.TimeZones.MsgID > 0)
                {
                    foreach (var chnl in Context.Guild.Channels)
                    {
                        if (chnl is IMessageChannel && await ((IMessageChannel)chnl).GetMessageAsync(BotGuild.SelfAssign.TimeZones.MsgID) != null)
                            await ((IMessageChannel)chnl).DeleteMessageAsync(BotGuild.SelfAssign.TimeZones.MsgID);
                    }
                }

                var Embed = await SelfAssign.PrefabTimeAsync(Context.Guild.Id);

                var msgID = await Context.Channel.SendMessageAsync("", false, Embed);

                SelfAssign.SetTimeMessageID(msgID.Id, Context.Guild.Id);

                foreach (var role in BotGuild.SelfAssign.TimeZones.Roles)
                {
                    await msgID.AddReactionAsync(Emote.Parse(role.Emoji));
                }

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                    "SelfAssign.SelfAssign_Commands.SelfAssignTime()",
                    "Generated the prefabricated TimeZone Self Assign.",
                    $"A TimeZone Self Assign prefab was generated in {Context.Channel.Name}",
                        Context.Guild.Id,
                    (SocketGuildUser)Context.User);
            }
        }
    }
}