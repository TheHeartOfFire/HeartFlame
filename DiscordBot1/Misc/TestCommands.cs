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
            foreach(var User in GuildManager.GetAllUsers())
            {
                GuildManager.SetBetaTester(User);
            }

            await ReplyAsync("Your Patreon status is: " + GuildManager.GetUser(Context.User).Banner.Badges.Global.BetaTester.ToString());
        }


    }
}
