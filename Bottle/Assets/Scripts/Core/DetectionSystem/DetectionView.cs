using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.GridObjectAbility;
using Bottle.Core.Manager;
using Bottle.Core.GridObjectData;
using System.Linq;
using Bottle.Core.PathSystem;
namespace Bottle.Core.DetectionSystem
{
    [ExecuteInEditMode]
    public class DetectionView
    {
        [SerializeField]
        private List<GridEntity> allGridEntitiesInView = new List<GridEntity>();
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


        public static void CalculateDetectionView(Dictionary<string, object> message, GridEntity currentGridEntity, GridEntity targetGridEntity)
        {
            if ((GridEntity)message["GridEntity"] == currentGridEntity)
            {
                List<int> convertedHeightList = ConvertDetectionViewHeightToEntityHeight(yBoundingBoxSize);
                // Scan each grid height
                foreach (int height in convertedHeightList)
                {
                    for (int i = 1; i <= xzBoundingBoxSize; i++)
                    {
                        GridEntity targetEntity = ScanTargetInDetectionView(currentGridEntity, targetGridEntity, (int)currentGridEntity.gridHeight + height, i);
                        if (targetEntity != null)
                            allGridEntitiesInView.Add(targetEntity);
                    }
                }


                if (allGridEntitiesInView.Count > 0)
                {
                    currentGridEntity.currentStandingGridTile.gCost = 0;
                    currentGridEntity.currentStandingGridTile.hCost = PathFinding.CalculateDistanceCost(currentGridEntity.currentStandingGridTile, allGridEntitiesInView[0].currentStandingGridTile);
                    var foundPath = PathFinding.FindingPath(currentGridEntity.currentStandingGridTile, allGridEntitiesInView[0].currentStandingGridTile);
                    finalPath = foundPath;
                    allGridEntitiesInView.RemoveAt(0);
                    GridTile[] allGridTiles = GridManager.Instance.TileHolder.GetComponentsInChildren<GridTile>();
                    for (int i = 0; i < allGridTiles.Length; i++)
                    {
                        PathFinding.ResetDistanceCost(allGridTiles[i]);
                    }
                }
            }
        }

        private List<int> ConvertDetectionViewHeightToEntityHeight(int detectionViewHeight)
        {
            if (detectionViewHeight == 2)
            {
                return new List<int>() { 0, -1 };
            }
            else if (detectionViewHeight > 2)
            {
                List<int> result = new List<int>();
                for (int i = 0; i < detectionViewHeight / 2; i++)
                {
                    result.Add(i);
                }
                for (int i = 1; i <= detectionViewHeight / 2; i++)
                {
                    result.Add(-i);
                }
                return result;
            }
            return null;
        }

        public static GridEntity ScanTargetInDetectionView(GridEntity currentEntity, GridEntity targetEntity, int gridHeight, int detectionBoundingBoxLength)
        {
            // Scan the target in the same grid position of the current grid entity
            /*

             - - -
             - * -
             - - -

             */
            if (detectionBoundingBoxLength == 1)
            {
                Vector2Int targetGridEntity = new Vector2Int(currentEntity.gridPosition.x, currentEntity.gridPosition.y);
                List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(targetGridEntity, gridHeight);
                if (gridEntitiesAtPosition.Count > 0)
                {
                    foreach (GridEntity gridEntity in gridEntitiesAtPosition)
                    {
                        if (gridEntity != currentEntity && gridEntity == targetEntity)
                            return gridEntity;
                    }
                }
            }

            // Scan the target in the neighbour grid tiles of the current grid entity

            /*

             - * -
             * * *
             - * -

             */
            if (detectionBoundingBoxLength == 2)
            {
                foreach (Vector2Int direction in eightDirections)
                {
                    Vector2Int targetGridEntity = new Vector2Int(currentEntity.gridPosition.x, currentEntity.gridPosition.y) + direction;
                    List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(targetGridEntity, gridHeight);
                    if (gridEntitiesAtPosition.Count > 0)
                    {
                        foreach (GridEntity gridEntity in gridEntitiesAtPosition)
                        {
                            if (gridEntity != currentEntity && gridEntity == targetEntity)
                                return gridEntity;
                        }
                    }
                }
            }

            // Scan the target in the further neighbour grid tiles of the current grid entity

            /*

             - - - - - - -
             - * * * * * -
             - * * * * * -
             - * * * * * -
             - * * * * * -
             - * * * * * -
             - - - - - - -

             */

            if (detectionBoundingBoxLength > 2)
            {
                for (int n = 0; n < eightDirections.Length; n++)
                {
                    Vector2Int currentTargetEntity = new Vector2Int();
                    Vector2Int nextTargetEntity = new Vector2Int();
                    // Get all of the grid positions between the last direction and the first direction in the eight directions array
                    if (n == (eightDirections.Length - 1))
                    {
                        currentTargetEntity = new Vector2Int(currentEntity.gridPosition.x, currentEntity.gridPosition.y) + eightDirections[n] * (detectionBoundingBoxLength - 1);
                        nextTargetEntity = new Vector2Int(currentEntity.gridPosition.x, currentEntity.gridPosition.y) + eightDirections[0] * (detectionBoundingBoxLength - 1);
                    }
                    // The other directions
                    else
                    {
                        currentTargetEntity = new Vector2Int(currentEntity.gridPosition.x, currentEntity.gridPosition.y) + eightDirections[n] * (detectionBoundingBoxLength - 1);
                        nextTargetEntity = new Vector2Int(currentEntity.gridPosition.x, currentEntity.gridPosition.y) + eightDirections[n + 1] * (detectionBoundingBoxLength - 1);
                    }

                    Vector2Int value = nextTargetEntity - currentTargetEntity;
                    if (currentTargetEntity.x != nextTargetEntity.x)
                    {
                        for (int n2 = 1; n2 <= Mathf.Abs(value.x); n2++)
                        {
                            if (value.x < 0)
                            {
                                Vector2Int middlePos = new Vector2Int(currentTargetEntity.x + -n2, currentTargetEntity.y);
                                List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(middlePos, gridHeight);
                                if (gridEntitiesAtPosition.Count > 0)
                                {
                                    foreach (GridEntity gridEntity in gridEntitiesAtPosition)
                                    {
                                        if (gridEntity != currentEntity && gridEntity == targetEntity)
                                            return gridEntity;
                                    }
                                }
                            }
                            else
                            {
                                Vector2Int middlePos = new Vector2Int(currentTargetEntity.x + n2, currentTargetEntity.y);
                                List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(middlePos, gridHeight);
                                if (gridEntitiesAtPosition.Count > 0)
                                {
                                    foreach (GridEntity gridEntity in gridEntitiesAtPosition)
                                    {
                                        if (gridEntity != currentEntity && gridEntity == targetEntity)
                                            return gridEntity;
                                    }
                                }
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
                                List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(middlePos, gridHeight);
                                if (gridEntitiesAtPosition.Count > 0)
                                {
                                    foreach (GridEntity gridEntity in gridEntitiesAtPosition)
                                    {
                                        if (gridEntity != currentEntity && gridEntity == targetEntity)
                                            return gridEntity;
                                    }
                                }
                            }
                            else
                            {
                                Vector2Int middlePos = new Vector2Int(currentTargetEntity.x, currentTargetEntity.y + n2);
                                List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(middlePos, gridHeight);
                                if (gridEntitiesAtPosition.Count > 0)
                                {
                                    foreach (GridEntity gridEntity in gridEntitiesAtPosition)
                                    {
                                        if (gridEntity != currentEntity && gridEntity == targetEntity)
                                            return gridEntity;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void Awake() 
        {
            //currentGridEntity = GetComponent<GridEntity>();
        }
        public void Start()
        {
            //EventManager.Instance.StartListening("RecalculateDetectionView", CalculateDetectionView);
            //_thisGridObject.OnPositionChanged += OnDetectionViewChangedHandler;
        }
    }
}

