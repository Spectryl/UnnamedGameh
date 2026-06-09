using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class PickablePistol : Pickable {
   protected override ItemData CreateData() => new PistolData();
}

