using Godot;
using System;

public partial class Level : Node {
	private void CreatePlayer(int id) {
		Player newPlayer = (Player) GD.Load<PackedScene>(UIDS.Player).Instantiate();
		newPlayer.Name = id.ToString();
		AddChild(newPlayer);
    	newPlayer.GlobalPosition = new Vector3(GD.RandRange(-10, 10), 1.0f, GD.RandRange(-10, 10));
	}
	private void RemovePlayer(int id) {
        GetNodeOrNull(id.ToString())?.QueueFree();
    }
}
