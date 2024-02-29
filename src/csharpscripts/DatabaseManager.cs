using Godot;
using System;

public partial class DatabaseManager : MarginContainer
{

	[Signal]
	public delegate void OnDatabaseFolderSelectedEventHandler(string databaseFolderPath);
	[Signal]
	public delegate void OnAddFilesToDatabaseEventHandler(string[] filePaths);
	[Signal]
	public delegate void OnProcessFilesEventHandler();
	
	
	
	private string databaseFolderPath;
	private string[] filePaths;

	private Button chooseFilesForDatabaseButton, chooseDatabaseFolderButton, processFilesButton;

	private FileDialog chooseFilesForDatabaseFileDialog, chooseDatabaseFolderFileDialog;
    private RichTextLabel currentDatabaseFolderRichTextLabel, currentFilesStagedRichTextLabel;

    public override void _Ready()
	{

        currentFilesStagedRichTextLabel = GetNode<RichTextLabel>("%CurrentFilesStagedRichTextLabel");
		currentDatabaseFolderRichTextLabel = GetNode<RichTextLabel>("%CurrentDatabaseFolderRichTextLabel");

        chooseFilesForDatabaseButton = GetNode<Button>("%ChooseFilesForDatabaseButton");
		chooseDatabaseFolderButton = GetNode<Button>("%ChooseDatabaseFolderButton");


		chooseFilesForDatabaseFileDialog = GetNode<FileDialog>("%ChooseFilesForDatabaseFileDialog");
		chooseDatabaseFolderFileDialog = GetNode<FileDialog>("%ChooseDatabaseFolderFileDialog");

		chooseFilesForDatabaseButton.Pressed += OnChooseFilesForDatabaseButtonPressed;
		chooseDatabaseFolderButton.Pressed += OnChooseDatabaseFolderButtonPressed;

		chooseDatabaseFolderFileDialog.DirSelected += OnChooseDatabaseFolderDirSelected;
		chooseFilesForDatabaseFileDialog.FilesSelected += OnAddFilesToDatabaseFilesSelected;

    }


	private void OnChooseFilesForDatabaseButtonPressed()
	{
		chooseFilesForDatabaseFileDialog.CallDeferred("show");
	}

	private void OnAddFilesToDatabaseFilesSelected(string[] filePaths)
	{
		this.filePaths = filePaths;
		currentFilesStagedRichTextLabel.Text = $"Current files staged: {filePaths}";
		EmitSignal(SignalName.OnAddFilesToDatabase, filePaths);
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
		chooseFilesForDatabaseFileDialog.CallDeferred("hide");
		chooseDatabaseFolderFileDialog.CallDeferred("hide");
	}

	
	public override void _Process(double delta)
	{
	}
}
