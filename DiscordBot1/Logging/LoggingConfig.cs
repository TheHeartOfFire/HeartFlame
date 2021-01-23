using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HeartFlame.Logging
{
    public class LoggingConfig
    {
        public bool LogJoins { get; set; }
        public bool LogLeaves { get; set; }
        public bool SplitJoinLeave { get; set; }
        public bool SplitServerBotLogging { get; set; }
        public ulong JoinChannel { get; set; }
        public ulong LeaveChannel { get; set; }
        public ulong ServerLoggingChannel { get; set; }

        public LoggingConfig()
        {
            LogJoins = true;
            LogLeaves = true;
        }
    }
}
