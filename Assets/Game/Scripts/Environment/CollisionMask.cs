using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class CollisionMask : MonoBehaviour
{
    public EnvironmentSurfaceDataCollection SurfaceData;

    private Tilemap m_tilemap;

    // Start is called before the first frame update
    void Start()
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

    public void SetTile(Vector3Int location, ItemType type)
    {
        var data = SurfaceData.GetSurfaceData($"CM_{type.ToString()}");
        if (data == null)
        {
            return;
        }

        m_tilemap.SetTile(location, data.CollisionMapTile);
    }

    public void SetTile(Vector3 location, ItemType type)
    {
        SetTile(ToTilemapLoc(location), type);
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
}
