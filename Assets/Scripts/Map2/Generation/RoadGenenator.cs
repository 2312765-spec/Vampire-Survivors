using UnityEngine;

public class RoadGenerator
{
    private readonly GridData gridData;

    private readonly RandomSeed rds;

    public RoadGenerator(GridData gridData, RandomSeed rds)
    {
        this.gridData = gridData;
        this.rds = rds;
    }

    public void Generate(int roadCount)
    {
        Vector2Int center = new(
            gridData.width / 2,
            gridData.height / 2
        );

        for (int i = 0; i < roadCount; i++)
        {
            Vector2Int target = RandomBorderPoint();

            GenerateRoad(center, target);
        }
    }

    private void GenerateRoad(Vector2Int start, Vector2Int target)
    {
        Vector2Int current = start;

        int maxSteps =
            gridData.width + gridData.height;

        for (int i = 0; i < maxSteps; i++)
        {
            Vector2Int dir = DirectionTo(current, target);

            dir = TryPerturbDirection(dir);

            current += dir;

            if (!gridData.IsInside(current.x, current.y))
                break;

            CarveRoad(current, dir);

            if (Distance(current, target) < 3)
                break;
        }
    }

    private void CarveRoad(Vector2Int pos, Vector2Int dir)
    {
        if (dir.x != 0)
        {
            for (int y = -1; y <= 1; y++)
            {
                Paint(pos.x, pos.y + y);
            }
        }
        else
        {
            for (int x = -1; x <= 1; x++)
            {
                Paint(pos.x + x, pos.y);
            }
        }
    }

    private void Paint(int x, int y)
    {
        if (!gridData.IsInside(x, y))
            return;

        CellData cell = gridData.GetCell(x, y);

        cell.Type = CellType.Road;
        cell.Walkable = true;
    }

    private Vector2Int DirectionTo(Vector2Int from, Vector2Int to)
    {
        Vector2Int delta = to - from;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            return delta.x > 0
                ? Vector2Int.right
                : Vector2Int.left;
        }

        return delta.y > 0
            ? Vector2Int.up
            : Vector2Int.down;
    }

    private Vector2Int TryPerturbDirection(Vector2Int dir)
    {
        if (!rds.Chance(0.12f))
            return dir;

        if (dir.x != 0)
        {
            return rds.Chance(0.5f)
                ? Vector2Int.up
                : Vector2Int.down;
        }

        return rds.Chance(0.5f)
            ? Vector2Int.left
            : Vector2Int.right;
    }

    private Vector2Int RandomBorderPoint()
    {
        int side = rds.Range(0, 4);

        return side switch
        {
            0 => new Vector2Int(
                0,
                rds.Range(0, gridData.height)
            ),

            1 => new Vector2Int(
                gridData.width - 1,
                rds.Range(0, gridData.height)
            ),

            2 => new Vector2Int(
                rds.Range(0, gridData.width),
                0
            ),

            _ => new Vector2Int(
                rds.Range(0, gridData.width),
                gridData.height - 1
            )
        };
    }

    private int Distance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) +
               Mathf.Abs(a.y - b.y);
    }
}