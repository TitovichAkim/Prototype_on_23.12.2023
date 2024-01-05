using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.Search;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu = 0,
        LevelRedactor = 1,
        Game = 2
    }
    [Header("SetInInspector")]
    public static GameManager gameManager;
    public GameInterFace gameInterFace;
    public GameObject mainMenuCanvas;
    public GameObject temporaryMenuPanel;
    public GameObject redactorMenuCanvas;
    public GameObject sizeAdjustmentPanel;
    public GameObject levelItemsPanel;
    [Header("SetDynamically")]
    public GameObject mapAnchor;
    public List<Character> queueOfCharacters = new List<Character>();
    [SerializeField]private Character _currentCharacter;
    private MapAnchor mapAnchorScr;


    [SerializeField]private static GameState _currentGameState;
    private bool _newMap;
    public static GameState currentGameState
    {
        get
        {
            return (_currentGameState);
        }
        set
        {
            GameState oldState = _currentGameState;
            _currentGameState = value;
            gameManager.ChangeTheGameMode(currentGameState, oldState);
        }
    }
    public Character currentCharacter
    {
        get
        {
            return (_currentCharacter);
        }
        set
        {
            _currentCharacter = value;
            _currentCharacter.activeBackground.SetActive(true);
        }
    }
    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveTheTurnQueue();
        }
    }
    public void Start ()
    {
        gameManager = this;
        currentGameState = GameState.MainMenu;
    }
    public void ChangeTheGameMode (GameState newGameState, GameState oldGameState)
    {
        switch(newGameState)
        {
            case GameState.MainMenu:
                mainMenuCanvas.SetActive(true);
                temporaryMenuPanel.SetActive(true);
                redactorMenuCanvas.SetActive(false);
                if (mapAnchor != null)
                {
                    foreach(Character charcater in mapAnchor.GetComponent<MapAnchor>().charactersList)
                    {
                        Destroy(charcater.gameObject);
                    }
                    mapAnchor.GetComponent<MapAnchor>().charactersList.Clear();
                    Destroy(mapAnchor);
                }
                break;
            case GameState.LevelRedactor:
                mainMenuCanvas.SetActive(false);
                redactorMenuCanvas.SetActive(true);
                sizeAdjustmentPanel.SetActive(_newMap);
                levelItemsPanel.SetActive(!_newMap);
                break;
            case GameState.Game:
                mainMenuCanvas.SetActive(false);
                redactorMenuCanvas.SetActive(false);
                gameInterFace.gameObject.SetActive(true);
                gameInterFace.mapAnchor = mapAnchor.GetComponent<MapAnchor>();
                mapAnchorScr = mapAnchor.GetComponent<MapAnchor>();
                SortTheQueue();
                break;
        }
    }
    public void StartMainMenu ()
    {
        currentGameState = GameState.MainMenu;
    }
    public void StartLevelRedactorState (bool newMap)
    {
        _newMap = newMap;
        currentGameState = GameState.LevelRedactor;
    }
    public void StartGameState ()
    {
        currentGameState = GameState.Game;
    }

    public void SortTheQueue ()
    {
        Debug.Log("�������� ����������");
        MapAnchor mapAnchorScr = mapAnchor.GetComponent<MapAnchor>();   // �������� ������ �� ����� MapAnchor
        List<Character> temporaryList = new List<Character>();
        temporaryList.AddRange(mapAnchorScr.charactersList); // �������� ������ ���� ����������
        Debug.Log("���������� � ���������� = " + temporaryList.Count);

        mapAnchorScr.charactersList.Clear(); // ������� ������ ���������� � ����� �����
        // ������� ���������� �������
        for (int i = 0; i < temporaryList.Count; i++)
        {
            int randomNumber = Random.Range(0, temporaryList.Count); // ����� ��������� �������� �� 0 �� ���������� ���������� � ��������� ������
            mapAnchorScr.charactersList.Add(temporaryList[randomNumber]); // ��������� ���������� ��������� � ������ �� ����� �����
            Debug.Log($"������ � ������ ��������� { mapAnchorScr.charactersList}");
            temporaryList.RemoveAt(randomNumber); // ������� ��������� ��������� �� ���������� ������
        }
        temporaryList.AddRange(mapAnchorScr.charactersList); // ��������� ��������� ������ ��� ����������� � ������� ������� � �����
        mapAnchorScr.charactersList.Clear(); // ����� ������� ������, ������� ��������� �� �����
        List<Character> sortedList = new List<Character>(); // ������� ������������� ������
        while(temporaryList.Count > 0)
        {
            Character minInitiativeCharacter = temporaryList[0]; // �������� � ����������� �����������
            float minInitiative = minInitiativeCharacter.initiative; // ����������� ����������, ���������� ����� �� � �������� ���������
            for(int j = 0; j < temporaryList.Count; j++)
            {
                if(temporaryList[j].initiative < minInitiative) // ���� ���������� ���������� ��������� ������, ��� ���������
                {
                    minInitiativeCharacter = temporaryList[j]; // �������� ��������� � ��� �����������
                    minInitiative = minInitiativeCharacter.initiative; // �������� ����� �������� ��������
                }
            }
            sortedList.Add(minInitiativeCharacter); // �������� ���������������� � ����� ������ (����� ����� ����������� � �����)
            mapAnchorScr.charactersList.Insert(0, minInitiativeCharacter); // ���������������� �������� � ������ �������� ������, ��� 0 ����� � ��������� �����������
            temporaryList.Remove(minInitiativeCharacter); // ������� ���������������� �� ���������� ������
        }
        gameInterFace.DownloadTheInterface(sortedList); // ������������ ������� � ���������� 
        SetTheCurrentPlayer();
    }
    // ����������� ������� � ���� ����
    public void MoveTheTurnQueue ()
    {
        Debug.Log("�����������");
        MapAnchor mapAnchorScr = mapAnchor.GetComponent<MapAnchor>();   // �������� ������ �� ����� MapAnchor
        mapAnchorScr.charactersList.Add(mapAnchorScr.charactersList[0]);
        mapAnchorScr.charactersList.RemoveAt(0);
        List<Character> sortedList = new List<Character>();
        for(int i = 0; i < mapAnchorScr.charactersList.Count; i++)
        {
            Debug.Log("���������");
            sortedList.Insert(0, mapAnchorScr.charactersList[i]);
        }
        gameInterFace.DownloadTheInterface(sortedList);
        SetTheCurrentPlayer();
    }
    public void SetTheCurrentPlayer ()
    {
        Debug.Log("��������� �������� �����");
        if(currentCharacter != null) // ���� �� ������ ������ ���-�� ��������� ������� ����������
        {
            currentCharacter.characterState = Character.CharacterState.Expectation; // ��������� ��� � ��������� ��������
        }
        currentCharacter = mapAnchorScr.charactersList[0]; // ��������� ������ �������� ������
        currentCharacter.characterState = Character.CharacterState.Readiness; // ��������� �������� ������ � ��������� ����������
        currentCharacter.gameObject.layer = 16;
    }
    // �������� ��������� ���������� � ��������
    public void GetMovementCharacterState ()
    {
        ChangeCellsStates(LandscapeCell.CellState.�alculation);
        currentCharacter.characterState = Character.CharacterState.Movement;
    }
    public void GetAttackCharacterState ()
    {
        ChangeCellsStates(LandscapeCell.CellState.Expectation);
        currentCharacter.characterState = Character.CharacterState.Attack;
    }
    public void GetAbilityCharacterState (int index)
    {
        ChangeCellsStates(LandscapeCell.CellState.Expectation);
        currentCharacter.attackMode.abilitiesIndex = index;
        currentCharacter.characterState = Character.CharacterState.Ability;
    }
    public void ChangeCellsStates (LandscapeCell.CellState cellState)
    {
        MapAnchor mapAnchorScr = mapAnchor.GetComponent<MapAnchor>();
        for(int i = 0; i < mapAnchorScr.landscapeCells.Length; i++) // ��� ������ �������� � ��������� ���������� � ���������������
        {
            foreach(LandscapeCell cell in mapAnchorScr.landscapeCells[i])
            {
                if(cell.landscapeSO.surmountable)
                {
                    cell.cellState = cellState;
                }
            }
        }
    }
}
