using HeartFlame.Configuration;
using HeartFlame.GuildControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Misc
{
    public class PersistentDataType
    {
        public Configuration_DataType Config { get; set; }
        public List<GuildData> Guilds { get; set; }

        public PersistentDataType()
        {
            Config = new Configuration_DataType();
            Guilds = new List<GuildData>();
        }
    }
}
