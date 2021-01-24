using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HeartFlame.SelfAssign
{//level 3
    public class RoleCategory<t> : IEquatable<RoleCategory<t>>
    {
        public ulong MsgID { get; set; }
        public List<t> Roles { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public ulong DividerRoleID { get; set; }

        public bool Equals([AllowNull] RoleCategory<t> other)
        {
            if (other is null) return false;
            return MsgID == other.MsgID;
        }
    }
}