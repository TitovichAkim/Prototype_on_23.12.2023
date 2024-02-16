using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

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
    public SpriteRenderer teamColorBackgroundRenderer;
    public EnergyCostCalculator energyCostCalculator;
    public CharacterMovement characterMovement;
    public AttackMode attackMode;
    public SuperimposedEffects superimposedEffects;
    public float costOfTheAttack;

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
    public CaracterRedactorPanel caracterRedactorPanelScr;
    public List<EffectItems> effects = new List<EffectItems>();

    public bool lShiftDowning;
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
    [SerializeField]private bool firstStart = true;
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
                if (_currentEdurance <= 0)
                {
                    personalCharactersCanvas.enduranceNumberText.color = Color.red;
                }
                else
                {
                    personalCharactersCanvas.enduranceNumberText.color = Color.green;
                }
                personalCharactersCanvas.RedrawAbilitiesBoxes();
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
            personalCharactersCanvas.healthNumberText.text = $"{_currentHealth.ToString("F0")} / {health.ToString("F0")}";
            personalCharactersCanvas.targetHealthNumberText.text = $"{_currentHealth.ToString("F0")} / {health.ToString("F0")}";
            personalCharactersCanvas.currentHealthProgressBar.fillAmount = _currentHealth / health;
            personalCharactersCanvas.targetHealthProgressBar.fillAmount = _currentHealth / health;
            if (caracterRedactorPanelScr != null)
            {
                caracterRedactorPanelScr.currentHealthProgressBar.fillAmount = originCurrentHealth / health;
            }
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
                HealthChanger healthChanger = Instantiate(healthChangerPrefab).GetComponent<HealthChanger>();
                healthChanger.gameObject.transform.position = this.gameObject.transform.position;
                if(value - currentHealth > 0)
                {

                    healthChanger.number.color = Color.green;
                    healthChanger.number.text = "+" + (value - currentHealth).ToString();
                }
                else
                {
                    healthChanger.number.color = Color.red;
                    healthChanger.number.text = (value - currentHealth).ToString();
                }
            }
            float delta = currentHealth - value;
            if(superimposedEffects.shield.x - Mathf.Sqrt(delta * delta) > 0)
            {
                superimposedEffects.shield = new Vector3(superimposedEffects.shield.x - delta, superimposedEffects.shield.y, superimposedEffects.shield.z);
            }
            else
            {
                _currentHealth = value;
                superimposedEffects.shield = Vector3.zero;
                superimposedEffects.shield = superimposedEffects.shield;
            }

            if(currentHealth <= 0)
            {
                characterState = CharacterState.Death;
            }
            personalCharactersCanvas.healthNumberText.text = $"{_currentHealth.ToString("F0")} / {health.ToString("F0")}";
            personalCharactersCanvas.targetHealthNumberText.text = $"{_currentHealth.ToString("F0")} / {health.ToString("F0")}";
            personalCharactersCanvas.currentHealthProgressBar.fillAmount = _currentHealth / health;
            personalCharactersCanvas.targetHealthProgressBar.fillAmount = _currentHealth / health;
            if(caracterRedactorPanelScr != null)
            {
                caracterRedactorPanelScr.currentHealthProgressBar.fillAmount = originCurrentHealth / health;
            }
        }
    }
    public float originCurrentHealth
    {
        get
        {
            return (_currentHealth);
        }
        set
        {
            _currentHealth = value;
            if(_currentHealth <= 0)
            {
                characterState = CharacterState.Death;
            }
            personalCharactersCanvas.healthNumberText.text = $"{_currentHealth.ToString("F0")} / {health.ToString("F0")}";
            personalCharactersCanvas.targetHealthNumberText.text = $"{_currentHealth.ToString("F0")} / {health.ToString("F0")}";
            personalCharactersCanvas.currentHealthProgressBar.fillAmount = originCurrentHealth / health;
            personalCharactersCanvas.targetHealthProgressBar.fillAmount = originCurrentHealth / health;
            if(caracterRedactorPanelScr != null)
            {
                caracterRedactorPanelScr.currentHealthProgressBar.fillAmount = originCurrentHealth / health;
            }
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
            personalCharactersCanvas.manaNumberText.text = $"{currentMana.ToString("F0")} / {mana.ToString("F0")}";
            personalCharactersCanvas.targetManaNumberText.text = $"{currentMana.ToString("F0")} / {mana.ToString("F0")}";
            personalCharactersCanvas.currentManaProgressBar.fillAmount = _currentMana / mana;
            personalCharactersCanvas.targetManaProgressBar.fillAmount = _currentMana / mana;
            if(caracterRedactorPanelScr != null)
            {
                caracterRedactorPanelScr.currentManaProgressBar.fillAmount = currentMana / mana;
            }
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
            personalCharactersCanvas.currentManaProgressBar.fillAmount = _currentMana / mana;
            personalCharactersCanvas.targetManaProgressBar.fillAmount = _currentMana / mana;
            if(caracterRedactorPanelScr != null)
            {
                caracterRedactorPanelScr.currentManaProgressBar.fillAmount = currentMana / mana;
            }
            personalCharactersCanvas.RedrawAbilitiesBoxes();
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
            return _speed - (_speed * superimposedEffects.slowingDownFromEndurance.x) - (_speed * slowingDown) + (_speed * boost);
        }
        set
        {
            if (value <= 0)
            {
                _speed = 1;
            }
            else
            {
                _speed = value;
            }
            personalCharactersCanvas.speedNumberText.text = speed.ToString("F0");
        }
    }
    public float standardSpeed
    {
        get
        {
            return (_speed);
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
            if (gameManager != null)
            {
                gameManager.initiativeHasChanged = true;
            }
            if (portraitInTheQueueScr!= null)
            {
                portraitInTheQueueScr.initiativeNumber.text = initiative.ToString("F0");
            }
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
            CharacterState oldValue = characterState;
            _characterState = value;
            ChangeCharacterState(characterState);
            if(characterState == CharacterState.Readiness && oldValue == CharacterState.Expectation)
            {
                ActionsStartOfTheTurn();
            }
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
        firstStart = true;
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
        for(int i = 0; i < 4; i++)
        {
            abilitiesRecharge[i].x = 1;
            abilitiesRecharge[i].y = characterSO.abilities[i].recharge;
            personalCharactersCanvas.RedrawAbilitiesBoxes();
        }
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
            if(Input.GetMouseButtonDown(0) && lShiftDowning && caracterRedactorPanel == null)
            {
                caracterRedactorPanel = Instantiate(characterRedactorPrefab).GetComponent<CaracterRedactorPanel>();
                gameManager.currentCharacterRedactor = caracterRedactorPanel;
                caracterRedactorPanelScr = caracterRedactorPanel.GetComponent<CaracterRedactorPanel>();
                caracterRedactorPanelScr.characterIcon.sprite = _characterSO.characterIcon;
                CreateEffectsPanelsOnRedactingPanel();
                caracterRedactorPanel.character = this;
                LevelRedactor.redactingCharacter = this;
            }
    }
    public void CreateEffectsPanelsOnRedactingPanel ()
    {
        superimposedEffects.slowingDownInCycles = superimposedEffects.slowingDownInCycles;
        superimposedEffects.slowingDownFromEndurance = superimposedEffects.slowingDownFromEndurance;
        superimposedEffects.staticEndGetDamage = superimposedEffects.staticEndGetDamage;
        superimposedEffects.staticStartSetDamage = superimposedEffects.staticStartSetDamage;
        superimposedEffects.lossOfEndurance = superimposedEffects.lossOfEndurance;
        superimposedEffects.infectionIsThePunishmentOfHeaven = superimposedEffects.infectionIsThePunishmentOfHeaven;
        superimposedEffects.boost = superimposedEffects.boost;
        superimposedEffects.shield = superimposedEffects.shield;
    }
    public void OnPointerEnterDelegate (PointerEventData data)
    {
        if(GameManager.currentGameState == GameManager.GameState.Game)
        {
            if(characterState == CharacterState.Expectation)
            {
                personalCharactersCanvas.targetCharacterPanel.SetActive(true);
            }
            if(this != gameManager.currentCharacter && teamNumber != gameManager.currentCharacter.teamNumber && gameManager.currentCharacter.characterState == CharacterState.Readiness)
            {
                if(costOfTheAttack <= gameManager.currentCharacter.currentEdurance)
                {
                    gameManager.currentCharacter.characterState = CharacterState.Attack;
                }
            }
        }
    }
    public void OnPointerExitDelegate (PointerEventData data)
    {
        if(GameManager.currentGameState == GameManager.GameState.Game)
        {
            if(characterState == CharacterState.Expectation)
            {
                personalCharactersCanvas.targetCharacterPanel.SetActive(false);
            }
            if(this != gameManager.currentCharacter && teamNumber != gameManager.currentCharacter.teamNumber && gameManager.currentCharacter.characterState == CharacterState.Attack)
            {
                gameManager.currentCharacter.characterState = CharacterState.Readiness;
            }
        }
    }
    public void RegisterKeystrokes ()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            lShiftDowning = true;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            lShiftDowning = false;
        }
        if(caracterRedactorPanel != null)
        {
            if(caracterRedactorPanel.character == this && Input.GetKeyDown(KeyCode.Delete))
            {
                Destroy(caracterRedactorPanel.gameObject);
                characterState = CharacterState.Death;
            }
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
                personalCharactersCanvas.targetCharacterPanel.SetActive(false);
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
                if (GameManager.currentGameState != GameManager.GameState.LevelRedactor)
                {
                    if(gameManager.currentCharacter == this) // Если этот герой сейчас должен ходить
                    {
                        gameManager.MoveTheTurnQueue(); // Перенести ход дальше
                    }
                }
                currentLandscapeCell.currentCharacter = null;
                mapAnchor.charactersList.Remove(this);
                gameManager.queueOfCharacters.Remove(this);
                Destroy(personalCharactersCanvas.gameObject);
                Destroy(portraitInTheQueueGO);
                Destroy(this.gameObject);
                break;
        }
    }
    public void ActionsStartOfTheTurn ()
    {
        if(!firstStart)
        {
            currentEdurance += endurance;
            movementPoints = 15;
            currentMana += 15;
            characterState = CharacterState.Attack;
            characterState = CharacterState.Readiness;
        }
        firstStart = false;
        if(superimposedEffects.listOfInfectedEnemies.Count > 0) // применение кары небес на тех игроков, на которых было наложено ранее
        {
            foreach(Character target in superimposedEffects.listOfInfectedEnemies)
            {
                if (target != null)
                {
                    target.currentHealth -= characterSO.abilities[2].firstDamage;
                    target.superimposedEffects.infectionIsThePunishmentOfHeaven--;
                    currentEdurance -= 10;
                    superimposedEffects.RecalculateEffectsByMoves(10);
                }
            }
            superimposedEffects.listOfInfectedEnemies.Clear();
        }
        if(superimposedEffects.lossOfEndurance.Count > 0) // Применение взмах щитами 
        {
            int countNumber = superimposedEffects.lossOfEndurance.Count -1;
            for(int i = countNumber; i > -1; i--)
            {
                currentEdurance -= superimposedEffects.lossOfEndurance[i].x;
                superimposedEffects.RecalculateEffectsByMoves(superimposedEffects.lossOfEndurance[i].x);
                superimposedEffects.lossOfEndurance[i] = new Vector3(superimposedEffects.lossOfEndurance[i].x, superimposedEffects.lossOfEndurance[i].y - 1, 0);
                superimposedEffects.slowingDownFromEndurance = new Vector3(superimposedEffects.slowingDownFromEndurance.x, superimposedEffects.slowingDownFromEndurance.y - superimposedEffects.lossOfEndurance[i].x, 0);
            }
            superimposedEffects.lossOfEndurance = superimposedEffects.lossOfEndurance;
            superimposedEffects.slowingDownFromEndurance = superimposedEffects.slowingDownFromEndurance;
        }
        superimposedEffects.shield = superimposedEffects.vector3Changer(superimposedEffects.shield);
        personalCharactersCanvas.RedrawAbilitiesBoxes();
        energyCostCalculator.CalculateEnergyCost(currentLandscapeCell);
        if(currentEdurance <= 0)
        {
            currentEdurance = 0;
            gameManager.MoveTheTurnQueue();
        }
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
        superimposedEffects.RecalculateEffectsByMoves(movementPoints);
        if (currentEdurance > 0)
        {
            superimposedEffects.RecalculateEffectsByMoves(currentEdurance);
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
        if(personalCharactersCanvas.flyingText != null)
        {
            Destroy(personalCharactersCanvas.flyingText);
        }
        attackMode.abilityIsOn = false;
    }
}
