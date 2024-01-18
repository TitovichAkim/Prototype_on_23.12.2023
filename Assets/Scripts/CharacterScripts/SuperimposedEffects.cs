using System.Collections.Generic;
using UnityEngine;

public class SuperimposedEffects:MonoBehaviour
{
    [Header("SetInInspector")]
    public Character character;
    public GameObject effectPanelPrefab;

    [Header("SetDynamically")]
    public List<Character> listOfInfectedEnemies = new List<Character>(); // Список врагов, которым нужно нанести урон в начале следующего хода

    [SerializeField]private List<Vector3> _slowingDownInCycles = new List<Vector3>(); // Замедление на x процентов на y циклов персонажа 
    [SerializeField]private Vector3 _slowingDownFromEndurance; // Замедление на x процентов на y ходов (выносливости) персонажа
    [SerializeField]private Vector3 _staticEndGetDamage; // Получить урон в конце каждого хода x - сила, y - количество ходов, z - радиус
    [SerializeField]private Vector3 _staticStartSetDamage; // Урон в начале каждого хода x - сила, y - количество ходов, z - радиус
    [SerializeField]private List<Vector3> _lossOfEndurance = new List<Vector3>(); // Эффект потери выносливости в начале хода x - на сколько, y - сколько ходов осталось

    [SerializeField]private List<Vector3> _boost = new List<Vector3>(); // Ускорение на x процентов на y циклов
    [SerializeField]private Vector3 _shield; // Щит на x пунктов на y циклов
    public List<Vector3> slowingDownInCycles
    {
        get
        {
            return (_slowingDownInCycles);
        }
        set
        {
            _slowingDownInCycles = value;
            if (slowingDownInCycles.Count > 0)
            {
                for(int i = slowingDownInCycles.Count -1; i > -1; i--)
                {
                    if(slowingDownInCycles[i].y == 0)
                    {
                        slowingDownInCycles.RemoveAt(i);
                        TransferTheEffectToTheInterface(0, slowingDownInCycles.Count > 0, 0, slowingDownInCycles.Count);
                    }
                    else
                    {
                        TransferTheEffectToTheInterface(0, slowingDownInCycles.Count > 0, slowingDownInCycles[i].y, slowingDownInCycles.Count);
                    }
                }
            }
        }
    }
    public Vector3 slowingDownFromEndurance
    {
        get
        {
            return (_slowingDownFromEndurance);
        }
        set
        {
            _slowingDownFromEndurance = value;
            TransferTheEffectToTheInterface(1, slowingDownFromEndurance.y > 0, slowingDownFromEndurance.y);
        }
    }
    public Vector3 staticEndGetDamage
    {
        get
        {
            return (_staticEndGetDamage);
        }
        set
        {
            _staticEndGetDamage = value;
            TransferTheEffectToTheInterface(2, staticEndGetDamage.y > 0, staticEndGetDamage.y);
        }
    }
    public Vector3 staticStartSetDamage
    {
        get
        {
            return (_staticStartSetDamage);
        }
        set
        {
            _staticStartSetDamage = value;
            TransferTheEffectToTheInterface(3, staticStartSetDamage.y > 0, staticStartSetDamage.y);
        }
    }
    public List<Vector3> lossOfEndurance
    {
        get
        {
            return (_lossOfEndurance);
        }
        set
        {
            _lossOfEndurance = value;
            for(int i = lossOfEndurance.Count - 1; i > -1; i--)
            {
                if(lossOfEndurance[i].y == 0)
                {
                    lossOfEndurance.RemoveAt(i);
                    TransferTheEffectToTheInterface(4, lossOfEndurance.Count > 0, 0, lossOfEndurance.Count);
                }
                else
                {
                    TransferTheEffectToTheInterface(4, lossOfEndurance.Count > 0, lossOfEndurance[i].y, lossOfEndurance.Count);
                }
            }
        }
    }

    public List<Vector3> boost
    {
        get
        {
            return (_boost);
        }
        set
        {
            _boost = value;

            for(int i = boost.Count - 1; i > -1; i--)
            {
                if(boost[i].y == 0)
                {
                    boost.RemoveAt(i);
                    TransferTheEffectToTheInterface(5, boost.Count > 0, 0, boost.Count);
                }
                else
                {
                    TransferTheEffectToTheInterface(5, boost.Count > 0, boost[i].y, boost.Count);
                }
            }
        }
    }
    public Vector3 shield
    {
        get
        {
            return (_shield);
        }
        set
        {
            _shield = value;
            character.personalCharactersCanvas.shieldNumberText.gameObject.SetActive(shield.x > 0);
            character.personalCharactersCanvas.targetShieldNumberText.gameObject.SetActive(shield.x > 0);
            character.personalCharactersCanvas.shieldNumberText.text = $"+{shield.x}";
            character.personalCharactersCanvas.targetShieldNumberText.text = $"+{shield.x}";

            TransferTheEffectToTheInterface(6, shield.y > 6, shield.y);
        }
    }
    public void RecalculateEffectsByEndCycle ()
    {
        for(int i = 0; i < slowingDownInCycles.Count; i++)
        {
            slowingDownInCycles[i] = vector3Changer(slowingDownInCycles[i]);
        }
        slowingDownInCycles = slowingDownInCycles;
        staticEndGetDamage = vector3Changer(staticEndGetDamage);
        staticStartSetDamage = vector3Changer(staticStartSetDamage);
        for(int i = 0; i < boost.Count; i++)
        {
            boost[i] = vector3Changer(boost[i]);
        }
        boost = boost;
        shield = vector3Changer(shield);
    }
    public void RecalculateEffectsByMoves (float points)
    {
        slowingDownFromEndurance = vector3Changer(slowingDownFromEndurance, points);
        for(int i = 0; i < boost.Count; i++)
        {
            boost[i] = vector3Changer(boost[i], points);
        }
        boost = boost;
    }

    public Vector3 vector3Changer (Vector3 inputVector, float inputChanger = 1)
    {
        Debug.Log($"Было {inputVector.y}");
        Vector3 result = Vector3.zero;
        if(inputVector.y - inputChanger > 0)
        {
            result = inputVector;
            result.y -= inputChanger;
        }
        return result;
    }

    public void TransferTheEffectToTheInterface (int effectIndex, bool enableEffect, float durationOfTheEffect, int effectsNumber = 0)
    {
        if (character.personalCharactersCanvas != null)
        {
            if(enableEffect)
            {
                if(character.personalCharactersCanvas.effectsGOs[effectIndex] != null)
                {
                    ChangeEffectPanel(effectIndex, durationOfTheEffect, effectsNumber);
                }
                else
                {
                    character.personalCharactersCanvas.effectsGOs[effectIndex] = Instantiate(effectPanelPrefab, character.personalCharactersCanvas.effectsPanelTransform);
                    character.personalCharactersCanvas.targetEffectsGOs[effectIndex] = Instantiate(effectPanelPrefab, character.personalCharactersCanvas.currentEffectsTransform);
                    ChangeEffectPanel(effectIndex, durationOfTheEffect, effectsNumber);
                }
            }
            else
            {
                if(character.personalCharactersCanvas.effectsGOs[effectIndex] != null)
                {
                    Destroy(character.personalCharactersCanvas.effectsGOs[effectIndex]);
                    character.personalCharactersCanvas.effectsGOs[effectIndex] = null;
                    Destroy(character.personalCharactersCanvas.targetEffectsGOs[effectIndex]);
                    character.personalCharactersCanvas.targetEffectsGOs[effectIndex] = null;
                }
            }
        }
    }
    public void ChangeEffectPanel (int effectIndex, float durationOfTheEffect = 0, int effectsNumber = 0)
    {
        Debug.Log($"Меняю цифры на {effectIndex} {durationOfTheEffect} {effectsNumber}");
        character.personalCharactersCanvas.effectsGOs[effectIndex].GetComponent<EffectItems>().effectNumberTextGO.SetActive(effectsNumber > 1);
        character.personalCharactersCanvas.effectsGOs[effectIndex].GetComponent<EffectItems>().effectNumberText.text = effectsNumber.ToString("F0");
        character.personalCharactersCanvas.effectsGOs[effectIndex].GetComponent<EffectItems>().durationOfTheEffectText.text = durationOfTheEffect.ToString("F0");

        character.personalCharactersCanvas.targetEffectsGOs[effectIndex].GetComponent<EffectItems>().effectNumberTextGO.SetActive(effectsNumber > 1);
        character.personalCharactersCanvas.targetEffectsGOs[effectIndex].GetComponent<EffectItems>().effectNumberText.text = effectsNumber.ToString("F0");
        character.personalCharactersCanvas.targetEffectsGOs[effectIndex].GetComponent<EffectItems>().durationOfTheEffectText.text = durationOfTheEffect.ToString("F0");
    }
}
