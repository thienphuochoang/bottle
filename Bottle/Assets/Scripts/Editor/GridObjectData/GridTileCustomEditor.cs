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
            // [-----------------------------------------------------------------]
            // Please research on how to detect while pressing Left Control and then scroll the mouse wheel to execute function
            if (Event.current.type == EventType.ScrollWheel)
            {
                // Scroll up
                if ((Event.current.delta).normalized.y == -1)
                {
                    currentGridTile.transform.Rotate(0, 90.0f, 0, Space.World);
                }
                // Scroll down
                else if ((Event.current.delta).normalized.y == 1)
                {
                    currentGridTile.transform.Rotate(0, -90.0f, 0, Space.World);
                }
                Event.current.Use();
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            currentGridTile = (GridTile)target;
        }
    }
}
