using UnityEngine;

public class PlayerSpawnBootstrap : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    public void Spawn(Vector2Int cellPosition)
    {
        Vector3 worldPos = new(
            cellPosition.x + 0.5f,
            cellPosition.y + 0.5f,
            0f
        );

        player.transform.position = worldPos;
    }
}