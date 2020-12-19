using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Permissions
{
    public class PermissionsData
    {
        public bool Mod {
            get 
            {
                if (!Mod)
                    if (!Admin)
                        return false;
                return true;
            }
            set
            {
                Mod = value;
            } 
        }
        public bool Admin { get; set; }

        public PermissionsData()
        {
            Mod = false;
            Admin = false;
        }
    }
}
