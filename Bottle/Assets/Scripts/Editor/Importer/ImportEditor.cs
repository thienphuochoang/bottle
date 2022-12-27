using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bottle.Extensions.Helper;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
namespace Bottle.Editor.Importer
{
    public class ImportEditor : OdinEditorWindow
    {
        [MenuItem("Bottle/Importer Editor")]
        private static void Init()
        {
            GetWindow<ImportEditor>().Show();
        }
        //private const string _VISUAL_TREE_PATH = "Assets/Scripts/Editor/Importer/ImporterUXMLFile.uxml";
        //private const string _USS_FILE_PATH = "Assets/Scripts/Editor/Importer/ImporterUSSFile.uss";
        private const string _PATH_SETTINGS = "Assets/Resources/ImporterDatabase/ImporterPathSettings.json";
        private Dictionary<string, string> _textFieldDatas;
        public static ImportEditor Instance { get; private set; }
        [FolderPath(RequireExistingPath = true), BoxGroup("Importer Settings")]
        public string Models_Path;
        [FolderPath(RequireExistingPath = true), BoxGroup("Importer Settings")]
        public string Materials_Path;
        [FolderPath(RequireExistingPath = true), BoxGroup("Importer Settings")]
        public string Textures_Path;
        [FolderPath(RequireExistingPath = true), BoxGroup("Importer Settings")]
        public string Entity_Prefabs_Path;
        [FolderPath(RequireExistingPath = true), BoxGroup("Importer Settings")]
        public string Tile_Prefabs_Path;

        [Button("Import Model", ButtonSizes.Small)]
        private void ImportModel()
        {
            
        }

        [Button("Save New Settings", ButtonSizes.Small)]
        private void SaveNewSettings()
        {
            SetPathSettingsToJsonFile();
        }
        
        protected virtual void OnEnable()
        {
            base.OnEnable();
            Instance = this;
            _textFieldDatas = new Dictionary<string, string>();
            
            // Load current settings path from Json File
            JToken[] allTextFieldDatas = GetPathSettingFromJsonFile();
            foreach (var textFieldData in allTextFieldDatas)
            {
                var textFieldName = textFieldData.SelectToken("name").ToString();
                var textFieldPath = textFieldData.SelectToken("path").ToString();
                switch (textFieldName)
                {
                    case "Models_Path":
                    {
                        Models_Path = textFieldPath;
                        break;
                    }
                    case "Materials_Path":
                    {
                        Materials_Path = textFieldPath;
                        break;
                    }
                    case "Textures_Path":
                    {
                        Textures_Path = textFieldPath;
                        break;
                    }
                    case "Entity_Prefabs_Path":
                    {
                        Entity_Prefabs_Path = textFieldPath;
                        break;
                    }
                    case "Tile_Prefabs_Path":
                    {
                        Tile_Prefabs_Path = textFieldPath;
                        break;
                    }
                }
            }
        }

        
        

        private void GetPathSettingFromTextField()
        {
            _textFieldDatas.Clear();
            JToken[] allTextFieldDatas = GetPathSettingFromJsonFile();
            for (int i = 0; i < allTextFieldDatas.Length; i++)
            {
                var textFieldName = allTextFieldDatas[i].SelectToken("name").ToString();
                switch (textFieldName)
                {
                    case "Models_Path":
                    {
                        _textFieldDatas.Add("properties[" + i + "].path", Models_Path);
                        break;
                    }
                    case "Textures_Path":
                    {
                        _textFieldDatas.Add("properties[" + i + "].path", Textures_Path);
                        break;
                    }
                    case "Materials_Path":
                    {
                        _textFieldDatas.Add("properties[" + i + "].path", Materials_Path);
                        break;
                    }
                    case "Entity_Prefabs_Path":
                    {
                        _textFieldDatas.Add("properties[" + i + "].path", Entity_Prefabs_Path);
                        break;
                    }
                    case "Tile_Prefabs_Path":
                    {
                        _textFieldDatas.Add("properties[" + i + "].path", Tile_Prefabs_Path);
                        break;
                    }
                }
            }
        }
        private JToken[] GetPathSettingFromJsonFile()
        {
            JObject jObject = DatabaseHelper.GetDatabase(_PATH_SETTINGS);
            JToken[] jTokenString = jObject.SelectToken("properties").ToArray();
            return jTokenString;
        }

        private void SetPathSettingsToJsonFile()
        {
            GetPathSettingFromTextField();
            DatabaseHelper.UpdateDatabase<string, string>(_textFieldDatas, _PATH_SETTINGS);
        }
    }
}
