using Godot;
using System;

public partial class Player : Area2D {
	// VARIABLES SECTION
	[Export]
	public int Speed { get; set; } = 400;
	public Vector2 ScreenSize;
	public static Player Instance { get; private set; }
	private static Vector2 _defaultScale;
	private Vector2 _velocity = Vector2.Zero;
	private AnimatedSprite2D _sprite;
	private CollisionShape2D _hitbox;

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
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_hitbox = GetNode<CollisionShape2D>("CollisionShape2D");
		_defaultScale = Scale;
		InitializePlayer();
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

	// SIGNAL HANDLERS SECTION
	private void OnBodyEntered(Node2D body) {
		Hide();
		EmitSignal(SignalName.Hit);
		_hitbox.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	// METHODS SECTION
	private void InitializePlayer() {
		Show();
		Position = new Vector2(x: ScreenSize.X / 2, y: ScreenSize.Y / 2);
		_hitbox.Disabled = false;
	}

	private void ProcessAnimation() {
		// process X
		if (_velocity.X < 0) {
			_sprite.Animation = "left";
			if (_sprite.Rotation > -0.3f) _sprite.Rotate(-0.3f);
		} else if (_velocity.X > 0) {
			_sprite.Animation = "right";
			if (_sprite.Rotation < 0.3f) _sprite.Rotate(0.3f);
		} else {
			_sprite.Animation = "default";
			_sprite.Rotation = 0;
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
