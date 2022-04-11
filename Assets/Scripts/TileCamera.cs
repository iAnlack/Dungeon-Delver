using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileSwap
{
    public int TileNum;
    public GameObject SwapPrefab;
    public GameObject GuaranteedItemDrop;
    public int OverrideTileNum = -1;
}

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
    public int DefaultTileNum;
    public List<TileSwap> TileSwaps;

    private Dictionary<int, TileSwap> _tileSwapDict;
    private Transform _enemyAnchor, _itemAnchor;

    private void Awake()
    {
        COLLISIONS = Utils.RemoveLineEndings(MapCollisions.text);
        PrepareTileSwapDict();
        _enemyAnchor = (new GameObject("Enemy Anchor")).transform;
        _itemAnchor = (new GameObject("Item Anchor")).transform;
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
                CheckTileSwaps(i, j);
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

    private void PrepareTileSwapDict()
    {
        _tileSwapDict = new Dictionary<int, TileSwap>();
        foreach(TileSwap ts in TileSwaps)
        {
            _tileSwapDict.Add(ts.TileNum, ts);
        }
    }

    private void CheckTileSwaps(int i, int j)
    {
        int tNum = GET_MAP(i, j);
        if (!_tileSwapDict.ContainsKey(tNum))
        {
            return;
        }
        // Мы должны заменить плитку
        TileSwap ts = _tileSwapDict[tNum];
        if (ts.SwapPrefab != null)
        {
            GameObject go = Instantiate(ts.SwapPrefab);
            Enemy e = go.GetComponent<Enemy>();
            if (e != null)
            {
                go.transform.SetParent(_enemyAnchor);
            }
            else
            {
                go.transform.SetParent(_itemAnchor);
            }
            go.transform.position = new Vector3(i, j, 0);
            if (ts.GuaranteedItemDrop != null)
            {
                if (e != null)
                {
                    e.GuaranteedItemDrop = ts.GuaranteedItemDrop;
                }
            }
        }

        // Заменить другой плиткой
        if (ts.OverrideTileNum == -1)
        {
            SET_MAP(i, j, DefaultTileNum);
        }
        else
        {
            SET_MAP(i, j, ts.OverrideTileNum);
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
