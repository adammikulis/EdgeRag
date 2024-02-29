using Godot;
using System;

public partial class ChatManager : Control
{

    [Signal]
    public delegate void OnManageKernelButtonPressedEventHandler();
    [Signal]
    public delegate void OnPromptSubmitButtonPressedEventHandler(string prompt);

	private Button manageDatabasesButton, manageKernelButton, promptSubmitButton, startNewChat;

    private Control chatManagerControl;

    private LineEdit mainChatInput;
    private RichTextLabel mainChatOutput;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        promptSubmitButton = GetNode<Button>("%PromptSubmitButton");
        manageKernelButton = GetNode<Button>("%ManageKernelButton");
        mainChatInput = GetNode<LineEdit>("%MainChatInput");
        mainChatOutput = GetNode<RichTextLabel>("%MainOutputTextLabel");

        // LLMController listens for these signals, decouples so it doesn't have to know the actual button
        manageKernelButton.Pressed += () => EmitSignal(SignalName.OnManageKernelButtonPressed);
        promptSubmitButton.Pressed += PromptSubmitButtonPressed;
    }

    public void HideUI()
    {
        CallDeferred("hide");
    }

    public void ShowUI()
    {
        CallDeferred("show");
    }


    private void PromptSubmitButtonPressed()
    {
        string prompt = mainChatInput.Text;
        mainChatOutput.Text += $"\nPrompt:\n{prompt}\n\nResponse:\n";
        mainChatInput.Text = "";
        GD.Print($"Submitting prompt {prompt}");
        EmitSignal(SignalName.OnPromptSubmitButtonPressed, prompt);
    }

    public void PrintModelOutput(string text)
    {
        mainChatOutput.Text += text;
    }


    public override void _Process(double delta)
	{
	}
}
