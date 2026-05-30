using Godot;
using System;

public partial class MeshCollisionMirror : MeshInstance3D {
	[Export] private CollisionShape3D _collisionShape3D;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		if (Mesh is CapsuleMesh capsuleMesh && _collisionShape3D.Shape is CapsuleShape3D capsuleShape) {
			capsuleShape.Radius = capsuleMesh.Radius;
			capsuleShape.Height = capsuleMesh.Height;
		} 
	}
}
