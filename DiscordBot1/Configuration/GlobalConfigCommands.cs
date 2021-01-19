using Discord.Commands;
using HeartFlame.GuildControl;
using HeartFlame.Logging;
using HeartFlame.Misc;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Configuration
{
    [Group("Global Configuration"), Alias("Global Config", "G Config", "GC")]
    [RequirePermission(Roles.CREATOR)]
    public class GlobalConfigCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Remarks("Global_Config_Help"), Summary("Get all of the commands in the Global Configuration Group.")]
        public async Task GlobalConfigHelp()
        {
            var embeds = Configuration_Command.HelpEmbed("Global Config Help", "Global_Config_Help", 0);
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Prefix"), Alias("pfx"), Summary("Set the prefix for all commands. A space is added to the end by default."), Priority(1)]
        public async Task SetPrefix(string Prefix)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);

            PersistentData.Data.Config.CommandPrefix = Prefix + " ";
            PersistentData.SaveChangesToJson();
            await Context.Channel.SendMessageAsync($"Command Prefix has been set to {Prefix}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                        $"Bot commands must now begin with \"{Prefix} \"",
                        Context);
        }

        [Command("Game"), Alias("g"), Summary("Set the game that discord shows the bot to be playing. Input: string \"name\""), Priority(1)]
        public async Task SetGame(string Game)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);

            PersistentData.Data.Config.Game = Game;
            PersistentData.SaveChangesToJson();
            await Context.Channel.SendMessageAsync($"{PersistentData.BotName} is now playing {Game}");

            if (BotGuild.ModuleControl.IncludeLogging)
                BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.DeclaringType,
                        $"The bot is now playing {Game}",
                        Context);
        }
    }
}
