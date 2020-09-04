using Godot;

public class LeafManager : Node2D
{
	[Export]
	PackedScene LeafScene;

	Leaf[] Leaves;
	Leaf HeldLeaf;

	public override void _Ready()
	{
		Leaves = new Leaf[3];
		for(int x = 0; x < Leaves.Length; x++)
		{
			var leaf = LeafScene.Instance() as Leaf;
			leaf.GlobalPosition = new Vector2(100 * (x + 1), (100 * (x + 1)) / 2);
			AddChild(leaf);
			Leaves[x] = leaf;
			leaf.Connect("LeafClicked", this, "OnLeafClicked", new Godot.Collections.Array() { x });
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if(HeldLeaf != null)
		{
			HeldLeaf.GlobalPosition = GetGlobalMousePosition();

			if(!Input.IsActionPressed("interact"))
			{
				HeldLeaf = null;
			}
		}
	}

	void OnLeafClicked(int leafNumber)
	{
		if(HeldLeaf == null)
		{
			GD.Print($"Manger managing leaf: {leafNumber}");
			HeldLeaf = Leaves[leafNumber];
		}
	}
}
