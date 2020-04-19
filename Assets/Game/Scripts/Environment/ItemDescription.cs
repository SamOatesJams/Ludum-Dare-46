using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Description")]
public class ItemDescription : ScriptableObject
{
    public ItemType Type;
    public Sprite PreviewImage;
    public string DisplayName;
    public PlaceableItem Prefab;
    public ResourceType ResourceUsed;
    public int NumberOfResourcesUsed;
}
