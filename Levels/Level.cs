using Godot;
using System;

public partial class Level : Node {
    private TerrainGenerator _TerrainGenerator;

    public override void _Ready() {
        //_TerrainGenerator = new TerrainGenerator();
        //AddChild(_TerrainGenerator);
        TerrainGenerator.TerrainReady += OnTerrainReady;
        ServerManager.GameStarted += CreateAllPlayers;
        ReadyManager.LevelLoaded?.Invoke();
    }

    public override void _ExitTree() {
        TerrainGenerator.TerrainReady -= OnTerrainReady;
        ServerManager.GameStarted -= CreateAllPlayers;
    }

    private void OnTerrainReady() {
        GD.Print("Terrain ready");
    }

    private void CreateAllPlayers() {
        foreach (PlayerData playerData in GameManager.PlayerDataList) {
            CreatePlayer(playerData.Id);
        }
    }

    private void CreatePlayer(int id) {
        Player newPlayer = (Player)GD.Load<PackedScene>(UIDS.Player).Instantiate();
        newPlayer.Name = id.ToString();
        AddChild(newPlayer);
        float x = (float)GD.RandRange(-50, 50);
        float z = (float)GD.RandRange(-50, 50);
        //float y = _TerrainGenerator.terrain3D != null
        //    ? (float)_TerrainGenerator.terrain3D.Data.GetHeight(new Vector3(x, 0, z)) + 2.0f
        //    : 5.0f;
        newPlayer.GlobalPosition = new Vector3(x, 5f, z);
        newPlayer.GiveItem(new PistolData());
        //newPlayer.GiveItem(new PistolData());
    }

    private void RemovePlayer(int id) {
        GetNodeOrNull(id.ToString())?.QueueFree();
    }
}
