using Godot;
using System;

public partial class AppleData : ItemData {
    public override int Uses    { get => HealsRemaining; set => HealsRemaining = value; }
    public override int MaxUses { get => TotalHeals;     set => TotalHeals = value; }
	public int HealAmount = 50;
	public int HealsRemaining = 3;
	public int TotalHeals = 3;
    public override ItemType Type => ItemType.Apple;

	public AppleData() {
		Name = "Apple";
        Icon = GetIcon(new Vector2I(2,0));
	}
	public bool CanHeal() => HealsRemaining > 0;

    public void Heal(Player player) {
        if (!CanHeal()) return;
        player.Health.Health += HealAmount;
        HealsRemaining--;
    }
}
