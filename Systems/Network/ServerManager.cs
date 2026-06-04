using Godot;
using System;
public partial class ServerManager : Node {
    public static ServerManager Instance { get; private set; }
	public static Action<string, string> ChatMessageSent;
	public static Action PlayerListUpdated;
	public static Action GameStarted;

    public override void _Ready() {
        Instance = this;
        //GameManager.Instance.Multiplayer.PeerConnected += OnPeerConnected;
        GameManager.Instance.Multiplayer.PeerDisconnected += OnPeerDisconnected;
		GameManager.Instance.Multiplayer.ConnectedToServer += OnPeerConnectedToServer;
    }
	public static bool IsHost() {
		return GameManager.Instance.Multiplayer.GetUniqueId() == 1;
	}

	public static string GetUsernameById(int id) {
		foreach(PlayerData playerData in GameManager.PlayerDataList) if (playerData.Id == id) return playerData.Username;
		return "Unknown";
	}

	public void CreateServer() {
		NetworkManager.CreateServer();
		NetworkManager.CurrentPlayerCount = 0;
		AddPlayerToList(1, GameManager.Username);
		PlayerListUpdated?.Invoke();
	}

	public void LeaveServer() { GD.Print("Leaving Server"); Cleanup();}
	public void CloseServer() {
		if (!IsHost()) return;
		Rpc(nameof(RpcCloseServer));
		Cleanup();
	}
	public static void KickPlayer(int id) {
		if (!IsHost()) return;
		GD.Print($"Kicking Player: {id}");
		GameManager.Instance.Multiplayer.MultiplayerPeer.DisconnectPeer(id, force:true);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
	private void RpcCloseServer() => Cleanup();

	private void Cleanup() {
		if (GameManager.Instance.Multiplayer.MultiplayerPeer != null) {
			GameManager.Instance.Multiplayer.MultiplayerPeer.Close();
			GameManager.Instance.Multiplayer.MultiplayerPeer = null;
		}
		NetworkManager.CurrentPlayerCount = 0;
		GameManager.PlayerDataList.Clear();
	}

	private void OnPeerConnectedToServer() {
		GD.Print("Peer has connected to server");
		RpcId(1, nameof(RpcRegisterPlayer), GameManager.Username);
	}
	private void OnPeerDisconnected(long id) {
		if (!IsHost()) return;
		GameManager.PlayerDataList.RemoveAll(p => p.Id == (int)id);
        NetworkManager.CurrentPlayerCount = GameManager.PlayerDataList.Count;
        ReadyManager.PlayersLoaded.Remove((int)id);
		BroadcastPlayerList();
	}


	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
	private void RpcRegisterPlayer(string username) {
		if (!IsHost()) return;
		int senderId = GameManager.Instance.Multiplayer.GetRemoteSenderId();
		GD.Print($"Adding Player {senderId}: {username}");
		AddPlayerToList(senderId, username);
		BroadcastPlayerList();
	}
	private void AddPlayerToList(int id, string username) {
		GameManager.PlayerDataList.Add(new PlayerData(id, username));
		NetworkManager.CurrentPlayerCount++;
	}

	private void BroadcastPlayerList() {
		GD.Print("Receiving Player List");
		int[] ids = new int[NetworkManager.CurrentPlayerCount];
		string[] usernames = new string[NetworkManager.CurrentPlayerCount];

		for (int i = 0; i < NetworkManager.CurrentPlayerCount; i++) {
			ids[i] = GameManager.PlayerDataList[i].Id;
			usernames[i] = GameManager.PlayerDataList[i].Username;
		}
		Rpc(nameof(RpcSyncPlayerList), ids, usernames);
		PlayerListUpdated?.Invoke();
	}	

	[Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
    private void RpcSyncPlayerList(int[] ids, string[] usernames) {
        GD.Print("Syncing player list, count: ", ids.Length);
        NetworkManager.CurrentPlayerCount = 0;
		GameManager.PlayerDataList.Clear();
        for (int i = 0; i < ids.Length; i++) {
			AddPlayerToList(ids[i],usernames[i]);
        }
		PlayerListUpdated?.Invoke();
    }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = (int)NetworkManager.ChannelEnum.CHAT)]
	public void RpcSendChatMessage(string message) {
		if (!IsHost()) return;
		int senderId = GameManager.Instance.Multiplayer.GetRemoteSenderId();
		if (senderId == 0) senderId = Multiplayer.GetUniqueId();
		string senderName = GetUsernameById(senderId);
		Rpc(nameof(RpcReceiveChatMessage), senderName, message);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = (int)NetworkManager.ChannelEnum.CHAT)]
	private void RpcReceiveChatMessage(string sender, string message) {
		ChatMessageSent?.Invoke(sender, message);
	}
}

