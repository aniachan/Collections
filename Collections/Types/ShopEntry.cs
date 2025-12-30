namespace Collections;

public class Shop
{
    public List<(Item Item, int Amount)> Cost { get; init; }
    public uint? ENpcResidentId { get; init; }
    public uint ShopId { get; init; }
    public Shop(List<(Item Item, int Amount)> cost, uint? eNpcResidentId, uint shopId)
    {
        Cost = cost;
        ENpcResidentId = eNpcResidentId;
        ShopId = shopId;
    }
}
