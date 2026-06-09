using Godot;
using System;

public partial class HeldItem : Node3D {
	public virtual void Setup(ItemData data) {}
	public virtual void SetupRemote() {
        SetProcessUnhandledInput(false);
    }
}
