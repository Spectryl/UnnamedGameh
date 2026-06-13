using Godot;
using System;

public partial class PlayerSyncManager : Node {
    public static PlayerSyncManager Instance { get; private set; }
    private static int _TickRate = (int)ProjectSettings.GetSetting("physics/common/physics_ticks_per_second");
    private int _Tick = 0;
    private int SyncInterval = 1;
    private string _LocalId => GameManager.Instance.Multiplayer.GetUniqueId().ToString();

    public override void _Ready() {
        Instance = this;
    }

    public override void _PhysicsProcess(double delta) {
        _Tick++;
        if (_Tick < SyncInterval) return;
        _Tick = 0;
        SyncLocalPlayer();
    }

    private static float LerpAngle(float from, float to, float weight) {
        float delta = ((to - from + Mathf.Pi * 3f) % (Mathf.Pi * 2f)) - Mathf.Pi;
        return from + delta * weight;
    }

    private static Vector3 LerpRotation(Vector3 from, Vector3 to, float weight) {
        return new Vector3(
            LerpAngle(from.X, to.X, weight),
            LerpAngle(from.Y, to.Y, weight),
            LerpAngle(from.Z, to.Z, weight)
        );
    }

    private void SyncLocalPlayer() {
        Player localPlayer = GameManager.PlayerList.Find(p => p.Name == _LocalId);
        if (localPlayer == null) return;
        int offset = 0;
        byte[] data = new byte[28];
        float xPosition = localPlayer.GlobalPosition.X;
        float yPosition = localPlayer.GlobalPosition.Y;
        float zPosition = localPlayer.GlobalPosition.Z;
        float xRotation = localPlayer.GlobalRotation.X;
        float yRotation = localPlayer.GlobalRotation.Y;
        float zRotation = localPlayer.GlobalRotation.Z;
        float cameraX   = localPlayer.GetCameraRotationX();
        Buffer.BlockCopy(BitConverter.GetBytes(xPosition), 0, data, offset,      4);
        Buffer.BlockCopy(BitConverter.GetBytes(yPosition), 0, data, offset + 4,  4);
        Buffer.BlockCopy(BitConverter.GetBytes(zPosition), 0, data, offset + 8,  4);
        Buffer.BlockCopy(BitConverter.GetBytes(xRotation), 0, data, offset + 12, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(yRotation), 0, data, offset + 16, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(zRotation), 0, data, offset + 20, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(cameraX),   0, data, offset + 24, 4);
        Rpc(nameof(RpcSyncPlayer), _LocalId, data);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable, TransferChannel = 0)]
    private void RpcSyncPlayer(string id, byte[] data) {
        if (id == _LocalId) return;
        Player player = GameManager.PlayerList.Find(p => p.Name == id);
        if (player == null) return;
        float xPosition = BitConverter.ToSingle(data, 0);
        float yPosition = BitConverter.ToSingle(data, 4);
        float zPosition = BitConverter.ToSingle(data, 8);
        float xRotation = BitConverter.ToSingle(data, 12);
        float yRotation = BitConverter.ToSingle(data, 16);
        float zRotation = BitConverter.ToSingle(data, 20);
        float cameraX   = BitConverter.ToSingle(data, 24);
        player.GlobalPosition = player.GlobalPosition.Lerp(new Vector3(xPosition, yPosition, zPosition), 0.3f);
        player.GlobalRotation = LerpRotation(player.GlobalRotation, new Vector3(xRotation, yRotation, zRotation), 0.3f);
        player.SetCameraRotationX(cameraX);
    }
}