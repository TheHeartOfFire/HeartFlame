﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.Misc;
using System;
using System.Collections.Generic;

namespace HeartFlame.GuildControl
{
    public class GuildManager
    {
        public static void AddGuild(SocketGuild guild)
        {
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if(Guild.GuildID == guild.Id)
                    return;
            }

            PersistentData.Data.Guilds.Add(new GuildData(guild));
            PersistentData.SaveChangesToJson();
        }

        public static void RemoveGuild(ulong GuildID)
        {
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == GuildID)
                    PersistentData.Data.Guilds.Remove(Guild);
            }
            PersistentData.SaveChangesToJson();
        }

        public static GuildData GetGuild(ulong GuildID)
        {
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == GuildID)
                    return Guild;
            }
            return null;
        }

        public static GuildData GetGuild(SocketGuild Guild)
        {
            return GetGuild(Guild.Id);
        }
        public static GuildData GetGuild(IUser User)
        {
            return GetGuild((SocketGuildUser)User);
        }
        public static GuildData GetGuild(SocketGuildUser User)
        {
            return GetGuild(User.Guild);
        }

        public static GuildData GetGuild(SocketUser User)
        {
            return GetGuild((SocketGuildUser)User);
        }

        public static void UpdateGuildName(SocketGuild Guild)
        {
            foreach(var BotGuild in PersistentData.Data.Guilds)
            {
                if (Guild.Id == BotGuild.GuildID)
                    if (!BotGuild.Name.Equals(Guild.Name))
                    {
                        BotGuild.Name = Guild.Name;
                        PersistentData.SaveChangesToJson();
                        return;
                    }
            }
        }

        public static GuildUser GetUser(IUser User) => GetUser(((SocketGuildUser)User).Guild, User.Id);
        public static GuildUser GetUser(SocketGuild Guild, ulong UserId)
        {
            return GetGuild(Guild).GetUser(UserId);
        }


        public static List<GuildUser> GetGlobalUser(IUser User) => GetGlobalUser(GetUser(User));
        public static List<GuildUser> GetGlobalUser(GuildUser User)
        {
            var Users = new List<GuildUser>();
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                foreach(var _user in Guild.Users)
                {
                    if (User.DiscordID == _user.DiscordID)
                        Users.Add(_user);
                }
            }
            return Users;
        }

        public static void SetGlobalRank1(GuildUser User)
        {
            foreach(var Record in GetGlobalUser(User))
            {
                Record.Banner.Badges.Global.Rank1 = true;
            }
            PersistentData.SaveChangesToJson();
        }

        public static void SetPatreon(GuildUser User)
        {
            foreach (var Record in GetGlobalUser(User))
            {
                Record.Banner.Badges.Global.Patreon = true;
            }
            PersistentData.SaveChangesToJson();
        }

        public static void SetBetaTester(GuildUser User)
        {
            foreach (var Record in GetGlobalUser(User))
            {
                Record.Banner.Badges.Global.BetaTester = true;
            }
            PersistentData.SaveChangesToJson();
        }

        public static List<GuildUser> GetAllUsers()
        {
            var UserList = new List<GuildUser>();
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                foreach(var User in Guild.Users)
                {
                    UserList.Add(User);
                }
            }
            return UserList;
        }

        public static void CompareGuildUsers(SocketGuild Guild)
        {
            var BotGuild = GetGuild(Guild);

            foreach(var User in Guild.Users)
            {
                if (!User.IsBot)
                    BotGuild.AddUser(User);
            }
            PersistentData.SaveChangesToJson();
        }
    }
}
