using Godot;
using System;

public partial class SyncManager : Node {
	public float[] ThingsToSync;
	private static int _TickRate = (int) ProjectSettings.GetSetting("physics/common/physics_ticks_per_second");
    public override void _PhysicsProcess(double delta) {

    }
}
