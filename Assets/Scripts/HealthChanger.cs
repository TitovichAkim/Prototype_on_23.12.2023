using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthChanger : MonoBehaviour
{
    public TMP_Text number;
    public Transform thisTransform;
    public float timer = 1;

    private void FixedUpdate ()
    {
        thisTransform.localPosition += Vector3.up * Time.deltaTime;
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }

}
