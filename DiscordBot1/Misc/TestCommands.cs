using Discord.Commands;
using HeartFlame.ChatLevels;
using HeartFlame.GuildControl;
using System.Threading.Tasks;

namespace HeartFlame.Misc
{
    public class TestCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Test")]
        public async Task Test(ulong id)
        {
            if (!Context.User.Id.ToString().Equals(Properties.Resources.CreatorID))
                return;

            PersistentData.Data.Config.Reporting.MessageID = id;
            PersistentData.SaveChangesToJson();
        }
    }
}
