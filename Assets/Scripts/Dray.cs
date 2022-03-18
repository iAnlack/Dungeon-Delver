using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float Speed = 5;

    [Header("Set Dynamically")]
    public int DirHeld = -1; // Направление, соответсвующее удерживаемой клавише
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
        DirHeld = -1;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            DirHeld = 0;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            DirHeld = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            DirHeld = 2;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            DirHeld = 3;
        }

        Vector3 vel = Vector3.zero;
        // Полностью удалите инструкцию switch, бывшую здесь
        if (DirHeld > -1)
        {
            vel = _directions[DirHeld];
        }

        _rigidbody.velocity = vel * Speed;

        // Анимация
        if (DirHeld == -1)
        {
            _animator.speed = 0;
        }
        else
        {
            _animator.CrossFade("Dray_Walk_" + DirHeld, 0);
            _animator.speed = 1;
        }
    }
}
