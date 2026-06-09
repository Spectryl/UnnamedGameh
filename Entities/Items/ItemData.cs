using Godot;
using System;

public partial class ItemData {
	public string Name = "ITEM";
	public Texture2D Icon = GetIcon(new Vector2I(0,0));
	public string Description = "";
	public virtual string HeldScene   => null;
	public virtual string PickupScene => null;
	private static Texture2D _atlas;
	private static Texture2D Atlas => _atlas ??= GD.Load<Texture2D>("res://Assets/ItemAtlas.png");
	protected static AtlasTexture GetIcon(Vector2I gridPosition) {
		AtlasTexture icon = new AtlasTexture();
		icon.Atlas  = Atlas;
		icon.Region = gridPosition.GetIconRegionFromVectorID();
		return icon;
	}
}
