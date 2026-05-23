using UnityEngine;

public class RuntimeObstacleSpawner : MonoBehaviour
{
    [SerializeField]
    private ObstaclePrefabDatabase database;

    [SerializeField]
    private Transform obstacleRoot;

    public void Spawn(GridData gridData)
    {
        Clear();

        for (int x = 0; x < gridData.width; x++)
        {
            for (int y = 0; y < gridData.height; y++)
            {
                CellData cell =
                    gridData.GetCell(x, y);

                GameObject prefab =
                    GetPrefab(cell.Type);

                if (prefab == null)
                    continue;

                Vector3 pos = new(
                    x + 0.5f,
                    y + 0.5f,
                    0f
                );

                Instantiate(
                    prefab,
                    pos,
                    Quaternion.identity,
                    obstacleRoot
                );
            }
        }
    }

    private GameObject GetPrefab(CellType type)
    {
        return type switch
        {
            CellType.Tree => database.TreePrefab,

            CellType.Rock => database.RockPrefab,

            CellType.House => database.HousePrefab,

            _ => null
        };
    }

    private void Clear()
    {
        for (int i = obstacleRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(
                obstacleRoot.GetChild(i).gameObject
            );
        }
    }
}