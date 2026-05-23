using UnityEngine;

public class PlayerSpawnBootstrap : MonoBehaviour
{
    [SerializeField]
    private PlayerControllerMap2 player;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = player.GetComponent<Rigidbody2D>();
    }

    public void Spawn(Vector2Int cellPosition)
    {
        Vector2 worldPos = new(
            cellPosition.x + 0.5f,
            cellPosition.y + 0.5f
        );

        rb.position = worldPos;

        rb.velocity = Vector2.zero;
    }
}