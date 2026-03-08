using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Export]
	private TextureRect[] Hearts { get; set; }
	[Export]
	private Player player;
	
	public override void _Ready()
	{
		player.OnHealthChange += OnHealthChange;
	}
	
	private void OnHealthChange(int hp)
	{
		for (int i = 0; i < Hearts.Length; i++) {
			if (i < hp) Hearts[i].Show();
			else Hearts[i].Hide();
		}
	}
}
