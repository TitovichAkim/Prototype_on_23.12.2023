using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class MapAnchor:MonoBehaviour
{
    [Header("SetInInspector")]
    public GameObject mapAnchorGO;
    public GameObject landscapeCellPrefab;

    [Header("SetDynamically")]
    public int horizontalNumer;
    public int verticalNumber;
    public LandscapeSO[][] landscapeSOs;
    public LandscapeCell[][] landscapeCells;
    public LevelRedactor levelRedactor;

    public void ApplyMapParameters()
    {
        landscapeSOs = new LandscapeSO[horizontalNumer][];
        landscapeCells = new LandscapeCell[horizontalNumer][];
        for (int i = 0; i < horizontalNumer; i++)
        {
            landscapeSOs[i] = new LandscapeSO[verticalNumber];
            landscapeCells[i] = new LandscapeCell[verticalNumber];
            for(int j = 0; j < verticalNumber; j++)
            {
                CreateCell(i, j);
            }
        }
        for(int i = 0; i < horizontalNumer; i++) // Пройти все строки
        {
            for(int j = 0; j < verticalNumber; j++) // Пройти каждую горизонталь 
            {
                for(int y = -1; y < 2; y++) // Пройти все возможные варианты y
                {
                    for(int x = 1; x > -2; x--) // Пройти все возможные варианты x
                    {
                        if (i + x >= 0 && j + y >= 0 && i + x < horizontalNumer && j + y < verticalNumber) // Если i + x и j + y соответствую величине массивов
                        {
                            if(landscapeCells[i + x][j + y] != null && landscapeCells[i + x][j + y] != landscapeCells[i][j]) // Если ячейка, находящаяяся в координатах, отличающихся от этой ячейки на x по x и на y по y существует и не равна самой себе
                            {
                                if(x + y == 1 || x + y == -1) // Если сумма x и y равна единице или -1 
                                {
                                    landscapeCells[i][j].adjacentLandscapeCellsInAStraightLine.Add(landscapeCells[i + x][j + y]); // Добавить в список прямых соединений
                                }
                                else
                                {
                                    landscapeCells[i][j].adjacentLandscapeCellsDiagonally.Add(landscapeCells[i + x][j + y]); // Добавить в список диагональных соединений
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void CreateCell (int x, int y)
    {
        GameObject cellGO;
        cellGO = Instantiate(landscapeCellPrefab, mapAnchorGO.transform);
        LandscapeCell cell = cellGO.GetComponent<LandscapeCell>();
        cell.gameObject.transform.localPosition = new Vector2(x, y);
        cell.levelRedactor = levelRedactor;
        cell.landscapeSO = landscapeSOs[x][y];
        cell.coordinates = new Vector2(x, y);
        landscapeCells[x][y] = cell;
    }
}
