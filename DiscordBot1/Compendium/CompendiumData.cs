using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Compendium
{//level 4
    public class CompendiumData
    {
        public GamingData Games { get; set; }
        public SocialData Social { get; set; }

        public CompendiumData()
        {
            Games = new GamingData();
            Social = new SocialData();
        }
    }
}
