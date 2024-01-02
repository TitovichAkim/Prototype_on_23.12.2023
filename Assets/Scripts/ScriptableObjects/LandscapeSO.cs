using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Landscape", menuName = "ScriptableObjects/Landscape")]

public class LandscapeSO :ScriptableObject
{
    public Sprite landscapeIcon;
    public Sprite landscapeImage;
    public string landscapeName; // название
    public bool surmountable; // преодолимость
    public bool shootingRange; // Простреливаемость
    public int slowingDown; // замедление
    public int landscapeIndex; // Индекс ячейки для сохранения
}