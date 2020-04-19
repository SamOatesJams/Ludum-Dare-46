using SamOatesGames.Systems;

public class ItemSelection : SubscribableMonoBehaviour
{
    public ItemDescription[] Items;

    public void WoodClicked()
    {
        Publish(new SwapItemEvent(Items[0]));
    }

    public void StoneClicked()
    {
        Publish(new SwapItemEvent(Items[1]));
    }

    public void MetalClicked()
    {
        Publish(new SwapItemEvent(Items[2]));
    }
}
