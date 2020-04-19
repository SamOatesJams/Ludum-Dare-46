using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Game/Environment/Surface Data")]
public class EnvironmentSurfaceData : ScriptableObject
{
    public float MovementModifier;
    public float PathCost;

    public bool Blocked;
    public TileBase CollisionMapTile;

    public string SurfaceName
    {
        get => CollisionMapTile.name;
    }
}
