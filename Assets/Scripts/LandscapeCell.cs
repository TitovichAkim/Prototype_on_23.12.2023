using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LandscapeCell: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("SetInInspector")]
    public SpriteRenderer landscapeSpriteRenderer;
    public GameObject characterPrefab;

    [Header("SetDynamically")]
    public Character currentCharacter;
    public LevelRedactor levelRedactor;
    public List<LandscapeCell> shortestPath; // список для хранения кратчайшего пути
    public float costOfThePath; // стоимость пути 
    public List<LandscapeCell> adjacentLandscapeCellsInAStraightLine; // Соседние ячейки по прямой
    public List<LandscapeCell> adjacentLandscapeCellsDiagonally; // Соседние ячейки по диагонали
    public float minimumMovementCosts; // Минимальная стоимость движения к этой клетке

    private LandscapeSO _landscapeSO;
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
            Debug.Log("Нельзя разместить сдесь");
        }
        else
        {
            Debug.Log("Разместите прохлдимый рельеф");
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

    public void OnPointerUp (PointerEventData pointerEventData)
    {
        // Регистация отпускания клавиши (будет реализована позже)
    }

}
