using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Common
{
    public class SaveManager
    {
        /**
         * Saves the save data to the disk
         */
        public static void Save(string identifier, object obj)
        {
            var path = GetPath(identifier);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, obj);
            file.Close();
        }

        /**
         * Loads the save data from the disk
         */
        public static T Load<T>(string identifier) where T : class
        {
            var path = GetPath(identifier);

            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = null;
                try
                {
                    file = File.Open(path, FileMode.Open);
                    var obj = (T) bf.Deserialize(file);
                    file.Close();
                    return obj;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    file?.Close();
                    return null;
                }
            }
            else
            {
                Debug.LogError("saved data not found");
                return null;
            }
        }

        private static string GetPath(string identifier)
        {
            return $"{Application.persistentDataPath}/{identifier}.dat";
        }
    }
}