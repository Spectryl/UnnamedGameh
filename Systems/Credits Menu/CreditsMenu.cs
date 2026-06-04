using Godot;
using System;

public partial class CreditsMenu : MainMenuSubMenu {
	private Button _BackButton;
    public override void _Ready() {
        _BackButton = GetNode<Button>("MarginContainer/BackButton");
		_BackButton.Pressed += OnBackButtonPressed;
    }


	public void OnBackButtonPressed() => MainMenu.CurrentState = MainMenu.SubMenu.TITLE_SCREEN;

}
