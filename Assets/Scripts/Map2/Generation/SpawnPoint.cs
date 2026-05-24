using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static SpawnPoint Instance;
    private readonly List<Vector3> spawnPoints = new List<Vector3>();

    private void Awake()
    {
        Instance = this;
    }

    public void Clear()
    {
        spawnPoints.Clear();
    }

    public void Add(Vector3 pos)
    {
        spawnPoints.Add(pos);
    }

    public Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }
}