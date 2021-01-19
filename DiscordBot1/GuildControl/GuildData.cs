﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.Configuration;
using HeartFlame.Misc;
using HeartFlame.Moderation;
using HeartFlame.ModuleControl;
using HeartFlame.SelfAssign;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HeartFlame.GuildControl
{
    public class GuildData
    {//level 1
        public ulong GuildID { get; set; }
        public string Name { get; set; }
        public ModerationData Moderation { get; set; }
        public ModuleData ModuleControl { get; set; }
        public AllRoles SelfAssign { get; set; }
        public GuildConfigurationData Configuration { get; set; }
        public List<GuildUser> Users { get; set; }

        public GuildData(SocketGuild guild)
        {
            if (guild is null)
                return;
            guild.DownloadUsersAsync();
            Name = guild.Name;
            GuildID = guild.Id;
            Moderation = new ModerationData();
            ModuleControl = new ModuleData();
            SelfAssign = new AllRoles();
            Configuration = new GuildConfigurationData();
            Users = new List<GuildUser>();
            AddUsers(guild.Users);
        }
        [JsonConstructor]
        public GuildData() { }

        private void AddUsers(IReadOnlyCollection<SocketGuildUser> users)
        {
            foreach(var User in users)
            {
                if(!User.IsBot)
                    Users.Add(new GuildUser(User));
            }
        }

        public void AddUser(SocketGuildUser User)
        {
            if (UserExists(User)) return;
            Users.Add(new GuildUser(User));
        }

        public bool UserExists(SocketGuildUser User)
        {
            foreach(var GUser in Users)
            {
                if (User.Id == GUser.DiscordID)
                    return true;
            }
            return false;
        }
        
        public void RemoveUser(SocketGuildUser User)
        {
            foreach (var GUser in Users)
            {
                if (User.Id == GUser.DiscordID)
                    Users.Remove(GUser);
            }
        }

        public GuildUser GetUser(SocketGuildUser User)
        {
            return GetUser(User.Id);
        }

        public GuildUser GetUser(SocketUser User)
        {
            return GetUser(User.Id);
        }

        public GuildUser GetUser(IUser User)
        {
            return GetUser(User.Id);
        }

        public GuildUser GetUser(ulong UserID)
        {

            foreach (var GUser in Users)
            {
                if (GUser.DiscordID == UserID)
                {
                    return GUser;
                }
            }
            return null;
        }

        public IMessageChannel GetChatChannel(SocketCommandContext Context)
        {
            return GetChatChannel(Context.Channel);
        }

        public IMessageChannel GetChatChannel(IMessageChannel MessageChannel)
        {
            if (Configuration.UseChatChannel)
                return Program.Client.GetChannel(Configuration.ChatChannel) as ISocketMessageChannel;
            return MessageChannel;
        }

        
    }
}
