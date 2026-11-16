using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private float moveSpeed = 2f;       // Kecepatan jalan (lebih lambat dari player yang 5)
    [SerializeField] private int maxHealth = 3;          // HP dasar musuh
    [SerializeField] private int contactDamage = 10;     // Damage ke player kalau nabrak

    [Header("Drop")]
    [SerializeField] private GameObject xpGemPrefab;     // Prefab XP gem yang di-drop pas mati

    private Rigidbody2D rb;            // Fisika musuh
    private SpriteRenderer sr;         // Buat flip & efek kedip
    private Transform player;          // Posisi player yang dikejar
    private PlayerHealth playerHealth; // Buat ngecek player mati & ngasih damage
    private int currentHealth;         // HP musuh sekarang
    private Color originalColor;       // Warna asli sprite

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();                 // Ambil Rigidbody2D
        sr = GetComponentInChildren<SpriteRenderer>();    // Ambil SpriteRenderer
        originalColor = sr.color;                         // Simpen warna asli
        currentHealth = maxHealth;                        // HP awal (bisa ditimpa Init di bawah)
    }

    private void Start()
    {
        // Cari object bertag Player (dilakuin sekali pas musuh muncul)
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;                         // Simpen posisinya
            playerHealth = p.GetComponent<PlayerHealth>();// Simpen script HP-nya
        }
    }

    // Dipanggil spawner buat nge-scale HP musuh seiring waktu (1 = normal, 2 = 2x lipat)
    public void Init(float healthMultiplier)
    {
        currentHealth = Mathf.CeilToInt(maxHealth * healthMultiplier);   // Bulatkan ke atas
    }

    private void FixedUpdate()
    {
        // Kalau player ga ada / sudah mati, musuh berhenti
        if (player == null || playerHealth.IsDead)
        {
            rb.linearVelocity = Vector2.zero;             // Unity lama: rb.velocity
            return;
        }

        // Arah ke player = (posisi player - posisi musuh), dibikin panjang 1
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;        // Jalan ke arah player

        // Hadap kanan kalau player di kanan (asumsi sprite default menghadap kanan)
        sr.flipX = direction.x < 0;
    }

    // Dipanggil TERUS selama musuh bersentuhan dengan collider lain
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))      // Yang disentuh player?
        {
            // Beri damage. Jeda kebal di PlayerHealth yang mencegah damage tiap frame
            playerHealth.TakeDamage(contactDamage);
        }
    }

    // Dipanggil peluru pas kena musuh
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;                          // Kurangi HP
        StartCoroutine(Flash());                          // Kedip merah

        if (currentHealth <= 0) Die();                    // HP habis, mati
    }

    private void Die()
    {
        // Drop XP gem di posisi musuh mati
        if (xpGemPrefab != null)
        {
            Instantiate(xpGemPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);                              // Hapus musuh dari scene
    }

    private IEnumerator Flash()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.08f);
        // Cek dulu: musuh bisa saja sudah di-Destroy pas nunggu
        if (sr != null) sr.color = originalColor;
    }
}