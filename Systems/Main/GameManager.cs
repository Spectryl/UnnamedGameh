using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node {
	public static GameManager Instance {get; private set;}
	public static PersistentFileManager PersistentFileManager {get; private set;}
	public static OptionsScreenManager OptionsScreenManager   {get; private set;}
	public static ServerManager        ServerManager          {get; private set;}
	public static ReadyManager         ReadyManager           {get; private set;}
	public static PlayerSyncManager    PlayerSyncManager      {get; private set;}
	public static RigidBodySyncManager RigidBodySyncManager   {get; private set;}
	public static Camera3D Camera;
	public static event Action StateChanged;
	public static bool IsOptionsScreenOpen;
    public static Action OptionScreenToggled;
	public static List<Player> PlayerList;
	public static List<PlayerData> PlayerDataList;
	public static string Username {
		get {return _Username;}
		set {_Username = value;}
	}
	public enum GameState {
		MAIN_MENU,
		LOBBY,
		GAME,
	}

	public GameState CurrentState {
		get {return _CurrentState;}
		set {
			_CurrentState = value;
			_CurrentScene?.QueueFree();
			_CurrentScene = value switch {
				GameState.MAIN_MENU => GD.Load<PackedScene>(UIDS.MainMenu).Instantiate(),
				GameState.LOBBY     => GD.Load<PackedScene>(UIDS.Lobby).Instantiate(),
				GameState.GAME      => GD.Load<PackedScene>(UIDS.Game).Instantiate()
			};
			AddChild(_CurrentScene);
			StateChanged?.Invoke();
		}
	}

	private static string _Username;
	private CanvasLayer _OptionsMenuLayer;
	private Node _CurrentScene;
	private GameState _CurrentState;
	private Tween _OptionsTween;


	public override void _Ready() {
		GD.Print("GameManager Ready");
GD.Print("CmdlineArgs: " + string.Join(", ", Godot.OS.GetCmdlineArgs()));
GD.Print("CmdlineUserArgs: " + string.Join(", ", Godot.OS.GetCmdlineUserArgs()));
		if (Instance != null) {
			GD.PrintErr("Error: Multiple GameManager instances detected!");
			QueueFree();
			return;
		}
		Instance = this;
		Camera = GetNode<Camera3D>("Camera");
		_OptionsMenuLayer = GetNode<CanvasLayer>("OptionsMenuCanvasLayer");
		PlayerList = new List<Player>();
		PlayerDataList = new List<PlayerData>();
		Username = GD.RandRange(0,1000).ToString();
		GetViewport().UseHdr2D = true;
		SetupPersisentFileManager();
		SetupServerManager();
		SetupReadyManager();
		SetupSyncManager();
		OptionScreenToggled += ToggleOptionsScreen;
		string[] args = Godot.OS.GetCmdlineArgs();
		if (args.Contains("--host")) {
			CurrentState = GameState.LOBBY;
			ServerManager.Instance.CreateServer();
		} else if (args.Contains("--client")) {
			CurrentState = GameState.LOBBY;
			NetworkManager.JoinServer();
		} else CurrentState = GameState.MAIN_MENU;
		

	}
    public override void _UnhandledInput(InputEvent @event) {
        if (Input.IsActionJustPressed("Pause")) ToggleOptionsScreen();
    }
	private void ToggleOptionsScreen() {
        _OptionsTween?.Kill();
        if (IsOptionsScreenOpen) {
			if (CurrentState == GameState.GAME) Input.MouseMode = Input.MouseModeEnum.Captured;
			OptionsScreenManager CurrentOptions = OptionsScreenManager;
			OptionsScreenManager = null;
			Tween closingTween = CurrentOptions.CreateTween();
			closingTween.TweenProperty(CurrentOptions, "scale", Vector2.Zero, 0.25f)
			.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
			closingTween.Parallel().TweenProperty(CurrentOptions, "modulate:a", 0.0f, 0.2f);
			closingTween.TweenCallback(Callable.From(() => CurrentOptions.QueueFree()));
        } else {
			Input.MouseMode = Input.MouseModeEnum.Visible;
			PackedScene scene = GD.Load<PackedScene>(UIDS.OptionsScreen);
			OptionsScreenManager = (OptionsScreenManager) scene.Instantiate();
			_OptionsMenuLayer.AddChild(OptionsScreenManager);
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
	private void SetupServerManager() {
		ServerManager = new ServerManager();
		AddChild(ServerManager, true);
	}
	private void SetupReadyManager() {
		ReadyManager = new ReadyManager();
		AddChild(ReadyManager, true);
	}
	private void SetupSyncManager() {
		PlayerSyncManager = new PlayerSyncManager();
		AddChild(PlayerSyncManager, true);
		RigidBodySyncManager = new RigidBodySyncManager();
		AddChild(RigidBodySyncManager, true);
	}
	
}
