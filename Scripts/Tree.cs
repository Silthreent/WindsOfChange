using Godot;
using System;
using System.Collections.Generic;

public class Tree : Sprite
{
	List<Leaf> Leaves;

	public Tree()
	{
		Leaves = new List<Leaf>();
	}

	public void AddLeaf(Leaf leaf)
	{
		Leaves.Add(leaf);
	}

	public void RemoveLeaf(Leaf leaf)
	{
		Leaves.Remove(leaf);
	}

	public Leaf GetLeaf(int leafID)
	{
		return Leaves.Find(x => x.ID == leafID);
	}
}
