using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Common.SettingsManagement
{
    public class SettingsTest
    {
        [Setting("TestCategory", "AwesomeIntSetting", "TestTooltip")]
        public int AwesomeIntSetting { get; set; }

        [Setting("TestCategory", "AwesomeStringSetting", "TestTooltip")]
        public string AwesomeStringSetting { get; set; }
    }
}
