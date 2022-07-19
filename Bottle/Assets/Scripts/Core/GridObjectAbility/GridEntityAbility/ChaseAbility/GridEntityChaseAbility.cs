using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectAbility;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
public class GridEntityChaseAbility : GridEntityAbility
{
    [BoxGroup("Chase Ability Settings")]
    [Tooltip("The target Grid Entity which is also the final destination")]
    [SerializeField]
    private List<GridEntity> targetEntity = new List<GridEntity>();
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
    public override void AbilityOnAwake()
    {
        
    }

    public override void AbilityOnEnable()
    {
        
    }

    public override void AbilityStart()
    {
        
    }

    public override void AbilityUpdate()
    {
        
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
}
