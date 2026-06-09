using Godot;

public partial class InventorySlot : PanelContainer {
    private TextureRect _Icon;
    private Label _Uses;

    private static StyleBoxFlat _normalStyle;
    private static StyleBoxFlat _selectedStyle;

    public override void _Ready() {
        _Icon = GetNode<TextureRect>("MarginContainer/Icon");
        _Uses = GetNode<Label>("MarginContainer/Uses");

        if (_normalStyle == null) {
            _normalStyle = new StyleBoxFlat();
            _normalStyle.BgColor     = new Color(0, 0, 0, 0.5f);
            _normalStyle.BorderColor = new Color(1, 1, 1, 0.3f);
            _normalStyle.SetBorderWidthAll(2);
        }
        if (_selectedStyle == null) {
            _selectedStyle = new StyleBoxFlat();
            _selectedStyle.BgColor     = new Color(1, 1, 1, 0.2f);
            _selectedStyle.BorderColor = new Color(1, 1, 1, 0.9f);
            _selectedStyle.SetBorderWidthAll(2);
        }
        AddThemeStyleboxOverride("panel", _normalStyle);
    }

    public void SetItem(ItemData item) {
        if (item == null) {
            _Icon.Texture = null;
            _Uses.Text    = "";
            return;
        }
        _Icon.Texture = item.Icon;
        _Uses.Text = item switch {
            PistolData gun     => $"{gun.CurrentAmmo}/{gun.MaxAmmo}",
            AppleData apple => $"{apple.HealsRemaining}",
            _               => ""
        };
    }

    public void SetSelected(bool selected) {
        AddThemeStyleboxOverride("panel", selected ? _selectedStyle : _normalStyle);
    }
}