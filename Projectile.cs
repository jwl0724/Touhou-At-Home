using Godot;
using System;

public partial class Projectile : CharacterBody2D {

	private int Speed {get; set;} = 300;
	private float Lifespan {get; set;}
	private Vector2 Direction {get; set;}
	private float lifeTime = 0;

	public override void _PhysicsProcess(double delta) {
		Vector2 velocity = Velocity;

		if (Direction != Vector2.Zero) {
			velocity.X = Direction.X * Speed;
		} else {
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		if (lifeTime > Lifespan) {
			QueueFree();
		} else lifeTime += (float) delta;
		

		Velocity = velocity;
		MoveAndSlide();
	}

	private void OnVisibleOnScreenNotifier2DScreenExited() {
		QueueFree();
	}
}
