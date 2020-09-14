using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DiscordBot1.SelfAssign
{
    public class RoleObject : IEquatable<RoleObject>, IComparable<RoleObject>
    {
        public ulong RoleID { get; set; }
        public string Emoji { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }

        public int CompareTo(RoleObject other)
        {
            if (other is null) return 1;
            else if (other.Position == Position) return string.Compare(Name, other.Name);
            else return Position.CompareTo(other.Position);
        }

        public bool Equals(RoleObject other)
        {
            if (other is null) return false;
            return RoleID == other.RoleID;
        }

        public override int GetHashCode()
        {
            return (int)RoleID;
        }
    }
}