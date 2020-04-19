using SamOatesGames.Systems;

public class ItemSelection : SubscribableMonoBehaviour
{
    public PlaceableItem[] Items;
    public PlaceableItem SelectedItem;

    public void WoodClicked()
    {
        SelectedItem = Items[0];
        Publish(new SwapItemEvent(SelectedItem, this));
    }

    public void StoneClicked()
    {
        SelectedItem = Items[1];
        Publish(new SwapItemEvent(SelectedItem, this));
    }

    public void MetalClicked()
    {
        SelectedItem = Items[2];
        Publish(new SwapItemEvent(SelectedItem, this));
    }
}
