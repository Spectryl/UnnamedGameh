using Godot;

public class JumpComponent {
    public int MaxJumps { get; set; }
    public int JumpsRemaining { get; private set; }

    public JumpComponent(int maxJumps = 2) {
        MaxJumps = maxJumps;
        JumpsRemaining = maxJumps;
    }

    public void OnLanded() => JumpsRemaining = MaxJumps;
    public bool CanJump() => JumpsRemaining > 0;
    public void ConsumeJump() => JumpsRemaining--;
}