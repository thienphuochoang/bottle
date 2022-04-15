using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Bottle.Editor.GridSystem
{
    public class AddGridTileBrushPopup : EditorWindow
    {
        //private GridTileObjectData _newBrush = new GridTileObjectData();
        private void OnGUI()
        {
            EditorGUILayout.Space();
            //EditorGUILayout.ObjectField("GridTile Prefab", _newBrush.gridTile, typeof(GridTile), false);
            //EditorGUILayout.FloatField("Tile Height", scale);
            //EditorGUILayout.Vector3Field("Rotation Offset", _newBrush.rotation);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
        }
    }
}

