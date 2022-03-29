using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour
{
    public enum eMode { Idle, Move, Attack, Transition }

    [Header("Set in Inspector")]
    public float Speed = 5;
    public float AttackDuration = 0.25f; // Продолжительность атаки в секундах
    public float AttackDelay = 0.5f;     // Задержка между атаками

    [Header("Set Dynamically")]
    public int DirHeld = -1; // Направление, соответсвующее удерживаемой клавише
    public int Facing = 1;   // Направление движения Дрея
    public eMode Mode = eMode.Idle;

    private float _timeAttackDone = 0;
    private float _timeAttackNext = 0;

    private Rigidbody _rigidbody;
    private Animator _animator;

    private Vector3[] _directions = new Vector3[] { Vector3.right, Vector3.up, Vector3.left, Vector3.down };
    private KeyCode[] _keys = new KeyCode[] { KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow };

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //----Обработка ввода с клавиатуры и управление режимами eMode----
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
            Mode = eMode.Attack;
            _timeAttackDone = Time.time + AttackDuration;
            _timeAttackNext = Time.time + AttackDelay;
        }

        // Завершить атаку, если время истекло
        if (Time.time >= _timeAttackDone)
        {
            Mode = eMode.Idle;
        }

        // Выбрать правильный режим, если Дрей не атакует
        if (Mode != eMode.Attack)
        {
            if (DirHeld == -1)
            {
                Mode = eMode.Idle;
            }
            else
            {
                Facing = DirHeld;
                Mode = eMode.Move;
            }
        }

        //----Действия в текущем режиме----
        Vector3 vel = Vector3.zero;
        switch (Mode)
        {
            case eMode.Idle:
                _animator.CrossFade("Dray_Walk_" + Facing, 0);
                _animator.speed = 0;
                break;
            case eMode.Move:
                vel = _directions[DirHeld];
                _animator.CrossFade("Dray_Walk_" + Facing, 0);
                _animator.speed = 1;
                break;
            case eMode.Attack:
                _animator.CrossFade("Dray_Attack_" + Facing, 0);
                _animator.speed = 0;
                break;
        }

        _rigidbody.velocity = vel * Speed;
    }
}
