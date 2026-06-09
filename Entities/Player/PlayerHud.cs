using Godot;
using System.Collections.Generic;

public partial class PlayerHud : CanvasLayer {
	private TextureRect _CrosshairTexture;
    private List<InventorySlot> _Slots;

    public override void _Ready() {
        _CrosshairTexture = GetNode<TextureRect>("Control/CrosshairContainer/CrosshairTexture");
        _Slots = new List<InventorySlot>();

        for (int i = 1; i <= 5; i++) {
            _Slots.Add(GetNode<InventorySlot>($"Control/MarginContainer/HBoxContainer/InventorySlot{i}"));
        }
        
    }

        public void Setup(PlayerInventory inventory) {
            inventory.SlotChanged         += OnSlotChanged;
            inventory.SelectedSlotChanged += OnSelectedSlotChanged;
        }

        private void OnSlotChanged(int slot, ItemData item) {
            _Slots[slot].SetItem(item);
        }

        private void OnSelectedSlotChanged(int slot) {
            for (int i = 0; i < _Slots.Count; i++) {
                _Slots[i].SetSelected(i == slot);
            }
        }
    }
    
