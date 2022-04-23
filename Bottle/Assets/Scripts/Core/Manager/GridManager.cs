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
        private static string gridSetupPrefabPath = "Prefabs/SceneSetup/GridSetup";
        public Grid grid; // Reference to the current Grid being used
        [SerializeField]
        protected GameObject _tileHolder;
        public GameObject TileHolder
        {
            get
            {
                return _tileHolder; // Only need to return _tileHolder because we alread defined this in prefab
            }
            set
            {
                _tileHolder = value;
            }
        }

        [MenuItem("Bottle/Scene Setup/Grid Setup", false, 1)]
        public static void GridSetup()
        {
            var gridSetupPrefab = Resources.Load(gridSetupPrefabPath);
            PrefabUtility.InstantiatePrefab(gridSetupPrefab);
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        public T GetGridObjectAtPosition<T>(Vector2Int gridPosition, float gridHeight)
        {
            System.Type gridObjectType = typeof(T);
            if (gridObjectType.Equals(typeof(GridTile)))
            {
                GridTile[] allGridTiles = TileHolder.GetComponentsInChildren<GridTile>();
                for (int i = 0; i < allGridTiles.Length; i++)
                {
                    if (gridPosition == allGridTiles[i].gridPosition && gridHeight == allGridTiles[i].gridHeight)
                        return (T)(object)allGridTiles[i];
                }
            }
            return default(T);
        }

        public void EraseGridObjectAtPosition<T>(Vector2Int gridPosition, float gridHeight)
        {
            System.Type gridObjectType = typeof(T);
            if (gridObjectType.Equals(typeof(GridTile)))
            {
                GridTile gridTileAtPosition = GetGridObjectAtPosition<GridTile>(gridPosition, gridHeight);
                if (gridTileAtPosition != null)
                {
                    Undo.DestroyObjectImmediate(gridTileAtPosition.gameObject);
                }
                    
            }
        }

        public T CreateGridObject<T>(T gridObjectPrefab,
                                    Vector2Int gridPosition,
                                    float gridHeight,
                                    float scale,
                                    Quaternion rotation)
        {
            if (gridObjectPrefab == null)
                return default(T);

            Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));

            if (gridObjectPrefab is GridTile)
            {

                GridTile instantiatedGridTile = null;
                
                if (!Application.isPlaying) // Instantiate prefab in editor mode 
                {
                    instantiatedGridTile = (GridTile)PrefabUtility.InstantiatePrefab((GridTile)(object)gridObjectPrefab);
                    if (instantiatedGridTile != null)
                        Undo.RegisterCreatedObjectUndo((Object)instantiatedGridTile.gameObject, "Paint Grid Object Prefab");
                }
                else // Instantiate prefab in game mode 
                {
                    instantiatedGridTile = Instantiate(instantiatedGridTile) as GridTile;
                }

                // Transform values
                var targetWorldPosition = worldPosition + new Vector3(0f, gridHeight, 0f);
                instantiatedGridTile.transform.SetParent(TileHolder.transform);
                instantiatedGridTile.transform.position = targetWorldPosition;
                instantiatedGridTile.transform.localScale = new Vector3(scale, scale, scale);
                instantiatedGridTile.transform.rotation = rotation;
                // Set the grid object's settings
                instantiatedGridTile.gridPosition = gridPosition;
                instantiatedGridTile.gridHeight = gridHeight;
                return (T)(object)instantiatedGridTile;
            }
            return default(T);
        }
    }
}

