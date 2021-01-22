using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Moderation
{//level 2
    public class ModerationData
    {
        public string JoinCommand { get; set; }
        public ulong JoinRole { get; set; }
        public bool UseJoinCommand { get; set; }
        public ModerationData()
        {
            JoinCommand = "Join";
        }
    }
}
