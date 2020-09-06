using Godot;
using System.Collections.Generic;

public class Tree : Sprite
{
	[Signal]
	delegate void LeavesDropped();

	List<Leaf> Leaves;

	float DropWait = .15f;
	float DropTimer;
	int FallAtOnce = 3;
	Vector2 FallImpulse = new Vector2(200, 70);

	public Tree()
	{
		Leaves = new List<Leaf>();
	}

	public override void _Ready()
	{
		SetPhysicsProcess(false);
	}

	// This should only be running if the tree is dropping it's leaves
	public override void _PhysicsProcess(float delta)
	{
		// If we have any leaves left to drop, think about dropping one
		if(Leaves.Count > 0)
		{
			DropTimer -= delta;

			// Time's up, time to drop another leaf
			if(DropTimer <= 0)
			{
				// Drop some amount of leaves at random, giving them different impulses
				for(int x = 0; x < FallAtOnce; x++)
				{
					Leaves[0].Body.ApplyImpulse(Vector2.Zero, FallImpulse * ((x + 1) / 1));
					Leaves[0].Monitorable = false;
					Leaves[0].Monitoring = false;
					Leaves.RemoveAt(0);

					if (Leaves.Count <= 0)
						break;
				}

				DropTimer = DropWait;
			}
		}
		// No more leaves to drop, so we must be done time to tell everyone
		else
		{
			EmitSignal("LeavesDropped");

			SetPhysicsProcess(false);
		}
	}

	public void AddLeaf(Leaf leaf)
	{
		Leaves.Add(leaf);
	}

	public void RemoveLeaf(Leaf leaf)
	{
		Leaves.Remove(leaf);
	}

	// Find the first empty leaf, starting from an index, and color it the given color
	public void ColorFirstAvailableLeaf(LeafColor color, int starting)
	{
		if (starting >= Leaves.Count)
			starting = 0;

		if(Leaves[starting].Color == LeafColor.None)
		{
			if (Leaves[starting].Color != color)
			{
				Leaves[starting].SetColor(color);
				return;
			}
		}

		ColorFirstAvailableLeaf(color, starting + 1);
	}

	// Start dropping every leaf off the tree
	public void DropLeaves()
	{
		foreach(var x in Leaves)
		{
			x.Body.GravityScale = 1;
		}

		DropTimer = DropWait;
		SetPhysicsProcess(true);
	}

	public Leaf GetLeafByID(int leafID)
	{
		return Leaves.Find(x => x.ID == leafID);
	}

	public Leaf GetLeafByIndex(int index)
	{
		return Leaves[index];
	}

	public int GetLeafCount()
	{
		return Leaves.Count;
	}
}
