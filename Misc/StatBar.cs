using Godot;

[Tool]
[GlobalClass]
public partial class StatBar : Control {
    [Export] public float Percent { get; set; } = 1f;
    [Export] public Color ColorLeft { get; set; } = new Color(0.1f, 0.8f, 0.3f);
    [Export] public Color ColorRight { get; set; } = new Color(0.35f, 1f, 0.55f);
    [Export] public Color BackgroundColor { get; set; } = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    [Export] public float Skew { get; set; } = 12f;

    public void Setup(float initialPercent = 1f) {
        Percent = initialPercent;
        QueueRedraw();
    }

    public void SetPercent(float percent) {
        Percent = Mathf.Clamp(percent, 0f, 1f);
        QueueRedraw();
    }

    public override void _Draw() {
        float width = Size.X;
        float height = Size.Y;

        DrawColoredPolygon(GetParallelogram(0, 0, width, height, Skew), BackgroundColor);

        if (Percent > 0f)
            DrawPolygon(GetParallelogram(0, 0, width * Percent, height, Skew), new Color[] { ColorLeft, ColorRight, ColorRight, ColorLeft });
    }

    public override void _Process(double delta) {
        if (Engine.IsEditorHint()) QueueRedraw();
    }

    private Vector2[] GetParallelogram(float x, float y, float width, float height, float skew) {
        return new Vector2[] {
            new Vector2(x + skew,         y),
            new Vector2(x + width + skew, y),
            new Vector2(x + width - skew, y + height),
            new Vector2(x - skew,         y + height),
        };
    }
}