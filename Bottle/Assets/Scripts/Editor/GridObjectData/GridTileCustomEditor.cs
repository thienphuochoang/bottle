using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector.Editor;
using Bottle.Core.Manager;
namespace Bottle.Editor.GridObjectData
{
    [CustomEditor(typeof(GridTile))]
    class GridTileCustomEditor : OdinEditor
    {
        GridTile currentGridTile;
        private void OnSceneGUI()
        {
            currentGridTile = (GridTile)target;
            if (Event.current.type == EventType.ScrollWheel)
            {
                if ((Event.current.delta).normalized.y == -1)
                {
                    Debug.Log(Input.mouseScrollDelta.y);
                }
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
