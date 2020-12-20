using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Misc
{
    public class ModuleControlData
    {
        public bool IncludePermissions { get; set; }
        public bool IncludeLogging { get; set; }
        public bool IncludeChat { get; set; }
        public bool IncludeSelfAssign { get; set; }
        public bool IncludeCompendium { get; set; }
    
        public ModuleControlData()
        {
            IncludeChat = true;
            IncludeLogging = true;
            IncludePermissions = true;
            IncludeSelfAssign = true;
            IncludeCompendium = true;
        }
    }
}
