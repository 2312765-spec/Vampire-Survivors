using UnityEngine;

public class PlayerSpawnFinder
{
    private readonly GridData gridData;
    private readonly RandomSeed rds;

    public PlayerSpawnFinder(GridData gridData, RandomSeed rds)
    {
        this.gridData = gridData;
        this.rds = rds;
    }

    public Vector2Int FindSpawnPosition()
    {
        Vector2Int current = new(
            gridData.width / 2,
            gridData.height / 2
        );

        int maxAttempts =
            gridData.width * gridData.height;

        for (int i = 0; i < maxAttempts; i++)
        {
            CellData cell =
                gridData.GetCell(current.x, current.y);

            if (cell != null && cell.Walkable)
            {
                return current;
            }

            current += RandomDirection();

            current.x = Mathf.Clamp(
                current.x,
                0,
                gridData.width - 1
            );

            current.y = Mathf.Clamp(
                current.y,
                0,
                gridData.height - 1
            );
        }

        return new Vector2Int(
            gridData.width / 2,
            gridData.height / 2
        );
    }

    private Vector2Int RandomDirection()
    {
        int v = rds.Range(0, 4);

        return v switch
        {
            0 => Vector2Int.up,
            1 => Vector2Int.down,
            2 => Vector2Int.left,
            _ => Vector2Int.right
        };
    }
}