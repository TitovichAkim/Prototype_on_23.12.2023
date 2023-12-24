using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelItemsPanel : MonoBehaviour
{
    [Header("SetInInspector")]
    public GameObject itemsContentPrefab;
    public GameObject itemPrefab;
    public TMP_Dropdown itemsDropdown;
    public SetOfLevelEditor setOfLevelEditor;
    public LevelRedactor levelRedactor;
    [Header("SetDynamically")]
    public GameObject[] itemsContentPanels;
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
                break;
            case CharacterSO:
                itemPanel.GetComponent<Image>().sprite = setOfLevelEditor.characterSOs[itemsIndex].characterIcon;
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
}
