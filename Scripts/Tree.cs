using Godot;
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
