using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Referensi")]
    [SerializeField] private GameObject enemyPrefab;   // Prefab musuh yang di-spawn
    [SerializeField] private Transform player;         // Drag Player ke sini

    [Header("Spawn")]
    [SerializeField] private float startInterval = 1.5f;      // Jeda spawn di awal (detik)
    [SerializeField] private float minInterval = 0.3f;        // Jeda spawn tercepat (batas bawah)
    [SerializeField] private float intervalShrinkPerSecond = 0.01f; // Seberapa cepat jeda mengecil
    [SerializeField] private float offscreenMargin = 2f;      // Jarak ekstra di luar layar

    [Header("Scaling")]
    [SerializeField] private float healthRampSeconds = 60f;   // Tiap sekian detik, HP musuh +100% dari dasar

    [Header("Batas Map")]
    [SerializeField] private Vector2 mapHalfSize = new Vector2(23f, 23f); // Samain dengan batas player

    private Camera cam;              // Buat ngitung ukuran layar
    private PlayerHealth playerHealth;
    private float elapsed;           // Waktu yang sudah lewat sejak mulai
    private float spawnTimer;        // Hitung mundur ke spawn berikutnya

    private void Start()
    {
        cam = Camera.main;                                   // Kamera bertag MainCamera
        playerHealth = player.GetComponent<PlayerHealth>();  // Ambil script HP player
        spawnTimer = startInterval;                          // Spawn pertama setelah 1 jeda
    }

    private void Update()
    {
        if (playerHealth.IsDead) return;                     // Player mati, berhenti spawn

        elapsed += Time.deltaTime;                           // Tambah waktu berjalan
        spawnTimer -= Time.deltaTime;                        // Hitung mundur

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();                                    // Waktunya spawn
            spawnTimer = CurrentInterval();                  // Reset timer dengan jeda terbaru
        }
    }

    // Jeda spawn makin kecil seiring waktu, tapi ga kurang dari minInterval
    private float CurrentInterval()
    {
        return Mathf.Max(minInterval, startInterval - elapsed * intervalShrinkPerSecond);
    }

    // Pengali HP: menit 0 = 1x, menit 1 = 2x, menit 2 = 3x, dst
    private float HealthMultiplier()
    {
        return 1f + elapsed / healthRampSeconds;
    }

    private void SpawnEnemy()
    {
        // Setengah tinggi & lebar layar, lalu jarak ke pojok layar (diagonal) + margin
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        float radius = Mathf.Sqrt(halfW * halfW + halfH * halfH) + offscreenMargin;

        Vector2 spawnPos = player.position;   // Posisi cadangan kalau semua percobaan gagal

        // Coba sampai 10 kali cari titik di lingkaran luar layar yang masih di dalam map
        for (int i = 0; i < 10; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);                                  // Sudut acak (radian)
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;      // Titik di lingkaran
            Vector2 candidate = (Vector2)player.position + offset;                          // Posisi dunia

            bool insideMap = Mathf.Abs(candidate.x) <= mapHalfSize.x &&
                             Mathf.Abs(candidate.y) <= mapHalfSize.y;

            spawnPos = candidate;
            if (insideMap) break;                    // Ketemu titik valid, berhenti nyari
        }

        // Kalau percobaan terakhir pun di luar map, jepit ke dalam map
        spawnPos.x = Mathf.Clamp(spawnPos.x, -mapHalfSize.x, mapHalfSize.x);
        spawnPos.y = Mathf.Clamp(spawnPos.y, -mapHalfSize.y, mapHalfSize.y);

        // Bikin musuh, lalu atur HP-nya sesuai pengali waktu
        GameObject obj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy != null) enemy.Init(HealthMultiplier());
    }
}