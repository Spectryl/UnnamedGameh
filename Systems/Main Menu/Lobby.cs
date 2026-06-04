using Godot;
using System;

public partial class Lobby : Control {
	private Label _PlayerCount;
	private Button _StartButton;
	private Button _LeaveButton;
	public override void _Ready() {
		_PlayerCount = GetNode<Label>("PlayerCount");
		_StartButton = GetNode<Button>("StartButton");
		_LeaveButton = GetNode<Button>("LeaveButton");
		ServerManager.PlayerListUpdated += UpdatePlayerCount;
		ServerManager.PlayerListUpdated += UpdateStartButton;
		_StartButton.Pressed += OnStartButtonPressed;
		_LeaveButton.Pressed += OnLeaveButtonPressed;
		
	}

    public override void _ExitTree() {
        ServerManager.PlayerListUpdated -= UpdatePlayerCount;
		ServerManager.PlayerListUpdated -= UpdateStartButton;
    }

	private void UpdatePlayerCount() {
		_PlayerCount.Text = $"Player List ({NetworkManager.CurrentPlayerCount})";
	}

	private void UpdateStartButton() {
		_StartButton.Disabled = !ServerManager.IsHost();
	}	

	private void OnStartButtonPressed() {
		ReadyManager.Instance.StartLoadingGame();
	}

	private void OnLeaveButtonPressed() {
		if (ServerManager.IsHost()) {
			ServerManager.Instance.CloseServer();
		} else {
			ServerManager.Instance.LeaveServer();
		}
		GameManager.Instance.CurrentState = GameManager.GameState.MAIN_MENU;
	}
}
