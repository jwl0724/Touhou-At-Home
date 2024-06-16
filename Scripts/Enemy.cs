using Godot;
using System;
using System.Threading;

public partial class Enemy : RigidBody2D {
	
	// VARIABLES SECTION
	[Signal]
	public delegate void FireEnemyProjectileEventHandler();
	public static int EnemyCount {get; private set;} = 0;
	public int Health {get; set;}
	public float AttackCD {get; set;}
	public int Attack {get; set;}
	public AttackPattern AttackPattern {get; set;}
	private float attackTimer = 0;
	private static Vector2 defaultScale;
	private AnimatedSprite2D sprite;
	private CollisionShape2D hitbox;
	private const float stationaryTime = 3f;
	private float stationaryTimer = 0;
	private Vector2 leaveVelocity = Vector2.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D")
		.Connect("screen_exited", Callable.From(() => OnVisibleOnScreenNotifier2DScreenExited()));
		
		// activate detection of collisions
		ContactMonitor = true;
		Connect("body_entered", Callable.From((Projectile body) => OnBodyEntered(body)));

		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		hitbox = GetNode<CollisionShape2D>("CollisionShape2D");
		defaultScale = Scale;
		EnemyCount += 1;
	}

	private void OnVisibleOnScreenNotifier2DScreenExited() {
		EnemyCount -= 1;
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		ProcessAnimation();

		// handle enemy attack
		if (attackTimer >= AttackCD) {
			attackTimer = 0;
			EmitSignal(SignalName.FireEnemyProjectile);
		} else attackTimer += (float) delta;

		if (stationaryTimer > stationaryTime) {
			startMovement(delta);
		} else if (LinearVelocity.IsZeroApprox()) {
			stationaryTimer += (float) delta;
		} else {
			stopMovement(delta);
		}
	}

	// SIGNAL HANDLERS SECTION
	private void OnBodyEntered(Projectile projectile) {
		GD.Print("Hit");
		Health -= projectile.Damage;
		if (Health <= 0) {
			QueueFree();
			return;
		}
	}

	// METHODS SECTION
	public void SetRandomVelocity() {
		Vector2 velocity = new((float) GD.RandRange(150f, 250f), 0);
		float angle = (float) GD.RandRange(0, Math.PI);
		LinearVelocity = velocity.Rotated(angle);
	}

	private void startMovement(double delta) {
		if (leaveVelocity.IsZeroApprox()) 
			leaveVelocity = new((float) GD.RandRange(-150f, 150f), (float) GD.RandRange(-150f, 150f));
		
		LinearVelocity = new Vector2(
			LinearVelocity.X + leaveVelocity.X * 1f * (float) delta, 
			LinearVelocity.Y + leaveVelocity.Y * 1f * (float) delta
		);
	}

	private void stopMovement(double delta) {
		// slows down enemy till it stops
		float newVelocityX, newVelocityY, decceleration = 75f * (float) delta;

		// handle X component
		if (LinearVelocity.X > decceleration) {
			newVelocityX = LinearVelocity.X - decceleration;
		} else if (LinearVelocity.X < -decceleration) {
			newVelocityX = LinearVelocity.X + decceleration;
		} else {
			newVelocityX = 0;
		}
		// handle Y component
		if (LinearVelocity.Y > decceleration) {
			newVelocityY = LinearVelocity.Y - decceleration;
		} else if (LinearVelocity.Y < -decceleration) {
			newVelocityY = LinearVelocity.Y + decceleration;
		} else {
			newVelocityY = 0;
		}
		LinearVelocity = new Vector2(newVelocityX, newVelocityY);
	}

	private void ProcessAnimation() {
		
		// process X
		if (LinearVelocity.X < -75) {
			sprite.Animation = "left";
			if (sprite.Rotation > -0.2f) sprite.Rotate(-0.2f);
		} else if (LinearVelocity.X > 75) {
			sprite.Animation = "right";
			if (sprite.Rotation < 0.2f) sprite.Rotate(0.2f);
		} else {
			sprite.Animation = "default";
			sprite.Rotation = 0;
		}

		// process Y
		if (LinearVelocity.Y != 0) {
			Scale = new Vector2(defaultScale.X - 0.1f, defaultScale.Y + 0.2f);
		} else {
			Scale = defaultScale;
		}
	}
}
