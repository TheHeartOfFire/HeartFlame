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
    public class TimeCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Time")]
        [RequireModule(Modules.TIME)]
        public async Task Time()
        {
            await Context.Channel.SendMessageAsync("", false, TimeManager.BuildEmbed(Context.User));
        }

        [Command("Time")]
        [RequireModule(Modules.TIME)]
        public async Task Time(SocketGuildUser User)
        {
            await Context.Channel.SendMessageAsync("", false, TimeManager.BuildEmbed(User));
        }
        //TODO: be able to add or remove timezones from the embed
        //TODOH: Group time commands and add help command.
    }
}
