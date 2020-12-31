using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.GuildControl;
using HeartFlame.Permissions;
using System.Threading.Tasks;

namespace HeartFlame.Misc
{
    public class TestCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Test")]
        [RequirePermission(Roles.CREATOR)]
        public async Task Test(SocketGuildUser arg)
        {
            GuildManager.GetGuild(arg).AddUser(arg);
            PersistentData.SaveChangesToJson();
        }
    }
}
