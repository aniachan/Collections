namespace Collections;

public class CollectibleKeyDataGenerator
{
    public readonly Dictionary<Type, Dictionary<uint, ItemAdapter>> collectibleIdToItem = new();
    public readonly Dictionary<Type, Dictionary<uint, Quest>> collectibleIdToQuest = new();
    public readonly Dictionary<Type, Dictionary<uint, ContentFinderCondition>> collectibleIdToInstance = new();
    public readonly Dictionary<Type, Dictionary<uint, Achievement>> collectibleIdToAchievement = new();
    public readonly Dictionary<Type, Dictionary<uint, string>> collectibleIdToMisc = new();
    public readonly Dictionary<uint, ENpcResident> cardItemIdToNpc = new();

    private static readonly int MountItemActionType = 1322;
    private static readonly int MinionItemActionType = 853;
    private static readonly int EmoteHairstyleItemActionType = 2633;
    private static readonly int TripleTriadItemActionType = 3357;
    private static readonly int BardingItemActionType = 1013;

    public CollectibleKeyDataGenerator()
    {
        PopulateItemData();
        PopulateQuestData();
        PopulateInstanceData();
        PopulateAchievementData();
        PopulateMiscData();
        PopulateTripleTriadToNpcData();
    }

    private void PopulateItemData()
    {
        foreach (var item in ExcelCache<ItemAdapter>.GetSheet())
        {
            var type = item.ItemAction.Value?.Type;
            var collectibleData = item.ItemAction.Value?.Data;
            if (type == MountItemActionType)
            {
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(Mount), collectibleData[0], item);
            }
            else if (type == MinionItemActionType)
            {
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(Companion), collectibleData[0], item);
            }
            else if (type == EmoteHairstyleItemActionType)
            {
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(Emote), collectibleData[0], item);
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(CharaMakeCustomize), collectibleData[0], item);
            }
            else if (type == TripleTriadItemActionType)
            {
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(TripleTriadCard), collectibleData[0], item);
            }
            else if (type == BardingItemActionType)
            {
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(BuddyEquip), collectibleData[0], item);
            }
        }
    }

    private void PopulateQuestData()
    {
        foreach (var quest in ExcelCache<Quest>.GetSheet())
        {
            var emote = quest.EmoteReward.Value;
            if (emote is not null && emote.RowId != 0)
            {
                AddCollectibleKeyEntry(collectibleIdToQuest, typeof(Emote), emote.UnlockLink, quest);
            }
        }

        foreach (var emote in ExcelCache<Emote>.GetSheet())
        {
            if (emote.UnlockLink > ExcelCache<Quest>.GetSheet().First().RowId && emote.UnlockLink < ExcelCache<Quest>.GetSheet().Last().RowId)
            {
                var quest = ExcelCache<Quest>.GetSheet().GetRow(emote.UnlockLink);
                AddCollectibleKeyEntry(collectibleIdToQuest, typeof(Emote), emote.UnlockLink, quest);
            }
        }

        foreach (var (type, dict) in DataOverrides.collectibleIdToUnlockQuestId)
        {
            foreach (var (collectibleId, questId) in dict)
            {
                var quest = ExcelCache<Quest>.GetSheet().GetRow(questId);
                AddCollectibleKeyEntry(collectibleIdToQuest, type, collectibleId, quest);
            }
        }
    }

    private void PopulateInstanceData()
    {
        foreach (var (type, dict) in DataOverrides.collectibleIdToUnlockInstanceId)
        {
            foreach (var (collectibleId, instanceId) in dict)
            {
                var instance = ExcelCache<ContentFinderCondition>.GetSheet().GetRow(instanceId);
                AddCollectibleKeyEntry(collectibleIdToInstance, type, collectibleId, instance);
            }
        }
    }

    private void PopulateAchievementData()
    {
        foreach (var (type, dict) in DataOverrides.collectibleIdToUnlockAchievementId)
        {
            foreach (var (collectibleId, achievementId) in dict)
            {
                var achievement = ExcelCache<Achievement>.GetSheet().GetRow(achievementId);
                AddCollectibleKeyEntry(collectibleIdToAchievement, type, collectibleId, achievement);
            }
        }
    }

    private void PopulateMiscData()
    {
        foreach (var (type, dict) in DataOverrides.collectibleIdToUnlockMisc)
        {
            foreach (var (collectibleId, misc) in dict)
            {
                AddCollectibleKeyEntry(collectibleIdToMisc, type, collectibleId, misc);
            }
        }
    }

    private void PopulateTripleTriadToNpcData()
    {
        foreach (var tripleTriadCardResident in ExcelCache<TripleTriadCardResident>.GetSheet())
        {
            if (tripleTriadCardResident.AcquisitionType == 6 || tripleTriadCardResident.AcquisitionType == 10)
            {
                if (tripleTriadCardResident.Acquisition != 0)
                {
                    if (collectibleIdToItem.TryGetValue(typeof(TripleTriadCard), out var dict))
                    {
                        if (dict.TryGetValue(tripleTriadCardResident.RowId, out var cardItem))
                        {
                            cardItemIdToNpc[cardItem.RowId] = ExcelCache<ENpcResident>.GetSheet().GetRow(tripleTriadCardResident.Acquisition);
                        }
                    }
                }
            }
        }
    }

    private void AddCollectibleKeyEntry<T>(Dictionary<Type, Dictionary<uint, T>> dict, Type type, uint id, T entry)
    {
        if (!dict.ContainsKey(type))
        {
            dict[type] = new Dictionary<uint, T>();
        }
        dict[type][id] = entry;
    }
}