using Godot;
using System;

public partial class GameManager : Node {
	public static GameManager Instance {get; private set;}
	public static PersistentFileManager PersistentFileManager {get; private set;}
	public static Camera3D Camera;
	public static event Action StateChanged;
	public enum GameState {
		MAIN_MENU,
		GAME,
		SETTINGS,
	}

	public GameState CurrentState {
		get {return _CurrentState;}
		set {
			_CurrentState = value;
			switch(CurrentState) {
				//  CurrentScene?.QueueFree();
				// _CurrentScene = ???;
				case GameState.MAIN_MENU:
					break;
				case GameState.GAME:
					break;
				case GameState.SETTINGS:
					break;
			}
			StateChanged?.Invoke();
		}
	}

	private Node _CurrentScene;
	private GameState _CurrentState;


	public override void _Ready() {
		if (Instance != null) {
			GD.PrintErr("Error: Multiple GameManager instances detected!");
			QueueFree();
			return;
		}
		Instance = this;
		Camera = (Camera3D) GetNode<Camera3D>("Camera"); 
		GetViewport().UseHdr2D = true;
		ChangeState(GameState.MAIN_MENU);
		SetupPersisentFileManager();
	}
	public void ChangeState(GameState newState) => CurrentState = newState;
	private void SetupPersisentFileManager() => PersistentFileManager = new PersistentFileManager();
	
}
