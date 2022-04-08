using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeletos : Enemy, IFacingMover
{
    [Header("Set in Inspector: Skeletos")]
    public int Speed = 2;
    public float TimeThinkMin = 1f;
    public float TimeThinkMax = 4f;

    [Header("Set Dynamically: Skeletos")]
    public int Facing = 0;
    public float TimeNextDecision = 0;

    private InRoom _inRoom;

    protected override void Awake()
    {
        base.Awake();
        _inRoom = GetComponent<InRoom>();
    }

    override protected void Update()
    {
        base.Update();
        if (Knockback)
        {
            return;
        }

        if (Time.time >= TimeNextDecision)
        {
            DecideDirection();
        }

        // Поле Rigidbody унаследовано от класса Enemy и инициализируется в Enemy.Awake()
        Rigidbody.velocity = Directions[Facing] * Speed;
    }

    private void DecideDirection()
    {
        Facing = Random.Range(0, 4);
        TimeNextDecision = Time.time + Random.Range(TimeThinkMin, TimeThinkMax);
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
            return true;
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
}
