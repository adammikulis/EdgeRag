using Godot;
using System;

public partial class LLMController : Node
{

	private Button startProgramButton;
	private ChatManager chatManager;
	private ModelManager modelManager;
	private DatabaseManager databaseManager;
	private Control splashScreen;

	public override void _Ready()
	{
		chatManager = GetNode<ChatManager>("%ChatManager");
		modelManager = GetNode<ModelManager>("%ModelManager");
        databaseManager = GetNode<DatabaseManager>("%DatabaseManager");

        splashScreen = GetNode<Control>("%SplashScreen");
		startProgramButton = GetNode<Button>("%StartProgramButton");

		modelManager.OnModelLoaded += ManageChatState;
		modelManager.NewChatMessage += chatManager.PrintModelOutput;

		chatManager.OnManageModelsButtonPressed += ManageModelState;
		chatManager.OnPromptSubmitButtonPressed += OnPromptSubmit;
		chatManager.OnManageDatabasesButtonPressed += ManageDatabaseState;


		startProgramButton.Pressed += OnStartProgramButtonPressed;

		ShowSplashScreen();
    }

    private void ManageDatabaseState()
    {
        HideAllUI();
		databaseManager.ShowUI();
    }

    private void OnPromptSubmit(string prompt)
    {
        _ = modelManager.SubmitPromptAsync(prompt);
    }


    public void HideAllUI()
	{
		chatManager.HideUI();
		modelManager.HideUI();
		databaseManager.HideUI();
		splashScreen.CallDeferred("hide");
	}

	private void ShowSplashScreen()
	{
        HideAllUI();
        splashScreen.CallDeferred("show");
	}

	private void OnStartProgramButtonPressed()
	{
		ManageModelState();
	}

    private void ManageModelState()
	{
        HideAllUI();
        modelManager.ShowUI();
    }

	private void ManageChatState()
	{
		HideAllUI();
		chatManager.ShowUI();
	}


	public override void _Process(double delta)
	{
	}
}
