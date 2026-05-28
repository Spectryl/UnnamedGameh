using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public static class GodotUtils {
    public static readonly Vector2I[] ResolutionPresets = {
        new(640, 480),
        new(720, 480),
        new(1280, 720),
        new(1920, 1080),
        new(2560, 1440),
        new(3440, 1440),
        new(3840, 2160),
    };
    public static List<Vector2I> AvailableResolutions = new();
    public static T Pop<[MustBeVariant] T>(this Godot.Collections.Array<T> array) {
        T value = array[0];
        array.RemoveAt(0);
        return value;
    }
    public static T PopAt<[MustBeVariant] T>(this Godot.Collections.Array<T> array, int index) {
        T value = array[index];
        array.RemoveAt(index);
        return value;
    }
    public static Rect2 GetIconRegionFromVectorID(this Vector2I textureID) {
        Vector2I iconTileSize = new Vector2I(64,64);
        return new Rect2(
            textureID.X * iconTileSize.X,
            textureID.Y * iconTileSize.Y,
            iconTileSize.X,
            iconTileSize.Y
        );
    }
    public static Color Inverse(this Color color) {
        return new Color(1.0f - color.R, 1.0f - color.G, 1.0f - color.B, color.A);
    }
    public static void UpdateTheme(Control root, Theme newTheme) {
        root.Theme = newTheme;
        foreach(var child in root.GetChildren()) {
            if (child is Control cChild) UpdateTheme(cChild, newTheme);
            else UpdateTheme(child, newTheme);
            
        }
    }
    public static void UpdateTheme(Node root, Theme newTheme) {
        foreach(var child in root.GetChildren()) {
            if (child is Control cChild) UpdateTheme(cChild, newTheme);
            else UpdateTheme(child, newTheme);
        }
    }

    public static void ChangeBusVolume(int busIndex, float newVolume) {
        GD.Print($"Modifying Bus Index: {busIndex} with New Volume: {newVolume}");
        AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(newVolume));

        string key = busIndex switch {
            0 => "master_volume",
            1 => "music_volume",
            2 => "game_volume",
            _ => "master_volume"
        };
        PersistentFileManager.Settings.SetValue("settings", key, newVolume);
    }
    public static float GetBusVolume(int busIndex) {
        GD.Print($"Getting Bus Index of {busIndex}:");
        return Mathf.DbToLinear(AudioServer.GetBusVolumeDb(busIndex));
    }
    public static int ChangeDisplayMonitor(int newMonitorIndex) {
        GD.Print($"Modifying Current Monitor to new monitor {newMonitorIndex}");
        int TotalMonitorCount = DisplayServer.GetScreenCount();
        if (newMonitorIndex > TotalMonitorCount - 1) newMonitorIndex = 0; 
        newMonitorIndex = Mathf.Clamp(newMonitorIndex, 0, TotalMonitorCount - 1);
        DisplayServer.WindowSetCurrentScreen((int)DisplayServer.MainWindowId, newMonitorIndex);
        PersistentFileManager.Settings.SetValue("settings", "last_used_monitor", newMonitorIndex);
        return newMonitorIndex;
    }
    public static VideoOptions.WindowType ChangeWindowType(VideoOptions.WindowType newType) {
        GD.Print($"Modifying Window Type -> {newType}");
        int windowID = (int)DisplayServer.MainWindowId;
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false, windowID);
        switch (newType) {
            case VideoOptions.WindowType.WINDOWED:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed, windowID);
                break;
            case VideoOptions.WindowType.BORDERLESS:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed, windowID);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true, windowID);
                break;
            case VideoOptions.WindowType.FULLSCREEN:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen, windowID);
                break;
        }
        PersistentFileManager.Settings.SetValue("settings", "window_type", (int) newType);
        return newType;
    }
    public static void BuildResolutionList() {
        AvailableResolutions.Clear();
        Vector2I max = DisplayServer.ScreenGetSize();
        foreach(Vector2I resolution in ResolutionPresets) {
            if (resolution.X <= max.X && resolution.Y <= max.Y) AvailableResolutions.Add(resolution);
        }
        AvailableResolutions.Sort((a,b) => a.X * a.Y - b.X * b.Y);
    }
    public static int ChangeResolution(int index) {
        index = Mathf.Clamp(index, 0, AvailableResolutions.Count - 1);
        
        DisplayServer.WindowSetSize(AvailableResolutions[index]);
        Vector2I screenSize = DisplayServer.ScreenGetSize();
        Vector2I windowSize = DisplayServer.WindowGetSize();
        DisplayServer.WindowSetPosition((screenSize - windowSize) / 2);
        PersistentFileManager.Settings.SetValue("settings", "resolution_index", index);
        return index;
    }
    public static bool ChangeVSync(bool enabled) {
        var mode = enabled ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled;
        DisplayServer.WindowSetVsyncMode(mode);
        PersistentFileManager.Settings.SetValue("settings", "vSync", enabled);
        return enabled;
    }
    public static int ChangeMSAA(Viewport viewport, int newMSAA) {
        viewport.Msaa2D = (Viewport.Msaa) newMSAA;
        PersistentFileManager.Settings.SetValue("settings", "antialias", newMSAA);
        return newMSAA;
    }
}
