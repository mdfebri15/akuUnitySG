using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Referensi")]
    [SerializeField] private GameObject projectilePrefab;   // Drag prefab Projectile

    [Header("Stat")]
    [SerializeField] private float fireRate = 1.5f;         // Tembakan per detik
    [SerializeField] private int damage = 1;                // Damage per peluru

    private float cooldown;                                 // Hitung mundur ke tembakan berikutnya
    private Camera cam;                                     // Kamera utama, buat ngubah posisi mouse ke posisi dunia
    private Vector2 lastDirection = Vector2.right;          // Arah tembak terakhir (cadangan kalau mouse tepat di atas player)

    private void Awake()
    {
        cam = Camera.main;                                  // Ambil kamera bertag MainCamera
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;                         // Kurangi cooldown tiap frame
        if (cooldown > 0f) return;                          // Belum waktunya nembak

        Vector2 direction = GetAimDirection();              // Hitung arah ke mouse
        Shoot(direction);                                   // Tembak ke arah itu
        cooldown = 1f / fireRate;                           // Jeda antar tembakan (1 / tembakan per detik)
    }

    // Arah dari player ke posisi mouse
    private Vector2 GetAimDirection()
    {
        // Input.mousePosition = posisi mouse di layar (satuan pixel)
        // ScreenToWorldPoint = ubah jadi posisi di dunia game (satuan unit)
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

        // Arah = posisi mouse - posisi player (z diabaikan karena game 2D)
        Vector2 toMouse = (Vector2)mouseWorld - (Vector2)transform.position;

        // Kalau mouse hampir tepat di atas player, arahnya ga stabil, jadi pakai arah terakhir saja
        if (toMouse.sqrMagnitude > 0.01f)
        {
            lastDirection = toMouse.normalized;             // Simpan arah baru, panjang vektor dibikin 1
        }
        return lastDirection;
    }

    private void Shoot(Vector2 direction)
    {
        // Bikin peluru di posisi player
        GameObject obj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        obj.GetComponent<Projectile>().Init(direction, damage);   // Kasih arah & damage
    }

    // ---- Dipakai saat level up (P3: otomatis, P4: lewat menu pilihan) ----
    public void UpgradeDamage(int amount)
    {
        damage += amount;                                   // Damage naik
        Debug.Log($"Damage jadi {damage}");
    }

    public void UpgradeFireRate(float percent)
    {
        fireRate *= 1f + percent;                           // 0.2 = tembak 20% lebih cepat
        Debug.Log($"Fire rate jadi {fireRate:F2}");
    }
}