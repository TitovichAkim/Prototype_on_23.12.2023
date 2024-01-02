using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSaveAndLoad : MonoBehaviour
{
    [Header("SetInInspector")]
    public LevelRedactor levelRedactor;
    [Header("SetDynamically")]
    public MapAnchor mapAnchor;
    public void SaveMap ()
    {
        string[][] saveString = new string[2][];    // 0 - �������� �����
                                                    // 1 - ���������� �����

        string[] saveString_0 = { mapAnchor.horizontalNumer.ToString(), mapAnchor.verticalNumber.ToString() }; // ������� ������ � ��������� �����
        string[] saveString_1 = SetCellsParameters(mapAnchor.horizontalNumer, mapAnchor.verticalNumber); // ������� � ��������� ������ � �������� ��� ������ ������ �����

        // ���������� ������ � ����� ������
        saveString[0] = saveString_0;
        saveString[1] = saveString_1;

        FormStringForSaving(saveString); // ������������ ������ ��� ����������
    }
    public void LoadMap ()
    {
    
    }
    public string[] SetCellsParameters (int horizontalNumer, int verticalNumber)
    {
        string[] stringParameters = new string[horizontalNumer * verticalNumber];
        for (int i = 0; i < horizontalNumer; i++)
        {
            for (int j = 0; j < verticalNumber; j++)
            {
                if(mapAnchor.landscapeCells[i][j].landscapeSO != null)
                {
                    stringParameters[(i * verticalNumber) +  j] = mapAnchor.landscapeCells[i][j].landscapeSO.landscapeIndex.ToString();
                }
            }
        }
        return stringParameters;
    }
    public void FormStringForSaving (string[][] saveString)
    {
        string theSavedString = "";
        for(int i = 0; i < saveString.Length; i++)
        {
            for(int j = 0; j < saveString[i].Length; j++)
            {
                theSavedString += saveString[i][j] + "*";
                Debug.Log(theSavedString);
            }
            
            if (i < saveString.Length - 1) 
            { 
                theSavedString += "\n"; 
            }
        }
    }
}
