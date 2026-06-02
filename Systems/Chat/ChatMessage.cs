using Godot;
using System;
[GlobalClass]
public partial class ChatMessage : RichTextLabel {
	public void Setup(string sender, String message) {
		Text = $" {sender}: {message}";
		FitContent = true;
		ScrollActive = false;
		BbcodeEnabled = true;
		CustomMinimumSize = new Vector2(450, 20);
	}

}
