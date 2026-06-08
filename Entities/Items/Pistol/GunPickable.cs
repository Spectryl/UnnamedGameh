using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class GunPickable : Pickable {
    public override void _Ready() {
		Data = new GunData();
    }


}

public class GunData : ItemData {
	public int Damage = 10;
	public int MaxAmmo = 30;
	public int CurrentAmmo;
	public float FireRate = 0.1f;
	public override string HeldScene => UIDS.HeldGun;
	public GunData() {
		Name = "Gun";
		CurrentAmmo = MaxAmmo;
	}
}