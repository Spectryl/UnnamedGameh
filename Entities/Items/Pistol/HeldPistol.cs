using Godot;

public partial class HeldPistol : HeldItem {
    private PistolData _Data;
    private RayCast3D _ShootRay;
    private bool _IsReloading = false;

    public override void _Ready() {
        _ShootRay = GetNode<RayCast3D>("ShootRay");
    }

    public override void Setup(ItemData data) {
        _Data = data as PistolData;
    }
    public override void SetupRemote(ItemData data) {
        base.SetupRemote(data);
        _Data = data as PistolData;
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("LeftClick") && !_IsReloading) PerformAction(HeldItemAction.PrimaryUse);
        if (@event.IsActionPressed("Interact2") && !_IsReloading) PerformAction(HeldItemAction.SecondaryInteractUse);
    }

    public override void OnPrimaryUse() {
        GD.Print($"OnPrimaryUse called, authority: {IsMultiplayerAuthority()}, ammo: {_Data?.CurrentAmmo}");
        if (_Data.CurrentAmmo <= 0) {
            StartReload();
            return;
        }
        _Data.CurrentAmmo--;
        GetPlayer().Inventory.NotifySlotChanged();

        if (IsMultiplayerAuthority()) {
            GD.Print($"Raycast colliding: {_ShootRay.IsColliding()}, collider: {_ShootRay.GetCollider()}");
            if (_ShootRay.IsColliding() && _ShootRay.GetCollider() is Player target) {
                GD.Print($"Hitting player {target.Name} for {_Data.Damage}");
                Rpc(nameof(RpcHit), int.Parse(target.Name), _Data.Damage);
            }
        }
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RpcHit(int targetId, int damage) {
        Player target = GameManager.PlayerList.Find(p => int.Parse(p.Name) == targetId);
        if (target == null) return;
        target.Health.Health -= damage;
    }
    public override void OnSecondaryInteractUse() {
        if(!_IsReloading) StartReload();
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