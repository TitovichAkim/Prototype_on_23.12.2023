using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LesionArea : MonoBehaviour
{
    public Character parentCharacter;
    public List<Character> charactersInZone = new List<Character>();
    public PolygonCollider2D polygonCollider2D;
    public Transform thisTransform;

    private void FixedUpdate ()
    {
        // �������� ������� ������� � ������� �����������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // ������� ������, ����������� �� ������� � ������� �������
        Vector3 direction = mousePosition - transform.position;
        // ������� ���� ����� �������� ����������� � ���� x
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // ������������ ������ � ����������� �������
        thisTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }
    public void OnTriggerEnter2D (Collider2D collision)
    {
        if(collision.CompareTag("Character"))
        {
            Character target = collision.GetComponent<Character>();
            if(target.teamNumber != parentCharacter.teamNumber)
            {
                charactersInZone.Add(target);
            }
        }
    }
    public void OnTriggerExit2D (Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            Character target = collision.GetComponent<Character>();
            charactersInZone.Remove(target);
        }
    }
}
