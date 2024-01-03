using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelItemsPanel : MonoBehaviour
{
    [Header("SetInInspector")]
    public GameObject itemsContentPrefab;
    public GameObject itemPrefab;
    public GameObject flyingTextPrefab;
    public TMP_Dropdown itemsDropdown;
    public SetOfLevelEditor setOfLevelEditor;
    public LevelRedactor levelRedactor;
    
    [Header("SetDynamically")]
    public GameObject[] itemsContentPanels;
    public GameObject flyingText;
    public void Start ()
    {
        itemsContentPanels = new GameObject[2];
        GenerateContent();
    }
    public void GenerateContent ()
    {
        if(setOfLevelEditor.landscapeSOs.Length != 0)
        {
            GameObject landscapeItemsContent = Instantiate(itemsContentPrefab, this.gameObject.transform);
            for(int i = 0; i < setOfLevelEditor.landscapeSOs.Length; i++)
            {
                CreateAnItemPanel(landscapeItemsContent.GetComponent<ItemsContentPanel>().dropdownContent, i, setOfLevelEditor.landscapeSOs[i]);
            }
            landscapeItemsContent.GetComponent<ItemsContentPanel>().teamNumberDropdown.SetActive(false);
            itemsContentPanels[0] = landscapeItemsContent;
        }
        if(setOfLevelEditor.landscapeSOs.Length != 0)
        {
            GameObject characterItemsContent = Instantiate(itemsContentPrefab, this.gameObject.transform);
            for(int i = 0; i < setOfLevelEditor.characterSOs.Length; i++)
            {
                CreateAnItemPanel(characterItemsContent.GetComponent<ItemsContentPanel>().dropdownContent, i, setOfLevelEditor.characterSOs[i]);
            }
            itemsContentPanels[1] = characterItemsContent;
            characterItemsContent.SetActive(false);
        }
    }
    public void CreateAnItemPanel (GameObject parentGO, int itemsIndex, ScriptableObject itemType)
    {
        GameObject itemPanel = Instantiate(itemPrefab, parentGO.transform);
        itemPanel.name = $"Item{itemsIndex}";
        switch(itemType)
        {
            case LandscapeSO:
                itemPanel.GetComponent<Image>().sprite = setOfLevelEditor.landscapeSOs[itemsIndex].landscapeIcon;
                AssignActions(itemPanel, setOfLevelEditor.landscapeSOs[itemsIndex].landscapeName);
                break;
            case CharacterSO:
                itemPanel.GetComponent<Image>().sprite = setOfLevelEditor.characterSOs[itemsIndex].characterIcon;
                AssignActions(itemPanel, setOfLevelEditor.characterSOs[itemsIndex].characterName);
                break;
        }
        itemPanel.GetComponent<Button>().onClick.AddListener(() => levelRedactor.SetFlyingItem(itemType, itemsIndex));
    }
    public void SwitchThePanel ()
    {
        foreach(GameObject panel in itemsContentPanels)
        {
            if(panel != null)
            {
                panel.SetActive(false);
            }
        }
        itemsContentPanels[itemsDropdown.value].SetActive(true);
    }

    public void AssignActions (GameObject itemPanel, string itemName)
    {
        // Получаем компонент EventTrigger
        EventTrigger eventTrigger = itemPanel.GetComponent<EventTrigger>();

        // Проверяем, что компонент EventTrigger присутствует
        if(eventTrigger != null)
        {
            // Добавляем слушатель события OnPointerEnter
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data, itemName); });
            eventTrigger.triggers.Add(entry);

            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(exit);
        }
    }
    public void OnPointerEnterDelegate (PointerEventData data, string itemName)
    {
        flyingText = Instantiate(flyingTextPrefab, this.gameObject.transform);
        flyingText.GetComponent<FlyingTextPrefab>().TMPtext.text = itemName;
    }
    public void OnPointerExitDelegate (PointerEventData data)
    {
        Destroy(flyingText);
    }
}
