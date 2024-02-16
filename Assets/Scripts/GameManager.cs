using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameManager:MonoBehaviour
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
    public LevelRedactor levelRedactor;
    public GameObject mainMenuCanvas;
    public GameObject temporaryMenuPanel;
    public GameObject redactorMenuCanvas;
    public GameObject sizeAdjustmentPanel;
    public GameObject levelItemsPanel;
    [Header("SetDynamically")]
    [SerializeField]public static bool cursorOnUI;
    public bool initiativeHasChanged;
    public GameObject mapAnchor;
    public List<Character> queueOfCharacters = new List<Character>();
    public MapAnchor mapAnchorScr;
    public CaracterRedactorPanel currentCharacterRedactor;
    [SerializeField]private Character _currentCharacter;
    private Character boundaryCharacter; // граничный персонаж - тот, с которого началась очередь ходов


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
    public void Start ()
    {
        gameManager = this;
        currentGameState = GameState.MainMenu;
    }
    private void Update ()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.G))
        {
            StartLevelRedactorState(false);
        }
        if(GameManager.currentGameState == GameState.Game)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(GameManager.currentGameState == GameState.Game && currentCharacter.characterMovement.targetCells.Count <= 0)
                {
                    MoveTheTurnQueue();
                }
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {

            }
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                GetAbilityCharacterState(0);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                GetAbilityCharacterState(1);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                GetAbilityCharacterState(2);
            }
            if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                GetAbilityCharacterState(3);
            }
        }

    }
    public void ChangeTheGameMode (GameState newGameState, GameState oldGameState)
    {
        if(levelRedactor.currentHostedItem != null)
        {
            Destroy(levelRedactor.currentHostedItem);
            levelRedactor.flyingItem = null;
        }
        if(LevelRedactor.redactingCharacter != null)
        {
            LevelRedactor.redactingCharacter.caracterRedactorPanel.GetComponent<CaracterRedactorPanel>().DestroyCharacterRedactor();
        }

        switch(newGameState)
        {
            case GameState.MainMenu:
                mainMenuCanvas.SetActive(true);
                temporaryMenuPanel.SetActive(true);
                redactorMenuCanvas.SetActive(false);
                sizeAdjustmentPanel.SetActive(false);

                if(mapAnchor != null)
                {
                    while(mapAnchorScr.charactersList.Count > 0)
                    {
                        mapAnchorScr.charactersList[0].characterState = Character.CharacterState.Death;
                    }
                    mapAnchor.GetComponent<MapAnchor>().charactersList.Clear();
                    Destroy(mapAnchor);
                    levelRedactor.saveAndLoad.FillInTheSavePanel();
                }
                break;
            case GameState.LevelRedactor:
                mainMenuCanvas.SetActive(false);
                gameInterFace.gameObject.SetActive(false);
                redactorMenuCanvas.SetActive(true);
                sizeAdjustmentPanel.SetActive(_newMap);
                levelItemsPanel.SetActive(!_newMap);
                if(currentCharacter != null)
                {
                    currentCharacter.personalCharactersCanvas.currentCharacterPanel.SetActive(false);
                    foreach(LandscapeCell[] cells in mapAnchorScr.landscapeCells)
                    {
                        foreach(LandscapeCell cell in cells)
                        {
                            cell.cellState = LandscapeCell.CellState.Expectation;
                        }
                    }
                }
                break;
            case GameState.Game:
                mainMenuCanvas.SetActive(false);
                redactorMenuCanvas.SetActive(false);
                gameInterFace.gameObject.SetActive(true);
                gameInterFace.mapAnchor = mapAnchor.GetComponent<MapAnchor>();
                mapAnchorScr = mapAnchor.GetComponent<MapAnchor>();
                if(currentCharacter != null)
                {
                    currentCharacter.personalCharactersCanvas.currentCharacterPanel.SetActive(true);
                    currentCharacter.energyCostCalculator.CalculateEnergyCost(currentCharacter.currentLandscapeCell);
                    gameInterFace.DownloadTheInterface(InvertTheList(mapAnchorScr.charactersList));
                }
                else
                {
                    SortTheQueue();
                }
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
        if (currentGameState == GameState.LevelRedactor)
        {
            currentGameState = GameState.Game;
        }
        else
        {
            currentGameState = GameState.LevelRedactor;
        }
    }
    public void StartGameState ()
    {
        currentGameState = GameState.Game;
    }

    public void SortTheQueue () // —ортирует очередь по значению инициативы
    {
        MapAnchor mapAnchorScr = mapAnchor.GetComponent<MapAnchor>();   // ѕолучаем ссылку на класс MapAnchor
        List<Character> temporaryList = new List<Character>();
        temporaryList.AddRange(mapAnchorScr.charactersList); // ѕолучаем список всех персонажей

        mapAnchorScr.charactersList.Clear(); // ќчищаем список персонажей в €коре карты
        // —начала перемешаем игроков
        for(int i = 0; i < temporaryList.Count; i++)
        {
            int randomNumber = Random.Range(0, temporaryList.Count); // берем рандомное значение от 0 до количества персонажей в временном списке
            mapAnchorScr.charactersList.Add(temporaryList[randomNumber]); // добавл€ем рандомного персонажа в список на €коре карты
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
        boundaryCharacter = mapAnchorScr.charactersList[0];
        gameInterFace.DownloadTheInterface(sortedList); // —формировать очередь в интерфейсе 
        SetTheCurrentPlayer();
    }
    public List<Character> InvertTheList (List<Character> temporaryList)
    {
        List < Character > startList = new List<Character>();
        startList.AddRange(temporaryList);
        List < Character > resultList = new List<Character>();
        while(startList.Count > 0)
        {
            resultList.Add(startList[startList.Count - 1]);
            startList.RemoveAt(startList.Count - 1);
        }
        return resultList;
    }
    // ѕередвигает очередь в ходе игры
    public void MoveTheTurnQueue ()
    {
        MapAnchor mapAnchorScr = mapAnchor.GetComponent<MapAnchor>();   // ѕолучаем ссылку на класс MapAnchor
        mapAnchorScr.charactersList.Add(mapAnchorScr.charactersList[0]);
        mapAnchorScr.charactersList.RemoveAt(0);
        List<Character> sortedList = new List<Character>();
        for(int i = 0; i < mapAnchorScr.charactersList.Count; i++)
        {
            sortedList.Insert(0, mapAnchorScr.charactersList[i]);
        }
        gameInterFace.DownloadTheInterface(sortedList);
        if(mapAnchorScr.charactersList[0] == boundaryCharacter && initiativeHasChanged)
        {
            SortTheQueue();
            initiativeHasChanged = false;
        }
        else
        {
            SetTheCurrentPlayer();
        }
    }
    public void SetTheCurrentPlayer ()
    {
        if(mapAnchorScr.charactersList.Count > 0)
        {
            if(currentCharacter != null) // ≈сли на данный момент кто-то считаетс€ текущим персонажем
            {
                currentCharacter.characterState = Character.CharacterState.Expectation; // ѕеревести его в состо€ние ожидани€
            }
            currentCharacter = mapAnchorScr.charactersList[0]; // Ќазначить нового текущего игрока
            currentCharacter.characterState = Character.CharacterState.Readiness; // ѕеревести текущего игрока в состо€ние готовности
            currentCharacter.gameObject.layer = 16;
        }
    }
    // ¬ключает состо€ние подготовки к движению
    public void GetMovementCharacterState ()
    {
        ChangeCellsStates(LandscapeCell.CellState.—alculation);
        currentCharacter.characterState = Character.CharacterState.Movement;
    }
    public void GetAttackCharacterState ()
    {
        if(currentCharacter.characterMovement.targetCells.Count <= 0)
        {
            ChangeCellsStates(LandscapeCell.CellState.Expectation);
            currentCharacter.characterState = Character.CharacterState.Attack;
        }
    }
    public void GetAbilityCharacterState (int index)
    {
        if(currentCharacter.abilitiesRecharge[(index % 4)].x > 0 && currentCharacter.characterMovement.targetCells.Count <= 0
            && currentCharacter.characterSO.abilities[index % 4].requiredEndurance <= currentCharacter.currentEdurance 
            && currentCharacter.characterSO.abilities[index % 4].requiredMana <= currentCharacter.currentMana)
        {
            currentCharacter.characterState = Character.CharacterState.Readiness;
            ChangeCellsStates(LandscapeCell.CellState.Expectation);
            currentCharacter.attackMode.abilitiesIndex = index;
            currentCharacter.characterState = Character.CharacterState.Ability;
        }
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
    public static void SetCursorOnUI (bool value)
    {
        cursorOnUI = value;
    }
}
