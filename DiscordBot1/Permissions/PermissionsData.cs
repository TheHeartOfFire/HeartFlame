using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Permissions
{//level 4
    public class PermissionsData
    {
        public bool mod { get; set; }
        public bool Mod {
            get 
            {
                if (!mod)
                    if (!Admin)
                        return false;
                return true;
            }
            set
            {
                mod = value;
            } 
        }
        public bool Admin { get; set; }

    }
}
