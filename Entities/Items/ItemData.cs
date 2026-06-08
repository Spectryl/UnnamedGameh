using Godot;
using System;

public partial class ItemData {
	public string Name = "ITEM";
	public Texture2D Icon = null;
	public string Description = "";
	public virtual string HeldScene => null;
}
