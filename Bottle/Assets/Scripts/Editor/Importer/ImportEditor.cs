using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
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

        private string[] jsonKeyValues = new string[]
        {
            "Models_Path", "Materials_Path", "Textures_Path", "Entity_Prefabs_Path", "Tile_Prefabs_Path"
        };
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
            
        }
        
        protected virtual void OnEnable()
        {
            base.OnEnable();
            Instance = this;
            
            List<string> allTextFields = new List<string>();
            allTextFields.Add(Models_Path);
            allTextFields.Add(Materials_Path);
            allTextFields.Add(Textures_Path);
            allTextFields.Add(Entity_Prefabs_Path);
            allTextFields.Add(Tile_Prefabs_Path);
            // Load current settings path from Json File
            foreach (var eachTextField in jsonKeyValues)
            {
                var path = GetPathSettingFromJsonFile(eachTextField);
                switch (eachTextField)
                {
                    case "Models_Path":
                        Models_Path = path;
                        break;
                    case "Materials_Path":
                        Materials_Path = path;
                        break;
                    case "Textures_Path":
                        Textures_Path = path;
                        break;
                    case "Entity_Prefabs_Path":
                        Entity_Prefabs_Path = path;
                        break;
                    case "Tile_Prefabs_Path":
                        Tile_Prefabs_Path = path;
                        break;
                }
            }
            
        }
        
        private VisualTreeAsset _visualTree;
        //private const string _VISUAL_TREE_PATH = "Assets/Scripts/Editor/Importer/ImporterUXMLFile.uxml";
        //private const string _USS_FILE_PATH = "Assets/Scripts/Editor/Importer/ImporterUSSFile.uss";
        private const string _PATH_SETTINGS = "Assets/Resources/ImporterDatabase/ImporterPathSettings.json";
        private Dictionary<string, string> _textFieldDatas;
        public static ImportEditor Instance { get; private set; }
        /*
        [MenuItem("Bottle/Importer Editor")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            ImportEditor window = (ImportEditor)GetWindow(typeof(ImportEditor));
            window.titleContent = new GUIContent("Importer Tool");
            window.minSize = new Vector2(400, 200);
        }
        */
        void OnDragUpdatedEvent(DragUpdatedEvent evt)
        {
            if (DragAndDrop.objectReferences.Length == 1)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                return;
            }
            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
        }

        void OnDragPerformed(DragPerformEvent evt, TextField inputTextField)
        {
            if (DragAndDrop.objectReferences.Length == 1)
            {
                inputTextField.value = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]);
                DragAndDrop.AcceptDrag();
            }
        }
        /*
        private void OnEnable()
        {
            Instance = this;
            _textFieldDatas = new Dictionary<string, string>();
            _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_VISUAL_TREE_PATH);
            StyleSheet loadedStyleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(_USS_FILE_PATH);
            rootVisualElement.styleSheets.Add(loadedStyleSheet);
            //ImportModel();
        }
        */
        protected virtual void OnGUI()
        {
            base.OnGUI();
            /*
            rootVisualElement.Clear();
            _visualTree.CloneTree(rootVisualElement);
            UQueryBuilder<VisualElement> allTextFields =
                rootVisualElement.Query(classes: new string[] { "generalTextField" });
            */
        }

        private void GetPathSettingFromTextField()
        {
            _textFieldDatas.Clear();
            UQueryBuilder<VisualElement> allTextFields =
                rootVisualElement.Query(classes: new string[] { "generalTextField" });
            
            var _textFieldsList = allTextFields.ToList();
            foreach (var eachTextField in _textFieldsList)
            {
                var data = eachTextField as TextField;
                _textFieldDatas.Add(data.name, data.value);
            }
        }
        private string GetPathSettingFromJsonFile(string jsonTextFieldKey)
        {
            JObject jObject = DatabaseHelper.GetDatabase(_PATH_SETTINGS);
            string jTokenString = jObject.SelectToken(jsonTextFieldKey).ToObject<string>();
            if (jTokenString != null)
            {
                return jTokenString;
            }
            return null;
        }

        private void GetAllKeysInJsonFile()
        {
            JObject jObject = DatabaseHelper.GetDatabase(_PATH_SETTINGS);
        }

        private void SetPathSettingsToJsonFile()
        {
            GetPathSettingFromTextField();
            DatabaseHelper.UpdateDatabase<string, string>(_textFieldDatas, _PATH_SETTINGS);
        }
        
        private void AddCallBack(VisualElement textField)
        {
            TextField currentTextField = textField as TextField;
            textField.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            textField.RegisterCallback<DragPerformEvent, TextField>(OnDragPerformed, currentTextField);
        }
        
        /*
        private void ImportModel()
        {
            var watcher = new FileSystemWatcher("Assets/Art/Isometric Pack 3d/Props/Models/Materials/Models");
            watcher.NotifyFilter = NotifyFilters.Attributes
                                   | NotifyFilters.CreationTime
                                   | NotifyFilters.DirectoryName
                                   | NotifyFilters.FileName
                                   | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.Security
                                   | NotifyFilters.Size;
            
            watcher.Changed += OnChanged;
            watcher.Filter = "*.fbx";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }
        */
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Debug.Log(e.ChangeType);
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Debug.Log($"Changed: {e.FullPath}");
        }
        
    }
}
