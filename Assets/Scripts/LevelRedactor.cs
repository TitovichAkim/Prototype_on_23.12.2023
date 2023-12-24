using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelRedactor:MonoBehaviour
{
    [Header("SetInInspector")]
    public GameObject flyingItemPrefab;
    public GameObject mapAnchorPrefab;
    public GameObject valueSettingPanel;
    public Transform mainCameraTransform;
    public TMP_InputField horizontalInputField;
    public TMP_InputField verticalInputField;

    public LevelItemsPanel levelItemsPanel;

    [Header("SetDynamically")]
    public ScriptableObject scriptableObjectInHand;
    public MapAnchor mapAnchor;
    public int indexItemInHand;
    public GameObject currentHostedItem;

    public void SetFlyingItem (ScriptableObject itemType, int itemIndex)
    {
        if (currentHostedItem != null)
        {
            Destroy(currentHostedItem);
        }
        currentHostedItem = Instantiate(flyingItemPrefab);
        switch(itemType)
        {
            case LandscapeSO:
                currentHostedItem.GetComponent<SpriteRenderer>().sprite = levelItemsPanel.setOfLevelEditor.landscapeSOs[itemIndex].landscapeIcon;
                scriptableObjectInHand = itemType;
                break;
            case CharacterSO:
                currentHostedItem.GetComponent<SpriteRenderer>().sprite = levelItemsPanel.setOfLevelEditor.characterSOs[itemIndex].characterIcon;
                break;
        }
        indexItemInHand = itemIndex;
    }

    public void FormCanvas ()
    {
        int x = int.Parse(horizontalInputField.text);
        int y = int.Parse(verticalInputField.text);
        if(x > 0)
        {
            if(y > 0)
            {
                horizontalInputField.GetComponent<Image>().color = Color.white;
                verticalInputField.GetComponent<Image>().color = Color.white;
                valueSettingPanel.SetActive(false);
                levelItemsPanel.gameObject.SetActive(true);
                InstantiateMap(x, y);
            }
            else
            {
                horizontalInputField.GetComponent<Image>().color = Color.white;
                verticalInputField.GetComponent<Image>().color = Color.red;
            }
        }
        else
        {
            horizontalInputField.GetComponent<Image>().color = Color.red;

            if(int.Parse(verticalInputField.text) > 0)
            {
                verticalInputField.GetComponent<Image>().color = Color.white;
            }
            else
            {
                verticalInputField.GetComponent<Image>().color = Color.red;
            }
        }
        mainCameraTransform.position = new Vector3(x / 2, y / 2, -10);
    }

    private void InstantiateMap (int x, int y)
    {
        mapAnchor = Instantiate(mapAnchorPrefab).GetComponent<MapAnchor>();
        mapAnchor.horizontalNumer = x;
        mapAnchor.verticalNumber = y;
        mapAnchor.levelRedactor = this;
        mapAnchor.ApplyMapParameters();
    }
}