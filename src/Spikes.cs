using Godot;
using System;

public partial class Spikes : Area2D
{
	public void OnBodyEntered(Node2D body)
	{
		if (body is not Player) return;
		var player = body as Player;
		player.TakeDamage();
	}
}
