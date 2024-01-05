using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackMode:MonoBehaviour
{
    [Header("SetInInspector")]
    public Character character;
    public LineRenderer lineRenderer;
    public GameObject scopeOfApplication;

    [Header("SetDynamically")]
    public Character targetCharacter;
    [SerializeField]private bool _attackIsOn;
    [SerializeField]private bool _abilityIsOn;
    private int _abilitiesIndex;
    public AbilitiesSO currentAbility;
    public Vector3 characterPos;
    public LayerMask layerMask;
    public int abilitiesIndex
    {
        get
        {
            return (_abilitiesIndex);
        }
        set
        {
            _abilitiesIndex = value;
            currentAbility = character.characterSO.abilities[abilitiesIndex];
        }
    }
    public bool attackIsOn
    {
        get
        {
            return (_attackIsOn);
        }
        set
        {
            _attackIsOn = value;
            if (!_attackIsOn)
            {
                lineRenderer.positionCount = 1;
            }
            else
            {
                lineRenderer.positionCount = 2;
            }
        }
    }
    public bool abilityIsOn
    {
        get
        {
            return (_abilityIsOn);
        } 
        set
        {
            _abilityIsOn = value;
            if(!_abilityIsOn)
            {
                lineRenderer.positionCount = 1;
            }
            else
            {
                lineRenderer.positionCount = 2;
            }
            if (!_abilityIsOn)
            {
                DisplayTheRadius(_abilityIsOn);
            }
        }
    }

    private void Start ()
    {
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        layerMask += 1 << 12;
        layerMask += 1 << 15;
    }
    private void Update ()
    {
        if(attackIsOn)
        {
            // Атака включена
            DrowSightingLine(character.attackRange);
            if(Input.GetMouseButtonDown(0) && targetCharacter != null)
            {
                Attack(targetCharacter);
            }
        }
        if (abilityIsOn)
        {
            ShowTheScopeOfApplication(currentAbility.abilitiesIndex);

        }
    }
    public void DrowSightingLine (float range)
    {
        targetCharacter = null;
        characterPos = this.gameObject.transform.position;
        lineRenderer.SetPosition(0, characterPos);
        characterPos.z = 0;

        Vector3 mousPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector3 direction = (mousPos - characterPos).normalized;
        float dist = Vector2.Distance(characterPos, mousPos);
        direction.z = 0;
        RaycastHit2D hit = Physics2D.Raycast(characterPos, direction, range/5, layerMask);
        if(hit.collider != null && dist > Vector2.Distance(hit.point, characterPos))
        {
            Vector2 hitpoint = hit.point;
            lineRenderer.SetPosition(1, new Vector3(hitpoint.x, hitpoint.y, -1));
            if (hit.collider.gameObject.GetComponent<Character>() != null)
            {
                targetCharacter = hit.collider.gameObject.GetComponent<Character>();
            }
        }
        else
        {
            if(dist > (range / 5))
            {
                lineRenderer.SetPosition(1, new Vector3(characterPos.x + direction.x * (range / 5), characterPos.y + direction.y * (range / 5), -1));
            }
            else
            {
                lineRenderer.SetPosition(1, new Vector3(mousPos.x, mousPos.y, -1));
            }
        }
    }
    // Отображает радиус применения
    public void DisplayTheRadius (bool on, float radius = 0)
    {
        scopeOfApplication.SetActive(on);
        scopeOfApplication.transform.localScale = Vector3.one * (radius / 5);
    }
    public void Attack (Character target)
    {
        if (target.teamNumber != character.teamNumber)
        {
            target.currentHealth -= character.attackPower;
        }
    }
    public void ShowTheScopeOfApplication (int abilitiesIndex)
    {
        switch(abilitiesIndex)
        {
            case 0:
                DrowSightingLine(currentAbility.rangeOfApplication);
                break;
            case 1:
                DrowSightingLine(currentAbility.rangeOfApplication);
                break;
            case 2:
                DrowSightingLine(currentAbility.rangeOfApplication);
                break;
            case 3:
                DisplayTheRadius(_abilityIsOn, currentAbility.rangeOfApplication);
                break;
            case 5:
                DisplayTheRadius(_abilityIsOn, currentAbility.rangeOfApplication);
                break;
            case 6:

                break;
        }
        if(Input.GetMouseButtonDown(0))
        {
            ApplyTheAbility(abilitiesIndex);
        }
    }
    public void ApplyTheAbility (int abilitiesIndex)
    {

        bool exit = false;
        GatherResources();
        switch(abilitiesIndex)
        {
            case 0:

                RaycastHit hit;
                LayerMask mask = 1 << 15;

                if(Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), out hit, mask))
                {
                    Debug.Log($"То, что нужно {hit}");

                    hit.collider.gameObject.GetComponent<Character>().currentHealth -= currentAbility.firstDamage;
                    Collider2D[] characters = Physics2D.OverlapCircleAll(hit.collider.transform.position, currentAbility.radius, mask);
                    foreach(Collider2D collider in characters)
                    {
                        Character targetChar = collider.gameObject.GetComponent<Character>();
                        if (targetChar != character)
                        {
                            targetChar.currentHealth -= currentAbility.secondDamage;
                        }
                    }
                    exit = true;
                }
                break;
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            case 4:

                break;
            case 5:

                break;
            case 6:

                break;
            case 7:

                break;
        }
        if (exit)
        {
            abilityIsOn = false;
            currentAbility = null;
        }
    }
    public void GatherResources ()
    {
        character.currentMana -= currentAbility.requiredMana;
        character.currentEdurance -= currentAbility.requiredEndurance;
    }
}
