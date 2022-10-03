using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Sirenix.Utilities;

public class ImportEditor : EditorWindow
{
    private VisualTreeAsset _visualTree;
    private const string _VISUAL_TREE_PATH = "Assets/Scripts/Editor/Importer/ImporterUXMLFile.uxml";
    private const string _USS_FILE_PATH = "Assets/Scripts/Editor/Importer/ImporterUSSFile.uss";
    private const string _PATH_SETTINGS = "Assets/Scripts/Editor/Importer/ImporterPathSettings.json";
    private Dictionary<string, string> _textFieldDatas;
    
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
        GetPathSettingFromTextField();
    }

    private void GetPathSettingFromTextField()
    {
        UQueryBuilder<VisualElement> allTextFields =
            rootVisualElement.Query(classes: new string[] { "generalTextField" });
        
        var _textFieldsList = allTextFields.ToList();
        _textFieldDatas.Clear();
        foreach (var eachTextField in _textFieldsList)
        {
            var data = eachTextField as TextField;
            _textFieldDatas.Add(data.name, data.value);
        }
    }
    private string GetPathSettingFromJsonFile(string jsonTextFieldKey)
    {
        StreamReader strReader = new StreamReader(_PATH_SETTINGS);
        string json = strReader.ReadToEnd();
        strReader.Close();
        JObject jObject = JsonConvert.DeserializeObject(json) as JObject;
        string jTokenString = jObject.SelectToken(jsonTextFieldKey).ToObject<string>();
        if (jTokenString != null)
        {
            return jTokenString;
        }
        return null;
    }

    private void SetPathSettingsToJsonFile()
    {
        string json = string.Empty;
        using (StreamReader strReader = new StreamReader(_PATH_SETTINGS))
        {
            json = strReader.ReadToEnd();
        }
        JObject jObject = JsonConvert.DeserializeObject(json) as JObject;
        foreach (KeyValuePair<string, string> dataAndValue in _textFieldDatas)
        {
            JToken jToken = jObject.SelectToken(dataAndValue.Key);
            jToken.Replace(dataAndValue.Value);
        }
        string output = JsonConvert.SerializeObject(jObject, Formatting.Indented);
        using (StreamWriter strWriter = new StreamWriter(_PATH_SETTINGS))
        {
            strWriter.WriteLine(output);
        }
    }
    
    private void AddCallBack(VisualElement textField)
    {
        TextField currentTextField = textField as TextField;
        textField.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
        textField.RegisterCallback<DragPerformEvent, TextField>(OnDragPerformed, currentTextField);
    }
}
