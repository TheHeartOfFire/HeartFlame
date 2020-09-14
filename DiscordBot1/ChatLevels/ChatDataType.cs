using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot1.ChatLevels
{
    public class ChatDataType : IComparable<ChatDataType>, IEquatable<ChatDataType>
    {
        public ulong DiscordID { get; set; }
        public string DiscordUsername { get; set; }
        public int ChatLevel { get; set; }
        public int ChatExp { get; set; }
        public int MessagesSent { get; set; }
        public int[] ColorARGB { get; set; }
        public bool ExpPending { get; set; }
        public bool LevelPending { get; set; }
        public string ProfileImage { get; set; }
        public string BannerImage { get; set; }
        public bool TextBackground { get; set; }
        public int Greyscale { get; set; }

        public int CompareTo(ChatDataType other)
        {
            if (other is null) return 1;
            else return other.ChatExp.CompareTo(ChatExp);
        }

        public bool Equals(ChatDataType other)
        {
            if (other is null) return false;
            return DiscordID == other.DiscordID;
        }
    }
}