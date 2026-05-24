using UnityEngine;

public class GridData
{
    public int width;
    public int height;
    public CellData[,] Cells;

    public GridData(int width, int height)
    {
        this.width = width;
        this.height = height;

        Cells = new CellData[width, height];

        Initialize();
    }

    private void Initialize()
    {
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                Cells[x, y] = new CellData(new Vector2Int(x, y));
            }
        }
    }

    public bool IsInside(int x, int y)
    {
        return x >= 0 && y >= 0 && x < this.width && y < this.height;
    }

    public CellData GetCell(int x, int y)
    {
        if (!IsInside(x, y))
            return null;

        return Cells[x, y];
    }
}