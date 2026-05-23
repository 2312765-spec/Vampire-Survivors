using UnityEngine;
using UnityEngine.Tilemaps;

public class GridRender : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap roadTilemap;
    [SerializeField] private Tilemap houseTilemap;
    [SerializeField] private Tilemap treeTilemap;
    [SerializeField] private Tilemap rockTilemap;
    [SerializeField] private Tilemap spawnTilemap;

    [Header("Database")]
    [SerializeField] private TileDatabase tileDatabase;

    public void Render(GridData gridData)
    {
        Clear();

        for (int x = 0; x < gridData.width; x++)
        {
            for (int y = 0; y < gridData.height; y++)
            {
                CellData cell = gridData.Cells[x, y];

                Vector3Int pos = new Vector3Int(x, y, 0);
                switch (cell.Type)
                {
                    case CellType.Ground:
                        groundTilemap.SetTile(pos, tileDatabase.GroundTile);
                        break;
                    case CellType.Road:
                        roadTilemap.SetTile(pos, tileDatabase.RoadTile);
                        break;
                    case CellType.House:
                        houseTilemap.SetTile(pos, tileDatabase.HouseTile);
                        break;
                    case CellType.Tree:
                        treeTilemap.SetTile(pos, tileDatabase.TreeTile);
                        break;
                    case CellType.Rock:
                        rockTilemap.SetTile(pos, tileDatabase.RockTile);
                        break;
                    case CellType.Spawn:
                        spawnTilemap.SetTile(pos, tileDatabase.SpawnTile);
                        break;
                }
            }
        }
    }

    public void Clear()
    {
        groundTilemap.ClearAllTiles();
        roadTilemap.ClearAllTiles();
        houseTilemap.ClearAllTiles();
        treeTilemap.ClearAllTiles();
        rockTilemap.ClearAllTiles();
        spawnTilemap.ClearAllTiles();
    }
}