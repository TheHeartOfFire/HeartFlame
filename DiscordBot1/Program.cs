using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using HeartFlame.GuildControl;
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
            await Client.LoginAsync(TokenType.Bot, Token);
            await Client.StartAsync();
            ModuleManager.InitializeModules();

            await Task.Delay(-1);
        }


        private async Task Client_GuildMemberUpdated(SocketGuildUser arg1, SocketGuildUser arg2)
        {
            var Guild = GuildManager.GetGuild(arg2);

            if ((bool)arg1.IsPending && !(bool)arg2.IsPending)
                ModerationManager.GiveJoinRole(Guild, arg2);

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
            GuildManager.RemoveGuild(arg.Id);
            await Task.CompletedTask;
        }

        private async Task Client_JoinedGuild(SocketGuild arg)
        {
            GuildManager.AddGuild(arg);
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
            await Task.CompletedTask;
        }

        private async Task Client_Ready()
        {
            string game = Game;
            await Client.SetGameAsync(game).ConfigureAwait(false);//what game is the bot "playing"
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            var Message = arg as SocketUserMessage;
            if (Message is null)
                return;

            var Context = new SocketCommandContext(Client, Message);

            if (Context.Message == null || Context.Message.Content == "") return; //dont want empty messages
            if (Context.User.IsBot) return;//dont want messages from bot
            int argpos = 0;

            if (GuildManager.GetUser(Context.User).Moderation.isMuted()) await arg.DeleteAsync();

            bool HasPfx = false;

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
            GuildManager.UpdateGuildName(Context.Guild);
            ModuleManager.MessageTunnel(arg);
        }
    }
}