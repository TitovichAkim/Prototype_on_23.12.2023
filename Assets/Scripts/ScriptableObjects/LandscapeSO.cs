using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Landscape", menuName = "ScriptableObjects/Landscape")]

public class LandscapeSO :ScriptableObject
{
    public Sprite landscapeIcon;
    public Sprite landscapeImage;
    public string landscapeName; // ��������
    public bool surmountable; // �������������
    public bool shootingRange; // �����������������
    public int slowingDown; // ����������
    public int landscapeIndex; // ������ ������ ��� ����������
}