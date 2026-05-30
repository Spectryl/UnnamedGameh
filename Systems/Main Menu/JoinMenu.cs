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
		
		_BackButton.Pressed += OnBackButtonPressed;
	}

	public void OnHostButtonPressed() {
		
	}
	
	public void OnJoinButtonPressed() {
		
	}
	public void OnBackButtonPressed() => MainMenu.CurrentState = MainMenu.SubMenu.TITLE_SCREEN;
}
