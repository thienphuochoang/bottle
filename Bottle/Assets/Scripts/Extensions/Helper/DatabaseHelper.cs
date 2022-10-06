using System.IO;
using System.Collections.Generic;

using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Bottle.Core.GridObjectData;
namespace Bottle.Extensions.Helper
{
    public class DatabaseHelper
    {
        public static string sceneStateDatabaseFileName = Application.dataPath + "/Resources" + "/" + "SceneState" + "/" + "SceneStateDatabase.json";

        // Methods 
        // ----------------------------------------------------------------------------------------------------------------------
        
        /**
        * Get all data in a json file.
        */
        public static JObject GetDatabase(string jsonDatabaseFilePath)
        {
            string json = string.Empty;
            using (StreamReader strReader = new StreamReader(jsonDatabaseFilePath))
            {
                json = strReader.ReadToEnd();
            }
            JObject jObject = JsonConvert.DeserializeObject(json) as JObject;
            return jObject;
        }

        public static Dictionary<int, GridObjectSaveData> GetLevelDatabase(string inputFileName)
        {
            StreamReader strReader = new StreamReader(inputFileName);
            string json = strReader.ReadToEnd();
            strReader.Close();
            Dictionary<int, GridObjectSaveData> database = JsonConvert.DeserializeObject<Dictionary<int, GridObjectSaveData>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            //Dictionary<int, GridObjectSaveData> database = JsonConvert.DeserializeObject<Dictionary<int, GridObjectSaveData>>(json, settings);
            //{
            //    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
            //    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            //});
            return database;
        }

        /*
         * Update the value of specific dictionary and save to an existing json database file
         */
        public static void UpdateDatabase<F, T>(Dictionary<F, T> dataAndNewValueDict, string inputFileName)
        {
            // Get database first
            JObject jObject = GetDatabase(inputFileName);
            foreach (KeyValuePair<F, T> dataAndValue in dataAndNewValueDict)
            {
                JToken jToken = jObject.SelectToken(dataAndValue.Key as string);
                jToken.Replace(dataAndValue.Value as string);
            }
            AddNewDatabase(inputFileName, jObject);
        }
        public static void AddNewDatabase<F, T>(string inputFileName, Dictionary<F, T> inputNewDatabase)
        {
            string newDatabase = JsonConvert.SerializeObject(inputNewDatabase, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            using (StreamWriter strWriter = new StreamWriter(inputFileName))
            {
                strWriter.WriteLine(newDatabase);
            }
        }
        public static void AddNewDatabase(string inputFileName, JObject jObject)
        {
            string newDatabase = JsonConvert.SerializeObject(jObject, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            using (StreamWriter strWriter = new StreamWriter(inputFileName))
            {
                strWriter.WriteLine(newDatabase);
            }
        }
        public static void AppendDatabase(string inputFileName, Dictionary<int, GridObjectSaveData> inputNewDatabase)
        {
            string json = string.Empty;
            using (StreamReader strReader = new StreamReader(inputFileName))
            {
                json = strReader.ReadToEnd();
            }
            Dictionary<int, GridObjectSaveData> currentDatabase = JsonConvert.DeserializeObject<Dictionary<int, GridObjectSaveData>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            foreach (KeyValuePair<int, GridObjectSaveData> newData in inputNewDatabase)
            {
                currentDatabase.Add(newData.Key, newData.Value);
            }
            string newDatabase = JsonConvert.SerializeObject(currentDatabase, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            using (StreamWriter strWriter = new StreamWriter(inputFileName))
            {
                strWriter.WriteLine(newDatabase);
            }
        }
    }
}

