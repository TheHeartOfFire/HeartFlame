using HeartFlame.ChatLevels;
using HeartFlame.Configuration;
using HeartFlame.Misc;
using HeartFlame.Permissions;
using HeartFlame.SelfAssign;
using System.Collections.Generic;

namespace HeartFlame.GuildControl
{
    public class GuildData
    {
        public ulong GuildID { get; }
        public ModuleControlData ModuleControl { get; set; }
        public PermissionsDataType Permissions { get; set; }
        public AllRoles SelfAssign { get; set; }
        public GuildConfiguration Configuration { get; set; }
        public List<ChatDataType> Chat { get; set; }

        public GuildData(ulong guildID)
        {
            GuildID = guildID;
            ModuleControl = new ModuleControlData();
            Permissions = new PermissionsDataType();
            SelfAssign = new AllRoles();
            Configuration = new GuildConfiguration();
            Chat = new List<ChatDataType>();
        }
    }
}
