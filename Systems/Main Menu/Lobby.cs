using Godot;
using System;

public partial class Lobby : MainMenuSubMenu {
	private Label _PlayerCount;
	public override void _Ready() {
		_PlayerCount = GetNode<Label>("PlayerCount");
		ServerManager.PlayerListUpdated += UpdatePlayerCount;
	}

	private void UpdatePlayerCount() {
		_PlayerCount.Text = $"Player List ({NetworkManager.CurrentPlayerCount})";
	}
}
