using Godot;
using System;

public partial class MovingPlatform : AnimatableBody2D
{
	[Export]
	private Vector2[] Waypoints;
	
	[Export]
	private float MoveSpeed = 80.0f;
	
	private int WaypointIndex = 0;
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 next = Waypoints[WaypointIndex];
		GlobalPosition = GlobalPosition.MoveToward(next, MoveSpeed * (float)delta);
		
		if (GlobalPosition.DistanceSquaredTo(next) < 4.0f) {
			WaypointIndex++;
			if (WaypointIndex >= Waypoints.Length) {
				WaypointIndex = 0;
			}
		}
	}
}
