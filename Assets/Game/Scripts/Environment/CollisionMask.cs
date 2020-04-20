using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class CollisionMask : MonoBehaviour
{
    public EnvironmentSurfaceDataCollection SurfaceData;

    private Tilemap m_tilemap;

    // Start is called before the first frame update
    void Awake()
    {
        m_tilemap = GetComponent<Tilemap>();
    }

    public Vector3Int ToTilemapLoc(Vector3 location)
    {
        var tilemapPos = new Vector3Int(Mathf.FloorToInt(location.x - transform.localPosition.x), Mathf.FloorToInt(location.y - transform.localPosition.y), 0);
        return tilemapPos;
    }

    public bool IsTileBlocked(Vector3Int location)
    {
        var tile = m_tilemap.GetTile(location);
        if (tile == null)
        {
            return true;
        }

        var data = SurfaceData.GetSurfaceData(tile.name);
        if (data == null)
        {
            return true;
        }

        return data.Blocked;
    }

    public bool IsTileBlocked(Vector3 location)
    {
        return IsTileBlocked(ToTilemapLoc(location));
    }

    public void SetTile(Vector3Int location, TileBase tile)
    {
        m_tilemap.SetTile(location, tile);
    }

    public void SetTile(Vector3 location, TileBase tile)
    {
        SetTile(ToTilemapLoc(location), tile);
    }

    public TileBase SwapTile(Vector3Int location, TileBase tile)
    {
        var existingTile = m_tilemap.GetTile(location);
        m_tilemap.SetTile(location, tile);
        return existingTile;
    }

    public TileBase SwapTile(Vector3 location, TileBase tile)
    {
        return SwapTile(ToTilemapLoc(location), tile);
    }

    public float GetMovementMultiplier(Vector3Int location)
    {
        var tile = m_tilemap.GetTile(location);
        if (tile == null)
        {
            return float.PositiveInfinity;
        }
        return SurfaceData.GetMovementModifier(tile.name);
    }

    public float GetMovementMultiplier(Vector3 location)
    {
        return GetMovementMultiplier(ToTilemapLoc(location));
    }

    public float GetPathCost(Vector3Int location)
    {
        var tile = m_tilemap.GetTile(location);
        if (tile == null)
        {
            return float.PositiveInfinity;
        }
        return SurfaceData.GetPathCost(tile.name);
    }

    public float GetPathCost(Vector3 location)
    {
        return GetPathCost(ToTilemapLoc(location));
    }
}
