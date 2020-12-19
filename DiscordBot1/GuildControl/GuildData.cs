using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.Configuration;
using HeartFlame.Misc;
using HeartFlame.Permissions;
using HeartFlame.SelfAssign;
using System.Collections.Generic;

namespace HeartFlame.GuildControl
{
    public class GuildData
    {
        public ulong GuildID { get; }
        public ModuleControlData ModuleControl { get; set; }
        public AllRoles SelfAssign { get; set; }
        public GuildConfigurationData Configuration { get; set; }
        public List<GuildUser> Users { get; set; }

        public GuildData(ulong guildID, List<SocketGuildUser> users)
        {
            GuildID = guildID;
            ModuleControl = new ModuleControlData();
            SelfAssign = new AllRoles();
            Configuration = new GuildConfigurationData();
            Users = new List<GuildUser>();
            AddUsers(users);
        }

        private void AddUsers(List<SocketGuildUser> users)
        {
            foreach(var User in users)
            {
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
            foreach(var GUser in Users)
            {
                if(GUser.DiscordID == User.Id)
                {
                    return GUser;
                }
            }
            return null;
        }
        
    }
}
