using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common.SettingsManagement
{
    public class SettingsTest
    {
        [Setting("Test/TestCategory", "AwesomeIntSettingProperty", "TestTooltip")]
        public static int AwesomeIntSettingProperty { get; set; }

        [Setting("Test/TestCategory", "AwesomeStringSettingProperty", "TestTooltip")]
        public static string AwesomeStringSettingProperty { get; set; }

        [Setting("Test/TestCategory", "AwesomeIntSetting", "TestTooltip")]
        public static int AwesomeIntSetting { get; set; }

        [Setting("Test/TestCategory", "AwesomeStringSetting", "TestTooltip")]
        public static string AwesomeStringSetting { get; set; }
    }
}
