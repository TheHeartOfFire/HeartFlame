using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.ModuleControl;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Comp
{
    [Group("Comp"), Summary("Commands relating to information about comp teams and competitive play")]
    [RequireModule(Modules.COMP)]
    public class CompCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Comp Group"), Remarks("Comp_Help")]

        public async Task CompHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Comp Help", "Comp_Help");
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Mute"), Summary("Mute a user."), Priority(1)]
        [RequirePermission(Roles.MOD)]
        public async Task AddTeam(SocketGuildUser User, int Incriment, string Duration)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"{User.Username} has been muted by {BotGuild.GetUser(Context.User).Name} for {Incriment}.",
                    Context);
        }

        [Group("Team"), Summary("Commands relating to information about comp teams specifically")]
        public class CompTeamCommands : ModuleBase<SocketCommandContext>
        {

        }

        [Group("Player"), Summary("Commands relating to information about comp team members")]
        public class CompPlayerCommands : ModuleBase<SocketCommandContext>
        {

        }

    }
}
