using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Najm
{
    class Configs : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue(".")]
        public string ColorMapsFolder
        {
            get { return (String)this["ColorMapsFolder"]; }
            set { this["ColorMapsFolder"] = value; }
        }
    }
}
