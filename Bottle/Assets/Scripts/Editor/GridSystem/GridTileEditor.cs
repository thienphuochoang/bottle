using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
using Bottle.Extensions.Helper;
namespace Bottle.Editor.GridSystem
{
    [CustomEditor(typeof(GridTileBrushEditor))]
    public class GridTileEditor : GridBrushEditorBase
    {
        private float _brushButtonSize = 100;
        private Vector2 _scrollPosition = new Vector2();
        public GridTileBrushDatabase database;
        private GridTileBrushDatabaseList _gridTileDatabaseList;
        public int selectedDatabaseIndex = 0;
        private string newDatabaseName;

        private void OnEnable()
        {
            ReloadDatabases();
        }

        public override void OnPaintInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("New Grid Object Database Name: ");
                newDatabaseName = EditorGUILayout.TextField("", newDatabaseName);
                if (GUILayout.Button(new GUIContent("+", "Add new grid object database"),
                                     GUILayout.MaxWidth(100)))
                {
                    CreateNewDatabase(newDatabaseName);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Active Grid Object Database:");
                if (_gridTileDatabaseList.brushDatabases.Count > 0)
                {
                    selectedDatabaseIndex = EditorGUILayout.Popup(selectedDatabaseIndex, _gridTileDatabaseList.GetNameList());
                    database = _gridTileDatabaseList.brushDatabases[selectedDatabaseIndex];
                }
                
                if (GUILayout.Button(new GUIContent("Refresh", "Refresh all databases to get the updated content."),
                                     GUILayout.MaxWidth(100)))
                {
                    ReloadDatabases();
                }
            }
            EditorGUILayout.EndHorizontal();
            _brushButtonSize = EditorGUILayout.Slider(new GUIContent("Button Size: "), _brushButtonSize, 50, 150);
            BrushButtonUI();
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(new GUIContent("Add Grid Object", "Add a Grid Object to the database.")))
            {

            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(new GUIContent("Remove Selected Grid Object", "Removes the selected grid object from the database.")) &&
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

        private static void CreateNewDatabase(string newDatabaseName)
        {
            ScriptableObjectHelper.CreateScriptableObject<GridTileBrushDatabase>(newDatabaseName, "Assets/Resources/GridObjectDatabase/");
        }

        private void ReloadDatabases()
        {
            _gridTileDatabaseList = GridTileBrushDatabase.GetAllGridTileDatabaseGUIDs();
        }
        private void BrushButtonUI()
        {
            int rowLength = 1;
            int maxRowLength = Mathf.FloorToInt((Screen.width - 15) / _brushButtonSize);
            int columns = Mathf.CeilToInt((database.GridTileDatas.Count / maxRowLength)) * 3;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);

            if (maxRowLength < 1)
                maxRowLength = 1;

            foreach (var brushData in database.GridTileDatas)
            {
                if (rowLength > maxRowLength)
                {
                    rowLength = 1;
                    EditorGUILayout.EndHorizontal();
                }
                if (rowLength == 1)
                    EditorGUILayout.BeginHorizontal();

                Color guiColor = GUI.backgroundColor;
                GUIContent btnContent = new GUIContent(AssetPreview.GetAssetPreview(brushData.gameObject), brushData.gameObject.name);
                if (GUILayout.Button(btnContent, GUILayout.Width(_brushButtonSize), GUILayout.Height(_brushButtonSize)))
                {

                }
                rowLength++;
            }
            // Check if row is longer than max row length
            if (rowLength > maxRowLength)
            {
                rowLength = 1;
                EditorGUILayout.EndHorizontal();
            }
            if (rowLength == 1)
                EditorGUILayout.BeginHorizontal();

            // Add button
            if (GUILayout.Button(new GUIContent("+", "Add a GridTile to the current database."),
                                 GUILayout.Width(_brushButtonSize),
                                 GUILayout.Height(_brushButtonSize)))
            {

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
    }
}

