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
        public static List<PathFindingGridTile> FindingPath(PathFindingGridTile startNode, PathFindingGridTile endNode)
        {
            List<PathFindingGridTile> activeTiles = new List<PathFindingGridTile>();
            List<PathFindingGridTile> visitedTiles = new List<PathFindingGridTile>();
            List<PathFindingGridTile> finalPath = new List<PathFindingGridTile>();
            activeTiles.Add(startNode);
            while (activeTiles.Count > 0)
            {
                var currentNode = activeTiles.OrderByDescending(x => x.fCost).Last();
                // Path is found
                if (currentNode.currentGridTile.gridPosition == endNode.currentGridTile.gridPosition)
                {
                    finalPath.Clear();
                    finalPath.Add(endNode);
                    var previousNode = endNode;
                    //while (previousNode.previousGridTile != null)
                    //{
                    //    finalPath.Add(previousNode);
                    //    previousNode = previousNode.previousGridTile;
                    //}
                    finalPath.Reverse();
                    //foreach (var eachTile in finalPath)
                    //{
                    //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //    cube.transform.position = new Vector3(eachTile.transform.position.x, eachTile.transform.position.y + 2, eachTile.transform.position.z);
                    //}
                    //allGridEntitiesInView.RemoveAt(0);
                    return finalPath;
                }
                visitedTiles.Add(currentNode);
                activeTiles.Remove(currentNode);
                foreach (var neighbourNode in GridManager.Instance.GetNeighbourGridTiles(currentNode))
                {
                    if (visitedTiles.Contains(neighbourNode))
                        continue;

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode.currentGridTile, neighbourNode.currentGridTile);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.previousGridTile = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode.currentGridTile, endNode.currentGridTile);

                        if (!activeTiles.Contains(neighbourNode))
                        {
                            activeTiles.Add(neighbourNode);
                        }
                    }
                }
            }
            Debug.Log("No Path Found!");
            return finalPath;
        }
    }
}

