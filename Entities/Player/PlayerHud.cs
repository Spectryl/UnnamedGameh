using Godot;
using System;

public partial class PlayerHud : CanvasLayer {
	private TextureRect _CrosshairTexture;

    public override void _Ready() {
        _CrosshairTexture = GetNode<TextureRect>("Control/CrosshairContainer/CrosshairTexture");
    }
}
