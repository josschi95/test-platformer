using Godot;
using System;
using System.Threading.Tasks;


public partial class Player : CharacterBody2D
{
	[Signal]
	public delegate void OnHealthChangeEventHandler(int hp);
	
	
	[ExportGroup("Movement")]
	[Export]
	private float CoyoteTime { get; set; } = 0.2f;
	
	[Export]
	private float JumpHoldTime { get; set; } = 0.2f;
	
	[Export]
	private float WallSlideMod { get; set; } = 0.2f;
	
	[Export]
	private float Speed { get; set; } = 240.0f;
	
	[Export]
	private float JumpVelocity { get; set; } = -240.0f;
	
	private AnimatedSprite2D Sprite;
	private double FallTimer = 0.0f;
	private float WallJumpTime = 0.0f;
	
	[ExportGroup("Health")]
	[Export]
	private float InvincibilityTime { get; set; } = 0.3f;
	private float LastDamageTime = 0.0f;
	private int MaxHealth = 3;
	private int CurrentHealth;
	
	public override void _Ready()
	{
		base._Ready();
		CurrentHealth = MaxHealth;
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (CurrentHealth <= 0) return;
		
		Vector2 velocity = Velocity;
		
		if (IsOnFloor()){
			FallTimer = 0.0f;
		}
		else {
			FallTimer += delta;
			
			if (Input.IsActionPressed("JUMP") && FallTimer <= JumpHoldTime) {
				// Do Nothing, extend jump. Should prob check if JUMP has been released...
			}
			else if (IsOnWallOnly() && Velocity.Y > 0) {
				bool WallSlide = Mathf.Sign(Input.GetAxis("LEFT", "RIGHT")) != Mathf.Sign(GetWallNormal().X);
				float mod = WallSlide ? WallSlideMod : 1.0f;
				velocity += GetGravity() *  mod *(float)delta;
			}
			else {
				velocity += GetGravity() * (float)delta;
			}
		}
		
		if (Input.IsActionJustPressed("JUMP"))
		{
			if (CanGroundJump()) {
				velocity.Y = JumpVelocity;
			}
			else if (IsOnWallOnly()) {
				WallJumpTime = Time.GetTicksMsec();
				velocity.Y = JumpVelocity;
				velocity.X = -GetWallNormal().X * JumpVelocity;
			}
		}
		
		// 0.2 second delay after wall jumping before allowing horizontal movement
		// So player doesn't stick to the wall
		if (Time.GetTicksMsec() >= WallJumpTime + 0.2 * 1000) {
			Vector2 direction = Input.GetVector("LEFT", "RIGHT", "UP", "DOWN");
			if (direction != Vector2.Zero)
			{
				velocity.X = direction.X * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			}
		}
		
		if (!IsOnFloor()){
			if (velocity.Y > 0) Sprite.Play("fall");
			else Sprite.Play("jump");
		}
		else {
			if (velocity.X > 0) {
				Sprite.Play("move");
				Sprite.FlipH = false;
			}
			else if (velocity.X < 0) {
				Sprite.Play("move");
				Sprite.FlipH = true;
			}
			else {
				Sprite.Play("idle");
			}
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}
	
	private bool CanGroundJump()
	{
		return IsOnFloor() || FallTimer <= CoyoteTime;
	}
	
	public void TakeDamage()
	{
		if (CurrentHealth <= 0) return;
		
		var now = Time.GetTicksMsec();
		if (now < LastDamageTime + InvincibilityTime * 1000) return;
		LastDamageTime = now;
		
		CurrentHealth -= 1;
		EmitSignal(SignalName.OnHealthChange, CurrentHealth);
		
		#pragma warning disable
		if (CurrentHealth <= 0) Die();
		#pragma warning restore
	}
	
	private async Task Die()
	{
		Sprite.Play("death");
		
		await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
		
		GetTree().ChangeSceneToFile("res://scn/main.tscn");
	}
}
