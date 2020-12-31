using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.Misc;
using HeartFlame.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartFlame.Reporting
{
    [Group("Reporting")]
    [RequirePermission(Roles.CREATOR)]
    public class ReportingCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Set")]
        public async Task SetPrimaryReport()
        {
            if (PersistentData.Data.Config.Reporting.MessageID != 0)
                ReportingManager.RemovePrimaryReport();

            var message = await ReplyAsync("Primary Report", false, ReportingManager.PrimaryReport());
            PersistentData.Data.Config.Reporting.MessageID = message.Id;
            await message.AddReactionAsync(Emote.Parse(EmoteRef.Emotes.GetValueOrDefault("ref")));
            PersistentData.SaveChangesToJson();
        }

        [Command("Remove")]
        public async Task RemovePrimaryReport()
        {
            if (PersistentData.Data.Config.Reporting.MessageID != 0)
                ReportingManager.RemovePrimaryReport();

            await ReplyAsync("Report Removed");

        }

        [Command("Guild")]
        public async Task SetGuild()
        {
            PersistentData.Data.Config.Reporting.GuildID = Context.Guild.Id;

            await ReplyAsync("This guild has been set as the guild for reporting.");
        }
    }
}
