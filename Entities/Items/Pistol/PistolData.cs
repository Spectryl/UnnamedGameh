public class PistolData : ItemData {
	public override int Uses    { get => CurrentAmmo;  set => CurrentAmmo = value; }
    public override int MaxUses { get => MaxAmmo;      set => MaxAmmo = value; }
	public int Damage = 10;
	public int MaxAmmo = 30;
	public int CurrentAmmo;
	public float FireRate = 0.1f;
	public override string HeldScene => UIDS.HeldPistol;
    public override string PickupScene => UIDS.PickupPistol;
	public override ItemType Type => ItemType.Pistol;
	public PistolData() {
		Name = "Pistol";
		CurrentAmmo = MaxAmmo;
		Icon = GetIcon(new Godot.Vector2I(1,0));
	}
}