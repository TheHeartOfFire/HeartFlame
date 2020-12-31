using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.GuildControl;
using System.Threading.Tasks;

namespace HeartFlame.Misc
{
    public class TestCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Test")]
        public async Task Test(SocketGuildUser arg)
        {
            if (!Context.User.Id.ToString().Equals(Properties.Resources.CreatorID))
                return;

            GuildManager.GetGuild(arg).AddUser(arg);
            PersistentData.SaveChangesToJson();
        }
    }
}
