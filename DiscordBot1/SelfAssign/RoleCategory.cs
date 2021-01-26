using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HeartFlame.SelfAssign
{//level 3
    public class RoleCategory : IEquatable<RoleCategory>
    {
        public ulong MsgID { get; set; }
        public List<RoleObject> Roles { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public ulong DividerRoleID { get; set; }

        public bool Equals([AllowNull] RoleCategory other)
        {
            if (other is null) return false;
            return MsgID == other.MsgID;
        }

        public void RemoveRole(ulong RoleID)
        {
            foreach(var Role in Roles)
            {
                if (Role.RoleID == RoleID)
                    Roles.Remove(Role);
            }
            PersistentData.SaveChangesToJson();
        }

        public void AddRole(string Name, string Emoji, int Position, ulong RoleID)
        {
            foreach (var role in Roles)
            {
                if (role.Position >= Position)
                    role.Position++;
            }

            Roles.Add(new RoleObject
            {
                Name = Name,
                Emoji = Emoji,
                Position = Position,
                RoleID = RoleID
            });

            Roles.Sort();
            PersistentData.SaveChangesToJson();
        }

        public RoleObject GetRole(ulong RoleID)
        {
            foreach (var Role in Roles)
            {
                if (Role.RoleID == RoleID)
                    return Role;
            }

            return null;
        }

        public void SetDivider(ulong RoleID)
        {
            DividerRoleID = RoleID;
            PersistentData.SaveChangesToJson();
        }

        public void SetMessageID(ulong MessageID)
        {
            MsgID = MessageID;
            PersistentData.SaveChangesToJson();
        }
    }
}