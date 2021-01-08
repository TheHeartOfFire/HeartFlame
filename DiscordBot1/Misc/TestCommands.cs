using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.GuildControl;
using HeartFlame.Permissions;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace HeartFlame.Misc
{
    [Group("Test")]
    [RequirePermission(Roles.CREATOR)]
    public class TestCommands : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task Test(SocketGuildUser User = null)
        {
            await Context.Channel.SendFileAsync(BannerMaker.ToStream(await BannerMaker.BuildBannerAsync(User ?? (SocketGuildUser)Context.User, false), ImageFormat.Png), "Test.png");
        }
    }
}
