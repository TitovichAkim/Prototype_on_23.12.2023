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
    public GameObject mainMenu;
    public GameObject loadPanel;
    public GameObject saveButtonPrefab;
    public Toggle withCharacters;
    [Header("SetDynamically")]
    public MapAnchor mapAnchor;
    public List<GameObject> loadButtonsList = new List<GameObject>();
    private void Start ()
    {
        FillInTheSavePanel();
    }
    private void Update ()
    {
        if(loadPanel.activeSelf || gameManager.sizeAdjustmentPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                mainMenu.SetActive((loadPanel.activeSelf || gameManager.sizeAdjustmentPanel.activeSelf));
                loadPanel.SetActive(!mainMenu.activeSelf);
                gameManager.sizeAdjustmentPanel.SetActive(!mainMenu.activeSelf);
                GameManager.currentGameState = GameManager.GameState.MainMenu;
            }
        }
    }
    public void SaveMap ()
    {
        int totalSaves = PlayerPrefs.GetInt("TotalSaves");
        string[][] saveString = new string[4][];
        // 0 - Индекс карты, имя карты, дата карты
        // 1 - Величина карты
        // 2 - Параментры ячеек

        string[] saveString_0 = { totalSaves.ToString(), $"Save_{totalSaves}", DateTime.Now.ToString()}; // Создать массив: индекс, имя, дата сохранения
        string[] saveString_1 = { mapAnchor.horizontalNumer.ToString(), mapAnchor.verticalNumber.ToString() }; // Создать массив с величиной карты
        string[] saveString_2 = SetCellsParameters(mapAnchor.horizontalNumer, mapAnchor.verticalNumber); // Создать и заполнить массив с наборами для каждой ячейки карты
        string[] saveString_3 = SetCharactersParameters(mapAnchor.charactersList); // Создать и заполнить массив с персонажами

        // Произвести запись в общий массив
        saveString[0] = saveString_0;
        saveString[1] = saveString_1;
        saveString[2] = saveString_2;
        if(withCharacters.isOn)
        {
            saveString[3] = saveString_3;
        }
        FormStringForSaving(saveString, totalSaves); // Сформировать строку для сохранения
        totalSaves++; // Всего сохранений +1
        PlayerPrefs.SetInt("TotalSaves", totalSaves); // Записать новое количество сохранений
    }
    public void FillInTheSavePanel ()
    {
        foreach(GameObject butt in loadButtonsList)
        {
            Destroy(butt);
        }
        loadButtonsList.Clear();
        int totalSaves = PlayerPrefs.GetInt("TotalSaves");
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
                saveButton.GetComponent<Button>().onClick.AddListener(() => loadPanel.SetActive(false));
            }
        }
    }
    public void LoadMap (string loadString)
    {
        levelRedactor.mapAnchor = Instantiate(levelRedactor.mapAnchorPrefab).GetComponent<MapAnchor>();
        gameManager.mapAnchor = levelRedactor.mapAnchor.gameObject;
        gameManager.mapAnchorScr = mapAnchor;
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
                        mapAnchor.landscapeSOs = new LandscapeSO[mapAnchor.horizontalNumer][]; // Инициаизация горизонтального массива сетов ячеек
                        for(int k = 0; k < mapAnchor.horizontalNumer; k++)// Инициаизация вертикального массива сетов ячеек
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
    #region Методы для сохранения
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
            parameters += characters[i].characterSO.characterIndex + "^"; // 0
            parameters += characters[i].currentLandscapeCell.coordinates.x + "^"; // 1
            parameters += characters[i].currentLandscapeCell.coordinates.y + "^"; // 2
            parameters += characters[i].teamNumber + "^"; // 3
            parameters += characters[i].characterName + "^"; // 4
            parameters += characters[i].endurance + "^"; // 5
            parameters += characters[i].currentEdurance + "^"; // 6
            parameters += characters[i].health + "^"; // 7
            parameters += characters[i].currentHealth + "^"; // 8
            parameters += characters[i].mana + "^"; // 9
            parameters += characters[i].currentMana + "^"; // 10
            parameters += characters[i].speed + "^"; // 11
            parameters += characters[i].movementPoints + "^"; // 12
            parameters += characters[i].attackPower + "^"; // 13
            parameters += characters[i].attackRange + "^"; // 14
            parameters += characters[i].initiative; // 15
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

    #region Методы для загрузки
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
        Character character = mapAnchor.landscapeCells[coordinateX][coordinateY].currentCharacter;
        character.characterName = characterParam[4];
        character.endurance = float.Parse(characterParam[5]);
        character.currentEdurance = float.Parse(characterParam[6]);
        character.health = float.Parse(characterParam[7]);
        character.currentHealth = float.Parse(characterParam[8]);
        character.mana = float.Parse(characterParam[9]);
        character.currentMana = float.Parse(characterParam[10]);
        character.speed = float.Parse(characterParam[11]);
        character.movementPoints = float.Parse(characterParam[12]);
        character.attackPower = float.Parse(characterParam[13]);
        character.attackRange = float.Parse(characterParam[14]);
        character.initiative = float.Parse(characterParam[15]); ;
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
