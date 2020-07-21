using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common.SettingsManagement
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SettingAttribute : Attribute
    {
        public string Category { get; }
        public string Key { get; } // TODO: get based on reflection if not set explicitly

        public GUIContent Title { get; }

        public SettingAttribute(string category, string title, string tooltip = null)
        {
            Category = category;
            Title = new GUIContent(title, tooltip);
        }
    }

    // scoped settings attribute?
}
