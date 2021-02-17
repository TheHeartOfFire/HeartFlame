using Discord.Commands;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.CustomCommands
{
    [Group("Response"), Alias("echo", "resp"), Summary("Commands related to the creating custom commands")]
    public class ResponseCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Remarks("Response_Help"), Summary("Get all of the commands in the Response Commands Group.")]
        public async Task ResponseCommandsHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Response Help", "Response_Help");
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Add"), Summary("Add a custom command with a specified name and message."), Priority(1)]
        [RequirePermission(Roles.ADMIN)]
        public async Task AddCommand(string Name, params string[] Message)
        {
            var Guild = GuildManager.GetGuild(Context.Guild);

            if (!Guild.Commands.AddCommand(Name, string.Join(" ", Message)))
            {
                await ReplyAsync(Properties.Resources.BadCommandName);
                return;
            }

            await ReplyAsync($"A command named {Name} has been added with the following message.\n`{string.Join(" ", Message)}`");

            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                $"A command named {Name} has been added with the following message.\n`{string.Join(" ", Message)}`",
                    Context);
        }

        [Command("Remove"), Alias("delete", "rem", "del"), Summary("Remove a custom command with the specified name."), Priority(1)]
        [RequirePermission(Roles.ADMIN)]
        public async Task RemoveCommand(string Name)
        {
            var Guild = GuildManager.GetGuild(Context.Guild);

            var Message = Guild.Commands.GetCommand(Name);

            if (!Guild.Commands.RemoveCommand(Name))
            {
                await ReplyAsync(Properties.Resources.CommandNameNotFound);
                return;
            }

            await ReplyAsync($"A command named {Name} has been removed with the following message.\n`{Message}`");

            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                $"A command named {Name} has been removed with the following message.\n`{Message}`",
                    Context);
        }

        [Command("Update"), Summary("Update the custom command with the specified name to have the specified message."), Priority(1)]
        [RequirePermission(Roles.ADMIN)]
        public async Task UpdateCommand(string Name, params string[] Message)
        {
            var Guild = GuildManager.GetGuild(Context.Guild);
            var OldMessage = Guild.Commands.GetCommand(Name);

            if (!Guild.Commands.UpdateCommand(Name, string.Join(" ", Message)))
            {
                await ReplyAsync(Properties.Resources.CommandNameNotFound);
                return;
            }

            await ReplyAsync($"A command named {Name} has had it's message updated from:\n`{OldMessage}`\nTo\n`{string.Join(" ", Message)}`");

            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                $"A command named {Name} has had it's message updated from:\n`{OldMessage}`\nTo\n`{string.Join(" ", Message)}`",
                    Context);
        }

        [Command("List"), Alias("all"), Summary("Get all custom commands and their messages set for this server."), Priority(1)]
        public async Task List()
        {
            var Guild = GuildManager.GetGuild(Context.Guild);
            foreach (var Embed in Guild.Commands.GetAllCommands())
            {
                await ReplyAsync("", false, Embed);
            }
        }
    }
}
