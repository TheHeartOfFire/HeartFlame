using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.GuildControl;
using HeartFlame.Permissions;
using HeartFlame.Time;
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
            await Context.Channel.SendMessageAsync("", false, TimeManager.BuildEmbed(Context.User));

        }


    }
}
