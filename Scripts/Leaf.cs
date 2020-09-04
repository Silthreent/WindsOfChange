using Godot;

public class Leaf : Area2D
{
	[Signal]
	delegate void LeafClicked(int tree, int id);

	public int ParentTree { get; set;  }
	public int ID { get; set; }

	static int IDCount = 0;

	public Leaf()
	{
		ID = IDCount++;
	}

	public override void _Ready()
	{
		Connect("input_event", this, "OnInputEvent");
	}

	void OnInputEvent(Node viewport, InputEvent input, int shapeID)
	{
		if (input.IsActionPressed("interact"))
		{
			EmitSignal("LeafClicked", ParentTree, ID);
		}
	}
}
