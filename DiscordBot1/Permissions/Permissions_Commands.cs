using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Permissions
{
    [Group("Permissions"), Alias("perms")]
    public class Permissions_Command : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Permissions Group"), Remarks("Permissions_Help")]
        public async Task PermissionsHelp()
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            if (!BotGuild.ModuleControl.IncludePermissions)
            {
                await ReplyAsync(Properties.Resources.NotPerms);
                return;
            }

            var embeds = Configuration.Configuration_Command.HelpEmbed("Permissions Help", "Permissions_Help", 0);
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Mod"), Alias("m"), Summary("Set the mod state for the user. Input: SocketGuildUser \"Mentioned Discord User\" bool \"TRUE / FALSE\""), Priority(1)]
        public async Task PermissionsMod(SocketGuildUser User, bool MakeMod = true)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);
            if (!BotGuild.ModuleControl.IncludePermissions)
            {
                await ReplyAsync(Properties.Resources.NotPerms);
                return;
            }

            if (!GUser.Perms.Admin)
            {
                await ReplyAsync(Properties.Resources.NotAdmin);
                return;
            }
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
                        "Permissions.Permissions_Command.PermissionsMod(SocketGuildUser User, bool MakeMod = true)",
                        "Set the mod status of a user",
                        $"{User.Username} is now a mod.",
                        Context.Guild.Id,
                        (SocketGuildUser)Context.User);
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
                            "Permissions.Permissions_Command.PermissionsMod(SocketGuildUser User, bool MakeMod = true)",
                            "Set the mod status of a user",
                            $"{User.Username} is no longer a mod.",
                        Context.Guild.Id,
                            (SocketGuildUser)Context.User);
                }
            }
        }

        [Command("Admin"), Alias("a"), Summary("Set the admin state for the user. Input: SocketGuildUser \"Mentioned Discord User\" bool \"TRUE / FALSE\""), Priority(1)]
        public async Task PermissionsAdmin(SocketGuildUser User, bool MakeAdmin = true)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);
            if (!BotGuild.ModuleControl.IncludePermissions)
            {
                await ReplyAsync(Properties.Resources.NotPerms);
                return;
            }

            if ((Context.Guild.OwnerId != Context.User.Id) && (Context.User.Id != 264555514510311424))
            {
                await ReplyAsync(Properties.Resources.NotOwner);
                return;
            }
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
                            "Permissions.Permissions_Command.PermissionsAfmin(SocketGuildUser User, bool MakeAdmin = true)",
                            "Set the admin status of a user",
                            $"{User.Username} is now an admin.",
                        Context.Guild.Id,
                            (SocketGuildUser)Context.User);
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
                            "Permissions.Permissions_Command.PermissionsAfmin(SocketGuildUser User, bool MakeAdmin = true)",
                            "Set the admin status of a user",
                            $"{User.Username} is no longer an admin.",
                        Context.Guild.Id,
                            (SocketGuildUser)Context.User);
                }
            }
        }
    }
}