using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Misc;
using HeartFlame.ModuleControl;
using HeartFlame.Permissions;
using System.Reflection;
using System.Threading.Tasks;

namespace HeartFlame.Moderation
{
    [Group("Moderation"), Alias("mod", "admin")]
    [RequireModule(Modules.MODERATION)]
    public class ModerationCommands: ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Moderation Group"), Remarks("Moderation_Help")]
        
        public async Task ModerationHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Moderation Help", "Moderation_Help", 0);
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

            if (ModerationManager.MuteUser(Duration, Incriment, User))
            {
                await ReplyAsync($"{BotGuild.GetUser(Context.User).Name} has muted {GUser.Name} for {Incriment} {ModerationManager.NormalizeTime(Duration)}(s)");
                return;
            }

            await ReplyAsync(Properties.Resources.BadDuration); 
            
            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
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
                        MethodBase.GetCurrentMethod(),
                    $"{User.Username} has been unmuted by {BotGuild.GetUser(Context.User).Name}.",
                    Context);
        }
    }
}
