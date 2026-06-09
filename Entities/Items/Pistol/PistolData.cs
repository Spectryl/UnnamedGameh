public class PistolData : ItemData {
	public int Damage = 10;
	public int MaxAmmo = 30;
	public int CurrentAmmo;
	public float FireRate = 0.1f;
	public override string HeldScene => UIDS.HeldPistol;
    public override string PickupScene => UIDS.PickupPistol;
	public PistolData() {
		Name = "Pistol";
		CurrentAmmo = MaxAmmo;
	}
}