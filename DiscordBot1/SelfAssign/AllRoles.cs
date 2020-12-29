using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.SelfAssign
{//level 2
    public class AllRoles
    {
        public RoleCategory<RoleObject> Consoles { get; set; }
        public RoleCategory<RoleObject> TimeZones { get; set; }
        public List<RoleCategory<RoleObject>> Misc { get; set; }//level 5

        public AllRoles()
        {
            Consoles = new RoleCategory<RoleObject>
            {
                Roles = new List<RoleObject>()
            };

            TimeZones = new RoleCategory<RoleObject>
            {
                Roles = new List<RoleObject>()
            };

            Misc = new List<RoleCategory<RoleObject>>();
        }
    }
}