using UnityEngine;
using System.Collections.Generic;

public class CentralAreaGenerator
{
    private readonly GridData gridData;

    private readonly RandomSeed rds;

    private readonly HashSet<Vector2Int> visited = new();

    public CentralAreaGenerator(GridData gridData, RandomSeed rds)
    {
        this.gridData = gridData;
        this.rds = rds;
    }

    public void Generate(float fillPercent)
    {
        int targetCount =
            Mathf.RoundToInt(gridData.width * gridData.height * fillPercent);

        Vector2Int center = new(
            gridData.width / 2,
            gridData.height / 2
        );

        Queue<Vector2Int> queue = new();

        queue.Enqueue(center);

        visited.Add(center);

        int carved = 0;

        while (queue.Count > 0 && carved < targetCount)
        {
            Vector2Int current = queue.Dequeue();

            Carve(current);

            carved++;

            foreach (Vector2Int dir in Directions())
            {
                Vector2Int next = current + dir;

                if (!gridData.IsInside(next.x, next.y))
                    continue;

                if (visited.Contains(next))
                    continue;

                if (rds.Chance(0.7f))
                {
                    queue.Enqueue(next);

                    visited.Add(next);
                }
            }
        }
    }

    private void Carve(Vector2Int pos)
    {
        CellData cell = gridData.GetCell(pos.x, pos.y);

        cell.Type = CellType.Road;
        cell.Walkable = true;
    }

    private IEnumerable<Vector2Int> Directions()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.down;
        yield return Vector2Int.left;
        yield return Vector2Int.right;
    }
}