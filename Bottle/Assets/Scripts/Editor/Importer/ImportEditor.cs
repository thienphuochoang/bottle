using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Bottle.Extensions.Helper;
using Sirenix.Utilities;

public class ImportEditor : EditorWindow
{
    private VisualTreeAsset _visualTree;
    private const string _VISUAL_TREE_PATH = "Assets/Scripts/Editor/Importer/ImporterUXMLFile.uxml";
    private const string _USS_FILE_PATH = "Assets/Scripts/Editor/Importer/ImporterUSSFile.uss";
    private const string _PATH_SETTINGS = "Assets/Scripts/Editor/Importer/ImporterPathSettings.json";
    private Dictionary<string, string> _textFieldDatas;
    private const string _MODEL_IMPORT_PATH = "C:/Temp_Exporter/temp_unity";
    
    [MenuItem("Bottle/Importer Editor")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        ImportEditor window = (ImportEditor)GetWindow(typeof(ImportEditor));
        window.titleContent = new GUIContent("Importer Tool");
        window.minSize = new Vector2(400, 200);
    }

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

    private void OnEnable()
    {
        _textFieldDatas = new Dictionary<string, string>();
        _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_VISUAL_TREE_PATH);
        StyleSheet loadedStyleSheet =
            AssetDatabase.LoadAssetAtPath<StyleSheet>(_USS_FILE_PATH);
        rootVisualElement.styleSheets.Add(loadedStyleSheet);
        ImportModel();
    }

    private void CreateGUI()
    {
        rootVisualElement.Clear();
        _visualTree.CloneTree(rootVisualElement);
        UQueryBuilder<VisualElement> allTextFields =
            rootVisualElement.Query(classes: new string[] { "generalTextField" });
        
        allTextFields.ForEach(AddCallBack);
        var _textFieldsList = allTextFields.ToList();
        
        // Load current settings path from Json File
        foreach (var eachTextField in _textFieldsList)
        {
            var path = GetPathSettingFromJsonFile(eachTextField.name);
            var newTextFieldValue = eachTextField as TextField;
            newTextFieldValue.value = path;
        }
        
        UQueryBuilder<VisualElement> saveSettingButtonElement =
            rootVisualElement.Query(name: "Save_New_Settings");
        var saveSettingButton = (Button)saveSettingButtonElement;
        saveSettingButton.clickable.clicked += SetPathSettingsToJsonFile;
        
        UQueryBuilder<VisualElement> importModelButtonElement =
            rootVisualElement.Query(name: "Import_FBX");
        var importModelButton = (Button)importModelButtonElement;
        importModelButton.clickable.clicked += ImportModel;
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
