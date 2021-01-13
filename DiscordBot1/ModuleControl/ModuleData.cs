using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HeartFlame.ModuleControl
{//level 2
    public class ModuleData
    {
        [DefaultValue(true)]
        public bool IncludePermissions { get; set; }
        [DefaultValue(true)]
        public bool IncludeLogging { get; set; }
        [DefaultValue(true)]
        public bool IncludeChat { get; set; }
        [DefaultValue(true)]
        public bool IncludeSelfAssign { get; set; }
        [DefaultValue(true)]
        public bool IncludeCompendium { get; set; }
        [DefaultValue(true)]
        public bool IncludeModeration { get; set; }
        [DefaultValue(true)]
        public bool IncludeServerLogging { get; set; }
    
    }
}
