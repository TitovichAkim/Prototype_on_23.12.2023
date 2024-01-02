using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class CharacterNavigation:MonoBehaviour
{
    [Header("SetInInspector")]
    public Character character;

    //public void thisBFS (LandscapeCell cell)
    //{
    //    BFS(cell, 30, 30, 30);
    //}
    //public void BFS (LandscapeCell start, float movementPoints, float endurance, float speed)
    //{
    //    HashSet<LandscapeCell> visited = new HashSet<LandscapeCell>();
    //    Queue<(LandscapeCell, List<LandscapeCell>)> queue = new Queue<(LandscapeCell, List<LandscapeCell>)>();
    //    queue.Enqueue((start, new List<LandscapeCell>()));
    //    while(movementPoints + endurance > 0) // Пока выносливjсть вместе с очками передвижения больше нуля
    //    {
    //        (LandscapeCell vertex, List<LandscapeCell> path) = queue.Dequeue();
    //        //if(vertex == end)
    //        //{
    //        //    path.Add(vertex);
    //        //    return (path);
    //        //}
    //        Debug.Log(movementPoints);
    //        float passageAbility = 30 / speed; // Сколько заберет очков один фунт
    //        float currentCost = 5 * passageAbility;
    //        if(endurance >= currentCost)
    //        {
    //            visited.Add(vertex);
    //            foreach(LandscapeCell neighbour in vertex.adjacentLandscapeCellsInAStraightLine)
    //            {
    //                if(!visited.Contains(neighbour))
    //                {

    //                    if(endurance - currentCost >= 0)
    //                    {
    //                        if(movementPoints - currentCost >= 0)
    //                        {
    //                            movementPoints -= currentCost;
    //                            vertex.costOfThePath = 30 - movementPoints;
    //                            // Раскасить ячейку в одну подсветку
    //                        }
    //                        else
    //                        {
    //                            endurance -= currentCost;
    //                            vertex.costOfThePath = 30 - movementPoints;
    //                            // Раскасить ячейку в другую подсветку
    //                        }
    //                        List<LandscapeCell> newPath = new List<LandscapeCell>(path);
    //                        newPath.Add(vertex);
    //                        queue.Enqueue((neighbour, newPath));
    //                        vertex.shortestPath = newPath;
    //                    }
    //                }
    //            }
    //            currentCost = Mathf.Sqrt(50) * passageAbility;
    //            Debug.Log(currentCost + " карент кост для 7,07");
    //            foreach(LandscapeCell neighbour in vertex.adjacentLandscapeCellsDiagonally)
    //            {
    //                if(!visited.Contains(neighbour))
    //                {

    //                    if(endurance - currentCost >= 0)
    //                    {
    //                        if(movementPoints - currentCost >= 0)
    //                        {
    //                            movementPoints -= currentCost;
    //                            vertex.costOfThePath = 30 - movementPoints;
    //                            // Раскасить ячейку в одну подсветку
    //                        }
    //                        else
    //                        {
    //                            endurance -= currentCost;
    //                            vertex.costOfThePath = 30 - movementPoints;
    //                            // Раскасить ячейку в другую подсветку
    //                        }
    //                        List<LandscapeCell> newPath = new List<LandscapeCell>(path);
    //                        newPath.Add(vertex);
    //                        queue.Enqueue((neighbour, newPath));
    //                        vertex.shortestPath = newPath;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
