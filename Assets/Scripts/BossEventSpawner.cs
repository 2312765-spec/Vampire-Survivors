using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEventSpawner : MonoBehaviour
{
    [Header("Boss Prefabs")]
    public GameObject dragonIcePrefab; // Kéo Prefab rồng băng vào đây
    public GameObject dragonFirePrefab; // Kéo Prefab rồng đỏ vào đây

    [Header("Spawn Settings")]
    public float spawnOffset = 15f; // Khoảng cách xuất hiện so với Player

    private Transform player;
    private int lastCheckedLevel = 0; // Để đảm bảo mỗi Level chỉ sinh Boss một lần duy nhất

    void Start()
    {
        // Lấy vị trí của Player thông qua Instance đã có sẵn của bạn
        if (PlayerHealthController.instance != null)
        {
            player = PlayerHealthController.instance.transform;
        }
    }

    void Update()
    {
        if (player == null || ExperienceLevelController.instance == null) return;

        int currentLevel = ExperienceLevelController.instance.currentLevel;

        // Nếu người chơi lên cấp mới và cấp đó lớn hơn hoặc bằng 8
        if (currentLevel >= 8 && currentLevel > lastCheckedLevel)
        {
            lastCheckedLevel = currentLevel; // Đánh dấu đã xử lý xong Level này
            SpawnBossesForLevel(currentLevel);
        }
    }

    void SpawnBossesForLevel(int level)
    {
        int iceDragonCount = 0;
        int fireDragonCount = 0;

        // --- CÔNG THỨC TOÁN HỌC TỰ ĐỘNG TÍNH SỐ LƯỢNG BOSS (Giữ nguyên) ---
        if (level == 8)
        {
            iceDragonCount = 1;
            fireDragonCount = 0;
        }
        else
        {
            iceDragonCount = 1 + (level - 9) / 9;
            fireDragonCount = 1 + (level - 9 + 1) / 9;
        }

        int totalBosses = iceDragonCount + fireDragonCount;
        Debug.Log($"⚔️ [BOSS EVENT] Cấp {level}! Tổng số Boss: {totalBosses} ({iceDragonCount} Băng, {fireDragonCount} Đỏ)");

        // Tạo một danh sách chứa tất cả các Boss cần sinh trong đợt này
        List<GameObject> bossesToSpawn = new List<GameObject>();
        
        for (int i = 0; i < iceDragonCount; i++) bossesToSpawn.Add(dragonIcePrefab);
        for (int i = 0; i < fireDragonCount; i++) bossesToSpawn.Add(dragonFirePrefab);

        // Sinh tất cả Boss ra và chia đều góc
        StartCoroutine(SpawnAllBossesEvenly(bossesToSpawn));
    }

    IEnumerator SpawnAllBossesEvenly(List<GameObject> bosses)
    {
    int total = bosses.Count;
    if (total == 0) yield break;

    float startingAngle = Random.Range(0f, 360f);
    float angleStep = 360f / total;

    for (int i = 0; i < total; i++)
    {
        if (bosses[i] == null) continue;

        float currentAngle = startingAngle + (i * angleStep);
        float angleRad = currentAngle * Mathf.Deg2Rad;

        Vector3 spawnPosition = player.position + 
            new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * spawnOffset;

        Instantiate(bosses[i], spawnPosition, Quaternion.identity);
        Debug.Log($"-> Boss thứ {i + 1} xuất hiện ở góc: {currentAngle % 360f} độ.");

        // ← Delay giữa mỗi con boss (chỉnh số giây tùy ý)
        yield return new WaitForSecondsRealtime(100f); // 2 giây/con, dùng Realtime vì Time.timeScale có thể = 0
    }
    }
}