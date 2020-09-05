using Godot;

public class GameManager : Node2D
{
	LeafManager LeafManager;
	Label WinCountText;
	Label MoveCountText;
	Label TotalMoveCountText;
	Label TimerText;

	int MoveCount = 0;
	int TotalMoveCount = 0;
	int WinCount = 0;
	float Timer = 0;
	bool IsTimerPaused = true;

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

		MoveCountText = FindNode("MoveCount") as Label;
		MoveCountText.Text = MoveCount.ToString();

		TotalMoveCountText = FindNode("TotalMoveCount") as Label;
		TotalMoveCountText.Text = TotalMoveCount.ToString();

		TimerText = FindNode("Timer") as Label;
		TimerText.Text = Timer.ToString("0").ToString();
	}

	public override void _Process(float delta)
	{
		if (IsTimerPaused)
			return;

		Timer += delta;
		TimerText.Text = Timer.ToString("0").ToString();
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
		TotalMoveCount++;
		MoveCountText.Text = MoveCount.ToString();
		TotalMoveCountText.Text = MoveCount.ToString();

		GD.Print($"Move Count: {MoveCount}");

		if (CheckVictory())
		{
			GD.Print("VICTORY");
			WinCount++;
			WinCountText.Text = WinCount.ToString();

			GD.Print($"Wins: {WinCount}");
			LeafManager.DropLeaves();

			IsTimerPaused = true;
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

		MoveCount = 0;
		MoveCountText.Text = MoveCount.ToString();

		IsTimerPaused = false;
	}
}
