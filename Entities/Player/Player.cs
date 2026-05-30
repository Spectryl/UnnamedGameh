using Godot;
using System;

public partial class Player : CharacterBody3D {
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	private MeshInstance3D _Mesh;
	private CollisionShape3D _CollisionShape;
	private Camera3D _Camera;

    public override void _Ready() {
		_Mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		_CollisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
		_Camera = GetNode<Camera3D>("Camera3D");
		if (_Mesh.Mesh is CapsuleMesh capsuleMesh && _CollisionShape.Shape is CapsuleShape3D capsuleShape) {
			capsuleShape.Radius = capsuleMesh.Radius;
			capsuleShape.Height = capsuleMesh.Height;
		} 
		_Camera.MakeCurrent();
	}

	public override void _PhysicsProcess(double delta) {
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
    public override void _UnhandledInput(InputEvent @event) {
        if (@event is InputEventMouseMotion mouseMovement) {
			RotateY(-mouseMovement.Relative.X * 0.005f);
			_Camera.RotateX(-mouseMovement.Relative.Y * 0.005f);
			Vector3 rotation = _Camera.Rotation;
			rotation.X = Mathf.Clamp(rotation.X, -Mathf.Pi / 2, Mathf.Pi / 2);
			_Camera.Rotation = rotation;
		}
    }
}
