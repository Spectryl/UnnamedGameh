using Godot;
using System;

public partial class HeldApple : HeldItem {
	private AppleData _Data;
    private MeshInstance3D[] _Slices;
	private MeshInstance3D _Stem;
	private MeshInstance3D _Leaves;

    public override void _Ready() {
        _Slices = new MeshInstance3D[] {
            GetNode<MeshInstance3D>("Apple/Slice3"),
            GetNode<MeshInstance3D>("Apple/Slice2"),
            GetNode<MeshInstance3D>("Apple/Slice1"),
        };
		_Stem = GetNode<MeshInstance3D>("Apple/Stem");
		_Leaves = GetNode<MeshInstance3D>("Apple/Leaf");
		if (_Data != null) UpdateSlices();
    }

    public override void Setup(ItemData data) {
        _Data = data as AppleData;
        if (_Data != null) UpdateSlices();
    }

    public override void SetupRemote(ItemData data) {
        base.SetupRemote(data);
		_Data = data as AppleData;
		if (_Data != null) UpdateSlices();
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("Interact")) PerformAction(HeldItemAction.InteractUse);
    }

	public override void OnInteractUse() {
		if (_Data == null || !_Data.CanHeal()) return;
		Player player = GetPlayer();
		if (player == null) return;

		_Data.Heal(player);
		player.Inventory.NotifySlotChanged();
		if (_Data.HealsRemaining <= 0) {
			player.Inventory.RemoveItem(player.Inventory.SelectedSlot);
			QueueFree();
		} 
		else Rpc(nameof(SyncAppleState), _Data.HealsRemaining);
		
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void SyncAppleState(int healsRemaining) {
		_Data.HealsRemaining = healsRemaining;
		UpdateSlices();
	}

    private void UpdateSlices() {
		GD.Print($"HealsRemaining: {_Data.HealsRemaining}");
		for (int i = 0; i < _Slices.Length; i++) {
			GD.Print($"Slice {i} visible: {i < _Data.HealsRemaining}");
			_Slices[i].Visible = i < _Data.HealsRemaining;
		}
		_Stem.Visible = _Data.HealsRemaining > 1;
		_Leaves.Visible = _Data.HealsRemaining > 1;
    }
}
