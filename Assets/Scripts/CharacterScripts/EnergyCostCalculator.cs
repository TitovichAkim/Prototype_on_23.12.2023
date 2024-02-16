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
        character.gameManager.ChangeCellsStates(LandscapeCell.CellState.Expectation);
        float possibleCosts = 0;
        if (character.currentEdurance >= 0)
        {
            possibleCosts = character.currentEdurance + character.movementPoints; // ����� �������������
        }
        else
        {
            possibleCosts = character.movementPoints; // ����� �������������
        }

        character.currentLandscapeCell.minimumMovementCosts = 0; // ��������� ���� � ������� ������ ������ ����
        // ������� ������� ��� ������ ������ � ������� �� ���������
        var queue = new Queue<(LandscapeCell, List<LandscapeCell>)>();
        var visitedCount = new Dictionary<LandscapeCell, int>(); // ������� ������� ��� ������������ ���������� ��������� ���������

        queue.Enqueue((startCell, new List<LandscapeCell>()));
        // ���� ������� �� �����
        while(queue.Count > 0)
        {
            (LandscapeCell vertex, List<LandscapeCell> path) = queue.Dequeue();
            // �������� �� �������� �������
            foreach(var neighbor in vertex.adjacentLandscapeCellsInAStraightLine)
            {
                // ������������ ��������� �������� � �������� ������
                float newCost = CalculateCost(vertex, neighbor, 5, vertex.minimumMovementCosts);
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
                float newCost = CalculateCost(vertex, neighbor, Mathf.Sqrt(50), vertex.minimumMovementCosts);
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
    private float CalculateCost (LandscapeCell from, LandscapeCell to, float pathLength, float minCost)
    {
        // ���������� ������� ��������� ��������
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

