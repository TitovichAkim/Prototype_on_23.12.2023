using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

// �������� ������ ��� ������� ��������� �������
public class EnergyCostCalculator:MonoBehaviour
{
    [Header("SetInInspector")]
    public Character character;


    // ����� ��� ������� ��������� ������� � �������
    public void CalculateEnergyCost (LandscapeCell startCell)
    {
        float possibleCosts = character.currentEdurance + character.movementPoints;
        character.currentLandscapeCell.minimumMovementCosts = 0; // ��������� ���� � ������� ������ ������ ����
        // ������� ������� ��� ������ ������ � ������� �� ���������
        var queue = new Queue<(LandscapeCell, List<LandscapeCell>)>();
        var visitedCount = new Dictionary<LandscapeCell, int>(); // ������� ������� ��� ������������ ���������� ��������� ���������
        // ������������� ��������� ������
      //  startCell.UpdateCost(0, new LandscapeCell[] { startCell });

        queue.Enqueue((startCell, new List<LandscapeCell>()));
        // ���� ������� �� �����
        while(queue.Count > 0)
        {
            (LandscapeCell vertex, List<LandscapeCell> path) = queue.Dequeue();
            // �������� �� �������� �������
            foreach(var neighbor in vertex.adjacentLandscapeCellsInAStraightLine)
            {
                // ������������ ��������� �������� � �������� ������
                float newCost = CalculateCost(vertex, neighbor, 5);
                float planned�alculation = vertex.minimumMovementCosts + newCost;
                // ���� � ������ ��� ������ ���������� � ��� ����������
                if(neighbor.currentCharacter == null && neighbor.landscapeSO.surmountable) 
                {
                    // ���� ���������� ����������� ��������� � ������� ������ ��� ����������� ��������� ������� � ������� ������ ���� ������� �� ��� � ������� � ����������� ������� �� ���� ����, ��� �����
                    if(neighbor.minimumMovementCosts > planned�alculation && neighbor.minimumMovementCosts != 0 && possibleCosts - planned�alculation >= 0)
                    {
                        List<LandscapeCell> localPath = new List<LandscapeCell>();
                        localPath.AddRange(vertex.shortestPath);
                        localPath.Add(neighbor);
                        path.Add(neighbor);
                        // ��������� ��������� � ���� � ������
                        // ���� ��������� ������� ������ � ������� ����� �������� 
                        if(planned�alculation <= character.movementPoints)
                        {
                            neighbor.UpdateCost(planned�alculation, localPath, LandscapeCell.CellState.EnoughPoints); // ��������� ������ = ���������� ����� ��������
                        }
                        else
                        {
                            neighbor.UpdateCost(planned�alculation, localPath, LandscapeCell.CellState.EnoughStamina);
                        }
                        // ��������� ��������� � ���� � ������
                        // ��������� ������ � ����� � ��� � �������
                        queue.Enqueue((neighbor, path));
                    }
                }
            }
            foreach(var neighbor in vertex.adjacentLandscapeCellsDiagonally)
            {
                // ������������ ��������� �������� � �������� ������
                float newCost = CalculateCost(vertex, neighbor, Mathf.Sqrt(50));
                float planned�alculation = vertex.minimumMovementCosts + newCost;
                // ���� � ������ ��� ������ ���������� � ��� ����������
                if(neighbor.currentCharacter == null && neighbor.landscapeSO.surmountable)
                {
                    // ���� ���������� ����������� ��������� � ������� ������ ��� ����������� ��������� ������� � ������� ������ ���� ������� �� ��� � ������� � ����������� ������� �� ���� ����, ��� �����
                    if(neighbor.minimumMovementCosts > planned�alculation && neighbor.minimumMovementCosts != 0 && possibleCosts - planned�alculation >= 0 && neighbor.landscapeSO.surmountable)
                    {
                        List<LandscapeCell> localPath = new List<LandscapeCell>();
                        localPath.AddRange(vertex.shortestPath);
                        localPath.Add(neighbor);
                        path.Add(neighbor);
                        // ��������� ��������� � ���� � ������
                        // ���� ��������� ������� ������ � ������� ����� �������� 
                        if(planned�alculation <= character.movementPoints)
                        {
                            neighbor.UpdateCost(planned�alculation, localPath, LandscapeCell.CellState.EnoughPoints); // ��������� ������ = ���������� ����� ��������
                        }
                        else
                        {
                            neighbor.UpdateCost(planned�alculation, localPath, LandscapeCell.CellState.EnoughStamina);
                        }
                        // ��������� ������ � ����� � ��� � �������
                        queue.Enqueue((neighbor, path));
                    }
                }
            }
        }
    }

// ����� ��� ������� ��������� �������� ����� ��������
    private float CalculateCost (LandscapeCell from, LandscapeCell to, float pathLength)
    {
        // ���������� ������� ��������� ��������
        float result = (30 / character.speed) * pathLength;
        return result;
    }
}

