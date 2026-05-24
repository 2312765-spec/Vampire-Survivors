using UnityEngine;
using System.Collections.Generic;

public class ConnectivityValidator
{
    private readonly GridData gridData;
    private readonly bool[,] visited;

    public ConnectivityValidator(GridData gridData)
    {
        this.gridData = gridData;

        visited = new bool[
            gridData.width,
            gridData.height
        ];
    }

    public void Validate()
    {
        Vector2Int center = new(
            gridData.width / 2,
            gridData.height / 2
        );

        FloodFill(center);

        RemoveInvalidSpawns();
    }

    private void FloodFill(Vector2Int start)
    {
        Queue<Vector2Int> queue = new();

        queue.Enqueue(start);

        visited[start.x, start.y] = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int dir in Directions())
            {
                Vector2Int next = current + dir;

                if (!gridData.IsInside(next.x, next.y))
                    continue;

                if (visited[next.x, next.y])
                    continue;

                CellData cell = gridData.GetCell(next.x, next.y);

                if (!cell.Walkable)
                    continue;

                visited[next.x, next.y] = true;

                queue.Enqueue(next);
            }
        }
    }

    private void RemoveInvalidSpawns()
    {
        for (int x = 0; x < gridData.width; x++)
        {
            for (int y = 0; y < gridData.height; y++)
            {
                CellData cell = gridData.GetCell(x, y);

                if (cell.Type != CellType.Spawn)
                    continue;

                if (visited[x, y])
                    continue;

                cell.Type = CellType.Ground;
            }
        }
    }

    private IEnumerable<Vector2Int> Directions()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.down;
        yield return Vector2Int.left;
        yield return Vector2Int.right;
    }
}