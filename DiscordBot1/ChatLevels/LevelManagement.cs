using Discord;
using HeartFlame.GuildControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.ChatLevels
{
    public class LevelManagement
    {
        /// <summary>
        /// Input: Current Level
        /// Output: Total experience for next level
        /// </summary>
        /// <returns>Experience</returns>
        public static int GetExpAtLevel(int level)
        {
            int exp2 = 0;
            level++;
            for (double i = 1; i <= level - 1; i++)
            {
                exp2 += (int)Math.Floor(i + (300f * Math.Pow(2, i / 7f)));
            }

            return (int)Math.Floor((double)exp2 / 4f);
        }

        /// <summary>
        /// Input: Current experience
        /// Output: Level for current experience
        /// </summary>
        /// <returns>Level</returns>
        public static int GetLevelAtExp(int exp)
        {
            for (int i = 1; i <= 99; i++)
            {
                int TestExp = GetExpAtLevel(i);
                if (TestExp > exp)
                    return i;
                else if (TestExp == exp)
                    return i;
            }
            return -1;
        }

        public static Embed Top10(ulong GuildID)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (Guild.GuildID == GuildID)
                {
                    var Users = Guild.Users;
                    Users.Sort();
                    var Embed = new EmbedBuilder();
                    Embed.WithTitle("Chat Level Top 10");

                    var Count = 10;
                    if (Users.Count < 10)
                        Count = Users.Count;

                    for (int i = 0; i < Count; i++)
                    {
                        Embed.AddField($"Name: {Users[i].Name}", $"Level: {Users[i].ChatLevel}\nExperience: {Users[i].ChatExp}/{GetExpAtLevel(Users[i].ChatLevel)}\nMessages Sent: {Users[i].MessagesSent}");
                    }

                    return Embed.Build();
                }
            }
            return null;
        }
    }
}