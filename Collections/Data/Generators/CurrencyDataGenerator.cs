namespace Collections;

public class CurrencyDataGenerator
{
    // manual information for item categories
    // if there's an in-data identifier, use that instead.
    public Dictionary<uint, SourceCategory> ItemIdToSourceCategory = new()
    {
        { 00001, SourceCategory.Gil }, // Gil
        { 00020, SourceCategory.CompanySeals }, // Storm Seal (designated company seals)
        { 00025, SourceCategory.PvP }, // Wolf Mark
        { 21067, SourceCategory.PvP }, // Wolf Collar
        { 36656, SourceCategory.PvP }, // Trophy Crystals
        { 30341, SourceCategory.Duty }, // Faux Leaves
        { 10310, SourceCategory.Scrips }, // Blue gatherers scrip
        { 10311, SourceCategory.Scrips }, // Red gatherers scrip
        { 17834, SourceCategory.Scrips }, // Yellow gatherers scrip
        { 25200, SourceCategory.Scrips }, // White gatherers scrip
        { 33914, SourceCategory.Scrips }, // Purple gatherers scrip
        { 10308, SourceCategory.Scrips }, // Blue gatherers scrip
        { 10309, SourceCategory.Scrips }, // Red gatherers scrip
        { 17833, SourceCategory.Scrips }, // Yellow gatherers scrip
        { 25199, SourceCategory.Scrips }, // White gatherers scrip
        { 33913, SourceCategory.Scrips }, // Purple gatherers scrip
        { 00027, SourceCategory.TheHunt }, // Allied Seal
        { 10307, SourceCategory.TheHunt }, // Centurio Seal
        { 26533, SourceCategory.TheHunt }, // Sack of nuts
        { 26807, SourceCategory.Fate}, // Bicolor gems
        { 00029, SourceCategory.MGP }, // MGP
        { 41629, SourceCategory.MGP}, // MGF (fall guys)
        { 37549, SourceCategory.IslandSanctuary }, // Seafarer's Cowrie
        { 37550, SourceCategory.IslandSanctuary }, // Islander's Cowrie
        { 28063, SourceCategory.RestorationZone}, // Skybuilder Scrips
        { 45690, SourceCategory.RestorationZone}, // Cosmocredit
        { 47343, SourceCategory.RestorationZone}, // Phaenna token booklet
        { 47594, SourceCategory.RestorationZone}, // Phaenna exploration token 
        { 48146, SourceCategory.RestorationZone}, // Phaenna credit
        // Occult Crescent
        { 47868, SourceCategory.FieldOperations}, // Sanguinite
    };

    public CurrencyDataGenerator()
    {
        PopulateData();
    }

    private void PopulateData()
    {
        // Add Tomestones
        var TomestonesItemSheet = ExcelCache<TomestonesItem>.GetSheet();
        foreach (var tomestone in TomestonesItemSheet)
        {
            ItemIdToSourceCategory[tomestone.Item.RowId] = SourceCategory.Tomestones;
        }

        // Add Beast tribe currencies
        var beastTribeSheet = ExcelCache<BeastTribe>.GetSheet();
        foreach (var beastTribe in beastTribeSheet)
        {
            ItemIdToSourceCategory[beastTribe.CurrencyItem.RowId] = SourceCategory.BeastTribes;
        }

        // generate currency items where we know categories
        var ItemSheet = ExcelCache<Item>.GetSheet();
        foreach (var item in ItemSheet)
        {
            switch (item.ItemSortCategory.RowId)
            {
                // Deep Dungeon Currency items
                case 41:
                    ItemIdToSourceCategory[item.RowId] = SourceCategory.DeepDungeon;
                    break;
                // Exploration Zones
                case 44: // Eureka
                case 48: // Bozja
                case 86: // Occult Crescent
                    ItemIdToSourceCategory[item.RowId] = SourceCategory.FieldOperations;
                    break;
                // FATEs
                case 55: 
                    // filters out non-fate currencies
                    if(item.Unknown4 == 24000)
                        ItemIdToSourceCategory[item.RowId] = SourceCategory.Fate;
                    break;
            }
        }
    }
}
