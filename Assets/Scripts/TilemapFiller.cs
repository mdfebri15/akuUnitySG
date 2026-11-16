using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapFiller : MonoBehaviour
{
    [Header("Referensi Tilemap")]
    [SerializeField] private Tilemap groundTilemap;  // drag object Ground
    [SerializeField] private Tilemap decorTilemap;   // drag object Decor (buat pagar)

    [Header("Tile")]
    [SerializeField] private TileBase groundTile;    // rumput polos
    [SerializeField] private TileBase borderTile;    // pagar / tembok

    [Header("Ukuran (dalam tile)")]
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;
    [SerializeField, Min(1)] private int borderThickness = 1;

    [ContextMenu("Fill All")]
    public void FillAll()
    {
        FillGround();
        FillBorder();
    }

    [ContextMenu("Fill Ground")]
    public void FillGround()
    {
        if (groundTilemap == null || groundTile == null)
        {
            Debug.LogWarning("Ground Tilemap atau Ground Tile belum diisi.");
            return;
        }

        int startX = -width / 2;
        int startY = -height / 2;

        BoundsInt area = new BoundsInt(startX, startY, 0, width, height, 1);
        TileBase[] tiles = new TileBase[width * height];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = groundTile;
        }

        groundTilemap.SetTilesBlock(area, tiles);
    }

    [ContextMenu("Fill Border")]
    public void FillBorder()
    {
        if (decorTilemap == null || borderTile == null)
        {
            Debug.LogWarning("Decor Tilemap atau Border Tile belum diisi.");
            return;
        }

        int startX = -width / 2;
        int startY = -height / 2;
        int endX = startX + width - 1;
        int endY = startY + height - 1;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                bool isEdge =
                    x < startX + borderThickness ||
                    x > endX - borderThickness ||
                    y < startY + borderThickness ||
                    y > endY - borderThickness;

                if (isEdge)
                {
                    decorTilemap.SetTile(new Vector3Int(x, y, 0), borderTile);
                }
            }
        }
    }

    [ContextMenu("Clear Ground")]
    public void ClearGround()
    {
        if (groundTilemap != null) groundTilemap.ClearAllTiles();
    }

    [ContextMenu("Clear Decor")]
    public void ClearDecor()
    {
        if (decorTilemap != null) decorTilemap.ClearAllTiles();
    }
}

