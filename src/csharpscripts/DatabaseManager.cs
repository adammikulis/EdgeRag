using Godot;
using System;

public partial class DatabaseManager : MarginContainer
{

	[Signal]
	public delegate void OnDatabaseFolderSelectedEventHandler(string databaseFolderPath);
	private string databaseFolderPath;

	private Button addFilesToDatabaseButton, chooseDatabaseFolderButton;

	private FileDialog addFilesFileDialog, chooseDatabaseFolderFileDialog;
    private RichTextLabel currentDatabaseFolderRichTextLabel, currentFilesRichTextLabel;

    public override void _Ready()
	{

        currentFilesRichTextLabel = GetNode<RichTextLabel>("%CurrentFilesRichTextLabel");
		currentDatabaseFolderRichTextLabel = GetNode<RichTextLabel>("%CurrentDatabaseFolderRichTextLabel");

        addFilesToDatabaseButton = GetNode<Button>("%AddFilesToDatabaseButton");
		chooseDatabaseFolderButton = GetNode<Button>("%ChooseDatabaseFolderButton");

		addFilesFileDialog = GetNode<FileDialog>("%AddFilesFileDialog");
		chooseDatabaseFolderFileDialog = GetNode<FileDialog>("%ChooseDatabaseFolderFileDialog");

		chooseDatabaseFolderButton.Pressed += OnChooseDatabaseFolderButtonPressed;
		chooseDatabaseFolderFileDialog.DirSelected += OnChooseDatabaseFolderDirSelected;
    }

	private void OnChooseDatabaseFolderButtonPressed()
	{
		chooseDatabaseFolderFileDialog.CallDeferred("popup_centered");
		
	}

	private void OnChooseDatabaseFolderDirSelected(string databaseFolderPath)
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
		addFilesFileDialog.CallDeferred("hide");
		chooseDatabaseFolderFileDialog.CallDeferred("hide");
	}

	
	public override void _Process(double delta)
	{
	}
}
