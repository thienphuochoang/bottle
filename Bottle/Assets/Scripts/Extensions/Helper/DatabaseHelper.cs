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
        public static Dictionary<int, GridObjectSaveData> GetDatabase(string inputFileName)
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

        public static void UpdateDatabase(Dictionary<string, string> dataAndNewValueDict, string inputFileName)
        {
            // Read file to string
            string json = string.Empty;
            using (StreamReader strReader = new StreamReader(inputFileName))
            {
                json = strReader.ReadToEnd();
            }
            JObject jObject = JsonConvert.DeserializeObject(json) as JObject;
            foreach (KeyValuePair<string, string> dataAndValue in dataAndNewValueDict)
            {
                JToken jToken = jObject.SelectToken(dataAndValue.Key);
                jToken.Replace(dataAndValue.Value);
            }
            string output = JsonConvert.SerializeObject(jObject, Formatting.Indented);
            using (StreamWriter strWriter = new StreamWriter(inputFileName))
            {
                strWriter.WriteLine(output);
            }
        }
        public static void AddNewDatabase<F, T>(string inputFileName, Dictionary<F, T> inputNewDatabase)
        {
            //var newDatabase = JsonConvert.SerializeObject(inputNewDatabase, Formatting.Indented);
            string newDatabase = JsonConvert.SerializeObject(inputNewDatabase, Formatting.Indented, new JsonSerializerSettings
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

