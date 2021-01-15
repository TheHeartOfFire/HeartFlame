using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.ChatLevels
{
    public class BadgeData
    {
        public bool Rank1 { get; set; }
        public bool Rank2 { get; set; }
        public bool Rank3 { get; set; }
        public GlobalBadgeData Global { get; set; }

        public BadgeData()
        {
            Global = new GlobalBadgeData();
        }
    }
}
