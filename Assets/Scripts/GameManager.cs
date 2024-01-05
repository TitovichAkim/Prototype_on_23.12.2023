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
        Debug.Log("Ќачинаем сортировку");
        MapAnchor mapAnchorScr = mapAnchor.GetComponent<MapAnchor>();   // ѕолучаем ссылку на класс MapAnchor
        List<Character> temporaryList = new List<Character>();
        temporaryList.AddRange(mapAnchorScr.charactersList); // ѕолучаем список всех персонажей
        Debug.Log("персонажей в сортировке = " + temporaryList.Count);

        mapAnchorScr.charactersList.Clear(); // ќчищаем список персонажей в €коре карты
        // —начала перемешаем игроков
        for (int i = 0; i < temporaryList.Count; i++)
        {
            int randomNumber = Random.Range(0, temporaryList.Count); // берем рандомное значение от 0 до количества персонажей в временном списке
            mapAnchorScr.charactersList.Add(temporaryList[randomNumber]); // добавл€ем рандомного персонажа в список на €коре карты
            Debug.Log($"“еперь в списке рандомных { mapAnchorScr.charactersList}");
            temporaryList.RemoveAt(randomNumber); // удал€ем рандоного персонажа из временного списка
        }
        temporaryList.AddRange(mapAnchorScr.charactersList); // заполн€ем временный список уже перебранным в рандоме списком с €кор€
        mapAnchorScr.charactersList.Clear(); // —нова очищаем список, который находитс€ на €коре
        List<Character> sortedList = new List<Character>(); // —оздаем сортированный список
        while(temporaryList.Count > 0)
        {
            Character minInitiativeCharacter = temporaryList[0]; // ѕерсонаж с минимальной инициативой
            float minInitiative = minInitiativeCharacter.initiative; // ћинимальна€ инициатива, изначально берем ее у нулевого персонажа
            for(int j = 0; j < temporaryList.Count; j++)
            {
                if(temporaryList[j].initiative < minInitiative) // ≈сли инициатива очередного персонажа меньше, чем временна€
                {
                    minInitiativeCharacter = temporaryList[j]; // записать персонажа с мин инициативой
                    minInitiative = minInitiativeCharacter.initiative; // «аписать новое значение минимума
                }
            }
            sortedList.Add(minInitiativeCharacter); // добавить отсортированного в конец списка (потом будем расставл€ть с конца)
            mapAnchorScr.charactersList.Insert(0, minInitiativeCharacter); // отсортированного добавить в начало главного списка, там 0 будет с наивысшей инициативой
            temporaryList.Remove(minInitiativeCharacter); // удалить отсортированного из временного списка
        }
        gameInterFace.DownloadTheInterface(sortedList); // —формировать очередь в интерфейсе 
        SetTheCurrentPlayer();
    }
    // ѕередвигает очередь в ходе игры
    public void MoveTheTurnQueue ()
    {
        Debug.Log("передвигаем");
        MapAnchor mapAnchorScr = mapAnchor.GetComponent<MapAnchor>();   // ѕолучаем ссылку на класс MapAnchor
        mapAnchorScr.charactersList.Add(mapAnchorScr.charactersList[0]);
        mapAnchorScr.charactersList.RemoveAt(0);
        List<Character> sortedList = new List<Character>();
        for(int i = 0; i < mapAnchorScr.charactersList.Count; i++)
        {
            Debug.Log("—ортируем");
            sortedList.Insert(0, mapAnchorScr.charactersList[i]);
        }
        gameInterFace.DownloadTheInterface(sortedList);
        SetTheCurrentPlayer();
    }
    public void SetTheCurrentPlayer ()
    {
        Debug.Log("Ќазначаем главного перса");
        if(currentCharacter != null) // ≈сли на данный момент кто-то считаетс€ текущим персонажем
        {
            currentCharacter.characterState = Character.CharacterState.Expectation; // ѕеревести его в состо€ние ожидани€
        }
        currentCharacter = mapAnchorScr.charactersList[0]; // Ќазначить нового текущего игрока
        currentCharacter.characterState = Character.CharacterState.Readiness; // ѕеревести текущего игрока в состо€ние готовности
        currentCharacter.gameObject.layer = 16;
    }
    // ¬ключает состо€ние подготовки к движению
    public void GetMovementCharacterState ()
    {
        ChangeCellsStates(LandscapeCell.CellState.—alculation);
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
        for(int i = 0; i < mapAnchorScr.landscapeCells.Length; i++) // ¬се €чейки привести в состо€ние готовности к калькулированию
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
