using Godot;
using System;

public class LeafManager : Node2D
{
	[Signal]
	delegate void SkyLeavesDropped();
	[Signal]
	delegate void LeafMoved(Leaf clickedLeaf, Leaf landedLeaf);
	[Signal]
	delegate void LeavesDropped();

	[Export]
	PackedScene LeafScene;
	public bool Paused { get; set; } = false;

	Tree[] Trees;
	Leaf HeldLeaf;
	Random RNG;

	int TreesDropping = 0;
	int SkyLeavesDropping = 0;
	
	public override void _Ready()
	{
		RNG = new Random();

		Trees = new Tree[3];

		for (int x = 0; x < Trees.Length; x++)
		{
			Trees[x] = GetNode<Tree>("Tree" + x);
			Trees[x].Connect("LeavesDropped", this, "OnLeavesDropped");

			if (Trees[x] == null)
				continue;

			foreach (Node2D leafPos in Trees[x].GetChildren())
			{
				leafPos.GetNode<Sprite>("TestLeaf").Visible = false;
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
				float WinningDistance = HeldLeaf.GlobalPosition.DistanceTo((HeldLeaf.GetParent() as Node2D).GlobalPosition);
				Leaf WinningLeaf = null;

				foreach(var x in HeldLeaf.GetOverlappingAreas())
				{
					var leaf = x as Leaf;
					if(leaf != null)
					{
						var distance = HeldLeaf.GlobalPosition.DistanceTo(leaf.GlobalPosition);

						if (distance < WinningDistance)
						{
							WinningLeaf = leaf;
							WinningDistance = distance;
						}
					}
				}

				if(WinningLeaf != null)
				{
					GD.Print($"Closest leaf is {WinningLeaf.ID} of tree {WinningLeaf.ParentTree}");
					SwapLeaves(HeldLeaf, WinningLeaf);
				}
				else
				{
					GD.Print("Placing leaf back");
					HeldLeaf.Position = Vector2.Zero;
				}

				HeldLeaf = null;
			}
		}
	}

	public void GenerateLeaves(LeafColor[] colors)
	{
		for (int x = 0; x < Trees.Length; x++)
		{
			if (Trees[x] == null)
				continue;

			TreesDropping++;

			foreach (Node2D leafPos in Trees[x].GetChildren())
			{
				var leaf = LeafScene.Instance() as Leaf;
				leafPos.AddChild(leaf);
				leaf.Connect("LeafClicked", this, "OnLeafClicked");
				leaf.Connect("SkyLeafDropped", this, "OnSkyLeafDropped");

				Trees[x].AddLeaf(leaf);

				leaf.ParentTree = x;
				leaf.Body.GlobalPosition = new Vector2(leaf.GlobalPosition.x, RNG.Next(-150, -50));
				leaf.Body.GravityScale = 2;
				leaf.IsSkyFalling = true;

				SkyLeavesDropping++;
			}
		}

		int leafCount = 0;
		for(int color = 0; color < colors.Length; color++)
		{
			for (int x = 0; x < Trees[0].GetLeafCount(); x++)
			{
				Trees[leafCount % Trees.Length].ColorFirstAvailableLeaf(colors[color], RNG.Next(0, Trees[leafCount % Trees.Length].GetLeafCount()));

				leafCount++;
			}
		}
	}

	public void DropLeaves()
	{
		foreach(var x in Trees)
		{
			x.DropLeaves();
		}

		HeldLeaf = null;
		Paused = true;
	}

	public Tree GetTree(int treeID)
	{
		if (treeID >= Trees.Length)
			return null;

		return Trees[treeID];
	}

	void SwapLeaves(Leaf leaf1, Leaf leaf2)
	{
		Trees[leaf1.ParentTree].RemoveLeaf(leaf1);
		Trees[leaf2.ParentTree].RemoveLeaf(leaf2);

		Trees[leaf1.ParentTree].AddLeaf(leaf2);
		Trees[leaf2.ParentTree].AddLeaf(leaf1);

		var parent = leaf1.GetParent();
		var parentTree = leaf1.ParentTree;

		leaf1.GetParent().RemoveChild(leaf1);
		leaf2.GetParent().AddChild(leaf1);

		leaf2.GetParent().RemoveChild(leaf2);
		parent.AddChild(leaf2);

		leaf1.Position = Vector2.Zero;

		leaf1.ParentTree = leaf2.ParentTree;
		leaf2.ParentTree = parentTree;

		EmitSignal("LeafMoved", leaf1, leaf2);
	}

	void OnLeafClicked(int treeNumber, int leafID)
	{
		if (Paused)
			return;

		if(HeldLeaf == null)
		{
			GD.Print($"Picking up leaf: {leafID}");
			HeldLeaf = Trees[treeNumber].GetLeafByID(leafID);
		}
	}

	void OnSkyLeafDropped()
	{
		SkyLeavesDropping--;
		if(SkyLeavesDropping <= 0)
		{
			EmitSignal("SkyLeavesDropped");
			Paused = false;
		}
	}

	void OnLeavesDropped()
	{
		TreesDropping--;
		if(TreesDropping <= 0)
		{
			EmitSignal("LeavesDropped");
		}
	}
}
