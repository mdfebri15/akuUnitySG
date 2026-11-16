using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Batas Map")]
    [SerializeField] private bool clampToMap = true;
    [SerializeField] private Vector2 mapHalfSize = new Vector2(25f, 25f); // setengah ukuran map (50x50)

    private Vector3 velocity;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;

        if (clampToMap && cam != null && cam.orthographic)
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            float maxX = Mathf.Max(0f, mapHalfSize.x - halfWidth);
            float maxY = Mathf.Max(0f, mapHalfSize.y - halfHeight);

            targetPos.x = Mathf.Clamp(targetPos.x, -maxX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, -maxY, maxY);
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
    
}
