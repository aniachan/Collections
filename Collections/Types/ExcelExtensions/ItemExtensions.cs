using Collections;

public static class ItemExtensions
{

    public static EquipSlot GetEquipSlot(this Item item) => item.EquipSlotCategory.Value.GetEquipSlot();
    public static bool IsEquipment(this Item item) => item.GetEquipSlot() != EquipSlot.None;
}