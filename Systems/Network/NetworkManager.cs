using Godot;
using System;

public static class NetworkManager {
	public static string IpAddress = "127.0.0.1";
	public static int Port = 6767;
	public static int CurrentPlayerCount = 0;
	public static int MaxPlayerCount = 4;
	public static ENetMultiplayerPeer EnetPeer = new ENetMultiplayerPeer();
	public enum ChannelEnum {DEFAULT = 0,CHAT,}
	public static void CreateServer() {
		EnetPeer = new ENetMultiplayerPeer();
		GD.Print("Creating Enet Host");
		Error error = EnetPeer.CreateServer(NetworkManager.Port);
		
		if (error != Error.Ok) {
			GD.PrintErr("Failed to create server: ", error);
			return;
		}

		GameManager.Instance.Multiplayer.MultiplayerPeer = EnetPeer;
		GD.Print("Sucessfully Created Enet Host");
	}

	public static void JoinServer() {
		EnetPeer = new ENetMultiplayerPeer();
		GD.Print("Creating Enet Client");
		Error error = EnetPeer.CreateClient(NetworkManager.IpAddress, NetworkManager.Port);

		if (error != Error.Ok) {
			GD.PrintErr("Failed to join server: ", error);
			return;
		}
		
		GameManager.Instance.Multiplayer.MultiplayerPeer = EnetPeer;
		GD.Print("Sucessfully Created Enet Client");
	}
}
