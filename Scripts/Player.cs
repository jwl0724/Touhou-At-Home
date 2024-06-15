using Godot;
using System;

public partial class Player : Area2D {

	[Export]
	public int Speed { get; set; } = 400;
	public Vector2 ScreenSize;
	public static Player Instance { get; private set; }
	private static Vector2 _defaultScale;
	private Vector2 _velocity = Vector2.Zero;
	private AnimatedSprite2D sprite;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		if (Instance != null) {
			GD.Print("Error, more than one player instance");
		}
		Instance = this;
		ScreenSize = GetViewportRect().Size;
		Position = new Vector2(x: ScreenSize.X / 2, y: ScreenSize.Y / 2);
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_defaultScale = Scale;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// process player movement
		ProcessInput();
		Position += _velocity * (float) delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);
		ProcessAnimation();
	}

	private void ProcessAnimation() {
		// process X
		if (_velocity.X < 0) {
			sprite.Animation = "left";
			if (sprite.Rotation > -0.3f) sprite.Rotate(-0.3f);
		} else if (_velocity.X > 0) {
			sprite.Animation = "right";
			if (sprite.Rotation < 0.3f) sprite.Rotate(0.3f);
		} else {
			sprite.Animation = "default";
			sprite.Rotation = 0;
		}

		// process Y
		if (_velocity.Y != 0) {
			Scale = new Vector2(_defaultScale.X - 0.1f, _defaultScale.Y + 0.2f);
		} else {
			Scale = _defaultScale;
		}
	}

	private void ProcessInput() {
		_velocity = Vector2.Zero;

		if (Input.IsActionPressed("moveRight")) _velocity.X += 1;
		if (Input.IsActionPressed("moveLeft")) _velocity.X -= 1;
		if (Input.IsActionPressed("moveUp")) _velocity.Y -= 1;
		if (Input.IsActionPressed("moveDown")) _velocity.Y += 1;

		if (_velocity.Length() > 0) {
			_velocity = _velocity.Normalized() * Speed;
		}
	}
}
