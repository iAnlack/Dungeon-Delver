using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour, IFacingMover, IKeyMaster
{
    public enum EMode { Idle, Move, Attack, Transition, Knockback }

    [Header("Set in Inspector")]
    public float Speed = 5;
    public float AttackDuration = 0.25f; // Продолжительность атаки в секундах
    public float AttackDelay = 0.5f;     // Задержка между атаками
    public float TransitionDelay = 0.5f;  // Задержка перехода между комнатами

    public int MaxHealth = 10;
    public float KnockbackSpeed = 10;
    public float KnockbackDuration = 0.25f;
    public float InvincibleDuration = 0.5f;

    [Header("Set Dynamically")]
    public int DirHeld = -1; // Направление, соответсвующее удерживаемой клавише
    public int Facing = 1;   // Направление движения Дрея
    public EMode Mode = EMode.Idle;
    public int NumKeys = 0;
    public bool Invincible = false;

    [SerializeField] private int _health;

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }

    private float _timeAttackDone = 0;
    private float _timeAttackNext = 0;

    private float _transitionDone = 0;
    private Vector2 _transitionPos;
    private float _knockbackDone = 0;
    private float _invincibleDone = 0;
    private Vector3 _knockbackVel;

    private SpriteRenderer _sRend;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private InRoom _inRoom;

    private Vector3[] _directions = new Vector3[] { Vector3.right, Vector3.up, Vector3.left, Vector3.down };
    private KeyCode[] _keys = new KeyCode[] { KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow };

    private void Awake()
    {
        _sRend = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _inRoom = GetComponent<InRoom>();
        Health = MaxHealth;
    }

    private void Update()
    {
        // Проверить состояние неуязвимости и необходимость выполнить отбрасывание
        if (Invincible && Time.time > _invincibleDone)
        {
            Invincible = false;
        }
        _sRend.color = Invincible ? Color.red : Color.white;
        if (Mode == EMode.Knockback)
        {
            _rigidbody.velocity = _knockbackVel;
            if (Time.time < _knockbackDone)
            {
                return;
            }
        }

        if (Mode == EMode.Transition)
        {
            _rigidbody.velocity = Vector3.zero;
            _animator.speed = 0;
            RoomPos = _transitionPos; // Оставить Дрея на месте
            if (Time.time < _transitionDone)
            {
                return;
            }
            // Следующая строка выполнится, тллько если Time.time >= _transitionDone
            Mode = EMode.Idle;
        }

        //----Обработка ввода с клавиатуры и управление режимами EMode----
        DirHeld = -1;
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKey(_keys[i]))
            {
                DirHeld = i;
            }
        }

        // Нажата клавиша атаки
        if (Input.GetKeyDown(KeyCode.Z) && Time.time >= _timeAttackNext)
        {
            Mode = EMode.Attack;
            _timeAttackDone = Time.time + AttackDuration;
            _timeAttackNext = Time.time + AttackDelay;
        }

        // Завершить атаку, если время истекло
        if (Time.time >= _timeAttackDone)
        {
            Mode = EMode.Idle;
        }

        // Выбрать правильный режим, если Дрей не атакует
        if (Mode != EMode.Attack)
        {
            if (DirHeld == -1)
            {
                Mode = EMode.Idle;
            }
            else
            {
                Facing = DirHeld;
                Mode = EMode.Move;
            }
        }

        //----Действия в текущем режиме----
        Vector3 vel = Vector3.zero;
        switch (Mode)
        {
            case EMode.Idle:
                _animator.CrossFade("Dray_Walk_" + Facing, 0);
                _animator.speed = 0;
                break;
            case EMode.Move:
                vel = _directions[DirHeld];
                _animator.CrossFade("Dray_Walk_" + Facing, 0);
                _animator.speed = 1;
                break;
            case EMode.Attack:
                _animator.CrossFade("Dray_Attack_" + Facing, 0);
                _animator.speed = 0;
                break;
        }

        _rigidbody.velocity = vel * Speed;
    }

    private void LateUpdate()
    {
        // Получить координаты узла сетки, с размером ячейки
        // в половину единицы, ближайшего к данному персонажу
        Vector2 rPos = GetRoomPosOnGrid(0.5f); // Размер ячейки в пол-единицы

        // Персонаж находится на плитке с дверью?
        int doorNum;
        for (doorNum = 0; doorNum < 4; doorNum++)
        {
            if (rPos == InRoom.DOORS[doorNum])
            {
                break;
            }
        }

        if (doorNum > 3 || doorNum != Facing)
        {
            return;
        }

        // Перейти в следующую комнату
        Vector2 rm = RoomNum;
        switch (doorNum)
        {
            case 0:
                rm.x += 1;
                break;
            case 1:
                rm.y += 1;
                break;
            case 2:
                rm.x -= 1;
                break;
            case 3:
                rm.y -= 1;
                break;
        }

        // Проверить, можно ли выполнить переход в комнату rm
        if (rm.x >= 0 && rm.x <= InRoom.MAX_RM_X)
        {
            if (rm.y >= 0 && rm.y <= InRoom.MAX_RM_Y)
            {
                RoomNum = rm;
                _transitionPos = InRoom.DOORS[(doorNum + 2) % 4];
                RoomPos = _transitionPos;
                Mode = EMode.Transition;
                _transitionDone = Time.time + TransitionDelay;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Invincible)
        {
            return; // Выйти, если Дрей пока неуязвим
        }
        DamageEffect damageEffect = collision.gameObject.GetComponent<DamageEffect>();
        if (damageEffect == null)
        {
            return; // Если компонент DamageEffect отсутсвует -выйти
        }

        Health -= damageEffect.Damage; // Вычесть величину ущерба из уровня здоровья
        Invincible = true; // Сделать Дрея неуязвимым
        _invincibleDone = Time.time + InvincibleDuration;

        if (damageEffect.Knockback) // Выполнить отбрасывание
        {
            // Определить направление отбрасывания
            Vector3 delta = transform.position - collision.transform.position;
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                // Отбрасывание по горизонтали
                delta.x = (delta.x > 0) ? 1 : -1;
                delta.y = 0;
            }
            else
            {
                // Отбрасывание по вертикали
                delta.x = 0;
                delta.y = (delta.y > 0) ? 1 : -1;
            }

            // Применить скорость отскока к компоненту Rigidbody
            _knockbackVel = delta * KnockbackSpeed;
            _rigidbody.velocity = _knockbackVel;

            // Установить режим Knockback и время прекращения отбрасывания
            Mode = EMode.Knockback;
            _knockbackDone = Time.time + KnockbackDuration;
        }
    }

    // Реализация интерфейса IFacingMover
    public int GetFacing()
    {
        return Facing;
    }

    public bool Moving
    {
        get
        {
            return (Mode == EMode.Move);
        }
    }

    public float GetSpeed()
    {
        return Speed;
    }

    public float GridMult
    {
        get
        {
            return _inRoom.GridMult;
        }
    }

    public Vector2 RoomPos
    {
        get
        {
            return _inRoom.RoomPos;
        }
        set 
        { 
            _inRoom.RoomPos = value; 
        }
    }

    public Vector2 RoomNum
    {
        get
        {
            return _inRoom.RoomNum;
        }
        set
        {
            _inRoom.RoomNum = value;
        }
    }

    public Vector2 GetRoomPosOnGrid(float mult = -1)
    {
        return _inRoom.GetRoomPosOnGrid(mult);
    }

    // Реализация интерфейса IKeyMaster
    public int KeyCount
    {
        get
        {
            return NumKeys;
        }
        set
        {
            NumKeys = value;
        }
    }
}
