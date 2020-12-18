using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Configuration
{
    public class GuildConfiguration
    {
        public List<ulong> LogChannel { get; set; }
        public List<ulong> ChatChannel { get; set; }
        public bool UseChatChannel { get; set; }

        public GuildConfiguration()
        {
            LogChannel = new List<ulong>();
            ChatChannel = new List<ulong>();
        }
    }
}
