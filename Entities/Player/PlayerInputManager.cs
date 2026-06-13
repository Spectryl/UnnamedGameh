using Godot;

public partial class PlayerInputHandler : Node {
    public Vector2 MoveInput { get; private set; }
    public bool Jump { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool Sprint { get; private set; }
    public bool SprintReleased { get; private set; }
    public bool Crouch { get; private set; }
    public bool Interact { get; private set; }
    public bool Interact2 { get; private set; }
    public bool LeftClick { get; private set; }
    public bool Drop { get; private set; }
    public bool ScrollUp { get; private set; }
    public bool ScrollDown { get; private set; }
    public bool Noclip { get; private set; }
    public bool MoveForward { get; private set; }
    public Vector2 MouseDelta { get; private set; }
    public int SlotPressed { get; private set; } = -1;

    public override void _PhysicsProcess(double delta) {
        JumpReleased = false;
        SprintReleased = false;
        Noclip = false;
        Interact = false;
        Interact2 = false;
        LeftClick = false;
        Drop = false;
        ScrollUp = false;
        ScrollDown = false;
        Crouch = false;
        MouseDelta = Vector2.Zero;
        SlotPressed = -1;

        MoveInput = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");
        Jump = Input.IsActionJustPressed("Jump");
        JumpHeld = Input.IsActionPressed("Jump");
        Sprint = Input.IsActionPressed("Sprint");
        MoveForward = Input.IsActionPressed("MoveForward");
    }

    public override void _UnhandledInput(InputEvent @event) {

        if (@event.IsActionPressed("Jump")) Jump = true;
        if (@event.IsActionReleased("Jump")) JumpReleased = true;
        if (@event.IsActionPressed("Sprint")) Sprint = true;
        if (@event.IsActionReleased("Sprint")) SprintReleased = true;
        if (@event.IsActionPressed("Crouch")) Crouch = true;
        if (@event.IsActionPressed("Interact")) Interact = true;
        if (@event.IsActionPressed("Interact2")) Interact2 = true;
        if (@event.IsActionPressed("LeftClick")) LeftClick = true;
        if (@event.IsActionPressed("Drop")) Drop = true;
        if (@event.IsActionPressed("ScrollUp")) ScrollUp = true;
        if (@event.IsActionPressed("ScrollDown")) ScrollDown = true;
        if (@event.IsActionPressed("Noclip")) Noclip = true;

        if (@event is InputEventMouseMotion mouse)
            MouseDelta = mouse.Relative;

        for (int i = 0; i < 5; i++)
            if (@event.IsActionPressed($"Slot{i + 1}")) SlotPressed = i;
    }
}