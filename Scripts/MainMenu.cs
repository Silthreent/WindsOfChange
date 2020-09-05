using Godot;
using System;

public class MainMenu : MarginContainer
{
	Button PlayButton;
	CheckButton StressFree;

	public override void _Ready()
	{
		PlayButton = FindNode("PlayButton") as Button;
		PlayButton.Connect("pressed", this, "OnPlayPressed");

		StressFree = FindNode("StressFreeToggle") as CheckButton;
	}

	void OnPlayPressed()
	{
		GD.Print("Starting game");
		var game = ResourceLoader.Load<PackedScene>("Scenes/Game.tscn").Instance() as GameManager;
		GetTree().Root.AddChild(game);
		game.ToggleStressFreeMode(StressFree.Pressed);
		QueueFree();
	}
}
