using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public Dictionary<string, List<SettingEntry>> m_Settings;

        public class SettingEntry
        {
            public GUIContent content { get; }

            private Wrapper wrapper;
            private string settingKey;

            public bool Secret { get; set; }

            public SettingEntry(SettingAttribute attribute, Wrapper wrapper)
            {
                this.content = attribute.Title;
                this.wrapper = wrapper;
                this.Secret = attribute.Secret;
                settingKey = $"Setting:{wrapper.FullName}"; // TODO: ability to supply setting name

                // Load and set value from playerprefs, should probably replace this with a "storefactory" would allow for server specific persistance as well

                var value = LoadSettingFromStore(wrapper.ValueType);

                wrapper.SetValue(value);
            }

            private object LoadSettingFromStore<T>(T type) where T : Type
            {
                if (!PlayerPrefs.HasKey(settingKey)) { return wrapper.defaultValue; }

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        return PlayerPrefs.GetInt(settingKey) == 1;
                    case TypeCode.Int32:
                        return PlayerPrefs.GetInt(settingKey);
                    case TypeCode.Single:
                        return PlayerPrefs.GetFloat(settingKey);
                    case TypeCode.String:
                        return PlayerPrefs.GetString(settingKey);
                }

                return wrapper.defaultValue;
            }

            private void PersistSettingToStore<T>(T value)
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Boolean:
                        PlayerPrefs.SetInt(settingKey, ((bool)(object)value) ? 1 : 0);
                        break;
                    case TypeCode.Int32:
                        PlayerPrefs.SetInt(settingKey, (int)(object)value);
                        break;
                    case TypeCode.Single:
                        PlayerPrefs.SetFloat(settingKey, (float)(object)value);
                        break;
                    case TypeCode.String:
                        PlayerPrefs.SetString(settingKey, (string)(object)value);
                        break;
                }
            }

            public Type ValueType { get => wrapper.ValueType;  }

            public object GetValue()
            {
                return wrapper.GetValue();
            }

            public void SetValue(bool value)
            {
                wrapper.SetValue(value);
                PersistSettingToStore(value);
            }

            public void SetValue(string o)
            {
                var type = wrapper.ValueType;
                var converter = TypeDescriptor.GetConverter(type);

                var value = converter.ConvertFromString(o);
                wrapper.SetValue(value);
                PersistSettingToStore(value);
            }
        }

        public void SearchForSettingsAttribute()
        {
            var m_Assemblies = System.AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("Assembly-CSharp"));

            var keywordsHash = new HashSet<string>();

            if (m_Settings != null)
                m_Settings.Clear();
            else
                m_Settings = new Dictionary<string, List<SettingEntry>>();

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
                ///
                // settingKey = $"Setting:{field.DeclaringType.FullName}.{field.Name}";
                var wrapper = new FieldWrapper(field);

                ////if (pref == null)
                ////{
                ////    Debug.LogWarning("[UserSettingAttribute] is only valid for types implementing the IUserSetting interface. Skipping \"" + field.Name + "\"");
                ////    continue;
                ////}

                var category = string.IsNullOrEmpty(attrib.Category) ? "Uncategorized" : attrib.Category;
                //var content = listByKey ? new GUIContent(pref.key) : attrib.Title;

                //if (developerModeCategory.Equals(category) && !isDeveloperMode)
                //    continue;

                List<SettingEntry> settings;

                // TODO: split categories on / to get a menu (tab) -> section list going.
                if (m_Settings.TryGetValue(category, out settings))
                    settings.Add(new SettingEntry(attrib, wrapper));
                else
                    m_Settings.Add(category, new List<SettingEntry>() { new SettingEntry(attrib, wrapper) });
            }

            var properties = types.SelectMany(x =>
                    x.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(prop => Attribute.IsDefined(prop, typeof(SettingAttribute)))).ToList();

            foreach (var property in properties)
            {
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

                List<SettingEntry> settings;

                // TODO: split categories on / to get a menu (tab) -> section list going.
                if (m_Settings.TryGetValue(category, out settings))
                    settings.Add(new SettingEntry(attrib, wrapper));
                else
                    m_Settings.Add(category, new List<SettingEntry>() { new SettingEntry(attrib, wrapper) });

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
    public abstract class Wrapper
    {

        internal object defaultValue;

        public abstract object GetValue();

        public abstract string FullName { get; }
        public abstract void SetValue(object value);

        public abstract Type ValueType { get; }

        public object GetDefault()
        {
            return defaultValue;
        }
    }

    public class FieldWrapper : Wrapper
    {
        private FieldInfo field;

        public FieldWrapper(FieldInfo field)
        {
            this.field = field;
            defaultValue = GetValue();
        }

        public override string FullName { get => $"{field.DeclaringType.FullName}.{field.Name}"; }
        public override Type ValueType { get => field.FieldType; }

        public override object GetValue()
        {
            // Get static value
            return field.GetValue(null);
        }

        public override void SetValue(object value)
        {
            // Set Static Value
            field.SetValue(null, value);
        }
    }
    public class PropertyWrapper : Wrapper
    {
        private PropertyInfo property;

        public override string FullName { get => $"{property.DeclaringType.FullName}.{property.Name}"; }
        public override Type ValueType { get => property.PropertyType; }
        public PropertyWrapper(PropertyInfo property)
        {
            this.property = property;
            defaultValue = GetValue();
        }

        public override object GetValue()
        {
            // Get static value
            return property.GetValue(null);
        }

        public override void SetValue(object value)
        {
            // Set Static Value
            property.SetValue(null, value);
        }
    }
}
