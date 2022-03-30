using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCamera : MonoBehaviour
{
    static private int      W, H;
    static private int[,]   MAP;
    static public Sprite[]  SPRITES;
    static public Transform TILE_ANCHOR;
    static public Tile[,]   TILES;
    static public string COLLISIONS;

    [Header("Set in Inspector")]
    public TextAsset MapData;
    public Texture2D MapTiles;
    public TextAsset MapCollisions;   // ...
    public Tile      TilePrefab;

    private void Awake()
    {
        COLLISIONS = Utils.RemoveLineEndings(MapCollisions.text);
        LoadMap();
    }

    public void LoadMap()
    {
        // Создать TILE_ANCHOR. Он будет играть роль родителя для всех плиток Tile
        GameObject gameObject = new GameObject("TILE_ANCHOR");
        TILE_ANCHOR = gameObject.transform;

        // Загрузить все спрайты из MapTiles
        SPRITES = Resources.LoadAll<Sprite>(MapTiles.name);

        // Прочитать информацию для карты
        string[] lines = MapData.text.Split('\n');
        H = lines.Length;
        string[] tileNums = lines[0].Split(' ');
        W = tileNums.Length;

        System.Globalization.NumberStyles hexNum;
        hexNum = System.Globalization.NumberStyles.HexNumber;
        // Сохранить информацию карты в двумерный массив для ускорения доступа
        MAP = new int[W, H];
        for (int j = 0; j < H; j++)
        {
            tileNums = lines[j].Split(' ');
            for (int i = 0; i < W; i++)
            {
                if (tileNums[i] == "..")
                {
                    MAP[i, j] = 0;
                }
                else
                {
                    MAP[i, j] = int.Parse(tileNums[i], hexNum);
                }
            }
        }

        Debug.Log("Parsed " + SPRITES.Length + " sprites.");
        Debug.Log("Map size: " + W + " wide by " + H + " high");

        ShowMap();
    }

    /// <summary>
    /// Генерирует плитки сразу для всей карты
    /// </summary>
    private void ShowMap()
        {
        TILES = new Tile[W, H];

        // Просмотреть всю карту и создать плитки, где необходимо
        for (int j = 0; j < H; j++)
        {
            for (int i = 0; i < W; i++)
            {
                if (MAP[i,j] != 0)
                {
                    Tile tile = Instantiate<Tile>(TilePrefab);
                    tile.transform.SetParent(TILE_ANCHOR);
                    tile.SetTile(i, j);
                    TILES[i, j] = tile;
                }
            }
        }
        }

    static public int GET_MAP(int x, int y)
    {
        if (x < 0 || x >= W || y < 0 || y >= H)
        {
            return -1;   // Предотвратить исключение IndexOutOfRangeException
        }

        return (MAP[x, y]);
    }

    static public int GET_MAP(float x, float y)
    {
        int tX = Mathf.RoundToInt(x);
        int tY = Mathf.RoundToInt(y - 0.25f);
        return (GET_MAP(tX, tY));
    }

    static public void SET_MAP(int x, int y, int tNum)
    {
        // Сюда можно поместить дополнительную защиту или точку останова
        if (x < 0 || x >= W || y < 0 || y >= H)
        {
            return;   // Предотвратить исключение IndexOutOfRangeException
        }

        MAP[x, y] = tNum;
    }
}
