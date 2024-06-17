using Godot;
using System;

public partial class Game : Node {
	[Export]
	public PackedScene EnemyScene {get; set;}
	[Export]
	public PackedScene ProjectileScene {get; set;}
	public Player Player;
	public HUD Hud;
	public PathFollow2D EnemySpawnPoint;
	public Timer EnemyTimer;
	public Timer StartTimer;
	public static bool Paused {get; private set;} = true;
	private float elapsedTime = 0;
	private int score = 0;
	private readonly Random rng = new();
	private const int maxEnemyCount = 100;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Player = GetNode<Player>("Player");
		Hud = GetNode<HUD>("HUD");
		EnemyTimer = GetNode<Timer>("EnemyTimer");
		StartTimer = GetNode<Timer>("StartTimer");
		EnemySpawnPoint = GetNode<PathFollow2D>("EnemyPath/EnemySpawnLocation");
		
		// connect signals
		Hud.Connect("StartGame", Callable.From(() => StartGame()));
		Player.Connect("FirePlayerProjectile", Callable.From(() => OnFirePlayerProjectile()));
		Player.Connect("PlayerDied", Callable.From(() => OnPlayerDied()));
		Player.Connect("Hit", Callable.From(() => OnPlayerHit()));
		EnemyTimer.Connect("timeout", Callable.From(() => OnEnemyTimeout()));
		StartTimer.Connect("timeout", Callable.From(() => OnStartTimeout()));

		Hud.ShowMainMenu();
	}

	public override void _Process(double delta) {
		elapsedTime += (float) delta;
		Hud.UpdateTime((int) elapsedTime);
	}

	// SIGNAL HANDLERS
	private void OnEnemyKilled() {
		const int baseScore = 10;
		score += CalculateStat(baseScore, 10);
		Hud.UpdateScore(score);
	}

	private void OnPlayerHit() {
		Hud.UpdateHealthBar(Player);
	}

	private void OnPlayerDied() {
		StopGame();
	}

	private void OnStartTimeout() {
		EnemyTimer.Start();
	}

	private void OnEnemyTimeout() {
		if (Enemy.EnemyCount >= maxEnemyCount) return;
		int spawnCount = rng.Next(CalculateStat(5, 5)) + 1;
		for (int i = 0; i < spawnCount; i++) {
			if (Enemy.EnemyCount >= maxEnemyCount) return;

			// create enemy and set properties
			Enemy enemy = EnemyScene.Instantiate<Enemy>();
			RandomizeEnemyProperties(enemy);

			// choose random spawn location
			EnemySpawnPoint.ProgressRatio = GD.Randf();
			enemy.Position = EnemySpawnPoint.Position;
			enemy.SetRandomVelocity();
			AddChild(enemy);

			// connect signals
			enemy.Connect("FireEnemyProjectile", Callable.From(() => OnFireEnemyProjectile(enemy)));
			enemy.Connect("EnemyKilled", Callable.From(() => OnEnemyKilled()));
		}
	}

	private void OnFireEnemyProjectile(Enemy enemy) {
		if (Projectile.ProjectileCount >= 600) return;

		// set projectile variables for enemy
		const float projectileLifespan = 10f;
		const float projectileSpeed = 90f;
		int damage = enemy.Attack;
		int projectileCount;
		if (enemy.AttackPattern == AttackPattern.LINE) projectileCount = 1;
		else if (enemy.AttackPattern == AttackPattern.CONE) projectileCount = 3;
		else projectileCount = 8; // set projectile count for circle 

		for (int i = 0; i < projectileCount; i++) {
			Projectile projectile = ProjectileScene.Instantiate<Projectile>();
			projectile.AssignSprite();
			projectile.Position = enemy.Position;
			projectile.Sprite.Animation = "enemyProjectile";

			// handle attack pattern projectile directionality
			if (enemy.AttackPattern == AttackPattern.LINE) {
				float direction = (float) Math.PI / 2;
				projectile.SetProperties(direction, projectileSpeed, projectileLifespan, damage);
			} else if (enemy.AttackPattern == AttackPattern.CONE) {
				float direction = (float) Math.PI * (2 + i) / 6;
				projectile.SetProperties(direction, projectileSpeed, projectileLifespan, damage);
			} else if (enemy.AttackPattern == AttackPattern.CIRCLE) {
				float direction = (float) Math.PI / 4 * i;
				projectile.SetProperties(direction, projectileSpeed, projectileLifespan, damage);
			}
			AddChild(projectile);
		}
	}

	private void OnFirePlayerProjectile() {
		const int projectileCount = 3;
		for (int i = 0; i < projectileCount; i++) {
			Projectile projectile = ProjectileScene.Instantiate<Projectile>();
			projectile.AssignSprite();
			const int damage = 20;
			const float projectileLifespan = 3f;
			float direction = (float) Math.PI * (11 + i) / 24;
			float projectileSpeed = Player.Speed * -2;

			projectile.SetProperties(direction, projectileSpeed, projectileLifespan, damage);
			projectile.Position = Player.Position;
			projectile.Sprite.Animation = "playerProjectile";
			projectile.Rotation = projectile.Velocity.Angle();
			AddChild(projectile);
		}

	}

	// METHODS SECTION
	private void StopGame() {
		Paused = true;
		EnemyTimer.Stop();
		StartTimer.Stop();
		Hud.ShowGameOver();

		// stop scrolling background
	 	(GetNode<TextureRect>("Background").Material as ShaderMaterial).SetShaderParameter("stop", true);
	}

	public void StartGame() {
		// clear objects from previous session
		SceneTree tree = GetTree();
		tree.CallGroup("Enemies", Node.MethodName.QueueFree);
		tree.CallGroup("Projectiles", Node.MethodName.QueueFree);

		StartTimer.Start();
		elapsedTime = 0;
		score = 0;
		Player.InitializePlayer();
		Hud.UpdateHealthBar(Player);
		Hud.UpdateTime((int) elapsedTime);
		Hud.UpdateScore(score);
		Hud.ShowGameUI();
		Paused = false;

		// start scrolling background
		(GetNode<TextureRect>("Background").Material as ShaderMaterial).SetShaderParameter("stop", false);
	}

	private void RandomizeEnemyProperties(Enemy enemy) {
		const int maxFactor = 2;
		const int baseAttack = 30;
		const int baseHealth = 200;
		int attackPatternCount = Enum.GetValues<AttackPattern>().Length;
		AttackPattern attackPattern = Enum.GetValues<AttackPattern>()[rng.Next(attackPatternCount)];
		
		enemy.AttackCD = (float) GD.RandRange(0.75f, 2.5f);
		enemy.AttackPattern = attackPattern;
		enemy.Health = CalculateStat(baseHealth, maxFactor);
		enemy.Attack = CalculateStat(baseAttack, maxFactor);
	}

	private int CalculateStat(int baseStat, int maxFactor) {
		return maxFactor * baseStat * (int) elapsedTime / (int) Math.Sqrt(120 * elapsedTime + Math.Pow(elapsedTime, 2));
	}
}
