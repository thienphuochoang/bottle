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
        public static int CalculateDistanceCost(GridTile a, GridTile b)
        {
            int xDistance = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
            int yDistance = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
            int result = Mathf.Abs(xDistance - yDistance);
            return result;
        }
        public static List<GridTile> FindingPath(GridTile startNode, GridTile endNode)
        {
            List<GridTile> activeTiles = new List<GridTile>();
            List<GridTile> visitedTiles = new List<GridTile>();
            List<GridTile> finalPath = new List<GridTile>();
            activeTiles.Add(startNode);
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
                    return finalPath;
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
                        neighbourNode.cameFromGridTile = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);

                        if (!activeTiles.Contains(neighbourNode))
                        {
                            activeTiles.Add(neighbourNode);
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

