using Godot;
using System.Collections.Generic;

public class Tree : Sprite
{
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

	public override void _PhysicsProcess(float delta)
	{
		if(Leaves.Count > 0)
		{
			DropTimer -= delta;

			if(DropTimer <= 0)
			{
				for(int x = 0; x < FallAtOnce; x++)
				{
					Leaves[0].Body.ApplyImpulse(Vector2.Zero, FallImpulse * ((x + 1) / 1));
					Leaves.RemoveAt(0);

					if (Leaves.Count <= 0)
						break;
				}

				DropTimer = DropWait;
			}
		}
		else
		{
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
