using Godot;
using System;

public class SprintComponent {
	public float SprintModifier {
		get {return _SprintModifier;}
		set {_SprintModifier = value;}
	}

	public bool IsSprinting {get; private set;}
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
    public PlayerSprintComponent(float sprintModifier = 1.35f) : base(sprintModifier) { }

    public float GetSpeed(float baseSpeed) {
        return base.GetSpeed(baseSpeed, Input.IsActionPressed("Sprint"));
    }
}