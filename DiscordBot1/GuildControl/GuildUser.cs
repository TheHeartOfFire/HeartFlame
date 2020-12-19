using Discord.WebSocket;
using System;
using System.Drawing;

namespace HeartFlame.GuildControl
{
    public class GuildUser : IComparable<GuildUser>, IEquatable<GuildUser>
    {
        public ulong DiscordID { get; private set; }
        public string Name { get; private set; }
        public bool Mod { private get; set; }
        public bool Admin { private get; set; }
        public int ChatLevel { get; set; }
        public int ChatExp { get; set; }
        public int MessagesSent { get; set; }
        private int[] ColorARGB { get; set; }
        public bool ExpPending { get; set; }
        public bool LevelPending { get; set; }
        public string ProfileImage { get; set; }
        public string BannerImage { get; set; }
        public bool TextBackground { get; set; }
        public int Greyscale { get; set; }

        public GuildUser(SocketGuildUser User)
        {
            DiscordID = User.Id;
            UpdateName(User);
            Mod = false;
            Admin = false;
        }

        public void UpdateName(SocketGuildUser User)
        {
            if (User.Nickname is null)
                Name = User.Username;
            else
                Name = User.Nickname;
        }

        public bool isMod()
        {
            if (Mod)
                return true;
            return Admin;
        }
        public bool isAdmin()
        {
            return Admin;
        }
        public int CompareTo(GuildUser other)
        {
            if (other is null) return 1;
            else return other.ChatExp.CompareTo(ChatExp);
        }

        public bool Equals(GuildUser other)
        {
            if (other is null) return false;
            return DiscordID == other.DiscordID;
        }

        public Color GetColor()
        {
            return ParseColor(ColorARGB);
        }
        private Color ParseColor(int[] input)
        {
            return Color.FromArgb(input[0], input[1], input[2], input[3]);
        }

        public void SetColor(Color color)
        {
            ColorARGB = new int[] { color.A, color.R, color.G, color.B };
        }
    }
}
