using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class EmoteCollectible : Collectible<Emote>, ICreateable<EmoteCollectible, Emote>
{
    public new static string CollectionName => "Emotes";

    public EmoteCollectible(Emote excelRow) : base(excelRow)
    {
    }

    public static EmoteCollectible Create(Emote excelRow)
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
        return ExcelRow.UnlockLink;
    }

    protected override string GetPrimaryDescription()
    {
        return "";
    }

    protected override string GetSecondaryDescription()
    {
        return "";
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->IsEmoteUnlocked((ushort)ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override void Interact()
    {
        // Do nothing
    }
}
