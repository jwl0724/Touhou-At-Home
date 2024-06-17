using Godot;
using System;

public partial class HUD : CanvasLayer {

	[Signal]
	public delegate void StartGameEventHandler();
	private Label messageLabel;
	private Label scoreLabel;
	private Label timerLabel;
	private Button button;
	private ProgressBar healthBar;

	public override void _Ready() {
		messageLabel = GetNode<Label>("MessageLabel");
		scoreLabel = GetNode<Label>("ScoreLabel");
		timerLabel = GetNode<Label>("TimeLabel");
		button = GetNode<Button>("Button");
		healthBar = GetNode<ProgressBar>("HealthBar");
		healthBar.MaxValue = Player.DefaultHealth;
		
		// connect signals
		button.Connect("pressed", Callable.From(() => OnButtonPressed()));
		ShowMainMenu();
	}

	// SIGNAL HANDLER SECTION
	private void OnButtonPressed() {
		EmitSignal(SignalName.StartGame);
	}

	// METHODS SECTION
	public void UpdateHealthBar(Player player) {
		healthBar.Value = player.Health;
	}

	public void UpdateScore(int score) {
		scoreLabel.Text = $"Score:{score}";
	}

	public void UpdateTime(int time) {
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		string formattedTime = timeSpan.ToString(@"mm\:ss");
		timerLabel.Text = formattedTime;
	}

	public void ShowMainMenu() {
		scoreLabel.Hide();
		timerLabel.Hide();
		healthBar.Hide();
		messageLabel.Text = "Touhou At Home";
		messageLabel.Show();
		button.Text = "Start";
		button.Show();
	}

	public void ShowGameUI() {
		scoreLabel.Show();
		timerLabel.Show();
		healthBar.Show();
		messageLabel.Hide();
		button.Hide();
	}

	public void ShowGameOver() {
		scoreLabel.Hide();
		timerLabel.Hide();
		healthBar.Hide();
		messageLabel.Text = $"Game Over\n\nTime:{timerLabel.Text}\n{scoreLabel.Text}";
		messageLabel.Show();
		button.Text = "Retry";
		button.Show();
	}
}
