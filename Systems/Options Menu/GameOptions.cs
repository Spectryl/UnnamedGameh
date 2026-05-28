using Godot;
using System;

public partial class GameOptions : OptionsSubMenu {
    private HSlider ScreenShakeSlider;
    private Label   ScreenShakeLabel;
    private HSlider CRTSlider;
    private Label   CRTLabel;
    private HSlider FuzzSlider;
    private Label   FuzzLabel;
    private HSlider BlurSlider;
    private Label   BlurLabel;
    private Button  ResetButtonOne;
    private Button  ResetButtonTwo;
    private Button  ResetButtonThree;
    private Button  ResetButtonFour;
    private Button  ResetButtonFive;
    private Tween   ScreenShakeTween;
    private Tween   CRTTween;
    private Tween   FuzzTween;
    private Tween   BlurTween;
    private double  ScreenShakeDisplay;
    private double  CRTDisplay;
    private double  FuzzDisplay;
    private double  BlurDisplay;
    public override void _Ready() {
        base._Ready();
        ScreenShakeSlider = GetNode<HSlider>("MarginContainer/VBoxContainer/ScreenShake/HBoxContainer/ScreenShakeSlider");
        ScreenShakeLabel = GetNode<Label>("MarginContainer/VBoxContainer/ScreenShake/HBoxContainer/Panel/ScreenShakeLabel");
        CRTSlider = GetNode<HSlider>("MarginContainer/VBoxContainer/CRT/HBoxContainer/CRTSlider");
        CRTLabel  = GetNode<Label>("MarginContainer/VBoxContainer/CRT/HBoxContainer/Panel/CRTLabel");
        FuzzSlider = GetNode<HSlider>("MarginContainer/VBoxContainer/Fuzz/HBoxContainer/FuzzSlider");
        FuzzLabel  = GetNode<Label>("MarginContainer/VBoxContainer/Fuzz/HBoxContainer/Panel/FuzzLabel");
        BlurSlider = GetNode<HSlider>("MarginContainer/VBoxContainer/Blur/HBoxContainer/BlurSlider");
        BlurLabel  = GetNode<Label>("MarginContainer/VBoxContainer/Blur/HBoxContainer/Panel/BlurLabel");

        ResetButtonOne   = GetNode<Button>("MarginContainer/VBoxContainer/ResetTimer/HBoxContainer/PanelContainer1/Button1");
        ResetButtonTwo   = GetNode<Button>("MarginContainer/VBoxContainer/ResetTimer/HBoxContainer/PanelContainer2/Button2");
        ResetButtonThree = GetNode<Button>("MarginContainer/VBoxContainer/ResetTimer/HBoxContainer/PanelContainer3/Button3");
        ResetButtonFour  = GetNode<Button>("MarginContainer/VBoxContainer/ResetTimer/HBoxContainer/PanelContainer4/Button4");
        ResetButtonFive  = GetNode<Button>("MarginContainer/VBoxContainer/ResetTimer/HBoxContainer/PanelContainer5/Button5");

        float resetButtonValue = 0;
        UpdateDisabledButton((int)resetButtonValue);

        ResetButtonOne.Pressed   += ResetButtonOnePressed;
        ResetButtonTwo.Pressed   += ResetButtonTwoPressed;
        ResetButtonThree.Pressed += ResetButtonThreePressed;
        ResetButtonFour.Pressed  += ResetButtonFourPressed;
        ResetButtonFive.Pressed  += ResetButtonFivePressed;

        float screenShakeValue = 0;
        ScreenShakeSlider.Value = screenShakeValue;
        ScreenShakeLabel.Text = $"{(int) screenShakeValue}";
        ScreenShakeSlider.ValueChanged += value => {
            ChangeScreenShake((float) value);
            AnimateLabel(ref ScreenShakeTween, ref ScreenShakeDisplay, value, ChangeScreenShake, ScreenShakeLabel);
        };

        float crtValue = 0;
        CRTSlider.Value = crtValue;
        CRTLabel.Text = $"{crtValue * 100:F0}";
        CRTSlider.ValueChanged += value => {
            ChangeCRTIntensity((float)value);
            AnimateLabel(ref CRTTween, ref CRTDisplay, value, ChangeCRTIntensity, CRTLabel);
        };

        float fuzzValue = 0;
        FuzzSlider.Value = fuzzValue / 100;
        FuzzLabel.Text = $"{fuzzValue:F0}";
        FuzzSlider.ValueChanged += value => {
            ChangeFuzzIntensity((float)value);
            AnimateLabel(ref FuzzTween, ref FuzzDisplay, value, ChangeFuzzIntensity, FuzzLabel);
        };

        float blurValue = 0;
        BlurSlider.Value = blurValue;
        BlurLabel.Text = $"{blurValue / 3.0 * 100:F0}";
        BlurSlider.ValueChanged += value => {
            ChangeBlurIntensity((float)value);
            AnimateLabel(ref BlurTween, ref BlurDisplay, value, ChangeBlurIntensity, BlurLabel);
        };

    }
    public override void _ExitTree() {
        ResetButtonOne.Pressed   -= ResetButtonOnePressed;
        ResetButtonTwo.Pressed   -= ResetButtonTwoPressed;
        ResetButtonThree.Pressed -= ResetButtonThreePressed;
        ResetButtonFour.Pressed  -= ResetButtonFourPressed;
        ResetButtonFive.Pressed  -= ResetButtonFivePressed;
    }

    private void ResetButtonOnePressed() => ResetButtonPressed(1);
    private void ResetButtonTwoPressed() => ResetButtonPressed(2);
    private void ResetButtonThreePressed() => ResetButtonPressed(3);
    private void ResetButtonFourPressed() => ResetButtonPressed(4);
    private void ResetButtonFivePressed() =>ResetButtonPressed(5);
    
    private void ResetButtonPressed(int index) {
        GD.Print($"Updated Reset Timer {index}");
        //GameDefs.ResetTimerDuration = index;
        PersistentFileManager.Settings.Save(PersistentFileManager.SettingsPath);
        UpdateDisabledButton(index);
    }
    private void UpdateDisabledButton(int index) {
        ResetButtonOne.Disabled   = false;
        ResetButtonTwo.Disabled   = false;
        ResetButtonThree.Disabled = false;
        ResetButtonFour.Disabled  = false;
        ResetButtonFive.Disabled  = false;
        switch (index) {
            case 1:
                ResetButtonOne.Disabled = true;
                break;
            case 2:
                ResetButtonTwo.Disabled = true;
                break;
            case 3:
                ResetButtonThree.Disabled = true;
                break;
            case 4:
                ResetButtonFour.Disabled = true;
                break;
            case 5:
                ResetButtonFive.Disabled = true;
                break;
        }
    }
    private void ChangeScreenShake(float value) {
        ScreenShakeLabel.Text = $"{(int) value}";
        //GameDefs.MaxScreenShake = value;
        PersistentFileManager.Settings.Save(PersistentFileManager.SettingsPath);
    }
    private void ChangeCRTIntensity(float value) {
        GD.Print($"Updating CRT Intensity {value}");
        //GameDefs.CRTIntensity = value;
        RenderingServer.GlobalShaderParameterSet("crt_strength", value);
        CRTLabel.Text = $"{value * 100:F0}";
        PersistentFileManager.Settings.Save(PersistentFileManager.SettingsPath);
    }
    private void ChangeFuzzIntensity(float value) {
        GD.Print($"Updating Fuzz Intensity {value}");
        //GameDefs.FuzzIntensity = value * 100;
        //RenderingServer.GlobalShaderParameterSet("color_offset_multiplier", GameDefs.FuzzIntensity * 0.02f / 100);
        //RenderingServer.GlobalShaderParameterSet("black_offset_multiplier", GameDefs.FuzzIntensity * 0.054f / 100);
        //RenderingServer.GlobalShaderParameterSet("color_offset", GameDefs.FuzzIntensity * 0.002f / 100);
        FuzzLabel.Text = $"{value*100:F0}";
        PersistentFileManager.Settings.Save(PersistentFileManager.SettingsPath);
    }
    private void ChangeBlurIntensity(float value) {
        GD.Print($"Updating Blur Intensity {value}");
        //GameDefs.BlurIntensity = value;
        RenderingServer.GlobalShaderParameterSet("blur_amount", value);
        BlurLabel.Text = $"{value/3.0*100:F0}";
        PersistentFileManager.Settings.Save(PersistentFileManager.SettingsPath);
    }
    private void AnimateLabel(ref Tween tween, ref double displayVal, double targetVal, Action<float> updateAction, Label label) {
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
}
