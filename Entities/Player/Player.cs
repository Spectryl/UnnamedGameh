using Godot;
using System.Collections.Generic;

public partial class Player : CharacterBody3D {

    // Movement
    public float Speed = 10f;
    public float Acceleration = 60f;
    public float JumpPower = 6f;
    public float MouseSensitivity = 0.005f;

    // Jump
    public int MaxJumps = 2;
    public int JumpsRemaining;
    public float JumpBufferTime = 0.15f;
    public float JumpBufferTimer = 0f;

    // Coyote
    public float CoyoteTime = 0.15f;
    public float CoyoteTimer = 0f;

	// Wall run
	public float WallRunDuration = 2f;
	public float WallRunTimer = 0f;
	public int WallSide = 0; // -1 left, 1 right
	public Vector3 WallNormal = Vector3.Zero;
	public Vector3 WallForwardDir = Vector3.Zero;
	public RayCast3D LeftWallCheck;
	public RayCast3D RightWallCheck;

    // Locomotion
    public PlayerStateMachine.State WalkOrRun = PlayerStateMachine.State.WALK;

    // Components
    public HealthComponent Health = new HealthComponent(100);
    public PlayerSprintComponent Sprint = new PlayerSprintComponent();
    public PlayerInventory Inventory = new PlayerInventory();
	public FovComponent Fov;

    // Node refs
    public Camera3D Camera;
    private MeshInstance3D _Mesh;
    private CollisionShape3D _CollisionShape;
    private RayCast3D _InteractRayCast;
    private Node3D _HandSlot;
    private HeldItem _HeldItem;
    private PlayerStateMachine _StateMachine;

    public override void _Ready() {
        GameManager.PlayerList.Add(this);

        _Mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        _CollisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
        Camera = GetNode<Camera3D>("Camera3D");
        _InteractRayCast = GetNode<RayCast3D>("Camera3D/RayCast3D");
        _HandSlot = GetNode<Node3D>("Camera3D/HandSlot");
        _StateMachine = GetNode<PlayerStateMachine>("PlayerStateMachine");

        SetMultiplayerAuthority(int.Parse(Name));

        if (_Mesh.Mesh is CapsuleMesh capsuleMesh && _CollisionShape.Shape is CapsuleShape3D capsuleShape) {
            capsuleShape.Radius = capsuleMesh.Radius;
            capsuleShape.Height = capsuleMesh.Height;
        }

        if (IsMultiplayerAuthority()) {
            Camera.MakeCurrent();
            Input.MouseMode = Input.MouseModeEnum.Captured;
            SetupHUD();
            Inventory.SelectedSlotChanged += OnSelectedSlotChanged;
        }

		Fov = new FovComponent(Camera, Camera.Fov);

		LeftWallCheck = new RayCast3D();
		LeftWallCheck.TargetPosition = new Vector3(-1.2f, 0, 0);
		LeftWallCheck.CollisionMask = 1;
		AddChild(LeftWallCheck);

		RightWallCheck = new RayCast3D();
		RightWallCheck.TargetPosition = new Vector3(1.2f, 0, 0);
		RightWallCheck.CollisionMask = 1;
		AddChild(RightWallCheck);
        Sprint.SprintModifier = 1.5f;
        JumpsRemaining = MaxJumps;
    }

    public override void _ExitTree() {
        GameManager.PlayerList.Remove(this);
    }

    public override void _PhysicsProcess(double delta) {
        if (!IsMultiplayerAuthority()) return;
		UpdateFov((float)delta);
        MoveAndSlide();
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (!IsMultiplayerAuthority()) return;
        HandleMouseLook(@event);
        if (@event.IsActionPressed("Interact")) TryInteract();
        if (@event.IsActionPressed("Drop")) TryDrop();
        if (@event.IsActionPressed("ScrollUp")) Inventory.SelectPrevious();
        if (@event.IsActionPressed("ScrollDown")) Inventory.SelectNext();
        for (int i = 0; i < Inventory.Size; i++)
            if (@event.IsActionPressed($"Slot{i + 1}")) Inventory.SelectSlot(i);
    }

    public float GetCameraRotationX() => Camera.Rotation.X;
    public void SetCameraRotationX(float x) {
        Vector3 rotation = Camera.Rotation;
        rotation.X = x;
        Camera.Rotation = rotation;
    }
	public void UpdateFov(float delta) {
		float currentSpeed = new Vector3(Velocity.X, 0, Velocity.Z).Length();
		float maxSpeed = Speed * Sprint.SprintModifier;
		Fov.Update(delta, currentSpeed, Speed, maxSpeed);
	}


    private void HandleMouseLook(InputEvent @event) {
        if (@event is InputEventMouseMotion mouseMovement) {
            RotateY(-mouseMovement.Relative.X * MouseSensitivity);
            Camera.RotateX(-mouseMovement.Relative.Y * MouseSensitivity);
            Vector3 rotation = Camera.Rotation;
            rotation.X = Mathf.Clamp(rotation.X, -Mathf.Pi / 2, Mathf.Pi / 2);
            Camera.Rotation = rotation;
        }
    }

    private void TryInteract() {
        if (!_InteractRayCast.IsColliding()) return;
        if (_InteractRayCast.GetCollider() is Pickable pickable) pickable.PickUp(this);
    }

    private void TryDrop() {
        ItemData item = Inventory.Slots[Inventory.SelectedSlot];
        if (item?.Type == null) return;
        Vector3 camPos = Camera.GlobalPosition;
        Vector3 throwDir = -Camera.GlobalTransform.Basis.Z;
        bool isOnFloor = IsOnFloor();
        Inventory.RemoveItem(Inventory.SelectedSlot);
        Rpc(nameof(RpcDropItem), (int)item.Type, item.Uses, item.MaxUses, camPos, throwDir, Velocity, isOnFloor);
    }

    private void OnSelectedSlotChanged(int slot) {
        ItemData item = Inventory.Slots[slot];
        if (IsMultiplayerAuthority())
            Rpc(nameof(RpcEquipItem), (int)(item?.Type ?? ItemData.ItemType.None), item?.Uses ?? 0, item?.MaxUses ?? 0);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, TransferChannel = 0)]
    public void RpcEquipItem(int itemType, int uses, int maxUses) {
        if (_HeldItem != null && IsInstanceValid(_HeldItem)) _HeldItem.QueueFree();
        _HeldItem = null;
        string scenePath = ItemData.GetHeldScene((ItemData.ItemType)itemType);
        if (scenePath == null) return;
        _HeldItem = GD.Load<PackedScene>(scenePath).Instantiate<Node3D>() as HeldItem;
        _HandSlot.AddChild(_HeldItem);
        if (IsMultiplayerAuthority()) _HeldItem.Setup(Inventory.Slots[Inventory.SelectedSlot]);
        else _HeldItem.SetupRemote(ItemData.CreateInstance(itemType, uses, maxUses));
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

    private void SetupHUD() {
        PlayerHud playerHud = GD.Load<PackedScene>(UIDS.PlayerHud).Instantiate<PlayerHud>();
        AddChild(playerHud);
        playerHud.Setup(Inventory);
    }

    public void GiveItem(ItemData item) {
        Inventory.AddItem(item);
    }
}