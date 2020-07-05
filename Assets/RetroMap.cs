using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class RetroMap : MonoBehaviour
{
    public enum MoveDirection
    {
        Right,
        Left,
        Up,
        Down,
    }
    public GameObject TilePrefab;
    [Range(5,20)]
    public int MapWidth;
    [Range(5,20)]
    public int MapHeight;
    public float TileSize;
    public float MoveTimeGap = 1;
    private MapTile[,] m_Tiles;
    // Start is called before the first frame update
    void Start()
    {
        m_Tiles = new MapTile[MapWidth, MapHeight];
        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                var tile = GameObject.Instantiate(TilePrefab);
                tile.transform.SetParent(transform);
                tile.transform.localPosition = new Vector3(
                    i * TileSize,
                    0,
                    j * TileSize
                    );
                m_Tiles[i, j] = tile.GetComponent<MapTile>();
            }
        }
        var centerTile = m_Tiles[MapWidth / 2, MapHeight / 2];
        centerTile.Status = MapTile.TileStatus.Snake;
        centerTile.PrevTile = m_Tiles[MapWidth / 2 - 1, MapHeight / 2];
        centerTile.Count = 2;
        centerTile.Refresh();
        centerTile.PrevTile.Status = MapTile.TileStatus.Snake;
        centerTile.PrevTile.Count = 1;
        centerTile.PrevTile.Refresh();
        m_CurrentSnakeLen = 2;
        m_CurrentSnakeHead = Tile2Index(MapWidth / 2, MapHeight / 2, MapWidth);
    }

    private float m_CurrTime;
    void FixedUpdate()
    {
        if(m_CurrTime< MoveTimeGap)
        {
            m_CurrTime += Time.fixedDeltaTime;
        }
        else
        {
            m_CurrTime = 0;
            snakeMove();
        }
        refreshInput();
    }
    private int createBean()
    {
        int index = UnityEngine.Random.Range(0, MapHeight * MapWidth);
        var pos = Index2Tile(index, MapWidth);
        var tile = m_Tiles[pos.x, pos.y];
        if (tile.Status != MapTile.TileStatus.Empty)
            return -1;
        tile.Status = MapTile.TileStatus.Bean;
        tile.Refresh();
        return index;
    }
    private void refreshInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (m_SnakeDir != MoveDirection.Right)
                m_InputDir = MoveDirection.Left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (m_SnakeDir != MoveDirection.Left)
                m_InputDir = MoveDirection.Right;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (m_SnakeDir != MoveDirection.Up)
                m_InputDir = MoveDirection.Down;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (m_SnakeDir != MoveDirection.Down)
                m_InputDir = MoveDirection.Up;
        }

    }
    private MoveDirection m_SnakeDir, m_InputDir;
    private int m_CurrentSnakeHead;
    private int m_CurrentSnakeLen;
    private int m_BeanPos=-1;
    private void snakeMove()
    {
        int prevIndex = m_CurrentSnakeHead;
        int2 currTile = Index2Tile(m_CurrentSnakeHead, MapWidth);
        switch (m_InputDir)
        {
            case MoveDirection.Down:
                currTile.y--;
                if (isInBound(currTile))
                    m_CurrentSnakeHead = Tile2Index(currTile.x, currTile.y, MapWidth);
                else
                {
                    Dead();
                    return;
                }
                break;
            case MoveDirection.Up:
                currTile.y++;
                if (isInBound(currTile))
                    m_CurrentSnakeHead = Tile2Index(currTile.x, currTile.y, MapWidth);
                else
                {
                    Dead();
                    return;
                }
                break;
            case MoveDirection.Left:
                currTile.x--;
                if (isInBound(currTile))
                    m_CurrentSnakeHead = Tile2Index(currTile.x, currTile.y, MapWidth);
                else
                {
                    Dead();
                    return;
                }
                break;
            case MoveDirection.Right:
                currTile.x++;
                if (isInBound(currTile))
                    m_CurrentSnakeHead = Tile2Index(currTile.x, currTile.y, MapWidth);
                else
                {
                    Dead();
                    return;
                }
                break;
        }
        if (m_BeanPos == -1)
        {
            m_BeanPos = createBean();
        }
        int2 tilePos = Index2Tile(m_CurrentSnakeHead, MapWidth);
        var tile = m_Tiles[tilePos.x, tilePos.y];
        var prevStats = tile.Status;
        tile.Status = MapTile.TileStatus.Snake;
        int2 prevTilePos = Index2Tile(prevIndex, MapWidth);
        var prevTile = m_Tiles[prevTilePos.x, prevTilePos.y];
        if (prevStats == MapTile.TileStatus.Bean)
        {
            m_CurrentSnakeLen++;
            //prevTile.Count--;
            //prevTile.Refresh();
            m_BeanPos = createBean();
        }
        else if (prevStats == MapTile.TileStatus.Snake)
        {
            Dead();
        }
        tile.Count = m_CurrentSnakeLen;
        tile.PrevTile = prevTile;
        tile.Refresh();
        m_SnakeDir = m_InputDir;
    }
    private bool isInBound(int2 pos)
    {
        return pos.x >= 0 && pos.x < MapWidth && pos.y >= 0 && pos.y < MapHeight;
    }
    public void Dead()
    {
        MoveTimeGap = float.MaxValue;
        print("You are shocked!!");
    }
    public static int Tile2Index(int x,int y,int mapW)
    {
        return y * mapW + x;
    }
    public static int2 Index2Tile(int index,int mapW)
    {
        return new int2(index % mapW, index / mapW);
    }
}
