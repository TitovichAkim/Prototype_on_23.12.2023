using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LandscapeCell:MonoBehaviour
{
    public enum CellState
    {
        Сalculation, // подготовка к расчету
        EnoughPoints, // достаточно очков перехода
        EnoughStamina, // хватает очков выносливости
        Expectation // ожидание
    }
    [Header("SetInInspector")]
    public SpriteRenderer landscapeSpriteRenderer;
    public TMP_Text minimumMovementCostsText;
    public GameObject characterPrefab;
    [Header("SetDynamically")]
    public Character currentCharacter; // Ссылка на персонажа, находящегося в этой клетке
    public LevelRedactor levelRedactor;
    public GameManager gameManager;
    public List<LandscapeCell> shortestPath = new List<LandscapeCell>(); // Массив клеток, через которые будет лежать кратчайший путь персонажа
    public List<LandscapeCell> adjacentLandscapeCellsInAStraightLine; // Соседние ячейки по прямой
    public List<LandscapeCell> adjacentLandscapeCellsDiagonally; // Соседние ячейки по диагонали
    public float minimumMovementCosts; // Минимальная стоимость движения к этой клетке
    public Vector2 coordinates; // Координаты клетки 
    private LandscapeSO _landscapeSO; // Базовые характеристики клетки
    [SerializeField]private CellState _cellState;
    public LandscapeSO landscapeSO
    {
        get
        {
            return (_landscapeSO);
        }
        set
        {
            _landscapeSO = value;
            SetLandscapeItem();
        }
    }
    public CellState cellState
    {
        get
        {
            return (_cellState);
        }
        set
        {
            _cellState = value;
            ChangeCellState(cellState);
        }
    }
    void Start ()
    {
        AssignActions(this.gameObject);
    }
    public void SetLandscapeItem ()
    {
        if(landscapeSO != null)
        {
            landscapeSpriteRenderer.sprite = landscapeSO.landscapeImage;
            if(!landscapeSO.shootingRange)
            {
                this.gameObject.layer = 12;
            }

        }
    }
    public void SetCharacterItem (int characterIndex, int teamNumber = 0)
    {
        if(landscapeSO != null)
        {
            if(landscapeSO.surmountable)
            {
                GameObject characterGO = Instantiate(characterPrefab);
                Character characterScr = characterGO.GetComponent<Character>();
                characterScr.characterSO = levelRedactor.levelItemsPanel.setOfLevelEditor.characterSOs[characterIndex];
                if(teamNumber == 0)
                {
                    characterScr.teamNumber = levelRedactor.levelItemsPanel.itemsContentPanels[1].GetComponent<ItemsContentPanel>().teamNumberDropdown.GetComponent<TMP_Dropdown>().value + 1;
                }
                else
                {
                    characterScr.teamNumber = teamNumber;
                }
                levelRedactor.mapAnchor.charactersList.Add(characterScr);
                characterGO.transform.position = this.gameObject.transform.position;
                currentCharacter = characterScr;
                characterScr.currentLandscapeCell = this;
                characterScr.gameManager = gameManager;
                characterScr.mapAnchor = gameManager.mapAnchor.GetComponent<MapAnchor>();
            }
            Debug.Log("Нельзя разместить сдесь");
        }
        else
        {
            Debug.Log("Разместите проходимый рельеф");
        }
    }

    public void AssignActions (GameObject cell)
    {
        // Получаем компонент EventTrigger
        EventTrigger eventTrigger = cell.GetComponent<EventTrigger>();

        // Проверяем, что компонент EventTrigger присутствует
        if(eventTrigger != null)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(entry);

            EventTrigger.Entry down = new EventTrigger.Entry();
            down.eventID = EventTriggerType.PointerDown;
            down.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(down);

            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(exit);
        }
    }
    public void OnPointerDownDelegate (PointerEventData data)
    {
        if(GameManager.currentGameState == GameManager.GameState.LevelRedactor)
        {
            if(Input.GetMouseButtonDown(1) && levelRedactor.flyingItem != null)
            {
                ApplyAnObjectInYourHand();
            }
        }
        else if(GameManager.currentGameState == GameManager.GameState.Game)
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(cellState == CellState.EnoughPoints || cellState == CellState.EnoughStamina)
                {
                    gameManager.currentCharacter.characterMovement.targetCells.AddRange(shortestPath);
                }
            }
        }
    }
    public void OnPointerEnterDelegate (PointerEventData data)
    {
        if(EventSystem.current.IsPointerOverGameObject())
            return;
        switch(GameManager.currentGameState)
        {
            case GameManager.GameState.LevelRedactor:
                if(levelRedactor.flyingItem != null)
                {
                    if(levelRedactor.flyingItem.paintOver)
                    {
                        ApplyAnObjectInYourHand();
                    }
                }
                break;
            case GameManager.GameState.Game:
                if((cellState == CellState.EnoughPoints || cellState == CellState.EnoughStamina) && gameManager.currentCharacter.characterState == Character.CharacterState.Readiness && _landscapeSO.surmountable)
                {
                    DisplayTheCostOfMovement(true);
                }
                else if(gameManager.currentCharacter.characterState == Character.CharacterState.Readiness)
                {
                    DisplayTheCostOfMovement(true);
                }
                break;
        }
    }
    public void OnPointerExitDelegate (PointerEventData data)
    {
        switch(GameManager.currentGameState)
        {
            case GameManager.GameState.Game:
                if(gameManager.currentCharacter.characterState == Character.CharacterState.Readiness)
                {
                    DisplayTheCostOfMovement(false);
                }
                break;
        }
    }
    // Метод для обновления стоимости прохода к клетке
    public void UpdateCost (float newCost, List<LandscapeCell> path, CellState state)
    {
        minimumMovementCosts = newCost;
        shortestPath = path;
        cellState = state;
        minimumMovementCostsText.text = newCost.ToString("F0");
    }

    public void ApplyAnObjectInYourHand ()
    {
        int indexItemInHand = levelRedactor.indexItemInHand;
        SetOfLevelEditor setOfLevelEditor = levelRedactor.levelItemsPanel.setOfLevelEditor;
        switch(levelRedactor.scriptableObjectInHand)
        {
            case LandscapeSO:
                landscapeSO = setOfLevelEditor.landscapeSOs[indexItemInHand];
                break;
            case CharacterSO:
                SetCharacterItem(indexItemInHand);
                break;
        }
    }
    public void ChangeCellState (CellState state)
    {
        switch(state)
        {
            case CellState.Expectation:
                minimumMovementCosts = 1000;
                shortestPath.Clear();
                minimumMovementCostsText.gameObject.SetActive(false);
                break;
            case CellState.Сalculation:
                minimumMovementCosts = 1000;
                shortestPath.Clear();
                minimumMovementCostsText.gameObject.SetActive(false);
                break;
            case CellState.EnoughPoints:
                minimumMovementCostsText.gameObject.SetActive(true);
                minimumMovementCostsText.color = Color.blue;
                minimumMovementCostsText.text = minimumMovementCosts.ToString();
                break;
            case CellState.EnoughStamina:
                minimumMovementCostsText.gameObject.SetActive(true);
                minimumMovementCostsText.color = Color.black;
                minimumMovementCostsText.text = minimumMovementCosts.ToString();
                break;
        }
    }
    public void DisplayTheCostOfMovement (bool enter)
    {

        if(enter)
        {
            if(cellState == LandscapeCell.CellState.EnoughPoints)
            {
                gameManager.currentCharacter.personalCharactersCanvas.pointsNumberText.text = (gameManager.currentCharacter.movementPoints - minimumMovementCosts).ToString("F0");
            }
            else if(cellState == LandscapeCell.CellState.EnoughStamina)
            {
                gameManager.currentCharacter.personalCharactersCanvas.pointsNumberText.text = "0";
                gameManager.currentCharacter.personalCharactersCanvas.pointsNumberText.color = Color.red;
                gameManager.currentCharacter.personalCharactersCanvas.enduranceNumberText.text = (gameManager.currentCharacter.currentEdurance - (minimumMovementCosts - gameManager.currentCharacter.movementPoints)).ToString("F0");
            }
            else
            {
                gameManager.currentCharacter.personalCharactersCanvas.pointsNumberText.text = "0";
                gameManager.currentCharacter.personalCharactersCanvas.pointsNumberText.color = Color.red;
                gameManager.currentCharacter.personalCharactersCanvas.enduranceNumberText.text = "0";
                gameManager.currentCharacter.personalCharactersCanvas.enduranceNumberText.color = Color.red;
            }
        }
        else
        {
            gameManager.currentCharacter.personalCharactersCanvas.pointsNumberText.color = Color.green;
            gameManager.currentCharacter.personalCharactersCanvas.enduranceNumberText.color = Color.blue;
            gameManager.currentCharacter.currentEdurance = gameManager.currentCharacter.currentEdurance;
            gameManager.currentCharacter.movementPoints = gameManager.currentCharacter.movementPoints;
        }
    }
}
