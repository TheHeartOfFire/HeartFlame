using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.SelfAssign
{//level 2
    public class SelfAssignData
    {
        public RoleCategory Consoles { get; set; }
        public RoleCategory TimeZones { get; set; }
        public List<RoleCategory> Misc { get; set; }//level 5

        public SelfAssignData()
        {
            Consoles = new RoleCategory
            {
                Roles = new List<RoleObject>()
            };

            TimeZones = new RoleCategory
            {
                Roles = new List<RoleObject>()
            };

            Misc = new List<RoleCategory>();
        }

        public RoleCategory AddCustom(string Name, string Title, ulong MsgID, List<RoleObject> Roles = null, ulong DividerID = 0)
        {
            var Module = new RoleCategory()
            {
                Name = Name,
                Title = Title,
                MsgID = MsgID
            };

            if (DividerID > 0)
                Module.DividerRoleID = DividerID;

            if (!(Roles is null))
                Module.Roles = Roles;

            return AddCustom(Module);
        }

        public RoleCategory AddCustom(RoleCategory Module)
        {
            if (Misc.Exists(x => x.Name == Module.Name))
                return null;

            Misc.Add(Module);
            PersistentData.SaveChangesToJson();
            return GetCustom(Module.Name);
        }

        public void RemoveCustom(RoleCategory Module)
        {
            Misc.Remove(Module);
            PersistentData.SaveChangesToJson();
        }

        public RoleCategory GetCustom(ulong MsgID)
        {
            foreach(var Module in Misc)
            {
                if (Module.MsgID == MsgID)
                    return Module;
            }
            return null;
        }

        public RoleCategory GetCustom(string Name)
        {
            foreach (var Module in Misc)
            {
                if (Module.Name == Name)
                    return Module;
            }
            return null;
        }
    }
}