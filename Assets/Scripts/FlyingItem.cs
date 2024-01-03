using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlyingItem : MonoBehaviour
{
    public bool paintOver;
    public LevelRedactor levelRedactor;
    public LandscapeSO landscapeSO;
    private Transform thisTransform;

    private void Awake ()
    {
        thisTransform = this.gameObject.transform;
    }
    private void Update ()
    {
        paintOver = Input.GetMouseButton(1);

        Vector2 currentCursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        thisTransform.position = currentCursorPosition + new Vector2(0.25f, 0.25f);
    }
}
