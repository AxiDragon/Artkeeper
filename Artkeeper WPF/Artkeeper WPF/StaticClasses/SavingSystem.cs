using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Artkeeper.StaticClasses
{
    public static class SavingSystem
    {
        public static Dictionary<string, object> SaveData = new Dictionary<string, object>();
        public static readonly string SaveDataPath = AppDomain.CurrentDomain.BaseDirectory + "data.txt";

        public static void Initialize()
        {
            SaveData = Load();

            Application.Current.Exit += OnApplicationExit;
        }

        private static void OnApplicationExit(object sender, ExitEventArgs e)
        {
            //Save();
            Debug.WriteLine("Saved data!");
        }

        public static void Save()
        {
            string json = JsonSerializer.Serialize(SaveData, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(SaveDataPath, json);
        }

        public static void SaveNewData(string key, object value)
        {
            AddNewData(key, value);

            Save();
        }

        public static void AddNewData(string key, object value)
        {
            SaveData[key] = value;
        }

        public static Dictionary<string, object> Load()
        {
            if (File.Exists(SaveDataPath))
            {
                try
                {
                    string json = File.ReadAllText(SaveDataPath);

                    SaveData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                    foreach (KeyValuePair<string, object> pair in SaveData)
                    {
                        //this is mainly to throw an exception if the value is null
                        Debug.WriteLine($"Loaded {pair.Key} with value {pair.Value}");
                    }

                    return SaveData;
                }
                catch
                {
                    MessageBox.Show("The save was corrupted, creating a new save.\nDid you mess with the save file?");
                    return new();
                }
            }
            else
            {
                return new();
            }
        }

        public static void ClearSaveData()
        {
            SaveData = new Dictionary<string, object>();
            File.Delete(SaveDataPath);
        }
    }
}
