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
        public async Task Test(SocketGuildUser User)
        {
            GuildManager.GetUser(User).Banner.Badges.Rank1 = false;
            PersistentData.SaveChangesToJson();

            await ReplyAsync($"User's Rank1 badge has been removed.");
        }


    }
}
