using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
public partial class ServerManager : Node {
    public static ServerManager Instance { get; private set; }
	public static PlayerData[] PlayerDataList = new PlayerData[NetworkManager.MaxPlayerCount];
	public static string Username = GD.RandRange(0,1000).ToString();
	public static Action<String, String> ChatMessageSent;

    public override void _Ready() {
        Instance = this;
        //GameManager.Instance.Multiplayer.PeerConnected += OnPeerConnected;
        //GameManager.Instance.Multiplayer.PeerDisconnected += OnPeerDisconnected;
		GameManager.Instance.Multiplayer.ConnectedToServer += OnPeerConnectedToServer;
    }

	public void CreateServer() {
		NetworkManager.CreateServer();
		NetworkManager.CurrentPlayerCount = 0;
		AddPlayerToList(1, Username);
	}
	private void OnPeerConnectedToServer() {
		GD.Print("Peer has connected to server");
		RpcId(1, nameof(RpcRegisterPlayer), Username);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
	private void RpcRegisterPlayer(String username) {
		if (!IsHost()) return;
		int senderId = GameManager.Instance.Multiplayer.GetRemoteSenderId();
		GD.Print($"Adding Player {senderId}: {username}");
		AddPlayerToList(senderId, username);
		BroadcastPlayerList();
	}
	private void AddPlayerToList(int id, string username) {
		PlayerDataList[NetworkManager.CurrentPlayerCount++] = new PlayerData((int)id, username);
	}
	private void BroadcastPlayerList() {
		GD.Print("Receiving Player List");
		int[] ids = new int[NetworkManager.CurrentPlayerCount];
		string[] usernames = new string[NetworkManager.CurrentPlayerCount];

		for (int i = 0; i < NetworkManager.CurrentPlayerCount; i++) {
			ids[i] = PlayerDataList[i].Id;
			usernames[i] = PlayerDataList[i].Username;
		}
		Rpc(nameof(RpcSyncPlayerList), ids, usernames);
	}	

	[Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
    private void RpcSyncPlayerList(int[] ids, string[] usernames) {
        GD.Print("Syncing player list, count: ", ids.Length);
        NetworkManager.CurrentPlayerCount = ids.Length;
        for (int i = 0; i < ids.Length; i++) {
            PlayerDataList[i] = new PlayerData(ids[i], usernames[i]);
        }
    }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = (int)NetworkManager.ChannelEnum.CHAT)]
	public void RpcSendChatMessage(string message) {
		if (!IsHost()) return;
		int senderId = GameManager.Instance.Multiplayer.GetRemoteSenderId();
		string senderName = GetUsernameById(senderId);
		Rpc(nameof(RpcReceiveChatMessage), senderName, message);
	}
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = (int)NetworkManager.ChannelEnum.CHAT)]
	private void RpcReceiveChatMessage(string sender, string message) {
		ChatMessageSent?.Invoke(sender, message);
	}

	public bool IsHost() {
		return GameManager.Instance.Multiplayer.GetUniqueId() == 1;
	}
	private string GetUsernameById(int id) {
		for (int i = 0; i < NetworkManager.CurrentPlayerCount; i++) {
			if (PlayerDataList[i].Id == id) return PlayerDataList[i].Username;
		}
		return "Unknown";
	}
}

