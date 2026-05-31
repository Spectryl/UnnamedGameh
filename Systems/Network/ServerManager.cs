using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
public partial class ServerManager : Node {
    public static ServerManager Instance { get; private set; }
	public static PlayerData[] PlayerDataList = new PlayerData[NetworkManager.MaxPlayerCount];
    public static event Action<int> OnPlayerConnected;
    public static event Action<int> OnPlayerDisconnected;

    public override void _Ready() {
        Instance = this;
        GameManager.Instance.Multiplayer.PeerConnected += OnPeerConnected;
        GameManager.Instance.Multiplayer.PeerDisconnected += OnPeerDisconnected;
		GameManager.Instance.Multiplayer.ConnectedToServer += OnPeerConnectedToServer;

		
    }

	public void CreateServer() {
		NetworkManager.CreateServer();
		NetworkManager.CurrentPlayerCount = 0;
		PlayerDataList[NetworkManager.CurrentPlayerCount++] = new PlayerData(1, "");
		OnPeerConnected(1);
	}

	private void OnPeerConnected(long id) {
		GD.Print("A Peer has connected: ", id);
		PlayerDataList[NetworkManager.CurrentPlayerCount++] = new PlayerData((int)id, "");
		int[] playerList = new int[NetworkManager.CurrentPlayerCount];
		for (int i = 0; i < NetworkManager.CurrentPlayerCount; i++) {
			playerList[i] = PlayerDataList[i].Id;
		}
		if (IsHost()) RpcId(id, nameof(RpcSendPlayerList), playerList);
		OnPlayerConnected?.Invoke((int)id);
	}
    private void OnPeerDisconnected(long id) {
        GD.Print("A Peer has disconnected: ", id);
        OnPlayerDisconnected?.Invoke((int)id);
    }
	private void OnPeerConnectedToServer() {
		GD.Print("Peer has connected to server");
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
	private void RpcSendPlayerList(int[] playerList) {
		GD.Print("Receiving Player List");
		for (int i = 0; i < playerList.Length; i++) {
			PlayerDataList[i] = new PlayerData(playerList[i], "");
		}
	}

	private bool IsHost() {
		return GameManager.Instance.Multiplayer.GetUniqueId() == 1;
	} 


}

