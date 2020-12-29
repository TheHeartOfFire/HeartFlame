using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
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

        }
    }
}
