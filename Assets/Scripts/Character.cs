using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("SetInInspector")]
    public GameObject bodyGO;

    [Header("SetDynamically")]
    public string characterName;
    public int teamNumber; // Номер команды
    public int endurance; // Выносливость
    public float health; // здоровье
    public float mana;  // Мана
    public float speed;// скорость
    public float movementPoints;//Очки передвижения
    public float attackPower;  // Сила атаки
    public float attackRange; // Дальность атаки
    public float initiative; // Инициатива
    public Vector2[] path;  // Маршрут движения 
    public LandscapeCell currentLandscapeCell; // Текущая ячейка игрока
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
