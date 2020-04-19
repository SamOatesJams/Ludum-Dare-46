using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Description")]
public class ItemDescription : ScriptableObject
{
    public ItemType Type;
    public string DisplayName;
    public PlaceableItem Prefab;
}
