using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeletos : Enemy
{
    [Header("Set in Inspector: Skeletos")]
    public int Speed = 2;
    public float TimeThinkMin = 1f;
    public float TimeThinkMax = 4f;

    [Header("Set Dynamically: Skeletos")]
    public int Facing = 0;
    public float TimeNextDecision = 0;

    private void Update()
    {
        if (Time.time >= TimeNextDecision)
        {
            DecideDirection();
        }

        // ѕоле Rigidbody унаследовано от класса Enemy и инициализируетс€ в Enemy.Awake()
        Rigidbody.velocity = Directions[Facing] * Speed;
    }

    private void DecideDirection()
    {
        Facing = Random.Range(0, 4);
        TimeNextDecision = Time.time + Random.Range(TimeThinkMin, TimeThinkMax);
    }
}
