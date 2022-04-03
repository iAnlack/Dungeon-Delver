using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMove : MonoBehaviour
{
    private IFacingMover _mover;

    private void Awake()
    {
        _mover = GetComponent<IFacingMover>();
    }

    private void FixedUpdate()
    {
        if (!_mover.Moving) // ���� ������ �� ������������ - �����
        {
            return;
        }
        int facing = _mover.GetFacing();

        // ���� ������ ������������, ��������� ������������ �� �����
        // ������� �������� ���������� ���������� ���� �����
        Vector2 rPos = _mover.RoomPos;
        Vector2 rPosGrid = _mover.GetRoomPosOnGrid();
        // ���� ��� ���������� �� ��������� IFacingMover
        // (������� ���������� InRoom) ��� ����������� ���� �����

        // ����� ��������� ������ � ������� ����� �����
        float delta = 0;
        if (facing == 0 || facing == 2)
        {
            // �������� �� �����������, ������������ �� ��� y
            delta = rPosGrid.y - rPos.y;
        }
        else
        {
            // �������� �� ���������, ������������ �� ��� x
            delta = rPosGrid.x - rPos.x;
        }
        if (delta == 0) // ������ ��� �������� �� �����
        {
            return;
        }

        float move = _mover.GetSpeed() * Time.fixedDeltaTime;
        move = Mathf.Min(move, Mathf.Abs(delta));
        if (delta < 0)
        {
            move = -move;
        }

        if (facing == 0 || facing == 2)
        {
            // �������� �� �����������, ������������ �� ��� y
            rPos.y += move;
        }
        else
        {
            // �������� �� ���������, ������������ �� ��� x
            rPos.x += move;
        }

        _mover.RoomPos = rPos;
    }
}
