using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
using Bottle.Extensions.Helper;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;

namespace Bottle.Editor.GridSystem
{
    [CustomEditor(typeof(GridObjectBrushEditor))]
    public class GridObjectEditor : GridBrushEditorBase
    {
        private float _brushButtonSize = 100;
        private Vector2 _scrollPosition = new Vector2();
        public GridObjectBrushDatabase currentBrushDatabase;
        private GridObjectBrushDatabaseList _gridObjectDatabaseList;
        public int selectedDatabaseIndex = 0;
        private string _newDatabaseName;
        private BrushCell _previewCell;
        public GridObjectBrushEditor TargetBrush { get { return target as GridObjectBrushEditor; } }

        private void OnEnable()
        {
            ReloadDatabases();
            _previewCell = new BrushCell();
        }
        
        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
            if (TargetBrush == false) return;
            if (tool != GridBrushBase.Tool.Erase)
            {
                Vector3Int min = position.min - TargetBrush.pivot;
                Vector3Int max = min + TargetBrush.size;
                BoundsInt bounds = new BoundsInt(min, max - min);

                if (brushTarget != null && gridLayout != null)
                {
                    Vector2Int gridPosition = new Vector2Int(position.x, position.y);
                    float gridHeight = position.z;
                    foreach (Vector3Int location in bounds.allPositionsWithin)
                    {
                        Vector3Int brushPosition = location - min;
                        Debug.Log(TargetBrush.cells[TargetBrush.GetCellIndex(brushPosition)]);
                        BrushCell cell = TargetBrush.cells[TargetBrush.GetCellIndex(brushPosition)];
                        {
                            _previewCell.Tile = cell.Tile;
                            
                        }
                        /*
                        if (cell.Tile)
                        {
                            if (_previewCell.Tile == null)
                            {
                                var previewGridTile = GridManager.Instance.CreateGridObject<GridTile>(cell.Tile,
                                    gridPosition, position.z, cell.Scale, cell.Rotation);
                                //GridManager.Instance.CreatePreviewGridObject(previewGridTile.transform);
                                GridManager.Instance.CreatePreviewGridObject(previewGridTile.transform);
                                _previewCell.Tile = previewGridTile;
                            }
                            else
                            {
                                _previewCell.Tile.gridPosition = gridPosition;
                                _previewCell.Tile.gridHeight = gridHeight;
                            }
                        }
                        else if (cell.Entity)
                        {
                            
                        }
                        */
                    }
                }
            }
            
        }
        public override void OnPaintInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("New Grid Object Database Name: ");
                _newDatabaseName = EditorGUILayout.TextField("", _newDatabaseName);
                if (GUILayout.Button(new GUIContent("+", "Add new grid object database"),
                                     GUILayout.MaxWidth(100)))
                {
                    CreateNewDatabase(_newDatabaseName);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Active Grid Object Database:");
                if (_gridObjectDatabaseList.brushDatabases.Count > 0)
                {
                    selectedDatabaseIndex = EditorGUILayout.Popup(selectedDatabaseIndex, _gridObjectDatabaseList.GetNameList());
                    currentBrushDatabase = _gridObjectDatabaseList.brushDatabases[selectedDatabaseIndex];
                    TargetBrush.ClearBrushCellData();
                    var tileBrush = currentBrushDatabase.SelectedGridBrush;
                    if (tileBrush != null && tileBrush.gridTile != null)
                    {
                        TargetBrush.SetBrushCellData(
                            tileBrush.gridTile,
                            tileBrush.scale,
                            Quaternion.Euler(tileBrush.rotation)
                        );
                    }
                    else if (tileBrush != null && tileBrush.gridEntity != null)
                    {
                        TargetBrush.SetBrushCellData(
                            tileBrush.gridEntity,
                            tileBrush.scale,
                            Quaternion.Euler(tileBrush.rotation)
                        );
                    }
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
                AddGridObjectBrushPopup.Initialize(currentBrushDatabase.GridBrushDatas);
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(new GUIContent("Remove Selected Grid Object", "Removes the selected grid object from the database.")) &&
                                 RemoveSelectedBrushesDialog(currentBrushDatabase.SelectedGridBrush))
            {
                if (currentBrushDatabase.SelectedGridBrush != null)
                {
                    currentBrushDatabase.GridBrushDatas.RemoveAt(currentBrushDatabase.selectedGridBrushIndex);
                }
            }
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = Color.grey;
        }
        private bool RemoveSelectedBrushesDialog(GridObjectBrushData currentSelectingBrush)
        {
            if (currentSelectingBrush.gridTile != null)
            {
                return EditorUtility.DisplayDialog(
                    "Remove selected Grid Object?",
                    "Are you sure you want to remove selected Grid Object (" + currentSelectingBrush.gridTile.name + ") from this collection?",
                    "Remove",
                    "Cancel");
            }
            else if (currentSelectingBrush.gridEntity != null)
            {
                return EditorUtility.DisplayDialog(
                    "Remove selected Grid Object?",
                    "Are you sure you want to remove selected Grid Object (" + currentSelectingBrush.gridEntity.name + ") from this collection?",
                    "Remove",
                    "Cancel");
            }
            return false;
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
            int columns = Mathf.CeilToInt((currentBrushDatabase.GridBrushDatas.Count / maxRowLength)) * 3;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);

            if (maxRowLength < 1)
                maxRowLength = 1;

            foreach (var brushData in currentBrushDatabase.GridBrushDatas)
            {
                if (rowLength > maxRowLength)
                {
                    rowLength = 1;
                    EditorGUILayout.EndHorizontal();
                }
                if (rowLength == 1)
                    EditorGUILayout.BeginHorizontal();

                if (currentBrushDatabase.SelectedGridBrush != null &&
                    currentBrushDatabase.SelectedGridBrush.gridTile != null &&
                    currentBrushDatabase.SelectedGridBrush.gridTile == brushData.gridTile)
                {
                    GUI.backgroundColor = Color.green;
                }

                if (currentBrushDatabase.SelectedGridBrush != null &&
                    currentBrushDatabase.SelectedGridBrush.gridEntity != null &&
                    currentBrushDatabase.SelectedGridBrush.gridEntity == brushData.gridEntity)
                {
                    GUI.backgroundColor = Color.green;
                }
                GUIContent btnContent = new GUIContent("NULL");
                try
                {
                    btnContent = new GUIContent(AssetPreview.GetAssetPreview(brushData.gridTile.gameObject), brushData.gridTile.gameObject.name + "\n" + "Scale: " + brushData.scale + "\n" + "Rotation: " + brushData.rotation);
                }
                catch
                {
                    btnContent = new GUIContent(AssetPreview.GetAssetPreview(brushData.gridEntity.gameObject), brushData.gridEntity.gameObject.name + "\n" + "Scale: " + brushData.scale + "\n" + "Rotation: " + brushData.rotation);
                }
                if (GUILayout.Button(btnContent, GUILayout.Width(_brushButtonSize), GUILayout.Height(_brushButtonSize)))
                {
                    
                    if (currentBrushDatabase.SelectedGridBrush != brushData)
                    {
                        currentBrushDatabase.selectedGridBrushIndex = currentBrushDatabase.GridBrushDatas.IndexOf(brushData);
                    }
                    else
                    {
                        currentBrushDatabase.selectedGridBrushIndex = -1;
                        TargetBrush.ClearBrushCellData();
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

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
    }
}

