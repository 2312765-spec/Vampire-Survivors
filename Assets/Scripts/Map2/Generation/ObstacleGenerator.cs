using UnityEngine;

public class ObstacleGenerator
{
    private readonly GridData gridData;
    private readonly RandomSeed rds;

    public ObstacleGenerator(GridData gridData, RandomSeed rds)
    {
        this.gridData = gridData;
        this.rds = rds;
    }

    public void Generate(float density, float houseChance, float rockChance)
    {
        int target =
            Mathf.RoundToInt(
                gridData.width *
                gridData.height *
                density
            );

        int placed = 0;

        int attempts = 0;

        while (placed < target && attempts < target * 10)
        {
            attempts++;

            int x = rds.Range(0, gridData.width);
            int y = rds.Range(0, gridData.height);

            CellData cell = gridData.GetCell(x, y);

            if (cell == null || 
                cell.Type is CellType.Road or CellType.House or CellType.Tree or CellType.Rock)
                continue;

            if (NearRoad(x, y, 4))
            {
                PlaceVillageObject(cell, houseChance);
            }
            else
            {
                PlaceNatureObject(cell, rockChance);
            }

            placed++;
        }
    }

    private void PlaceVillageObject(CellData cell, float houseChance)
    {
        if (rds.Chance(houseChance))
        {
            cell.Type = CellType.House;
        }
        else
        {
            cell.Type = CellType.Tree;
        }

        cell.Walkable = false;
    }

    private void PlaceNatureObject(CellData cell, float rockChance)
    {
        if (rds.Chance(rockChance))
        {
            cell.Type = CellType.Tree;
        }
        else
        {
            cell.Type = CellType.Rock;
        }

        cell.Walkable = false;
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