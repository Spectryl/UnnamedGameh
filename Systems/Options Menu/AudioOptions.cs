using Godot;
using static GodotUtils;
using System;

public partial class AudioOptions : OptionsSubMenu {
    private HSlider MasterVolume;
    private HSlider MusicVolume;
    private HSlider GameVolume;
    private Label MasterVolumeLabel;
    private Label MusicVolumeLabel;
    private Label GameVolumeLabel;
    private Tween MasterTween;
    private Tween MusicTween;
    private Tween GameTween;
    private double DisplayMaster;
    private double DisplayMusic;
    private double DisplayGame;
    public override void _Ready() {
        base._Ready();
        MasterVolume = GetNode<HSlider>("MarginContainer/VBoxContainer/MasterVolume/HBoxContainer/MasterVolumeSlider");
        MusicVolume  = GetNode<HSlider>("MarginContainer/VBoxContainer/MusicVolume/HBoxContainer/MusicVolumeSlider");
        GameVolume   = GetNode<HSlider>("MarginContainer/VBoxContainer/GameVolume/HBoxContainer/GameVolumeSlider");       
        
        MasterVolumeLabel = GetNode<Label>("MarginContainer/VBoxContainer/MasterVolume/HBoxContainer/Panel/MasterVolumeLabel");
        MusicVolumeLabel  = GetNode<Label>("MarginContainer/VBoxContainer/MusicVolume/HBoxContainer/Panel/MusicVolumeLabel");
        GameVolumeLabel   = GetNode<Label>("MarginContainer/VBoxContainer/GameVolume/HBoxContainer/Panel/GameVolumeLabel"); 
        
        int masterVolumeIndex = AudioServer.GetBusIndex("Master");
        int musicVolumeIndex  = AudioServer.GetBusIndex("Music");
        int gameVolumeIndex   = AudioServer.GetBusIndex("Game");

        MasterVolume.Value = GetBusVolume(masterVolumeIndex);
        MusicVolume.Value  = GetBusVolume(musicVolumeIndex);
        GameVolume.Value   = GetBusVolume(gameVolumeIndex);
        DisplayMaster = MasterVolume.Value;
        DisplayMusic  = MusicVolume.Value;
        DisplayGame   = GameVolume.Value;
        ChangeMasterVolumeLabel(MasterVolume.Value);
        ChangeMusicVolumeLabel(MusicVolume.Value);
        ChangeGameVolumeLabel(GameVolume.Value);

        MasterVolume.ValueChanged += value => ChangeBusVolume(masterVolumeIndex, (float) value);
        MusicVolume.ValueChanged  += value => ChangeBusVolume(musicVolumeIndex, (float) value);
        GameVolume.ValueChanged   += value => ChangeBusVolume(gameVolumeIndex, (float) value);

        MasterVolume.ValueChanged += (val) => {
            ChangeBusVolume(masterVolumeIndex, (float)val);
            AnimateLabel(ref MasterTween, ref DisplayMaster, val, ChangeMasterVolumeLabel, MasterVolumeLabel);
        };

        MusicVolume.ValueChanged += (val) => {
            ChangeBusVolume(musicVolumeIndex, (float)val);
            AnimateLabel(ref MusicTween, ref DisplayMusic, val, ChangeMusicVolumeLabel, MusicVolumeLabel);
        };

        GameVolume.ValueChanged += (val) => {
            ChangeBusVolume(gameVolumeIndex, (float)val);
            AnimateLabel(ref GameTween, ref DisplayGame, val, ChangeGameVolumeLabel, GameVolumeLabel);
        };

    }
    private void AnimateLabel(ref Tween tween, ref double displayVal, double targetVal, Action<double> updateAction, Label label) {
        if (tween != null) tween.Kill();
        tween = CreateTween();
        tween.SetParallel(true); 


        tween.TweenMethod(Callable.From(updateAction), displayVal, targetVal, 0.2f)
        .SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        
        Control container = label.GetParent<Control>();
        container.PivotOffsetRatio = new Vector2(0.5f,0.5f);
        
        tween.TweenProperty(container, "scale", new Vector2(1.2f, 1.2f), 0.1f)
        .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        
        tween.Chain().TweenProperty(container, "scale", Vector2.One, 0.1f);
        label.SelfModulate = new Color(1.5f, 1.5f, 1.5f); 
        tween.TweenProperty(label, "self_modulate", Colors.White, 0.2f);

        displayVal = targetVal;
    }


    private void ChangeMasterVolumeLabel(double value) {
        MasterVolumeLabel.Text = $"{(int)(value * 100)}";
    }
    private void ChangeMusicVolumeLabel(double value) {
        MusicVolumeLabel.Text = $"{(int)(value * 100)}";
    }
    private void ChangeGameVolumeLabel(double value) {
        GameVolumeLabel.Text = $"{(int)(value * 100)}";
    }
}
