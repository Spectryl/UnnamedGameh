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
	public float Stamina {get; private set;}
	public float DrainRate = 20.0f;
	public float RegenRate = 10.0f;
	private bool _IsDepleted = false;
    public PlayerSprintComponent(float sprintModifier = 1.35f) : base(sprintModifier) {
		Stamina = MaxStamina;
	}

    public float GetSpeed(float baseSpeed, float delta) {
		UpdateStamina(delta);
        return IsSprinting && !_IsDepleted ? baseSpeed * SprintModifier : baseSpeed;
    }

	private void UpdateStamina(float delta) {
			IsSprinting = Input.IsActionPressed("Sprint");
			if (Stamina <= 0) _IsDepleted = true;
			if (IsSprinting && !_IsDepleted) {
				Stamina = Mathf.Max(Stamina - DrainRate * delta, 0);
			} else {
				Stamina = Mathf.Min(Stamina + RegenRate * delta, MaxStamina);
				if (_IsDepleted && Stamina > MaxStamina * 0.25f) _IsDepleted = false;
			}
	}
}