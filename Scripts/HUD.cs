using Godot;
using System;

public partial class HUD : CanvasLayer {

	[Signal]
	public delegate void StartGameEventHandler();
	private Label messageLabel;
	private Label scoreLabel;
	private Label timerLabel;
	private ColorRect backgroundTint;
	private Button button;
	private ProgressBar healthBar;

	public override void _Ready() {
		messageLabel = GetNode<Label>("MessageLabel");
		scoreLabel = GetNode<Label>("ScoreLabel");
		timerLabel = GetNode<Label>("TimeLabel");
		button = GetNode<Button>("Button");
		healthBar = GetNode<ProgressBar>("HealthBar");
		backgroundTint = GetNode<ColorRect>("BackgroundTint");
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
		backgroundTint.Hide();
		messageLabel.Text = "Touhou At Home";
		messageLabel.Show();
		button.Text = "Start";
		button.Show();
	}

	public void ShowGameUI() {
		scoreLabel.Show();
		timerLabel.Show();
		healthBar.Show();
		backgroundTint.Hide();
		messageLabel.Hide();
		button.Hide();
	}

	public void ShowGameOver() {
		scoreLabel.Hide();
		timerLabel.Hide();
		healthBar.Hide();
		backgroundTint.Show();
		messageLabel.Text = $"Time:{timerLabel.Text}\n{scoreLabel.Text}\n\nGame Over";
		messageLabel.Show();
		button.Text = "Retry";
		button.Show();
	}
}
