namespace Collections;

public class GlamourCollectible : Collectible<ItemAdapter>, ICreateable<GlamourCollectible, ItemAdapter>
{
    public new static string CollectionName => "Glamour";

    public GlamourCollectible(ItemAdapter excelRow) : base(excelRow)
    {
        //CollectibleKey = CollectibleKeyCache<ItemCollectibleKey, ItemAdapter>.Instance.GetObject((excelRow, true));
    }

    public static GlamourCollectible Create(ItemAdapter excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        return ExcelRow.Name;
    }

    protected override uint GetId()
    {
        return ExcelRow.RowId;
    }

    public override void UpdateObtainedState()
    {
        isObtained = Services.ItemFinder.IsItemInInventory(ExcelRow.RowId)
                    || Services.ItemFinder.IsItemInArmoireCache(ExcelRow.RowId)
                    || Services.ItemFinder.IsItemInDresser(ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override void Interact()
    {
        // Do nothing - taken care of by event service (should probably unify this in some way)
    }
}
