using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.ChannelDirector
{
    [Group("Requirements"), Alias("req", "channels", "chnls")]
    public partial class ChannelDirectorCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("", "?"), Summary("Get all of the commands in the Requirements Group"), Remarks("Requirements_Help")]
        public async Task UsernameHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Requirements Help", "Requirements_Help");
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Get"), Alias("list"), Summary("Get the required channels")]
        public async Task GetChannels(bool MissingOrNot = false)
        {
            await ReplyAsync("", false, ChannelCreation.RequiredChannelsEmbed(Context.Guild, $"{PersistentData.Data.Config.CommandPrefix}Requirements Create {MissingOrNot}", MissingOrNot));
        }

        [Command("Create"), Alias("set", "make"), Summary("Create the required channels")]
        public async Task CreateChannels(bool MissingOrNot = false)
        {
            ChannelCreation.CreateRequiredChannels(Context.Guild, MissingOrNot);
            await Task.CompletedTask;
        }


    }
}
