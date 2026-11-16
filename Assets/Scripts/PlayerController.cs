using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Batas Map")]
    // Map 50x50 tile berpusat di (0,0), pagar 1 tile: 25 - 1 (pagar) - 1 (margin) = 23
    [SerializeField] private Vector2 mapHalfSize = new Vector2(23f, 23f);

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        // Ambil input (WASD / arrow key)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Normalize biar jalan diagonal ga lebih cepet
        moveInput = moveInput.normalized;

        // Flip sprite sesuai arah (asumsi sprite default menghadap kanan)
        if (moveInput.x != 0)
        {
            sr.flipX = moveInput.x < 0;
        }

        // Kirim ke Animator
        anim.SetBool("isMoving", moveInput.sqrMagnitude > 0f);
    }

    private void FixedUpdate()
    {
        // Gerakin lewat physics
        rb.linearVelocity = moveInput * moveSpeed;

        // Batasi player biar ga keluar dari map
        Vector2 pos = rb.position;
        pos.x = Mathf.Clamp(pos.x, -mapHalfSize.x, mapHalfSize.x);
        pos.y = Mathf.Clamp(pos.y, -mapHalfSize.y, mapHalfSize.y);
        rb.position = pos;
    }
}

