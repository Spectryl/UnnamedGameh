using Godot;
using System;

public partial class PlayerInventory {
    public int Size { get; private set; }
    public ItemData[] Slots { get; private set; }
    public int SelectedSlot { get; private set; }

    public Action<int, ItemData> SlotChanged;
    public Action<int> SelectedSlotChanged;

    public ItemData SelectedItem => Slots[SelectedSlot];

    public PlayerInventory(int size = 5) {
        Size = size;
        Slots = new ItemData[size];
    }

    public bool AddItem(ItemData item) {
        for (int i = 0; i < Size; i++) {
            if (Slots[i] == null) {
                Slots[i] = item;
				GD.Print($"AddItem: set slot {i} to {item.Name}, SelectedSlot is {SelectedSlot}");
                SlotChanged?.Invoke(i, item);
				if (i == SelectedSlot) SelectedSlotChanged?.Invoke(i);
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(int slot) {
        Slots[slot] = null;
        SlotChanged?.Invoke(slot, null);
        if (slot == SelectedSlot) SelectedSlotChanged?.Invoke(slot);
    }

    public void SelectSlot(int slot) {
        if (slot < 0 || slot >= Size) return;
        SelectedSlot = slot;
        SelectedSlotChanged?.Invoke(slot);
    }

    public void NotifySlotChanged() => SlotChanged?.Invoke(SelectedSlot, Slots[SelectedSlot]);

    public void SelectNext() => SelectSlot((SelectedSlot + 1) % Size);
    public void SelectPrevious() => SelectSlot((SelectedSlot - 1 + Size) % Size);
}