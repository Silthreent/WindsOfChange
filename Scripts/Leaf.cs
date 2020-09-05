using Godot;

public class Leaf : Area2D
{
	[Signal]
	delegate void LeafClicked(int tree, int id);
	[Signal]
	delegate void SkyLeafDropped();

	static int IDCount = 0;

	public int ParentTree { get; set;  }
	public int ID { get; set; }
	public LeafColor Color { get; protected set; }
	public RigidBody2D Body { get; protected set; }
	public bool IsSkyFalling { get; set; } = false;

	AnimatedSprite Sprite;

	public Leaf()
	{
		ID = IDCount++;
	}

	public override void _Ready()
	{
		Sprite = FindNode("Sprite") as AnimatedSprite;
		Body = GetNode<RigidBody2D>("Body");

		Connect("input_event", this, "OnInputEvent");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!IsSkyFalling)
		{
			if (Body.GlobalPosition.y >= GetViewport().Size.y + 50)
				QueueFree();

			return;
		}

		if(Body.Position.DistanceTo(Vector2.Zero) <= 5)
		{
			Body.LinearVelocity = Vector2.Zero;
			Body.GravityScale = 0;

			Body.Position = Vector2.Zero;

			IsSkyFalling = false;

			EmitSignal("SkyLeafDropped");
		}
	}

	public void SetColor(LeafColor color)
	{
		Color = color;

		Sprite.Modulate = color.GetLeafColor();
	}

	void OnInputEvent(Node viewport, InputEvent input, int shapeID)
	{
		if (IsSkyFalling)
			return;

		if (input.IsActionPressed("interact"))
		{
			EmitSignal("LeafClicked", ParentTree, ID);
		}
	}
}

public enum LeafColor
{
	None,
	Red,
	Green,
	Blue
}

public static class LeafColorExtension
{
	public static Color GetLeafColor(this LeafColor leafColor)
	{
		switch(leafColor)
		{
			case LeafColor.Red:
				return new Color(1, 0, 0);

			case LeafColor.Green:
				return new Color(0, 1, 0);

			case LeafColor.Blue:
				return new Color(0, 0, 1);

			default:
				return new Color(0, 0, 0);
		}
	}
}

