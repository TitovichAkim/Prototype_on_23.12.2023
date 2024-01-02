using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LandscapeCell: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
            if (landscapeSO.surmountable)
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
    public void OnPointerDown (PointerEventData pointerEventData)
    {
        if(Input.GetMouseButtonDown(0))
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



    // ����� ��� ���������� ��������� ������� � ������
    public void UpdateCost (float newCost, List<LandscapeCell> path)
    {
        minimumMovementCosts = newCost;
        shortestPath = path;
        minimumMovementCostsText.text = newCost.ToString("F0");
    }

    public void OnPointerUp (PointerEventData pointerEventData)
    {
        // ���������� ���������� ������� (����� ����������� �����)
    }

}
