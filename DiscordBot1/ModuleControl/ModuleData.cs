using System;
using System.Collections.Generic;
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
    
        public ModuleData()
        {
            IncludeChat = true;
            IncludeLogging = true;
            IncludePermissions = true;
            IncludeSelfAssign = true;
            IncludeCompendium = true;
            IncludeModeration = true;
        }
    }
}
