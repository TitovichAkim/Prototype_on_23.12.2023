using UnityEngine;
using UnityEngine.EventSystems;

public class AttackMode:MonoBehaviour
{
    [Header("SetInInspector")]
    public Character character;
    public LineRenderer lineRenderer;
    public GameObject scopeOfApplication;
    public GameObject lesionAreaGO;
    public LesionArea lesionAreaScr;

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
            if(!_attackIsOn)
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
            if(!_abilityIsOn)
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
        if(abilityIsOn)
        {
            ShowTheScopeOfApplication(currentAbility.abilitiesIndex);

        }
    }
    public void DrowSightingLine (float range)
    {
        if(lineRenderer.positionCount > 1)
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
                if(hit.collider.gameObject.GetComponent<Character>() != null)
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
    }
    // Отображает радиус применения
    public void DisplayTheRadius (bool on, float radius = 0)
    {
        scopeOfApplication.SetActive(on);
        scopeOfApplication.transform.localScale = Vector3.one * (radius / 5) * 2;

    }
    public void Attack (Character target)
    {
        if(target.teamNumber != character.teamNumber)
        {
            target.currentHealth -= character.attackPower;
        }
    }
    public void ShowTheScopeOfApplication (int abilitiesIndex)
    {
        lesionAreaGO.SetActive(false);
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
                DisplayTheRadius(_abilityIsOn, currentAbility.radius);
                break;
            case 4:
                DisplayTheRadius(_abilityIsOn, currentAbility.radius);
                break;
            case 5:
                DisplayTheRadius(_abilityIsOn, currentAbility.rangeOfApplication);
                break;
            case 6:
                lesionAreaGO.SetActive(true);
                break;
        }
        if(Input.GetMouseButtonDown(0))
        {
            if(!GameManager.cursorOnUI)
            {
                ApplyTheAbility(abilitiesIndex);
            }
        }
    }
    public void ApplyTheAbility (int abilitiesIndex)
    {
        LayerMask characterMask = 1 << 15;
        LayerMask landscapeMask = 1 << 11;
        Vector3 targetPosition = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, -12);
        RaycastHit2D characterHit = Physics2D.Raycast(targetPosition, Vector2.zero, 11, characterMask);
        RaycastHit2D landscapeHit = Physics2D.Raycast(targetPosition, Vector2.zero, 11, landscapeMask);
        bool exit = false;
        switch(abilitiesIndex)
        {
            case 0:
                if(characterHit && Vector2.Distance(this.gameObject.transform.position, characterHit.transform.position) <= currentAbility.rangeOfApplication / 5)
                {
                    GameObject targetGO = characterHit.collider.gameObject;
                    if(targetGO.GetComponent<Character>().teamNumber != character.teamNumber)
                    {
                        if(targetCharacter != null)
                        {
                            targetGO = targetCharacter.gameObject;
                        }
                        targetGO.GetComponent<Character>().currentHealth -= currentAbility.firstDamage;
                        Collider2D[] characters = Physics2D.OverlapCircleAll(characterHit.collider.transform.position, currentAbility.radius, characterMask);
                        foreach(Collider2D collider in characters)
                        {
                            Character targetChar = collider.gameObject.GetComponent<Character>();
                            if(targetChar != character && targetChar != targetGO.GetComponent<Character>())
                            {
                                targetChar.currentHealth -= currentAbility.secondDamage;
                            }
                        }
                        exit = true;
                    }
                }
                break;
            case 1:
                if(characterHit && Vector2.Distance(this.gameObject.transform.position, characterHit.transform.position) <= currentAbility.radius / 5)
                {
                    GameObject targetGO = characterHit.collider.gameObject;
                    if (targetGO.GetComponent<Character>().teamNumber != character.teamNumber)
                    {
                        if(targetCharacter != null)
                        {
                            targetGO = targetCharacter.gameObject;
                        }
                        Character targetCh = targetGO.GetComponent<Character>();
                        targetCh.currentHealth -= currentAbility.firstDamage;
                        targetCh.superimposedEffects.slowingDownInCycles.Add(currentAbility.slowingDown);
                        targetCh.superimposedEffects.slowingDownInCycles = targetCh.superimposedEffects.slowingDownInCycles;

                        character.superimposedEffects.boost.Add(currentAbility.boost);
                        character.superimposedEffects.boost = character.superimposedEffects.boost;
                        exit = true;
                    }
                }
                break;
            case 2:
                if(characterHit && Vector2.Distance(this.gameObject.transform.position, characterHit.transform.position) <= currentAbility.rangeOfApplication / 5)
                {
                    GameObject targetGO = characterHit.collider.gameObject;
                    if(targetGO.GetComponent<Character>().teamNumber != character.teamNumber)
                    {
                        if(targetCharacter != null)
                        {
                            character.superimposedEffects.listOfInfectedEnemies.Add(targetGO.GetComponent<Character>());
                        }
                        exit = true;
                    }
                }
                break;
            case 3:
                if(landscapeHit 
                    && Vector2.Distance(this.gameObject.transform.position, landscapeHit.transform.position) <= currentAbility.radius / 5 
                    && landscapeHit.collider.gameObject.GetComponent<LandscapeCell>().landscapeSO.surmountable)
                {
                    GameObject targetGO = landscapeHit.collider.gameObject;
                    if(targetGO != null)
                    {
                        TeleportationСharacter(targetGO);
                    }
                    exit = true;
                }
                break;
            case 4: // Большая молотилка
                LayerMask selfMask = 1 << 16;
                RaycastHit2D selfHit = Physics2D.Raycast(targetPosition, Vector2.zero, 11, selfMask);
if(selfHit)
{
                    character.superimposedEffects.staticStartSetDamage = new Vector3(currentAbility.firstDamage, currentAbility.durationOfTheEffect, currentAbility.radius);
                    exit = true;
                }
                break;
            case 5:
                if(landscapeHit 
                    && Vector2.Distance(this.gameObject.transform.position, landscapeHit.transform.position) <= currentAbility.rangeOfApplication / 5 
                    && landscapeHit.collider.gameObject.GetComponent<LandscapeCell>().landscapeSO.surmountable)
                {
                    GameObject targetGO = landscapeHit.collider.gameObject;
                    if(targetGO != null)
                    {
                        TeleportationСharacter(targetGO);
                        DealDamageByArea(characterMask, currentAbility.radius / 5, currentAbility.secondDamage);
                    }
                    exit = true;
                }
                break;
            case 6:
                if (lesionAreaScr.charactersInZone.Count != 0)
                {
                    foreach(Character target in lesionAreaScr.charactersInZone)
                    {
                        target.superimposedEffects.lossOfEndurance.Add(new Vector3 (currentAbility.firstDamage, 1, 0));
                    }
                }
                exit = true;
                break;
            case 7:
                if(abilityIsOn)
                {
                    Collider2D[] characters = Physics2D.OverlapCircleAll(this.gameObject.transform.position, currentAbility.radius/5, characterMask);
                    character.superimposedEffects.shield = currentAbility.shield;
                    foreach(Collider2D collider in characters)
                    {
                        Character targetChar = collider.gameObject.GetComponent<Character>();
                        if(targetChar != this && targetChar.teamNumber != character.teamNumber)
                        {
                            targetChar.superimposedEffects.slowingDownFromEndurance = currentAbility.slowingDown;
                                character.superimposedEffects.shield = new Vector3( character.superimposedEffects.shield.x + 10, 
                                                                                    character.superimposedEffects.shield.y, 
                                                                                    character.superimposedEffects.shield.z);
                        }
                    }
                    exit = true;
                }
                break;
        }
        if(exit)
        {
            GatherResources();
            abilityIsOn = false;
            currentAbility = null;
            lesionAreaGO.SetActive(false);
            character.characterState = Character.CharacterState.Readiness;
        }

    }
    // Телепортирует персонажа в указанную точку
    public void TeleportationСharacter (GameObject targetGO)
    {
        character.currentLandscapeCell = null;
        LandscapeCell targetLandscape = targetGO.GetComponent<LandscapeCell>();
        targetLandscape.currentCharacter = character;
        character.transform.position = targetGO.transform.position;
        character.currentLandscapeCell = targetLandscape;
    }
    // Нанесет урон по области (исключая союзников)
    public void DealDamageByArea (LayerMask characterMask, float radius, float damage)
    {
        Collider2D[] characters = Physics2D.OverlapCircleAll(this.gameObject.transform.position, radius, characterMask);
        foreach(Collider2D collider in characters)
        {
            Character targetChar = collider.gameObject.GetComponent<Character>();
            if(targetChar != this && targetChar.teamNumber != character.teamNumber)
            {
                targetChar.currentHealth -= damage;
            }
        }

    }
    public void GatherResources ()
    {
        character.currentMana -= currentAbility.requiredMana;
        character.currentEdurance -= currentAbility.requiredEndurance;
    }
}
