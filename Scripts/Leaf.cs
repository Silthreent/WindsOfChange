using Godot;
using System;

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
		// If we aren't falling from the sky, make sure we aren't below the viewport
		// Purge the leaf if we are
		// Otherwise, just the skip the rest of the method
		if (!IsSkyFalling)
		{
			if (Body.GlobalPosition.y >= GetViewport().Size.y + 50)
				QueueFree();

			return;
		}

		// The leaf has arrived into position, stop it from moving anymore and set it to true position
		if(Body.Position.DistanceTo(Vector2.Zero) <= 5)
		{
			Body.LinearVelocity = Vector2.Zero;
			Body.GravityScale = 0;

			var transform = (Transform2D)Physics2DServer.BodyGetState(Body.GetRid(), Physics2DServer.BodyState.Transform);
			transform.origin = new Vector2(0, 0) + GlobalPosition;
			Physics2DServer.BodySetState(Body.GetRid(), Physics2DServer.BodyState.Transform, transform);

			IsSkyFalling = false;

			EmitSignal("SkyLeafDropped");
		}
	}

	// Color the leaf the specified color
	public void SetColor(LeafColor color)
	{
		Color = color;

		Sprite.Modulate = color.GetRandomColor();
	}

	// A leaf is clicked, so make sure we aren't doing an animation then tell everyone
	void OnInputEvent(Node viewport, InputEvent input, int shapeID)
	{
		if (IsSkyFalling)
			return;

		if(input is InputEventMouseButton)
		{
			if(input.IsPressed())
			{
				EmitSignal("LeafClicked", ParentTree, ID);
			}
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
	static Random RNG;

	static LeafColorExtension()
	{
		RNG = new Random();
	}

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

	public static Color GetRandomColor(this LeafColor leafColor)
	{
		// Set starting color values
		var v = new Vector3(155, 155, 155);

		// The chosen color should be given more focus, so up it's color value
		switch(leafColor)
		{
			case LeafColor.Red:
				v.x += 100;
				break;

			case LeafColor.Green:
				v.y += 100;
				break;

			case LeafColor.Blue:
				v.z += 100;
				break;
		}

		// Modify each color randomly within the given threshold
		return new Color(
			RNG.Next((int)v.x - 50, (int)v.x) / 255f,
			RNG.Next((int)v.y - 50, (int)v.y) / 255f,
			RNG.Next((int)v.z - 50, (int)v.z) / 255f); ;
	}
}

