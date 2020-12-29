using Discord.Commands;
using System.Threading.Tasks;

namespace HeartFlame.Misc
{
    public class DonationCommands :ModuleBase<SocketCommandContext>
    {
        [Command("Donate")]
        public async Task Link()
        {
            await ReplyAsync("This bot is free to use and all server costs are paid out of pocket. There are no benefits for donation and it isn't compulsory in any way. All donations are however greatly appreciated and will go toward server costs and improving the quality of this bot. If you would like to donate you can do so at this patreon link: " + Properties.Resources.PatreonLink);
        }
    }
}
