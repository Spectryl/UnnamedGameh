using System;
using Godot;

/// <summary>
/// This is to handle anything and everything that would ever want/need health
/// Members: MaxHealth and Health (self explanatory)
/// Action : OnDeath (emitted when Health <= 0)
/// </summary>
public partial class HealthComponent {
	public event Action OnDeath;
	public event Action<int, int> OnHealthChanged;

	public int Health {
		get {return _Health;}
		set {
			int oldHealth = _Health;
			_Health = Mathf.Clamp(value, 0, MaxHealth);
			OnHealthChanged?.Invoke(oldHealth, _Health);
			if (_Health <= 0) OnDeath?.Invoke();
		}
	}
	public int MaxHealth {
		get {return _maxHealth;}
		set {_maxHealth = value;} //Unsure if clamping or no clamping health
	}

	private int _Health;
	private int _maxHealth;

	public HealthComponent(int nHealth, int nMaxHealth) {
		MaxHealth = nMaxHealth;
        Health = nHealth;
        
    }

	public HealthComponent(int nHealth) {
		MaxHealth = nHealth;
		Health = nHealth;
	}

}
