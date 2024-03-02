using Godot;
using System;

public partial class DatabaseManager : MarginContainer
{

	[Signal]
	public delegate void OnDatabaseFolderSelectedEventHandler(string databaseFolderPath);
	[Signal]
	public delegate void OnProcessFilesEventHandler();
	
	
	
	private string databaseFolderPath;
	private string[] filePaths;

	private Button selectDatabaseFolderButton;

	private FileDialog selectDatabaseFolderFileDialog;
    private RichTextLabel currentDatabaseFolderRichTextLabel;

    public override void _Ready()
	{

		currentDatabaseFolderRichTextLabel = GetNode<RichTextLabel>("%CurrentDatabaseFolderRichTextLabel");

		selectDatabaseFolderButton = GetNode<Button>("%SelectDatabaseFolderButton");

		selectDatabaseFolderFileDialog = GetNode<FileDialog>("%SelectDatabaseFolderFileDialog");
		selectDatabaseFolderButton.Pressed += OnSelectDatabaseFolderButtonPressed;
		selectDatabaseFolderFileDialog.DirSelected += OnSelectDatabaseFolderDirSelected;


    }



    private void OnSelectDatabaseFolderButtonPressed()
	{
		selectDatabaseFolderFileDialog.CallDeferred("popup_centered");
	}

	private void OnSelectDatabaseFolderDirSelected(string databaseFolderPath)
	{
		this.databaseFolderPath = databaseFolderPath;
		currentDatabaseFolderRichTextLabel.Text = $"Current Database Folder: {databaseFolderPath}";
		EmitSignal(SignalName.OnDatabaseFolderSelected, databaseFolderPath);
	}

    private void ShowUI()
	{
		CallDeferred("show");
	}

	private void HideUI()
	{
		CallDeferred("hide");
		selectDatabaseFolderFileDialog.CallDeferred("hide");
	}

	
	public override void _Process(double delta)
	{
	}
}
