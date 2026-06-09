using Godot;
using System;
using System.Collections.Generic;

public partial class RigidBodySyncManager : Node {
	public static RigidBodySyncManager Instance {get; private set;}
	private List<RigidBody3D> _RigidBodies = new List<RigidBody3D>();
	private int _Tick = 0;
    private int SyncInterval = 2;
    public override void _Ready() {
        Instance = this;
    }

    public override void _PhysicsProcess(double delta) {
        if (!ServerManager.IsHost()) return;
		_Tick++;
		if (_Tick < SyncInterval) return;
		_Tick = 0;
		SyncBodies();
    }
	public static void Register(RigidBody3D body)   => Instance._RigidBodies.Add(body);
    public static void Unregister(RigidBody3D body) => Instance._RigidBodies.Remove(body);

	private void SyncBodies() {
        int count = _RigidBodies.Count;
        if (count == 0) return;
        byte[] data = new byte[count * 24];
        int offset = 0;
		foreach (RigidBody3D body in _RigidBodies) {
            Buffer.BlockCopy(BitConverter.GetBytes(body.GlobalPosition.X), 0, data, offset,      4);
            Buffer.BlockCopy(BitConverter.GetBytes(body.GlobalPosition.Y), 0, data, offset + 4,  4);
            Buffer.BlockCopy(BitConverter.GetBytes(body.GlobalPosition.Z), 0, data, offset + 8,  4);
            Buffer.BlockCopy(BitConverter.GetBytes(body.GlobalRotation.X), 0, data, offset + 12, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(body.GlobalRotation.Y), 0, data, offset + 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(body.GlobalRotation.Z), 0, data, offset + 20, 4);
            
            offset += 24;
        }
        Rpc(nameof(RpcSyncBodies), data);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable, TransferChannel = 0)]
    private void RpcSyncBodies(byte[] data) {
        if (ServerManager.IsHost()) return;
        int count  = data.Length / 24;
		int loopCount = count % (_RigidBodies.Count + 1);
        int offset = 0;
		for (int i = 0; i < loopCount; i++) {
            float px = BitConverter.ToSingle(data, offset);
            float py = BitConverter.ToSingle(data, offset + 4);
            float pz = BitConverter.ToSingle(data, offset + 8);
            float rx = BitConverter.ToSingle(data, offset + 12);
            float ry = BitConverter.ToSingle(data, offset + 16);
            float rz = BitConverter.ToSingle(data, offset + 20);
            
            offset += 24;
            RigidBody3D body = _RigidBodies[i];            
            body.GlobalPosition = body.GlobalPosition.Lerp(new Vector3(px, py, pz), 0.3f);
            body.GlobalRotation = body.GlobalRotation.Lerp(new Vector3(rx, ry, rz), 0.3f);
        }
    }
}

