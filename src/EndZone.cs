using Godot;
using System;

public partial class EndZone : Area2D
{
	[Export]
	private Label label { get; set; }
	
	public void OnBodyEntered(Node2D body)
	{
		if (body is not Player) return;
		var player = body as Player;
		
		label.Show();
		
		player.TakeDamage();
		player.TakeDamage();
		player.TakeDamage();
	}
}
