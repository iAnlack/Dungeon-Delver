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
        if (!_mover.Moving) // Если объект не перемещается - выйти
        {
            return;
        }
        int facing = _mover.GetFacing();

        // Если объект перемещается, применить выравнивание по сетке
        // Сначала получить координаты ближайшего узла сетки
        Vector2 rPos = _mover.RoomPos;
        Vector2 rPosGrid = _mover.GetRoomPosOnGrid();
        // Этот код полагается на интерфейс IFacingMover
        // (который использует InRoom) для определения шага сетки

        // Затем подвинуть объект в сторону линии сетки
        float delta = 0;
        if (facing == 0 || facing == 2)
        {
            // Движение по горизонтали, выравнивание по оси y
            delta = rPosGrid.y - rPos.y;
        }
        else
        {
            // Движение по вертикали, выравнивание по оси x
            delta = rPosGrid.x - rPos.x;
        }
        if (delta == 0) // Объект уже выровнен по сетке
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
            // Движение по горизонтали, выравнивание по оси y
            rPos.y += move;
        }
        else
        {
            // Движение по вертикали, выравнивание по оси x
            rPos.x += move;
        }

        _mover.RoomPos = rPos;
    }
}
