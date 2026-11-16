using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;     // Kecepatan peluru
    [SerializeField] private float lifetime = 3f;   // Umur peluru (detik), biar ga numpuk di scene

    private Rigidbody2D rb;
    private int damage;                             // Damage yang dibawa (diisi oleh senjata)

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);              // Hapus peluru otomatis setelah 'lifetime' detik
    }

    // Dipanggil senjata tepat setelah peluru dibuat: arah tembak + damage
    public void Init(Vector2 direction, int dmg)
    {
        damage = dmg;                               // Simpen damage
        rb.linearVelocity = direction * speed;      // Luncurkan ke arah tembak (Unity lama: rb.velocity)
        transform.right = direction;                // Putar sprite biar "hidung" peluru searah gerak
    }

    // Dipanggil saat peluru (trigger) menyentuh collider lain
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;     // Abaikan kalau bukan musuh (player, pagar, dll)

        // TryGetComponent = ambil script Enemy kalau ada, aman tanpa error
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);               // Kasih damage ke musuh
        }
        Destroy(gameObject);                        // Peluru hilang setelah kena
    }
}