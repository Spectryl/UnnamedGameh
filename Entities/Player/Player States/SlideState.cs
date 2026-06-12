using Godot;

public partial class SlideState : PlayerState {
    public override PlayerStateMachine.State StateEnum => PlayerStateMachine.State.SLIDE;

    private Player _Player;
    private float _SlopeAngle = 0f;
    private const float MaxSlopeAngle = 30f;
    private float _CurrentSlideSpeed;

    public override void EnterState(Player player) {
        _Player = player;
        _Player.FloorSnapLength = 1f;
        _Player.JumpsRemaining = _Player.MaxJumps;
        _Player.CoyoteTimer = _Player.CoyoteTime;
        _Player.SlideTimer = _Player.SlideDuration;
        _Player.LastWallSide = 0;
        _Player.SlideDirection = (-_Player.Transform.Basis.Z).Normalized();
        _CurrentSlideSpeed = _Player.SlideSpeed;
        TiltCamera();
    }

    public override void ExitState() {
        _Player.SlideCooldown = _Player.SlideCooldownDuration;
        _Player.SlideDirection = Vector3.Zero;
        ResetCamera();
    }

    public override void _Process(double delta) { }

    public override void _PhysicsProcess(double delta) {
        if (!_Player.IsMultiplayerAuthority()) return;
        _Player.Sprint.UpdateStamina((float)delta);
        if (_Player.SlideCooldown > 0f) _Player.SlideCooldown -= (float)delta;
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

        _SlopeAngle = Mathf.RadToDeg(Mathf.Acos(_Player.GetFloorNormal().Dot(Vector3.Up)));

        if (_SlopeAngle < MaxSlopeAngle) {
            _Player.SlideTimer -= (float)delta;
            if (_Player.SlideTimer <= 0f) {
                _Player.SlideDirection = Vector3.Zero;
                TransitionTo(_Player.CeilingCheck.IsColliding()
                    ? PlayerStateMachine.State.IDLE
                    : _Player.WalkOrRun);
                return;
            }
        }
    }

    private void ApplyGravity(double delta) {
        if (!_Player.IsOnFloor())
            _Player.Velocity += _Player.GetGravity() * (float)delta;
    }

    private void Move(double delta) {
        if (_Player.SlideDirection == Vector3.Zero || !_Player.IsOnFloor()) return;

        if (_SlopeAngle < MaxSlopeAngle) {
            float newSpeed = _CurrentSlideSpeed - _Player.SlideDeceleration * (float)delta;
            _CurrentSlideSpeed = Mathf.Max(newSpeed, 0f);
        } else {
            _CurrentSlideSpeed += _Player.SlideDeceleration * (float)delta;
        }

        Vector3 velocity = _Player.Velocity;
        velocity.X = _Player.SlideDirection.X * _CurrentSlideSpeed;
        velocity.Z = _Player.SlideDirection.Z * _CurrentSlideSpeed;
        _Player.Velocity = velocity;
    }

	private void TiltCamera() {
		Tween tween = _Player.CreateTween();
		tween.TweenProperty(_Player.Camera, "position:y", _Player.CameraDefaultY - 0.4f, 0.15f)
			.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		tween.Parallel().TweenProperty(_Player.Camera, "rotation:z", 0.1f, 0.15f)
			.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
	}

	private void ResetCamera() {
		Tween tween = _Player.CreateTween();
		tween.TweenProperty(_Player.Camera, "position:y", _Player.CameraDefaultY, 0.15f)
			.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		tween.Parallel().TweenProperty(_Player.Camera, "rotation:z", 0f, 0.15f)
			.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
	}

    public override void _UnhandledInput(InputEvent @event) {
        if (_Player == null || !_Player.IsMultiplayerAuthority()) return;
        if (@event.IsActionPressed("Jump")) {
            if (_SlopeAngle > MaxSlopeAngle || !_Player.CeilingCheck.IsColliding()) {
                _Player.SlideDirection = Vector3.Zero;
                TransitionTo(PlayerStateMachine.State.JUMP);
            }
        }
        if (@event.IsActionPressed("Noclip")) TransitionTo(PlayerStateMachine.State.NOCLIP);
    }
}