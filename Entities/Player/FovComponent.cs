using Godot;

public class FovComponent {
    private const float MaxFovBonus = 15f;
    private const float FovLerpSpeed = 8f;

    private Camera3D _Camera;
    private float _BaseFov;

    public FovComponent(Camera3D camera, float baseFov) {
        _Camera = camera;
        _BaseFov = baseFov;
    }

    public float BaseFov {
        get => _BaseFov;
        set {
            _BaseFov = value;
            _Camera.Fov = value;
        }
    }

    public void Update(float delta, float currentSpeed, float baseSpeed, float maxSpeed) {
        float t = Mathf.Clamp((currentSpeed - baseSpeed) / (maxSpeed - baseSpeed), 0f, 1f);
        float targetFov = _BaseFov + t * MaxFovBonus;
        _Camera.Fov = Mathf.Lerp(_Camera.Fov, targetFov, FovLerpSpeed * delta);
    }
}