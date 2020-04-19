using UnityEngine;

[CreateAssetMenu(menuName = "Game/Resource Description")]
public class ResourceDescription : ScriptableObject
{
    public ResourceType Type;
    public PickupableResource Prefab;
    public Vector2Int SpawnableAmount;
}
