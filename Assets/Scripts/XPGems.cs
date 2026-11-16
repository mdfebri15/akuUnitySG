using UnityEngine;

public class XPGem : MonoBehaviour
{
    [SerializeField] private int xpValue = 1;               // XP yang didapat player
    [SerializeField] private float magnetRange = 3f;        // Jarak gem mulai "ketarik" ke player
    [SerializeField] private float magnetSpeed = 8f;        // Kecepatan gem narik ke player
    [SerializeField] private float collectDistance = 0.3f;  // Jarak dianggap "sudah diambil"

    private Transform player;
    private PlayerXP playerXP;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerXP = p.GetComponent<PlayerXP>();
        }
    }

    private void Update()
    {
        if (player == null) return;                          // Player ga ada, diam

        float dist = Vector2.Distance(transform.position, player.position);   // Jarak ke player

        if (dist <= collectDistance)
        {
            playerXP.AddXP(xpValue);                         // Kasih XP
            Destroy(gameObject);                             // Gem hilang
        }
        else if (dist <= magnetRange)
        {
            // MoveTowards = geser sedikit ke arah target tiap frame
            transform.position = Vector3.MoveTowards(
                transform.position, player.position, magnetSpeed * Time.deltaTime);
        }
    }
}