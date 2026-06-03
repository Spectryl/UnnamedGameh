using Godot;
using System;

public partial class Level : Node {
    public override void _Ready() {
		ServerManager.GameStarted += CreateAllPlayers;
        ReadyManager.LevelLoaded?.Invoke();
		
    }

	private void CreateAllPlayers() {
		foreach(PlayerData playerData in GameManager.PlayerDataList) {
			CreatePlayer(playerData.Id);
		}
	}
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
