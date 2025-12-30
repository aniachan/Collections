namespace Collections;

[Serializable]
public class GlamourTree
{
    public List<GlamourDirectory> Directories = new();
}

[Serializable]
public class GlamourDirectory
{
    public string Name;
    public List<GlamourSet> GlamourSets = new();

    public GlamourDirectory(string name)
    {
        Name = name;
    }
}

[Serializable]
public class GlamourSet
{
    public string Name;
    public Dictionary<EquipSlot, GlamourItem> Items = new();

    public GlamourSet(string name)
    {
        Name = name;
    }

    public GlamourItem? GetItem(EquipSlot equipSlot)
    {
        Items.TryGetValue(equipSlot, out var item);
        return item;
    }

    public void SetItem(EquipSlot equipSlot, GlamourItem glamouritem)
    {
        if (!Services.DataProvider.SupportedEquipSlots.Contains(equipSlot))
            throw new ArgumentOutOfRangeException($"Equip slot {equipSlot} not supported for GlamourSet");

        Items[equipSlot] = glamouritem;
    }

    public void SetItem(Item item, uint stain0Id, uint stain1Id, EquipSlot? equipSlot = null)
    {
        if (!Services.DataProvider.SupportedEquipSlots.Contains(item.GetEquipSlot()))
            throw new ArgumentOutOfRangeException($"Equip slot {item.GetEquipSlot()} not supported for GlamourSet");

        Items[equipSlot ?? item.GetEquipSlot()] = new GlamourItem(item.RowId, stain0Id, stain1Id);
    }

    public void ClearEquipSlot(EquipSlot equipSlot)
    {
        Items.Remove(equipSlot);
    }
}

[Serializable]
public class GlamourItem
{
    public uint ItemId;
    public uint Stain0Id;
    public uint Stain1Id;

    public GlamourItem(uint itemId, uint stain0Id, uint stain1Id)
    {
        ItemId = itemId;
        Stain0Id = stain0Id;
        Stain1Id = stain1Id;
    }

    public GlamourCollectible GetCollectible()
    {
        return CollectibleCache<GlamourCollectible, Item>.Instance.GetObject(ItemId);
    }

    public Stain GetStainPrimary()
    {
        return (Stain)ExcelCache<Stain>.GetSheet().GetRow(Stain0Id)!;
    }
    public Stain GetStainSecondary()
    {
        return (Stain)ExcelCache<Stain>.GetSheet().GetRow(Stain1Id)!;
    }


    public EquipSlot GetEquipSlot()
    {
        return GetCollectible().ExcelRow.GetEquipSlot();
    }
}
