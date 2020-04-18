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

    public bool IsTileBlocked(Vector3Int location)
    {
        var tile = m_tilemap.GetTile(location);
        if (tile == null)
        {
            return false;
        }
        return tile.name.Contains("_B_");
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
        var tilemapPos = new Vector3Int(Mathf.FloorToInt(location.x - transform.localPosition.x), Mathf.FloorToInt(location.y - transform.localPosition.y), 0);
        return GetMovementMultiplier(tilemapPos);
    }
}
