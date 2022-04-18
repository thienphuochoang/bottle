using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
using Bottle.Extensions.Helper;
using Bottle.Core.GridObjectData;
namespace Bottle.Editor.GridSystem
{
    [CustomEditor(typeof(GridObjectBrushEditor))]
    public class GridObjectEditor : GridBrushEditorBase
    {
        private float _brushButtonSize = 100;
        private Vector2 _scrollPosition = new Vector2();
        public GridObjectBrushDatabase database;
        private GridObjectBrushDatabaseList _gridObjectDatabaseList;
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
                if (_gridObjectDatabaseList.brushDatabases.Count > 0)
                {
                    selectedDatabaseIndex = EditorGUILayout.Popup(selectedDatabaseIndex, _gridObjectDatabaseList.GetNameList());
                    database = _gridObjectDatabaseList.brushDatabases[selectedDatabaseIndex];
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
                "Remove selected Grid Object?",
                "Are you sure you want to remove selected Grid Object (" + brushName + ") from this collection?",
                "Remove",
                "Cancel");
        }

        private static void CreateNewDatabase(string newDatabaseName)
        {
            ScriptableObjectHelper.CreateScriptableObject<GridObjectBrushDatabase>(newDatabaseName, "Assets/Resources/GridObjectDatabase/");
        }

        private void ReloadDatabases()
        {
            _gridObjectDatabaseList = GridObjectBrushDatabase.GetAllGridObjectDatabaseGUIDs();
        }

        private void BrushButtonUI()
        {
            int rowLength = 1;
            int maxRowLength = Mathf.FloorToInt((Screen.width - 15) / _brushButtonSize);
            int columns = Mathf.CeilToInt((database.GridBrushDatas.Count / maxRowLength)) * 3;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);

            if (maxRowLength < 1)
                maxRowLength = 1;

            foreach (var brushData in database.GridBrushDatas)
            {
                if (rowLength > maxRowLength)
                {
                    rowLength = 1;
                    EditorGUILayout.EndHorizontal();
                }
                if (rowLength == 1)
                    EditorGUILayout.BeginHorizontal();

                if (database.SelectedGridBrush != null &&
                    database.SelectedGridBrush.gridTile != null &&
                    database.SelectedGridBrush.gridTile == brushData.gridTile)
                {
                    GUI.backgroundColor = Color.green;
                }
                GUIContent btnContent = new GUIContent(AssetPreview.GetAssetPreview(brushData.gridTile.gameObject), brushData.gridTile.gameObject.name);
                if (GUILayout.Button(btnContent, GUILayout.Width(_brushButtonSize), GUILayout.Height(_brushButtonSize)))
                {
                    
                    // Select the currently edited brush and deselect all selected brushes
                    if (database.SelectedGridBrush != brushData)
                    {
                        database.selectedGridBrushIndex = database.GridBrushDatas.IndexOf(brushData);
                        //TargetBrush.SetTileFromEditor(Vector3Int.zero,
                        //                              brushData.gridTile,
                        //                              brushData.height,
                        //                              Quaternion.Euler(brushData.rotation));
                    }
                    else
                    {
                        database.selectedGridBrushIndex = -1;
                        //TargetBrush.ClearTileFromEditor();
                    }
                }
                GUI.backgroundColor = Color.grey;
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
                //if (database.Sele)
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
    }
}

