using Godot;
using System;

[GlobalClass]
public partial class TweenContainer : PanelContainer
{
    [Export] public float TransitionDuration = 0.3f;
    [Export] public Tween.TransitionType TransType = Tween.TransitionType.Cubic;
    [Export] public Tween.EaseType EaseType = Tween.EaseType.Out;

    private Vector2 _lastTargetSize;
    private Tween _sizeTween;

    public override void _Ready() {
        _lastTargetSize = GetCombinedMinimumSize();
        CustomMinimumSize = _lastTargetSize;
        ChildOrderChanged += UpdateLayout;

    }

    public void UpdateLayout() {
        Vector2 currentCustomMin = CustomMinimumSize;
        CustomMinimumSize = Vector2.Zero;
        
        Vector2 targetSize = GetCombinedMinimumSize();
        
        CustomMinimumSize = currentCustomMin;

        if (targetSize.IsEqualApprox(_lastTargetSize)) return;
        _lastTargetSize = targetSize;

        if (_sizeTween != null && _sizeTween.IsRunning()) _sizeTween.Kill();

        _sizeTween = CreateTween();
        _sizeTween.SetTrans(TransType).SetEase(EaseType);
        _sizeTween.TweenProperty(this, "custom_minimum_size", targetSize, TransitionDuration);
    }
    

    public override void _Notification(int what){
        if (what == NotificationSortChildren)
        UpdateLayout();
    }
}