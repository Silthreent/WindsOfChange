using Godot;

public class Leaf : Area2D
{
	[Signal]
	delegate void LeafClicked();

	public override void _Ready()
	{
		Connect("input_event", this, "OnInputEvent");
	}

	void OnInputEvent(Node viewport, InputEvent input, int shapeID)
	{
		if (input.IsActionPressed("interact"))
		{
			GD.Print("Leaf clicked");
			EmitSignal("LeafClicked");
		}
	}
}
