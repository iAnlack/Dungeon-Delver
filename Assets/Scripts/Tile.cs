using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X;
    public int Y;
    public int TileNum;

    public void SetTile(int eX, int eY, int eTileNum = -1)
    {
        X = eX;
        Y = eY;
        transform.localPosition = new Vector3(X, Y, 0);
        gameObject.name = X.ToString("D3") + "x" + Y.ToString("D3");

        if (eTileNum == -1)
        {
            eTileNum = TileCamera.GET_MAP(X, Y);
        }

        TileNum = eTileNum;
        GetComponent<SpriteRenderer>().sprite = TileCamera.SPRITES[TileNum];
    }
}
