using Godot;

public class MainMenu : MarginContainer
{
	Button PlayButton;
	CheckButton StressFree;
	LeafManager Leaves;

	float Timer;
	bool TimerRunning = false;

	public override void _Ready()
	{
		PlayButton = FindNode("PlayButton") as Button;
		PlayButton.Connect("pressed", this, "OnPlayPressed");

		StressFree = FindNode("StressFreeToggle") as CheckButton;

		Leaves = FindNode("LeafManager") as LeafManager;
		Leaves.SkipFirstLevel();
		Leaves.GenerateLeaves();
		Leaves.Connect("LeavesDropped", this, "OnLeavesDropped");
		Leaves.Connect("SkyLeavesDropped", this, "OnSkyLeavesDropped");
	}

	public override void _PhysicsProcess(float delta)
	{
		// Manage the tree on the home screen
		// Once the timer ticks down, drop it's leaves and spawn new ones once those are done
		if (!TimerRunning)
			return;

		Timer -= delta;
		if(Timer <= 0)
		{
			TimerRunning = false;
			Leaves.DropLeaves();
		}
	}

	void OnPlayPressed()
	{
		// Start the game
		GD.Print("Starting game");
		var game = ResourceLoader.Load<PackedScene>("Scenes/Game.tscn").Instance() as GameManager;
		GetTree().Root.AddChild(game);
		game.ToggleStressFreeMode(StressFree.Pressed);
		QueueFree();
	}

	void OnLeavesDropped()
	{
		// The leaves were done dropping, so spawn the next set
		Leaves.GenerateLeaves();
	}

	void OnSkyLeavesDropped()
	{
		// The new set is done spawning, so start the timer again
		Timer = 2.5f;
		TimerRunning = true;
	}
}
