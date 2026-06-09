using Godot;
using System;

public partial class ItemData {
	public string Name = "ITEM";
	public Texture2D Icon = GetIcon(new Vector2I(0,0));
	public string Description = "";
	public virtual string HeldScene   => null;
	public virtual string PickupScene => null;
	public virtual int Uses    { get; set; }
    public virtual int MaxUses { get; set; }
	private static Texture2D _atlas;
	private static Texture2D Atlas => _atlas ??= GD.Load<Texture2D>("res://Assets/ItemAtlas.png");
	public virtual ItemType Type => ItemType.None;
	protected static AtlasTexture GetIcon(Vector2I gridPosition) {
		AtlasTexture icon = new AtlasTexture();
		icon.Atlas  = Atlas;
		icon.Region = gridPosition.GetIconRegionFromVectorID();
		return icon;
	}

	public enum ItemType {
		None = 0,
		Pistol,
		Apple
	}

	public static string GetHeldScene(ItemType type) => type switch {
		ItemType.Pistol => UIDS.HeldPistol,
		ItemType.Apple  => UIDS.HeldApple,
		_               => null
	};
}
