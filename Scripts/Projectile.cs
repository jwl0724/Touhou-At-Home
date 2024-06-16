using Godot;
using System;

public partial class Projectile : CharacterBody2D {
	public static int projectileCount {get; private set;} = 0;
	public AnimatedSprite2D Sprite;
	public int Damage {get; private set;}
	private float Speed;
	private float Lifespan;
	private Vector2 Direction;
	private float Acceleration;
	private float lifeTime = 0;

	public void Start(Vector2 direction, float speed, float lifespan, float acceleration, int damage) {
		// connect exit signal to function
		GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D")
		.Connect("screen_exited", Callable.From(() => OnVisibleOnScreenNotifier2DScreenExited()));
		
		// set projectile properties
		projectileCount += 1;
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Direction = direction;
		Speed = speed;
		Lifespan = lifespan;
		Acceleration = acceleration;
		Damage = damage;
		Sprite.Play();
	}

	public override void _PhysicsProcess(double delta) {
		// handle lifetime
		if (lifeTime > Lifespan) {
			QueueFree();
			projectileCount -= 1;
		} else lifeTime += (float) delta;

		Velocity = new Vector2(Speed, 0).Rotated(Direction.Angle());
		KinematicCollision2D collision = MoveAndCollide(Velocity * (float) delta);
		if (collision != null) QueueFree();
	}

	private void OnVisibleOnScreenNotifier2DScreenExited() {
		QueueFree();
		projectileCount -= 1;
	}
}
