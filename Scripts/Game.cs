using Godot;
using System;

public partial class Game : Node {
	[Export]
	public PackedScene EnemyScene {get; set;}
	[Export]
	public PackedScene ProjectileScene {get; set;}
	public Player Player;
	public Timer EnemyTimer;
	private float elapsedTime = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Player = GetNode<Player>("Player");
		EnemyTimer = GetNode<Timer>("EnemyTimer");
		Player.Connect("FirePlayerProjectile", Callable.From(() => OnFirePlayerProjectile()));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		elapsedTime += (float) delta;
	}

	// SIGNAL HANDLERS
	private void OnFirePlayerProjectile() {
		Projectile projectile = ProjectileScene.Instantiate<Projectile>();
		const int damage = 5;
		const float projectileLifespan = 3f;
		const float acceleration = 0;
		float projectileSpeed = Player.Speed * -2;
		Vector2 direction = new Vector2(0, 1);

		projectile.Start(direction, projectileSpeed, projectileLifespan, acceleration, damage);
		projectile.Position = Player.Position;
		projectile.Sprite.Animation = "playerProjectile";
		AddChild(projectile);
	}
}
