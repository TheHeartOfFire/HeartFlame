﻿using HeartFlame.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Configuration
{
    public class GuildConfigurationData
    {//level 2
        public ulong LogChannel { get; set; }
        public ulong ChatChannel { get; set; }
        public bool UseChatChannel { get; set; }
        public LoggingConfig Logging { get; set; }
        public List<string> Prefixes { get; set; }
        
        public GuildConfigurationData()
        {
            Logging = new LoggingConfig();
            Prefixes = new List<string>();
        }



    }
}
