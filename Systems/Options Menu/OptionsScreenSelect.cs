using Godot;
using System;
using System.Diagnostics;

public partial class OptionsScreenSelect : OptionsSubMenu {
    private Button GameButton;
    private Button AudioButton;
    private Button VideoButton;
    private Button ControlsButton;
    private Button MainMenuButton;
    private Button NewRunButton;
    public override void _Ready() {
        base._Ready();
        OptionsScreenManager.ChangeBackButtonVisibility(false);
        GameButton     = GetNode<Button>("MarginContainer/VBoxContainer/Game/GameButton");
        AudioButton    = GetNode<Button>("MarginContainer/VBoxContainer/Audio/AudioButton");
        VideoButton    = GetNode<Button>("MarginContainer/VBoxContainer/Video/VideoButton");
        ControlsButton = GetNode<Button>("MarginContainer/VBoxContainer/Controls/ControlsButton");
        MainMenuButton = GetNode<Button>("MarginContainer/VBoxContainer/MainMenu/MainMenuButton");
        NewRunButton   = GetNode<Button>("MarginContainer/VBoxContainer/NewRun/NewRunButton");
        GameButton.Pressed     += OptionsScreenManager.SwitchToGameScreen;
        AudioButton.Pressed    += OptionsScreenManager.SwitchToAudioScreen;
        VideoButton.Pressed    += OptionsScreenManager.SwitchToVideoScreen;
        MainMenuButton.Pressed += ReturnToTitleScreen;
    }

    private void ReturnToTitleScreen() {
        //GameManager.Instance.HideOptionsScreen();
        GameManager.Instance.ChangeState(GameManager.GameState.MAIN_MENU);
    }

}
