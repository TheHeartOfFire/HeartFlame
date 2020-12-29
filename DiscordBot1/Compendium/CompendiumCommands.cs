using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Compendium
{
    [Group("Usernames"), Alias("User", "Username", "Users")]
    public class CompendiumCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Username Group"), Remarks("Usernames_Help")]
        public async Task UsernameHelp()
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            if (!BotGuild.ModuleControl.IncludeCompendium)
            {
                await ReplyAsync(Properties.Resources.NotComp);
                return;
            }

            var embeds = Configuration.Configuration_Command.HelpEmbed("Usernames Help", "Usernames_Help", 0);
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Get"), Summary("Get the username for the user. Input: string \"Xbox, Playstation, Activision etc\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
        public async Task UsernameGet(string Platform, SocketGuildUser User = null)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);
            var GSender = BotGuild.GetUser((SocketGuildUser)Context.User);

            if (!BotGuild.ModuleControl.IncludeCompendium)
            {
                await ReplyAsync(Properties.Resources.NotComp);
                return;
            }

            Platform = CompendiumManager.Normalizer(Platform);

            var Name = CompendiumManager.GetUsername(Platform, (SocketGuildUser)Context.User);
            if (User is null)
            {
                if (Name is null)
                {
                    await ReplyAsync(BadPlatform(Platform, (SocketGuildUser)Context.User));
                    return;
                }
                await ReplyAsync($"{GSender.Name}'s {Platform} username is: {Name}");
                return;
            }

            Name = CompendiumManager.GetUsername(Platform, User);
            if (Name is null)
            {
                await ReplyAsync(BadPlatform(Platform, (SocketGuildUser)Context.User));
                return;
            }
            await ReplyAsync($"{GUser.Name}'s {Platform} username is: {Name}");

        }


        [Command("Set"), Summary("Set the username for the user. Input: string \"Xbox, Playstation, Activision etc\" string \"username\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
        public async Task UsernameSet(string Platform, string Username, SocketGuildUser User = null)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var GUser = BotGuild.GetUser(User);
            var GSender = BotGuild.GetUser((SocketGuildUser)Context.User);

            if (!BotGuild.ModuleControl.IncludeCompendium)
            {
                await ReplyAsync(Properties.Resources.NotComp);
                return;
            }

            var NormPlat = CompendiumManager.Normalizer(Platform);
            if (User is null)
            {
                if (!CompendiumManager.SetUsername(Platform, Username, (SocketGuildUser)Context.User))
                {
                    await ReplyAsync(BadPlatform(Platform, (SocketGuildUser)Context.User));
                    return;
                }
                await ReplyAsync($"{GSender.Name}'s {NormPlat} username has been set to: {Username}");
                return;
            }

            if (BotGuild.ModuleControl.IncludePermissions)
                if (!GSender.Perms.Mod)
                {
                    await ReplyAsync(Properties.Resources.NotMod);
                    return;
                }

            if (!CompendiumManager.SetUsername(Platform, Username, User))
            {
                await ReplyAsync(BadPlatform(Platform, (SocketGuildUser)Context.User));
                return;
            }
            await ReplyAsync($"{GUser.Name}'s {NormPlat} username has been set to: {Username}");

        }
        [Group("All")]
        public class CompendiumAllCommands : ModuleBase<SocketCommandContext>
        {
            [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Username All Group"), Remarks("Usernames_All_Help")]
            public async Task UsernameHelp()
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                if (!BotGuild.ModuleControl.IncludeCompendium)
                {
                    await ReplyAsync(Properties.Resources.NotComp);
                    return;
                }

                var embeds = Configuration.Configuration_Command.HelpEmbed("Usernames All Help", "Usernames_All_Help", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("Platform"), Alias("plat", "console", "game"), Summary("Get all of the usernames for a platform. Input: string \"Xbox, Playstation, Activision etc\""), Priority(1)]
            public async Task UsernameAll(string Platform)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);

                if (!BotGuild.ModuleControl.IncludeCompendium)
                {
                    await ReplyAsync(Properties.Resources.NotComp);
                    return;
                }
                var Embeds = CompendiumManager.GetUsernamesForPlatform(Platform, (SocketGuildUser)Context.User);

                foreach (var Embed in Embeds)
                {
                    await ReplyAsync("", false, Embed);
                }
            }

            [Command("User"), Summary("Get all of the usernames for a user. Input: SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
            public async Task UsernameAll(SocketGuildUser User = null)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);

                if (!BotGuild.ModuleControl.IncludeCompendium)
                {
                    await ReplyAsync(Properties.Resources.NotComp);
                    return;
                }

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
