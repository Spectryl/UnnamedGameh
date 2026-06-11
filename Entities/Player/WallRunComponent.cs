using Godot;

public class WallRunComponent {
    private const float WallRunSpeed = 12f;
    private const float WallJumpUpForce = 6f;
    private const float WallJumpAwayForce = 8f;
    private const float CameraTiltAngle = 0.3f;
    private const float CameraTiltSpeed = 8f;

    public bool IsWallRunning { get; private set; } = false;
    public float WallRunDuration = 2f;

    private float _WallRunTimer = 0f;
    private Vector3 _WallNormal = Vector3.Zero;
    private bool _IsOnRightWall = false;

    private Node3D _Owner;
    private Camera3D _Camera;
    private RayCast3D _LeftRay;
    private RayCast3D _RightRay;
    private JumpComponent _Jump;

    public WallRunComponent(Node3D owner, Camera3D camera, JumpComponent jump) {
        _Owner = owner;
        _Camera = camera;
        _Jump = jump;

        _LeftRay = new RayCast3D();
        _LeftRay.TargetPosition = new Vector3(-1.2f, 0, 0);
        _LeftRay.CollisionMask = 1;
        _Owner.AddChild(_LeftRay);

        _RightRay = new RayCast3D();
        _RightRay.TargetPosition = new Vector3(1.2f, 0, 0);
        _RightRay.CollisionMask = 1;
        _Owner.AddChild(_RightRay);
    }

    public void TryStartWallRun(Vector3 velocity, bool isOnFloor) {
        if (isOnFloor || IsWallRunning) return;
        if (velocity.Length() < 4f) return;

        if (_RightRay.IsColliding()) {
            IsWallRunning = true;
            _IsOnRightWall = true;
            _WallNormal = _RightRay.GetCollisionNormal();
            _WallRunTimer = WallRunDuration;
            _Jump.OnLanded();
            TiltCamera(CameraTiltAngle);
        } else if (_LeftRay.IsColliding()) {
            IsWallRunning = true;
            _IsOnRightWall = false;
            _WallNormal = _LeftRay.GetCollisionNormal();
            _WallRunTimer = WallRunDuration;
            _Jump.OnLanded();
            TiltCamera(-CameraTiltAngle);
        }
    }

    public Vector3 HandleWallRun(float delta, Vector3 currentVelocity) {
        _WallRunTimer -= delta;

        bool stillOnWall = _IsOnRightWall ? _RightRay.IsColliding() : _LeftRay.IsColliding();
        if (_WallRunTimer <= 0f || !stillOnWall) {
            EndWallRun();
            return currentVelocity;
        }

        Vector3 wallForward = _WallNormal.Cross(Vector3.Up).Normalized();

        if (wallForward.Dot(-_Owner.GlobalTransform.Basis.Z) < 0)
            wallForward = -wallForward;

        return new Vector3(wallForward.X * WallRunSpeed, 0f, wallForward.Z * WallRunSpeed);
    }

    public void TryWallJump(ref Vector3 velocity) {
        if (!IsWallRunning || !Input.IsActionJustPressed("Jump")) return;
        EndWallRun();
        velocity = _WallNormal * WallJumpAwayForce;
        velocity.Y = WallJumpUpForce;
    }

    public void EndWallRun() {
        if (!IsWallRunning) return;
        IsWallRunning = false;
        TiltCamera(0f);
    }

    private void TiltCamera(float targetZ) {
        Tween tween = _Owner.CreateTween();
        tween.TweenProperty(_Camera, "rotation:z", targetZ, 0.2f)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
    }
}