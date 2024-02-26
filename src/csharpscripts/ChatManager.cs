using Godot;
using System;

public partial class ChatManager : Node
{

    [Signal]
    public delegate void ManageModelsButtonEventHandler();

	private Button manageModelsButton, chatSubmitButton, startNewChat;
    [Export]
    private VBoxContainer buttonContainer;
    [Export]
    private GridContainer chatGridContainer;

    private Control chatManagerControl;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        chatManagerControl = GetNode<Control>("%ChatManagerControl");
        chatSubmitButton = GetNode<Button>("%ChatSubmitButton");
        manageModelsButton = GetNode<Button>("%ManageModelsButton");


		manageModelsButton.Pressed += OnManageModelsButtonPressed;
		chatSubmitButton.Pressed += OnSubmitButtonPressed;
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


    private void OnSubmitButtonPressed()
    {
        throw new NotImplementedException();
    }

    private void OnManageModelsButtonPressed()
    {
        EmitSignal(SignalName.ManageModelsButton);
    }


    public override void _Process(double delta)
	{
	}
}
