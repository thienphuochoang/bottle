using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bottle.Core.GridObjectData;
namespace Bottle.Editor.GridSystem
{
    public class AddGridObjectBrushPopup : EditorWindow
    {
        public List<GridTileBrushData> brushes;
        public static AddGridObjectBrushPopup Instance;
        private GridTileBrushData _newBrush = new GridTileBrushData();

        public static void Initialize(List<GridTileBrushData> brushes)
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
            _newBrush.gridTile = (GridTile)EditorGUILayout.ObjectField("GridTile Prefab", _newBrush.gridTile, typeof(GridObject), false);
            _newBrush.scale = EditorGUILayout.FloatField("Scale", _newBrush.scale);
            _newBrush.rotation = EditorGUILayout.Vector3Field("Rotation", _newBrush.rotation);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Add"))
            {
                if (_newBrush != null && _newBrush.gridTile != null && PrefabUtility.IsPartOfAnyPrefab(_newBrush.gridTile.gameObject))
                {
                    var newGridTileBrushData = new GridTileBrushData(_newBrush);
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
                this.Close();
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Cancel"))
                this.Close();
            EditorGUILayout.EndHorizontal();
        }
    }
}

