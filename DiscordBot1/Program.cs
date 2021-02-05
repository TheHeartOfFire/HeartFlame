using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Misc;
using HeartFlame.Moderation;
using HeartFlame.ModuleControl;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace HeartFlame
{
    public class Program
    {
        public static DiscordSocketClient Client;
        public static CommandService Commands;
        private IServiceProvider Service;

        private string Token = "";
        private string Game = "";
        private string Prefix = "";
        public static readonly bool BetaActive = true;

        private static void Main(string[] args)
         => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            await PersistentData.Constructor();
            var Config = PersistentData.Data.Config;
            Token = Config.Token;
            Game = Config.Game;
            Prefix = Config.CommandPrefix;

            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            Service = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Service);
            Client.MessageReceived += Client_MessageReceived;
            Client.Ready += Client_Ready;
            Client.Log += Client_Log;
            Client.ReactionAdded += Client_ReactionAdded;
            Client.ReactionRemoved += Client_ReactionRemoved;
            Client.JoinedGuild += Client_JoinedGuild;
            Client.LeftGuild += Client_LeftGuild;
            Client.UserJoined += Client_UserJoined;
            Client.UserLeft += Client_UserLeft;
            Client.GuildMemberUpdated += Client_GuildMemberUpdated;
            Client.Disconnected += Client_Disconnected;

            Client.GuildUpdated += Client_GuildUpdated;
            Client.ChannelCreated += Client_ChannelCreated;
            Client.ChannelUpdated += Client_ChannelUpdated;
            Client.ChannelDestroyed += Client_ChannelDestroyed;
            Client.UserBanned += Client_UserBanned;
            Client.UserUnbanned += Client_UserUnbanned;
            Client.RoleCreated += Client_RoleCreated;
            Client.RoleDeleted += Client_RoleDeleted;
            Client.RoleUpdated += Client_RoleUpdated;
            Client.InviteCreated += Client_InviteCreated;
            Client.InviteDeleted += Client_InviteDeleted;
            Client.MessageDeleted += Client_MessageDeleted;
            Client.MessagesBulkDeleted += Client_MessagesBulkDeleted;

            await Client.LoginAsync(TokenType.Bot, Token);
            await Client.StartAsync();
            ModuleManager.InitializeModules();

            await Task.Delay(-1);
        }

        private Task Client_MessagesBulkDeleted(System.Collections.Generic.IReadOnlyCollection<Cacheable<IMessage, ulong>> arg1, ISocketMessageChannel arg2)
        {
            var Channel = ((SocketGuildChannel)arg2);

            ServerLogging.AuditLog(GuildManager.GetGuild(Channel.Guild), "Message Bulk Delete", $"{arg1.Count} messages were deleted from {Channel.Name}");
            return Task.CompletedTask;
        }

        private Task Client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            var Channel = ((SocketGuildChannel)arg2);

            ServerLogging.AuditLog(GuildManager.GetGuild(Channel.Guild), "Message Delete", $"A message was deleted from {Channel.Name}");
            return Task.CompletedTask;
        }

        private Task Client_InviteDeleted(SocketGuildChannel arg1, string arg2)
        {
            ServerLogging.AuditLog(GuildManager.GetGuild(arg1.Guild), "Invite Delete", $"An invite with code {arg2} was deleted for channel {arg1.Name}");
            return Task.CompletedTask;
        }

        private Task Client_InviteCreated(SocketInvite arg)
        {
            ServerLogging.AuditLog(GuildManager.GetGuild(arg.Guild), "Invite Create", $"An invite with code {arg.Code} was created for channel {arg.Channel.Name} by {GuildManager.GetUser(arg.Inviter).Name}");
            return Task.CompletedTask;
        }

        private Task Client_RoleUpdated(SocketRole arg1, SocketRole arg2)
        {
            var Updates = DiscordObjectComparison.Role(arg1, arg2);
            if (Updates.Count > 0)
                ServerLogging.AuditLog(GuildManager.GetGuild(arg1.Guild), "Role Update", Updates);
            return Task.CompletedTask;
        }

        private Task Client_RoleDeleted(SocketRole arg)
        {
            ServerLogging.AuditLog(GuildManager.GetGuild(arg.Guild), "Role Delete", $"The {arg.Name} role was deleted");
            return Task.CompletedTask;
        }

        private Task Client_RoleCreated(SocketRole arg)
        {
            ServerLogging.AuditLog(GuildManager.GetGuild(arg.Guild), "Role Create", $"The {arg.Name} role was created");
            return Task.CompletedTask;
        }

        private Task Client_UserUnbanned(SocketUser arg1, SocketGuild arg2)
        {
            ServerLogging.AuditLog(GuildManager.GetGuild(arg2), "User Unban", $"{arg1.Username} was unbanned");
            return Task.CompletedTask;
        }

        private Task Client_UserBanned(SocketUser arg1, SocketGuild arg2)
        {
            ServerLogging.AuditLog(GuildManager.GetGuild(arg2), "User Ban", $"{arg1.Username} was banned");
            return Task.CompletedTask;
        }

        private Task Client_ChannelDestroyed(SocketChannel arg)
        {
            var Channel = (SocketGuildChannel)arg;
            ServerLogging.AuditLog(GuildManager.GetGuild(Channel.Guild), "Channel Delete", $"{Channel.Name} was deleted");
            return Task.CompletedTask;
        }

        private Task Client_ChannelUpdated(SocketChannel arg1, SocketChannel arg2)
        {
            var Channel = (SocketGuildChannel)arg2;
            var Updates = DiscordObjectComparison.Channel((SocketGuildChannel)arg1, (SocketGuildChannel)arg2);
            if(Updates.Count > 0)
            ServerLogging.AuditLog(GuildManager.GetGuild(Channel.Guild), "Channel Update", Updates);

            return Task.CompletedTask;
        }

        private Task Client_ChannelCreated(SocketChannel arg)
        {
            var Channel = (SocketGuildChannel)arg;
            ServerLogging.AuditLog(GuildManager.GetGuild(Channel.Guild), "Channel Create", $"{Channel.Name} was created");
            return Task.CompletedTask;
        }

        private Task Client_GuildUpdated(SocketGuild arg1, SocketGuild arg2)
        {
            var Updates = DiscordObjectComparison.Guild(arg1, arg2);
            if (Updates.Count > 0)
                ServerLogging.AuditLog(GuildManager.GetGuild(arg2), "Guild Update", Updates);
            return Task.CompletedTask;
        }

        private Task Client_Disconnected(Exception arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask; 
        }

        private async Task Client_GuildMemberUpdated(SocketGuildUser arg1, SocketGuildUser arg2)
        {
            try
            {
                var Guild = GuildManager.GetGuild(arg2);

                if (arg2.IsPending is null)
                    return;

                if ((bool)arg1.IsPending && !(bool)arg2.IsPending)
                    ModerationManager.GiveJoinRole(Guild, arg2);
            }
            catch(Exception e)
            {
                ErrorHandling.GlobalErrorLogging(e.Message, $"Guild Memeber Updated\nUser Value 1: {arg1.Username} | User Value 2: {arg2.Username}");
            }

            var Updates = DiscordObjectComparison.User(arg1, arg2);
            if (Updates.Count > 0)
                ServerLogging.AuditLog(GuildManager.GetGuild(arg2),"User Update", Updates);

            await Task.CompletedTask;

        }

        private async Task Client_UserLeft(SocketGuildUser arg)
        {
            ModuleManager.OnUserLeft(arg);
            await Task.CompletedTask;
        }

        private async Task Client_UserJoined(SocketGuildUser arg)
        {
            ModuleManager.OnUserJoined(arg);
            await Task.CompletedTask;
        }

        private async Task Client_LeftGuild(SocketGuild arg)
        {
            ModuleManager.OnBotLeaveGuild(arg);
            await Task.CompletedTask;
        }

        private async Task Client_JoinedGuild(SocketGuild arg)
        {
            ModuleManager.OnBotJoinGuild(arg);
            await Task.CompletedTask;
        }

        private async Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            ModuleManager.OnReactionRemoved(Cache, Channel, Reaction);
            await Task.CompletedTask;
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            ModuleManager.OnReactionAdded(Cache, Channel, Reaction);
            await Task.CompletedTask;
        }

        private async Task Client_Log(LogMessage arg)
        {
            Console.WriteLine($"{DateTime.Now} at {arg.Source}: {arg.Message}");//debug log message formatting
            if (!(arg.Message is null))
                if (arg.Severity == LogSeverity.Critical || arg.Severity == LogSeverity.Error || arg.Message.Equals("Connecting") || arg.Message.Equals("Disconnected") || arg.Message.Equals("Disconnecting"))
                    ErrorHandling.GlobalErrorLogging(arg.Message, arg.Source);

            await Task.CompletedTask;
        }

        private async Task Client_Ready()
        {
            string game = Game;
            await Client.SetGameAsync(game, null, ActivityType.Watching).ConfigureAwait(false);//what game is the bot "playing"

            foreach (var Guild in Client.Guilds)
                GuildManager.CompareGuildUsers(Guild);
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage Message))
                return;

            var Context = new SocketCommandContext(Client, Message);

            if (Context.Message == null || Context.Message.Content == "") return; //dont want empty messages
            if (Context.User.IsBot) return;//dont want messages from bot
            int argpos = 0;

            if (GuildManager.GetUser(Context.User).Moderation.isMuted()) await arg.DeleteAsync();

            bool HasPfx = false;

            GuildManager.UpdateGuildName(Context.Guild);
            ModuleManager.MessageTunnel(arg);

            if (GuildManager.GetGuild(Context.User).Configuration.Prefixes.Count > 0)
                foreach (var prefix in GuildManager.GetGuild(Context.User).Configuration.Prefixes)
                {
                    if (Message.HasStringPrefix(prefix, ref argpos))
                        HasPfx = true;
                }

            if (!(Message.HasStringPrefix(Prefix, ref argpos) || Message.HasMentionPrefix(Client.CurrentUser, ref argpos) || HasPfx)) return;
            //only want messages with prefix or @bot mention, or guild specific prefix

            
            if (Message.Content.ToLowerInvariant().Equals(Prefix + GuildManager.GetGuild(Context.Guild.Id).Moderation.JoinCommand))//Handle Join Message
            {
                if (!ModerationManager.JoinCommand(Message))
                    await Context.Channel.SendMessageAsync(Properties.Resources.NoJoinRole);

                await arg.DeleteAsync();
            }

            var Result = await Commands.ExecuteAsync(Context, argpos, Service).ConfigureAwait(false);

            if (!Result.IsSuccess)
            {
                ErrorHandling.DotNetCommandException((CommandError)Result.Error, Context);
                Console.WriteLine($"{DateTime.Now} at Commands: Something went wrong while evecuting a command. Text: {Context.Message.Content} | Error: {Result.ErrorReason}");//what went wrong?
            }
        }
        //TODOL: Announcements
        //TODOL: Voting / Poll
        //TODOL: Dice
        //TODOL: Server Info
    }
}