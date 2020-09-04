using Godot;
using System;

public class LeafManager : Node2D
{
	[Export]
	PackedScene LeafScene;

	Tree[] Trees;
	Leaf HeldLeaf;
	Random RNG;
	 
	public override void _Ready()
	{
		RNG = new Random();

		Trees = new Tree[2];

		for (int x = 0; x < Trees.Length; x++)
		{
			Trees[x] = GetNode<Tree>("Tree" + x);

			if (Trees[x] == null)
				continue;

			int count = 0;
			foreach (Node2D leafPos in Trees[x].GetChildren())
			{
				leafPos.GetNode<Sprite>("TestLeaf").Visible = false;

				var leaf = LeafScene.Instance() as Leaf;
				leafPos.AddChild(leaf);
				leaf.Connect("LeafClicked", this, "OnLeafClicked");
				Trees[x].AddLeaf(leaf);
				leaf.ParentTree = x;

				leaf.SetColor((LeafColor)RNG.Next(0, Enum.GetValues(typeof(LeafColor)).Length));
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
				float WinningDistance = 9999;
				Leaf WinningLeaf = null;

				foreach(var x in HeldLeaf.GetOverlappingAreas())
				{
					var leaf = x as Leaf;
					if(leaf != null)
					{
						var distance = HeldLeaf.GlobalPosition.DistanceTo(leaf.GlobalPosition);

						if (WinningLeaf == null || distance < WinningDistance)
						{
							WinningLeaf = leaf;
							WinningDistance = distance;
						}
					}
				}

				// TODO: Have leaf remember where it came from and make sure it's not being set down in the same place
				GD.Print($"Closest leaf is {WinningLeaf.ID} of tree {WinningLeaf.ParentTree}");
				Trees[HeldLeaf.ParentTree].RemoveLeaf(HeldLeaf);
				Trees[WinningLeaf.ParentTree].RemoveLeaf(WinningLeaf);

				Trees[HeldLeaf.ParentTree].AddLeaf(WinningLeaf);
				Trees[WinningLeaf.ParentTree].AddLeaf(HeldLeaf);

				var parent = HeldLeaf.GetParent();
				var parentTree = HeldLeaf.ParentTree;

				HeldLeaf.GetParent().RemoveChild(HeldLeaf);
				WinningLeaf.GetParent().AddChild(HeldLeaf);

				WinningLeaf.GetParent().RemoveChild(WinningLeaf);
				parent.AddChild(WinningLeaf);

				HeldLeaf.Position = Vector2.Zero;

				HeldLeaf.ParentTree = WinningLeaf.ParentTree;
				WinningLeaf.ParentTree = parentTree;

				HeldLeaf = null;
			}
		}
	}

	void OnLeafClicked(int treeNumber, int leafID)
	{
		if(HeldLeaf == null)
		{
			GD.Print($"Picking up leaf: {leafID}");
			HeldLeaf = Trees[treeNumber].GetLeaf(leafID);
		}
	}
}
