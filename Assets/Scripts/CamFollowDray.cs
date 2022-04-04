using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowDray : MonoBehaviour
{
    static public bool TRANSITIONING = false;

    [Header("Set in Inspector")]
    public InRoom DrayInRoom;
    public float TransTime = 0.5f;

    private Vector3 _p0, _p1;

    private InRoom _inRoom;
    private float _transStart;

    private void Awake()
    {
        _inRoom = GetComponent<InRoom>();
    }

    private void Update()
    {
        if (TRANSITIONING)
        {
            float u = (Time.time - _transStart) / TransTime;
            if (u >= 1)
            {
                u = 1;
                TRANSITIONING = false;
            }

            transform.position = (1 - u) * _p0 + u * _p1;
        }
        else
        {
            if (DrayInRoom.RoomNum != _inRoom.RoomNum)
            {
                TransitionTo(DrayInRoom.RoomNum);
            }
        }
    }

    private void TransitionTo(Vector2 rm)
    {
        _p0 = transform.position;
        _inRoom.RoomNum = rm;
        _p1 = transform.position + (Vector3.back * 10);
        transform.position = _p0;

        _transStart = Time.time;
        TRANSITIONING = true;
    }
}
