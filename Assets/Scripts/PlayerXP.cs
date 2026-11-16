using System;
using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int xpToNextLevel = 5;          // XP yang dibutuhin buat level 2
    [SerializeField] private float xpGrowth = 1.5f;          // Kebutuhan XP naik 1.5x tiap level

    public int Level { get; private set; } = 1;             // Level sekarang
    public int CurrentXP { get; private set; }              // XP sekarang di level ini
    public int XPToNextLevel => xpToNextLevel;              // Biar UI bisa baca

    public event Action<int, int> OnXPChanged;              // Kirim (XP sekarang, XP dibutuhin)
    public event Action<int> OnLevelUp;                     // Kirim level baru

    private PlayerWeapon weapon;

    private void Awake()
    {
        weapon = GetComponent<PlayerWeapon>();              // Ambil senjata buat upgrade otomatis
    }

    public void AddXP(int amount)
    {
        CurrentXP += amount;                                // Tambah XP

        // 'while' (bukan 'if') karena bisa naik beberapa level sekaligus kalau XP-nya banyak
        while (CurrentXP >= xpToNextLevel)
        {
            CurrentXP -= xpToNextLevel;                     // Sisa XP dibawa ke level berikutnya
            Level++;                                        // Naik level
            xpToNextLevel = Mathf.CeilToInt(xpToNextLevel * xpGrowth);   // Kebutuhan XP naik
            LevelUp();
        }

        OnXPChanged?.Invoke(CurrentXP, xpToNextLevel);      // Umumin buat UI
    }

    private void LevelUp()
    {
        Debug.Log($"LEVEL UP! Sekarang level {Level}");
        OnLevelUp?.Invoke(Level);                           // Umumin ke yang berlangganan

        // SEMENTARA (P3): upgrade acak otomatis. Di P4 bagian ini diganti menu pilih 1 dari 3
        if (UnityEngine.Random.value < 0.5f) weapon.UpgradeDamage(1);
        else weapon.UpgradeFireRate(0.2f);
    }
}