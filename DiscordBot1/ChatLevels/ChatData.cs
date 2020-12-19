using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.ChatLevels
{
    public class ChatData
    {
        public int ChatLevel { get; set; }
        public int ChatExp { get; set; }
        public int MessagesSent { get; set; }
        public bool ExpPending { get; set; }
        public bool LevelPending { get; set; }
    }
}
