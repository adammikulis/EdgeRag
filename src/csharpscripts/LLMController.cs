using Godot;
using System;

public partial class LLMController : Node
{

	private Button startProgramButton;
	private ChatManager chatManager;
	private ModelManager modelManager;
	private Control splashScreen;

	public override void _Ready()
	{
		chatManager = GetNode<ChatManager>("%ChatManager");
		modelManager = GetNode<ModelManager>("%ModelManager");
		splashScreen = GetNode<Control>("%SplashScreen");
		startProgramButton = GetNode<Button>("%StartProgramButton");

		modelManager.OnModelLoaded += ManageChatState;
		modelManager.NewChatMessage += chatManager.PrintModelOutput;

		chatManager.ManageModelsButtonPressed += ManageModelState;
		chatManager.OnPromptSubmit += OnPromptSubmit;

		startProgramButton.Pressed += OnStartProgramButtonPressed;

		ShowSplashScreen();
    }

    private void OnPromptSubmit(string prompt)
    {
        _ = modelManager.SubmitPromptAsync(prompt);
    }


    public void HideAllUI()
	{
		chatManager.HideUI();
		modelManager.HideUI();
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
