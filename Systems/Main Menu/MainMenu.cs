using Godot;
using System;

public partial class MainMenu : Node3D {
	public enum SubMenu {
		TITLE_SCREEN,
		OPTIONS,
		CREDITS,
		EXTRAS
	}
	public SubMenu CurrentState {
		get {return _CurrentState;}
		set {
			_CurrentMenu?.QueueFree();
			_CurrentMenu = value switch {
				SubMenu.TITLE_SCREEN => (Control) GD.Load<PackedScene>(UIDS.TitleScreen).Instantiate(),
			};
			MainMenuCanvasLayer.AddChild(_CurrentMenu);
		}
	}
	private CanvasLayer MainMenuCanvasLayer;
	private SubMenu _CurrentState;
	private Control _CurrentMenu;
	public override void _Ready() {
		MainMenuCanvasLayer = GetNode<CanvasLayer>("UI");
		CurrentState = SubMenu.TITLE_SCREEN;

	}
}
