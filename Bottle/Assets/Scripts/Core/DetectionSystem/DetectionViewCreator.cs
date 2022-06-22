using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.GridObjectAbility;
using Bottle.Core.Manager;
using Bottle.Core.GridObjectData;
using System.Linq;
namespace Bottle.Core.DetectionSystem
{
    [ExecuteInEditMode]
    public class DetectionViewCreator : MonoBehaviour
    {
        private GridEntity currentGridEntity;
        private List<GridTile> openList = new List<GridTile>();
        private List<GridEntity> allGridEntitiesInView = new List<GridEntity>();
        private List<GridTile> visitedTiles = new List<GridTile>();
        [SerializeField]
        private List<GridTile> finalPath = new List<GridTile>();
        public static Vector2Int[] eightDirections = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1)
        };
        [BoxGroup("Detection View Settings", true, true)]
        [Tooltip("The X size of this detection view in the Grid.")]
        [SerializeField]
        private int _xBoundingBoxSize = 1;
        public int xBoundingBoxSize
        {
            get => _xBoundingBoxSize;
            set
            {
                if (_xBoundingBoxSize == value) return;
                _xBoundingBoxSize = value;
                if (OnSizeChanged != null)
                    OnSizeChanged(_xBoundingBoxSize);
            }
        }
        [BoxGroup("Detection View Settings", true, true)]
        [Tooltip("The Y size of this detection view in the Grid.")]
        [SerializeField]
        private int _yBoundingBoxSize = 1;
        public int yBoundingBoxSize
        {
            get => _yBoundingBoxSize;
            set
            {
                if (_yBoundingBoxSize == value) return;
                _yBoundingBoxSize = value;
                if (OnSizeChanged != null)
                    OnSizeChanged(_yBoundingBoxSize);
            }
        }
        [BoxGroup("Detection View Settings", true, true)]
        [Tooltip("The Z size of this detection view in the Grid.")]
        [SerializeField]
        private int _zBoundingBoxSize = 1;
        public int zBoundingBoxSize
        {
            get => _zBoundingBoxSize;
            set
            {
                if (_zBoundingBoxSize == value) return;
                _zBoundingBoxSize = value;
                if (OnSizeChanged != null)
                    OnSizeChanged(_zBoundingBoxSize);
            }
        }
        [TitleGroup("Color Settings", alignment: TitleAlignments.Centered), Tooltip("Color Settings of nodes and paths")]
        public Color defaultColor = Color.white;
        public Color hoveredColor = Color.red;
        public Color selectedColor = Color.green;

        public delegate void OnSizeChangedDelegate(int newBoundingBoxSize);
        public event OnSizeChangedDelegate OnSizeChanged;

        private void OnDetectionViewChangedHandler(Vector2Int newGridPosition, int newGridHeight)
        {
        }
        public void CalculateDetectionView(Dictionary<string, object> message)
        {
            if ((GridEntity)message["GridEntity"] == currentGridEntity)
            {
                for (int i = 1; i <= xBoundingBoxSize; i++)
                {
                    if (i == 1)
                    {
                        Vector2Int targetGridEntity = new Vector2Int(currentGridEntity.gridPosition.x, currentGridEntity.gridPosition.y);
                        List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(targetGridEntity, (int)currentGridEntity.gridHeight);
                        if (gridEntitiesAtPosition.Count > 0)
                        {
                            foreach (GridEntity gridEntity in gridEntitiesAtPosition)
                            {
                                if (gridEntity != currentGridEntity)
                                    allGridEntitiesInView.Add(gridEntity);
                            }
                            
                        }
                    }
                    if (i == 2)
                    {
                        foreach (Vector2Int direction in eightDirections)
                        {
                            Vector2Int targetGridEntity = new Vector2Int(currentGridEntity.gridPosition.x, currentGridEntity.gridPosition.y) + direction;
                            List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(targetGridEntity, (int)currentGridEntity.gridHeight);
                            if (gridEntitiesAtPosition.Count > 0)
                            {
                                allGridEntitiesInView.Add(gridEntitiesAtPosition[0]);
                            }
                        }
                    }
                    if (i > 2)
                    {
                        for (int n = 0; n < eightDirections.Length; n++)
                        {
                            Vector2Int currentTargetEntity = new Vector2Int();
                            Vector2Int nextTargetEntity = new Vector2Int();
                            if (n == (eightDirections.Length - 1))
                            {
                                currentTargetEntity = new Vector2Int(currentGridEntity.gridPosition.x, currentGridEntity.gridPosition.y) + eightDirections[n] * (i - 1);
                                nextTargetEntity = new Vector2Int(currentGridEntity.gridPosition.x, currentGridEntity.gridPosition.y) + eightDirections[0] * (i - 1);
                            }
                            else
                            {
                                currentTargetEntity = new Vector2Int(currentGridEntity.gridPosition.x, currentGridEntity.gridPosition.y) + eightDirections[n] * (i - 1);
                                nextTargetEntity = new Vector2Int(currentGridEntity.gridPosition.x, currentGridEntity.gridPosition.y) + eightDirections[n + 1] * (i - 1);
                            }

                            Vector2Int value = nextTargetEntity - currentTargetEntity;
                            if (currentTargetEntity.x != nextTargetEntity.x)
                            {
                                for (int n2 = 1; n2 <= Mathf.Abs(value.x); n2++)
                                {
                                    if (value.x < 0)
                                    {
                                        Vector2Int middlePos = new Vector2Int(currentTargetEntity.x + -n2, currentTargetEntity.y);
                                        List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(middlePos, (int)currentGridEntity.gridHeight);
                                        if (gridEntitiesAtPosition.Count > 0)
                                            allGridEntitiesInView.Add(gridEntitiesAtPosition[0]);
                                    }
                                    else
                                    {
                                        Vector2Int middlePos = new Vector2Int(currentTargetEntity.x + n2, currentTargetEntity.y);
                                        List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(middlePos, (int)currentGridEntity.gridHeight);
                                        if (gridEntitiesAtPosition.Count > 0)
                                            allGridEntitiesInView.Add(gridEntitiesAtPosition[0]);
                                    }

                                }
                            }
                            if (currentTargetEntity.y != nextTargetEntity.y)
                            {
                                for (int n2 = 1; n2 <= Mathf.Abs(value.y); n2++)
                                {
                                    if (value.y < 0)
                                    {
                                        Vector2Int middlePos = new Vector2Int(currentTargetEntity.x, currentTargetEntity.y + -n2);
                                        List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(middlePos, (int)currentGridEntity.gridHeight);
                                        if (gridEntitiesAtPosition.Count > 0)
                                            allGridEntitiesInView.Add(gridEntitiesAtPosition[0]);
                                    }
                                    else
                                    {
                                        Vector2Int middlePos = new Vector2Int(currentTargetEntity.x, currentTargetEntity.y + n2);
                                        List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(middlePos, (int)currentGridEntity.gridHeight);
                                        if (gridEntitiesAtPosition.Count > 0)
                                            allGridEntitiesInView.Add(gridEntitiesAtPosition[0]);
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
        private void Awake() 
        {
            currentGridEntity = GetComponent<GridEntity>();
        }
        public void Start()
        {
            //EventManager.Instance.StartListening("RecalculateDetectionView", CalculateDetectionView);
            //_thisGridObject.OnPositionChanged += OnDetectionViewChangedHandler;
        }
        private void Update()
        {
            //if (xBoundingBoxSize % 2 == 0)
            //{
            //    xBoundingBoxSize = xBoundingBoxSize + 1;
            //}
            //if (zBoundingBoxSize % 2 == 0)
            //{
            //    zBoundingBoxSize = zBoundingBoxSize + 1;
            //}
            if (yBoundingBoxSize % 2 != 0)
            {
                yBoundingBoxSize = yBoundingBoxSize + 1;
            }
            if (allGridEntitiesInView.Count > 0)
            {
                openList.Add(currentGridEntity.currentStandingGridTile);
                FindingPath();
            }
                
        }

        private static List<GridTile> GetWalkableTiles(GridTile currentTile, GridTile targetTile)
        {
            List<GridTile> possibleTiles = new List<GridTile>();
            Vector2Int firstAroundTilePos = new Vector2Int(currentTile.gridPosition.x, currentTile.gridPosition.y - 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile firstAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(firstAroundTilePos, currentTile.gridHeight)[0];
                firstAroundTile.parent = currentTile;
                possibleTiles.Add(firstAroundTile);
            }

            Vector2Int secondAroundTilePos = new Vector2Int(currentTile.gridPosition.x, currentTile.gridPosition.y + 1);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile secondAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(secondAroundTilePos, currentTile.gridHeight)[0];
                secondAroundTile.parent = currentTile;
                possibleTiles.Add(secondAroundTile);
            }

            Vector2Int thirdAroundTilePos = new Vector2Int(currentTile.gridPosition.x - 1, currentTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile thirdAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(thirdAroundTilePos, currentTile.gridHeight)[0];
                thirdAroundTile.parent = currentTile;
                possibleTiles.Add(thirdAroundTile);
            }

            Vector2Int fourthAroundTilePos = new Vector2Int(currentTile.gridPosition.x + 1, currentTile.gridPosition.y);
            if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthAroundTilePos, currentTile.gridHeight).Count > 0)
            {
                GridTile fourthAroundTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(fourthAroundTilePos, currentTile.gridHeight)[0];
                fourthAroundTile.parent = currentTile;
                possibleTiles.Add(fourthAroundTile);
            }


            foreach (GridTile eachTile in possibleTiles)
            {
                //if (GridManager.Instance.GetGridObjectAtPosition<GridTile>(eachTile.gridPosition, eachTile.gridHeight).Count > 0)
                eachTile.SetDistance(targetTile.gridPosition.x, targetTile.gridPosition.y);
                //else
                //    possibleTiles.Remove(eachTile);
            }
            return possibleTiles;
        }

        private void FindingPath()
        {
            while (openList.Any())
            {
                var checkTile = openList.OrderByDescending(x => x.costDistance).Last();
                if (checkTile.gridPosition == allGridEntitiesInView[0].currentStandingGridTile.gridPosition) //&& checkTile.Y == finish.Y)
                {
                    var tile = allGridEntitiesInView[0].currentStandingGridTile;
                    finalPath.Add(tile);
                    finalPath.Add(tile.parent);
                    allGridEntitiesInView.RemoveAt(0);
                    return;
                    //while (true)
                    //{
                    //    finalPath.Add(tile.parent);
                    //    tile = tile.parent;
                    //    if (tile == null)
                    //    {
                    //        foreach (var eachTile in finalPath)
                    //        {
                    //            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //            cube.transform.position = new Vector3(eachTile.transform.position.x, eachTile.transform.position.y + 3, eachTile.transform.position.z);
                    //        }
                    //        allGridEntitiesInView.RemoveAt(0);
                    //        return;
                    //    }
                    //}
                }
                visitedTiles.Add(checkTile);
                openList.Remove(checkTile);
                var walkableTiles = GetWalkableTiles(checkTile, allGridEntitiesInView[0].currentStandingGridTile);
                foreach (var walkableTile in walkableTiles)
                {
                    if (visitedTiles.Any(x => x.gridPosition == walkableTile.gridPosition))
                        continue;
                    if (openList.Any(x => x.gridPosition == walkableTile.gridPosition))
                    {
                        var existingTile = openList.First(x => x.gridPosition == walkableTile.gridPosition);
                        if (existingTile.costDistance > checkTile.costDistance)
                        {
                            openList.Remove(existingTile);
                            openList.Add(walkableTile);
                        }
                    }
                    else
                    {
                        openList.Add(walkableTile);
                    }
                }
            }
            Debug.Log("No Path Found!");
        }

        private void OnValidate()
        {
            if (xBoundingBoxSize < 1)
                xBoundingBoxSize = 1;
            if (zBoundingBoxSize < 1)
                zBoundingBoxSize = 1;
            if (yBoundingBoxSize < 2)
                yBoundingBoxSize = 2;
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(transform.position, new Vector3(xBoundingBoxSize + xBoundingBoxSize - 1, yBoundingBoxSize, zBoundingBoxSize + zBoundingBoxSize - 1));
        }
#endif
    }
}

