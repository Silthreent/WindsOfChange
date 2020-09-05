using Godot;

public class GameManager : Node2D
{
	LeafManager LeafManager;
	Label WinCountText;

	int MoveCount = 0;
	int WinCount = 0;

	bool GeneratedLeaves = false;

	public override void _Ready()
	{
		LeafManager = GetNode<LeafManager>("LeafManager");
		LeafManager.Connect("LeafMoved", this, "OnLeafMoved");
		LeafManager.Connect("LeavesDropped", this, "OnLeavesDropped");
		LeafManager.Connect("SkyLeavesDropped", this, "OnSkyLeavesDropped");

		LeafManager.GenerateLeaves(new LeafColor[] { LeafColor.Blue, LeafColor.Green, LeafColor.Red });

		WinCountText = FindNode("WinCount") as Label;
		WinCountText.Text = WinCount.ToString();
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
			WinCount++;
			WinCountText.Text = WinCount.ToString();
			GD.Print($"Wins: {WinCount}");
			LeafManager.DropLeaves();
		}
	}

	void OnLeavesDropped()
	{
		if(!GeneratedLeaves)
		{
			LeafManager.GenerateLeaves(new LeafColor[] { LeafColor.Blue, LeafColor.Green, LeafColor.Red });
			GeneratedLeaves = true;
		}
	}

	void OnSkyLeavesDropped()
	{
		GeneratedLeaves = false;
	}
}
