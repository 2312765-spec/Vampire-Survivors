using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleGenerator : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private int width = 100;

    [SerializeField] private int height = 100;
    [SerializeField] private int roadCount = 3;
    [SerializeField] private float centralAreaPercent = .25f;
    [SerializeField] private float obstacleDensity = .06f;
    [SerializeField] private float houseChance = .15f;
    [SerializeField] private float rockChance = .75f;
    [SerializeField] private int spawnerCount = 20;

    [Header("Seed")]
    [SerializeField] private int seed = 12345;

    [Header("Render")]
    [SerializeField] private GridRender renderner;
    [Header("Other")]
    [SerializeField]
    private PlayerSpawnBootstrap playerSpawnBootstrap;
    [SerializeField]
    private RuntimeObstacleSpawner obstacleSpawner;

    private GridData gridData;

    private RandomSeed rdm;

    private void Start()
    {
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        rdm = new RandomSeed(seed);

        gridData = new GridData(width, height);

        Debug.Log("Gene ground");
        GenerateGround();

        Debug.Log("Gene central");
        CentralAreaGenerator centralArea = new CentralAreaGenerator(gridData, rdm);
        centralArea.Generate(this.centralAreaPercent);

        Debug.Log("Gene road");
        RoadGenerator roadGenerator = new RoadGenerator(gridData, rdm);
        roadGenerator.Generate(this.roadCount);

        Debug.Log("Gene obstacle");
        ObstacleGenerator obstacleGenerator = new ObstacleGenerator(gridData, rdm);
        obstacleGenerator.Generate(this.obstacleDensity, houseChance, rockChance);

        Debug.Log("Gene spawner");
        SpawnGenerator spawnGenerator = new SpawnGenerator(gridData, rdm);
        spawnGenerator.Generate(this.spawnerCount);

        Debug.Log("checking obstacle");
        ConnectivityValidator validator = new ConnectivityValidator(gridData);
        validator.Validate();

        PlayerSpawnFinder spawnFinder = new PlayerSpawnFinder(gridData, rdm);
        Vector2Int playerSpawn = spawnFinder.FindSpawnPosition();
        playerSpawnBootstrap.Spawn(playerSpawn);

        Debug.Log("gene completed");
        renderner.Render(gridData);
        obstacleSpawner.Spawn(gridData);
    }

    private void GenerateGround()
    {
        int startX = 0;
        int endX = width;

        int startY = 0;
        int endY = height;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                CellData cell = gridData.GetCell(x, y);

                cell.Type = CellType.Ground;
                cell.Walkable = true;
            }
        }
    }
}