using Godot;
using System;

public partial class Player : CharacterBody3D {
	public float Speed {
		get {return _Speed;}
		set {_Speed = value;}
	}
	private float _Speed;
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
	public PlayerSprintComponent Sprint = new PlayerSprintComponent();
	public PlayerInventory Inventory    = new PlayerInventory();

	private float MouseSensitivity = 0.005f; //TODO: Add this to options menu
	private float Friction { // TODO: Should be based on ground so later ig
		get {return _Acceleration;}
		set {;}
	}

	private float _JumpBufferTime = 0.15f;
	private float _JumpBufferTimer = 0.00f;
	private float _CoyoteTime = 0.15f;
	private float _CoyoteTimer = 0.00f;
	private bool _WasOnFloor = false;

	private Camera3D _Camera;
	private MeshInstance3D _Mesh;
	private CollisionShape3D _CollisionShape;
	private RayCast3D        _InteractRayCast;
	private Node3D _HandSlot;
	private HeldItem _HeldItem;
	

    public override void _Ready() {
		GameManager.PlayerList.Add(this);

		_Mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		_CollisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
		_Camera = GetNode<Camera3D>("Camera3D");
		_InteractRayCast = GetNode<RayCast3D>("Camera3D/RayCast3D");
		_HandSlot       = GetNode<Node3D>("Camera3D/HandSlot");

		SetMultiplayerAuthority(int.Parse(Name));

		if (_Mesh.Mesh is CapsuleMesh capsuleMesh && _CollisionShape.Shape is CapsuleShape3D capsuleShape) {
			capsuleShape.Radius = capsuleMesh.Radius;
			capsuleShape.Height = capsuleMesh.Height;
		} 

		if (IsMultiplayerAuthority()) {
			_Camera.MakeCurrent();
			Input.MouseMode = Input.MouseModeEnum.Captured;
			SetupHUD();
			Inventory.SelectedSlotChanged += OnSelectedSlotChanged;
		}

		Speed = 10.0f;
		Sprint.SprintModifier = 1.5f;
		JumpPower = 6f;
		Acceleration = 60f;
	}
    public override void _ExitTree() {
        GameManager.PlayerList.Remove(this);
    }

	public override void _PhysicsProcess(double delta) {
		if (!IsMultiplayerAuthority()) return;
		HandleGravity(delta);
		HandleMovement(delta);
		HandleJump(delta);
		MoveAndSlide();
	}

	private void HandleGravity(double delta) {
		if (!IsOnFloor()) Velocity += GetGravity() * (float)delta;
	}

	private void HandleMovement(double delta) {
		Vector2 inputDirection = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y)).Normalized();
		Vector3 velocity = this.Velocity;
		float speed = Sprint.GetSpeed(Speed, (float)delta);
		
		if (direction != Vector3.Zero) {
            velocity.X = Mathf.MoveToward(velocity.X, direction.X * speed, Acceleration * (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, direction.Z * speed, Acceleration * (float)delta);
        } else {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Friction * (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, Friction * (float)delta);
        }

        Velocity = velocity;
	}

	public float GetCameraRotationX() => _Camera.Rotation.X;
	public void SetCameraRotationX(float x) {
		Vector3 rotation = _Camera.Rotation;
		rotation.X = x;
		_Camera.Rotation = rotation;
	}
	private void HandleJump(double delta) {
		bool isOnFloor = IsOnFloor();

		if (_WasOnFloor && !isOnFloor) _CoyoteTimer = _CoyoteTime;
		if (_CoyoteTimer > 0) _CoyoteTimer -= (float) delta;

		if (Input.IsActionJustPressed("Jump")) _JumpBufferTimer = _JumpBufferTime;
        
		if (_JumpBufferTimer > 0) _JumpBufferTimer -= (float) delta;
		
		if ((IsOnFloor() || _CoyoteTimer > 0) && _JumpBufferTimer > 0) {
			Vector3 velocity = Velocity;
			velocity.Y = JumpPower;
			Velocity = velocity;
			_JumpBufferTimer = 0.0f;
			_CoyoteTimer     = 0.0f;
		}

		_WasOnFloor = isOnFloor;
		
		if (Input.IsActionJustPressed("Jump")) {
						Vector3 velocity = Velocity;
			velocity.Y = JumpPower;
			Velocity = velocity;
			_JumpBufferTimer = 0.0f;
			_CoyoteTimer     = 0.0f;
		}
	}


    public override void _UnhandledInput(InputEvent @event) {
		if (!IsMultiplayerAuthority()) return;
		HandleMouseLook(@event);
		if (@event.IsActionPressed("Interact"))    TryInteract();
		if (@event.IsActionPressed("Drop"))        TryDrop();
		if (@event.IsActionPressed("ScrollUp"))    Inventory.SelectPrevious();
		if (@event.IsActionPressed("ScrollDown"))  Inventory.SelectNext();
		for (int i = 0; i < Inventory.Size; i++) {
			if (@event.IsActionPressed($"Slot{i + 1}")) Inventory.SelectSlot(i);
		}
    }

	private void OnSelectedSlotChanged(int slot) {
		if (IsMultiplayerAuthority()) Rpc(nameof(RpcEquipItem), (int)(Inventory.Slots[slot]?.Type ?? ItemData.ItemType.None));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
	public void RpcEquipItem(int itemType) {
		_HeldItem?.QueueFree();
		_HeldItem = null;
		string scenePath = ItemData.GetHeldScene((ItemData.ItemType)itemType);
		if (scenePath == null) return;
		_HeldItem = GD.Load<PackedScene>(scenePath).Instantiate<Node3D>() as HeldItem;
		_HandSlot.AddChild(_HeldItem);
		if (IsMultiplayerAuthority()) _HeldItem.Setup(Inventory.Slots[Inventory.SelectedSlot]);
		else _HeldItem.SetupRemote();
		
	}
	private void SetupHUD() {
		PlayerHud playerHud = (PlayerHud) GD.Load<PackedScene>(UIDS.PlayerHud).Instantiate();
		AddChild(playerHud);
		playerHud.Setup(Inventory);
	}

	private void HandleMouseLook(InputEvent @event) {
		if (@event is InputEventMouseMotion mouseMovement) {
			RotateY(-mouseMovement.Relative.X * MouseSensitivity);
			_Camera.RotateX(-mouseMovement.Relative.Y * MouseSensitivity);
			Vector3 rotation = _Camera.Rotation;
			rotation.X = Mathf.Clamp(rotation.X, -Mathf.Pi / 2, Mathf.Pi / 2);
			_Camera.Rotation = rotation;
		}
	}

	private void TryInteract() {
		if (!_InteractRayCast.IsColliding()) return;
		if (_InteractRayCast.GetCollider() is Pickable pickable) pickable.PickUp(this);
	}

	private void TryDrop() {
		ItemData item = Inventory.Slots[Inventory.SelectedSlot];
		if (item?.Type == null) return;

		Vector3 camPos = _Camera.GlobalPosition;
        Vector3 throwDir = -_Camera.GlobalTransform.Basis.Z;
        bool isOnFloor = IsOnFloor();
		Inventory.RemoveItem(Inventory.SelectedSlot);
		Rpc(nameof(RpcDropItem), (int) item.Type, item.Uses, item.MaxUses, camPos, throwDir, Velocity, isOnFloor);
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
	private void RpcDropItem(int itemType, int uses, int maxUses, Vector3 camPos, Vector3 throwDir, Vector3 playerVelocity, bool isOnFloor) {
		Pickable dropped = GD.Load<PackedScene>(ItemData.GetPickupScene(itemType)).Instantiate<Pickable>();
		dropped.Data = ItemData.CreateInstance(itemType, uses, maxUses);
		GetTree().CurrentScene.AddChild(dropped, true);
		dropped.GlobalPosition = camPos + (throwDir * 0.5f);
		float throwForce = 8.0f;
        Vector3 throwVelocity = throwDir * throwForce + playerVelocity;
        if (!isOnFloor) throwVelocity += Vector3.Up * 3.0f;
        
        dropped.LinearVelocity = throwVelocity;
		if (!ServerManager.IsHost()) return;
        dropped.AngularVelocity = new Vector3(
            (float)GD.RandRange(-5.0, 5.0),
            (float)GD.RandRange(-5.0, 5.0),
            (float)GD.RandRange(-5.0, 5.0)
        );
	}
	public void GiveItem(ItemData item) {
		Inventory.AddItem(item);
	}
}
