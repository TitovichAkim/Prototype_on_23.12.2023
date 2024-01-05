using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Abilities", menuName = "ScriptableObjects/Abilities")]

public class AbilitiesSO : ScriptableObject
{
    public string abilitiesName; //�������� �����������
    public int abilitiesIndex; // ������ �����������
    public int recharge; // �����������
    public float requiredMana; // ��������� ����
    public float requiredEndurance; // ��������� ������������
    public float firstDamage; // ��������� ����
    public float secondDamage; // ��������� ����
    public float rangeOfApplication; // ��������� ���������� 
    public float radius; // ������ ����������
    public Vector2 slowingDown; // ���������� �� x ��������� �� ����� ������� ���������� ��������� �� y 
    public Vector2 boost; // ��������� �� x ��������� �� y �����
    public Vector2 shield; // ��� �� x ������� �� y �����
    public int durationOfTheEffect; // ������������ �������
    public bool instantAction; // ������������� ��������
}
