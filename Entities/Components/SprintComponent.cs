using Godot;
using System;

public class SprintComponent {
	public float SprintModifier {
		get {return _SprintModifier;}
		set {_SprintModifier = value;}
	}

	public bool IsSprinting {get; protected set;}
	private float _SprintModifier;

	public SprintComponent(float sprintModifier = 1.35f) {
		SprintModifier = sprintModifier;
	}

	public float GetSpeed(float baseSpeed, bool isSprinting) {
		this.IsSprinting = isSprinting;
		return IsSprinting ? baseSpeed * SprintModifier : baseSpeed;
	}
}

public class PlayerSprintComponent : SprintComponent {
    public float MaxStamina = 100.0f;
    public float Stamina {
        get { return _Stamina; }
        private set {
            float oldStamina = _Stamina;
            _Stamina = value;
            if (!Mathf.IsEqualApprox(oldStamina, _Stamina)) OnStaminaChanged?.Invoke(oldStamina, _Stamina);
        }
    }
	private float _Stamina;
    public float DrainRate = 20.0f;
    public float RegenRate = 10.0f;
    public bool IsDepleted { get; private set; } = false;
	public event Action<float, float> OnStaminaChanged;

    public PlayerSprintComponent(float sprintModifier = 1.35f) : base(sprintModifier) {
        Stamina = MaxStamina;
    }
	public void UpdateStamina(double delta) {
		if (Input.IsActionPressed("Sprint") && !IsDepleted) {
			DrainStamina((float)delta);
		} else {
			RegenStamina((float)delta);
		}
	}
    private void DrainStamina(float delta) {
        Stamina = Mathf.Max(Stamina - DrainRate * delta, 0);
        if (Stamina <= 0) IsDepleted = true;
    }

    private void RegenStamina(float delta) {
        Stamina = Mathf.Min(Stamina + RegenRate * delta, MaxStamina);
        if (IsDepleted && Stamina > MaxStamina * 0.25f) IsDepleted = false;
    }
}