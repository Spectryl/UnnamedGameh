using Godot;
using System;

public partial class Pickable : RigidBody3D {
	public ItemData Data;
	[Export] public string ItemName = "Item";
	[Export] public float PickupRange = 2.0f;
	public override void _Ready() {
        if (Data == null) Data = CreateData();
    }
	public virtual void PickUp(Player player) {
		if (Data == null) return;
		GD.Print($"Picked Up Item: {ItemName}");
		player.Inventory.AddItem(Data);
		QueueFree();
	}
	protected virtual ItemData CreateData() => null;
}
