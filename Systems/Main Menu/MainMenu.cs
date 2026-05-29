using Godot;
using System;

public partial class MainMenu : Node3D {
	public enum MainMenuMenu {
		TITLE_SCREEN,
		OPTIONS,
		CREDITS,
		EXTRAS
	}
	public MainMenuMenu CurrentState {
		get {return _CurrentState;}
		set {
			_CurrentMenu?.QueueFree();
			_CurrentMenu = value switch {
				MainMenuMenu.TITLE_SCREEN => (Control) GD.Load<PackedScene>(UIDS.TitleScreen).Instantiate(),
			};
			MainMenuCanvasLayer.AddChild(_CurrentMenu);
		}
	}
	private CanvasLayer MainMenuCanvasLayer;
	private MainMenuMenu _CurrentState;
	private Control _CurrentMenu;
	public override void _Ready() {
		MainMenuCanvasLayer = GetNode<CanvasLayer>("UI");
		CurrentState = MainMenuMenu.TITLE_SCREEN;

	}
}
