using Godot;
using System;
using System.Collections.Generic;

public partial class ReadyManager : Node {
    public static ReadyManager Instance { get; private set; }
    private List<int> _playersLoaded = new List<int>();

    public static Action LevelLoaded;

    public override void _Ready() {
        Instance = this;
        LevelLoaded += OnLevelReady;
    }

    private void OnLevelReady() {
        RpcId(1, nameof(RpcPlayerLoaded));
    }

    public void StartLoadingGame() {
        if (!ServerManager.Instance.IsHost()) return;
        Rpc(nameof(RpcLoadGame));
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
    private void RpcLoadGame() {
        GameManager.Instance.CurrentState = GameManager.GameState.GAME;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
    private void RpcPlayerLoaded() {
		int id = GameManager.Instance.Multiplayer.GetRemoteSenderId();
        if (!ServerManager.Instance.IsHost()) return;
        if (!_playersLoaded.Contains(id)) _playersLoaded.Add(id);
        GD.Print($"Players loaded: {_playersLoaded.Count}/{NetworkManager.CurrentPlayerCount}");
        if (_playersLoaded.Count >= NetworkManager.CurrentPlayerCount) {
            _playersLoaded.Clear();
            Rpc(nameof(RpcStartGame));
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
    private void RpcStartGame() {
        GD.Print("All players loaded, starting game!");
    }
}