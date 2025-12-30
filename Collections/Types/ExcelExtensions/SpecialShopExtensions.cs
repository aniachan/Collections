using Collections;
public static class SpecialShopExtensions
{
    public static Dictionary<uint, uint> BuildTomestones(this SpecialShop shop)
    {
        var tomestoneItems = ExcelCache<TomestonesItem>.GetSheet()
            .Where(t => t.Tomestones.RowId > 0)
            .OrderBy(t => t.Tomestones.RowId)
            .ToArray();

        var tomeStones = new Dictionary<uint, uint>();

        for (uint i = 0; i < tomestoneItems.Length; i++)
        {
            tomeStones[i + 1] = tomestoneItems[i].Item.RowId;
        }

        return tomeStones;
    }

    public static uint FixItemId(this SpecialShop shop, uint itemId, byte UseCurrencyType)
    {

        if (itemId == 0 || itemId > 7)
        {
            return itemId;
        }

        switch (UseCurrencyType)
        {
            // Scrips
            case 16:
                switch (itemId)
                {
                    case 1: return 28;
                    case 2: return 25199;
                    case 4: return 25200;
                    case 6: return 33913;
                    case 7: return 33914;
                    default: return itemId;
                }
            // Gil
            case 8:
                return 1;
            case 4:
                var tomestones = shop.BuildTomestones();
                return tomestones[itemId];
            case 2:
                if (itemId == 1)
                {
                    tomestones = shop.BuildTomestones();
                    return tomestones[itemId];
                }
                else
                {
                    return itemId;
                }
            default:
                return itemId;
                // Tomestones
                //case 4:
                //    if (TomeStones.ContainsKey(itemId))
                //    {
                //        return TomeStones[itemId];
                //    }

                //    return itemId;
        }
    }
}