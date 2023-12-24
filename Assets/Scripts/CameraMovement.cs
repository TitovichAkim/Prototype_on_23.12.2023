using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CameraMovement : MonoBehaviour
{
    [Header("SetInInspector")]
    public float moveSpeed;

    [Header("SetDynamically")]
    private Transform _transform;

    private void Awake ()
    {
        _transform = this.gameObject.transform;
}
private void Update ()
    {
        if(Input.mousePosition.x >= Screen.width)
        {
            _transform.position += moveSpeed * Time.deltaTime * Vector3.right;
        }
        if(Input.mousePosition.y >= Screen.height)
        {
            _transform.position += moveSpeed * Time.deltaTime * Vector3.up;
        }
        if(Input.mousePosition.x <= 0)
        {
            _transform.position += moveSpeed * Time.deltaTime * Vector3.left;
        }
        if(Input.mousePosition.y <= 0)
        {
            _transform.position += moveSpeed * Time.deltaTime * Vector3.down;
        }
    }
}
