using Godot;

public class GameManager : Node2D
{
	LeafManager LeafManager;

	int MoveCount = 0;

	public override void _Ready()
	{
		LeafManager = GetNode<LeafManager>("LeafManager");
		LeafManager.Connect("LeafMoved", this, "OnLeafMoved");

		LeafManager.GenerateLeaves(new LeafColor[] { LeafColor.Blue, LeafColor.Green });
	}

	bool CheckVictory()
	{
		int treeCheck = 0;
		Tree workingTree;
		while ((workingTree = LeafManager.GetTree(treeCheck)) != null)
		{
			LeafColor color = LeafColor.None;
			for(int x = 0; x < workingTree.GetLeafCount(); x++)
			{
				if(color == LeafColor.None)
				{
					color = workingTree.GetLeafByIndex(x).Color;
					continue;
				}

				if(workingTree.GetLeafByIndex(x).Color != color)
				{
					return false;
				}
			}

			treeCheck++;
		}

		return true;
	}

	void OnLeafMoved(Leaf leaf1, Leaf leaf2)
	{
		MoveCount++;
		GD.Print($"Move Count: {MoveCount}");

		if (CheckVictory())
		{
			GD.Print("VICTORY");
			int treeCheck = 0;
			Tree workingTree;
			while ((workingTree = LeafManager.GetTree(treeCheck)) != null)
			{
				workingTree.DropLeaves();

				treeCheck++;
			}
		}
	}
}
