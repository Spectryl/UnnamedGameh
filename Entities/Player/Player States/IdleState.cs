using Godot;

public partial class IdleState : PlayerState {
    public override PlayerStateMachine.State StateEnum => PlayerStateMachine.State.IDLE;
    private Player _Player;

    public override void EnterState(Player player) {
        _Player = player;
        _Player.JumpsRemaining = _Player.MaxJumps;
        _Player.CoyoteTimer = 0f;
		_Player.LastWallSide = 0;
    }

    public override void ExitState() { }

    public override void _Process(double delta) { }

    public override void _PhysicsProcess(double delta) {
        if (!_Player.IsMultiplayerAuthority()) return;
		_Player.Sprint.UpdateStamina((float)delta);
        Applies(delta);
        ApplyFriction(delta);
        Move();
    }

    private void Applies(double delta) {
		if (_Player.SlideCooldown > 0f) _Player.SlideCooldown -= (float)delta;
        if (_Player.JumpBufferTimer > 0f) {
            _Player.JumpBufferTimer -= (float)delta;
            if (_Player.IsOnFloor()) {
                TransitionTo(PlayerStateMachine.State.JUMP);
                return;
            }
        }
        if (!_Player.IsOnFloor()) {
            TransitionTo(PlayerStateMachine.State.INAIR);
            return;
        }
    }

    private void ApplyFriction(double delta) {
        _Player.Velocity = new Vector3(
            Mathf.MoveToward(_Player.Velocity.X, 0f, _Player.Acceleration * (float)delta),
            _Player.Velocity.Y,
            Mathf.MoveToward(_Player.Velocity.Z, 0f, _Player.Acceleration * (float)delta)
        );
    }

    private void Move() {
        Vector2 input = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");
        if (input == Vector2.Zero) return;
        TransitionTo(_Player.WalkOrRun);
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (_Player == null || !_Player.IsMultiplayerAuthority()) return;
        if (@event.IsActionPressed("Jump")) {
            _Player.JumpBufferTimer = _Player.JumpBufferTime;
            TransitionTo(PlayerStateMachine.State.JUMP);
        }
        if (@event.IsActionPressed("Noclip")) TransitionTo(PlayerStateMachine.State.NOCLIP);
    }
}