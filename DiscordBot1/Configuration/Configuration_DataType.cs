using System.Collections.Generic;

namespace HeartFlame.Configuration
{
    public class Configuration_DataType
    {
        public string Token { get; set; }
        public string CommandPrefix { get; set; }
        public string Game { get; set; }
        public List<ulong> LogChannel { get; set; }
        public List<ulong> ChatChannel { get; set; }
        public bool UseChatChannel { get; set; }

        public Configuration_DataType()
        {
            LogChannel = new List<ulong>();
            ChatChannel = new List<ulong>();
        }
    }
}