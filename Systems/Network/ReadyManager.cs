using Godot;
using System;
using System.Collections.Generic;

public partial class ReadyManager : Node {
    public static ReadyManager Instance { get; private set; }
    public static List<int> PlayersLoaded = new List<int>();
    public static Action LevelLoaded;
    private Timer _KickTimer;
    private int KICK_TIME = 60;
    public override void _Ready() {
        Instance = this;
        LevelLoaded += OnLevelReady;
        _KickTimer = new Timer();
        _KickTimer.WaitTime = KICK_TIME;
        _KickTimer.OneShot = true;
        _KickTimer.Timeout += OnKickTimerTimeout;
        AddChild(_KickTimer);
        
    }

    public void StartLoadingGame() {
        if (!ServerManager.IsHost()) return;
        PlayersLoaded.Clear();
        _KickTimer.Start();
        Rpc(nameof(RpcLoadGame));
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
    private void RpcLoadGame() {
        GameManager.Instance.CurrentState = GameManager.GameState.GAME;
    }

    private void OnLevelReady() {
        RpcId(1, nameof(RpcPlayerLoaded));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
    private void RpcPlayerLoaded() {
		int id = GameManager.Instance.Multiplayer.GetRemoteSenderId();
        if (!ServerManager.IsHost()) return;
        if (!PlayersLoaded.Contains(id)) PlayersLoaded.Add(id);
        GD.Print($"Players loaded: {PlayersLoaded.Count}/{NetworkManager.CurrentPlayerCount}");
        if (PlayersLoaded.Count >= NetworkManager.CurrentPlayerCount) {
            PlayersLoaded.Clear();
            Rpc(nameof(RpcStartGame));
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
    private void RpcStartGame() {
        GD.Print("All players loaded, starting game!");
        PlayersLoaded.Clear();
        _KickTimer.Stop();
        ServerManager.GameStarted?.Invoke();

    }

    private void OnKickTimerTimeout() {
        List<int> kickList = new List<int>();
        foreach(PlayerData playerData in GameManager.PlayerDataList) {
            int id = playerData.Id;
            if(!PlayersLoaded.Contains(id)) kickList.Add(id);
        }

        foreach(int id in kickList) {
            PlayersLoaded.Remove(id);
            ServerManager.KickPlayer(id);
        }
    }
}