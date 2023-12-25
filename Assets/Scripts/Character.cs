using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("SetInInspector")]
    public GameObject bodyGO;

    [Header("SetDynamically")]
    public string characterName;
    public int teamNumber; // ����� �������
    public int endurance; // ������������
    public float health; // ��������
    public float mana;  // ����
    public float speed;// ��������
    public float movementPoints;//���� ������������
    public float attackPower;  // ���� �����
    public float attackRange; // ��������� �����
    public float initiative; // ����������
    public Vector2[] path;  // ������� �������� 
    public LandscapeCell currentLandscapeCell; // ������� ������ ������
    private CharacterSO _characterSO;
    public CharacterSO characterSO
    {
        get
        {
            return _characterSO;
        }
        set
        {
            _characterSO = value;
            SetParameters();
        }
    }

    public void SetParameters ()
    {
        bodyGO.GetComponent<SpriteRenderer>().sprite = characterSO.characterImage;
    }
}
