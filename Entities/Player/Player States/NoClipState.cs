using Godot;

public partial class NoClipState : PlayerState {
    public override PlayerStateMachine.State StateEnum => PlayerStateMachine.State.NOCLIP;

    private Player _Player;

    public override void EnterState(Player player) {
        _Player = player;
        _Player.CollisionLayer = 0u;
        _Player.CollisionMask = 0u;
    }

    public override void ExitState() {
        _Player.CollisionLayer = 1u;
        _Player.CollisionMask = 1u;
    }

    public override void _Process(double delta) { }

    public override void _PhysicsProcess(double delta) {
        if (!_Player.IsMultiplayerAuthority()) return;
		_Player.Sprint.UpdateStamina((float)delta);
		if (_Player.SlideCooldown > 0f) _Player.SlideCooldown -= (float)delta;
        Move(delta);
    }

    private void Move(double delta) {
        Vector2 input = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");
        Vector3 direction = (_Player.Camera.GlobalTransform.Basis * new Vector3(input.X, 0, input.Y)).Normalized();
        if (Input.IsActionPressed("Jump")) direction += Vector3.Up;
        if (Input.IsKeyPressed(Key.Ctrl)) direction += Vector3.Down;
        float speed = Input.IsActionPressed("Sprint")
            ? _Player.Speed * _Player.Sprint.SprintModifier
            : _Player.Speed;
        _Player.Velocity = direction * speed;
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (_Player == null || !_Player.IsMultiplayerAuthority()) return;
        if (@event.IsActionPressed("Noclip")) TransitionTo(PlayerStateMachine.State.IDLE);
    }
}