using SamOatesGames.Systems;

public class ItemSelection : SubscribableMonoBehaviour
{
    public void ItemClicked(ItemDescription item)
    {
        Publish(new SwapItemEvent(item));
    }
}
