using Godot;
using System;

public partial class HeldItem : Node3D {
	public enum HeldItemAction { PrimaryUse, SecondaryUse, InteractUse, SecondaryInteractUse }
	public virtual void Setup(ItemData data) {}
	public virtual void SetupRemote(ItemData data) {
        SetProcessUnhandledInput(false);
    }

	public virtual void OnPrimaryUse() {}
	public virtual void OnSecondaryUse() {}
	public virtual void OnInteractUse() {}
	public virtual void OnSecondaryInteractUse() {}
	

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void PerformAction(int action) {
		switch ((HeldItemAction)action) {
			case HeldItemAction.PrimaryUse: OnPrimaryUse(); break;
			case HeldItemAction.SecondaryUse: OnSecondaryUse(); break;
			case HeldItemAction.InteractUse: OnInteractUse(); break;
			case HeldItemAction.SecondaryInteractUse: OnSecondaryInteractUse(); break;
		}
	}

	public void PerformAction(HeldItemAction action) => Rpc(nameof(PerformAction), (int)action);
	protected Player GetPlayer() {
        Node node = GetParent();
        while (node != null) {
            if (node is Player player) return player;
            node = node.GetParent();
        }
        return null;
    }
	protected PlayerInputHandler Input => GetPlayer().InputHandler;
}
