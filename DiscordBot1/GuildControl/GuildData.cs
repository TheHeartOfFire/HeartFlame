using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.Configuration;
using HeartFlame.Misc;
using HeartFlame.Moderation;
using HeartFlame.Permissions;
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
        public ModuleControlData ModuleControl { get; set; }
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
            ModuleControl = new ModuleControlData();
            SelfAssign = new AllRoles();
            Configuration = new GuildConfigurationData();
            Users = new List<GuildUser>();
            AddUsers(guild.Users);
        }
        [JsonConstructor]
        public GuildData()
        {

        }

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
            if (User is null) return null;
            foreach(var GUser in Users)
            {
                if(GUser.DiscordID == User.Id)
                {
                    return GUser;
                }
            }
            return null;
        }

        public GuildUser GetUser(SocketUser User)
        {

            foreach (var GUser in Users)
            {
                if (GUser.DiscordID == User.Id)
                {
                    return GUser;
                }
            }
            return null;
        }
    }
}
