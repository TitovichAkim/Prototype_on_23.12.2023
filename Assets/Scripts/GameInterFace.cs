using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInterFace : MonoBehaviour
{
    [Header("SetInInspector")]
    public GameManager gameManager;
    public GameObject menuButton;
    public GameObject menuPanel;

    public GameObject queuePanel;
    public GameObject currentCharacterPanel;
    public GameObject CharacterImageInQueuePrefab;

    [Header("SetDynamically")]
    public MapAnchor mapAnchor;

    private void Update ()
    {
        if(GameManager.currentGameState == GameManager.GameState.Game)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(gameManager.currentCharacterRedactor != null)
                {
                    gameManager.currentCharacterRedactor.DestroyCharacterRedactor();
                }
                else
                {
                    menuButton.SetActive(menuPanel.activeSelf);
                    menuPanel.SetActive(!menuButton.activeSelf);
                }
            }
        }
    }
    public void DownloadTheInterface (List<Character> sortedList)
    {
        for(int i = 0; i < sortedList.Count; i++)
        {
            if (sortedList[i].portraitInTheQueueGO == null) // Если у персонажа не существует панельки
            {
                sortedList[i].portraitInTheQueueGO = Instantiate(CharacterImageInQueuePrefab, queuePanel.transform); // создать панельку
                sortedList[i].portraitInTheQueueScr = sortedList[i].portraitInTheQueueGO.GetComponent<PortraitInTheQueue>(); // заполнить ссылку на скрипт
                sortedList[i].portraitInTheQueueScr.character = sortedList[i]; // заполнить ссылку на персонажа в панельке
            }
            sortedList[i].portraitInTheQueueGO.transform.SetParent(queuePanel.transform); // Установить очередь родителем для панели
            sortedList[i].portraitInTheQueueGO.transform.SetSiblingIndex(0); // Установить в начало (i == 0 - самая низкая инициатива, в конечном итоге он окажется в конце)
            if(i == sortedList.Count - 1)
            {
                sortedList[i].portraitInTheQueueGO.transform.SetParent(currentCharacterPanel.transform); // Установить левое окно родителем для панели
            }
        }
    }
}
