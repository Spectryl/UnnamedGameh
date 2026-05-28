using Godot;
using static GodotUtils;
using System;

public partial class PersistentFileManager {
    public static ConfigFile SaveData;
    public static ConfigFile Settings;
    public const string SaveDataPath = "user://SaveData.cfg";
    public const string SettingsPath = "user://Settings.cfg";

    public PersistentFileManager() {
        SaveData = new ConfigFile();
        Settings = new ConfigFile();
        SaveData.Load(SaveDataPath);
        Settings.Load(SettingsPath);
        BuildResolutionList();
        LoadSettings();
        

    }
    private void LoadSettings() {
        LoadAudio();
        LoadVideo();
        //LoadGame();
    }
    private void LoadAudio() {
        int masterVolumeIndex = AudioServer.GetBusIndex("Master");
        int musicVolumeIndex  = AudioServer.GetBusIndex("Music");
        int gameVolumeIndex   = AudioServer.GetBusIndex("Game");

        float MasterVolume = (float) Settings.GetValue("settings", "master_volume", 1.0f);
        float MusicVolume  = (float) Settings.GetValue("settings", "music_volume", 1.0f);
        float GameVolume   = (float) Settings.GetValue("settings", "game_volume", 1.0f);
        GD.Print($"Getting Audio Values: Master:{MasterVolume}, Music:{MusicVolume}, Game: {GameVolume}");
        ChangeBusVolume(masterVolumeIndex, MasterVolume);
        ChangeBusVolume(musicVolumeIndex,  MusicVolume);
        ChangeBusVolume(gameVolumeIndex,   GameVolume);
    }
    private void LoadVideo() {
        int currentMonitorIndex            = (int) Settings.GetValue("settings", "last_used_monitor", 0);
        int windowType                     = (int) Settings.GetValue("settings", "window_type", 0);
        int resolutionIndex                = (int)Settings.GetValue("settings", "resolution_index", 1);
        bool vSync                         = (bool)Settings.GetValue("settings", "vSync", true);
        int antialias                      = (int)Settings.GetValue("settings", "antialias", (int)Viewport.Msaa.Disabled);
        
        GD.Print($"Getting Video Values: MonitorIndex:{currentMonitorIndex}, WindowType:{windowType}, Resolution{AvailableResolutions[resolutionIndex]}, VSync:{vSync}, Antialias:{antialias}");
        ChangeDisplayMonitor(currentMonitorIndex);
        ChangeWindowType((VideoOptions.WindowType)windowType);
        ChangeResolution(resolutionIndex);
        ChangeVSync(vSync);
        ChangeMSAA(GameManager.Instance.GetViewport(), antialias);
    }
    private void LoadGame() {
        //This is from marble matrix so probably not needed
        /*
        GameDefs.MaxScreenShake       = (float) Settings.GetValue("settings", "max_screen_shake", 12.5f);
        GameDefs.ResetTimerDuration   = (float) Settings.GetValue("settings", "reset_timer", 1.0f);
        GameDefs.CRTIntensity         = (float) Settings.GetValue("settings", "crt", 1.0f);
        GameDefs.FuzzIntensity        = (float) Settings.GetValue("settings", "fuzz", 0.01);
        GameDefs.BlurIntensity        = (float) Settings.GetValue("settings","blur", 1.0);
        GD.Print($"Getting Game Values: ScreenShake:{GameDefs.MaxScreenShake}, ResetTimer:{GameDefs.ResetTimerDuration}, CRT:{GameDefs.CRTIntensity}, Fuzz:{GameDefs.FuzzIntensity}, Blur:{GameDefs.BlurIntensity}");
        RenderingServer.GlobalShaderParameterSet("crt_strength", GameDefs.CRTIntensity);
        RenderingServer.GlobalShaderParameterSet("color_offset_multiplier", GameDefs.FuzzIntensity * 0.02f / 100);
        RenderingServer.GlobalShaderParameterSet("black_offset_multiplier", GameDefs.FuzzIntensity * 0.054f / 100);
        RenderingServer.GlobalShaderParameterSet("color_offset", GameDefs.FuzzIntensity * 0.002f / 100);
        RenderingServer.GlobalShaderParameterSet("blur_amount", GameDefs.BlurIntensity);
        */
        return;
    }
}
