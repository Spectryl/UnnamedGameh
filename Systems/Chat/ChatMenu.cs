using Godot;
using System;
using System.Collections.Generic;
public partial class ChatMenu : Control {
	private VBoxContainer _ChatHistoryContainer;
	private LineEdit _ChatEntry;
	private Button _SubmitButton;
	private Queue<ChatMessage> _MessageQueue = new Queue<ChatMessage>();
	private int _MaxMessages = 50;

    public override void _Ready() {
        _ChatHistoryContainer = GetNode<VBoxContainer>("MarginContainer/PanelContainer/VBoxContainer/ScrollContainer/ChatHistoryContainer");
		_ChatEntry = GetNode<LineEdit>("MarginContainer/PanelContainer/VBoxContainer/HBoxContainer/MessageEntry");
		_SubmitButton = GetNode<Button>("MarginContainer/PanelContainer/VBoxContainer/HBoxContainer/SubmitButton");
		ServerManager.ChatMessageSent += AddMessage;
		_SubmitButton.Pressed += OnSubmitButtonPressed;
	}
    public override void _ExitTree() {
        ServerManager.ChatMessageSent -= AddMessage;
    }

	public void AddMessage(string sender, string message) {
		GD.Print($"Adding New Message: {sender}:{message}");
		ChatMessage newMessage = new ChatMessage();
		_ChatHistoryContainer.AddChild(newMessage);
		newMessage.Setup(sender, message);
		_MessageQueue.Enqueue(newMessage);
		if (_MessageQueue.Count > _MaxMessages) {
			GD.Print("Deleting Oldest Message!");
			_MessageQueue.Dequeue().QueueFree();
		}

	}
	public void OnSubmitButtonPressed() {
		String message = _ChatEntry.Text;
		_ChatEntry.Text = "";
		ServerManager.Instance.RpcId(1, nameof(ServerManager.Instance.RpcSendChatMessage), message);
		
	}



}
