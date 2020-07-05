using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public enum TileStatus
    {
        Empty,
        Bean,
        Snake,
    }
    public TileStatus Status;
    public int Count;
    public MapTile PrevTile;

    private Material m_Mat;

    void Start()
    {
    }
    public void Refresh()
    {
        if(m_Mat==null)
            m_Mat = GetComponent<Renderer>().material;
        if (Count <= 0 && Status != TileStatus.Bean)
        {
            PrevTile = null;
            Status = TileStatus.Empty;
        }
        switch (Status)
        {
            case TileStatus.Empty:
                m_Mat.color = Color.white;
                break;
            case TileStatus.Snake:
                m_Mat.color = Color.yellow;
                break;
            case TileStatus.Bean:
                m_Mat.color = Color.green;
                break;
        }
        if (PrevTile && Count == PrevTile.Count)
        {
            PrevTile.Count--;
            PrevTile.Refresh();
        }
    }
}
