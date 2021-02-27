using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HeartFlame.ModuleControl
{//level 2
    public class ModuleData
    {
        public bool IncludePermissions { get; set; }
        public bool IncludeLogging { get; set; }
        public bool IncludeChat { get; set; }
        public bool IncludeSelfAssign { get; set; }
        public bool IncludeCompendium { get; set; }
        public bool IncludeModeration { get; set; }
        public bool IncludeServerLogging { get; set; }
        public bool IncludeTime { get; set; }
        public bool IncludeCustomCommands { get; set; }
        public bool IncludeJoinMessages { get; set; }
        public bool IncludePatchNotes { get; set; }

        public ModuleData()
        {
            IncludeChat = true;
            IncludeCompendium = true;
            IncludeLogging = true;
            IncludeModeration = true;
            IncludePermissions = true;
            IncludeSelfAssign = true;
            IncludeServerLogging = true;
            IncludeTime = true;
            IncludeCustomCommands = true;
            IncludeJoinMessages = true;
            IncludePatchNotes = false;
        }
    }
}
