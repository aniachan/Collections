namespace Collections;

public class OutfitsCollectible : Collectible<Item>, ICreateable<OutfitsCollectible, Item>
{
    public new static string CollectionName => "Outfits";

    public OutfitsCollectible(Item excelRow) : base(excelRow)
    {
    }

    public static OutfitsCollectible Create(Item excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        return ExcelRow.Name.ToString();
    }

    protected override uint GetId()
    {
        return ExcelRow.RowId;
    }

    protected override string GetDescription()
    {
        return "";
        // var items = Services.ItemFinder.ItemsInOutfit(ExcelRow.RowId);
        // return items.Select(i => i.Name).Aggregate((full, item) => full + "\n" + item).ToString();
    }

    public override void DrawAdditionalTooltip()
    {
        var items = Services.ItemFinder.ItemIdsInOutfit(ExcelRow.RowId);
        var collectibles = Services.DataProvider.GetCollection<GlamourCollectible>()?.Where(c => items.Contains(c.Id)).ToList();
        for(int i = 0; i < collectibles?.Count; i++)
        {
            var c = collectibles[i];
            var icon = c.GetIconLazy();
            if (icon != null)
            {
                var origPos = ImGui.GetCursorPos();
                ImGui.Image(icon.GetWrapOrEmpty().Handle, new Vector2(50, 50));
                c.UpdateObtainedState();
                if (c.GetIsObtained())
                {
                    var _ = true;
                    UiHelper.IconButtonWithOffset(0, FontAwesomeIcon.Check, 32, -32, ref _, 1.1f, new Vector4(1f, .741f, .188f, 1).Darken(.7f), ColorsPalette.BLACK.WithAlpha(0));
                    UiHelper.IconButtonWithOffset(0, FontAwesomeIcon.Check, 32, -32, ref _, 1.0f, new Vector4(1f, .741f, .188f, 1), ColorsPalette.BLACK.WithAlpha(0));
                }
                if (i < collectibles.Count - 1)
                {
                    ImGui.SameLine();
                }
            }
        }
    }

    protected override HintModule GetSecondaryHint()
    {
        if (this.CollectibleKey == null) return base.GetSecondaryHint();
        return new HintModule($"{(this.CollectibleKey as OutfitKey).FirstItem.ClassJobCategory.Value.Name}, Lv. {(this.CollectibleKey as OutfitKey).FirstItem.LevelEquip}", null);
    }

    public override void UpdateObtainedState()
    {
        isObtained = Services.ItemFinder.IsItemInDresser(ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public int GetNumberOfDyeSlots()
    {
        if (this.CollectibleKey == null) return 0;
        return (CollectibleKey as OutfitKey).FirstItem.DyeCount;
    }

    public override void Interact()
    {
        // Do nothing - taken care of by event service (should probably unify this in some way)
    }
}
