using SamOatesGames.Systems;

public class ItemButton : SubscribableMonoBehaviour
{
    public ItemDescription ItemDescription;
    public UnityEngine.UI.Image SelectedImage;

    private UnityEngine.UI.Button m_button;

    public void Start()
    {
        m_button = GetComponent<UnityEngine.UI.Button>();

        var eventAggregator = EventAggregator.GetInstance();
        eventAggregator.Subscribe<ResourcePickupEvent>(this, OnResourcePickedUp);
        eventAggregator.Subscribe<ResourceUsedEvent>(this, OnResourceUsedEvent);
        eventAggregator.Subscribe<SwapItemEvent>(this, OnSwapItemEvent);

        EnsureEnabledState();
    }

    private void OnSwapItemEvent(SwapItemEvent args)
    {
        SelectedImage.enabled = args.ItemDescription == ItemDescription;
    }

    private void OnResourcePickedUp(ResourcePickupEvent args)
    {
        EnsureEnabledState();
    }

    private void OnResourceUsedEvent(ResourceUsedEvent args)
    {
        EnsureEnabledState();
    }

    private void EnsureEnabledState()
    {
        var inventorySystem = InventorySystem.GetInstance();
        var resourceCount = inventorySystem.GetItemAmount(ItemDescription.ResourceUsed);
        m_button.interactable = resourceCount >= ItemDescription.NumberOfResourcesUsed;
    }
}
