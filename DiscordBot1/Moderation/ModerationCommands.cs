﻿using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using System.Threading.Tasks;

namespace HeartFlame.Moderation
{
    [Group("Moderation"), Alias("mod", "admin")]
    public class ModerationCommands: ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Moderation Group"), Remarks("Moderation_Help")]
        public async Task PermissionsHelp()
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            if (!BotGuild.ModuleControl.IncludeModeration)
            {
                await ReplyAsync(Properties.Resources.NotModeration);
                return;
            }

            var embeds = Configuration.Configuration_Command.HelpEmbed("Moderation Help", "Moderation_Help", 0);
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Mute"), Summary("Mute a user. Input: SocketGuildUser \"Mentioned Discord User\" int \"Duration i.e. 4\" string \"Duration i.e. days\""), Priority(1)]
        public async Task ModerationMute(SocketGuildUser User, int Incriment, string Duration)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);

            if (!BotGuild.ModuleControl.IncludePermissions)
            {
                await ReplyAsync(Properties.Resources.NotPerms);
                return;
            }
            if (BotGuild.ModuleControl.IncludePermissions)
                if (!GUser.Perms.Mod)
                {
                    await ReplyAsync(Properties.Resources.NotMod);
                    return;
                }

            if (ModerationManager.MuteUser(Duration, Incriment, User))
            {
                await ReplyAsync($"{BotGuild.GetUser(Context.User).Name} has muted {GUser.Name} for {Incriment} {ModerationManager.NormalizeTime(Duration)}(s)");
                return;
            }

            await ReplyAsync(Properties.Resources.BadDuration); 
            
            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                    "ModerationCommands.ModerationMute(SocketGuildUser User, int Incriment, string Duration)",
                    "Mute a user.",
                    $"{User.Username} has been muted by {BotGuild.GetUser(Context.User).Name} for {Incriment}.",
                    Context.Guild.Id,
                    (SocketGuildUser)Context.User);
        }

        [Command("Unmute"), Summary("Unmute a user. Input: SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
        public async Task ModerationUnmute(SocketGuildUser User)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);

            if (!BotGuild.ModuleControl.IncludePermissions)
            {
                await ReplyAsync(Properties.Resources.NotPerms);
                return;
            }
            if (BotGuild.ModuleControl.IncludePermissions)
                if (!GUser.Perms.Mod)
                {
                    await ReplyAsync(Properties.Resources.NotMod);
                    return;
                }

            GUser.Moderation.UnMute();
            await ReplyAsync($"{BotGuild.GetUser(Context.User).Name} has unmuted {GUser.Name}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                    "ModerationCommands.ModerationUnmute(SocketGuildUser User)",
                    "Unmute a user.",
                    $"{User.Username} has unmuted by {BotGuild.GetUser(Context.User).Name}.",
                    Context.Guild.Id,
                    (SocketGuildUser)Context.User);
        }
    }
}