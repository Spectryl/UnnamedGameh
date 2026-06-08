using Godot;
using System;

public partial class Pickable : RigidBody3D {
	public ItemData Data;
	[Export] public string ItemName = "Item";
	
	[Export] public float PickupRange = 2.0f;

	public virtual void PickUp(Player player) {
		GD.Print($"Picked Up Item: {ItemName}");
		player.Inventory.AddItem(Data);
		QueueFree();
	}
}
