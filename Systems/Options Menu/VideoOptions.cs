using Godot;
using System;
using System.Collections.Generic;
using static GodotUtils;
using System.Linq.Expressions;

public partial class VideoOptions : OptionsSubMenu {
    public enum WindowType {
        WINDOWED,
        BORDERLESS,
        FULLSCREEN,
    }
    
    private readonly WindowType[] WindowTypeOrder = {
        WindowType.WINDOWED,
        WindowType.BORDERLESS,
        WindowType.FULLSCREEN
    };

    private Button DisplayMonitorLeftButton;
    private Button DisplayMonitorRightButton;
    private Label  DisplayMonitorLabel;
    private Button WindowTypeLeftButton;
    private Button WindowTypeRightButton;
    private Label  WindowTypeLabel;
    private Button ResolutionLeftButton;
    private Button ResolutionRightButton;
    private Label  ResolutionLabel;
    private Button VSyncLeftButton;
    private Button VSyncRightButton;
    private Label  VSyncLabel;
    private Button MSAALeftButton;
    private Button MSAARightButton;
    private Label  MSAALabel;
    private int TotalMonitorCount = DisplayServer.GetScreenCount();
    private int CurrentMonitorIndex = 0;
    private WindowType CurrentWindowType;
    private int CurrentResolutionIndex = 0;
    private bool CurrentVSync = false;
    private int CurrentMSAA;
    public override void _Ready() {
        base._Ready();
        SetUpDisplayMonitor();
        SetUpWindowType();
        SetUpResolution();
        SetUpVsync(); 
        SetUpMSAA();
    }
    private void JuiceLabel(Label label) {
        label.PivotOffsetRatio = new Vector2(0.5f,0.5f);

        if (label.HasMeta("juice_tween")) {
            Variant meta = label.GetMeta("juice_tween");

            Tween existing = meta.As<Tween>();
            if (IsInstanceValid(existing) && existing.IsRunning()) existing.Kill();
        }

        Tween t = CreateTween();
        label.SetMeta("juice_tween", t);

        t.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        t.TweenProperty(label, "scale", new Vector2(1.2f, 1.2f), 0.1f);
        t.TweenProperty(label, "scale", Vector2.One, 0.2f);
    }
    private void SetUpDisplayMonitor() {
        CurrentMonitorIndex = DisplayServer.WindowGetCurrentScreen((int) DisplayServer.MainWindowId);
        DisplayMonitorLeftButton  = GetNode<Button>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/DisplayMonitor/DisplayMonitorHBoxContainer/LeftButton");
        DisplayMonitorRightButton = GetNode<Button>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/DisplayMonitor/DisplayMonitorHBoxContainer/RightButton");
        DisplayMonitorLabel       = GetNode<Label>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/DisplayMonitor/DisplayMonitorHBoxContainer/MonitorLabel");
        UpdateMonitorLabel();
        if (TotalMonitorCount == 1) {
            DisplayMonitorLeftButton.Disabled  = true;
            DisplayMonitorRightButton.Disabled = true;
        } else {
            DisplayMonitorLeftButton.Pressed  += OnMonitorLeftButtonPressed;
            DisplayMonitorRightButton.Pressed += OnMonitorRightButtonPressed;
        }
    
    }
    private void ChangeDisplayMonitors(int newMonitorIndex) {
        TotalMonitorCount = DisplayServer.GetScreenCount();
        newMonitorIndex = Mathf.Clamp(newMonitorIndex, 0, TotalMonitorCount - 1);
        CurrentMonitorIndex = ChangeDisplayMonitor(newMonitorIndex);
        UpdateMonitorLabel();
    }
    private void OnMonitorLeftButtonPressed() {
        int newIndex = CurrentMonitorIndex - 1;
        if (newIndex < 0) newIndex = TotalMonitorCount - 1;
        ChangeDisplayMonitors(newIndex);
    }
    private void OnMonitorRightButtonPressed() {
        int newIndex = CurrentMonitorIndex + 1 % TotalMonitorCount;
        ChangeDisplayMonitors(newIndex);
    }
    private void UpdateMonitorLabel() {
        DisplayMonitorLabel.Text = $"Monitor {CurrentMonitorIndex + 1}";
        JuiceLabel(DisplayMonitorLabel);
    }
    private void SetUpWindowType() {
        WindowTypeLeftButton  = GetNode<Button>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/WindowType/WindowHBoxContainer/LeftButton");
        WindowTypeRightButton = GetNode<Button>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/WindowType/WindowHBoxContainer/RightButton");
        WindowTypeLabel       = GetNode<Label>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/WindowType/WindowHBoxContainer/WindwLabel");
    
        GetCurrentWindowType();
        UpdateWindowTypeLabel();
        WindowTypeLeftButton.Pressed  += OnWindowTypeLeftPressed;
        WindowTypeRightButton.Pressed += OnWindowTypeRightPressed;
    }
    private void GetCurrentWindowType() {
        DisplayServer.WindowMode mode = DisplayServer.WindowGetMode((int)DisplayServer.MainWindowId);
        bool isBorderless = DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.Borderless, (int)DisplayServer.MainWindowId);
        
        if (mode == DisplayServer.WindowMode.Fullscreen) CurrentWindowType = WindowType.FULLSCREEN;
        else if(isBorderless)                            CurrentWindowType = WindowType.BORDERLESS;
        else                                             CurrentWindowType = WindowType.WINDOWED;
    }
    private void SetWindowType(WindowType newType) {
        CurrentWindowType = ChangeWindowType(newType);
        UpdateWindowTypeLabel();
    }
    private void UpdateWindowTypeLabel() {
        WindowTypeLabel.Text = CurrentWindowType switch
        {
            WindowType.WINDOWED => "Windowed",
            WindowType.BORDERLESS => "Borderless",
            WindowType.FULLSCREEN => "Fullscreen",
            _ => "Windowed"
        };
        JuiceLabel(WindowTypeLabel);
    }
    private void OnWindowTypeLeftPressed() {
        int index = ((int) CurrentWindowType - 1 + 3) % 3;
        SetWindowType((WindowType)index);
    }
    private void OnWindowTypeRightPressed() {
        int index = ((int) CurrentWindowType + 1 + 3) % 3;
        SetWindowType((WindowType)index);
    }
    private void SetUpResolution() {
        ResolutionLeftButton  = GetNode<Button>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/Resolution/ResolutionHBoxContainer/LeftButton");
        ResolutionRightButton = GetNode<Button>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/Resolution/ResolutionHBoxContainer/RightButton");
        ResolutionLabel       = GetNode<Label>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/Resolution/ResolutionHBoxContainer/ResolutionLabel");
        GetCurrentResolutionIndex();
        UpdateResolutionLabel();
        UpdateResolutionButtons();
        ResolutionLeftButton.Pressed  += OnResolutionLeftPressed;
        ResolutionRightButton.Pressed += OnResolutionRightPressed;
    }

    private void GetCurrentResolutionIndex() {
        Vector2I currentResolution = DisplayServer.WindowGetSize((int)DisplayServer.MainWindowId);
        int closestIndex = 0;
        int closestDifference = int.MaxValue;

        for (int i = 0; i < AvailableResolutions.Count; i++) {
            Vector2I res = AvailableResolutions[i];
            int diff = Math.Abs(res.X - currentResolution.X) + Math.Abs(res.Y - currentResolution.Y);
            if (diff < closestDifference) {
                closestDifference = diff;
                closestIndex = i;
            }
        }

    CurrentResolutionIndex = closestIndex;
    }
    private void ApplyResolution(int index) {
        CurrentResolutionIndex = GodotUtils.ChangeResolution(index);
        UpdateResolutionLabel();
        UpdateResolutionButtons();
    }
    private void UpdateResolutionLabel() {
        Vector2I resolution = AvailableResolutions[CurrentResolutionIndex];
        ResolutionLabel.Text = $"{resolution.X} x {resolution.Y}";
        JuiceLabel(ResolutionLabel);
    }
    private void OnResolutionLeftPressed() {
        if (CurrentResolutionIndex < 0) return;
        ApplyResolution(CurrentResolutionIndex - 1);
    }
    private void OnResolutionRightPressed() {
        if (CurrentResolutionIndex >= AvailableResolutions.Count - 1) return;
        ApplyResolution(CurrentResolutionIndex + 1);
    }
    private void UpdateResolutionButtons() {
        ResolutionLeftButton.Disabled  = CurrentResolutionIndex <= 0;
        ResolutionRightButton.Disabled = CurrentResolutionIndex >= AvailableResolutions.Count - 1;
    }
    private void SetUpVsync() {
        VSyncLeftButton  = GetNode<Button>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/VSync/VsyncHBoxContainer/LeftButton");
        VSyncRightButton = GetNode<Button>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/VSync/VsyncHBoxContainer/RightButton");
        VSyncLabel       = GetNode<Label>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/VSync/VsyncHBoxContainer/VSyncLabel");
        VSyncLeftButton.Pressed  += OnVSyncLeftPressed;
        VSyncRightButton.Pressed += OnVSyncRightPressed;
        GetCurrentVSync();
    }
    private void GetCurrentVSync() {
        DisplayServer.VSyncMode currentVSyncMode = DisplayServer.WindowGetVsyncMode((int)DisplayServer.MainWindowId);
        CurrentVSync = currentVSyncMode == DisplayServer.VSyncMode.Enabled ? true : false;
        UpdateVsyncButtons();
        UpdateVSyncLabel();
    }
    private void ApplyVSync(bool isEnabled) {
        CurrentVSync = ChangeVSync(isEnabled);
        UpdateVsyncButtons();
        UpdateVSyncLabel();
    }
    private void OnVSyncLeftPressed() {
        ApplyVSync(!CurrentVSync);
    }
    private void OnVSyncRightPressed() {
        ApplyVSync(!CurrentVSync);
    }
    private void UpdateVsyncButtons() {
        VSyncLeftButton.Disabled  = !CurrentVSync;
        VSyncRightButton.Disabled = CurrentVSync;
    }
    private void UpdateVSyncLabel() {
        VSyncLabel.Text = $"VSync {(CurrentVSync ? "On" : "Off")}";
        JuiceLabel(VSyncLabel);
    }

    private void SetUpMSAA() {
        MSAALeftButton = GetNode<Button>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/MSAA/MSAAHBoxContainer/LeftButton");
        MSAARightButton = GetNode<Button>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/MSAA/MSAAHBoxContainer/RightButton");
        MSAALabel = GetNode<Label>("MarginContainer/ScrollContainer/HBoxContainer/VBoxContainer/MSAA/MSAAHBoxContainer/MSAALabel");
        MSAALeftButton.Pressed  += OnMSAALeftButtonPressed;
        MSAARightButton.Pressed += OnMSAARightButtonPressed;
        GetCurrentMSAA();
    }
    private void GetCurrentMSAA() {
        CurrentMSAA = (int) GetViewport().Msaa2D;
        UpdateMSAAButtons();
        UpdateMSAALabel();
    }
    private void ApplyMSAA(int newMsaa) {
        CurrentMSAA = ChangeMSAA(GetViewport(), newMsaa);
        UpdateMSAAButtons();
        UpdateMSAALabel();
    }
    private void UpdateMSAAButtons() {
        MSAALeftButton.Disabled  = CurrentMSAA == (int)Viewport.Msaa.Disabled;
        MSAARightButton.Disabled = CurrentMSAA == (int)Viewport.Msaa.Msaa8X;
    }
    private void UpdateMSAALabel() {
        string newText = (Viewport.Msaa) CurrentMSAA switch {
            Viewport.Msaa.Disabled => "Disabled",
            Viewport.Msaa.Msaa2X   => "MSAA 2X",
            Viewport.Msaa.Msaa4X   => "MSAA 4x",
            Viewport.Msaa.Msaa8X   => "MSAA 8x",
            _                      => "Disabled",
        };
        MSAALabel.Text = $"{newText}";
        JuiceLabel(MSAALabel);
    }
    private void OnMSAALeftButtonPressed() {
        ApplyMSAA(CurrentMSAA-1);
    }
    private void OnMSAARightButtonPressed() {
        ApplyMSAA(CurrentMSAA+1);
    }
}
