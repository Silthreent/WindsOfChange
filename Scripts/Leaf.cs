using Godot;

public class Leaf : Area2D
{
	[Signal]
	delegate void LeafClicked(int tree, int id);

	public int ParentTree { get; set;  }
	public int ID { get; set; }

	static int IDCount = 0;

	LeafColor Color;
	Sprite Sprite;

	public Leaf()
	{
		ID = IDCount++;
	}

	public override void _Ready()
	{
		Sprite = GetNode<Sprite>("Sprite");

		Connect("input_event", this, "OnInputEvent");
	}

	public void SetColor(LeafColor color)
	{
		Color = color;

		Sprite.Modulate = color.GetLeafColor();
	}

	void OnInputEvent(Node viewport, InputEvent input, int shapeID)
	{
		if (input.IsActionPressed("interact"))
		{
			EmitSignal("LeafClicked", ParentTree, ID);
		}
	}
}

public enum LeafColor
{
	Red,
	Green,
	Blue,
	None
}

public static class LeafColorExtension
{
	public static Color GetLeafColor(this LeafColor leafColor)
	{
		switch(leafColor)
		{
			case LeafColor.Red:
				return new Color(255, 0, 0);

			case LeafColor.Green:
				return new Color(0, 255, 0);

			case LeafColor.Blue:
				return new Color(0, 0, 255);

			default:
				return new Color(0, 0, 0);
		}
	}
}

