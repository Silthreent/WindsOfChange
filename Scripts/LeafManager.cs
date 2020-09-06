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

	AudioStreamPlayer[] LeafRustle;

	bool FirstLevel = true;

	public override void _Ready()
	{
		RNG = new Random();

		Trees = new Tree[3];

		// Find every tree and save to them to an array
		// Also hide it's test leaves and connect to signals
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

		LeafRustle = new AudioStreamPlayer[3];
		LeafRustle[0] = GetNode<AudioStreamPlayer>("LeafRustle0");
		LeafRustle[1] = GetNode<AudioStreamPlayer>("LeafRustle1");
		LeafRustle[2] = GetNode<AudioStreamPlayer>("LeafRustle2");
	}

	public override void _PhysicsProcess(float delta)
	{
		// If we're holding a leaf, we need to have it track our cursor
		// Also place it if we let go
		if(HeldLeaf != null)
		{
			HeldLeaf.GlobalPosition = GetGlobalMousePosition();

			// Button isn't being held, so find the place to set our leaf down
			if(!Input.IsMouseButtonPressed(1))
			{
				// Compare the distance of any nearby leaves to find the closest
				// The starting distance will be our parent position, where the leaf is located on the tree
				float WinningDistance = HeldLeaf.GlobalPosition.DistanceTo((HeldLeaf.GetParent() as Node2D).GlobalPosition);
				Leaf WinningLeaf = null;

				// Go over each nearby leaf and check how far it is
				foreach(var x in HeldLeaf.GetOverlappingAreas())
				{
					var leaf = x as Leaf;
					if(leaf != null)
					{
						var distance = HeldLeaf.GlobalPosition.DistanceTo(leaf.GlobalPosition);

						// If the leaf is closer to this leaf, then save that new distance and leaf
						if (distance < WinningDistance)
						{
							WinningLeaf = leaf;
							WinningDistance = distance;
						}
					}
				}

				// If we found a leaf, then swap positions with it
				if(WinningLeaf != null)
				{
					GD.Print($"Closest leaf is {WinningLeaf.ID} of tree {WinningLeaf.ParentTree}");
					SwapLeaves(HeldLeaf, WinningLeaf);
				}
				// If we didn't find a leaf, that means our home location is the closest so just set it back
				else
				{
					GD.Print("Placing leaf back");
					HeldLeaf.Position = Vector2.Zero;
				}

				LeafRustle[RNG.Next(0, LeafRustle.Length)].Play();
				HeldLeaf = null;
			}
		}
	}

	public void GenerateLeaves()
	{
		// Create a new set of leaves for every tree
		for (int x = 0; x < Trees.Length; x++)
		{
			// If this tree doesn't exist for some reason, just skip it
			if (Trees[x] == null)
				continue;

			// Keep track of how many trees have leaves to drop for signals later
			TreesDropping++;

			// Go through every possible location for a leaf in the tree and create a leaf
			foreach (Node2D leafPos in Trees[x].GetChildren())
			{
				// Create a new leaf, add it to it's position, and connect to signals
				var leaf = LeafScene.Instance() as Leaf;
				leafPos.AddChild(leaf);
				leaf.Connect("LeafClicked", this, "OnLeafClicked");
				leaf.Connect("SkyLeafDropped", this, "OnSkyLeafDropped");

				// Add the leaf to the tree so it can keep track of it
				Trees[x].AddLeaf(leaf);

				// Set the leaf's parent tree, and prepare it for dropping
				leaf.ParentTree = x;
				leaf.Body.GlobalPosition = new Vector2(leaf.GlobalPosition.x, RNG.Next(-150, -50));
				leaf.Body.GravityScale = 2;
				leaf.IsSkyFalling = true;

				// Keep track of how many leaves are dropping for the signals
				SkyLeavesDropping++;
			}
		}

		// Go through all the new leaves and color them
		// The first level has special simple coloring, for a tutorial of sorts
		// You might say "but silth, couldn't we do that at the same time as spawning them?"
		// And to that I say, yes
		if(!FirstLevel)
		{
			int leafCount = 0;
			for (int color = 1; color < Enum.GetValues(typeof(LeafColor)).Length; color++)
			{
				for (int x = 0; x < Trees[0].GetLeafCount(); x++)
				{
					Trees[leafCount % Trees.Length].ColorFirstAvailableLeaf((LeafColor)color, RNG.Next(0, Trees[leafCount % Trees.Length].GetLeafCount()));

					leafCount++;
				}
			}
		}
		// If it's the first level, make a custom level with only 2 leaves out of place
		else
		{
			for(int x = 0; x < Trees.Length; x++)
			{
				for (int y = 0; y < Trees[x].GetLeafCount(); y++)
				{
					Trees[x].GetLeafByIndex(y).SetColor((LeafColor)x + 1);
				}
			}

			Trees[0].GetLeafByIndex(RNG.Next(0, Trees[0].GetLeafCount())).SetColor((LeafColor)2);
			Trees[1].GetLeafByIndex(RNG.Next(0, Trees[1].GetLeafCount())).SetColor((LeafColor)1);

			FirstLevel = false;
		}
	}

	// Tell each leaf to drop off the tree
	public void DropLeaves()
	{
		foreach(var x in Trees)
		{
			x.DropLeaves();
		}

		HeldLeaf = null;
		Paused = true;
	}

	// Return whichever tree is asked for
	public Tree GetTree(int treeID)
	{
		if (treeID >= Trees.Length)
			return null;

		return Trees[treeID];
	}

	public void SkipFirstLevel()
	{
		FirstLevel = false;
	}

	// Swap two leaves
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

	// When a leaf is clicked, pick it up unless the game is paused
	void OnLeafClicked(int treeNumber, int leafID)
	{
		if (Paused)
			return;

		if(HeldLeaf == null)
		{
			GD.Print($"Picking up leaf: {leafID}");
			LeafRustle[RNG.Next(0, LeafRustle.Length)].Play();
			HeldLeaf = Trees[treeNumber].GetLeafByID(leafID);
		}
	}

	// A leaf finished falling from the sky, count down and if they all fell tell everyone
	void OnSkyLeafDropped()
	{
		SkyLeavesDropping--;
		if(SkyLeavesDropping <= 0)
		{
			EmitSignal("SkyLeavesDropped");
			Paused = false;
		}
	}

	// A leaf finished dropping off a tree, count down and tell everyone if that was the last one
	void OnLeavesDropped()
	{
		TreesDropping--;
		if(TreesDropping <= 0)
		{
			EmitSignal("LeavesDropped");
		}
	}
}
