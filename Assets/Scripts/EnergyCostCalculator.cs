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
        float possibleCosts = character.endurance + character.movementPoints;
        character.currentLandscapeCell.minimumMovementCosts = 0; // Присвоить путь к текущей ячейке равным нулю
        // Создаем очередь для обхода клеток в порядке их стоимости
        var queue = new Queue<(LandscapeCell, List<LandscapeCell>)>();
        var visitedCount = new Dictionary<LandscapeCell, int>(); // Создаем словарь для отслеживания количества повторных посещений
        // Устанавливаем начальную клетку
      //  startCell.UpdateCost(0, new LandscapeCell[] { startCell });

        queue.Enqueue((startCell, new List<LandscapeCell>()));
        // Пока очередь не пуста
        while(queue.Count > 0)
        {
            (LandscapeCell vertex, List<LandscapeCell> path) = queue.Dequeue();
            // Проходим по соседним клеткам
            foreach(var neighbor in vertex.adjacentLandscapeCellsInAStraightLine)
            {
                // Рассчитываем стоимость перехода к соседней клетке
                float newCost = CalculateCost(vertex, neighbor, 5);
                // Если записанная минимальная стоимость к вершине больше чем минимальная стоимость прохода к текущей ячейке плюс переход от нее к вершине и планируемые расходы не выше того, что имеем
                float plannedСalculation = vertex.minimumMovementCosts + newCost;

                if(neighbor.minimumMovementCosts > plannedСalculation && neighbor.minimumMovementCosts != 0 && possibleCosts - plannedСalculation >= 0 && neighbor.landscapeSO.surmountable)
                {
                    List<LandscapeCell> localPath = new List<LandscapeCell>();
                    localPath.AddRange(vertex.shortestPath);
                    localPath.Add(vertex);
                    path.Add(neighbor);
                    // Обновляем стоимость и путь к ячейке
                    neighbor.UpdateCost(plannedСalculation, localPath);
                    // Добавляем ячейку с путем к ней в очередь
                    queue.Enqueue((neighbor, path));
                }
            }
            foreach(var neighbor in vertex.adjacentLandscapeCellsDiagonally)
            {
                // Рассчитываем стоимость перехода к соседней клетке
                float newCost = CalculateCost(vertex, neighbor, Mathf.Sqrt(50));
                // Если записанная минимальная стоимость к вершине больше чем минимальная стоимость прохода к текущей ячейке плюс переход от нее к вершине и планируемые расходы не выше того, что имеем
                float plannedСalculation = vertex.minimumMovementCosts + newCost;
                if(neighbor.minimumMovementCosts > plannedСalculation && neighbor.minimumMovementCosts != 0 && possibleCosts - plannedСalculation >= 0 && neighbor.landscapeSO.surmountable)
                {
                    List<LandscapeCell> localPath = new List<LandscapeCell>();
                    localPath.AddRange(vertex.shortestPath);
                    localPath.Add(vertex);
                    path.Add(neighbor);
                    // Обновляем стоимость и путь к ячейке
                    neighbor.UpdateCost(plannedСalculation, localPath);
                    // Добавляем ячейку с путем к ней в очередь
                    queue.Enqueue((neighbor, path));
                }
            }
        }
    }

// Метод для расчета стоимости перехода между клетками
    private float CalculateCost (LandscapeCell from, LandscapeCell to, float pathLength)
    {
        // Реализация расчета стоимости перехода
        float result = (30 / character.speed) * pathLength;
        return result; // Замените на свою реализацию
    }
}

