using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
namespace Bottle.Editor.GridSystem
{
    [CustomEditor(typeof(GridTileBrushEditor))]
    public class GridTileEditor : GridBrushEditorBase
    {
        private float _brushButtonSize = 100;
        private Vector2 _scrollPosition = new Vector2();
        private GridTileBrushDatabaseList _gridTileDatabaseList;
        public int selectedCollectionIndex = 0;

        public override void OnPaintInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.TextField("New Database Name: ");
                EditorGUILayout.LabelField("Active GridTile Database:");
                //selectedCollectionIndex = EditorGUILayout.Popup(selectedCollectionIndex, _gridTileDatabaseList.GetNameList());
                if (GUILayout.Button("+"))
                {
                    CreateNewDatabase();
                }
                if (GUILayout.Button(new GUIContent("Refresh", "Refresh all databases to get the updated content."),
                                     GUILayout.MaxWidth(100)))
                {

                }
            }
            EditorGUILayout.EndHorizontal();
            _brushButtonSize = EditorGUILayout.Slider(new GUIContent("Button Size: "), _brushButtonSize, 50, 150);
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(new GUIContent("Add GridTile", "Add a GridTile to the collection.")))
            {

            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(new GUIContent("Remove Selected Tile", "Removes the selected gridtile from the collection.")) &&
                                 RemoveSelectedBrushesDialog("ahihi"))
            {

            }
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = Color.grey;
        }
        private bool RemoveSelectedBrushesDialog(string brushName)
        {
            return EditorUtility.DisplayDialog(
                "Remove selected GridTile?",
                "Are you sure you want to remove selected GridTile (" + brushName + ") from this collection?",
                "Remove",
                "Cancel");
        }

        public static void CreateNewDatabase()
        {
            CreateScriptableObject<GridTileBrushDatabase>("GridTileBrushDatabase", "Assets/Resources/GridObjectDatabase/");
        }
        public static void CreateScriptableObject<T>(string assetName, string assetPath) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, assetPath + assetName + ".asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}

