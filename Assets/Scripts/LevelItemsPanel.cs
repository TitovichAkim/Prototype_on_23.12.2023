using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelItemsPanel : MonoBehaviour
{
    [Header("SetInInspector")]
    public GameObject itemsContentPrefab;
    public GameObject itemPrefab;
    public SetOfLevelEditor setOfLevelEditor;
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
                CreateAnItemPanel(landscapeItemsContent.GetComponent<ItemsContentPanel>().dropdownContent, i);
            }
            itemsContentPanels[0] = landscapeItemsContent;
        }
        if(setOfLevelEditor.landscapeSOs.Length != 0)
        {
            GameObject characterItemsContent = Instantiate(itemsContentPrefab, this.gameObject.transform);
            for(int i = 0; i < setOfLevelEditor.characterSOs.Length; i++)
            {
                CreateAnItemPanel(characterItemsContent, i);
            }
            itemsContentPanels[0] = characterItemsContent;
            characterItemsContent.GetComponent<ItemsContentPanel>().teamNumberDropdown.SetActive(false);
            characterItemsContent.SetActive(false);
        }
    }
    public void CreateAnItemPanel (GameObject parentGO, int itemsIndex)
    {
        GameObject itemPanel = Instantiate(itemPrefab, parentGO.transform);
        itemPanel.name = $"Item{itemsIndex}";
    }
}
