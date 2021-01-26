using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Misc;
using HeartFlame.ModuleControl;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.SelfAssign
{
    [Group("SelfAssign"), Alias("Self Assign", "Self", "sa", "s a")]
    [RequireModule(Modules.SELFASSIGN)]
    public class SelfAssign_Commands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Group"), Remarks("SelfAssign_Help")]
        public async Task SelfAssignHelp()
        {
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
                var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Console Help", "SelfAssign_Console_Help", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("Prefab"), Alias("default", "pre"), Summary("Generate the prefabricated Console Self Assign."), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SelfAssignConsoles()
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);

                if (BotGuild.SelfAssign.Consoles.MsgID > 0)
                {
                    foreach (var chnl in Context.Guild.Channels)
                    {
                        if (CorrectChannel(chnl, BotGuild.SelfAssign.Consoles.MsgID))
                            await ((IMessageChannel)chnl).DeleteMessageAsync(BotGuild.SelfAssign.Consoles.MsgID);
                    }
                }

                var Embed = SelfAssign.PrefabConsoleAsync(Context.Guild);

                var msgID = await Context.Channel.SendMessageAsync("", false, Embed);

                BotGuild.SelfAssign.Consoles.SetMessageID(msgID.Id);

                foreach (var role in BotGuild.SelfAssign.Consoles.Roles)
                {
                    await msgID.AddReactionAsync(Emote.Parse(role.Emoji));
                }

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"A Console Self Assign prefab was generated in {Context.Channel.Name}",
                        Context);
            }

            [Command("Custom"), Alias("new"), Summary("Create a new custom console self assign module"), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SelfAssignCustom(params SocketRole[] Roles)
            {
                var Msg = await ReplyAsync("Module is loading. Please wait...");
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);

                var Module = new RoleCategory() { Name = "Console", Title = "Console", MsgID = Msg.Id };

                var Embed = SelfAssign.CustomModule(Roles.ToList(), Module, "the consoles you use");

                Utils.UpdateMessage(Context.Channel, Msg.Id, Embed, true);

                Module.SetDivider(await SelfAssign.CreateDivider(Context.Guild, "Console"));
                BotGuild.SelfAssign.Consoles = Module;
                PersistentData.SaveChangesToJson();
                foreach (var role in Module.Roles)
                {
                    await Msg.AddReactionAsync(Emote.Parse(role.Emoji));
                }

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"A custom Console Self Assign module was created in {Context.Channel.Name}",
                        Context);
            }
            //TODO: Remove Module
            //TODO: Remove Role
            //TODO: Add Role
        }

        [Group("TimeZone"), Alias("Time")]
        public class SelfAssign_Time_Commands : ModuleBase<SocketCommandContext>
        {
            [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Time Group"), Remarks("SelfAssign_Time_Help")]
            public async Task SelfAssignHelp()
            {
                var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Time Help", "SelfAssign_Time_Help", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("Prefab"), Alias("default", "pre"), Summary("Generate the prefabricated TimeZone Self Assign."), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SelfAssignTime()
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                if (BotGuild.SelfAssign.TimeZones.MsgID > 0)
                {
                    foreach (var chnl in Context.Guild.Channels)
                    {
                        if (CorrectChannel(chnl, BotGuild.SelfAssign.TimeZones.MsgID))
                            await ((IMessageChannel)chnl).DeleteMessageAsync(BotGuild.SelfAssign.TimeZones.MsgID);
                    }
                }

                var Embed = SelfAssign.PrefabTimeAsync(Context.Guild);

                var msgID = await Context.Channel.SendMessageAsync("", false, Embed);

                BotGuild.SelfAssign.TimeZones.SetMessageID(msgID.Id);

                foreach (var role in BotGuild.SelfAssign.TimeZones.Roles)
                {
                    await msgID.AddReactionAsync(Emote.Parse(role.Emoji));
                }

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"A TimeZone Self Assign prefab was generated in {Context.Channel.Name}",
                        Context);
            }

            [Command("Custom"), Alias("new"), Summary("Create a new custom timezone self assign module"), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SelfAssignCustom(string Name, string Title, params SocketRole[] Roles)
            {
                var Msg = await ReplyAsync("Module is loading. Please wait...");
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var Module = new RoleCategory() { Name = Name, Title = Title, MsgID = Msg.Id };

                var Embed = SelfAssign.CustomModule(Roles.ToList(), Module, "your timezone");

                Utils.UpdateMessage(Context.Channel, Msg.Id, Embed, true);

                Module.SetDivider(await SelfAssign.CreateDivider(Context.Guild, "TimeZone"));
                BotGuild.SelfAssign.TimeZones = Module;

                PersistentData.SaveChangesToJson();
                foreach (var role in Module.Roles)
                {
                    await Msg.AddReactionAsync(Emote.Parse(role.Emoji));
                }

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"A custom TimeZone Self Assign module was created in {Context.Channel.Name}",
                        Context);
            }
            //TODO: Remove Module
            //TODO: Remove Role
            //TODO: Add Role

        }

        [Group("Custom")]
        public class SelfAssign_Custom_Commands : ModuleBase<SocketCommandContext>
        {
            [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Custom Group"), Remarks("SelfAssign_Custom_Help")]
            public async Task SelfAssignHelp()
            {
                var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Custom Help", "SelfAssign_Custom_Help", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("Add"), Alias("new"), Summary("Create a new custom self assign module"), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SelfAssignCustom(string Name, string Title, string DividerRoleName, params SocketRole[] Roles)
            {
                var Msg = await ReplyAsync("Module is loading. Please wait...");
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);

                var Module = new RoleCategory() { Name = Name, Title = Title, MsgID = Msg.Id };

                var Embed = SelfAssign.CustomModule(Roles.ToList(), Module, "the role you want");

                Module.SetDivider(await SelfAssign.CreateDivider(Context.Guild, DividerRoleName));
                BotGuild.SelfAssign.AddCustom(Module);

                Utils.UpdateMessage(Context.Channel, Msg.Id, Embed, true);

                PersistentData.SaveChangesToJson();
                foreach (var role in Module.Roles)
                {
                    await Msg.AddReactionAsync(Emote.Parse(role.Emoji));
                }

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"A custom Self Assign module named {Name} was created in {Context.Channel.Name}",
                        Context);
            }
            //TODO: Remove Module
            //TODO: Remove Role
            //TODO: Add Role
        }

        //TODOL: Color Self Assign

        public static bool CorrectChannel(SocketGuildChannel Channel, ulong MessageID)
        {
            return Channel is IMessageChannel && ((IMessageChannel)Channel).GetMessageAsync(MessageID) != null;
        }
    }
}