using Godot;
using System;

public partial class Player : CharacterBody3D {
	public float Speed {
		get {return _Speed;}
		set {_Speed = value;}
	}
	private float _Speed;

	public float SprintModifier {
		get {return _SprintModifier;}
		set {_SprintModifier = value;}
	}
	private float _SprintModifier;

	public float Acceleration {
		get {return _Acceleration;}
		set {_Acceleration = value;}
	}
	private float _Acceleration;

	public float JumpPower {
		get {return _JumpPower;}
		set {_JumpPower = value;}
	}
	private float _JumpPower;

	public HealthComponent Health = new HealthComponent(100);

	private float MouseSensitivity = 0.005f; //TODO: Add this to options menu
	private float Friction { // TODO: Should be based on ground so later ig
		get {return _Acceleration;}
		set {;}
	}

	private Camera3D _Camera;
	private MeshInstance3D _Mesh;
	private CollisionShape3D _CollisionShape;
	

    public override void _Ready() {
		GameManager.PlayerList.Add(this);

		_Mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		_CollisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
		_Camera = GetNode<Camera3D>("Camera3D");

		SetMultiplayerAuthority(int.Parse(Name));

		if (_Mesh.Mesh is CapsuleMesh capsuleMesh && _CollisionShape.Shape is CapsuleShape3D capsuleShape) {
			capsuleShape.Radius = capsuleMesh.Radius;
			capsuleShape.Height = capsuleMesh.Height;
		} 

		if (IsMultiplayerAuthority()) {
			_Camera.MakeCurrent();
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}

		Speed = 10.0f;
		SprintModifier = 2.5f;
		JumpPower = 6f;
		Acceleration = 10f;
	}
    public override void _ExitTree() {
        GameManager.PlayerList.Remove(this);
    }

	public override void _PhysicsProcess(double delta) {
		if (!IsMultiplayerAuthority()) return;
		HandleGravity(delta);
		HandleMovement(delta);
		HandleJump();
		MoveAndSlide();
	}

	private void HandleGravity(double delta) {
		if (!IsOnFloor()) Velocity += GetGravity() * (float)delta;
	}

	private void HandleMovement(double delta) {
		Vector2 inputDirection = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y)).Normalized();
		Vector3 velocity = this.Velocity;
		float speed = Input.IsActionPressed("Sprint") ? Speed * SprintModifier : this.Speed;
		
		if (direction != Vector3.Zero) {
            velocity.X = Mathf.MoveToward(velocity.X, direction.X * speed, Acceleration * (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, direction.Z * speed, Acceleration * (float)delta);
        } else {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Friction * (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, Friction * (float)delta);
        }

        Velocity = velocity;
	}

	private void HandleJump() {
		if (Input.IsActionJustPressed("Jump") && IsOnFloor()) {
            Vector3 velocity = Velocity;
            velocity.Y = JumpPower;
            Velocity = velocity;
        }
	}


    public override void _UnhandledInput(InputEvent @event) {
		if (!IsMultiplayerAuthority()) return;
        if (@event is InputEventMouseMotion mouseMovement) {
			RotateY(-mouseMovement.Relative.X * MouseSensitivity);
			_Camera.RotateX(-mouseMovement.Relative.Y * MouseSensitivity);
			Vector3 rotation = _Camera.Rotation;
			rotation.X = Mathf.Clamp(rotation.X, -Mathf.Pi / 2, Mathf.Pi / 2);
			_Camera.Rotation = rotation;
		}
    }
}
