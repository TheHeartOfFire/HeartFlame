using Discord.Commands;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HeartFlame.PatchNotes
{
    [Group("Patch"), Summary("Commands relating to modifying and displaying patchnotes.")]
    public class PatchNotesCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Remarks("Patch_Help"), Summary("Get all of the commands in the Patch Commands Group.")]
        public async System.Threading.Tasks.Task PatchNotesCommandsHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Patch Help", "Patch_Help");
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Add"), Summary("Add a task that needs to be completed"), Priority(1)]
        [RequirePermission(Roles.MOD)]
        public async System.Threading.Tasks.Task AddTask(string Name, params string[] Details)
        {
            var Guild = GuildManager.GetGuild(Context.Guild);
            var Id = Guild.PatchNotes.NewTask(Name, string.Join(" ", Details));
            if (Id>=0)
            {
                await ReplyAsync(Properties.Resources.BadTaskName);
                return;
            }

            await ReplyAsync($"A task named {Name} has been added with Id: {Id}");

            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                $"A task named {Name} has been added with Id: {Id}",
                    Context);
        }

        [Command("Complete"), Summary("Complete a task in the current list of incomplete tasks."), Priority(1)]
        [RequirePermission(Roles.ADMIN)]
        public async System.Threading.Tasks.Task RemoveTask(int Id, params string[] Notes)
        {
            var Guild = GuildManager.GetGuild(Context.Guild);
            Notes[0] = string.Join(" ", Notes);
            string Name = Guild.PatchNotes.GetTask(Id).Name;
            Guild.PatchNotes.CompleteTask(Id, Notes[0]);


            await ReplyAsync($"A task named {Name} has been marked as complete.");

            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                $"A task named {Name} has been marked as complete.",
                    Context);
        }

        [Command("Generate"), Summary("Move all completed tasks from the list of uncompleted tasks to the list of completed tasks and generate Patchnotes."), Priority(1)]
        [RequirePermission(Roles.ADMIN)]
        public async System.Threading.Tasks.Task GeneratePatchNotes()
        {
            var Guild = GuildManager.GetGuild(Context.Guild);

            foreach(var Embed in Guild.PatchNotes.GeneratePatchNotes())
            {
                await Guild.GetPatchChannel(Context).SendMessageAsync("", false, Embed);
            }

            await ReplyAsync($"The most recent patch notes have been generated in {Guild.GetPatchChannel(Context).Name}");

            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                $"The most recent patch notes have been generated in {Guild.GetPatchChannel(Context).Name}",
                    Context);
        }

        [Command("Last"), Summary("Regenerate the last batch of patch notes."), Priority(1)]
        public async System.Threading.Tasks.Task ReGeneratePatchNotes()
        {
            var Guild = GuildManager.GetGuild(Context.Guild);

            foreach (var Embed in Guild.PatchNotes.LastPatchNotes())
            {
                await Guild.GetPatchChannel(Context).SendMessageAsync("", false, Embed);
            }

            await ReplyAsync($"The last batch of patch notes have been generated in {Guild.GetPatchChannel(Context).Name}");

            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                $"The last batch of patch notes have been generated in {Guild.GetPatchChannel(Context).Name}",
                    Context);
        }
    }
}
