using Godot;
using System.Collections.Generic;

public partial class PlayerStateMachine : Node {
    public State InitialState = State.IDLE;

	public State CurrentState {
		get => _CurrentState;
		set {
			if (_States.TryGetValue(_CurrentState, out PlayerState prev)) {
				prev.ExitState();
				prev.SetProcess(false);
				prev.SetPhysicsProcess(false);
				prev.SetProcessUnhandledInput(false);
			}
			_CurrentState = value;
			if (_States.TryGetValue(_CurrentState, out PlayerState next)) {
				next.SetProcess(true);
				next.SetPhysicsProcess(true);
				next.SetProcessUnhandledInput(true);
				next.EnterState(_Player);
			}
		}
	}

    private State _CurrentState;
    private Player _Player;
    private Dictionary<State, PlayerState> _States = new();

    public enum State {
        IDLE, WALK, SPRINT, JUMP, INAIR, WALLRUN, NOCLIP
    }

	public override void _Ready() {
		_Player = GetParent<Player>();
		foreach (PlayerState state in GetChildren()) {
			_States[state.StateEnum] = state;
			state.SetProcess(false);
			state.SetPhysicsProcess(false);
			state.SetProcessUnhandledInput(false);
		}
		if (InitialState != null) CurrentState = InitialState;
}
}