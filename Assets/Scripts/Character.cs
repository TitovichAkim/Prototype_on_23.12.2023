using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour
{
    [Header("SetInInspector")]
    public GameObject bodyGO;
    public GameObject characterRedactorPrefab;

    [Header("SetDynamically")]
    public  CaracterRedactorPanel caracterRedactorPanel;
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
    private CharacterSO _characterSO;
    public string characterName
    {
        get
        {
            return (_characterName);
        }
        set
        {
            _characterName = value;
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
            _currentEdurance = value;
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
            return (_currentHealth);
        }
        set
        {
            _currentHealth = value;
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
            _currentMana = value;
        }
    }
    public float speed
    {
        get
        {
            return (_speed);
        }
        set
        {
            _speed = value;
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
    private void Start ()
    {
        AssignActions(this.gameObject);
        Transform transform = this.gameObject.transform;
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
    }
    private void Update ()
    {
        RegisterKeystrokes();
    }
    public void SetParameters ()
    {
        bodyGO.GetComponent<SpriteRenderer>().sprite = characterSO.characterImage;
        characterName = characterSO.characterName;
        endurance = characterSO.endurance;
        currentEdurance = characterSO.endurance;
        health = characterSO.health;
        currentHealth = characterSO.health;
        mana = characterSO.mana;
        currentMana = characterSO.mana;
        speed = characterSO.speed;
        movementPoints = characterSO.movementPoints;
        attackPower = characterSO.attackPower;
        attackRange = characterSO.attackRange;
        initiative = characterSO.initiative;
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
            Debug.Log("Присоединили");
        }
    }

    public void OnPointerDownDelegate (PointerEventData data)
    {
        Debug.Log("Кликнул!");
        switch(GameManager.currentGameState)
        {
            case (GameManager.GameState.LevelRedactor):
                Debug.Log("Редактор!");

                if(Input.GetMouseButtonDown(0) && _lShiftDowning && caracterRedactorPanel == null)
                {
                    Debug.Log("Открыть окно!");

                    caracterRedactorPanel = Instantiate(characterRedactorPrefab).GetComponent<CaracterRedactorPanel>();
                    caracterRedactorPanel.character = this;
                }
                break;
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
}
