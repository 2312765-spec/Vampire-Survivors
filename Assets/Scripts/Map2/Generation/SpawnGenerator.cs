using UnityEngine;

public class SpawnGenerator
{
    private readonly GridData gridData;

    private readonly RandomSeed rds;

    public SpawnGenerator(GridData gridData, RandomSeed rds)
    {
        this.gridData = gridData;
        this.rds = rds;
    }

    public void Generate(int count)
    {
        int placed = 0;

        int attempts = 0;

        while (placed < count && attempts < count * 20)
        {
            attempts++;

            int x = rds.Range(0, gridData.width);
            int y = rds.Range(0, gridData.height);

            if (!IsFarFromCenter(x, y))
                continue;

            CellData cell = gridData.GetCell(x, y);

            if (cell == null)
                continue;

            if (!cell.Walkable)
                continue;

            if (cell.Type != CellType.Ground &&
                cell.Type != CellType.Road)
                continue;

            if (!NearRoad(x, y, 6))
                continue;

            cell.Type = CellType.Spawn;

            placed++;
        }
    }

    private bool IsFarFromCenter(int x, int y)
    {
        int centerX = gridData.width / 2;
        int centerY = gridData.height / 2;

        int dx = Mathf.Abs(x - centerX);
        int dy = Mathf.Abs(y - centerY);

        return dx + dy >
               Mathf.Min(gridData.width, gridData.height) / 4;
    }

    private bool NearRoad(int x, int y, int radius)
    {
        for (int ox = -radius; ox <= radius; ox++)
        {
            for (int oy = -radius; oy <= radius; oy++)
            {
                int nx = x + ox;
                int ny = y + oy;

                if (!gridData.IsInside(nx, ny))
                    continue;

                CellData cell = gridData.GetCell(nx, ny);

                if (cell.Type == CellType.Road)
                    return true;
            }
        }

        return false;
    }
}