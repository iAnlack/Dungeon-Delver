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

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
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
        switch (DirHeld)
        {
            case 0:
                vel = Vector3.right;
                break;
            case 1:
                vel = Vector3.up;
                break;
            case 2:
                vel = Vector3.left;
                break;
            case 3:
                vel = Vector3.down;
                break;
        }

        _rigidbody.velocity = vel * Speed;
    }
}
