using Godot;
using System;

public partial class DatabaseManager : Control
{
	private Button addDatabaseButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		addDatabaseButton = GetNode<Button>("%AddDatabaseButton");
	}

	public void ShowUI()
	{
		CallDeferred("show");
	}

	public void HideUI()
	{
		CallDeferred("hide");
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
