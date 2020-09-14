using System.Collections.Generic;

namespace HeartFlame.Permissions
{
    public class PermissionsDataType
    {
        public List<Permissions_User> Mods { get; set; }
        public List<Permissions_User> Admins { get; set; }

        public PermissionsDataType()
        {
            Mods = new List<Permissions_User>();
            Admins = new List<Permissions_User>();
        }
    }
}