using Godot;
using System;

public partial class LLMController : Node
{

	private Button disclaimerAcceptButton, startProgramButton;
	private PopupPanel disclaimerPopupPanel;

	private ChatManager chatManager;
	private KernelManager kernelManager;
	private Control splashScreen;

	private bool userAcceptedDisclaimer = false;

	public override void _Ready()
	{
		chatManager = GetNode<ChatManager>("%ChatManager");
		kernelManager = GetNode<KernelManager>("%KernelManager");

        splashScreen = GetNode<Control>("%SplashScreen");
		startProgramButton = GetNode<Button>("%StartProgramButton");
		disclaimerAcceptButton = GetNode<Button>("%DisclaimerAcceptButton");



        // These are custom signals from the KernelManager to decouple operations with a state machine
        kernelManager.OnChatButtonPressed += ManageChatState;
		kernelManager.NewChatMessage += chatManager.PrintModelOutput;

        // These are custom signals from the ChatManager to decouple operations with a state machine
        chatManager.OnManageKernelButtonPressed += ManageKernelState;
		chatManager.OnPromptSubmitButtonPressed += OnPromptSubmit;

		// For splash screen
		startProgramButton.Pressed += OnStartProgramButtonPressed;
		disclaimerAcceptButton.Pressed += OnDisclaimerAcceptButtonPressed;
		ShowSplashScreen();


    }

	private void OnDisclaimerAcceptButtonPressed()
	{
		startProgramButton.CallDeferred("set_disabled", false);
		disclaimerAcceptButton.CallDeferred("set_disabled", true);
		disclaimerAcceptButton.Text = "Disclaimer Accepted!";
		userAcceptedDisclaimer = true;
		disclaimerAcceptButton.Text = "Disclaimer Accepted!";
		disclaimerAcceptButton.CallDeferred("set_disabled", true);
	}


	// Wrapper for async method to avoid error with signal calling
	private void OnPromptSubmit(string prompt)
    {
        _ = kernelManager.QueryDatabaseAsync(prompt);
    }


    public void HideAllUI()
	{
		chatManager.HideUI();
		kernelManager.HideUI();
		splashScreen.CallDeferred("hide");
	}

	private void ShowSplashScreen()
	{
        HideAllUI();
        splashScreen.CallDeferred("show");
	}

	private void OnStartProgramButtonPressed()
	{
		ManageKernelState();
	}

    private void ManageKernelState()
	{
        HideAllUI();
        kernelManager.ShowUI();
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
