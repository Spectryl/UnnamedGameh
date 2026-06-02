using Godot;
using System;

public static class NetworkManager {
	public static string IP_ADDRESS = "127.0.0.1";
	public static int PORT = 6767;
	public static ENetMultiplayerPeer EnetPeer = new ENetMultiplayerPeer();
	public enum NetworkState {HOST,CLIENT}
	public static NetworkState InternalNetworkState;
	public static int MaxPlayerCount = 4;
	public static int CurrentPlayerCount = 0;
	public enum ChannelEnum {
		DEFAULT = 0,
		CHAT,
	}
	public static void CreateServer() {
		EnetPeer = new ENetMultiplayerPeer();
		Error error = EnetPeer.CreateServer(NetworkManager.PORT);
		GD.Print("Creating Enet Host");
		if (error != Error.Ok) {
			GD.PrintErr("Failed to create server: ", error);
			return;
		}
		GD.Print("Sucessfully Created Enet Host");
		InternalNetworkState = NetworkState.HOST;
		GameManager.Instance.Multiplayer.MultiplayerPeer = EnetPeer;
	}

	public static void JoinServer() {
		EnetPeer = new ENetMultiplayerPeer();
		Error error = EnetPeer.CreateClient(NetworkManager.IP_ADDRESS, NetworkManager.PORT);
		GD.Print("Creating Enet Client");
		if (error != Error.Ok) {
			GD.PrintErr("Failed to join server: ", error);
			return;
		}
		GD.Print("Sucessfully Created Enet Client");
		InternalNetworkState = NetworkState.CLIENT;
		GameManager.Instance.Multiplayer.MultiplayerPeer = EnetPeer;
	}
}
