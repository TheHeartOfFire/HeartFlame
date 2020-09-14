using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
            if (!ModuleControl.IncludeSelfAssign)
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
                if (!ModuleControl.IncludeSelfAssign)
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
                if (!ModuleControl.IncludeSelfAssign)
                {
                    await ReplyAsync(Properties.Resources.NotSelf);
                    return;
                }

                if (ModuleControl.IncludePermissions && !Permissions.Permissions.IsMod((SocketGuildUser)Context.User))
                {
                    await ReplyAsync(Properties.Resources.NotMod);
                    return;
                }

                if (SelfAssign.roles.Consoles.MsgID > 0)
                {
                    foreach (var chnl in Context.Guild.Channels)
                    {
                        if (chnl is IMessageChannel && await ((IMessageChannel)chnl).GetMessageAsync(SelfAssign.roles.Consoles.MsgID) != null)
                            await ((IMessageChannel)chnl).DeleteMessageAsync(SelfAssign.roles.Consoles.MsgID);
                    }
                }

                var Embed = await SelfAssign.PrefabConsoleAsync(Context.Guild.Id);

                var msgID = await Context.Channel.SendMessageAsync("", false, Embed);

                SelfAssign.SetConsoleMessageID(msgID.Id);

                foreach (var role in SelfAssign.roles.Consoles.Roles)
                {
                    await msgID.AddReactionAsync(Emote.Parse(role.Emoji));
                }

                BotLogging.PrintLogMessage(
                    "SelfAssign.SelfAssign_Commands.SelfAssign_Console_Commands.SelfAssignConsoles()",
                    "Generated the prefabricated Console Self Assign.",
                    $"A Console Self Assign prefab was generated in {Context.Channel.Name}",
                    (SocketGuildUser)Context.User);
            }
        }

        [Group("TimeZone"), Alias("Time")]
        public class SelfAssign_Time_Commands : ModuleBase<SocketCommandContext>
        {
            [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Time Group"), Remarks("SelfAssign_Time_Help")]
            public async Task SelfAssignHelp()
            {
                if (!ModuleControl.IncludeSelfAssign)
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
                if (!ModuleControl.IncludeSelfAssign)
                {
                    await ReplyAsync(Properties.Resources.NotSelf);
                    return;
                }

                if (ModuleControl.IncludePermissions && !Permissions.Permissions.IsMod((SocketGuildUser)Context.User))
                {
                    await ReplyAsync(Properties.Resources.NotMod);
                    return;
                }

                if (SelfAssign.roles.TimeZones.MsgID > 0)
                {
                    foreach (var chnl in Context.Guild.Channels)
                    {
                        if (chnl is IMessageChannel && await ((IMessageChannel)chnl).GetMessageAsync(SelfAssign.roles.TimeZones.MsgID) != null)
                            await ((IMessageChannel)chnl).DeleteMessageAsync(SelfAssign.roles.TimeZones.MsgID);
                    }
                }

                var Embed = await SelfAssign.PrefabTimeAsync(Context.Guild.Id);

                var msgID = await Context.Channel.SendMessageAsync("", false, Embed);

                SelfAssign.SetTimeMessageID(msgID.Id);

                foreach (var role in SelfAssign.roles.TimeZones.Roles)
                {
                    await msgID.AddReactionAsync(Emote.Parse(role.Emoji));
                }

                BotLogging.PrintLogMessage(
                    "SelfAssign.SelfAssign_Commands.SelfAssignTime()",
                    "Generated the prefabricated TimeZone Self Assign.",
                    $"A TimeZone Self Assign prefab was generated in {Context.Channel.Name}",
                    (SocketGuildUser)Context.User);
            }
        }
    }
}