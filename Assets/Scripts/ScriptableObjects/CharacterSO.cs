using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character")]

public class CharacterSO : ScriptableObject
{
    public Sprite characterIcon;
    public Sprite characterImage;
    public string characterName;
    public int endurance; // ������������
    public float health; // ��������
    public float mana; // ����
    public float speed; // ��������
    public float movementPoints; //���� ������������
    public float attackPower; // ���� �����
    public float attackRange; // ��������� �����
    public float initiative; // ����������
}