using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class CollisionMask : MonoBehaviour
{
    private readonly Dictionary<string, float> TileCosts = new Dictionary<string, float>();

    private Tilemap m_tilemap;

    // Start is called before the first frame update
    void Start()
    {
        m_tilemap = GetComponent<Tilemap>();

        TileCosts.Add("CollisionMask_Path", 1.0f);
        TileCosts.Add("CollisionMask_Grass", 5.0f);
        TileCosts.Add("CollisionMask_Wall", 100.0f);
        TileCosts.Add("CollisionMask_Fence", 50.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetMovementMultiplier(Vector3Int location)
    {
        var tile = m_tilemap.GetTile(location);
        if (tile == null || !TileCosts.ContainsKey(tile.name))
        {
            return float.PositiveInfinity;
        }
        return TileCosts[tile.name];
    }

    public float GetMovementMultiplier(Vector3 location)
    {
        var tilemapPos = new Vector3Int(Mathf.FloorToInt(location.x - transform.localPosition.x), Mathf.FloorToInt(location.y - transform.localPosition.y), 0);
        return GetMovementMultiplier(tilemapPos);
    }
}
