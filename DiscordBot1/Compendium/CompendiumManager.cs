using Discord;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Compendium
{
    public class CompendiumManager
    {
        public static string GetUsername(string Platform, SocketGuildUser User)
        {
            var Guild = GuildManager.GetGuild(User.Guild.Id);

            Platform = Normalizer(Platform);
            if (Platform is null)
                return null;

            var GUser = Guild.GetUser(User);

            foreach (var Prop in GUser.Usernames.Games.GetType().GetProperties())
            {
                if (Prop.Name.ToLower().Equals(Platform))
                    return (string)Prop.GetValue(GUser.Usernames.Games);

            }

            foreach (var Prop in GUser.Usernames.Social.GetType().GetProperties())
            {
                if (Prop.Name.ToLower().Equals(Platform))
                    return (string)Prop.GetValue(GUser.Usernames.Social);
            }
            

            return null;
        }
        /// <summary>
        /// False if bad platform
        /// </summary>
        /// <param name="Platform"></param>
        /// <param name="Username"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static bool SetUsername(string Platform, string Username, SocketGuildUser User)
        {
            var Guild = GuildManager.GetGuild(User.Guild.Id);

            Platform = Normalizer(Platform);
            if (Platform is null)
                return false;

            var GUser = Guild.GetUser(User);

            foreach (var Prop in GUser.Usernames.Games.GetType().GetProperties())
            {
                if (Prop.Name.ToLower().Equals(Platform))
                {
                    Prop.SetValue(GUser.Usernames.Games, Username);
                    PersistentData.SaveChangesToJson();
                    return true;
                }

            }

            foreach (var Prop in GUser.Usernames.Social.GetType().GetProperties())
            {
                if (Prop.Name.ToLower().Equals(Platform))
                {
                    Prop.SetValue(GUser.Usernames.Social, Username);
                    PersistentData.SaveChangesToJson();
                    return true;
                }
            }
            
            return false;
        }

        public static List<Embed> GetUsernamesForPlatform(string Platform, SocketGuildUser User)
        {
            var Guild = GuildManager.GetGuild(User.Guild.Id);

            Platform = Normalizer(Platform);
            if (Platform is null)
                return null;

            var Output = new List<Embed>();
            EmbedBuilder Embed = new EmbedBuilder();

            foreach (var GUser in Guild.Users)
            {
                foreach (var Prop in GUser.Usernames.Games.GetType().GetProperties())
                {
                    if (Prop.Name.ToLower().Equals(Platform))
                    {
                        if (!Prop.GetValue(GUser.Usernames.Games).Equals("Not Listed"))
                            Embed.AddField(GUser.Name, Prop.GetValue(GUser.Usernames.Games), true);

                        if (Embed.Fields.Count > 19)
                        {
                            Output.Add(Embed.Build());
                            Embed = new EmbedBuilder();
                        }
                    }
                }

                foreach (var Prop in GUser.Usernames.Social.GetType().GetProperties())
                {
                    if (Prop.Name.ToLower().Equals(Platform))
                    {
                        if (!Prop.GetValue(GUser.Usernames.Social).Equals("Not Listed"))
                            Embed.AddField(GUser.Name, Prop.GetValue(GUser.Usernames.Social), true);

                        if (Embed.Fields.Count > 20)
                        {
                            Output.Add(Embed.Build());
                            Embed = new EmbedBuilder();
                        }
                    }
                }
            }
            
            Output.Add(Embed.Build());
            return Output;
        }

        public static Embed GetUsernamesForUser(SocketGuildUser User)
        {
            var Guild = GuildManager.GetGuild(User.Guild.Id);
            var GUser = Guild.GetUser(User);

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithDescription(GUser.Name + "'s recorded usernames");

            Embed.AddField("~~~~~~~~~~~~~~~~~~~~", "Gaming Usernames");
            foreach(var Prop in GUser.Usernames.Games.GetType().GetProperties())
            {
                Embed.AddField(Prop.Name, Prop.GetValue(GUser.Usernames.Games), true);
            }

            Embed.AddField("~~~~~~~~~~~~~~~~~~~~", "Social Usernames");
            foreach (var Prop in GUser.Usernames.Social.GetType().GetProperties())
            {
                Embed.AddField(Prop.Name, Prop.GetValue(GUser.Usernames.Social), true);
            }

            return Embed.Build();
        }



        public static string Normalizer(string Platform)
        {
            Platform = Platform.ToLowerInvariant();

            if (Platform.Equals("youtube") || Platform.Equals("yt"))
                return "youtube";
            if (Platform.Equals("twitch") || Platform.Equals("ttv"))
                return "twitch";
            if (Platform.Equals("facebook") || Platform.Equals("fb"))
                return "facebook";
            if (Platform.Equals("twitter"))
                return "twitter";
            if (Platform.Equals("snapchat") || Platform.Equals("snap") || Platform.Equals("sc"))
                return "snapchat";
            if (Platform.Equals("instagram") || Platform.Equals("insta") || Platform.Equals("ig"))
                return "instagram";
            if (Platform.Equals("patreon"))
                return "patreon";
            if (Platform.Equals("xbox") || Platform.Equals("xbl") || Platform.Equals("gamertag") || Platform.Equals("gt"))
                return "xbox";
            if (Platform.Equals("playstation") || Platform.Equals("psn"))
                return "playstation";
            if (Platform.Equals("nintendo"))
                return "nintendo";
            if (Platform.Equals("steam"))
                return "steam";
            if (Platform.Equals("activision") || Platform.Equals("cod") || Platform.Equals("mw") || Platform.Equals("warzone") || Platform.Equals("cw"))
                return "activision";
            if (Platform.Equals("epic") || Platform.Equals("fortnite") || Platform.Equals("rocketleague"))
                return "epic";
            if (Platform.Equals("runescape") || Platform.Equals("rs"))
                return "runescape";
            if (Platform.Equals("worldofwarcraft") || Platform.Equals("wow"))
                return "worldofwarcraft";
            if (Platform.Equals("mojang") || Platform.Equals("minecraft") || Platform.Equals("mc"))
                return "mojang";
            if (Platform.Equals("electronicarts") || Platform.Equals("ea"))
                return "electronicarts";
            if (Platform.Equals("bethesda") || Platform.Equals("skyrim") || Platform.Equals("fallout") || Platform.Equals("elderscrolls") || Platform.Equals("eso"))
                return "bethesda";
            if (Platform.Equals("ubisoft") || Platform.Equals("kingdomhearts"))
                return "ubisoft";
            if (Platform.Equals("tiktok"))
                return "tiktok";

            return null;

        }
    }
}
