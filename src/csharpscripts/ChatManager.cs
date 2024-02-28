using Godot;
using System;

public partial class ChatManager : Node
{

    [Signal]
    public delegate void ManageModelsButtonPressedEventHandler();

    [Signal]
    public delegate void OnPromptSubmitEventHandler(string prompt);

	private Button manageModelsButton, chatSubmitButton, startNewChat;

    private Control chatManagerControl;

    private LineEdit mainChatInput;
    private RichTextLabel mainChatOutput;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        chatManagerControl = GetNode<Control>("%ChatManagerControl");
        chatSubmitButton = GetNode<Button>("%ChatSubmitButton");
        manageModelsButton = GetNode<Button>("%ManageModelsButton");
        mainChatInput = GetNode<LineEdit>("%MainChatInput");
        mainChatOutput = GetNode<RichTextLabel>("%MainOutputTextLabel");



		manageModelsButton.Pressed += OnManageModelsButtonPressed;
		chatSubmitButton.Pressed += OnPromptSubmitButtonPressed;
	}

    public void HideUI()
    {
        chatManagerControl.CallDeferred("hide");

        GD.Print("Hiding UI for" + Name);
    }

    public void ShowUI()
    {
        chatManagerControl.CallDeferred("show");
    }


    private void OnPromptSubmitButtonPressed()
    {
        mainChatOutput.Text = "";
        string prompt = mainChatInput.Text;
        mainChatInput.Text = "";
        GD.Print($"Submitting prompt {prompt}");
        EmitSignal(SignalName.OnPromptSubmit, prompt);
        
    }

    private void OnManageModelsButtonPressed()
    {
        EmitSignal(SignalName.ManageModelsButtonPressed);
    }

    public void PrintModelOutput(string text)
    {
        mainChatOutput.Text += text;
    }


    public override void _Process(double delta)
	{
	}
}
