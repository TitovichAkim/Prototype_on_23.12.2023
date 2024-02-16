using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static Character;

public class EffectItems:MonoBehaviour
{
    [Header("SetInInspector")]
    public Image effectImage;
    public GameObject effectNumberTextGO;
    public TMP_Text effectNumberText;
    public TMP_Text durationOfTheEffectText;
    public GameObject flyingTextForAbilityPrefab;

    [Header("SetDynamically")]
    public string effectDescription = "";
    public GameObject flyingText;
    private void Start ()
    {
        AssignActions(this.gameObject, effectDescription);
    }
    public void AssignActions (GameObject itemPanel, string itemText)
    {
        // Получаем компонент EventTrigger
        EventTrigger eventTrigger = itemPanel.GetComponent<EventTrigger>();

        // Проверяем, что компонент EventTrigger присутствует
        if(eventTrigger != null)
        {
            // Добавляем слушатель события OnPointerEnter
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(entry);

            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(exit);
        }
    }
    public void OnPointerEnterDelegate (PointerEventData data)
    {
        if(flyingText == null && effectDescription != "")
        {
            flyingText = Instantiate(flyingTextForAbilityPrefab, this.gameObject.transform);
            flyingText.GetComponent<FlyingTextForAbility>().tMP_Text.text = effectDescription;
        }
    }
    public void OnPointerExitDelegate (PointerEventData data)
    {
        Destroy(flyingText);
    }
}
