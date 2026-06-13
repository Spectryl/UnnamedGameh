using Godot;

public partial class NameTag : Label3D {
    public void Setup(bool isLocalPlayer) {
        Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
        NoDepthTest = true;
        PixelSize = 0.005f;
        FontSize = 32;
        Modulate = Colors.White;
        Position = new Vector3(0, 2.2f, 0);

        if (isLocalPlayer) {
            Visible = false;
            Rpc(nameof(SyncUsername), GameManager.Username);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void SyncUsername(string username) => Text = username;
    
}