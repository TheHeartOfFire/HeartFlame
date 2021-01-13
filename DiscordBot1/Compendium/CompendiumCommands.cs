using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using HeartFlame.ModuleControl;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeartFlame.Logging;

namespace HeartFlame.Compendium
{
    [Group("Usernames"), Alias("User", "Username", "Users")]
    [RequireModule(Modules.COMPENDIUM)]
    public class CompendiumCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Username Group"), Remarks("Usernames_Help")]
        public async Task UsernameHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Usernames Help", "Usernames_Help", 0);
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Get"), Summary("Get the username for the user. Input: string \"Xbox, Playstation, Activision etc\""), Priority(1)]
        public async Task UsernameGet(string Platform)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GSender = BotGuild.GetUser((SocketGuildUser)Context.User);

            var Name = CompendiumManager.GetUsername(ref Platform, (SocketGuildUser)Context.User);
            if (Name is null)
            {
                await ReplyAsync(BadPlatform(Platform, (SocketGuildUser)Context.User));
                return;
            }
            await ReplyAsync($"{GSender.Name}'s {Platform} username is: {Name}");

        }

        [Command("Get"), Summary("Get the username for the mentioned user. Input: string \"Xbox, Playstation, Activision etc\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
        public async Task UsernameGet(string Platform, SocketGuildUser User)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);
            var Name = CompendiumManager.GetUsername(ref Platform, User);

            if (Name is null)
            {
                await ReplyAsync(BadPlatform(Platform, (SocketGuildUser)Context.User));
                return;
            }
            await ReplyAsync($"{GUser.Name}'s {Platform} username is: {Name}");
        }

        [Command("Set"), Summary("Set the username for the user. Input: string \"Xbox, Playstation, Activision etc\" string \"username\""), Priority(1)]
        public async Task UsernameSet(string Platform, string Username)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GSender = BotGuild.GetUser((SocketGuildUser)Context.User);

            if (!CompendiumManager.SetUsername(ref Platform, Username, (SocketGuildUser)Context.User))
            {
                await ReplyAsync(BadPlatform(Platform, (SocketGuildUser)Context.User));
                return;
            }

            await ReplyAsync($"{GSender.Name}'s {Platform} username has been set to: {Username}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                    $"{GSender.Name}'s {Platform} username has been set to {Username}.",
                    Context);
        }

        [Command("Set"), Summary("Set the username for the mentioned user. Input: string \"Xbox, Playstation, Activision etc\" string \"username\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
        [RequirePermission(Roles.MOD)]
        public async Task UsernameSet(string Platform, string Username, SocketGuildUser User)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);


            if (!CompendiumManager.SetUsername(ref Platform, Username, User))
            {
                await ReplyAsync(BadPlatform(Platform, (SocketGuildUser)Context.User));
                return;
            }

            await ReplyAsync($"{GUser.Name}'s {Platform} username has been set to: {Username}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                    $"{GUser.Name}'s {Platform} username has been set to {Username}.",
                    Context);
        }

        [Group("All")]
        public class CompendiumAllCommands : ModuleBase<SocketCommandContext>
        {
            [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Username All Group"), Remarks("Usernames_All_Help")]
            public async Task UsernameHelp()
            {
                var embeds = Configuration.Configuration_Command.HelpEmbed("Usernames All Help", "Usernames_All_Help", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("Platform"), Alias("plat", "console", "game"), Summary("Get all of the usernames for a platform. Input: string \"Xbox, Playstation, Activision etc\""), Priority(1)]
            public async Task UsernameAll(string Platform)
            {
                var Embeds = CompendiumManager.GetUsernamesForPlatform(Platform, (SocketGuildUser)Context.User);

                foreach (var Embed in Embeds)
                {
                    await ReplyAsync("", false, Embed);
                }
            }

            [Command("User"), Summary("Get all of the usernames for a user. Input: SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
            public async Task UsernameAll(SocketGuildUser User = null)
            {
                if (User is null) User = (SocketGuildUser)Context.User;

                var Embed = CompendiumManager.GetUsernamesForUser(User);

                await ReplyAsync("", false, Embed);
            }
        }
        

        private static string BadPlatform(string Platform, SocketGuildUser User)
        {
            string Output = ":x: Uh oh, it appears that you have tried to get a username for an unsupported platform. Below are the supported platforms. Please check your spelling and try again!";

            var Guild = GuildManager.GetGuild(User.Guild.Id);
            var GUser = Guild.GetUser(User);

            Output += "\nGaming platforms:";
            foreach (var Prop in GUser.Usernames.Games.GetType().GetProperties())
            {
                Output += "\n" + Prop.Name;
            }

            Output += "\n\nSocial platforms:";
            foreach (var Prop in GUser.Usernames.Social.GetType().GetProperties())
            {
                Output += "\n" + Prop.Name;
            }
            
            return Output;
        }
    }
}
