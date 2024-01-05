using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInterFace : MonoBehaviour
{
    [Header("SetInInspector")]
    public GameManager gameManager;
    public GameObject queuePanel;
    public GameObject currentCharacterPanel;
    public GameObject CharacterImageInQueuePrefab;

    [Header("SetDynamically")]
    public MapAnchor mapAnchor;

    public void DownloadTheInterface (List<Character> sortedList)
    {
        for(int i = 0; i < sortedList.Count; i++)
        {
            if (sortedList[i].portraitInTheQueueGO == null) // ���� � ��������� �� ���������� ��������
            {
                sortedList[i].portraitInTheQueueGO = Instantiate(CharacterImageInQueuePrefab, queuePanel.transform); // ������� ��������
                sortedList[i].portraitInTheQueueScr = sortedList[i].portraitInTheQueueGO.GetComponent<PortraitInTheQueue>(); // ��������� ������ �� ������
                sortedList[i].portraitInTheQueueScr.character = sortedList[i]; // ��������� ������ �� ��������� � ��������
            }
            sortedList[i].portraitInTheQueueGO.transform.SetParent(queuePanel.transform); // ���������� ������� ��������� ��� ������
            sortedList[i].portraitInTheQueueGO.transform.SetSiblingIndex(0); // ���������� � ������ (i == 0 - ����� ������ ����������, � �������� ����� �� �������� � �����)
            if(i == sortedList.Count - 1)
            {
                sortedList[i].portraitInTheQueueGO.transform.SetParent(currentCharacterPanel.transform); // ���������� ����� ���� ��������� ��� ������
            }
        }
    }
}
