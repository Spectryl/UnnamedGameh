using Godot;
using System;

public partial class AppleData : ItemData {
	public int HealAmount = 50;
	public int HealsRemaining = 3;
	public int TotalHeals = 3;
    public override string HeldScene => UIDS.HeldApple;
    public override string PickupScene => UIDS.PickupApple;

	public AppleData() {
		Name = "Apple";
	}
	public bool CanHeal() => HealsRemaining > 0;

    public void Heal(Player player) {
        if (!CanHeal()) return;
        player.Health.Health += HealAmount;
        HealsRemaining--;
    }
}
