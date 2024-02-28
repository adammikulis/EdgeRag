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
		chatManager.ManageModelsButtonPressed += ManageModelsState;
		startProgramButton.Pressed += OnStartProgramButtonPressed;

        //CallDeferred("ManageModelsState");
        HideAllUI();
		ShowSplashScreen();
    }



	public void HideAllUI()
	{
		chatManager.HideUI();
		modelManager.HideUI();
		splashScreen.CallDeferred("hide");
	}

	private void ShowSplashScreen()
	{
		splashScreen.CallDeferred("show");
	}

	private void OnStartProgramButtonPressed()
	{
		ManageModelsState();
	}



    private void ManageModelsState()
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
