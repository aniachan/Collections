namespace Collections;

public class QuestsDataGenerator : BaseDataGenerator<Quest>
{
    public readonly Dictionary<uint, Quest> EmoteToQuest = new(); // TODO connect

    private static readonly string FileName = "ItemIdToQuest.csv";
    protected override void InitializeData()
    {
        // Based on sheet
        var questSheet = ExcelCache<Quest>.GetSheet();
        foreach (var quest in questSheet)
        {
            var rewards = quest.Reward;
            var items = new List<Item>();

            foreach (var reward in rewards)
            {
                if (reward.RowId != 0)
                {
                    Item? item = ExcelCache<Item>.GetSheet().GetRow(reward.RowId);
                    // we only care about counting items that unlock a collectible or are glam
                    if(item != null && item.HasValue && item.Value.ItemAction.RowId != 0 || item.Value.ItemSortCategory.RowId == 5)
                        items.Add(item.Value);
                }
            }
            foreach (var item in items)
            {
                AddEntry(item.RowId, quest);
            }
            // emotes can be rewarded by quests separate from the books
            var emoteId = quest.EmoteReward.RowId;
            if (emoteId != 0)
            {
                EmoteToQuest[emoteId] = quest;
            }
        }

        // Based on resource data
        var resourceData = CSVHandler.Load<ItemIdToSource>(FileName);
        foreach (var entry in resourceData)
        {
            if (entry.SourceId == 0)
                continue;

            var quest = questSheet.GetRow(entry.SourceId);
            if (quest != null)
            {
                AddEntry(entry.ItemId, quest.Value);
            }
        }
    }
}
