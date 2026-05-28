using Godot;
using System;
[GlobalClass]
public partial class NoFocusButton : Button {
    [Export] private bool IsTweened = false;
    private Vector2 _normalScale = Vector2.One;
    private Vector2 _hoverScale = new Vector2(1.1f, 1.1f);
    private Vector2 _pressedScale = new Vector2(0.9f, 0.7f);
    private Tween _activeTween;
    public override void _Ready() {
        base._Ready();
        if (IsTweened) ButtonTween(this);
    }
    public override void _Pressed() {
        base._Pressed();
        ReleaseFocus();
    }
    private void ButtonTween(Button button) {
        Control wrapper = (button.GetParent() is PanelContainer) ? button.GetParent<PanelContainer>() : button;
        
        wrapper.PivotOffsetRatio = new Vector2(0.5f, 1.0f);

        button.MouseEntered += () => StartTween(wrapper, _hoverScale, 0.2f, Tween.TransitionType.Quart);
        
        button.MouseExited += () => StartTween(wrapper, _normalScale, 0.2f, Tween.TransitionType.Quart);

        button.ButtonDown += () => StartTween(wrapper, _pressedScale, 0.1f, Tween.TransitionType.Back);

        button.ButtonUp += () => {
            Vector2 target = button.IsHovered() ? _hoverScale : _normalScale;
            StartTween(wrapper, target, 0.4f, Tween.TransitionType.Elastic);
        };
    }
    private void StartTween(Control target, Vector2 scale, float duration, Tween.TransitionType trans) {
        if (_activeTween != null) _activeTween.Kill();
        
        _activeTween = CreateTween();
        _activeTween.SetTrans(trans).SetEase(Tween.EaseType.Out);
        _activeTween.TweenProperty(target, "scale", scale, duration);
    }

}
