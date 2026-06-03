using Godot;
using System;

public partial class SyncManager : Node {
    public static SyncManager Instance {get; private set;}
	public float[] ThingsToSync;
	private static int _TickRate = (int) ProjectSettings.GetSetting("physics/common/physics_ticks_per_second");
    private int _Tick = 0;

    public override void _Ready() {
        Instance = this;
    }
    public override void _PhysicsProcess(double delta) {

    }

    private void SyncPlayers() {
        
    }
}
