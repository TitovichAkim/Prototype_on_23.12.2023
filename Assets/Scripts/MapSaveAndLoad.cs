using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSaveAndLoad:MonoBehaviour
{
    [Header("SetInInspector")]
    public LevelRedactor levelRedactor;
    public GameManager gameManager;
    public GameObject loadMapPanel;
    public GameObject saveButtonPrefab;
    public Toggle withCharacters;
    [Header("SetDynamically")]
    public MapAnchor mapAnchor;
    public List<GameObject> loadButtonsList = new List<GameObject>();
    private void Start ()
    {
        FillInTheSavePanel();
    }
    public void SaveMap ()
    {
        int totalSaves = PlayerPrefs.GetInt("TotalSaves");
        string[][] saveString = new string[4][];
        // 0 - ������ �����, ��� �����, ���� �����
        // 1 - �������� �����
        // 2 - ���������� �����

        string[] saveString_0 = { totalSaves.ToString(), $"Save_{totalSaves}", DateTime.Now.ToString()}; // ������� ������: ������, ���, ���� ����������
        string[] saveString_1 = { mapAnchor.horizontalNumer.ToString(), mapAnchor.verticalNumber.ToString() }; // ������� ������ � ��������� �����
        string[] saveString_2 = SetCellsParameters(mapAnchor.horizontalNumer, mapAnchor.verticalNumber); // ������� � ��������� ������ � �������� ��� ������ ������ �����
        string[] saveString_3 = SetCharactersParameters(mapAnchor.charactersList); // ������� � ��������� ������ � �����������

        // ���������� ������ � ����� ������
        saveString[0] = saveString_0;
        saveString[1] = saveString_1;
        saveString[2] = saveString_2;
        if(withCharacters.isOn)
        {
            saveString[3] = saveString_3;
        }
        FormStringForSaving(saveString, totalSaves); // ������������ ������ ��� ����������
        totalSaves++; // ����� ���������� +1
        PlayerPrefs.SetInt("TotalSaves", totalSaves); // �������� ����� ���������� ����������
    }
    public void FillInTheSavePanel ()
    {
        int totalSaves = PlayerPrefs.GetInt("TotalSaves");
        Debug.Log($"�������� {totalSaves} ����");
        for(int i = 0; i < totalSaves; i++)
        {
            string loadString = PlayerPrefs.GetString($"Save_{i}");
            if(loadString != null && loadString != "")
            {
                GameObject saveButton = Instantiate(saveButtonPrefab, loadMapPanel.transform);
                loadButtonsList.Add(saveButton);
                saveButton.GetComponentInChildren<TMP_Text>().text = $"{loadString.Split('\n')[0].Split('*')[1]}\n{loadString.Split('\n')[0].Split('*')[2]}";
                saveButton.GetComponent<Button>().onClick.AddListener(() => LoadMap(loadString));
                saveButton.GetComponent<Button>().onClick.AddListener(() => gameManager.StartLevelRedactorState(false));
                saveButton.GetComponent<Button>().onClick.AddListener(() => loadMapPanel.SetActive(false));
            }
        }
    }
    public void LoadMap (string loadString)
    {
        levelRedactor.mapAnchor = Instantiate(levelRedactor.mapAnchorPrefab).GetComponent<MapAnchor>();
        gameManager.mapAnchor = levelRedactor.mapAnchor.gameObject;
        mapAnchor = levelRedactor.mapAnchor;
        mapAnchor.levelRedactor = levelRedactor;

        string[] firstSplit = loadString.Split('\n');
        for(int i = 0; i < firstSplit.Length; i++)
        {
            string[] secondSplit = firstSplit[i].Split('*');
            for(int j = 0; j < secondSplit.Length; j++)
            {
                switch(i)
                {
                    case 1:
                        mapAnchor.horizontalNumer = int.Parse(secondSplit[0]);
                        mapAnchor.verticalNumber = int.Parse(secondSplit[1]);
                        mapAnchor.landscapeSOs = new LandscapeSO[mapAnchor.horizontalNumer][]; // ������������ ��������������� ������� ����� �����
                        for(int k = 0; k < mapAnchor.horizontalNumer; k++)// ������������ ������������� ������� ����� �����
                        {
                            mapAnchor.landscapeSOs[k] = new LandscapeSO[mapAnchor.verticalNumber];
                        }
                        break;
                    case 2:
                        if(secondSplit[j] != "")
                        {
                            GetCellsParameters(secondSplit[j], j);
                        }
                        if(j == secondSplit.Length - 1)
                        {
                            mapAnchor.levelRedactor.InstantiateMap(mapAnchor.horizontalNumer, mapAnchor.verticalNumber, false);
                        }
                        break;
                    case 3:
                        if(secondSplit[j] != "")
                        {
                            GetCharactersParameters(secondSplit[j], j);
                        }
                        break;
                }
            }
        }
    }
    #region ������ ��� ����������
    public string[] SetCellsParameters (int horizontalNumer, int verticalNumber)
    {
        string[] stringParameters = new string[horizontalNumer * verticalNumber];
        for(int i = 0; i < horizontalNumer; i++)
        {
            for(int j = 0; j < verticalNumber; j++)
            {
                if(mapAnchor.landscapeCells[i][j].landscapeSO != null)
                {
                    stringParameters[(i * verticalNumber) + j] = mapAnchor.landscapeCells[i][j].landscapeSO.landscapeIndex.ToString();
                }
            }
        }
        return stringParameters;
    }
    public string[] SetCharactersParameters (List<Character> characters)
    {
        string[] stringParameters = new string[characters.Count];
        for(int i = 0; i < characters.Count; i++)
        {
            string parameters = "";
            parameters += characters[i].characterSO.characterIndex + "^";
            parameters += characters[i].currentLandscapeCell.coordinates.x + "^";
            parameters += characters[i].currentLandscapeCell.coordinates.y + "^";
            parameters += characters[i].teamNumber;
            stringParameters[i] = parameters;
        }
        return stringParameters;
    }
    public void FormStringForSaving (string[][] saveString, int saveIndex)
    {
        string theSavedString = "";
        for(int i = 0; i < saveString.Length; i++)
        {
            if(saveString[i] != null)
            {
                for(int j = 0; j < saveString[i].Length; j++)
                {
                    theSavedString += saveString[i][j] + "*";
                    Debug.Log(theSavedString);
                }

            }

            if(i < saveString.Length - 1)
            {
                theSavedString += "\n";
            }
        }
        PlayerPrefs.SetString($"Save_{saveIndex}", theSavedString);
    }

    #endregion

    #region ������ ��� ��������
    public void GetCellsParameters (string cellIndex, int generalIndex)
    {

        int horizontalIndex = generalIndex / mapAnchor.verticalNumber;
        int verticalIndex;
        if(horizontalIndex == 0)
        {
            verticalIndex = generalIndex;
        }
        else
        {
            verticalIndex = generalIndex % mapAnchor.verticalNumber;
        }
        Debug.Log($"���������� x = {horizontalIndex}, y = {verticalIndex}, ������ = {cellIndex}");
        mapAnchor.landscapeSOs[horizontalIndex][verticalIndex] = levelRedactor.levelItemsPanel.setOfLevelEditor.landscapeSOs[int.Parse(cellIndex)];
    }
    public void GetCharactersParameters (string characterParameters, int generalIndex)
    {
        string[] characterParam = characterParameters.Split('^');
        int characterIndex = int.Parse(characterParam[0]);
        int coordinateX = int.Parse(characterParam[1]);
        int coordinateY = int.Parse(characterParam[2]);
        int teamNumber = int.Parse(characterParam[3]);
        mapAnchor.landscapeCells[coordinateX][coordinateY].SetCharacterItem(characterIndex, teamNumber);
    }
    #endregion

    public void DeleteSaveMaps ()
    {
        PlayerPrefs.DeleteAll();
        foreach(GameObject button in loadButtonsList)
        {
            Destroy(button);
        }
        loadButtonsList.Clear();
    }
}
