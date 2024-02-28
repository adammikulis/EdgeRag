using Godot;
using LLama.Common;
using LLama;
using System;
using System.Threading.Tasks;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.Handlers;
using LLama.Batched;
using LLamaSharp.KernelMemory;

public partial class ModelManager : Control
{

    [Signal]
    public delegate void OnModelLoadedEventHandler();
    [Signal]
    public delegate void NewChatMessageEventHandler(string message);

	public IKernelMemory modelKernel;


    private FileDialog modelFileDialog;
	private Button loadSelectedModelButton, selectModelButton;
	private HSlider contextSizeSlider, numGpuLayersSlider;
	private Label contextSizeLabel, numGpuLayersLabel;
	private RichTextLabel selectedModelInfoLabel;


    private LLamaWeights model;
	private LLamaEmbedder embedder;
	private LLamaContext context;
	private ModelParams modelParams;
	private ChatSession session;
	private InteractiveExecutor executor;
	private string modelPath;
	private uint contextSize = 512;

	private bool isModelLoaded = false;

	public override void _Ready()
	{
		modelFileDialog = GetNode<FileDialog>("%ModelFileDialog");

		selectModelButton = GetNode<Button>("%SelectModelButton");
		loadSelectedModelButton = GetNode<Button>("%LoadSelectedModelButton");

		contextSizeSlider = GetNode<HSlider>("%ContextSizeSlider");
		numGpuLayersSlider = GetNode<HSlider>("%NumGpuLayersSlider");

		contextSizeLabel = GetNode<Label>("%ContextSizeLabel");
		numGpuLayersLabel = GetNode<Label>("%NumGpuLayersLabel");
		selectedModelInfoLabel = GetNode<RichTextLabel>("%SelectedModelInfoLabel");

		contextSizeSlider.ValueChanged += OnContextSizeSliderChanged;

		selectModelButton.Pressed += OnSelectModel;
		modelFileDialog.FileSelected += OnModelSelected;
		loadSelectedModelButton.Pressed += OnLoadSelectedModel;
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
		contextSize = (uint)Math.Pow(sliderValue, 2);
		contextSizeLabel.Text = $"Context Size: {contextSize}";
	}

    private void OnNumGpuLayersSliderChanged(double sliderValue)
    {

    }

    private void OnSelectModel()
	{
		ShowModelFileDialog();
	}

	public void ShowModelFileDialog()
	{
		modelFileDialog.CallDeferred("popup_centered");
	}

	private void OnModelSelected(string modelPath)
	{
		this.modelPath = modelPath;
		selectedModelInfoLabel.Text = $"Current model selected: {modelPath}";
    }

	private async void OnLoadSelectedModel()
	{
        await LoadModelAsync(modelPath);
        EmitSignal(SignalName.OnModelLoaded);
    }

	private async Task LoadModelAsync(string modelPath)
	{
		await Task.Run(() =>
		{
			var searchClientConfig = new SearchClientConfig
			{
				MaxMatchesCount = 1,
				AnswerTokens = 100,
			};

			modelKernel = new KernelMemoryBuilder().WithLLamaSharpDefaults(new LLamaSharpConfig(modelPath)
			{
				DefaultInferenceParams = new LLama.Common.InferenceParams()
				{
					AntiPrompts = new string[] { "\n\n" }
				}
			})
			.WithSearchClientConfig(searchClientConfig)
			.With(new TextPartitioningOptions
			{
				MaxTokensPerParagraph = 300,
				MaxTokensPerLine = 100,
				OverlappingTokens = 30
			})
			.Build();
		});
        
    }

	public async Task SubmitPromptAsync(string prompt)
	{
		await Task.Run(async () =>
		{
			await foreach (var text in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { Temperature = 0.5f, AntiPrompts = ["\n\n"] }))
			{
				CallDeferred(nameof(DeferredEmitNewChatMessage), text);
			}
		});
	}

	public void DeferredEmitNewChatMessage(string message)
	{
		EmitSignal(nameof(NewChatMessage), message);
	}
	public override void _Process(double delta)
	{
	}
}
