using System.Collections.Generic;
using UnityEngine;

public class SuperimposedEffects:MonoBehaviour
{
    [Header("SetInInspector")]
    public Character character;
    public GameObject effectPanelPrefab;

    [Header("SetDynamically")]
    public List<EffectItems> targetBoostEffectItem = new List<EffectItems>(); // ����� ������� ��� ������� �� ������ �������� ������������� ����� �� ������ ������
    public List<EffectItems> currentBoostEffectItem = new List<EffectItems>(); // ����� ������� ��� ������� �� ������ �������� ������������� ����� �� ������ ������
    public List<EffectItems> targetSlowingDownEffectItem = new List<EffectItems>(); // ����� ������� ��� ������� �� ������ �������� ������������� ������� �������
    public List<EffectItems> currentSlowingDownEffectItem = new List<EffectItems>(); // ����� ������� ��� ������� �� ������ �������� ������������� ������� �������

    public List<Character> listOfInfectedEnemies = new List<Character>(); // ������ ������, ������� ����� ������� ���� � ������ ���������� ����

    [SerializeField]private List<Vector3> _slowingDownInCycles = new List<Vector3>(); // ���������� �� x ��������� �� y ������ ��������� 0
    [SerializeField]private Vector3 _slowingDownFromEndurance; // ���������� �� x ��������� �� y ����� (������������) ��������� 1
    [SerializeField]private Vector3 _staticEndGetDamage; // �������� ���� � ����� ������� ���� x - ����, y - ���������� �����, z - ������ 2 
    [SerializeField]private Vector3 _staticStartSetDamage; // ���� � ������ ������� ���� x - ����, y - ���������� �����, z - ������ 3 
    [SerializeField]private List<Vector3> _lossOfEndurance = new List<Vector3>(); // ������ ������ ������������ � ������ ���� x - �� �������, y - ������� ����� �������� 4 
    [SerializeField]private int _infectionIsThePunishmentOfHeaven; // �������� �� "���� �����". ��� �������� ���������� ����� ���������� �������� 7

    [SerializeField]private List<Vector3> _boost = new List<Vector3>(); // ��������� �� x ��������� �� y ������ 5
    [SerializeField]private Vector3 _shield; // ��� �� x ������� �� y ������ 6

    public List<Vector3> slowingDownInCycles
    {
        get
        {
            return (_slowingDownInCycles);
        }
        set
        {
            _slowingDownInCycles = value;
            for(int i = slowingDownInCycles.Count - 1; i > -1; i--)
            {
                if(slowingDownInCycles[i].y <= 0)
                {
                    slowingDownInCycles.RemoveAt(i);
                    Destroy(targetSlowingDownEffectItem[i].gameObject);
                    targetSlowingDownEffectItem.RemoveAt(i);
                    Destroy(currentSlowingDownEffectItem[i].gameObject);
                    currentSlowingDownEffectItem.RemoveAt(i);
                }
                else
                {
                    if(targetSlowingDownEffectItem.Count < slowingDownInCycles.Count)
                    {
                        targetSlowingDownEffectItem.Add(Instantiate(effectPanelPrefab, character.personalCharactersCanvas.currentEffectsTransform).GetComponent<EffectItems>());
                    }
                    targetSlowingDownEffectItem[i].durationOfTheEffectText.text = slowingDownInCycles[i].y.ToString("F0");
                    targetSlowingDownEffectItem[i].effectDescription = $"������� �������.\n�������� �� 25%";


                    if(currentSlowingDownEffectItem.Count < slowingDownInCycles.Count)
                    {
                        currentSlowingDownEffectItem.Add(Instantiate(effectPanelPrefab, character.personalCharactersCanvas.effectsPanelTransform).GetComponent<EffectItems>());
                    }
                    currentSlowingDownEffectItem[i].durationOfTheEffectText.text = slowingDownInCycles[i].y.ToString("F0");
                    currentSlowingDownEffectItem[i].effectDescription = $"������� �������.\n�������� �� 25%";

                    if(character.caracterRedactorPanelScr != null)
                    {
                        EffectItems effect = Instantiate(effectPanelPrefab, character.caracterRedactorPanelScr.effectsPanel.transform).GetComponent<EffectItems>();
                        effect.durationOfTheEffectText.text = slowingDownInCycles[i].y.ToString("F0");
                        effect.effectDescription = $"������� �������.\n�������� �� 25%";
                    }
                }
            }
            character.personalCharactersCanvas.speedNumberText.text = character.speed.ToString("F0");

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
            TransferTheEffectToTheInterface(1, slowingDownFromEndurance.y > 0, slowingDownFromEndurance.y, $"������ ����.\n�������� �� 30%");
            character.personalCharactersCanvas.speedNumberText.text = character.speed.ToString("F0");
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
            TransferTheEffectToTheInterface(2, staticEndGetDamage.y > 0, staticEndGetDamage.y, $"��������� ����� � ����� ����");
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
            TransferTheEffectToTheInterface(3, staticStartSetDamage.y > 0, staticStartSetDamage.y, $"������� ���������.\n� ����� ���� ������� 4 ����� ������ �����");
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
                    TransferTheEffectToTheInterface(4, lossOfEndurance.Count > 0, 0, $"����� ������.\n�������� 15 ������������ � ������ ����", lossOfEndurance.Count);
                }
                else
                {
                    TransferTheEffectToTheInterface(4, lossOfEndurance.Count > 0, lossOfEndurance[i].y, $"����� ������.\n�������� 15 ������������ � ������ ����", lossOfEndurance.Count);
                }
            }
        }
    }
    public int infectionIsThePunishmentOfHeaven
    {
        get
        {
            return (_infectionIsThePunishmentOfHeaven);
        }
        set
        {
            _infectionIsThePunishmentOfHeaven = value;
            if (_infectionIsThePunishmentOfHeaven > 0)
            {
                TransferTheEffectToTheInterface(7, true, 1, $"���� �����.\n� ������ ���� ������ ������� 32 �����", infectionIsThePunishmentOfHeaven, false);
            }
            else
            {
                TransferTheEffectToTheInterface(7, false, 0, $"����������� <����� �����>");
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
                if(boost[i].y <= 0)
                {
                    boost.RemoveAt(i);
                    Destroy(targetBoostEffectItem[i].gameObject);
                    targetBoostEffectItem.RemoveAt(i);
                    Destroy(currentBoostEffectItem[i].gameObject);
                    currentBoostEffectItem.RemoveAt(i);
                }
                else
                {
                    if (targetBoostEffectItem.Count < boost.Count)
                    {
                        targetBoostEffectItem.Add(Instantiate(effectPanelPrefab, character.personalCharactersCanvas.currentEffectsTransform).GetComponent<EffectItems>());
                    }
                    targetBoostEffectItem[i].durationOfTheEffectText.text = boost[i].y.ToString("F0");
                    targetBoostEffectItem[i].effectDescription = $"������� �������.\n������� �� 25%";


                    if(currentBoostEffectItem.Count < boost.Count)
                    {
                        currentBoostEffectItem.Add(Instantiate(effectPanelPrefab, character.personalCharactersCanvas.effectsPanelTransform).GetComponent<EffectItems>());
                    }
                    currentBoostEffectItem[i].durationOfTheEffectText.text = boost[i].y.ToString("F0");
                    currentBoostEffectItem[i].effectDescription = $"������� �������.\n������� �� 25%";

                    if(character.caracterRedactorPanelScr != null)
                    {
                        EffectItems effect = Instantiate(effectPanelPrefab, character.caracterRedactorPanelScr.effectsPanel.transform).GetComponent<EffectItems>();
                        effect.durationOfTheEffectText.text = boost[i].y.ToString("F0");
                        effect.effectDescription = $"������� �������.\n������� �� 25%";
                    }
                }
            }
            character.personalCharactersCanvas.speedNumberText.text = character.speed.ToString("F0");
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

            TransferTheEffectToTheInterface(6, shield.y > 0, shield.y, $"������ ����.\n���");
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
        Vector3 result = Vector3.zero;
        if(inputVector.y - inputChanger > 0)
        {
            result = inputVector;
            result.y -= inputChanger;
        }
        return result;
    }

    public void TransferTheEffectToTheInterface (int effectIndex, bool enableEffect, float durationOfTheEffect, string description, int effectsNumber = 0, bool enabledurationOfTheEffectText = true)
    {
        if (character.personalCharactersCanvas != null)
        {
            if(enableEffect)
            {
                if(character.personalCharactersCanvas.effectsGOs[effectIndex] != null)
                {
                    ChangeEffectPanel(effectIndex, description, durationOfTheEffect, effectsNumber, enabledurationOfTheEffectText);
                }
                else
                {
                    character.personalCharactersCanvas.effectsGOs[effectIndex] = Instantiate(effectPanelPrefab, character.personalCharactersCanvas.effectsPanelTransform);
                    character.personalCharactersCanvas.targetEffectsGOs[effectIndex] = Instantiate(effectPanelPrefab, character.personalCharactersCanvas.currentEffectsTransform);
                    ChangeEffectPanel(effectIndex, description, durationOfTheEffect, effectsNumber, enabledurationOfTheEffectText);
                }
                if (character.caracterRedactorPanelScr != null)
                {
                    EffectItems effect = Instantiate(effectPanelPrefab, character.caracterRedactorPanelScr.effectsPanel.transform).GetComponent<EffectItems>();
                    ChangeEffectPanel(effectIndex, description, durationOfTheEffect, effectsNumber, enabledurationOfTheEffectText, effect);
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
    public void ChangeEffectPanel (int effectIndex, string description,  float durationOfTheEffect = 0, int effectsNumber = 0, bool enabledurationOfTheEffectText = true, EffectItems effect = null)
    {
        character.personalCharactersCanvas.effectsGOs[effectIndex].GetComponent<EffectItems>().effectNumberTextGO.SetActive(effectsNumber > 1);
        character.personalCharactersCanvas.effectsGOs[effectIndex].GetComponent<EffectItems>().effectNumberText.text = effectsNumber.ToString("F0");
        character.personalCharactersCanvas.effectsGOs[effectIndex].GetComponent<EffectItems>().durationOfTheEffectText.text = durationOfTheEffect.ToString("F0");
        character.personalCharactersCanvas.effectsGOs[effectIndex].GetComponent<EffectItems>().durationOfTheEffectText.gameObject.SetActive(enabledurationOfTheEffectText);
        character.personalCharactersCanvas.effectsGOs[effectIndex].GetComponent<EffectItems>().effectDescription = description;
        
        character.personalCharactersCanvas.targetEffectsGOs[effectIndex].GetComponent<EffectItems>().effectNumberTextGO.SetActive(effectsNumber > 1);
        character.personalCharactersCanvas.targetEffectsGOs[effectIndex].GetComponent<EffectItems>().effectNumberText.text = effectsNumber.ToString("F0");
        character.personalCharactersCanvas.targetEffectsGOs[effectIndex].GetComponent<EffectItems>().durationOfTheEffectText.text = durationOfTheEffect.ToString("F0");
        character.personalCharactersCanvas.targetEffectsGOs[effectIndex].GetComponent<EffectItems>().durationOfTheEffectText.gameObject.SetActive(enabledurationOfTheEffectText);

        if (effect != null)
        {
            effect.effectNumberTextGO.SetActive(effectsNumber > 1);
            effect.effectNumberText.text = effectsNumber.ToString("F0");
            effect.durationOfTheEffectText.text = durationOfTheEffect.ToString("F0");
            effect.durationOfTheEffectText.gameObject.SetActive(enabledurationOfTheEffectText);
            effect.effectDescription = description;
        }
    }
}
