using System.Collections.Generic;

namespace HeartFlame.SelfAssign
{
    public class RoleCategory<t>
    {
        public ulong MsgID { get; set; }
        public List<t> Roles { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public ulong DividerRoleID { get; set; }
    }
}