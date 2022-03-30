using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X;
    public int Y;
    public int TileNum;

    private BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

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

        SetCollider();
    }

    // ���������� ��������� ��� ���� ������
    private void SetCollider()
    {
        // �������� ���������� � ���������� �� Collider DelverCollisios.txt
        _boxCollider.enabled = true;
        char c = TileCamera.COLLISIONS[TileNum];
        switch (c)
        {
            case 'S': // ��� ������
                _boxCollider.center = Vector3.zero;
                _boxCollider.size = Vector3.one;
                break;
            case 'W': // ������� ��������
                _boxCollider.center = new Vector3(0, 0.25f, 0);
                _boxCollider.size = new Vector3(1, 0.5f, 1);
                break;
            case 'A': // ����� ��������
                _boxCollider.center = new Vector3(-0.25f, 0, 0);
                _boxCollider.size = new Vector3(0.5f, 1, 1);
                break;
            case 'D': // ������ ��������
                _boxCollider.center = new Vector3(0.25f, 0, 0);
                _boxCollider.size = new Vector3(0.5f, 1, 1);
                break;

            // vvvvvvvv-------- �������������� ���� --------vvvvvvvv
            case 'Q': // ����� ������� ��������
                _boxCollider.center = new Vector3(-0.25f, 0.25f, 0);
                _boxCollider.size = new Vector3(0.5f, 0.5f, 1);
                break;
            case 'E': // ������ ������� ��������
                _boxCollider.center = new Vector3(0.25f, 0.25f, 0);
                _boxCollider.size = new Vector3(0.5f, 0.5f, 1);
                break;
            case 'Z': // ����� ������ ��������
                _boxCollider.center = new Vector3(-0.25f, -0.25f, 0);
                _boxCollider.size = new Vector3(0.5f, 0.5f, 1);
                break;
            case 'X': // ������ ��������
                _boxCollider.center = new Vector3(0, -0.25f, 0);
                _boxCollider.size = new Vector3(1, 0.5f, 1);
                break;
            case 'C': // ������ ������ ��������
                _boxCollider.center = new Vector3(0.25f, 0.25f, 0);
                _boxCollider.size = new Vector3(0.5f, 0.5f, 1);
                break;
            // ^^^^^^^^-------- �������������� ���� --------^^^^^^^^

            default: // �� ���������: _, |, � ��.
                _boxCollider.enabled = false;
                break;
        }
    }
}
