using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.SelfAssign
{
    public class TimeZoneCategory : RoleCategory
    {

        /// <summary>
        /// DO NOT USE. USE THE VERSION WITH TIMEZONEINFO INSTEAD.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Position"></param>
        /// <param name="RoleID"></param>
        /// <param name="Emoji"></param>
        public override void AddRole(string Name, int Position, ulong RoleID, string Emoji = null)
        { 
        }


        public void AddRole(string Name, int Position, ulong RoleID, TimeZoneInfo TimeZone, string Emoji = null)
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

            var Role = new TimeZoneRole
            {
                Name = Name,
                Emoji = Emoji,
                Position = Position,
                RoleID = RoleID,
                TimeZoneID = TimeZone.Id
            };

            if (Role.Emoji is null)
                Role.Emoji = GetEmoji(Role);

            Roles.Add(Role);


            Roles.Sort();
            PersistentData.SaveChangesToJson();
        }

        public void AddRole(TimeZoneRole Role)
        {
            if (Roles is null)
                Roles = new List<RoleObject>();

            if (Roles.Count > 0)
                foreach (var role in Roles)
                {
                    if (role.Position >= Role.Position)
                    {
                        role.Position++;
                        role.Emoji = GetEmoji(role);
                    }
                }


            if (Role.Emoji is null)
                Role.Emoji = GetEmoji(Role);

            Roles.Add(Role);
            Roles.Sort();
            PersistentData.SaveChangesToJson();
        }
        public void Sort()
        {
            Roles.Sort();
            foreach(var Role in Roles)
            {
                Role.Position = Roles.IndexOf(Role);
                Role.Emoji = EmoteRef.Emotes.GetValueOrDefault((Role.Position+1).ToString());
            }
            PersistentData.SaveChangesToJson();
        }

        public TimeZoneRole GetRole(TimeZoneInfo TZone)
        {
            foreach(var Role in Roles)
            {
                var ZoneRole = Role as TimeZoneRole;

                if (ZoneRole.TimeZoneID.Equals(TZone.Id))
                    return ZoneRole;
            }

            return null;
        }

        public TimeZoneRole GetRole(RoleObject role)
        {
            foreach (var Role in Roles)
            {
                var ZoneRole = Role as TimeZoneRole;
                if (Role.Equals(role))
                    return ZoneRole;
            }

            return null;
        }
    }
}
