using Godot;
using System;

public partial class TitleScreen : Control {


	private MarginContainer _ButtonContainer;
	private Button _PlayButton;
	private Button _OptionsButton;
	private Button _CreditsButton;
	private Button _QuitButton;
	private Label _VersionLabel;
	

    public override void _Ready() {
        _PlayButton    = GetNode<Button>("MarginContainer/VBoxContainer/Play/PlayButton");
		_OptionsButton = GetNode<Button>("MarginContainer/VBoxContainer/Options/OptionsButton");
		_CreditsButton = GetNode<Button>("MarginContainer/VBoxContainer/Credits/CreditsButton");
		_QuitButton    = GetNode<Button>("MarginContainer/VBoxContainer/Quit/QuitButton");
		_VersionLabel  = GetNode<Label>("Version/VersionLabel");

		_PlayButton.Pressed += Play;
		_OptionsButton.Pressed += Options;
		_CreditsButton.Pressed += Credits;
		_QuitButton.Pressed += Quit;
		SetupVersionText();
    }

	private void Play() => GameManager.Instance.CurrentState = GameManager.GameState.GAME; 
	private void Options() => GameManager.OptionScreenToggled.Invoke();
	private void Credits() {}
	private void Quit() => GetTree().Quit();

	private void SetupVersionText() => _VersionLabel.Text = $"{ProjectSettings.GetSetting("application/config/version")}";
}
