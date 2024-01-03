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
    public Character currentCharacter; // ������ �� ���������, ������������ � ���� ������
    public LevelRedactor levelRedactor;
    public List<LandscapeCell> shortestPath = new List<LandscapeCell>(); // ������ ������, ����� ������� ����� ������ ���������� ���� ���������

    public List<LandscapeCell> adjacentLandscapeCellsInAStraightLine; // �������� ������ �� ������
    public List<LandscapeCell> adjacentLandscapeCellsDiagonally; // �������� ������ �� ���������
    public float minimumMovementCosts; // ����������� ��������� �������� � ���� ������
    public Vector2 coordinates; // ���������� ������ 


    private LandscapeSO _landscapeSO; // ������� �������������� ������
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
            Debug.Log("������ ���������� �����");
        }
        else
        {
            Debug.Log("���������� ���������� ������");
        }
    }

    public void AssignActions (GameObject cell)
    {
        // �������� ��������� EventTrigger
        EventTrigger eventTrigger = cell.GetComponent<EventTrigger>();

        // ���������, ��� ��������� EventTrigger ������������
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
        Debug.Log("�������!");
        if(Input.GetMouseButtonDown(1))
        {
            ApplyAnObjectInYourHand();
        }
    }
    public void OnPointerEnterDelegate (PointerEventData data)
    {
        Debug.Log("�����!");

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
        // ��������� ����� ������ �������� ��� ����� ��������� � �������
    }
    // ����� ��� ���������� ��������� ������� � ������
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
