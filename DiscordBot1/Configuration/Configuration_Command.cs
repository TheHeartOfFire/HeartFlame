using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Misc;
using HeartFlame.ModuleControl;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Configuration
{
    [Group("Configuration"), Alias("Config")]
    public class Configuration_Command : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Remarks("Config_Help"), Summary("Get all of the commands in the Global Configuration Group.")]
        public async Task ConfigHelp()
        {
            var embeds = HelpEmbed("Config Help", "Config_Help", 0);
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Log"), Summary("Add a Channel used for log messages."), Priority(1), RequireModule(Modules.LOGGING), RequirePermission(Roles.ADMIN)]
        public async Task AddLogChannel(IChannel channel = null)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            string chnl;
            if (channel is null)
            {
                BotGuild.Configuration.LogChannel = Context.Channel.Id;
                chnl = Context.Channel.Name;
            }
            else
            {
                BotGuild.Configuration.LogChannel = channel.Id;
                chnl = channel.Name;
            }
            PersistentData.SaveChangesToJson();

            await Context.Channel.SendMessageAsync($"The bot's log channels has been set to {chnl}.");

            BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"The bot's log channel has been changed to {chnl}",
                        Context);
        }

        [Command("Prefix"), Alias("pfx"), Summary("Adds an additional prefix option for running commands."), Priority(1)]
        [RequirePermission(Roles.OWNER)]
        public async Task AddPrefix(string prefix)
        {
            var Guild = GuildManager.GetGuild(Context.Guild);
            Guild.Configuration.Prefixes.Add(prefix);
            PersistentData.SaveChangesToJson();
            await ReplyAsync($"`{prefix}` has been added as a prefix for this server."); 
            
            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"`{prefix}` has been added as a prefix for this server by {Guild.GetUser(Context.User).Name}.",
                        Context);
        }

        [Command("UnPrefix"), Alias("upfx"), Summary("Removes a prefix option for running commands."), Priority(1)]
        [RequirePermission(Roles.OWNER)]
        public async Task RemovePrefix(string prefix)
        {
            var Guild = GuildManager.GetGuild(Context.Guild);

            if(Guild.Configuration.Prefixes.Contains(prefix))
            {
                await ReplyAsync($"`{prefix}` is not currently a prefix for this server.");
                return;
            }

            Guild.Configuration.Prefixes.Remove(prefix);
            PersistentData.SaveChangesToJson();
            await ReplyAsync($"`{prefix}` has been removed as a prefix for this server.");

            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"`{prefix}` has been removed as a prefix for this server by {Guild.GetUser(Context.User).Name}.",
                        Context);
        }


        [Command("Chat"), Summary("Add a Channel used for Chat Level messages."), Priority(1), RequireModule(Modules.CHAT), RequirePermission(Roles.ADMIN)]
        public async Task AddChatChannel(IChannel channel = null)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);

            string chnl;
            if (channel is null)
            {
                BotGuild.Configuration.ChatChannel = Context.Channel.Id;
                chnl = Context.Channel.Name;
            }
            else
            {
                BotGuild.Configuration.ChatChannel = channel.Id;
                chnl = channel.Name;
            }
            PersistentData.SaveChangesToJson();

            await Context.Channel.SendMessageAsync($"The bot's chat level channel has been set to {chnl}.");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"The bot's chat level channel has been changed to {chnl}",
                        Context);
        }

        [Command("SetJoinRole"), Summary("Sets the role awarded to user when they join the server."), Priority(1)]
        [RequirePermission(Roles.OWNER)]
        public async Task SetJoinRole(SocketRole Role)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild);
            BotGuild.Moderation.JoinRole = Role.Id;
            PersistentData.SaveChangesToJson();

            await ReplyAsync($"The join role has been set to {Role.Mention}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                $"The join role has been set to {Role.Mention}",
                        Context);

        }

        [Command("Module"), Summary("Sets the role awarded to user when they join the server."), Priority(1)]
        [RequirePermission(Roles.OWNER)]
        public async Task ToggleModule(string Module, bool Active = true)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild);
            
            if(ModuleManager.UpdateModules(BotGuild, Module, Active))
            {
                await ReplyAsync(Properties.Resources.BadModule);
                return;
            }

            string Status = "off";
            if (Active)
                Status = "on";

            await ReplyAsync($"The {Module} module has been turned {Status}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                $"The {Module} module has been turned {Status} by {BotGuild.GetUser(Context.User).Name}",
                        Context);

        }

        [Command("UseChatChannel")]
        [Summary("Choose whether or not Chat Level messages are limited to a determined Channel.")]
        [Priority(1)]
        [RequireModule(Modules.CHAT)]
        [RequirePermission(Roles.ADMIN)]
        public async Task UseChatChannel(bool use = true)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            if (use && BotGuild.Configuration.ChatChannel <= 0)
            {
                await ReplyAsync($"There is no channel specified. Please use `{PersistentData.Data.Config.CommandPrefix}Configuration Chat` to select a channel for this purpose.");
                return;
            }

            BotGuild.Configuration.UseChatChannel = use;
            PersistentData.SaveChangesToJson();

            var msg = "Chat level messages will now be limited to a single channel.";
            if (!use)
                msg = "Chat level messages can now appear in any channel.";

            await Context.Channel.SendMessageAsync(msg);

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{msg}",
                        Context);
        }

        public static List<Embed> HelpEmbed(string CommandName, string Remarks, int level)
        {
            var Commands = Program.Commands.Commands.ToList();
            var Module = Commands.Find(x => x.Remarks == Remarks).Module;
            var ModCommands = Module.Commands.ToList();
            var Children = Module.Submodules.ToList();
            
            string intermediate = PersistentData.Data.Config.CommandPrefix;
            foreach (var Name in GetParents(Module))
                intermediate += $" {Name}";

            return BuildHelpEmbed(ModCommands, intermediate, Children, CommandName);
        }

        public static List<string> GetParents(ModuleInfo Module)
        {
            List<string> Names = new List<string>();
            while (true)
            {
                Names.Add(Module.Name);
                Module = Module.Parent;
                if (Module is null)
                    break;
            }

            Names.Reverse();
            return Names;
        }

        public static List<Embed> BuildHelpEmbed(List<CommandInfo> ModCommands, string intermediate, List<ModuleInfo> Children, string CommandName)
        {
            var Embed = new EmbedBuilder();
            var output = new List<Embed>();
            Embed.WithAuthor($"{Program.Client.CurrentUser.Username}");
            Embed.WithColor(Color.DarkRed);
            Embed.WithDescription($"The following are the available {CommandName} commands for this bot.");
            foreach (CommandInfo command in ModCommands)
            {
                if (Embed.Fields.Count == 25)
                {
                    output.Add(Embed.Build());
                    Embed = new EmbedBuilder();
                }
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";
                string name = " " + command.Name;
                string inputs = "";

                if (name.Equals(""))
                    name = "Unnamed Command";

                if (command.Parameters.Count > 0)
                    foreach (var parameter in command.Parameters)
                        inputs += " `" + parameter.Name + "`";

                Embed.AddField(intermediate + name + inputs, embedFieldText);
            }

            foreach (var Child in Children)
            {
                if (Embed.Fields.Count == 25)
                {
                    output.Add(Embed.Build());
                    Embed = new EmbedBuilder();
                }

                // Get the command Summary attribute information
                string embedFieldText = Child.Summary ?? "No description available\n";
                string name = " " + Child.Name;

                if (name.Equals(""))
                    name = "Unnamed Command";

                Embed.AddField(intermediate + name, embedFieldText);
            }
            output.Add(Embed.Build());
            return output;
        }
    }
}