using UnityEngine;
using TMPro;

public class FlyingTextForAbility : MonoBehaviour
{
    public TMP_Text tMP_Text;
    private Transform thisTransform;
    private void Awake ()
    {
        thisTransform = this.gameObject.transform;
    }
    private void Update ()
    {
        Vector2 currentCursorPosition = Input.mousePosition;
        thisTransform.position = currentCursorPosition + new Vector2(thisTransform.GetComponent<RectTransform>().sizeDelta.x / 2 + 5, thisTransform.GetComponent<RectTransform>().sizeDelta.y / 2 + 5);
    }
}