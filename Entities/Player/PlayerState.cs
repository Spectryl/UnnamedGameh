using Godot;

public abstract partial class PlayerState : Node {
    public abstract PlayerStateMachine.State StateEnum { get; }
    protected PlayerStateMachine StateMachine => GetParent<PlayerStateMachine>();
    protected void TransitionTo(PlayerStateMachine.State state) => StateMachine.CurrentState = state;
    public abstract void EnterState(Player player);
    public abstract void ExitState();
    public abstract override void _Process(double delta);
    public abstract override void _PhysicsProcess(double delta);
}