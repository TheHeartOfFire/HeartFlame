using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using HeartFlame.ModuleControl;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Configuration
{
    [Group("Configuration"), Alias("Config")]
    public class Configuration_Command : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Remarks("Config_Help"), Summary("Get all of the commands in the Configuration Group.")]
        public async Task ConfigHelp()
        {
            var embeds = HelpEmbed("Config Help", "Config_Help", 0);
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Prefix"), Alias("pfx"), Summary("Set the prefix for all commands. A space is added to the end by default. Input: string \"prefix\""), Priority(1)]
        public async Task SetPrefix(string Prefix)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser((SocketGuildUser)Context.User);

            if (BotGuild.ModuleControl.IncludePermissions && !GUser.Perms.Admin)
            {
                await ReplyAsync(":x:Uh oh! You do not have permission to do that. You must be at least an admin for the bot to use this command.");
                return;
            }

            PersistentData.Data.Config.CommandPrefix = Prefix + " ";
            PersistentData.SaveChangesToJson();
            await Context.Channel.SendMessageAsync($"Command Prefix has been set to {Prefix}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                "Configuration.Configuration_Command.SetPrefix(string Prefix)",
                "Set the command prefix for the bot.",
                $"Bot commands must now begin with \"{Prefix} \"",
                    Context.Guild.Id,
                (SocketGuildUser)Context.User);
        }

        [Command("Game"), Alias("g"), Summary("Set the game that discord shows the bot to be playing. Input: string \"name\""), Priority(1)]
        public async Task SetGame(string Game)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser((SocketGuildUser)Context.User);
            if (BotGuild.ModuleControl.IncludePermissions && !GUser.Perms.Admin)
            {
                await ReplyAsync(":x:Uh oh! You do not have permission to do that. You must be at least an admin for the bot to use this command.");
                return;
            }

            PersistentData.Data.Config.Game = Game;
            PersistentData.SaveChangesToJson();
            await Context.Channel.SendMessageAsync($"Animyst Bot is now playing {Game}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                "Configuration.Configuration_Command.SetGame(string Game)",
                "Set the game that discord displays the bot to be playing.",
                $"The bot is now playing {Game}",
                    Context.Guild.Id,
                (SocketGuildUser)Context.User);
        }

        [Command("Log"), Summary("Add a Channel used for log messages. Input: Discord Channel \"Mentioned Discord channel\""), Priority(1), RequireModule(Modules.LOGGING), RequirePermission(Roles.ADMIN)]
        public async Task AddLogChannel(IChannel channel = null)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            string chnl;
            if (channel is null)
            {
                BotGuild.Configuration.LogChannel.Add(Context.Channel.Id);
                chnl = Context.Channel.Name;
            }
            else
            {
                BotGuild.Configuration.LogChannel.Add(channel.Id);
                chnl = channel.Name;
            }
            PersistentData.SaveChangesToJson();

            await Context.Channel.SendMessageAsync($"The bot's log channels has been set to {chnl}.");

            BotLogging.PrintLogMessage(
            "Configuration.Configuration_Command.SetLogChannel(IChannel channel = null)",
            "Change the bot's Log channel.",
            $"The bot's log channel has been changed to {chnl}",
                    Context.Guild.Id,
            (SocketGuildUser)Context.User);
        }

        [Command("Chat"), Summary("Add a Channel used for Chat Level messages. Input: Discord Channel \"Mentioned Discord channel\""), Priority(1), RequireModule(Modules.CHAT), RequirePermission(Roles.ADMIN)]
        public async Task AddChatChannel(IChannel channel = null)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);

            string chnl;
            if (channel is null)
            {
                BotGuild.Configuration.ChatChannel.Add(Context.Channel.Id);
                chnl = Context.Channel.Name;
            }
            else
            {
                BotGuild.Configuration.ChatChannel.Add(channel.Id);
                chnl = channel.Name;
            }
            PersistentData.SaveChangesToJson();

            await Context.Channel.SendMessageAsync($"The bot's chat level channel has been set to {chnl}.");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                "Configuration.Configuration_Command.SetChatChannel(IChannel channel = null)",
                "Change the bot's chat level channel.",
                $"The bot's chat level channel has been changed to {chnl}",
                        Context.Guild.Id,
                (SocketGuildUser)Context.User);
        }

        [Command("UseChatChannel")]
        [Summary("Choose whether or not Chat Level messages are limited to a determined Channel. Input: Discord Channel \"Mentioned Discord channel\"")]
        [Priority(1)]
        [RequireModule(Modules.CHAT)]
        [RequirePermission(Roles.ADMIN)]
        public async Task UseChatChannel(bool use = true)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            if (use && BotGuild.Configuration.ChatChannel.Count <= 0)
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
                "Configuration.Configuration_Command.UseChatChannel(IChannel channel = null)",
                "Choose whether or not Chat Level messages are limited to a determined Channel.",
                $"{msg}",
                        Context.Guild.Id,
                (SocketGuildUser)Context.User);
        }

        public static List<Embed> HelpEmbed(string CommandName, string Remarks, int level)
        {
            var output = new List<Embed>();
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor($"{Program.Client.CurrentUser.Username}");
            Embed.WithColor(Color.DarkRed);
            Embed.WithDescription($"The following are the available {CommandName} commands for this bot.");

            var Commands = Program.Commands.Commands.ToList();
            var Command = Commands.Find(x => x.Remarks == Remarks);
            var Module = Command.Module;
            var ModCommands = Module.Commands.ToList();
            var Children = Module.Submodules.ToList();

            string intermediate = PersistentData.Data.Config.CommandPrefix;
            if (level == 0)
            {
                intermediate += Module.Name + " ";
            }
            else if (level == 1)
            {
                intermediate += Module.Parent.Name + " " + Module.Name + " ";
            }
            else if (level == 2)
            {
                intermediate += Module.Parent.Parent.Name + " " + Module.Parent.Name + " " + Module.Name + " ";
            }
            else if (level == 3)
            {
                intermediate += Module.Parent.Parent.Parent.Name + " " + Module.Parent.Parent.Name + " " + Module.Parent.Name + " " + Module.Name + " ";
            }

            foreach (CommandInfo command in ModCommands)
            {
                if (Embed.Fields.Count == 25)
                {
                    output.Add(Embed.Build());
                    Embed = new EmbedBuilder();
                }
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";
                string name = command.Name;
                string inputs = "";
                if (name.Equals(""))
                    name = "Unnamed Command";
                if (command.Parameters.Count > 0)
                    foreach (var parameter in command.Parameters)
                        inputs += parameter.Name + ", ";
                if (inputs != "")
                    inputs = inputs.Substring(0, inputs.Length - 2);

                Embed.AddField(intermediate + name + "(" + inputs + ")", embedFieldText);
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
                string name = Child.Name;
                if (name.Equals(""))
                    name = "Unnamed Command";
                Embed.AddField(intermediate + name, embedFieldText);
            }

            output.Add(Embed.Build());
            return output;
        }
    }
}