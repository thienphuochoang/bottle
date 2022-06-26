using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Extensions.Singleton;
using Bottle.Core.GridObjectData;
using UnityEditor;
using Bottle.Core.PathSystem;
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

        public List<T> GetGridObjectAtPosition<T>(Vector2Int gridPosition, float gridHeight)
        {
            System.Type gridObjectType = typeof(T);
            List<T> gridObjectList = new List<T>();
            if (gridObjectType.Equals(typeof(GridTile)))
            {
                
                GridTile[] allGridTiles = TileHolder.GetComponentsInChildren<GridTile>();
                for (int i = 0; i < allGridTiles.Length; i++)
                {
                    if (gridPosition == allGridTiles[i].gridPosition && gridHeight == allGridTiles[i].gridHeight)
                    {
                        gridObjectList.Add((T)(object)allGridTiles[i]);
                    }
                }
            }
            else if (gridObjectType.Equals(typeof(GridEntity)))
            {
                GridEntity[] allGridEntities = EntityHolder.GetComponentsInChildren<GridEntity>();
                for (int i = 0; i < allGridEntities.Length; i++)
                {
                    if (gridPosition == allGridEntities[i].gridPosition && gridHeight == allGridEntities[i].gridHeight)
                    {
                        gridObjectList.Add((T)(object)allGridEntities[i]);
                    }
                }
            }
            return gridObjectList;
        }

        public void EraseGridObjectAtPosition<T>(Vector2Int gridPosition, float gridHeight)
        {
            System.Type gridObjectType = typeof(T);
            if (gridObjectType.Equals(typeof(GridTile)))
            {
                GridTile gridTileAtPosition = GetGridObjectAtPosition<GridTile>(gridPosition, gridHeight)[0];
                if (gridTileAtPosition != null)
                {
                    Undo.DestroyObjectImmediate(gridTileAtPosition.gameObject);
                }
            }
            else if (gridObjectType.Equals(typeof(GridEntity)))
            {
                GridEntity gridEntityAtPosition = GetGridObjectAtPosition<GridEntity>(gridPosition, gridHeight)[0];
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

        public Vector3Int ConvertWorldPositionToGridPosition(GridObject gridObject)
        {
            Vector3Int cellPosition = this.grid.WorldToCell(gridObject.transform.position);
            Vector3 newPos = this.grid.GetCellCenterWorld(cellPosition);
            newPos.y = Mathf.Ceil(gridObject.transform.position.y);
            gridObject.transform.position = newPos;
            // Mathf.RoundToInt did not work because they always round up to even result
            Vector2Int gridPosition = new Vector2Int((int)(newPos.x - 0.5f), (int)(newPos.z - 0.5f));
            float gridHeight = (int)newPos.y + Mathf.Ceil(gridObject.pivotOffset.y);
            return new Vector3Int(gridPosition.x, (int)gridHeight, gridPosition.y);
        }

        public Vector3Int ConvertWorldPositionToGridPosition(Vector3 worldSpacePosition, float pivotOffsetYAxis)
        {
            Vector3Int cellPosition = this.grid.WorldToCell(worldSpacePosition);
            Vector3 newPos = this.grid.GetCellCenterWorld(cellPosition);
            newPos.y = Mathf.Ceil(worldSpacePosition.y);
            // Mathf.RoundToInt did not work because they always round up to even result
            Vector2Int gridPosition = new Vector2Int((int)(newPos.x - 0.5f), (int)(newPos.z - 0.5f));
            float gridHeight = (int)newPos.y + Mathf.Ceil(pivotOffsetYAxis);
            return new Vector3Int(gridPosition.x, (int)gridHeight, gridPosition.y);
        }

        public List<GridTile> GetNeighbourGridTiles(GridTile currentTile)
        {
            List<GridTile> possibleTiles = new List<GridTile>();
            Vector2Int firstAroundTilePos = new Vector2Int(currentTile.gridPosition.x, currentTile.gridPosition.y - 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile firstAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstAroundTilePos, currentTile.gridHeight)[0];
                possibleTiles.Add(firstAroundTile);
            }

            Vector2Int secondAroundTilePos = new Vector2Int(currentTile.gridPosition.x, currentTile.gridPosition.y + 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile secondAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondAroundTilePos, currentTile.gridHeight)[0];
                possibleTiles.Add(secondAroundTile);
            }

            Vector2Int thirdAroundTilePos = new Vector2Int(currentTile.gridPosition.x - 1, currentTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile thirdAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdAroundTilePos, currentTile.gridHeight)[0];
                possibleTiles.Add(thirdAroundTile);
            }

            Vector2Int fourthAroundTilePos = new Vector2Int(currentTile.gridPosition.x + 1, currentTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile fourthAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthAroundTilePos, currentTile.gridHeight)[0];
                possibleTiles.Add(fourthAroundTile);
            }
            return possibleTiles;
        }
        public List<PathFindingGridTile> GetNeighbourGridTiles(PathFindingGridTile currentPathFindingGridTile)
        {
            List<PathFindingGridTile> possibleTiles = new List<PathFindingGridTile>();
            Vector2Int firstAroundTilePos = new Vector2Int(currentPathFindingGridTile.currentGridTile.gridPosition.x, currentPathFindingGridTile.currentGridTile.gridPosition.y - 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstAroundTilePos, currentPathFindingGridTile.currentGridTile.gridHeight).Count > 0)
            {
                GridTile firstAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstAroundTilePos, currentPathFindingGridTile.currentGridTile.gridHeight)[0];
                PathFindingGridTile firstAroundPathFindingGridTile = new PathFindingGridTile();
                firstAroundPathFindingGridTile.currentGridTile = firstAroundTile;
                possibleTiles.Add(firstAroundPathFindingGridTile);
            }

            Vector2Int secondAroundTilePos = new Vector2Int(currentPathFindingGridTile.currentGridTile.gridPosition.x, currentPathFindingGridTile.currentGridTile.gridPosition.y + 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondAroundTilePos, currentPathFindingGridTile.currentGridTile.gridHeight).Count > 0)
            {
                GridTile secondAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondAroundTilePos, currentPathFindingGridTile.currentGridTile.gridHeight)[0];
                PathFindingGridTile secondAroundPathFindingGridTile = new PathFindingGridTile();
                secondAroundPathFindingGridTile.currentGridTile = secondAroundTile;
                possibleTiles.Add(secondAroundPathFindingGridTile);
            }

            Vector2Int thirdAroundTilePos = new Vector2Int(currentPathFindingGridTile.currentGridTile.gridPosition.x - 1, currentPathFindingGridTile.currentGridTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdAroundTilePos, currentPathFindingGridTile.currentGridTile.gridHeight).Count > 0)
            {
                GridTile thirdAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdAroundTilePos, currentPathFindingGridTile.currentGridTile.gridHeight)[0];
                PathFindingGridTile thirdAroundPathFindingGridTile = new PathFindingGridTile();
                thirdAroundPathFindingGridTile.currentGridTile = thirdAroundTile;
                possibleTiles.Add(thirdAroundPathFindingGridTile);
            }

            Vector2Int fourthAroundTilePos = new Vector2Int(currentPathFindingGridTile.currentGridTile.gridPosition.x + 1, currentPathFindingGridTile.currentGridTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthAroundTilePos, currentPathFindingGridTile.currentGridTile.gridHeight).Count > 0)
            {
                GridTile fourthAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthAroundTilePos, currentPathFindingGridTile.currentGridTile.gridHeight)[0];
                PathFindingGridTile fourthAroundPathFindingGridTile = new PathFindingGridTile();
                fourthAroundPathFindingGridTile.currentGridTile = fourthAroundTile;
                possibleTiles.Add(fourthAroundPathFindingGridTile);
            }
            return possibleTiles;
        }
    }
}

