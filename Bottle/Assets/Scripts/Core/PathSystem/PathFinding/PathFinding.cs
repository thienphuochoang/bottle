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
        private static int CalculateDistanceCost(GridTile a, GridTile b)
        {
            int xDistance = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
            int yDistance = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return remaining;
        }
        public static void FindingPath(PathFindingGridTile startNode, PathFindingGridTile endNode)
        {
            List<PathFindingGridTile> activeTiles = new List<PathFindingGridTile>();
            List<PathFindingGridTile> visitedTiles = new List<PathFindingGridTile>();
            List<PathFindingGridTile> finalPath = new List<PathFindingGridTile>();
            activeTiles.Add(startNode);
            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode.currentGridTile, endNode.currentGridTile);
            while (activeTiles.Count > 0)
            {
                var currentNode = activeTiles.OrderByDescending(x => x.fCost).Last();
                // Path is found
                if (currentNode.currentGridTile == endNode.currentGridTile)
                {
                    finalPath.Clear();
                    finalPath.Add(endNode);
                    var previousNode = endNode;
                    while (previousNode.previousGridTile != null)
                    {
                        finalPath.Add(previousNode);
                        previousNode = previousNode.previousGridTile;
                    }
                    finalPath.Reverse();
                    //foreach (var eachTile in finalPath)
                    //{
                    //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //    cube.transform.position = new Vector3(eachTile.transform.position.x, eachTile.transform.position.y + 2, eachTile.transform.position.z);
                    //}
                    //allGridEntitiesInView.RemoveAt(0);
                    return;
                }
                visitedTiles.Add(currentNode);
                activeTiles.Remove(currentNode);
                foreach (var neighbourNode in GridManager.Instance.GetNeighbourGridTiles(currentNode.currentGridTile))
                {
                    if (visitedTiles.Any(pathGridTile => pathGridTile.currentGridTile == neighbourNode))
                        continue;

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode.currentGridTile, neighbourNode);
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
    }
}

