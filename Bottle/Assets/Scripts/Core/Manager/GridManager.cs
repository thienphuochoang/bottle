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
        [SerializeField]
        protected GameObject _entityHolder;
        public GameObject EntityHolder
        {
            get
            {
                return _entityHolder; // Only need to return _entityHolder because we alread defined this in prefab
            }
            set
            {
                _entityHolder = value;
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
            else if (gridObjectType.Equals(typeof(GridEntity)))
            {
                GridEntity[] allGridEntities = EntityHolder.GetComponentsInChildren<GridEntity>();
                for (int i = 0; i < allGridEntities.Length; i++)
                {
                    if (gridPosition == allGridEntities[i].gridPosition && gridHeight == allGridEntities[i].gridHeight)
                        return (T)(object)allGridEntities[i];
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
            else if (gridObjectType.Equals(typeof(GridEntity)))
            {
                GridEntity gridEntityAtPosition = GetGridObjectAtPosition<GridEntity>(gridPosition, gridHeight);
                if (gridEntityAtPosition != null)
                {
                    Undo.DestroyObjectImmediate(gridEntityAtPosition.gameObject);
                }
            }
        }

        public T CreateGridObject<T>(T gridObjectPrefab,
                                    Vector2Int gridPosition,
                                    float gridHeight,
                                    float scale,
                                    Quaternion rotation) where T : Component
        {
            if (gridObjectPrefab == null)
                return default(T);

            Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));


            T instantiatedGridObject = null;
                
            if (!Application.isPlaying) // Instantiate prefab in editor mode 
            {
                string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot((T)(object)gridObjectPrefab);
                T asset = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(T)) as T;
                instantiatedGridObject = (T)PrefabUtility.InstantiatePrefab(asset);
                if (instantiatedGridObject != null)
                    Undo.RegisterCreatedObjectUndo(instantiatedGridObject.gameObject, "Paint Grid Object Prefab");
            }
            else // Instantiate prefab in game mode 
            {
                instantiatedGridObject = Instantiate(instantiatedGridObject);
            }

            // Transform values
            var targetWorldPosition = worldPosition + new Vector3(0f, gridHeight, 0f);
            System.Type gridObjectType = typeof(T);
            if (gridObjectType.Equals(typeof(GridTile)))
                instantiatedGridObject.transform.SetParent(TileHolder.transform);
            else if (gridObjectType.Equals(typeof(GridEntity)))
                instantiatedGridObject.transform.SetParent(EntityHolder.transform);
            instantiatedGridObject.transform.position = targetWorldPosition;
            instantiatedGridObject.transform.localScale = new Vector3(scale, scale, scale);
            instantiatedGridObject.transform.rotation = rotation;
            // Set the grid object's settings
            if (gridObjectPrefab is GridTile)
            {
                instantiatedGridObject.GetComponent<GridObject>().gridPosition = gridPosition;
                instantiatedGridObject.GetComponent<GridObject>().gridHeight = gridHeight;
            }
            else if (gridObjectPrefab is GridEntity)
            {
                instantiatedGridObject.GetComponent<GridObject>().gridPosition = gridPosition;
                instantiatedGridObject.GetComponent<GridObject>().gridHeight = gridHeight + Mathf.Ceil(instantiatedGridObject.GetComponent<GridObject>().pivotOffset.y);
            }
            return (T)(object)instantiatedGridObject;
        }
    }
}

