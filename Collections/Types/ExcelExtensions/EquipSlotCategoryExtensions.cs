using Collections;

public static class EquipSlotCategoryExtensions
{
    public static EquipSlot GetEquipSlot(this EquipSlotCategory slot)
    {
        if (slot.MainHand != 0)
            return EquipSlot.MainHand;
        if (slot.OffHand != 0)
            return EquipSlot.OffHand;
        if (slot.Head != 0)
            return EquipSlot.Head;
        if (slot.Body != 0)
            return EquipSlot.Body;
        if (slot.Gloves != 0)
            return EquipSlot.Gloves;
        if (slot.Legs != 0)
            return EquipSlot.Legs;
        if (slot.Feet != 0)
            return EquipSlot.Feet;
        if (slot.Ears != 0) return EquipSlot.Ears;
        if (slot.Neck != 0) return EquipSlot.Neck;
        if (slot.Wrists != 0) return EquipSlot.Wrists;
        if (slot.FingerR != 0) return EquipSlot.FingerR;
        if (slot.FingerL != 0) return EquipSlot.FingerL;
        return EquipSlot.None;
    }
}