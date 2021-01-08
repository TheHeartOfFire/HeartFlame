using Discord.WebSocket;
using HeartFlame.ChatLevels;
using HeartFlame.Compendium;
using HeartFlame.Moderation;
using HeartFlame.Permissions;
using Newtonsoft.Json;
using System;

namespace HeartFlame.GuildControl
{
    public class GuildUser : IComparable<GuildUser>, IEquatable<GuildUser>
    {//level 3
        public ulong DiscordID { get; set; }
        public string Name { get; set; }
        public PermissionsData Perms { get; set; }
        public ChatData Chat { get; set; }
        public BannerData Banner { get; set; }
        public CompendiumData Usernames { get; set; }
        public ModerationUserData Moderation { get; set; }
        public GuildUser(SocketGuildUser User)
        {
            DiscordID = User.Id;
            UpdateName(User);
            Perms = new PermissionsData();
            Banner = new BannerData();
            Usernames = new CompendiumData();
            Moderation = new ModerationUserData();
            Chat = new ChatData();
        }

        [JsonConstructor]
        public GuildUser()
        {

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
