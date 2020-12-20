using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.Compendium;
using HeartFlame.Permissions;
using System;

namespace HeartFlame.GuildControl
{
    public class GuildUser : IComparable<GuildUser>, IEquatable<GuildUser>
    {
        public ulong DiscordID { get; private set; }
        public string Name { get; private set; }
        public PermissionsData Perms { get; set; }
        public ChatData Chat { get; set; }
        public BannerData Banner { get; set; }
        public CompendiumData Usernames { get; set; }

        public GuildUser(SocketGuildUser User)
        {
            DiscordID = User.Id;
            UpdateName(User);
            Perms = new PermissionsData();
            Banner = new BannerData();
            Usernames = new CompendiumData();
        }

        public void UpdateName(SocketGuildUser User)
        {
            if (User.Nickname is null)
                Name = User.Username;
            else
                Name = User.Nickname;
        }
        public int CompareTo(GuildUser other)
        {
            if (other is null) return 1;
            else return other.Chat.ChatExp.CompareTo(Chat.ChatExp);
        }

        public bool Equals(GuildUser other)
        {
            if (other is null) return false;
            return DiscordID == other.DiscordID;
        }

        
    }
}
