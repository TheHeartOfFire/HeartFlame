using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Logging
{
    public class PermissionComparisonResult
    {
        public GuildPermission Permission { get; set; }
        public bool Gained { get; set; }

        public PermissionComparisonResult(GuildPermission Perm, bool _gained)
        {
            Permission = Perm;
            Gained = _gained;
        }
    }
}
