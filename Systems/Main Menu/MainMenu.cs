using Godot;
using System;

public partial class MainMenu : Node3D {
	public enum SubMenu {
		TITLE_SCREEN,
		JOIN,
		LOBBY,
		OPTIONS,
		CREDITS,
		EXTRAS
	}
	public SubMenu CurrentState {
		get {return _CurrentState;}
		set {
			_CurrentMenu?.QueueFree();
			_CurrentMenu = value switch {
				SubMenu.TITLE_SCREEN => (MainMenuSubMenu) GD.Load<PackedScene>(UIDS.TitleScreen).Instantiate(),
				SubMenu.JOIN         => (MainMenuSubMenu) GD.Load<PackedScene>(UIDS.JoinMenu).Instantiate(),
				SubMenu.LOBBY        => (MainMenuSubMenu) GD.Load<PackedScene>(UIDS.Lobby).Instantiate(),
			};
			_CurrentMenu.MainMenu = this;
			_MainMenuCanvasLayer.AddChild(_CurrentMenu);
		}
	}
	private CanvasLayer _MainMenuCanvasLayer;
	private SubMenu _CurrentState;
	private MainMenuSubMenu _CurrentMenu;
	public override void _Ready() {
		_MainMenuCanvasLayer = GetNode<CanvasLayer>("UI");
		CurrentState = SubMenu.TITLE_SCREEN;

	}
}
