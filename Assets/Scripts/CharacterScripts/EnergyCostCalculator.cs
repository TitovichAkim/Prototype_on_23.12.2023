using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

// Основной скрипт для расчета стоимости прохода
public class EnergyCostCalculator:MonoBehaviour
{
    [Header("SetInInspector")]
    public Character character;

    // Метод для расчета стоимости прохода к клеткам
    public void CalculateEnergyCost (LandscapeCell startCell)
    {
        character.gameManager.ChangeCellsStates(LandscapeCell.CellState.Expectation);
        float possibleCosts = 0;
        if (character.currentEdurance >= 0)
        {
            possibleCosts = character.currentEdurance + character.movementPoints; // Всего доступноходов
        }
        else
        {
            possibleCosts = character.movementPoints; // Всего доступноходов
        }

        character.currentLandscapeCell.minimumMovementCosts = 0; // Присвоить путь к текущей ячейке равным нулю
        // Создаем очередь для обхода клеток в порядке их стоимости
        var queue = new Queue<(LandscapeCell, List<LandscapeCell>)>();
        var visitedCount = new Dictionary<LandscapeCell, int>(); // Создаем словарь для отслеживания количества повторных посещений

        queue.Enqueue((startCell, new List<LandscapeCell>()));
        // Пока очередь не пуста
        while(queue.Count > 0)
        {
            (LandscapeCell vertex, List<LandscapeCell> path) = queue.Dequeue();
            // Проходим по соседним клеткам
            foreach(var neighbor in vertex.adjacentLandscapeCellsInAStraightLine)
            {
                // Рассчитываем стоимость перехода к соседней клетке
                float newCost = CalculateCost(vertex, neighbor, 5, vertex.minimumMovementCosts);
                float plannedСalculation = vertex.minimumMovementCosts + newCost;
                // Если в ячейке нет других персонажей и она проходимая
                if(neighbor.currentCharacter == null && neighbor.landscapeSO.surmountable) 
                {
                    // Если записанная минимальная стоимость к вершине больше чем минимальная стоимость прохода к текущей ячейке плюс переход от нее к вершине и планируемые расходы не выше того, что имеем
                    if(neighbor.minimumMovementCosts > plannedСalculation && neighbor.minimumMovementCosts != 0 && possibleCosts - plannedСalculation >= 0)
                    {
                        List<LandscapeCell> localPath = new List<LandscapeCell>();
                        localPath.AddRange(vertex.shortestPath);
                        localPath.Add(neighbor);
                        path.Add(neighbor);
                        // Обновляем стоимость и путь к ячейке
                        // Если стоимость прохода входит в пределы очков перехода 
                        if(plannedСalculation <= character.movementPoints)
                        {
                            neighbor.UpdateCost(plannedСalculation, localPath, LandscapeCell.CellState.EnoughPoints); // Состояние ячейки = достаточно очков перехода
                        }
                        else
                        {
                            neighbor.UpdateCost(plannedСalculation, localPath, LandscapeCell.CellState.EnoughStamina);
                        }
                        // Обновляем стоимость и путь к ячейке
                        // Добавляем ячейку с путем к ней в очередь
                        queue.Enqueue((neighbor, path));
                    }
                }
            }
            foreach(var neighbor in vertex.adjacentLandscapeCellsDiagonally)
            {
                // Рассчитываем стоимость перехода к соседней клетке
                float newCost = CalculateCost(vertex, neighbor, Mathf.Sqrt(50), vertex.minimumMovementCosts);
                float plannedСalculation = vertex.minimumMovementCosts + newCost;
                // Если в ячейке нет других персонажей и она проходимая
                if(neighbor.currentCharacter == null && neighbor.landscapeSO.surmountable)
                {
                    // Если записанная минимальная стоимость к вершине больше чем минимальная стоимость прохода к текущей ячейке плюс переход от нее к вершине и планируемые расходы не выше того, что имеем
                    if(neighbor.minimumMovementCosts > plannedСalculation && neighbor.minimumMovementCosts != 0 && possibleCosts - plannedСalculation >= 0 && neighbor.landscapeSO.surmountable)
                    {
                        List<LandscapeCell> localPath = new List<LandscapeCell>();
                        localPath.AddRange(vertex.shortestPath);
                        localPath.Add(neighbor);
                        path.Add(neighbor);
                        // Обновляем стоимость и путь к ячейке
                        // Если стоимость прохода входит в пределы очков перехода 
                        if(plannedСalculation <= character.movementPoints)
                        {
                            neighbor.UpdateCost(plannedСalculation, localPath, LandscapeCell.CellState.EnoughPoints); // Состояние ячейки = достаточно очков перехода
                        }
                        else
                        {
                            neighbor.UpdateCost(plannedСalculation, localPath, LandscapeCell.CellState.EnoughStamina);
                        }
                        // Добавляем ячейку с путем к ней в очередь
                        queue.Enqueue((neighbor, path));
                    }
                }
            }
        }
    }

// Метод для расчета стоимости перехода между клетками
    private float CalculateCost (LandscapeCell from, LandscapeCell to, float pathLength, float minCost)
    {
        // Реализация расчета стоимости перехода
        float result = (30 / RealSpeed(minCost)) * pathLength;
        return result;
    }
    private float RealSpeed (float resourcesSpent)
    {
        float speed = character.standardSpeed;
        Vector3 slowling = character.superimposedEffects.slowingDownFromEndurance;
        List<Vector3> booster = new List<Vector3>();
        List<Vector3> slowlingList = new List<Vector3>();
        booster.AddRange(character.superimposedEffects.boost);
        slowlingList.AddRange(character.superimposedEffects.slowingDownInCycles);
        float result;
        float slow = 0;
        float slowL = 0;
        float boost = 0;
        if(slowling.y > resourcesSpent)
        {
            slow = speed * slowling.x;
        }
        if (booster.Count > 0)
        {
            for (int i = 0; i < booster.Count; i++)
            {
                if (booster[i].y > resourcesSpent)
                {
                    boost += speed * booster[i].x;
                }
            }
        }
        if(slowlingList.Count > 0)
        {
            for(int i = 0; i < slowlingList.Count; i++)
            {
                if(slowlingList[i].y > 0)
                {
                    slowL += speed * slowlingList[i].x;
                }
            }
        }
        result = speed - slow - slowL + boost;
        return result;
    }
}

