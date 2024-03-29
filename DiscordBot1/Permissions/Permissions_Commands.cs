﻿using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Misc;
using HeartFlame.ModuleControl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Permissions
{
    [Group("Permissions"), Alias("perms")]
    [RequireModule(Modules.PERMISSIONS)]
    public class Permissions_Command : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Permissions Group"), Remarks("Permissions_Help")]
        public async Task PermissionsHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Permissions Help", "Permissions_Help");
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Mod"), Alias("m"), Summary("Set the mod state for the user."), Priority(1)]
        [RequirePermission(Roles.ADMIN)]
        public async Task PermissionsMod(SocketGuildUser User, bool MakeMod = true)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);
            if (User is null) return;
            if (MakeMod)
            {
                if (GUser.Perms.Mod)
                    await ReplyAsync($"{User.Username} is already a mod for the bot!");
                else
                {
                    Permissions.AddMod(User);
                    await ReplyAsync($"{User.Username} is now a mod for the bot!");
                    if (BotGuild.ModuleControl.IncludeLogging)
                        BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                        $"{User.Username} is now a mod.",
                        Context);
                }
            }
            else
            {
                if (!GUser.Perms.Mod)
                    await ReplyAsync($"{User.Username} is already not a mod for the bot!");
                else
                {
                    Permissions.AddMod(User);
                    await ReplyAsync($"{User.Username} is no longer a mod for the bot!");

                    if (BotGuild.ModuleControl.IncludeLogging)
                            BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                            $"{User.Username} is no longer a mod.",
                        Context);
                }
            }
        }

        [Command("Admin"), Alias("a"), Summary("Set the admin state for the user."), Priority(1)]
        [RequirePermission(Roles.OWNER)]
        public async Task PermissionsAdmin(SocketGuildUser User, bool MakeAdmin = true)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);

            if (User is null) return;

            if (MakeAdmin)
            {
                if (GUser.Perms.Admin)
                    await ReplyAsync($"{User.Username} is already an admin for the bot!");
                else
                {
                    Permissions.AddAdmin(User);
                    await ReplyAsync($"{User.Username} is now an admin for the bot!");

                    if (BotGuild.ModuleControl.IncludeLogging)
                            BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                            $"{User.Username} is now an admin.",
                        Context);
                }
            }
            else
            {
                if (!GUser.Perms.Admin)
                    await ReplyAsync($"{User.Username} is already not an admin for the bot!");
                else
                {
                    Permissions.AddAdmin(User);
                    await ReplyAsync($"{User.Username} is no longer an admin for the bot!");

                    if (BotGuild.ModuleControl.IncludeLogging)
                            BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                            $"{User.Username} is no longer an admin.",
                        Context);
                }
            }
        }
    }
}