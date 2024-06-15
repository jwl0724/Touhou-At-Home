using Godot;
using System;

public partial class Player : Area2D {
	// VARIABLES SECTION
	[Export]
	public int Speed { get; set; } = 400;
	private int Health { get; set; } = 300;
	private float AttackCD { get; set; } = 0.2f;
	private float attackTimer = 0;
	public Vector2 ScreenSize;
	public static Player Instance { get; private set; }
	private static Vector2 defaultScale;
	private Vector2 velocity = Vector2.Zero;
	private AnimatedSprite2D sprite;
	private CollisionShape2D hitbox;

	// SIGNALS SECTION
	[Signal]
	public delegate void HitEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		if (Instance != null) {
			GD.Print("Error, more than one player instance");
		}
		Instance = this;
		ScreenSize = GetViewportRect().Size;
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		hitbox = GetNode<CollisionShape2D>("CollisionShape2D");
		defaultScale = Scale;
		InitializePlayer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// process player movement
		ProcessInput();
		Position += velocity * (float) delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);
		ProcessAnimation();

		// process projectile firing
		if (attackTimer > AttackCD) {
			// add functionality later
			GD.Print("Projectile Fired!");
		} else attackTimer += (float) delta;
	}

	// SIGNAL HANDLERS SECTION
	private void OnBodyEntered(Node2D body) {
		Hide();
		EmitSignal(SignalName.Hit);
		hitbox.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	// METHODS SECTION

	private void InitializePlayer() {
		Show();
		Position = new Vector2(x: ScreenSize.X / 2, y: ScreenSize.Y / 2);
		hitbox.Disabled = false;
	}

	private void ProcessAnimation() {
		// process X
		if (velocity.X < 0) {
			sprite.Animation = "left";
			if (sprite.Rotation > -0.3f) sprite.Rotate(-0.3f);
		} else if (velocity.X > 0) {
			sprite.Animation = "right";
			if (sprite.Rotation < 0.3f) sprite.Rotate(0.3f);
		} else {
			sprite.Animation = "default";
			sprite.Rotation = 0;
		}

		// process Y
		if (velocity.Y != 0) {
			Scale = new Vector2(defaultScale.X - 0.1f, defaultScale.Y + 0.2f);
		} else {
			Scale = defaultScale;
		}
	}

	private void ProcessInput() {
		velocity = Vector2.Zero;

		if (Input.IsActionPressed("moveRight")) velocity.X += 1;
		if (Input.IsActionPressed("moveLeft")) velocity.X -= 1;
		if (Input.IsActionPressed("moveUp")) velocity.Y -= 1;
		if (Input.IsActionPressed("moveDown")) velocity.Y += 1;

		if (velocity.Length() > 0) {
			velocity = velocity.Normalized() * Speed;
		}
	}
}
