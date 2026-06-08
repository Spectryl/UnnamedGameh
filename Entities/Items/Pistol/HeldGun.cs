using Godot;

public partial class HeldGun : HeldItem {
    private GunData _Data;
    private RayCast3D _ShootRay;
    private bool _IsReloading = false;

    public override void _Ready() {
        _ShootRay = GetNode<RayCast3D>("ShootRay");
    }

    public override void Setup(ItemData data) {
        _Data = data as GunData;
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("LeftClick") && !_IsReloading)  TryShoot();
        if (@event.IsActionPressed("Interact2") && !_IsReloading) StartReload();
    }

    private void TryShoot() {
        if (_Data.CurrentAmmo <= 0) {
            StartReload();
            return;
        }
        if (!_ShootRay.IsColliding()) return;
        _Data.CurrentAmmo--;
        GD.Print($"Shot! Ammo remaining: {_Data.CurrentAmmo}");

        if (_ShootRay.GetCollider() is Player player) {
            player.Health.Health -= _Data.Damage;
        }
    }

    private async void StartReload() {
        if (_Data.CurrentAmmo == _Data.MaxAmmo) return;
        _IsReloading = true;
        GD.Print("Reloading...");
        await ToSignal(GetTree().CreateTimer(2.0f), Timer.SignalName.Timeout);
        _Data.CurrentAmmo = _Data.MaxAmmo;
        _IsReloading = false;
        GD.Print("Reloaded!");
    }
}