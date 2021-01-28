using Discord;
using Discord.Addons.CommandsExtension;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Misc;
using HeartFlame.ModuleControl;
using HeartFlame.Permissions;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace HeartFlame.Moderation
{
    [Group("Moderation"), Alias("mod", "admin")]
    [RequireModule(Modules.MODERATION)]
    public class ModerationCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Moderation Group"), Remarks("Moderation_Help")]

        public async Task ModerationHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Moderation Help", "Moderation_Help");
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Mute"), Summary("Mute a user."), Priority(1)]
        [RequirePermission(Roles.MOD)]
        public async Task ModerationMute(SocketGuildUser User, int Incriment, string Duration)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);

            if (!ModerationManager.MuteUser(Duration, Incriment, User))
            {
            await ReplyAsync(Properties.Resources.BadDuration);
                return;
            }

                await ReplyAsync($"{BotGuild.GetUser(Context.User).Name} has muted {GUser.Name} for {Incriment} {ModerationManager.NormalizeTime(Duration)}(s)");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"{User.Username} has been muted by {BotGuild.GetUser(Context.User).Name} for {Incriment}.",
                    Context);
        }

        [Command("Unmute"), Summary("Unmute a user."), Priority(1)]
        [RequirePermission(Roles.MOD)]
        public async Task ModerationUnmute(SocketGuildUser User)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);

            GUser.Moderation.UnMute();
            await ReplyAsync($"{BotGuild.GetUser(Context.User).Name} has unmuted {GUser.Name}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"{User.Username} has been unmuted by {BotGuild.GetUser(Context.User).Name}.",
                    Context);
        }

        [Command("Points"), Alias("point", "chat", "pts", "level", "lvl", "lv"), Summary("Give or take chat points from the user"), Priority(1)]
        [RequireModule(Modules.CHAT), RequirePermission(Roles.MOD)]
        public async Task ModerationChatPoints(SocketGuildUser User, int Points)
        {
            var GSender = GuildManager.GetUser(Context.User);
            var GUser = GuildManager.GetUser(User);
            var BotGuild = GuildManager.GetGuild(User);
            string action = "given";

            if (Points < 0)
            {
                if (!GSender.Perms.Admin)
                {
                    await ReplyAsync(Properties.Resources.NotAdmin);
                    return;
                }
                action = "taken";
            }


            ModerationManager.GivePoints(GUser, Points);

            Points = Math.Abs(Points);
            await ReplyAsync($"{GSender.Name} has {action} {Points} points from {GUser.Name}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"{GSender.Name} has {action} {Points} points from {GUser.Name}",
                    Context);
        }

        [Command("Kick"), Summary("Kick the user from the guild. This does not prevent the user from rejoining."), Priority(1)]
        [RequirePermission(Roles.MOD)]
        public async Task ModerationKick(SocketGuildUser User, params string[] Reason)
        {
            var GSender = GuildManager.GetUser(Context.User);
            var GUser = GuildManager.GetUser(User);
            var BotGuild = GuildManager.GetGuild(User);
            var Name = GUser.Name;
            ModerationManager.KickUser(User, string.Join(' ', Reason));

            var SenderName = GSender.Name;

            await ReplyAsync($"{SenderName} has kicked {Name} from the server!");
            

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"{SenderName} has kicked {Name} from the server!",
                    Context);
        }

        [Command("Ban"), Summary("Ban the user from the guild."), Priority(1)]
        [RequirePermission(Roles.ADMIN)]
        public async Task ModerationBan(SocketGuildUser User, int DaysToPrune, params string[] Reason)
        {
            var GSender = GuildManager.GetUser(Context.User);
            var GUser = GuildManager.GetUser(User);
            var BotGuild = GuildManager.GetGuild(User);

            ModerationManager.BanUser(User, DaysToPrune, string.Join(' ', Reason));

            await ReplyAsync($"{GSender.Name} has banned {GUser.Name} from the server!");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"{GSender.Name} has banned {GUser.Name} from the server!",
                    Context);
        }

        [Command("Unban"), Summary("Unban the user from the guild."), Priority(1)]
        [RequirePermission(Roles.ADMIN)]
        public async Task ModerationUnBan(ulong UserID)
        {
            var GSender = GuildManager.GetUser(Context.User);
            var Guild = GuildManager.GetGuild(Context.User);

            ModerationManager.UnBanUser(Context.Guild, UserID);

            await ReplyAsync($"{GSender.Name} has unbanned a user with ID {UserID} from the server!");

            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"{GSender.Name} has unbanned a user with ID {UserID} from the server!",
                    Context);
        }
    }
}
