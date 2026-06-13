using Godot;
using System.Collections.Generic;

public partial class PlayerHud : CanvasLayer {
    private TextureRect _CrosshairTexture;
    private List<InventorySlot> _Slots;
    private StatBar _HealthBar;
    private StatBar _StaminaBar;


    public override void _Ready() {
        _CrosshairTexture = GetNode<TextureRect>("Control/CrosshairContainer/CrosshairTexture");
        _Slots = new List<InventorySlot>();

        for (int i = 1; i <= 5; i++) {
            _Slots.Add(GetNode<InventorySlot>($"Control/Inventory/HBoxContainer/InventorySlot{i}"));
        }

        _HealthBar = GetNode<StatBar>("Control/Health/HBoxContainer/HealthBar");
        _StaminaBar = GetNode<StatBar>("Control/Stamina/HBoxContainer/StaminaBar");

    }

    public void Setup(PlayerInventory inventory, HealthComponent health, PlayerSprintComponent sprint) {
        inventory.SlotChanged         += OnSlotChanged;
        inventory.SelectedSlotChanged += OnSelectedSlotChanged;

        _HealthBar.Setup((float)health.Health / health.MaxHealth);
        health.OnHealthChanged += (_, newHealth) => _HealthBar.SetPercent((float)newHealth / health.MaxHealth);
        _StaminaBar.Setup(1f);
        sprint.OnStaminaChanged += (_, newStamina) => _StaminaBar.SetPercent(newStamina / sprint.MaxStamina);
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