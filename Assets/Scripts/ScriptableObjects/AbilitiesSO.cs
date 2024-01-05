using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Abilities", menuName = "ScriptableObjects/Abilities")]

public class AbilitiesSO : ScriptableObject
{
    public string abilitiesName; //название способности
    public int abilitiesIndex; // Индекс способности
    public int recharge; // перезарядка
    public float requiredMana; // Требуется маны
    public float requiredEndurance; // Требуемая выносливость
    public float firstDamage; // Первичный урон
    public float secondDamage; // Вторичный урон
    public float rangeOfApplication; // Дальность применения 
    public float radius; // Радиус применения
    public Vector2 slowingDown; // Замедление на x процентов на время затраты указанного параметра на y 
    public Vector2 boost; // Ускорение на x процентов на y ходов
    public Vector2 shield; // Щит на x пунктов на y ходов
    public int durationOfTheEffect; // длительность эффекта
    public bool instantAction; // Моментального действия
}
