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
        public List<string> m_Categories;
        public Dictionary<string, List<PrefEntry>> m_Settings;
        
        public struct PrefEntry
        {
            public GUIContent content { get; }

            public Wrapper /*IUserSetting*/ wrapper { get; }

            public PrefEntry(GUIContent content, Wrapper /*IUserSetting*/ wrapper)
            {
                this.content = content;
                this.wrapper = wrapper;
            }
        }
        
        public void SearchForSettingsAttribute()
        {
            var m_Assemblies = System.AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("Assembly-CSharp"));

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
                    .Where(prop => Attribute.IsDefined(prop, typeof(SettingAttribute)))).ToList();


            ////var methods = types.SelectMany(x => x.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ////        .Where(y => Attribute.IsDefined(y, typeof(UserSettingBlockAttribute))));
            Debug.Log($"{fields.Count} fields found");
            foreach (var field in fields)
            {
                Debug.Log(field.Name);
                if (!field.IsStatic)
                {
                    Debug.LogWarning("Cannot create setting entries for instance fields. Skipping \"" + field.Name + "\".");
                    continue;
                }

                var attrib = (SettingAttribute)Attribute.GetCustomAttribute(field, typeof(SettingAttribute));

                ////if (!attrib.visibleInSettingsProvider)
                ////    continue;

                var wrapper = new FieldWrapper(field);

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

                // TODO: split categories on / to get a menu (tab) -> section list going.
                if (m_Settings.TryGetValue(category, out settings))
                    settings.Add(new PrefEntry(content, wrapper));
                else
                    m_Settings.Add(category, new List<PrefEntry>() { new PrefEntry(content, wrapper) });
            }

            var properties = types.SelectMany(x =>
                    x.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(prop => Attribute.IsDefined(prop, typeof(SettingAttribute)))).ToList();
            Debug.Log($"{properties.Count} properties found");
            foreach (var property in properties)
            {
                Debug.Log(property.Name);
                //if (!property.)
                //{
                //    Debug.LogWarning("Cannot create setting entries for instance fields. Skipping \"" + field.Name + "\".");
                //    continue;
                //}
                var attrib = (SettingAttribute)Attribute.GetCustomAttribute(property, typeof(SettingAttribute));

                var wrapper = new PropertyWrapper(property);

                var category = string.IsNullOrEmpty(attrib.Category) ? "Uncategorized" : attrib.Category;
                //var content = listByKey ? new GUIContent(pref.key) : attrib.Title;
                var content = attrib.Title;

                //if (developerModeCategory.Equals(category) && !isDeveloperMode)
                //    continue;

                List<PrefEntry> settings;

                // TODO: split categories on / to get a menu (tab) -> section list going.
                if (m_Settings.TryGetValue(category, out settings))
                    settings.Add(new PrefEntry(content, wrapper));
                else
                    m_Settings.Add(category, new List<PrefEntry>() { new PrefEntry(content, wrapper) });

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
    public abstract class Wrapper {

        private object defaultValue;
        public Wrapper()
        {
            defaultValue = GetValue();
        }

        public abstract object GetValue();
    }

    public class FieldWrapper : Wrapper
    {
        private FieldInfo field;

        public FieldWrapper(FieldInfo field)
        {
            this.field = field;
        }

        public override object GetValue()
        {
            return field.GetValue(null);
        }
    }
    public class PropertyWrapper : Wrapper
    {
        private PropertyInfo property;

        public PropertyWrapper(PropertyInfo property)
        {
            this.property = property;
        }

        public override object GetValue()
        {
            return property.GetValue(null);
        }
    }
}
