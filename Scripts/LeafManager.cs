using Godot;

public class LeafManager : Node2D
{
	[Export]
	PackedScene LeafScene;

	Tree[] Trees;
	Leaf HeldLeaf;
	 
	public override void _Ready()
	{
		Trees = new Tree[3];
		Trees[0] = GetNode<Node2D>("Tree0") as Tree;

		for(int x = 0; x < Trees.Length; x++)
		{
			if (Trees[x] == null)
				continue;

			int count = 0;
			foreach (Node2D leafPos in Trees[x].GetChildren())
			{
				leafPos.GetNode<Sprite>("TestLeaf").Visible = false;

				var leaf = LeafScene.Instance() as Leaf;
				leafPos.AddChild(leaf);
				leaf.Connect("LeafClicked", this, "OnLeafClicked", new Godot.Collections.Array() { 0, count });
				Trees[0].AddLeaf(leaf);

				count++;
			}
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

	void OnLeafClicked(int treeNumber, int leafNumber)
	{
		if(HeldLeaf == null)
		{
			GD.Print($"Picking up leaf: {leafNumber}");
			HeldLeaf = Trees[treeNumber].GetLeaf(leafNumber);
		}
	}
}
