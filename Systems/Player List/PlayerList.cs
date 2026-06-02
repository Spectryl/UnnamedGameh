using Godot;
using System;

public partial class PlayerList : Control {
	private VBoxContainer _PlayerListContainer;
	public override void _Ready() {
		_PlayerListContainer = GetNode<VBoxContainer>("MarginContainer/PanelContainer/PlayerListContainer");
		foreach (Node child in _PlayerListContainer.GetChildren()) child.QueueFree();
		ServerManager.PlayerListUpdated += RefreshList;
		RefreshList();
	}

    public override void _ExitTree() {
        ServerManager.PlayerListUpdated -= RefreshList;
    }
    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
		//RefreshList();
    }
	public void RefreshList() {
		foreach (Node child in _PlayerListContainer.GetChildren()) {
			child.QueueFree();
		}
		for (int i = 0; i < NetworkManager.CurrentPlayerCount; i++) {
			AddPlayer(ServerManager.PlayerDataList[i].Id);
		}
	}
	public void AddPlayer(int id) {
		PlayerEntry newEntry = new PlayerEntry();
		_PlayerListContainer.AddChild(newEntry);
		string username = ServerManager.GetUsernameById(id);
		newEntry.Setup(id, username);
	}
	public void RemovePlayer(int id) {
		foreach (Node child in _PlayerListContainer.GetChildren()) {
			if (child is PlayerEntry entry && entry.Id == id) {
				entry.QueueFree();
				return;
			}
		}
	}
	

}
