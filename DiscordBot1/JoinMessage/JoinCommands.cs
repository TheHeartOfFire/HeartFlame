using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.JoinMessage
{
    [Group("Join"), Alias("Greeting", "Greet"), Summary("Commands related to greeting new users")]
    [RequirePermission(Roles.ADMIN)]
    public class JoinCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Remarks("Join_Help"), Summary("Get all of the commands in the Join Group.")]
        public async Task JoinCommandsHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Join Help", "Join_Help");
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Enable"), Summary("Allow the bot to greet new users. Input is true by default"), Priority(1)]
        public async Task Enable(bool Enable = true)
        {
            var Guild = GuildManager.GetGuild(Context.Guild);
            Guild.Join.Greet = Enable;
            PersistentData.SaveChangesToJson();

            string Action = Enable ? "now" : "no longer";

            await ReplyAsync($"The bot will {Action} greet new users");

            if (Guild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                $"The bot will {Action} greet new users",
                    Context);
        }

        [Group("Messages"), Alias("Message", "msgs", "msg"), Summary("Commands related to the messages used to greet new users")]
        public class JoinMessagesCommands : ModuleBase<SocketCommandContext>
        {
            [Command("Help"), Alias("", "?"), Remarks("Join_Messages_Help"), Summary("Get all of the commands in the Join Messages Group.")]
            public async Task JoinMessagesCommandsHelp()
            {
                var embeds = Configuration.Configuration_Command.HelpEmbed("Join Messages Help", "Join_Messages_Help");
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("Add"), Summary("Add a new message to the list of greetings for new users. `~user~` will be replaced with the user's name"), Priority(1)]
            public async Task AddMessage(params string[] MessageIn)
            {
                var Guild = GuildManager.GetGuild(Context.Guild);
                var Message = Guild.Join.ParseMessage(string.Join(" ", MessageIn));

                int ID = Guild.Join.AddMessage(Message);

                await ReplyAsync($"The following message has been added with an ID of {ID}\n`{string.Format(Message, ((SocketGuildUser)Context.User).Nickname ?? ((SocketGuildUser)Context.User).Username)}`");

                if (Guild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"The following message has been added with an ID of {ID}\n`{string.Format(Message, ((SocketGuildUser)Context.User).Nickname ?? ((SocketGuildUser)Context.User).Username)}`",
                        Context);
            }

            [Command("Remove"), Alias("rem", "delete", "del"), Summary("Remove a message from the list of greetings for new users."), Priority(1)]
            public async Task RemoveMessage(int ID)
            {
                var Guild = GuildManager.GetGuild(Context.Guild);
                if (ID < 0 || ID > Guild.Join.Messages.Count)
                {
                    await ReplyAsync(Properties.Resources.BadMessageID);
                    return;
                }

                string Message = Guild.Join.GetMessage(ID);
                Guild.Join.RemoveMessage(ID);
                await ReplyAsync($"The following message with Id {ID} has been removed.\n`{string.Format(Message, ((SocketGuildUser)Context.User).Nickname ?? ((SocketGuildUser)Context.User).Username)}`");

                if (Guild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"The following message with Id {ID} has been removed.\n`{string.Format(Message, ((SocketGuildUser)Context.User).Nickname ?? ((SocketGuildUser)Context.User).Username)}`",
                        Context);

            }

            [Command("List"), Alias("All"), Summary("List all messages used to greet users."), Priority(1)]
            public async Task List()
            {
                var Guild = GuildManager.GetGuild(Context.Guild);
                foreach (var Embed in Guild.Join.GetMessages(Context.User))
                {
                    await ReplyAsync("", false, Embed);
                }
            }

            [Command("Default"), Alias("Prefab"), Summary("Remove all existing messages and use a list of default greetings."), Priority(1)]
            public async Task Default()
            {
                var Guild = GuildManager.GetGuild(Context.Guild);
                Guild.Join.ResetToDefault();
                await ReplyAsync("All messages have been remove and replaced with some defaults");

                if (Guild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                    $"All messages have been remove and replaced with some defaults",
                        Context);
            }
        }

    }
}
