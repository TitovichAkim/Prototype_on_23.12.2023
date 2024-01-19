using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static LandscapeCell;

public class PersonalCharactersCanvas:MonoBehaviour
{
    [Header("SetInInspector")]
    public GameObject flyingTextForAbilityPrefab;

    [Header("CurrentChatacterObjects")]
    public GameObject currentCharacterPanel;
    public Image characterIcon;
    public Image currentHealthProgressBar;
    public Image currentManaProgressBar;
    public TMP_Text charactersName;
    public TMP_Text attackPowerNumberText;
    public TMP_Text attackRangeNumberText;
    public TMP_Text speedNumberText;
    public TMP_Text initiativeNumberText;
    public TMP_Text enduranceNumberText;
    public TMP_Text pointsNumberText;
    public Image[] abilitiesIcons;
    public TMP_Text[] manaAbilitiesCostTexts;
    public TMP_Text[] anduranceAbilitiesCostTexts;
    public TMP_Text[] abilitiesRechargeTexts;
    public Button[] abilitiesButtons;
    public GameObject[] abilitiesPanels;
    public TMP_Text healthNumberText;
    public TMP_Text manaNumberText;
    public TMP_Text shieldNumberText;
    public Transform effectsPanelTransform;

    [Header("TargetChatacterObjects")]
    public GameObject targetCharacterPanel;
    public Image targetCharacterIcon;
    public Image targetHealthProgressBar;
    public Image targetManaProgressBar;
    public TMP_Text targetCharacterNameText;
    public TMP_Text targetHealthNumberText;
    public TMP_Text targetManaNumberText;
    public TMP_Text targetShieldNumberText;
    public Transform currentEffectsTransform;

    [Header("SetDynamically")]
    public Character _parentCharacter;
    public GameObject flyingText;
    public GameObject[] effectsGOs = new GameObject[8];
    public GameObject[] targetEffectsGOs = new GameObject[8];

    private Character _character;
    private CharacterSO _characterSO;
    public Character character
    {
        get
        {
            return (_character);
        }
        set
        {
            _character = value;
            FillInTheFields();
        }
    }
    public CharacterSO characterSO
    {
        get
        {
            return (_characterSO);
        }
        set
        {
            _characterSO = value;
        }
    }


    public void FillInTheFields ()
    {
        characterSO = character.characterSO;
        characterIcon.sprite = _characterSO.characterIcon;
        for(int i = 0; i < abilitiesButtons.Length; i++)
        {
            abilitiesIcons[i].sprite = character.characterSO.abilities[i].abilitiesIcon;
            manaAbilitiesCostTexts[i].text = character.characterSO.abilities[i].requiredMana.ToString();
            anduranceAbilitiesCostTexts[i].text = character.characterSO.abilities[i].requiredEndurance.ToString();
            abilitiesRechargeTexts[i].text = character.abilitiesRecharge[i].y.ToString();
        }
        targetCharacterIcon.sprite = _characterSO.characterIcon;
        for(int i = 0; i < abilitiesButtons.Length; i++)
        {
            int numIndex = i;
            AssignActions(abilitiesPanels[i], character.characterSO.abilities[i].abilitiesDescription, numIndex);
        }
    }
    public void RedrawAbilitiesBoxes ()
    {
        for (int i = 0; i < 4; i++)
        {
            abilitiesRechargeTexts[i].text = character.abilitiesRecharge[i].y.ToString();
            switch(character.abilitiesRecharge[i].x)
            {
                case 0:
                    abilitiesRechargeTexts[i].rectTransform.sizeDelta = new Vector2(50f, 50f);
                    abilitiesButtons[i].enabled = false;
                    break;
                case 1:
                    abilitiesRechargeTexts[i].rectTransform.sizeDelta = new Vector2(50f, 50f);
                    abilitiesButtons[i].enabled = true;
                    break;
                case 2:
                    abilitiesRechargeTexts[i].rectTransform.sizeDelta = new Vector2(150f, 150f);
                    abilitiesButtons[i].enabled = true;
                    break;
            }
        }
    }
    public void AssignActions (GameObject itemPanel, string itemText, int numIndex)
    {
        // Получаем компонент EventTrigger
        EventTrigger eventTrigger = itemPanel.GetComponent<EventTrigger>();

        // Проверяем, что компонент EventTrigger присутствует
        if(eventTrigger != null)
        {
            // Добавляем слушатель события OnPointerEnter
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data, itemText); });
            eventTrigger.triggers.Add(entry);

            EventTrigger.Entry down = new EventTrigger.Entry();
            down.eventID = EventTriggerType.PointerDown;
            down.callback.AddListener((data) => { character.gameManager.GetAbilityCharacterState(numIndex); });
            entry.callback.AddListener((data) => { GameManager.SetCursorOnUI(true); });
            eventTrigger.triggers.Add(down);

            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
            exit.callback.AddListener((data) => { GameManager.SetCursorOnUI(false); });
            eventTrigger.triggers.Add(exit);


            exit.eventID = EventTriggerType.PointerExit;
        }
    }

    public void OnPointerEnterDelegate (PointerEventData data, string itemName)
    {
        if (flyingText == null)
        {
            flyingText = Instantiate(flyingTextForAbilityPrefab, this.gameObject.transform);
            flyingText.GetComponent<FlyingTextForAbility>().tMP_Text.text = itemName;
        }
    }
    public void OnPointerExitDelegate (PointerEventData data)
    {
        Destroy(flyingText);
    }
}

