using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.SelfAssign
{
    public class TimeZoneRole : RoleObject
    {
        public TimeZoneInfo TimeZone { get; set; }

        public override int CompareTo(RoleObject other)
        {

            if (other is null) return 1;

            if (!(other is TimeZoneRole))
            {
                if (other.Position == Position) return string.Compare(Name, other.Name);
                else return Position.CompareTo(other.Position);
            }
            else
            {
                return TimeZone.BaseUtcOffset.Hours.CompareTo((other as TimeZoneRole).TimeZone.BaseUtcOffset.Hours);
            }
        }
    }
}
