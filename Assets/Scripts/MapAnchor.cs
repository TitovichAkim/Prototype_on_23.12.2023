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
    public LevelRedactor levelRedactor;

    public void ApplyMapParameters()
    {
        landscapeSOs = new LandscapeSO[horizontalNumer][];
        for (int i = 0; i < horizontalNumer; i++)
        {
            landscapeSOs[i] = new LandscapeSO[verticalNumber];
            for(int j = 0; j < verticalNumber; j++)
            {
                CreateCell(i, j);
            }
        }
    }
    private void CreateCell (int x, int y)
    {
        LandscapeCell cell = Instantiate(landscapeCellPrefab, mapAnchorGO.transform).GetComponent<LandscapeCell>();
        cell.gameObject.transform.localPosition = new Vector2(x, y);
        cell.levelRedactor = levelRedactor;
        cell.landscapeSO = landscapeSOs[x][y];
    }
}
