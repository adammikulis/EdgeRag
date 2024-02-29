using Godot;
using System;

public partial class ModelManager : MarginContainer
{

    [Signal]
    public delegate void OnModelSelectedEventHandler(string modelFilePath);

    private RichTextLabel selectedModelRichTextLabel;
    private Button selectModelButton;
    private string modelFilePath;
    public uint contextSize = 512;

    private HSlider contextSizeSlider, numGpuLayersSlider;
    private Label contextSizeLabel, numGpuLayersLabel;

    private FileDialog modelFileDialog;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        contextSizeSlider = GetNode<HSlider>("%ContextSizeSlider");
        numGpuLayersSlider = GetNode<HSlider>("%NumGpuLayersSlider");

        contextSizeLabel = GetNode<Label>("%ContextSizeLabel");
        numGpuLayersLabel = GetNode<Label>("%NumGpuLayersLabel");
        selectedModelRichTextLabel = GetNode<RichTextLabel>("%SelectedModelRichTextLabel");

        selectModelButton = GetNode<Button>("%SelectModelButton");

        modelFileDialog = GetNode<FileDialog>("%ModelFileDialog");

        contextSizeSlider.ValueChanged += OnContextSizeSliderChanged;

        selectModelButton.Pressed += OnSelectModelButtonPressed;
        modelFileDialog.FileSelected += onModelFileDialogFileSelected;
        
    }

    private void OnSelectModelButtonPressed()
    {
        ShowModelFileDialog();
    }

    private void onModelFileDialogFileSelected(string modelFilePath)
    {
        this.modelFilePath = modelFilePath;
        selectedModelRichTextLabel.Text = $"Current model selected: {this.modelFilePath}";
        EmitSignal(SignalName.OnModelSelected, modelFilePath);
    }

    public void ShowModelFileDialog()
    {
        modelFileDialog.CallDeferred("popup_centered");
    }

    public void HideUI()
    {
        CallDeferred("hide");
        modelFileDialog.CallDeferred("hide");
    }

    public void ShowUI()
    {
        CallDeferred("show");
    }

    private void OnContextSizeSliderChanged(double sliderValue)
    {
        contextSize = (uint)Math.Pow(2, sliderValue + 3);
        contextSizeLabel.Text = $"Context Size: {contextSize}";
    }

    private void OnNumGpuLayersSliderChanged(double sliderValue)
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
