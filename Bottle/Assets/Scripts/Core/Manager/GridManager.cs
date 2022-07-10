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

        public List<GridTile> GetNeighbourGridTiles(GridTile currentTile, List<Vector2Int> offsetGridPositionValueList, List<int> offsetGridHeightValueList)
        {
            List<GridTile> possibleTiles = new List<GridTile>();
            foreach (int offsetGridHeightValue in offsetGridHeightValueList)
            {
                foreach (Vector2Int offsetGridPositionValue in offsetGridPositionValueList)
                {
                    Vector2Int neighbourTilePos = currentTile.gridPosition + offsetGridPositionValue;
                    if (Instance.GetGridObjectAtPosition<GridTile>(neighbourTilePos, currentTile.gridHeight + offsetGridHeightValue).Count > 0)
                    {
                        GridTile neighbourTile = Instance.GetGridObjectAtPosition<GridTile>(neighbourTilePos, currentTile.gridHeight + offsetGridHeightValue)[0];
                        if (offsetGridHeightValue != 0)
                        {
                            if (neighbourTile.isARamp)
                            {
                                possibleTiles.Add(neighbourTile);
                            }
                            else if (offsetGridHeightValueList.Count == 1 && offsetGridHeightValueList[0] == -1)
                                possibleTiles.Add(neighbourTile);
                        }
                        if (offsetGridHeightValue == 0)
                        {
                            if (neighbourTile.isARamp)
                            {
                                possibleTiles.Add(neighbourTile);
                                foreach (var tile in GetNeighbourGridTiles(neighbourTile, offsetGridPositionValueList, new List<int>() { -1 }))
                                {
                                    Debug.Log(tile);
                                    possibleTiles.Add(tile);
                                }
                                    
                            }
                            else
                                possibleTiles.Add(neighbourTile);
                        }
                    }
                }
            }
            return possibleTiles;
            /*
            List<GridTile> possibleTiles = new List<GridTile>();

            //The FIRST trio of grid tiles: higher + lower + same height grid tiles

            Vector2Int firstAroundTilePos = new Vector2Int(currentTile.gridPosition.x, currentTile.gridPosition.y - 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile firstAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstAroundTilePos, currentTile.gridHeight)[0];
                possibleTiles.Add(firstAroundTile);
                if (firstAroundTile.isARamp)
                {
                    Vector2Int firstLowerAroundRampTilePos = new Vector2Int(firstAroundTile.gridPosition.x, firstAroundTile.gridPosition.y - 1);
                    Vector2Int secondLowerAroundRampTilePos = new Vector2Int(firstAroundTile.gridPosition.x, firstAroundTile.gridPosition.y + 1);
                    Vector2Int thirdLowerAroundRampTilePos = new Vector2Int(firstAroundTile.gridPosition.x + 1, firstAroundTile.gridPosition.y);
                    Vector2Int fourthLowerAroundRampTilePos = new Vector2Int(firstAroundTile.gridPosition.x - 1, firstAroundTile.gridPosition.y);
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile firstLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(firstLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile secondLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(secondLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile thirdLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(thirdLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile fourthLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(fourthLowerAroundRampTile);
                    }
                }
            }

            Vector2Int firstHigherAroundTilePos = new Vector2Int(currentTile.gridPosition.x, currentTile.gridPosition.y - 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstHigherAroundTilePos, currentTile.gridHeight + 1).Count > 0)
            {
                GridTile firstAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstHigherAroundTilePos, currentTile.gridHeight + 1)[0];
                if (firstAroundTile.isARamp)
                    possibleTiles.Add(firstAroundTile);
            }

            Vector2Int firstLowerAroundTilePos = new Vector2Int(currentTile.gridPosition.x, currentTile.gridPosition.y - 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstLowerAroundTilePos, currentTile.gridHeight - 1).Count > 0)
            {
                GridTile firstAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstLowerAroundTilePos, currentTile.gridHeight - 1)[0];
                if (firstAroundTile.isARamp)
                    possibleTiles.Add(firstAroundTile);
            }

            //The SECOND trio of grid tiles: higher + lower + same height grid tiles

            Vector2Int secondAroundTilePos = new Vector2Int(currentTile.gridPosition.x, currentTile.gridPosition.y + 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile secondAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondAroundTilePos, currentTile.gridHeight)[0];
                possibleTiles.Add(secondAroundTile);
                if (secondAroundTile.isARamp)
                {
                    Vector2Int firstLowerAroundRampTilePos = new Vector2Int(secondAroundTile.gridPosition.x, secondAroundTile.gridPosition.y - 1);
                    Vector2Int secondLowerAroundRampTilePos = new Vector2Int(secondAroundTile.gridPosition.x, secondAroundTile.gridPosition.y + 1);
                    Vector2Int thirdLowerAroundRampTilePos = new Vector2Int(secondAroundTile.gridPosition.x + 1, secondAroundTile.gridPosition.y);
                    Vector2Int fourthLowerAroundRampTilePos = new Vector2Int(secondAroundTile.gridPosition.x - 1, secondAroundTile.gridPosition.y);
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile firstLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(firstLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile secondLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(secondLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile thirdLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(thirdLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile fourthLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(fourthLowerAroundRampTile);
                    }
                }
            }

            Vector2Int secondHigherAroundTilePos = new Vector2Int(currentTile.gridPosition.x, currentTile.gridPosition.y + 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondHigherAroundTilePos, currentTile.gridHeight + 1).Count > 0)
            {
                GridTile secondAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondHigherAroundTilePos, currentTile.gridHeight + 1)[0];
                if (secondAroundTile.isARamp)
                    possibleTiles.Add(secondAroundTile);
            }

            Vector2Int secondLowerAroundTilePos = new Vector2Int(currentTile.gridPosition.x, currentTile.gridPosition.y + 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondLowerAroundTilePos, currentTile.gridHeight - 1).Count > 0)
            {
                GridTile secondAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondLowerAroundTilePos, currentTile.gridHeight - 1)[0];
                if (secondAroundTile.isARamp)
                    possibleTiles.Add(secondAroundTile);
            }

            //The THIRD trio of grid tiles: higher + lower + same height grid tiles

            Vector2Int thirdAroundTilePos = new Vector2Int(currentTile.gridPosition.x - 1, currentTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile thirdAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdAroundTilePos, currentTile.gridHeight)[0];
                possibleTiles.Add(thirdAroundTile);
                if (thirdAroundTile.isARamp)
                {
                    Vector2Int firstLowerAroundRampTilePos = new Vector2Int(thirdAroundTile.gridPosition.x, thirdAroundTile.gridPosition.y - 1);
                    Vector2Int secondLowerAroundRampTilePos = new Vector2Int(thirdAroundTile.gridPosition.x, thirdAroundTile.gridPosition.y + 1);
                    Vector2Int thirdLowerAroundRampTilePos = new Vector2Int(thirdAroundTile.gridPosition.x + 1, thirdAroundTile.gridPosition.y);
                    Vector2Int fourthLowerAroundRampTilePos = new Vector2Int(thirdAroundTile.gridPosition.x - 1, thirdAroundTile.gridPosition.y);
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile firstLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(firstLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile secondLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(secondLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile thirdLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(thirdLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile fourthLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(fourthLowerAroundRampTile);
                    }
                }
            }

            Vector2Int thirdHigherAroundTilePos = new Vector2Int(currentTile.gridPosition.x - 1, currentTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdHigherAroundTilePos, currentTile.gridHeight + 1).Count > 0)
            {
                GridTile thirdAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdHigherAroundTilePos, currentTile.gridHeight + 1)[0];
                if (thirdAroundTile.isARamp)
                    possibleTiles.Add(thirdAroundTile);
            }

            Vector2Int thirdLowerAroundTilePos = new Vector2Int(currentTile.gridPosition.x - 1, currentTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdLowerAroundTilePos, currentTile.gridHeight - 1).Count > 0)
            {
                GridTile thirdAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdLowerAroundTilePos, currentTile.gridHeight - 1)[0];
                if (thirdAroundTile.isARamp)
                    possibleTiles.Add(thirdAroundTile);
            }

            //The FOURTH trio of grid tiles: higher + lower + same height grid tiles

            Vector2Int fourthAroundTilePos = new Vector2Int(currentTile.gridPosition.x + 1, currentTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile fourthAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthAroundTilePos, currentTile.gridHeight)[0];
                possibleTiles.Add(fourthAroundTile);
                if (fourthAroundTile.isARamp)
                {
                    Vector2Int firstLowerAroundRampTilePos = new Vector2Int(fourthAroundTile.gridPosition.x, fourthAroundTile.gridPosition.y - 1);
                    Vector2Int secondLowerAroundRampTilePos = new Vector2Int(fourthAroundTile.gridPosition.x, fourthAroundTile.gridPosition.y + 1);
                    Vector2Int thirdLowerAroundRampTilePos = new Vector2Int(fourthAroundTile.gridPosition.x + 1, fourthAroundTile.gridPosition.y);
                    Vector2Int fourthLowerAroundRampTilePos = new Vector2Int(fourthAroundTile.gridPosition.x - 1, fourthAroundTile.gridPosition.y);
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile firstLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(firstLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile secondLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(secondLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile thirdLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(thirdLowerAroundRampTile);
                    }
                    if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthLowerAroundRampTilePos, currentTile.gridHeight - 1).Count > 0)
                    {
                        GridTile fourthLowerAroundRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthLowerAroundRampTilePos, currentTile.gridHeight - 1)[0];
                        possibleTiles.Add(fourthLowerAroundRampTile);
                    }
                }
            }

            Vector2Int fourthHigherAroundTilePos = new Vector2Int(currentTile.gridPosition.x + 1, currentTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthHigherAroundTilePos, currentTile.gridHeight + 1).Count > 0)
            {
                GridTile fourthAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthHigherAroundTilePos, currentTile.gridHeight + 1)[0];
                if (fourthAroundTile.isARamp)
                    possibleTiles.Add(fourthAroundTile);
            }

            Vector2Int fourthLowerAroundTilePos = new Vector2Int(currentTile.gridPosition.x + 1, currentTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthLowerAroundTilePos, currentTile.gridHeight - 1).Count > 0)
            {
                GridTile fourthAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthLowerAroundTilePos, currentTile.gridHeight - 1)[0];
                if (fourthAroundTile.isARamp)
                    possibleTiles.Add(fourthAroundTile);
            }
            return possibleTiles;
            */
        }
    }
}

