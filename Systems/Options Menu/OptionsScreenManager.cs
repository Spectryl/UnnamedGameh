using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class OptionsScreenManager : Control {
    private OptionsSubMenu CurrentMenu;
    private Button QuitButton;
    private Button BackButton;
    private PanelContainer BackButtonContainer;
    private VBoxContainer MainPanel;
    private Tween CurrentTween;
    private Tween BackButtonTween;
    private TextureRect TopPanel;
    private static Dictionary<OptionsMenuID, PackedScene> MenuCache = new();
    public enum OptionsMenuID {
        OPTION_SELECT,
        GAME,
        AUDIO,
        VIDEO,
        CREDITS,
    }
    public override void _Ready() {
        QuitButton = GetNode<Button>("PanelContainer/VBoxContainer/MarginContainer/PanelContainer/Buttons/HBoxContainer/Close/QuitButton");
        BackButton = GetNode<Button>("PanelContainer/VBoxContainer/MarginContainer/PanelContainer/Buttons/HBoxContainer/Back/BackButton");
        BackButtonContainer = GetNode<PanelContainer>("PanelContainer/VBoxContainer/MarginContainer/PanelContainer/Buttons/HBoxContainer/Back");
        MainPanel  = GetNode<VBoxContainer>("PanelContainer/VBoxContainer");
        TopPanel   = GetNode<TextureRect>("PanelContainer/VBoxContainer/MarginContainer/PanelContainer/PanelTopColor");
        BackButtonContainer.Visible = false;
        QuitButton.Pressed += CloseOptionsScreen;
        BackButton.Pressed += SwitchToSelectScreen;
        SetupOptionsCache();
        SwitchMenu(OptionsMenuID.OPTION_SELECT);
    }
    public void UseBackButton() {
        BackButton?.EmitSignal("Pressed");
    }
    public void ChangeBackButtonVisibility(bool isVisible) {
        BackButtonTween?.Kill();
        if (isVisible) BackButtonContainer.Visible = true;

        BackButtonTween = CreateTween().SetParallel(true)
        .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

        if (isVisible) {
            BackButtonContainer.Modulate = new Color(1, 1, 1, 0);
            BackButtonContainer.Scale = Vector2.One * 0.8f;

            BackButtonTween.TweenProperty(BackButtonContainer, "modulate:a", 1.0f, 0.35f);
            BackButtonTween.TweenProperty(BackButtonContainer, "scale", Vector2.One, 0.35f);
        }
        else {
            BackButtonTween.TweenProperty(BackButtonContainer, "modulate:a", 0.0f, 0.35f);
            BackButtonTween.TweenProperty(BackButtonContainer, "scale", Vector2.One * 0.8f, 0.35f);

            BackButtonTween.Chain().SetParallel(false).TweenCallback(Callable.From(() => {
                BackButtonContainer.Visible = false;
                BackButtonContainer.Modulate = Colors.White;
                BackButtonContainer.Scale = Vector2.One;
            }));
        }

    }
    public void SwitchToSelectScreen() {
        SwitchMenu(OptionsMenuID.OPTION_SELECT);
    }
    public void SwitchToGameScreen() {
        SwitchMenu(OptionsMenuID.GAME);
    }
    public void SwitchToAudioScreen() {
        SwitchMenu(OptionsMenuID.AUDIO);
    }
    public void SwitchToVideoScreen() {
        SwitchMenu(OptionsMenuID.VIDEO);
    }

    private void CloseOptionsScreen() => GameManager.OptionScreenToggled?.Invoke();

    

    private async void SwitchMenu(OptionsMenuID menuID) {
        if (!MenuCache.TryGetValue(menuID, out PackedScene sceneToLoad)) return;
        OptionsSubMenu newMenu = await Task.Run(() => (OptionsSubMenu)sceneToLoad.Instantiate());
        newMenu.OptionsScreenManager = this;
        newMenu.Modulate = new Color(1, 1, 1, 0);
        newMenu.Scale = Vector2.One * 0.95f;
        MainPanel.AddChild(newMenu);
        CurrentMenu?.QueueFree();
        CurrentMenu = newMenu;
        Tween tween = newMenu.CreateTween().SetParallel(true)
        .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

        tween.TweenProperty(newMenu, "modulate:a", 1.0f, 0.35f);
        tween.TweenProperty(newMenu, "scale", Vector2.One, 0.35f);
    }

    private void UpdatePanelTopColor(Color newColor) {
        GradientTexture2D gradTexture = (GradientTexture2D) TopPanel.Texture;
        Gradient grad = gradTexture.Gradient;
        grad.Colors = [
            newColor,
            newColor * 0.75f,
        ];
        TopPanel.Texture = gradTexture;
    }
    private void SetupOptionsCache() {
        MenuCache[OptionsMenuID.OPTION_SELECT] = GD.Load<PackedScene>(UIDS.OptionScreenSelect);
        MenuCache[OptionsMenuID.GAME]          = GD.Load<PackedScene>(UIDS.GameOptions);
        MenuCache[OptionsMenuID.AUDIO]         = GD.Load<PackedScene>(UIDS.AudioOptions);
        MenuCache[OptionsMenuID.VIDEO]         = GD.Load<PackedScene>(UIDS.VideoOptions);
    }
}
