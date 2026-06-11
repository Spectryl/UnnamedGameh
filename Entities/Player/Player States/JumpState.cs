using Godot;

public partial class JumpState : PlayerState {
    public override PlayerStateMachine.State StateEnum => PlayerStateMachine.State.JUMP;
    private Player _Player;

    public override void EnterState(Player player) {
        _Player = player;
        _Player.FloorSnapLength = 0f;
        Jump();
    }

    public override void ExitState() {
        _Player.FloorSnapLength = 1f;
    }

    public override void _Process(double delta) { }

    public override void _PhysicsProcess(double delta) {
        if (!_Player.IsMultiplayerAuthority()) return;
		_Player.Sprint.UpdateStamina((float)delta);
        Applies(delta);
        ApplyGravity(delta);
        Move(delta);
        WallCheck();
    }

    private void Applies(double delta) {
        if (_Player.JumpBufferTimer > 0f) _Player.JumpBufferTimer -= (float)delta;
        if (_Player.CoyoteTimer > 0f) _Player.CoyoteTimer -= (float)delta;

        if (!_Player.IsOnFloor()) {
            if (_Player.Velocity.Y < 0f) {
                TransitionTo(PlayerStateMachine.State.INAIR);
                return;
            }
        }
        if (_Player.IsOnFloor()) {
            Vector2 input = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");
            TransitionTo(input != Vector2.Zero ? _Player.WalkOrRun : PlayerStateMachine.State.IDLE);
            return;
        }
    }

    private void ApplyGravity(double delta) {
        if (!_Player.IsOnFloor())
            _Player.Velocity += _Player.GetGravity() * (float)delta;
    }

	private void Move(double delta) {
		if (_Player.IsOnFloor()) return;
		Vector2 input = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");
		Vector3 direction = (_Player.Transform.Basis * new Vector3(input.X, 0, input.Y)).Normalized();
		Vector3 velocity = _Player.Velocity;

		if (direction != Vector3.Zero) {
			float targetSpeed = _Player.WalkOrRun == PlayerStateMachine.State.SPRINT
				? _Player.Speed * _Player.Sprint.SprintModifier
				: _Player.Speed;
			velocity.X = Mathf.Lerp(velocity.X, direction.X * targetSpeed, _Player.Acceleration * 0.1f * (float)delta);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * targetSpeed, _Player.Acceleration * 0.1f * (float)delta);
		}

		_Player.Velocity = velocity;
	}

	private void WallCheck() {
		if (_Player.IsOnFloor()) return;
		if (_Player.LeftWallCheck.IsColliding() && !_Player.RightWallCheck.IsColliding()) {
			_Player.WallSide = -1;
			TransitionTo(PlayerStateMachine.State.WALLRUN);
		} else if (_Player.RightWallCheck.IsColliding() && !_Player.LeftWallCheck.IsColliding()) {
			_Player.WallSide = 1;
			TransitionTo(PlayerStateMachine.State.WALLRUN);
		}
	}

    private void Jump() {
        if (_Player.JumpsRemaining <= 0) return;
        _Player.JumpsRemaining--;
        Vector3 velocity = _Player.Velocity;
        velocity.Y = _Player.JumpPower;
        _Player.Velocity = velocity;
        _Player.JumpBufferTimer = 0f;
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (_Player == null || !_Player.IsMultiplayerAuthority()) return;
        if (@event.IsActionPressed("Jump") && _Player.JumpsRemaining > 0) Jump();
        if (@event.IsActionPressed("Noclip")) TransitionTo(PlayerStateMachine.State.NOCLIP);
		if (@event.IsActionPressed("Sprint")) _Player.WalkOrRun = PlayerStateMachine.State.SPRINT;
		if (@event.IsActionReleased("Sprint")) _Player.WalkOrRun = PlayerStateMachine.State.WALK;

    }
}