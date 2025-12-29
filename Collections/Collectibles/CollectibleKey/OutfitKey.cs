namespace Collections;

public class OutfitKey : ItemKey, ICreateable<OutfitKey, (Item, int)>
{
    private IconHandler iconHandler { get; init; }
    // stores first item in an outfit
    public Item FirstItem { get; init; }

    public OutfitKey((Item, int) input) : base(input)
    {
        iconHandler = new IconHandler(input.Item1.Icon);
        List<Item> relatedItems = Services.ItemFinder.ItemsInOutfit(input.Item1.RowId);
        // this should always be true, but safeguard here just in case
        if (relatedItems.Count > 0)
        {
            FirstItem = relatedItems.First();
        }
    }

    public static new OutfitKey Create((Item, int) input)
    {
        return new(input);
    }

    protected override List<ICollectibleSource> GetCollectibleSources((Item, int) input)
    {
        var excelRow = input.Item1;
        var collectibleSources = new List<ICollectibleSource>();
        var dataGenerator = Services.DataGenerator.SourcesDataGenerator;
        
        // get other item IDs
        var relatedItems = Services.ItemFinder.ItemIdsInOutfit(excelRow.RowId);
        bool initializedSources = false;
        foreach(var item in relatedItems)
        {
            // try and re-use previous item sources
            var key = CollectibleKeyCache<ItemKey, Item>.Instance.GetObject((item, 0));
            if(key is not null)
            {
                // on the first pass, we'll just copy over the first item's sources
                if(!initializedSources)
                {
                    // deep copy
                    foreach(var source in key.CollectibleSources)
                    {
                        collectibleSources.Add(source.Clone());
                    }
                    initializedSources = true;
                    continue;
                }
                // handle combination of next related item sources
                foreach(var source in key.CollectibleSources)
                {
                    // add amount to next shop item, or if not found, add new shop source
                    if(source.GetType() == typeof(ShopSource))
                    {
                        var shopSourceIndex = collectibleSources.FindIndex(s => s.GetType() == typeof(ShopSource) && ((ShopSource)s).ShopId == ((ShopSource)source).ShopId);
                        // item obtained from a different shop than the first item.
                        if(shopSourceIndex == -1)
                        {
                            collectibleSources.Add(source.Clone());
                            continue;
                        }
                        // combine costs
                        var costItems = ((ShopSource)collectibleSources[shopSourceIndex]).costItems;
                        List<(ItemKey collectibleKey, int amount)>combined = costItems.Join(((ShopSource)source).costItems, c => c.collectibleKey.Id, d => d.collectibleKey.Id, (c, d) => (c.collectibleKey, c.amount + d.amount)).ToList();
                        ((ShopSource)collectibleSources[shopSourceIndex]).costItems = combined;
                    }
                }
            }
        }
        return collectibleSources;
    }

    public override Tradeability GetIsTradeable()
    {
        return !Input.Item1.IsUntradable ? Tradeability.Tradeable : Tradeability.UntradeableSingle;
    }
}

