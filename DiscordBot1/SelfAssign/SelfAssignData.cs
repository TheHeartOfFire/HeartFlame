using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.SelfAssign
{//level 2
    public class SelfAssignData
    {
        public RoleCategory<RoleObject> Consoles { get; set; }
        public RoleCategory<RoleObject> TimeZones { get; set; }
        public List<RoleCategory<RoleObject>> Misc { get; set; }//level 5

        public SelfAssignData()
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

        public void AddCustom(RoleCategory<RoleObject> Module)
        {
            Misc.Add(Module);
            PersistentData.SaveChangesToJson();
        }

        public void RemoveCustom(RoleCategory<RoleObject> Module)
        {
            Misc.Remove(Module);
            PersistentData.SaveChangesToJson();
        }
    }
}