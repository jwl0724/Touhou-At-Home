using Godot;
using System;

public partial class HUD : CanvasLayer {

	[Signal]
	public delegate void StartGameEventHandler();
	private Label messageLabel;
	private Label scoreLabel;
	private Label timerLabel;
	private Button button;

	public override void _Ready() {
		messageLabel = GetNode<Label>("MessageLabel");
		scoreLabel = GetNode<Label>("ScoreLabel");
		timerLabel = GetNode<Label>("TimerLabel");
		button = GetNode<Button>("Button");
		button.Connect("pressed", Callable.From(() => OnButtonPressed()));
		ShowMainMenu();
	}

	public override void _Process(double delta) {

	}

	// SIGNAL HANDLER SECTION
	private void OnButtonPressed() {
		EmitSignal(SignalName.StartGame);
	}

	// METHODS SECTION
	public void UpdateScore(int score) {
		scoreLabel.Text = score.ToString();
	}

	public void UpdateTime(int time) {
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		string formattedTime = timeSpan.ToString(@"mm\:ss");
		timerLabel.Text = formattedTime;
	}

	public void ShowMainMenu() {
		scoreLabel.Hide();
		timerLabel.Hide();
		messageLabel.Text = "Touhou At Home";
		messageLabel.Show();
		button.Text = "Start";
		button.Show();
	}

	public void ShowGameUI() {
		scoreLabel.Show();
		timerLabel.Show();
		messageLabel.Hide();
		button.Hide();
	}

	public void ShowGameOver() {
		scoreLabel.Hide();
		timerLabel.Hide();
		messageLabel.Text = $"Game Over\nTime:{timerLabel.Text}\n{scoreLabel.Text}";
		messageLabel.Show();
		button.Text = "Retry";
		button.Show();
	}
}
