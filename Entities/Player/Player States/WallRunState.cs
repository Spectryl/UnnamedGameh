using Godot;

public partial class WallRunState : PlayerState {
    public override PlayerStateMachine.State StateEnum => PlayerStateMachine.State.WALLRUN;

    private const float WallRunSpeed = 12f;
    private const float WallRunAcceleration = 10f;
    private const float WallRunFallGravityMultiplier = 0.1f;
    private const float CameraTiltAngle = 0.3f;

    private Player _Player;

    public override void EnterState(Player player) {
        _Player = player;
        _Player.FloorSnapLength = 0f;
        _Player.JumpsRemaining = _Player.MaxJumps;
        _Player.CoyoteTimer = _Player.CoyoteTime;
        _Player.WallRunTimer = _Player.WallRunDuration;

        CalculateWallForwardDir();
        Vector3 velocity = _Player.Velocity;
        velocity.Y = 0f;
        _Player.Velocity = velocity;

        TiltCamera(_Player.WallSide == -1 ? CameraTiltAngle : CameraTiltAngle);
    }

    public override void ExitState() {
        _Player.FloorSnapLength = 1f;
        TiltCamera(0f);
    }

    public override void _Process(double delta) { }

    public override void _PhysicsProcess(double delta) {
        if (!_Player.IsMultiplayerAuthority()) return;
		_Player.Sprint.UpdateStamina((float)delta);
        Applies(delta);
        ApplyGravity(delta);
        Move(delta);
    }

    private void Applies(double delta) {
        CalculateWallForwardDir();

        _Player.WallRunTimer -= (float)delta;
        if (_Player.WallRunTimer <= 0f) {
            TransitionTo(PlayerStateMachine.State.INAIR);
            return;
        }

        bool onWall = _Player.WallSide == -1
            ? _Player.LeftWallCheck.IsColliding()
            : _Player.RightWallCheck.IsColliding();

        if (!onWall || _Player.IsOnFloor()) {
            TransitionTo(PlayerStateMachine.State.INAIR);
            return;
        }
    }

    private void ApplyGravity(double delta) {
        Vector3 velocity = _Player.Velocity;
        velocity.Y += _Player.GetGravity().Y * WallRunFallGravityMultiplier * (float)delta;
        velocity.Y = Mathf.Min(velocity.Y, 0f);
        _Player.Velocity = velocity;
    }

    private void Move(double delta) {
        if (!Input.IsActionPressed("MoveForward")) {
            TransitionTo(PlayerStateMachine.State.INAIR);
            return;
        }

        Vector3 velocity = _Player.Velocity;
        velocity.X = Mathf.Lerp(velocity.X, _Player.WallForwardDir.X * WallRunSpeed, WallRunAcceleration * (float)delta);
        velocity.Z = Mathf.Lerp(velocity.Z, _Player.WallForwardDir.Z * WallRunSpeed, WallRunAcceleration * (float)delta);
        _Player.Velocity = velocity;
    }

    private void CalculateWallForwardDir() {
        _Player.WallNormal = _Player.WallSide == -1
            ? _Player.LeftWallCheck.GetCollisionNormal()
            : _Player.RightWallCheck.GetCollisionNormal();

        Vector3 velocityDir = _Player.Velocity.Normalized();
        _Player.WallForwardDir = (velocityDir - _Player.WallNormal * velocityDir.Dot(_Player.WallNormal)).Normalized();
    }

    private void TiltCamera(float targetZ) {
        Tween tween = _Player.CreateTween();
        tween.TweenProperty(_Player.Camera, "rotation:z", targetZ, 0.2f)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (_Player == null || !_Player.IsMultiplayerAuthority()) return;
        if (@event.IsActionPressed("Jump")) TransitionTo(PlayerStateMachine.State.JUMP);
        if (@event.IsActionPressed("Noclip")) TransitionTo(PlayerStateMachine.State.NOCLIP);
    }
}