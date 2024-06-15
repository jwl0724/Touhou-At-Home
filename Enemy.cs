using Godot;
using System;

public partial class Enemy : RigidBody2D {
	// VARIABLES SECTION
	private int health {get; set;}
	private float attackCD {get; set;}
	private float attackTimer = 0;
	private static Vector2 defaultScale;
	private AnimatedSprite2D sprite;
	private CollisionShape2D hitbox;
	private int speed;
	private Vector2 velocity = Vector2.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		sprite = GetNode<AnimatedSprite2D>("AniamtedSprite2D");
		hitbox = GetNode<CollisionShape2D>("CollisionShape2D");
		defaultScale = Scale;
	}

	private void OnVisibleOnScreenNotifier2DScreenExited() {
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		ProcessAnimation();
	}

	// METHODS SECTION

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
}
