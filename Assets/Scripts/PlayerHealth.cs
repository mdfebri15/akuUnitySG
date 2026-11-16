using System;              // Dibutuhin buat 'Action' (dipakai di event)
using System.Collections;  // Dibutuhin buat Coroutine (efek kedip)
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private int maxHealth = 100;          // HP maksimal player
    [SerializeField] private float invincibleTime = 0.5f;  // Jeda kebal (detik) setelah kena hit

    // Properti: bisa dibaca script lain, tapi cuma script ini yang boleh ngubah
    public int CurrentHealth { get; private set; }   // HP sekarang
    public int MaxHealth => maxHealth;               // Biar script lain bisa baca HP maksimal
    public bool IsDead { get; private set; }         // true kalau player sudah mati

    // Event = "pengumuman". Script lain (UI di P4) bisa berlangganan dan dikabarin kalau HP berubah
    public event Action<int, int> OnHealthChanged;   // Kirim (HP sekarang, HP maksimal)
    public event Action OnDeath;                     // Dikabarin pas player mati

    private float lastHitTime = -999f;   // Waktu terakhir kena hit (angka kecil biar hit pertama pasti lolos)
    private SpriteRenderer sr;           // Buat efek kedip merah
    private Color originalColor;         // Warna asli sprite

    private void Awake()
    {
        CurrentHealth = maxHealth;                         // Mulai dengan HP penuh
        sr = GetComponentInChildren<SpriteRenderer>();     // Ambil SpriteRenderer player
        originalColor = sr.color;                          // Simpen warna asli
    }

    // Dipanggil musuh pas nabrak player
    public void TakeDamage(int amount)
    {
        if (IsDead) return;                                    // Sudah mati, abaikan
        if (Time.time < lastHitTime + invincibleTime) return;  // Masih masa kebal, abaikan

        lastHitTime = Time.time;                                       // Catat waktu hit ini
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);          // Kurangi HP, minimal 0
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);             // Umumin ke yang berlangganan ('?' = cek ada yang dengerin)
        Debug.Log($"Player HP: {CurrentHealth}/{maxHealth}");          // Buat ngecek di Console

        StartCoroutine(Flash());                                       // Kedip merah sebagai feedback

        if (CurrentHealth <= 0) Die();                                 // HP habis, mati
    }

    // Buat nambah HP (potion, level up, dll)
    public void Heal(int amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);  // Tambah HP, maksimal sampai penuh
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        IsDead = true;                                                 // Tandai mati (dicek musuh & spawner)
        Debug.Log("Player mati!");

        // Matiin kontrol dan senjata
        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;
        var weapon = GetComponent<PlayerWeapon>();
        if (weapon != null) weapon.enabled = false;

        // Hentiin gerakan sisa biar player ga meluncur
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;              // Unity lama: rb.velocity

        OnDeath?.Invoke();                                             // Umumin ke yang berlangganan (Game Over di P4)
    }

    // Coroutine = fungsi yang bisa "nunggu" di tengah jalan
    private IEnumerator Flash()
    {
        sr.color = Color.red;                        // Ubah jadi merah
        yield return new WaitForSeconds(0.1f);       // Tunggu 0.1 detik
        sr.color = originalColor;                    // Balikin warna asli
    }
}