using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelRedactor:MonoBehaviour
{
    [Header("SetInInspector")]
    public MapSaveAndLoad saveAndLoad;
    public GameManager gameManager;
    public GameObject flyingItemPrefab;
    public GameObject mapAnchorPrefab;
    public GameObject valueSettingPanel;
    public GameObject menuPanel;
    public GameObject levelItemsPanelGO;
    public Transform mainCameraTransform;
    public TMP_InputField horizontalInputField;
    public TMP_InputField verticalInputField;
    public Color[] teamColors; // � ���������� ����������� ����� ������
    public LevelItemsPanel levelItemsPanel;

    [Header("SetDynamically")]
    public ScriptableObject scriptableObjectInHand;
    public MapAnchor mapAnchor;
    public int indexItemInHand;
    public int horizontalNumer;
    public int verticalNumber;
    public GameObject currentHostedItem;
    public FlyingItem flyingItem;
    public static Character redactingCharacter; // ��������, ������� ��������� � ������ ��������������
    public LevelItemsPanel levelItemsPanelScr;
    private void Update ()
    {
        if(currentHostedItem != null && Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(currentHostedItem);
            currentHostedItem = null;
            flyingItem = null;
        }
        if(GameManager.currentGameState == GameManager.GameState.LevelRedactor && Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameManager.currentCharacterRedactor != null)
            {
                gameManager.currentCharacterRedactor.DestroyCharacterRedactor();
            }
            else
            {
                menuPanel.SetActive(levelItemsPanelGO.activeSelf);
                levelItemsPanelGO.SetActive(!menuPanel.activeSelf);
            }
        }
    }
    public void SetFlyingItem (ScriptableObject itemType, int itemIndex)
    {
        if(currentHostedItem != null)
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
                scriptableObjectInHand = itemType;
                break;
        }
        indexItemInHand = itemIndex;
        flyingItem = currentHostedItem.GetComponent<FlyingItem>();
        flyingItem.levelRedactor = this;
    }

    public void FormCanvas ()
    {
        int x; int y;
        x = int.Parse(horizontalInputField.text);
        y = int.Parse(verticalInputField.text);
        if(x > 0)
        {
            if(y > 0)
            {
                horizontalInputField.GetComponent<Image>().color = Color.white;
                verticalInputField.GetComponent<Image>().color = Color.white;
                valueSettingPanel.SetActive(false);
                levelItemsPanel.gameObject.SetActive(true);
                mapAnchor = Instantiate(mapAnchorPrefab).GetComponent<MapAnchor>();
                gameManager.mapAnchor = mapAnchor.gameObject;
                gameManager.mapAnchorScr = mapAnchor;
                InstantiateMap(x, y, true);
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

    public void InstantiateMap (int x, int y, bool newMap)
    {
        mapAnchor.horizontalNumer = x;
        mapAnchor.verticalNumber = y;
        saveAndLoad.mapAnchor = mapAnchor;
        mapAnchor.levelRedactor = this;
        mapAnchor.ApplyMapParameters(newMap);
    }
}