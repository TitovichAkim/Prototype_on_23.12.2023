using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LandscapeCell:MonoBehaviour
{
    [Header("SetInInspector")]
    public SpriteRenderer landscapeSpriteRenderer;
    public TMP_Text minimumMovementCostsText;
    public GameObject characterPrefab;

    [Header("SetDynamically")]
    public Character currentCharacter; // Ссылка на персонажа, находящегося в этой клетке
    public LevelRedactor levelRedactor;
    public List<LandscapeCell> shortestPath = new List<LandscapeCell>(); // Массив клеток, через которые будет лежать кратчайший путь персонажа

    public List<LandscapeCell> adjacentLandscapeCellsInAStraightLine; // Соседние ячейки по прямой
    public List<LandscapeCell> adjacentLandscapeCellsDiagonally; // Соседние ячейки по диагонали
    public float minimumMovementCosts; // Минимальная стоимость движения к этой клетке
    public Vector2 coordinates; // Координаты клетки 


    private LandscapeSO _landscapeSO; // Базовые характеристики клетки
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
    void Start ()
    {
        AssignActions(this.gameObject);
    }
    public void SetLandscapeItem ()
    {
        if(landscapeSO != null)
        {
            landscapeSpriteRenderer.sprite = landscapeSO.landscapeImage;
        }
    }
    public void SetCharacterItem (int characterIndex)
    {
        if(landscapeSO != null)
        {
            if(landscapeSO.surmountable)
            {
                GameObject characterGO = Instantiate(characterPrefab);
                Character characterScr = characterGO.GetComponent<Character>();
                characterScr.characterSO = levelRedactor.levelItemsPanel.setOfLevelEditor.characterSOs[characterIndex];
                characterGO.transform.position = this.gameObject.transform.position;
                currentCharacter = characterScr;
                characterScr.currentLandscapeCell = this;
            }
            Debug.Log("Нельзя разместить сдесь");
        }
        else
        {
            Debug.Log("Разместите прохлдимый рельеф");
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
        Debug.Log("Кликнул!");
        if(Input.GetMouseButtonDown(1))
        {
            ApplyAnObjectInYourHand();
        }
    }
    public void OnPointerEnterDelegate (PointerEventData data)
    {
        Debug.Log("Вошел!");

        if(levelRedactor.flyingItem != null)
        {
            if(levelRedactor.flyingItem.paintOver)
            {
                ApplyAnObjectInYourHand();
            }
        }
    }
    public void OnPointerExitDelegate (PointerEventData data)
    {
        // Выполните здесь нужные действия при входе указателя в область
    }
    // Метод для обновления стоимости прохода к клетке
    public void UpdateCost (float newCost, List<LandscapeCell> path)
    {
        minimumMovementCosts = newCost;
        shortestPath = path;
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
}
