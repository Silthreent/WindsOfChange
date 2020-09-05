using Godot;
using System;

public class MainMenu : MarginContainer
{
	Button PlayButton;

	public override void _Ready()
	{
		PlayButton = FindNode("PlayButton") as Button;
		PlayButton.Connect("pressed", this, "OnPlayPressed");
	}

	void OnPlayPressed()
	{
		GD.Print("Starting game");
		var game = ResourceLoader.Load<PackedScene>("Scenes/Game.tscn").Instance() as GameManager;
		GetTree().Root.AddChild(game);
		QueueFree();
	}
}
