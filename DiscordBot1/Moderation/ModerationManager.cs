using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Moderation
{
    public class ModerationManager
    {
        /// <summary>
        /// returns true if successful
        /// </summary>
        /// <param name="Guild"></param>
        /// <returns></returns>
        public static bool JoinCommand(SocketUserMessage Message)
        {
            var User = (SocketGuildUser)Message.Author;
            var Guild = User.Guild;
            var Botguild = GuildManager.GetGuild(Guild);

            if (Botguild.Moderation.JoinRole is 0) return false;//if no role is set
            var Role = Guild.GetRole(Botguild.Moderation.JoinRole);

            if (Role is null) return false;//if role doesn't exist

            if (User.Roles.Contains(Role)) return true;//if user already has role

            User.AddRoleAsync(Role);
            return true;
        }

        public static bool MuteUser(string Duration, int Incriment, SocketGuildUser User)
        {
            var Time = ParseTime(Duration, Incriment);
            if (Time.Equals(TimeSpan.Zero))
                return false;

            var Guild = GuildManager.GetGuild(User.Guild);
            var GUser = Guild.GetUser(User);

            GUser.Moderation.Mute(Time);
            return true;
        }

        public static TimeSpan ParseTime(string Duration, int Incriment)
        {
            Duration = NormalizeTime(Duration);
            if (Duration is null)
                return TimeSpan.Zero;

            if (Duration.Equals("second"))
                return TimeSpan.FromSeconds(Incriment);
            if (Duration.Equals("minute"))
                return TimeSpan.FromMinutes(Incriment);
            if (Duration.Equals("hour"))
                return TimeSpan.FromHours(Incriment);
            if (Duration.Equals("day"))
                return TimeSpan.FromDays(Incriment);
            if (Duration.Equals("week"))
                return TimeSpan.FromDays(Incriment * 7);
            if (Duration.Equals("month"))
                return TimeSpan.FromDays(Incriment * 30);
            if (Duration.Equals("year"))
                return TimeSpan.FromDays(Incriment * 365);

            return TimeSpan.Zero;
        }

        public static string NormalizeTime(string Duration)
        {
            Duration = Duration.ToLowerInvariant();

            if (Duration.Equals("second") || Duration.Equals("sec") || Duration.Equals("s") || Duration.Equals("seconds") || Duration.Equals("secs"))
                return "second";
            if (Duration.Equals("minute") || Duration.Equals("min") || Duration.Equals("m") || Duration.Equals("minutes") || Duration.Equals("mins"))
                return "minute";
            if (Duration.Equals("hour") || Duration.Equals("hr") || Duration.Equals("h") || Duration.Equals("hours") || Duration.Equals("hrs"))
                return "hour";
            if (Duration.Equals("day") || Duration.Equals("d") || Duration.Equals("days"))
                return "day";
            if (Duration.Equals("week") || Duration.Equals("wk") || Duration.Equals("w") || Duration.Equals("weeks") || Duration.Equals("wks"))
                return "week";
            if (Duration.Equals("month") || Duration.Equals("mo") || Duration.Equals("months") || Duration.Equals("mos"))
                return "month";
            if (Duration.Equals("year") || Duration.Equals("yr") || Duration.Equals("y") || Duration.Equals("years") || Duration.Equals("yrs"))
                return "year";
            return null;
        }

        public static async void GiveJoinRole(GuildData Guild, SocketGuildUser User)
        {
            if (Guild.Moderation.JoinRole != 0)
            {
                var role = User.Guild.GetRole(Guild.Moderation.JoinRole);
                await User.AddRoleAsync(role);
            }
        }
    }
}
