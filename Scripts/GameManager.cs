using Godot;

public class GameManager : Node2D
{
	LeafManager LeafManager;

	public override void _Ready()
	{
		LeafManager = GetNode<LeafManager>("LeafManager");

		LeafManager.GenerateLeaves(new LeafColor[] { LeafColor.Blue, LeafColor.Green });
	}
}
