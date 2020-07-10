using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common.SettingsManagement
{
    public class SettingsProvider
    {
        List<string> m_Categories;
        Dictionary<string, List<PrefEntry>> m_Settings;
        
        struct PrefEntry
        {
            public GUIContent content { get; }

            public IUserSetting pref { get; }

            public PrefEntry(GUIContent content, IUserSetting pref)
            {
                this.content = content;
                this.pref = pref;
            }
        }
        
        void SearchForUserSettingAttributes()
        {
            var m_Assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            var keywordsHash = new HashSet<string>();

            if (m_Settings != null)
                m_Settings.Clear();
            else
                m_Settings = new Dictionary<string, List<PrefEntry>>();

            ////if (m_SettingBlocks != null)
            ////    m_SettingBlocks.Clear();
            ////else
            ////    m_SettingBlocks = new Dictionary<string, List<MethodInfo>>();

            var types = m_Assemblies.SelectMany(x => x.GetTypes());

            // collect instance fields/methods too, but only so we can throw a warning that they're invalid.
            var fields = types.SelectMany(x =>
                    x.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Where(prop => Attribute.IsDefined(prop, typeof(SettingAttribute))));

            ////var methods = types.SelectMany(x => x.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ////        .Where(y => Attribute.IsDefined(y, typeof(UserSettingBlockAttribute))));

            foreach (var field in fields)
            {
                if (!field.IsStatic)
                {
                    Debug.LogWarning("Cannot create setting entries for instance fields. Skipping \"" + field.Name + "\".");
                    continue;
                }

                var attrib = (SettingAttribute)Attribute.GetCustomAttribute(field, typeof(SettingAttribute));

                ////if (!attrib.visibleInSettingsProvider)
                ////    continue;

                var pref = (IUserSetting)field.GetValue(null);

                ////if (pref == null)
                ////{
                ////    Debug.LogWarning("[UserSettingAttribute] is only valid for types implementing the IUserSetting interface. Skipping \"" + field.Name + "\"");
                ////    continue;
                ////}

                var category = string.IsNullOrEmpty(attrib.Category) ? "Uncategorized" : attrib.Category;
                //var content = listByKey ? new GUIContent(pref.key) : attrib.Title;
                var content = attrib.Title;

                //if (developerModeCategory.Equals(category) && !isDeveloperMode)
                //    continue;

                List<PrefEntry> settings;

                if (m_Settings.TryGetValue(category, out settings))
                    settings.Add(new PrefEntry(content, pref));
                else
                    m_Settings.Add(category, new List<PrefEntry>() { new PrefEntry(content, pref) });
            }

            //foreach (var method in methods)
            //{
            //    var attrib = (UserSettingBlockAttribute)Attribute.GetCustomAttribute(method, typeof(UserSettingBlockAttribute));
            //    var category = string.IsNullOrEmpty(attrib.category) ? "Uncategorized" : attrib.category;

            //    if (developerModeCategory.Equals(category) && !isDeveloperMode)
            //        continue;

            //    List<MethodInfo> blocks;

            //    var parameters = method.GetParameters();

            //    if (!method.IsStatic || parameters.Length < 1 || parameters[0].ParameterType != typeof(string))
            //    {
            //        Debug.LogWarning("[UserSettingBlockAttribute] is only valid for static functions with a single string parameter. Ex, `static void MySettings(string searchContext)`. Skipping \"" + method.Name + "\"");
            //        continue;
            //    }

            //    if (m_SettingBlocks.TryGetValue(category, out blocks))
            //        blocks.Add(method);
            //    else
            //        m_SettingBlocks.Add(category, new List<MethodInfo>() { method });
            //}

            //if (showHiddenSettings)
            //{
            //    var unlisted = new List<PrefEntry>();
            //    m_Settings.Add("Unlisted", unlisted);
            //    foreach (var pref in UserSettings.FindUserSettings(m_Assemblies, SettingVisibility.Unlisted | SettingVisibility.Hidden))
            //        unlisted.Add(new PrefEntry(new GUIContent(pref.key), pref));
            //}

            //if (showUnregisteredSettings)
            //{
            //    var unregistered = new List<PrefEntry>();
            //    m_Settings.Add("Unregistered", unregistered);
            //    foreach (var pref in UserSettings.FindUserSettings(m_Assemblies, SettingVisibility.Unregistered))
            //        unregistered.Add(new PrefEntry(new GUIContent(pref.key), pref));
            //}

            //foreach (var cat in m_Settings)
            //{
            //    foreach (var entry in cat.Value)
            //    {
            //        var content = entry.content;

            //        if (content != null && !string.IsNullOrEmpty(content.text))
            //        {
            //            foreach (var word in content.text.Split(' '))
            //                keywordsHash.Add(word);
            //        }
            //    }
            //}

            //keywords = keywordsHash;
            //m_Categories = m_Settings.Keys.Union(m_SettingBlocks.Keys).ToList();
            //m_Categories.Sort();
        }
    }
}
