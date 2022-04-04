using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateKeeper : MonoBehaviour
{
    // Следующие константы зависят от файла изображения по умолчанию DelverTiles.
    // Если вы переупорядочиваете спрайты в DelverTiles, возможно, вам придётся изменить эти константы!

    //-------- Индексы плиток с запертыми дверьми
    const int lockedR = 95;
    const int lockedUR = 81;
    const int lockedUL = 80;
    const int lockedL = 100;
    const int lockedDL = 101;
    const int lockedDR = 102;

    //-------- Индексы плиток с открытыми дверьми
    const int openR = 48;
    const int openUR = 93;
    const int openUL = 92;
    const int openL = 51;
    const int openDL = 26;
    const int openDR = 27;

    private IKeyMaster _keys;

    private void Awake()
    {
        _keys = GetComponent<IKeyMaster>();
    }

    private void OnCollisionStay(Collision collision)
    {
        // Если ключей нет, можно не продолжать
        if (_keys.KeyCount < 1)
        {
            return;
        }

        Tile ti = collision.gameObject.GetComponent<Tile>();
        if (ti == null)
        {
            return;
        }

        // Открывать, только если Дрей обращён лицом к двери (предотвратить случайное использование ключа)
        int facing = _keys.GetFacing();
        // Проверить, является ли плитка закрытой дверью
        Tile ti2;
        switch (ti.TileNum)
        {
            case lockedR:
                if (facing != 0)
                {
                    return;
                }
                ti.SetTile(ti.X, ti.Y, openR);
                break;

            case lockedUR:
                if (facing != 1)
                {
                    return;
                }
                ti.SetTile(ti.X, ti.Y, openUR);
                ti2 = TileCamera.TILES[ti.X - 1, ti.Y];
                ti2.SetTile(ti2.X, ti2.Y, openUL);
                break;

            case lockedUL:
                if (facing != 1)
                {
                    return;
                }
                ti.SetTile(ti.X, ti.Y, openUL);
                ti2 = TileCamera.TILES[ti.X + 1, ti.Y];
                ti2.SetTile(ti2.X, ti2.Y, openUR);
                break;

            case lockedL:
                if (facing != 2)
                {
                    return;
                }
                ti.SetTile(ti.X, ti.Y, openL);
                break;

            case lockedDL:
                if (facing != 3)
                {
                    return;
                }
                ti.SetTile(ti.X, ti.Y, openDL);
                ti2 = TileCamera.TILES[ti.X + 1, ti.Y];
                ti2.SetTile(ti2.X, ti2.Y, openDR);
                break;

            case lockedDR:
                if (facing != 3)
                {
                    return;
                }
                ti.SetTile(ti.X, ti.Y, openDR);
                ti2 = TileCamera.TILES[ti.X - 1, ti.Y];
                ti2.SetTile(ti2.X, ti2.Y, openDL);
                break;

            default:
                return; // Выйти, чтобы исключить уменьшение счётчика ключей
        }

        _keys.KeyCount--;
    }
}
