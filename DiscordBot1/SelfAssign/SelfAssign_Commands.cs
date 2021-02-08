using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Misc;
using HeartFlame.ModuleControl;
using HeartFlame.Permissions;
using HeartFlame.Time;
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
    [RequirePermission(Roles.MOD)]
    public class SelfAssign_Commands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Group"), Remarks("SelfAssign_Help")]
        public async Task SelfAssignHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Help", "SelfAssign_Help");
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
                var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Console Help", "SelfAssign_Console_Help");
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
                        {

                            try {
                                await ((IMessageChannel)chnl).DeleteMessageAsync(BotGuild.SelfAssign.Consoles.MsgID);
                            } catch (Exception) { };
                        }
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

            [Command("Remove"), Alias("rem", "delete", "del"), Summary("Create a new custom console self assign module"), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SelfAssignRemove()
            {
                var Guild = GuildManager.GetGuild(Context.Guild);

                Utils.UpdateMessage(Context.Guild, Guild.SelfAssign.Consoles.MsgID, $"This Message has been deleted by {GuildManager.GetUser(Context.User).Name}!");

                Guild.SelfAssign.Consoles = new RoleCategory();
                PersistentData.SaveChangesToJson();

                await Context.Message.DeleteAsync();

                if (Guild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"The Console Self Assign module was removed.",
                        Context);
            }

            [Group("Role")]
            public class SelfAssign_Console_Role_Commands : ModuleBase<SocketCommandContext>
            {
                [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Console Role Group"), Remarks("SelfAssign_Console_Role_Help")]
                public async Task SelfAssignHelp()
                {
                    var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Console Role Help", "SelfAssign_Console_Role_Help");
                    foreach (var embed in embeds)
                    {
                        await ReplyAsync("", false, embed);
                    }
                }

                [Command("Add"), Summary("Add a role to the Console Self Assign module."), Priority(1)]
                public async Task SelfAssignRoleAdd(SocketRole Role, int Position = -1)
                {
                    var Guild = GuildManager.GetGuild(Context.Guild);

                    if (Position < 0)
                        Position = Guild.SelfAssign.Consoles.Roles.Count + 1;

                    Guild.SelfAssign.Consoles.AddRole(Role.Name, Position, Role.Id);

                    await Context.Message.DeleteAsync();

                    var Message = await Utils.UpdateMessage(Context.Guild,
                        Guild.SelfAssign.Consoles.MsgID,
                        SelfAssign.GenerateEmbed(Guild.SelfAssign.Consoles, "the consoles you use"));

                    await Message.RemoveAllReactionsAsync();
                    foreach (var role in Guild.SelfAssign.Consoles.Roles)
                    {
                        await Message.AddReactionAsync(Emote.Parse(role.Emoji));
                    }
                    if (Guild.ModuleControl.IncludeLogging)
                        BotLogging.PrintLogMessage(
                            MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                        $"The {Role.Name} role was added to the Console Self Assign Module!",
                            Context);
                }

                [Command("Remove"), Alias("Delete", "rem", "del"), Summary("Remove a role from the Console Self Assign module."), Priority(1)]
                public async Task SelfAssignRoleRemove(SocketRole Role)
                {
                    var Guild = GuildManager.GetGuild(Context.Guild);

                    try
                    {
                        Guild.SelfAssign.Consoles.RemoveRole(Role.Id);
                    }
                    catch (Exception) { };
                    await Context.Message.DeleteAsync();

                    var Message = await Utils.UpdateMessage(Context.Guild,
                        Guild.SelfAssign.Consoles.MsgID,
                        SelfAssign.GenerateEmbed(Guild.SelfAssign.Consoles, "the consoles you use"));

                    await Message.RemoveAllReactionsAsync();
                    foreach (var role in Guild.SelfAssign.Consoles.Roles)
                    {
                        await Message.AddReactionAsync(Emote.Parse(role.Emoji));
                    }
                    if (Guild.ModuleControl.IncludeLogging)
                        BotLogging.PrintLogMessage(
                            MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                        $"The {Role.Name} role was removed from the Console Self Assign Module!",
                            Context);
                }

            }
        }

        [Group("TimeZone"), Alias("Time")]
        [RequireModule(Modules.TIME)]
        public class SelfAssign_Time_Commands : ModuleBase<SocketCommandContext>
        {
            [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Time Group"), Remarks("SelfAssign_Time_Help")]
            public async Task SelfAssignHelp()
            {
                var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Time Help", "SelfAssign_Time_Help");
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
                        {

                            try
                            {
                                await ((IMessageChannel)chnl).DeleteMessageAsync(BotGuild.SelfAssign.TimeZones.MsgID);
                            }
                            catch (Exception) { };
                        }
                        
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

            [Command("Remove"), Alias("rem", "delete", "del"), Summary("Remove the TimeZone self assign module"), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SelfAssignRemove()
            {
                var Guild = GuildManager.GetGuild(Context.Guild);

                Utils.UpdateMessage(Context.Guild, Guild.SelfAssign.TimeZones.MsgID, $"This Message has been deleted by {GuildManager.GetUser(Context.User).Name}!");

                Guild.SelfAssign.TimeZones = new TimeZoneCategory();
                PersistentData.SaveChangesToJson();

                await Context.Message.DeleteAsync();

                if (Guild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"The Time Self Assign module was removed.",
                        Context);
            }

            [Group("Role"), Alias("zone")]
            public class SelfAssign_Console_Role_Commands : ModuleBase<SocketCommandContext>
            {//Change Timezone Roles to add or remove individual Timezones. Check for duplicates
                [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign TimeZone Role Group"), Remarks("SelfAssign_TimeZone_Role_Help")]
                public async Task SelfAssignHelp()
                {
                    var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Timezone Role Help", "SelfAssign_TimeZone_Role_Help");
                    foreach (var embed in embeds)
                    {
                        await ReplyAsync("", false, embed);
                    }
                }

                [Command("Add"), Summary("Add a role to the TimeZone Self Assign module."), Priority(1)]
                public async Task SelfAssignRoleAdd(string TimeZone)
                {
                    var Guild = GuildManager.GetGuild(Context.Guild);
                    var TZone = TimeManager.GetTimezone(TimeZone);

                    if(TZone.Equals(TimeZoneInfo.Utc))
                    {
                        await ReplyAsync("", false, BadTimeZone(TimeZone));
                        return;
                    }

                    SelfAssign.AddTimeZoneRole(Guild.SelfAssign.TimeZones, 9, TZone, Context.Guild);
                    Guild.SelfAssign.TimeZones.Sort();

                    await Context.Message.DeleteAsync();

                    var Message = await Utils.UpdateMessage(Context.Guild, 
                        Guild.SelfAssign.TimeZones.MsgID, 
                        SelfAssign.GenerateEmbed(Guild.SelfAssign.TimeZones, "your timezone"));

                    await Message.RemoveAllReactionsAsync();
                    foreach (var role in Guild.SelfAssign.TimeZones.Roles)
                    {
                        await Message.AddReactionAsync(Emote.Parse(role.Emoji));
                    }

                    if (Guild.ModuleControl.IncludeLogging)
                        BotLogging.PrintLogMessage(
                            MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                        $"The {TimeZone} role was added to the TimeZone Self Assign Module!",
                            Context);
                }

                [Command("Remove"), Alias("Delete", "rem", "del"), Summary("Remove a role from the TimeZone Self Assign module."), Priority(1)]
                public async Task SelfAssignRoleRemove(SocketRole Role)
                {
                    var Guild = GuildManager.GetGuild(Context.Guild);

                    try
                    {
                        Guild.SelfAssign.TimeZones.RemoveRole(Role.Id);
                    }
                    catch (Exception) { };
                    await Context.Message.DeleteAsync();

                    var Message = await Utils.UpdateMessage(Context.Guild, 
                        Guild.SelfAssign.TimeZones.MsgID, 
                        SelfAssign.GenerateEmbed(Guild.SelfAssign.TimeZones, "your timezone"));

                    await Message.RemoveAllReactionsAsync();
                    foreach (var role in Guild.SelfAssign.TimeZones.Roles)
                    {
                        await Message.AddReactionAsync(Emote.Parse(role.Emoji));
                    }

                    if (Guild.ModuleControl.IncludeLogging)
                        BotLogging.PrintLogMessage(
                            MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                        $"The {Role.Name} role was removed from the TimeZone Self Assign Module!",
                            Context);
                }

            }

        }

        [Group("Custom")]
        public class SelfAssign_Custom_Commands : ModuleBase<SocketCommandContext>
        {
            [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Custom Group"), Remarks("SelfAssign_Custom_Help")]
            public async Task SelfAssignHelp()
            {
                var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Custom Help", "SelfAssign_Custom_Help");
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

                Module.SetDivider(await SelfAssign.CreateDivider(Context.Guild, DividerRoleName, Roles));
                if(BotGuild.SelfAssign.AddCustom(Module) == null)
                {
                    await ReplyAsync(Properties.Resources.ModuleExists);
                    return; 
                }

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

            [Command("Remove"), Alias("rem", "delete", "del"), Summary("Create a new custom console self assign module"), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SelfAssignRemove(string Name)
            {
                var Guild = GuildManager.GetGuild(Context.Guild);

                var Module = Guild.SelfAssign.GetCustom(Name);

                if (Module is null)
                {
                    await ReplyAsync(Properties.Resources.ModuleNotFound);
                    return;
                }

                Utils.UpdateMessage(Context.Guild, Module.MsgID, $"This Message has been deleted by {GuildManager.GetUser(Context.User).Name}!");
                
                Guild.SelfAssign.RemoveCustom(Module);
                PersistentData.SaveChangesToJson();

                await Context.Message.DeleteAsync();

                if (Guild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"The Custom Self Assign module named {Name} was removed.",
                        Context);
            }

            [Group("Role")]
            public class SelfAssign_Console_Role_Commands : ModuleBase<SocketCommandContext>
            {
                [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Self Assign Custom Role Group"), Remarks("SelfAssign_Custom_Role_Help")]
                public async Task SelfAssignHelp()
                {
                    var embeds = Configuration.Configuration_Command.HelpEmbed("Self Assign Custom Role Help", "SelfAssign_Custom_Role_Help");
                    foreach (var embed in embeds)
                    {
                        await ReplyAsync("", false, embed);
                    }
                }

                [Command("Add"), Summary("Add a role to a Custom Self Assign module."), Priority(1)]
                public async Task SelfAssignRoleAdd(string Name, SocketRole Role, int Position = -1)
                {
                    var Guild = GuildManager.GetGuild(Context.Guild);
                    var Module = Guild.SelfAssign.GetCustom(Name);

                    if (Position < 0)
                        Position = Module.Roles.Count + 1;

                    Module.AddRole(Role.Name, Position, Role.Id);

                    await Context.Message.DeleteAsync();

                    var Message = await Utils.UpdateMessage(Context.Guild, 
                        Module.MsgID, 
                        SelfAssign.GenerateEmbed(Module, "the role you want"));

                    await Message.RemoveAllReactionsAsync();
                    foreach (var role in Module.Roles)
                    {
                        await Message.AddReactionAsync(Emote.Parse(role.Emoji));
                    }
                    if (Guild.ModuleControl.IncludeLogging)
                        BotLogging.PrintLogMessage(
                            MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                        $"The {Role.Name} role was added to a Custom Self Assign Module!",
                            Context);
                }

                [Command("Remove"), Alias("Delete", "rem", "del"), Summary("Remove a role from a Custom Self Assign module."), Priority(1)]
                public async Task SelfAssignRoleRemove(string Name, SocketRole Role)
                {
                    var Guild = GuildManager.GetGuild(Context.Guild);
                    var Module = Guild.SelfAssign.GetCustom(Name);

                    try
                    {
                        Module.RemoveRole(Role.Id);
                    }
                    catch (Exception) { };
                    await Context.Message.DeleteAsync();

                    var Message = await Utils.UpdateMessage(Context.Guild, Module.MsgID, SelfAssign.GenerateEmbed(Module, "the role you want"));

                    await Message.RemoveAllReactionsAsync();
                    foreach (var role in Module.Roles)
                    {
                        await Message.AddReactionAsync(Emote.Parse(role.Emoji));
                    }
                    if (Guild.ModuleControl.IncludeLogging)
                        BotLogging.PrintLogMessage(
                            MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                        $"The {Role.Name} role was removed from a Custom Self Assign Module!",
                            Context);
                }

            }
        }

        //TODOL: Color Self Assign

        public static bool CorrectChannel(SocketGuildChannel Channel, ulong MessageID)
        {
            return Channel is IMessageChannel && ((IMessageChannel)Channel).GetMessageAsync(MessageID) != null;
        }

        public static Embed BadTimeZone(string TimeZone)
        {
            var Embed = new EmbedBuilder();
            Embed.WithDescription($"`{TimeZone}` was not recognized as a valid Timezone.");
            return Embed.Build();
        }
    }
}