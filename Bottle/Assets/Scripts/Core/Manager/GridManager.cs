using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Extensions.Singleton;
using Bottle.Core.GridObjectData;
using UnityEditor;
namespace Bottle.Core.Manager
{
    public class GridManager : PersistentObject<GridManager>
    {
        public Grid grid; // Reference to the current Grid being used

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        //public T GetGridObjectAtPosition<T>(Vector2Int gridPosition, float gridHeight)
        //{
        //    if (!Application.isPlaying)
        //    {
        //        return GetGridTileAtPositionInEditor(gridPosition);
        //    }
        //    else
        //    {
        //        if (!gridTiles.ContainsKey(gridPosition))
        //            return null;
        //        return gridTiles[gridPosition];
        //    }
        //}

        //public virtual void EraseGridTileAtPosition(Vector2Int gridPosition, float gridHeight)
        //{
        //    var tileAtPosition = GetTileAtPosition(gridPosition);
        //    if (tileAtPosition != null)
        //        EraseGridTile(tileAtPosition);
        //}


        public T CreateGridObject<T>(T gridObjectPrefab,
                                    Vector2Int gridPosition,
                                    float gridHeight,
                                    float scale,
                                    Quaternion rotation)
        {
#if UNITY_EDITOR
            if (gridObjectPrefab == null)
                return gridObjectPrefab;

            var worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));

            if (gridObjectPrefab is GridTile)
            {

                GridTile instantiatedGridTile = null;

                instantiatedGridTile = (GridTile)PrefabUtility.InstantiatePrefab((GridTile)(object)gridObjectPrefab);
                if (instantiatedGridTile != null)
                    Undo.RegisterCreatedObjectUndo((Object)instantiatedGridTile.gameObject, "Paint Grid Object Prefab");

                // Transform values
                var targetWorldPosition = worldPosition + new Vector3(0f, gridHeight, 0f);
                instantiatedGridTile.transform.position = targetWorldPosition;
                instantiatedGridTile.transform.rotation = rotation;
                // Set the tile's settings
                instantiatedGridTile.gridPosition = gridPosition;
                instantiatedGridTile.gridHeight = gridHeight;
            }

            return gridObjectPrefab;
#endif
        }
    }
}

