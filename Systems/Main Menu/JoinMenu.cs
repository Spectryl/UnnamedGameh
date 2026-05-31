using Godot;
using System;

public partial class JoinMenu : MainMenuSubMenu {

	private Button _HostButton;
	private Button _JoinButton;
	private Button _BackButton;
	private LineEdit _IPEntry;
	private LineEdit _PortEntry;
	public override void _Ready() {
		_HostButton = GetNode<Button>("MarginContainer/PanelContainer/VBoxContainer/HostButton");
		_JoinButton = GetNode<Button>("MarginContainer/PanelContainer/VBoxContainer/JoinButton");
		_BackButton = GetNode<Button>("MarginContainer2/BackButton");
		_IPEntry    = GetNode<LineEdit>("MarginContainer/PanelContainer/VBoxContainer/IPEntry");
		_PortEntry  = GetNode<LineEdit>("MarginContainer/PanelContainer/VBoxContainer/PortEntry");
		
		_HostButton.Pressed += OnHostButtonPressed;
		_JoinButton.Pressed += OnJoinButtonPressed;
		_BackButton.Pressed += OnBackButtonPressed;
	}

	public void OnHostButtonPressed() {
		GameManager.Instance.CurrentState = GameManager.GameState.GAME;
		ServerManager.Instance.CreateServer();
		
	}
	
	public void OnJoinButtonPressed() {
		GameManager.Instance.CurrentState = GameManager.GameState.GAME;
		NetworkManager.JoinServer();
	}
	public void OnBackButtonPressed() => MainMenu.CurrentState = MainMenu.SubMenu.TITLE_SCREEN;
}
