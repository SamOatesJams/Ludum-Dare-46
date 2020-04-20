using UnityEngine;
using UnityEngine.Tilemaps;
using SamOatesGames.Systems;

public class PlaceableItem : SubscribableMonoBehaviour
{
    public double MaxHealth = 20.0f;
    public ItemType Type;
    public EnvironmentSurfaceData SurfaceData;

    private CollisionMask m_collisionMask;
    private TileBase m_previousTile;
    private Vector3Int m_collisionMaskLocation;

    public void RestoreCollisionMask()
    {
        m_collisionMask.SetTile(m_collisionMaskLocation, m_previousTile);
    }

    public void SwapCollisionMask(CollisionMask mask, Vector3Int tileLoc)
    {
        m_collisionMaskLocation = tileLoc;
        m_collisionMask = mask;
        m_previousTile = m_collisionMask.SwapTile(tileLoc, SurfaceData.CollisionMapTile);
    }
}
