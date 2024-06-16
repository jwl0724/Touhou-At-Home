using Godot;
using System;

public partial class Projectile : CharacterBody2D {
	public static int ProjectileCount {get; private set;} = 0;
	public AnimatedSprite2D Sprite;
	public int Damage {get; private set;}
	private float Speed;
	private float Lifespan;
	private float Angle;
	private float lifeTime = 0;

	// NOTE: Must be called after instantiating projectile since _Ready doesnt work for some reason
	public void AssignSprite() {
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public void SetProperties(float angle, float speed, float lifespan, int damage) {
		// connect exit signal to function
		GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D")
		.Connect("screen_exited", Callable.From(() => OnVisibleOnScreenNotifier2DScreenExited()));
		
		// set projectile properties
		ProjectileCount += 1;
		Angle = angle;
		Speed = speed;
		Lifespan = lifespan;
		Damage = damage;
		Sprite.Play();
	}

	public override void _PhysicsProcess(double delta) {
		// handle lifetime
		if (lifeTime > Lifespan) {
			QueueFree();
			ProjectileCount -= 1;
		} else lifeTime += (float) delta;

		Velocity = new Vector2(Speed, 0).Rotated(Angle);
		KinematicCollision2D collision = MoveAndCollide(Velocity * (float) delta);
		if (collision != null) QueueFree();
	}

	private void OnVisibleOnScreenNotifier2DScreenExited() {
		QueueFree();
		ProjectileCount -= 1;
	}
}
