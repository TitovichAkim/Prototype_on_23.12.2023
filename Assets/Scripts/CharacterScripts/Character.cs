using UnityEngine;
using UnityEngine.EventSystems;

public class Character:MonoBehaviour
{
    public enum CharacterState
    {
        Expectation, // ожидание
        Readiness, // Готовность
        Movement, // движение
        Attack, // Атака
        Ability, // Умение
        Death // Смерть
    }

    [Header("SetInInspector")]
    public GameObject bodyGO;
    public GameObject activeBackground;
    public GameObject characterRedactorPrefab;
    public GameObject healthChangerPrefab;
    public GameObject personalCharactersCanvasPrefab;
    public EnergyCostCalculator energyCostCalculator;
    public CharacterMovement characterMovement;
    public AttackMode attackMode;
    public SuperimposedEffects superimposedEffects;

    [Header("SetDynamically")]
    public GameObject portraitInTheQueueGO;
    public GameManager gameManager;
    public MapAnchor mapAnchor;
    public PortraitInTheQueue portraitInTheQueueScr;
    public CaracterRedactorPanel caracterRedactorPanel;
    public PersonalCharactersCanvas personalCharactersCanvas;
    public Vector2[] abilitiesRecharge; // x - Сколько сейчас зарядов, y - сколько ходов осталось до зарядки следующего
    public Vector2[] path;  // Маршрут движения 
    public LandscapeCell currentLandscapeCell; // Текущая ячейка игрока
    public int teamNumber; // Номер команды

    [SerializeField]private bool _lShiftDowning;
    [SerializeField]private string _characterName;
    [SerializeField]private float _endurance; // Максимальная Выносливость
    [SerializeField]private float _currentEdurance; // Текущая Выносливость
    [SerializeField]private float _health; // max здоровье
    [SerializeField]private float _currentHealth; // текущее здоровье
    [SerializeField]private float _mana;  // max Мана
    [SerializeField]private float _currentMana;  //  Текущая мана
    [SerializeField]private float _speed;// скорость
    [SerializeField]private float _movementPoints;//Очки передвижения
    [SerializeField]private float _attackPower;  // Сила атаки 
    [SerializeField]private float _attackRange; // Дальность атаки
    [SerializeField]private float _initiative; // Инициатива
    private bool firstStart = true;
    private CharacterSO _characterSO;
    [SerializeField]private CharacterState _characterState;
    public string characterName
    {
        get
        {
            return (_characterName);
        }
        set
        {
            _characterName = value;
            personalCharactersCanvas.charactersName.text = characterName;
            personalCharactersCanvas.targetCharacterNameText.text = characterName;
        }
    }
    public float endurance
    {
        get
        {
            return (_endurance);
        }
        set
        {
            _endurance = value;
        }
    }
    public float currentEdurance
    {
        get
        {
            return (_currentEdurance);
        }
        set
        {
            if(value > endurance)
            {
                _currentEdurance = endurance;
            }
            else
            {
                _currentEdurance = value;
                personalCharactersCanvas.enduranceNumberText.text = currentEdurance.ToString("F0");
            }
        }
    }
    public float health
    {
        get
        {
            return (_health);
        }
        set
        {
            _health = value;
        }
    }
    public float currentHealth
    {
        get
        {
            return (_currentHealth + superimposedEffects.shield.x);
        }
        set
        {
            if(value != currentHealth)
            {
                HealthChanger healthChanger = Instantiate(healthChangerPrefab, this.transform).GetComponent<HealthChanger>();
                if(value - currentHealth > 0)
                {

                    healthChanger.number.color = Color.green;
                    healthChanger.number.text = "+" + (value - _currentHealth).ToString();
                }
                else
                {
                    healthChanger.number.color = Color.red;

                    healthChanger.number.text = (value - _currentHealth).ToString();
                }
            }

            if(superimposedEffects.shield.x - value > 0)
            {
                superimposedEffects.shield = new Vector3(superimposedEffects.shield.x - value, superimposedEffects.shield.y, superimposedEffects.shield.z);
            }
            else
            {
                _currentHealth = value - superimposedEffects.shield.x;
                superimposedEffects.shield = Vector3.zero;
            }

            if(currentHealth <= 0)
            {
                characterState = CharacterState.Death;
            }
            personalCharactersCanvas.healthNumberText.text = $"{_currentHealth.ToString("F0")} / {health.ToString("F0")}";
            personalCharactersCanvas.targetHealthNumberText.text = $"{_currentHealth.ToString("F0")} / {health.ToString("F0")}";
            personalCharactersCanvas.currentHealthProgressBar.fillAmount = currentHealth / health;
            personalCharactersCanvas.targetHealthProgressBar.fillAmount = currentHealth / health;
        }
    }
    public float mana
    {
        get
        {
            return (_mana);
        }
        set
        {
            _mana = value;
        }
    }
    public float currentMana
    {
        get
        {
            return (_currentMana);
        }
        set
        {
            if(value > mana)
            {
                _currentMana = mana;
            }
            else
            {
                _currentMana = value;
            }
            personalCharactersCanvas.manaNumberText.text = $"{currentMana.ToString("F0")} / {mana.ToString("F0")}";
            personalCharactersCanvas.targetManaNumberText.text = $"{currentMana.ToString("F0")} / {mana.ToString("F0")}";
            personalCharactersCanvas.currentManaProgressBar.fillAmount = currentMana / mana;
            personalCharactersCanvas.targetManaProgressBar.fillAmount = currentMana / mana;
        }
    }
    public float speed
    {
        get
        {
            float slowingDown = 0;
            float boost = 0;
            if (superimposedEffects.slowingDownInCycles.Count != 0)
            {
                foreach(Vector3 slowingDownVector in superimposedEffects.slowingDownInCycles)
                {
                    slowingDown += slowingDownVector.x;
                }
            }
            if (superimposedEffects.boost.Count != 0)
            {
                foreach(Vector3 boostVector in superimposedEffects.boost)
                {
                    boost += boostVector.x;
                }
            }
            return (_speed * (1 - superimposedEffects.slowingDownFromEndurance.x) * (1 - slowingDown) + (_speed * boost));
        }
        set
        {
            _speed = value;
            personalCharactersCanvas.speedNumberText.text = speed.ToString("F0");
        }
    }
    public float movementPoints
    {
        get
        {
            return (_movementPoints);
        }
        set
        {
            _movementPoints = value;
            personalCharactersCanvas.pointsNumberText.text = movementPoints.ToString("F0");
        }
    }
    public float attackPower
    {
        get
        {
            return (_attackPower);
        }
        set
        {
            _attackPower = value;
            personalCharactersCanvas.attackPowerNumberText.text = attackPower.ToString("F0");
        }
    }
    public float attackRange
    {
        get
        {
            return (_attackRange);
        }
        set
        {
            _attackRange = value;
            personalCharactersCanvas.attackRangeNumberText.text = attackRange.ToString("F0");
        }
    }
    public float initiative
    {
        get
        {
            return (_initiative);
        }
        set
        {
            _initiative = value;
            personalCharactersCanvas.initiativeNumberText.text = initiative.ToString("F0");
        }
    }
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
    public CharacterState characterState
    {
        get
        {
            return (_characterState);
        }
        set
        {
            if(value == CharacterState.Readiness && characterState == CharacterState.Expectation)
            {
                ActionsStartOfTheTurn();
            }
            _characterState = value;
            ChangeCharacterState(characterState);
        }
    }
    private void Start ()
    {
        AssignActions(this.gameObject);
        Transform transform = this.gameObject.transform;
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        characterState = CharacterState.Expectation;
    }
    private void Update ()
    {
        RegisterKeystrokes();
    }
    public void SetParameters ()
    {
        personalCharactersCanvas = Instantiate(personalCharactersCanvasPrefab).GetComponent<PersonalCharactersCanvas>();
        personalCharactersCanvas.character = this;
        bodyGO.GetComponent<SpriteRenderer>().sprite = characterSO.characterImage;
        characterName = characterSO.characterName;
        endurance = characterSO.endurance;
        currentEdurance = characterSO.endurance;
        health = characterSO.health;
        currentHealth = characterSO.health;
        mana = characterSO.mana;
        currentMana = characterSO.mana / 2;
        speed = characterSO.speed;
        movementPoints = characterSO.movementPoints;
        attackPower = characterSO.attackPower;
        attackRange = characterSO.attackRange;
        initiative = characterSO.initiative;
        personalCharactersCanvas.FillInTheFields();
    }

    public void AssignActions (GameObject character)
    {
        // Получаем компонент EventTrigger
        EventTrigger eventTrigger = character.GetComponent<EventTrigger>();

        // Проверяем, что компонент EventTrigger присутствует
        if(eventTrigger != null)
        {
            EventTrigger.Entry down = new EventTrigger.Entry();
            down.eventID = EventTriggerType.PointerDown;
            down.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(down);

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(entry);

            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(exit);
        }
    }

    public void OnPointerDownDelegate (PointerEventData data)
    {
        switch(GameManager.currentGameState)
        {
            case (GameManager.GameState.LevelRedactor):

                if(Input.GetMouseButtonDown(0) && _lShiftDowning && caracterRedactorPanel == null)
                {

                    caracterRedactorPanel = Instantiate(characterRedactorPrefab).GetComponent<CaracterRedactorPanel>();
                    caracterRedactorPanel.character = this;
                }
                break;
        }
    }
    public void OnPointerEnterDelegate (PointerEventData data)
    {
        if (characterState == CharacterState.Expectation)
        {
            personalCharactersCanvas.targetCharacterPanel.SetActive(true);
        }
    }
    public void OnPointerExitDelegate (PointerEventData data)
    {
        if(characterState == CharacterState.Expectation)
        {
            personalCharactersCanvas.targetCharacterPanel.SetActive(false);
        }
    }
    public void RegisterKeystrokes ()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            _lShiftDowning = true;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _lShiftDowning = false;
        }
    }
    public void ChangeCharacterState (CharacterState state)
    {
        ResetModes(); // Сначала сбросить эффекты других режимов
        switch(state)
        {
            case CharacterState.Expectation:
                ActionsEndOfTheTurn(); // Выполнить действия конца хода
                activeBackground.SetActive(false);
                this.gameObject.layer = 15;
                personalCharactersCanvas.currentCharacterPanel.SetActive(false);
                break;
            case CharacterState.Readiness:
                gameManager.ChangeCellsStates(LandscapeCell.CellState.Expectation);
                personalCharactersCanvas.currentCharacterPanel.SetActive(true);
                energyCostCalculator.CalculateEnergyCost(currentLandscapeCell);
                break;
            case CharacterState.Movement:
                break;
            case CharacterState.Attack:
                attackMode.attackIsOn = true;
                break;
            case CharacterState.Ability:
                attackMode.abilityIsOn = true;
                break;
            case CharacterState.Death:
                if(gameManager.currentCharacter == this) // Если этот герой сейчас должен ходить
                {
                    gameManager.MoveTheTurnQueue(); // Перенести ход дальше
                }
                currentLandscapeCell.currentCharacter = null;
                mapAnchor.charactersList.Remove(this);
                Destroy(portraitInTheQueueGO);
                Destroy(this.gameObject);
                break;
        }
    }
    public void ActionsStartOfTheTurn ()
    {
        if(!firstStart)
        {
            currentEdurance += 30;
            movementPoints = 15;
            currentMana += 15;

        }
        else
        {
            firstStart = false;
            for(int i = 0; i < 4; i++)
            {
                abilitiesRecharge[i].x = 1;
                abilitiesRecharge[i].y = characterSO.abilities[i].recharge;
            }
        }
        if(superimposedEffects.listOfInfectedEnemies.Count > 0) // применение кары небес на тех игроков, на которых было наложено ранее
        {
            foreach(Character target in superimposedEffects.listOfInfectedEnemies)
            {
                target.currentHealth -= characterSO.abilities[2].firstDamage;
                currentEdurance -= 10;
            }
            superimposedEffects.listOfInfectedEnemies.Clear();
        }
        if(superimposedEffects.lossOfEndurance.Count > 0) // Применение взмах щитами 
        {
            int countNumber = superimposedEffects.lossOfEndurance.Count -1;
            for(int i = countNumber; i > -1; i--)
            {
                currentEdurance -= superimposedEffects.lossOfEndurance[i].x;
                superimposedEffects.lossOfEndurance[i] = new Vector3(superimposedEffects.lossOfEndurance[i].x, superimposedEffects.lossOfEndurance[i].y - 1, 0);
                if(superimposedEffects.lossOfEndurance[i].y <= 0)
                {
                    superimposedEffects.lossOfEndurance.RemoveAt(i);
                }
            }
        }
        personalCharactersCanvas.RedrawAbilitiesBoxes();
    }
    // Действия конца хода
    public void ActionsEndOfTheTurn ()
    {
        ApplyDamageDistribution();
        for(int i = 0; i < 4; i++) // Пересчитать перезарядку умений
        {
            if (abilitiesRecharge[i].x < 2)
            {
                abilitiesRecharge[i].y--;
                if(abilitiesRecharge[i].y == 0)
                {
                    abilitiesRecharge[i].x++;
                    abilitiesRecharge[i].y = characterSO.abilities[i].recharge;
                }
            }
        }
        superimposedEffects.RecalculateEffectsByEndCycle();
    }
    public void ApplyDamageDistribution ()
    {
        if(superimposedEffects.staticStartSetDamage.y > 0) // Применение большой молотилки
        {
            LayerMask characterMask = 1 << 15;
            attackMode.DealDamageByArea(characterMask, superimposedEffects.staticStartSetDamage.z / 5, superimposedEffects.staticStartSetDamage.x);
        }
    }
    // Сбрасывает эффекты других режимов
    public void ResetModes ()
    {
        attackMode.attackIsOn = false;
    }
}
