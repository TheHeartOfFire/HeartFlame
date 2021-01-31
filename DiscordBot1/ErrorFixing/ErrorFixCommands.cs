using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.ErrorFixing
{
    [Group("Error")]
    [RequirePermission(Roles.CREATOR)]
    public class ErrorFixCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Beta")]
        public async Task Beta()
        {
            foreach (var _User in GuildManager.GetAllUsers())
            {
                _User.Banner.Badges.Global.BetaTester = true;
            }
            PersistentData.SaveChangesToJson();

            await ReplyAsync($"All users have bee set as beta testers.");
        }

        [Command("Badges")]
        public async Task Badges(SocketGuildUser User = null)
        {
            bool Beta = false;
            bool Rank1 = false;
            bool Patreon = false;
            var GUser = GuildManager.GetUser(User is null ? Context.User : User);

            var Profiles = GuildManager.GetGlobalUser(GUser);

            foreach (var Profile in Profiles)
            {
                var Badges = Profile.Banner.Badges.Global;
                if (Badges.BetaTester)
                    Beta = true;
                if (Badges.Patreon)
                    Patreon = true;
                if (Badges.Rank1)
                    Rank1 = true;
            }

            foreach (var Profile in Profiles)
            {
                var Badges = Profile.Banner.Badges.Global;
                Badges.BetaTester = Beta;
                Badges.Rank1 = Rank1;
                Badges.Patreon = Patreon;
            }

            PersistentData.SaveChangesToJson();

            await ReplyAsync($"{GUser.Name}'s various profiles have had their badge data updated.");
        }
    }
}
