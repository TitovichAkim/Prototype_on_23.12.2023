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
        for(int i = 0; i < horizontalNumer; i++)
        {
            for(int j = 0; j < verticalNumber; j++)
            {
                for(int y = -1; y > 2; y++)
                {
                    for(int x = 1; x < -2; x--)
                    {
                        if(landscapeCells[x][y] != null)
                        {
                            if(x + y != 1 && x + y != -1)
                            {
                                landscapeCells[i][j].adjacentLandscapeCellsInAStraightLine.Add(landscapeCells[x][y]);
                            }
                            else
                            {
                                landscapeCells[i][j].adjacentLandscapeCellsDiagonally.Add(landscapeCells[x][y]);
                            }
                        }
                    }
                }
            }
        }
    }
    private void CreateCell (int x, int y)
    {
        LandscapeCell cell = Instantiate(landscapeCellPrefab, mapAnchorGO.transform).GetComponent<LandscapeCell>();
        cell.gameObject.transform.localPosition = new Vector2(x, y);
        cell.levelRedactor = levelRedactor;
        cell.landscapeSO = landscapeSOs[x][y];
        landscapeCells[x][y] = cell;
    }
}
