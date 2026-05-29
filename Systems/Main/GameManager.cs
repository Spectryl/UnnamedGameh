using Godot;
using System;

public partial class GameManager : Node {
	public static GameManager Instance {get; private set;}
	public static PersistentFileManager PersistentFileManager {get; private set;}
	public static OptionsScreenManager OptionsScreenManager  {get; private set;}
	public static Camera3D Camera;
	public static event Action StateChanged;
	public static bool IsOptionsScreenOpen;
    public static Action OptionScreenToggled;
	public enum GameState {
		MAIN_MENU,
		GAME,
	}

	public GameState CurrentState {
		get {return _CurrentState;}
		set {
			_CurrentState = value;
			_CurrentScene?.QueueFree();
			_CurrentScene = value switch {
				GameState.MAIN_MENU => GD.Load<PackedScene>(UIDS.MainMenu).Instantiate(),
				GameState.GAME      => GD.Load<PackedScene>(UIDS.Game).Instantiate()
			};
			AddChild(_CurrentScene);
			StateChanged?.Invoke();
		}
	}

	private CanvasLayer OptionsMenuLayer;
	private Node _CurrentScene;
	private GameState _CurrentState;
	private Tween _OptionsTween;


	public override void _Ready() {
		if (Instance != null) {
			GD.PrintErr("Error: Multiple GameManager instances detected!");
			QueueFree();
			return;
		}
		Instance = this;
		Camera = (Camera3D) GetNode<Camera3D>("Camera");
		OptionsMenuLayer = GetNode<CanvasLayer>("OptionsMenuCanvasLayer");
		GetViewport().UseHdr2D = true;
		SetupPersisentFileManager();
		CurrentState = GameState.MAIN_MENU;
		OptionScreenToggled += ToggleOptionsScreen;
	}
    public override void _UnhandledInput(InputEvent @event) {
        if (Input.IsActionJustPressed("Pause")) ToggleOptionsScreen();
    }
	/// <summary>
	/// Toggles The Option Screen on and off based on player input, handled by GameManager so Pausing is Always Available
	/// </summary>
	private void ToggleOptionsScreen() {
        _OptionsTween?.Kill();
        if (IsOptionsScreenOpen) {
			OptionsScreenManager CurrentOptions = OptionsScreenManager;
			OptionsScreenManager = null;
			Tween closingTween = CurrentOptions.CreateTween();
			closingTween.TweenProperty(CurrentOptions, "scale", Vector2.Zero, 0.25f)
				.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
			closingTween.Parallel().TweenProperty(CurrentOptions, "modulate:a", 0.0f, 0.2f);
			closingTween.TweenCallback(Callable.From(() => CurrentOptions.QueueFree()));
        } else {
			PackedScene scene = GD.Load<PackedScene>(UIDS.OptionsScreen);
			OptionsScreenManager = (OptionsScreenManager) scene.Instantiate();
			OptionsMenuLayer.AddChild(OptionsScreenManager);
			OptionsScreenManager.PivotOffset = OptionsScreenManager.Size / 2;
			OptionsScreenManager.Scale = Vector2.Zero;
			OptionsScreenManager.Modulate = new Color(1, 1, 1, 0);

			_OptionsTween = OptionsScreenManager.CreateTween();
			_OptionsTween.TweenProperty(OptionsScreenManager, "scale", Vector2.One, 0.4f)
			.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
			_OptionsTween.Parallel().TweenProperty(OptionsScreenManager, "modulate:a", 1.0f, 0.2f);
		}
		IsOptionsScreenOpen = !IsOptionsScreenOpen;
		PersistentFileManager.Settings.Save(PersistentFileManager.SettingsPath);
    }
	private void SetupPersisentFileManager() => PersistentFileManager = new PersistentFileManager();
	
}
