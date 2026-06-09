using Godot;
using System;

public partial class PickableApple : Pickable{
	private MeshInstance3D[] _slices;
    private MeshInstance3D _stem;
	private MeshInstance3D _Leaves;

    protected override ItemData CreateData() => Data = new AppleData();

    public override void _Ready() {
        base._Ready();
        _slices = new MeshInstance3D[] {
            GetNode<MeshInstance3D>("Apple/Slice3"),
            GetNode<MeshInstance3D>("Apple/Slice2"),
            GetNode<MeshInstance3D>("Apple/Slice1"),
        };
        _stem = GetNode<MeshInstance3D>("Apple/Stem");
		_Leaves = GetNode<MeshInstance3D>("Apple/Leaf");
        UpdateSlices();
    }

	private void UpdateSlices() {
		GD.Print($"UpdateSlices called, Data: {Data?.GetType().Name ?? "null"}");
		if (Data is not AppleData appleData) {
			GD.Print("Data is not AppleData, returning");
			return;
		}
		GD.Print($"HealsRemaining: {appleData.HealsRemaining}");
		for (int i = 0; i < _slices.Length; i++) {
			_slices[i].Visible = i < appleData.HealsRemaining;
			GD.Print($"Slice {i} visible: {_slices[i].Visible}");
		}
		_stem.Visible   = appleData.HealsRemaining > 1;
		_Leaves.Visible = appleData.HealsRemaining > 1;
	}
}
