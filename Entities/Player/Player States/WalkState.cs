using Godot;

public partial class WalkState : PlayerState {
    public override PlayerStateMachine.State StateEnum => PlayerStateMachine.State.WALK;
    private Player _Player;

    public override void EnterState(Player player) {
        _Player = player;
        _Player.WalkOrRun = PlayerStateMachine.State.WALK;
        _Player.JumpsRemaining = _Player.MaxJumps;
        _Player.CoyoteTimer = _Player.CoyoteTime;
    }

    public override void ExitState() { }

    public override void _Process(double delta) { }

    public override void _PhysicsProcess(double delta) {
        if (!_Player.IsMultiplayerAuthority()) return;
		_Player.Sprint.UpdateStamina((float)delta);
        Applies(delta);
        ApplyGravity(delta);
        Move(delta);
    }

    private void Applies(double delta) {
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

    private void ApplyGravity(double delta) {
        if (!_Player.IsOnFloor())
            _Player.Velocity += _Player.GetGravity() * (float)delta;
    }

    private void Move(double delta) {
        Vector2 input = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");
        Vector3 direction = (_Player.Transform.Basis * new Vector3(input.X, 0, input.Y)).Normalized();
        Vector3 velocity = _Player.Velocity;

        if (direction != Vector3.Zero && _Player.IsOnFloor()) {
            velocity.X = Mathf.Lerp(velocity.X, direction.X * _Player.Speed, _Player.Acceleration * (float)delta);
            velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * _Player.Speed, _Player.Acceleration * (float)delta);
        } else {
            TransitionTo(PlayerStateMachine.State.IDLE);
        }

        _Player.Velocity = velocity;
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (_Player == null || !_Player.IsMultiplayerAuthority()) return;
		
        if (@event.IsActionPressed("Jump")) {
            _Player.JumpBufferTimer = _Player.JumpBufferTime;
            TransitionTo(PlayerStateMachine.State.JUMP);
        }

        if (@event.IsActionPressed("Sprint")) {
            _Player.WalkOrRun = PlayerStateMachine.State.SPRINT;
            TransitionTo(PlayerStateMachine.State.SPRINT);
        }

        if (@event.IsActionPressed("Noclip")) TransitionTo(PlayerStateMachine.State.NOCLIP);
    }
}