using Godot;
using System;
[GlobalClass]
public partial class PlayerEntry : RichTextLabel {
    public int Id;
    public void Setup(int id, string username) {
        Id = id;
        Text = $" {username}";
		FitContent = true;
		ScrollActive = false;
		BbcodeEnabled = true;
    }
}
