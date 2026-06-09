using Godot;
using System;

public partial class ItemData {
	public string Name = "ITEM";
	public Texture2D Icon = GetIcon(new Vector2I(0,0));
	public string Description = "";
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
	public static string GetHeldScene(int type) => GetHeldScene((ItemType)type);

	public static string GetPickupScene(ItemType type) => type switch {
		ItemType.Pistol => UIDS.PickupPistol,
		ItemType.Apple  => UIDS.PickupApple,
		_               => null
	};
	public static string GetPickupScene(int type) => GetPickupScene((ItemType)type);
	
	public static ItemData CreateInstance(int itemType, int uses, int maxUses) {
        ItemData item = (ItemType)itemType switch {
            ItemType.Pistol => new PistolData(),
            ItemType.Apple  => new AppleData(),
            _               => new ItemData()
        };

        item.Uses = uses;
        item.MaxUses = maxUses;
        return item;
    }
}
