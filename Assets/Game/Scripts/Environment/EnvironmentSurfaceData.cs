using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Game/Environment/Surface Data")]
public class EnvironmentSurfaceData : ScriptableObject
{
    public string SurfaceName;
    public float MovementModifier;
    public TileBase CollisionMapEntry;
}
