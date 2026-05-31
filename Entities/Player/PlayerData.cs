using Godot;
using System;

public partial class PlayerData {
	public int Id;
	public string Username;

	public PlayerData(int id, string username) {
		Id = id;
		Username = username;
	}
}
