using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ArenaArea : MonoSingleton<ArenaArea>
{
    [SerializeField] Tilemap Wall;
    Bounds WallBounds;
    public Bounds wallBounds => WallBounds;

    // Start is called before the first frame update
    void Start()
    {
        InitWallBound();
    }

    void OnValidate()
    {
        InitWallBound();
    }

    public void InitWallBound()
    {
        Wall.CompressBounds();
        BoundsInt cb = Wall.cellBounds;
        WallBounds = new Bounds(
            Wall.CellToWorld((cb.min + cb.max) / 2),
            Wall.CellToWorld(cb.max) - Wall.CellToWorld(cb.min)
        );
    }

    public bool CheckInWall(Vector2 point, Vector2? padding = null)
    {
        if (padding == null) padding = Vector2.one * 3;

        // 바운드 복사 및 축소
        Bounds b = WallBounds;
        b.Expand(-padding.Value * 2f); // padding만큼 축소 (좌우/상하)

        return b.Contains(point);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(WallBounds.center, WallBounds.size);

        Debug.Log(WallBounds.min + " " + WallBounds.max);
    }
}
