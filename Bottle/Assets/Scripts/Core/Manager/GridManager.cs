using System.Collections;
using System.Collections.Generic;
using Bottle.Core.GridObjectAbility;
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

        public List<GridTile> GetNeighbourGridTiles(GridTile currentTile, List<Vector2Int> offsetGridPositionValueList, List<int> offsetGridHeightValueList, bool isFromDownToUpperHeight)
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
                            {
                                var (blockableGridTiles, blockableGridEntities) = GridEntityMovementAbility.GetBlockableGridObjects(neighbourTile);
                                if (neighbourTile.isStandable == true && blockableGridTiles.Count == 0 && blockableGridEntities.Count == 0)
                                {
                                    possibleTiles.Add(neighbourTile);
                                }
                            }
                        }
                        if (offsetGridHeightValue == 0)
                        {
                            if (neighbourTile.isARamp)
                            {
                                possibleTiles.Add(neighbourTile);
                                foreach (var tile in GetNeighbourGridTiles(neighbourTile, offsetGridPositionValueList, new List<int>() { -1 }, isFromDownToUpperHeight))
                                {
                                    var (blockableGridTiles, blockableGridEntities) = GridEntityMovementAbility.GetBlockableGridObjects(tile);
                                    if (tile.isStandable == true && blockableGridTiles.Count == 0 && blockableGridEntities.Count == 0)
                                    {
                                        if (isFromDownToUpperHeight == false)
                                        {
                                            possibleTiles.Add(tile);
                                            tile.cameFromGridTile = neighbourTile;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var (blockableGridTiles, blockableGridEntities) = GridEntityMovementAbility.GetBlockableGridObjects(neighbourTile);
                                if (neighbourTile.isStandable == true && blockableGridTiles.Count == 0 && blockableGridEntities.Count == 0)
                                {
                                    possibleTiles.Add(neighbourTile);
                                }
                            }
                        }
                    }
                }
            }
            return possibleTiles;
        }
        public Transform SetupPreviewGridObject(Transform transform)
        {
            // Attempt to get reference to GameObject Renderer
            Renderer meshRenderer = transform.gameObject.GetComponent<Renderer>();

            // If a Renderer was found
            if (meshRenderer != null)
            {
                // Define temporary Material used to create transparent copy of GameObject Material
                Material tempMat = new Material(Shader.Find("Bottle/Preview_Tiles_PBL"));
                Material[] tempMats = new Material[meshRenderer.sharedMaterials.Length];
                
                // Loop through each material in GameObject
                for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                {
                    // Get material from GameObject
                    tempMat = new Material(meshRenderer.sharedMaterials[i]);

                    // Change Shader to "Standard"
                    tempMat.shader = Shader.Find("Bottle/Preview_Tiles_PBL");
                    

                    // Replace GameObject Material with transparent one
                    tempMats[i] = tempMat;
                }
                
                meshRenderer.sharedMaterials = tempMats;
            }

            // Recursively run this method for each child transform
            foreach (Transform child in transform)
            {
                SetupPreviewGridObject(child);
            }

            return transform;
        }
        /*public T CreateGridObject<T>(T gridObjectPrefab,
            Vector2Int gridPosition,
            float gridHeight,
            float scale,
            Quaternion rotation) where T : Component*/
        public void RemovePreviewGridObjects<T>() where T : Component
        {
            if (typeof(T) == typeof(GridTile))
            {
                GridTile[] allGridTiles = GridManager.Instance.TileHolder.GetComponentsInChildren<GridTile>();
                for (int i = 0; i < allGridTiles.Length; i++)
                {
                    if (allGridTiles[i].IsPreviewObject)
                    {
                        DestroyImmediate(allGridTiles[i].gameObject);
                    }
                }
            }
            else if (typeof(T) == typeof(GridEntity))
            {
                GridEntity[] allGridEntities = GridManager.Instance.EntityHolder.GetComponentsInChildren<GridEntity>();
                for (int i = 0; i < allGridEntities.Length; i++)
                {
                    if (allGridEntities[i].IsPreviewObject)
                    {
                        DestroyImmediate(allGridEntities[i].gameObject);
                    }
                }
            }
        }
    }
}

