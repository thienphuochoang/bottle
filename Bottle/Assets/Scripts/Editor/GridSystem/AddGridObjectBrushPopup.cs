using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bottle.Core.GridObjectData;
namespace Bottle.Editor.GridSystem
{
    public class AddGridObjectBrushPopup : EditorWindow
    {
        public List<GridObjectBrushData> brushes;
        public static AddGridObjectBrushPopup Instance;
        private GridObjectBrushData _newBrush = new GridObjectBrushData();

        public static void Initialize(List<GridObjectBrushData> brushes)
        {
            if (Instance != null) return;

            Instance = (AddGridObjectBrushPopup)EditorWindow.GetWindowWithRect(typeof(AddGridObjectBrushPopup), new Rect(0, 0, 400, 180));
            GUIContent titleContent = new GUIContent("Add New Grid Object Brush");
            Instance.titleContent = titleContent;
            Instance.brushes = brushes;
            Instance.ShowUtility();
            Instance.Repaint();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            _newBrush.gridEntity = (GridEntity)EditorGUILayout.ObjectField("GridEntity Prefab", _newBrush.gridEntity, typeof(GridObject), false);
            _newBrush.gridTile = (GridTile)EditorGUILayout.ObjectField("GridTile Prefab", _newBrush.gridTile, typeof(GridObject), false);
            _newBrush.scale = EditorGUILayout.FloatField("Scale", _newBrush.scale);
            _newBrush.rotation = EditorGUILayout.Vector3Field("Rotation", _newBrush.rotation);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Add"))
            {
                if (_newBrush != null && _newBrush.gridTile != null && _newBrush.gridEntity == null && PrefabUtility.IsPartOfAnyPrefab(_newBrush.gridTile.gameObject))
                {
                    var newGridTileBrushData = new GridObjectBrushData(_newBrush);
                    bool isBrushDuplicated = false;
                    foreach (var brush in Instance.brushes)
                    {
                        if (brush.gridTile == newGridTileBrushData.gridTile)
                        {
                            isBrushDuplicated = true;
                        }
                    }
                    if (isBrushDuplicated == false)
                    {
                        Instance.brushes.Add(newGridTileBrushData);
                    }
                }
                else if (_newBrush != null && _newBrush.gridEntity != null && _newBrush.gridTile == null && PrefabUtility.IsPartOfAnyPrefab(_newBrush.gridEntity.gameObject))
                {
                    var newGridEntityBrushData = new GridObjectBrushData(_newBrush);
                    bool isBrushDuplicated = false;
                    foreach (var brush in Instance.brushes)
                    {
                        if (brush.gridEntity == newGridEntityBrushData.gridEntity)
                        {
                            isBrushDuplicated = true;
                        }
                    }
                    if (isBrushDuplicated == false)
                    {
                        Instance.brushes.Add(newGridEntityBrushData);
                    }
                }
                this.Close();
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Cancel"))
                this.Close();
            EditorGUILayout.EndHorizontal();
            GUIStyle tooltipLabelStyle = new GUIStyle();
            tooltipLabelStyle.normal.textColor = Color.red;
            tooltipLabelStyle.fontStyle = FontStyle.Bold;
            if (_newBrush.gridEntity != null && _newBrush.gridTile != null)
            {
                EditorGUILayout.LabelField("Please assign only one Grid Entity Prefab or one Grid Tile Prefab", tooltipLabelStyle);
            }
            else
            {
                EditorGUILayout.LabelField("", tooltipLabelStyle);
            }
        }
    }
}

