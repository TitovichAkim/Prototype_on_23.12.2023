using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character")]

public class CharacterSO : ScriptableObject
{
    public Sprite characterIcon;
    public Sprite characterImage;
    public string characterName;
    public int endurance; // Выносливость
    public float health; // здоровье
    public float mana; // Мана
    public float speed; // скорость
    public float movementPoints; //Очки передвижения
    public float attackPower; // Сила атаки
    public float attackRange; // Дальность атаки
    public float initiative; // Инициатива
}