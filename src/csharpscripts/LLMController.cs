using Godot;
using GodotStateCharts;
using System;

public partial class LLMController : Node
{

	private ChatManager chatManager;
	private ModelManager modelManager;
    private StateChart stateChart;
	private State chatManagementState, modelManagementState;

	public override void _Ready()
	{
		chatManager = GetNode<ChatManager>("%ChatManager");
		modelManager = GetNode<ModelManager>("%ModelManager");

		stateChart = StateChart.Of(GetNode("%LLMStateChart"));
		chatManagementState = State.Of(GetNode("%ChatManagementState"));
		modelManagementState = State.Of(GetNode("%ModelManagementState"));

		chatManager.ManageModelsButton += ManageModels;


        CallDeferred(nameof(HideAllUI));
	}

	public void HideAllUI()
	{
		chatManager.HideUI();
		modelManager.HideUI();
	}

	private void ManageModels()
	{
		stateChart.SendEvent("onModelManagementEvent");
	}

	private void ManageChat()
	{
		CallDeferred(nameof(HideAllUI));
		chatManager.ShowUI();
	}


	public override void _Process(double delta)
	{
	}
}
