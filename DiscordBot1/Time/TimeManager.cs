using Discord;
using HeartFlame.GuildControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.WebSocket;
using HeartFlame.SelfAssign;

namespace HeartFlame.Time
{
    public class TimeManager
    {
        public static RoleObject UserTimezone(IUser User)
        {
            var Guild = GuildManager.GetGuild(User);
            var Roles = ((SocketGuildUser)User).Roles.ToList();


            foreach (var Role in Guild.SelfAssign.TimeZones.Roles)
            {
                if (Roles.Exists(x => x.Id == Role.RoleID))
                    return Guild.SelfAssign.TimeZones.GetRole(Role);
            }

            return null;
        }

        public static Embed BuildEmbed(SocketUser User)
        {
            return BuildEmbed((SocketGuildUser)User);
        }

        public static Embed BuildEmbed(SocketGuildUser User)
        {
            var GUser = GuildManager.GetUser(User);
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("World Time");
            Embed.WithColor(128, 255, 0);
            var TZone = GetTimezone("-8");
            Embed.AddField(TZone.DisplayName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone), true);
            TZone = GetTimezone("az");
            Embed.AddField(TZone.DisplayName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone), true);
            TZone = GetTimezone("-7");
            Embed.AddField(TZone.DisplayName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone), true);
            TZone = GetTimezone("-6");
            Embed.AddField(TZone.DisplayName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone), true);
            TZone = GetTimezone("-5");
            Embed.AddField(TZone.DisplayName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone), true);
            TZone = GetTimezone("-4");
            Embed.AddField(TZone.DisplayName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone), true);
            TZone = GetTimezone("0");
            Embed.AddField(TZone.DisplayName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone), true);
            TZone = GetTimezone("+1");
            Embed.AddField(TZone.DisplayName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone), true);
            TZone = GetTimezone("+10");
            Embed.AddField(TZone.DisplayName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone), true);
            TimeZoneRole userTimeZone = UserTimezone(User) as TimeZoneRole;
            if (!(userTimeZone is null))
                Embed.AddField($"{GUser.Name}'s TimeZone is {TimeZoneInfo.FindSystemTimeZoneById(userTimeZone.TimeZoneID).StandardName}",
                    $"Their current time is {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(userTimeZone.TimeZoneID))}");
            return Embed.Build();
        }

        public static TimeZoneInfo GetTimezone(string TZone)
        {
            if(Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Hawaii", "-10", "hst"))
                return TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Alaska", "-9"))
                return TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Pacific", "-8", "pst", "(UTC-08:00) Pacific Time (US & Canada)"))
                return TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Arizona", "az", "(UTC-07:00) Arizona"))
                return TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Mountain", "-7", "mst", "(UTC-07:00) Mountain Time (US & Canada)"))
                return TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Central", "-6", "cst", "(UTC-06:00) Central Time (US & Canada)"))
                return TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Eastern", "-5", "est", "(UTC-05:00) Eastern Time (US & Canada)"))
                return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Atlantic", "-4", "ast"))
                return TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Dublin", "London", "gmt", "0", "-0", "+0", "(UTC+00:00) Dublin, Edinburgh, Lisbon, London"))
                return TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Amsterdam", "+1", "west", "berlin", "rome", "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"))
                return TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Jerusalem", "+2", "ist", "Israel"))
                return TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Moscow", "+3", "rst"))
                return TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Samara", "+4", "rtz3"))
                return TimeZoneInfo.FindSystemTimeZoneById("Russia Time Zone 3");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Pakistan", "+5", "pak"))
                return TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Central Asia", "+6", "cast"))
                return TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Bangkok", "+7", "sest"))
                return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Beijing", "+8", "China", "Hong Kong"))
                return TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Tokyo", "+9", "Japan"))
                return TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Australia", "+10", "Sydney", "AUS", "(UTC+10:00) Canberra, Melbourne, Sydney"))
                return TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Soloman Islands", "+11", "CPST"))
                return TimeZoneInfo.FindSystemTimeZoneById("Central Pacific Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "New Zealand", "+12", "NZST"))
                return TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
            if (Utils.AdvancedCompare(StringComparison.OrdinalIgnoreCase, TZone, "Samoa", "+13", "SST"))
                return TimeZoneInfo.FindSystemTimeZoneById("Samoa Standard Time");

            return TimeZoneInfo.Utc;
        }
    }
}
