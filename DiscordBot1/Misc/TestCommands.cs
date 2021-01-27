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
            foreach(var _User in GuildManager.GetAllUsers())
            {
                _User.Banner.Badges.Global.BetaTester = true;
            }
            PersistentData.SaveChangesToJson();

            await ReplyAsync($"All users have bee set as beta testers.");
        }


    }
}
