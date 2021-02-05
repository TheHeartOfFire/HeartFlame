using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.ModuleControl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Time
{
    [Group("Time")]
    [RequireModule(Modules.TIME)]
    public class TimeCommands : ModuleBase<SocketCommandContext>
    {

        [Command("Help"), Alias("?"), Summary("Get all of the commands in the Time Group"), Remarks("Time_Help")]
        public async Task SelfAssignHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Time Help", "Time_Help");
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command]
        public async Task Time()
        {
            await Context.Channel.SendMessageAsync("", false, TimeManager.BuildEmbed(Context.User));
        }

        [Command]
        public async Task Time(SocketGuildUser User)
        {
            await Context.Channel.SendMessageAsync("", false, TimeManager.BuildEmbed(User));
        }
    }
}
