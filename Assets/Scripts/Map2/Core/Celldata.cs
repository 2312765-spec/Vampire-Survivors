using UnityEngine;

public class CellData
{
    public Vector2Int Position;
    public CellType Type;
    public bool Walkable;

    public CellData(Vector2Int position)
    {
        Position = position;
        Type = CellType.Empty;
        Walkable = false;
    }
}