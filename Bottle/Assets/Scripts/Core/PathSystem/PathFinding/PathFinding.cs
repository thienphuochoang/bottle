using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using System.Linq;
using Bottle.Core.Manager;
namespace Bottle.Core.PathSystem
{
    public class PathFinding
    {
        public static List<Vector2Int> offsetGridPositionValueList = new List<Vector2Int>() { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
        public static List<int> offsetGridHeightValueList = new List<int>() { 1, 0, -1 };
        public static int CalculateDistanceCost(GridTile a, GridTile b)
        {
            int xDistance = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
            int yDistance = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
            int heightDistance = (int)Mathf.Abs(a.gridHeight - b.gridHeight);
            int result = Mathf.Abs(xDistance - yDistance);
            return result * 10 + heightDistance * 15;
        }
        public static List<GridTile> FindingPath(GridTile startNode, GridTile endNode)
        {
            List<GridTile> activeTiles = new List<GridTile>();
            List<GridTile> visitedTiles = new List<GridTile>();
            List<GridTile> finalPath = new List<GridTile>();
            activeTiles.Add(startNode);
            bool isFromDownToUpperHeight = startNode.gridHeight - endNode.gridHeight > 0 ? false : true;
            while (activeTiles.Count > 0)
            {
                var currentNode = activeTiles.OrderByDescending(x => x.fCost).Last();
                // Path is found
                if (currentNode == endNode)
                {
                    finalPath.Clear();
                    finalPath.Add(endNode);
                    var currentTile = endNode;
                    while (currentTile.cameFromGridTile != null)
                    {
                        finalPath.Add(currentTile.cameFromGridTile);
                        currentTile = currentTile.cameFromGridTile;
                    }
                    finalPath.Reverse();
                    foreach (GridTile tile in finalPath)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 2, tile.transform.position.z);
                    }
                    return finalPath;
                }
                visitedTiles.Add(currentNode);
                activeTiles.Remove(currentNode);
                foreach (var neighbourNode in GridManager.Instance.GetNeighbourGridTiles(currentNode, offsetGridPositionValueList, offsetGridHeightValueList, isFromDownToUpperHeight))
                {
                    if (visitedTiles.Contains(neighbourNode))
                        continue;

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        if (isFromDownToUpperHeight == true)
                        {
                            neighbourNode.cameFromGridTile = currentNode;
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);

                            if (!activeTiles.Contains(neighbourNode))
                            {
                                activeTiles.Add(neighbourNode);
                            }
                        }
                        else
                        {
                            if (neighbourNode.cameFromGridTile != null)
                            {
                            }
                            else
                            {
                                neighbourNode.cameFromGridTile = currentNode;
                            }
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);

                            if (!activeTiles.Contains(neighbourNode))
                            {
                                activeTiles.Add(neighbourNode);
                            }
                        }
                    }
                }
            }
            Debug.Log("No Path Found!");
            return null;
        }
        public static void ResetDistanceCost(GridTile tile)
        {
            tile.gCost = int.MaxValue;
            tile.hCost = 0;
            tile.cameFromGridTile = null;
        }
    }
}

