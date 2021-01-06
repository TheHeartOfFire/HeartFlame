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
        public async Task Test()
        {
            await Context.Channel.SendFileAsync(BannerMaker.ToStream(await BannerMaker.Testing((SocketGuildUser)Context.User), ImageFormat.Png), "Test.png");
        }
    }
}
