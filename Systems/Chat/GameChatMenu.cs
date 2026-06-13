using Godot;
using System.Collections.Generic;

public partial class GameChatMenu : Control {
    [Export] private float LingerTime = 5f;
    [Export] private float FadeDuration = 1.5f;

    private VBoxContainer _ChatHistoryContainer;
    private LineEdit _ChatEntry;
    private Queue<ChatMessage> _MessageQueue = new Queue<ChatMessage>();
    private int _MaxMessages = 50;

    private float _LingerTimer = 0f;
    private bool _IsOpen = false;
    private bool _IsVisible = false;

    public override void _Ready() {
        _ChatHistoryContainer = GetNode<VBoxContainer>("VBoxContainer/ScrollContainer/ChatHistoryContainer");
        _ChatEntry = GetNode<LineEdit>("VBoxContainer/ChatEntry");

        ServerManager.ChatMessageSent += AddMessage;
        _ChatEntry.TextSubmitted += OnChatEntrySubmitted;
        _ChatEntry.KeepEditingOnTextSubmit = true;

        Modulate = new Color(1, 1, 1, 0);
    }

    public override void _ExitTree() {
        ServerManager.ChatMessageSent -= AddMessage;
    }

    public override void _Process(double delta) {
        if (_IsOpen) return;
        if (!_IsVisible) return;

        _LingerTimer -= (float)delta;
        if (_LingerTimer <= 0f) {
            FadeOut();
        }
    }

	public override void _UnhandledInput(InputEvent @event) {
		if (_IsOpen) {
			if (@event.IsActionPressed("ToggleChat")) {
				Close();
				GetViewport().SetInputAsHandled();
				return;
			}
			if (@event.IsActionPressed("ui_cancel")) {
				Close();
				GetViewport().SetInputAsHandled();
				return;
			}
			GetViewport().SetInputAsHandled();
			return;
		}

		if (@event.IsActionPressed("ToggleChat")) {
			Open();
			GetViewport().SetInputAsHandled();
		}
	}
    public void AddMessage(string sender, string message) {
        ChatMessage newMessage = new ChatMessage();
        _ChatHistoryContainer.AddChild(newMessage);
        newMessage.Setup(sender, message);
        _MessageQueue.Enqueue(newMessage);

        if (_MessageQueue.Count > _MaxMessages) {
            _MessageQueue.Dequeue().QueueFree();
        }

        ShowChat();
    }

    private void Open() {
        _IsOpen = true;
        ShowChat();
        _ChatEntry.GrabFocus();
    }

    private void Close() {
        _IsOpen = false;
        _ChatEntry.ReleaseFocus();
        ResetLingerTimer();
    }

    private void ShowChat() {
        _IsVisible = true;
        ResetLingerTimer();
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 1f, 0.2f);
    }

    private void FadeOut() {
        _IsVisible = false;
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0f, FadeDuration);
    }

    private void ResetLingerTimer() {
        _LingerTimer = LingerTime;
    }

    private void OnChatEntrySubmitted(string newMessage) {
        if (newMessage == "") return;
        _ChatEntry.Text = "";
        ServerManager.Instance.RpcId(1, nameof(ServerManager.Instance.RpcSendChatMessage), newMessage);
    }
}