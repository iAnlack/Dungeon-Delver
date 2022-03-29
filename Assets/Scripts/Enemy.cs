using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector3[] Directions = new Vector3[] 
        { Vector3.right, Vector3.up, Vector3.left, Vector3.down };

    [Header("Set in Inspector: Enemy")]
    public float MaxHealth = 1;

    [Header("Set Dynamycally: Enemy")]
    public float Health;

    protected Animator Animator;
    protected Rigidbody Rigidbody;
    protected SpriteRenderer SpriteRenderer;

    protected virtual void Awake()
    {
        Health = MaxHealth;
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
}
