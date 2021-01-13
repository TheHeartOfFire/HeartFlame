using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HeartFlame.Logging
{
    public class LoggingConfig
    {
        [DefaultValue(true)]
        public bool LogJoins { get; set; }
        [DefaultValue(true)]
        public bool LogLeaves { get; set; }
        public bool SplitJoinLeave { get; set; }
        public ulong JoinChannel { get; set; }
        public ulong LeaveChannel { get; set; }
        public ulong ServerLoggingChannel { get; set; }
        public ulong LastAuditLog { get; set; }
    }
}
