using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "VillageGenerator/Tile Database")]
public class TileDatabase : ScriptableObject
{
    public TileBase GroundTile;
    public TileBase RoadTile;
    public TileBase HouseTile;
    public TileBase TreeTile;
    public TileBase RockTile;
    public TileBase SpawnTile;
}