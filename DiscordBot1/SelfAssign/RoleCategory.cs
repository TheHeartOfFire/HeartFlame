using HeartFlame.Misc;
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
            if (!Roles.Exists(x => x.RoleID == RoleID))
                return;
            int Position = GetRole(RoleID).Position;

            Roles.Remove(GetRole(RoleID));

            foreach(var Role in Roles)
            {
                 if(Role.Position > Position)
                {
                    Role.Position--;
                    Role.Emoji = GetEmoji(Role);
                }  
            }

            PersistentData.SaveChangesToJson();
        }

        public virtual void AddRole(string Name, int Position, ulong RoleID, string Emoji = null)
        {
            if (Roles is null)
                Roles = new List<RoleObject>();

            if (Roles.Count > 0)
                foreach (var role in Roles)
                {
                    if (role.Position >= Position)
                    {
                        role.Position++;
                        role.Emoji = GetEmoji(role);
                    }
                }

            var Role = new RoleObject
            {
                Name = Name,
                Emoji = Emoji,
                Position = Position,
                RoleID = RoleID
            };

            if (Role.Emoji is null)
                Role.Emoji = GetEmoji(Role);

            Roles.Add(Role);


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

        protected string GetEmoji(RoleObject Role)
        {
            if (Role.Name.Equals("xbox", StringComparison.OrdinalIgnoreCase))
                return EmoteRef.Emotes.GetValueOrDefault("Xbox");
            if (Role.Name.Equals("PlayStation", StringComparison.OrdinalIgnoreCase))
                return EmoteRef.Emotes.GetValueOrDefault("PlayStation");
            if (Role.Name.Equals("PC", StringComparison.OrdinalIgnoreCase))
                return EmoteRef.Emotes.GetValueOrDefault("PC");
            if (Role.Name.Equals("Nintendo", StringComparison.OrdinalIgnoreCase))
                return EmoteRef.Emotes.GetValueOrDefault("Nintendo");

            return EmoteRef.Emotes.GetValueOrDefault((Role.Position+1).ToString());
        }
    }
}