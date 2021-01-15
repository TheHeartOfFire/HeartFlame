using HeartFlame.GuildControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.ChatLevels
{
    public class GlobalBadgeData
    {
        public bool Rank1 { get {return  } set { } }
        public bool BetaTester { get; set; }
        public bool Patreon { get; set; }

        [JsonConstructor]
        public GlobalBadgeData() { }

    }
}
