using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyingTextPrefab : MonoBehaviour
{
    public TMP_Text TMPtext;
    private Transform thisTransform;
    
    private void Awake ()
    {
        thisTransform = this.gameObject.transform;
    }
    private void Update ()
    {
        Vector2 currentCursorPosition = Input.mousePosition;
        thisTransform.position = currentCursorPosition + new Vector2(-100, 30);
    }
}
