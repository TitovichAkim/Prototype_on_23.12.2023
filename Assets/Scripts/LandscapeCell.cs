using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LandscapeCell:MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("SetInInspector")]
    public SpriteRenderer landscapeSpriteRenderer;

    [Header("SetDynamically")]
    public LevelRedactor levelRedactor;
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
                    break;
            }
        }
    }

    public void OnPointerUp (PointerEventData pointerEventData)
    {
        // Регистация отпускания клавиши (будет реализована позже)
    }
}
