//using System;
//using UnityEditor;
//using UnityEngine;

//namespace Assets.Scripts.Common.SettingsManagement
//{
//    [Serializable]
//    sealed class JsonValueWrapper<T>
//    {
//#if PRETTY_PRINT_JSON
//        const bool k_PrettyPrintJson = true;
//#else
//        const bool k_PrettyPrintJson = false;
//#endif

//        [SerializeField]
//        T m_Value;

//        public static string Serialize(T value)
//        {
//            var obj = new JsonValueWrapper<T>() { m_Value = value };
//            return EditorJsonUtility.ToJson(obj, k_PrettyPrintJson);
//        }

//        public static T Deserialize(string json)
//        {
//            var value = (object)Activator.CreateInstance<JsonValueWrapper<T>>();
//            EditorJsonUtility.FromJsonOverwrite(json, value);
//            return ((JsonValueWrapper<T>)value).m_Value;
//        }

//        public static T DeepCopy(T value)
//        {
//            if (typeof(ValueType).IsAssignableFrom(typeof(T)))
//                return value;
//            var str = Serialize(value);
//            return Deserialize(str);
//        }
//    }
//}
