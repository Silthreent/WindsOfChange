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

		LeafManager.GenerateLeaves();

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
		// Count up the on screen timer if the game is paused from something like animations
		if (IsTimerPaused)
			return;

		Timer += delta;
		TimerText.Text = Timer.ToString("0").ToString();
	}

	public void ToggleStressFreeMode(bool mode)
	{
		// Stress free mode hides the timer and move texts
		// Play without worrying about how well you're doing!
		MoveCountText.Visible = !mode;
		TotalMoveCountText.Visible = !mode;
		TimerText.Visible = !mode;
	}

	bool CheckVictory()
	{
		// Did the player win?

		// Go through every tree one at a time and make sure all it's leaves match
		int treeCheck = 0;
		Tree workingTree;
		while ((workingTree = LeafManager.GetTree(treeCheck)) != null)
		{
			LeafColor color = LeafColor.None;
			for(int x = 0; x < workingTree.GetLeafCount(); x++)
			{
				// Check each leaf, if it doesn't match then we didn't win
				if(color == LeafColor.None)
				{
					// There wasn't a color already, so just save the first one seen
					color = workingTree.GetLeafByIndex(x).Color;
					continue;
				}

				// If it doesn't match, then we lost
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
		// A leaf was moved, so progress each stat then check if player won
		MoveCount++;
		TotalMoveCount++;
		MoveCountText.Text = MoveCount.ToString();
		TotalMoveCountText.Text = MoveCount.ToString();

		GD.Print($"Move Count: {MoveCount}");

		if (CheckVictory())
		{
			// Player won, so progress stats and drop all the leaves
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
		// Leaves finished falling, generate new set but only if it was the first tree
		if(!GeneratedLeaves)
		{
			LeafManager.GenerateLeaves();
			GeneratedLeaves = true;
		}
	}

	void OnSkyLeavesDropped()
	{
		// New leaves finished spawning, so enable the spawn next time and reset our moves
		GeneratedLeaves = false;

		MoveCount = 0;
		MoveCountText.Text = MoveCount.ToString();

		IsTimerPaused = false;
	}
}
