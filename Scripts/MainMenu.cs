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
		Leaves.GenerateLeaves(new LeafColor[] { LeafColor.Green, LeafColor.Blue, LeafColor.Red });
		Leaves.Connect("LeavesDropped", this, "OnLeavesDropped");
		Leaves.Connect("SkyLeavesDropped", this, "OnSkyLeavesDropped");
	}

	public override void _PhysicsProcess(float delta)
	{
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
		GD.Print("Starting game");
		var game = ResourceLoader.Load<PackedScene>("Scenes/Game.tscn").Instance() as GameManager;
		GetTree().Root.AddChild(game);
		game.ToggleStressFreeMode(StressFree.Pressed);
		QueueFree();
	}

	void OnLeavesDropped()
	{
		Leaves.GenerateLeaves(new LeafColor[] { LeafColor.Green, LeafColor.Blue });
	}

	void OnSkyLeavesDropped()
	{
		Timer = 2.5f;
		TimerRunning = true;
	}
}
