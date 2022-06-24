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
        [SerializeField]
        private List<GridTile> activeTiles = new List<GridTile>();
        [SerializeField]
        private List<GridEntity> allGridEntitiesInView = new List<GridEntity>();
        [SerializeField]
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
                if (allGridEntitiesInView.Count > 0)
                {
                    activeTiles.Add(currentGridEntity.currentStandingGridTile);
                    currentGridEntity.currentStandingGridTile.gCost = 0;
                    currentGridEntity.currentStandingGridTile.hCost = CalculateDistanceCost(currentGridEntity.currentStandingGridTile, allGridEntitiesInView[0].currentStandingGridTile);
                    FindingPath();
                }
            }
        }
        private void Awake() 
        {
            currentGridEntity = GetComponent<GridEntity>();
        }
        public void Start()
        {
            EventManager.Instance.StartListening("RecalculateDetectionView", CalculateDetectionView);
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
                
        }

        private int CalculateDistanceCost(GridTile a, GridTile b)
        {
            int xDistance = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
            int yDistance = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return remaining;
        }

        private void FindingPath()
        {
            while (activeTiles.Count > 0)
            {
                var currentNode = activeTiles.OrderByDescending(x => x.fCost).Last();
                // Path is found
                if (currentNode == allGridEntitiesInView[0].currentStandingGridTile)
                {
                    finalPath.Clear();
                    finalPath.Add(allGridEntitiesInView[0].currentStandingGridTile);
                    var currentTile = allGridEntitiesInView[0].currentStandingGridTile;
                    while (currentTile.previousTile != null)
                    {
                        finalPath.Add(currentTile.previousTile);
                        currentTile = currentTile.previousTile;
                    }
                    finalPath.Reverse();
                    //foreach (var eachTile in finalPath)
                    //{
                    //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //    cube.transform.position = new Vector3(eachTile.transform.position.x, eachTile.transform.position.y + 2, eachTile.transform.position.z);
                    //}
                    allGridEntitiesInView.RemoveAt(0);
                    return;
                }
                visitedTiles.Add(currentNode);
                activeTiles.Remove(currentNode);
                foreach (var neighbourNode in GridManager.Instance.GetNeighbourGridTiles(currentNode))
                {
                    if (visitedTiles.Contains(neighbourNode))
                        continue;

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.previousTile = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, allGridEntitiesInView[0].currentStandingGridTile);

                        if (!activeTiles.Contains(neighbourNode))
                        {
                            activeTiles.Add(neighbourNode);
                        }
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

