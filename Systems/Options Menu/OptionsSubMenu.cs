using Godot;
[GlobalClass]
public abstract partial class OptionsSubMenu : Control {
    public OptionsScreenManager OptionsScreenManager;
    public override void _Ready() {
        if (OptionsScreenManager == null) {
            GD.PrintErr("OptionsScreenManager is not set!");
            return;
        }
        OptionsScreenManager.ChangeBackButtonVisibility(true);
    }
    protected void UseBackButton() {
        OptionsScreenManager.ChangeBackButtonVisibility(false);
        OptionsScreenManager.UseBackButton();
    }
}