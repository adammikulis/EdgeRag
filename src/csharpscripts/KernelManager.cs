using Godot;
using LLama;
using LLama.Common;
using System;
using System.Threading.Tasks;
using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.ContentStorage.DevTools;
using Microsoft.KernelMemory.FileSystem.DevTools;
using Microsoft.KernelMemory.MemoryStorage.DevTools;

public partial class KernelManager : Control
{

    [Signal]
    public delegate void OnChatButtonPressedEventHandler();
    [Signal]
    public delegate void NewChatMessageEventHandler(string message);

	public IKernelMemory modelKernel;


    private FileDialog modelFileDialog;
	private Button addFilesToDatabaseButton, loadDatabaseButton, saveDatabaseButton, loadSelectedModelButton, selectModelButton;
	private HSlider contextSizeSlider, numGpuLayersSlider;
	private Label contextSizeLabel, numGpuLayersLabel;
	private RichTextLabel currentFilesRichTextLabel, selectedModelRichTextLabel;


    private LLamaWeights model;
	private LLamaEmbedder embedder;
	private LLamaContext context;
	private ModelParams modelParams;
	private ChatSession session;
	private InteractiveExecutor executor;
	private string modelPath;
	private string databaseStorageDirectoryPath;
	private string filesToEmbedDirectoryPath;
	private uint contextSize = 512;

	private bool isModelLoaded = false;

	public override void _Ready()
	{
		modelFileDialog = GetNode<FileDialog>("%ModelFileDialog");

        currentFilesRichTextLabel = GetNode<RichTextLabel>("%CurrentFilesRichTextLabel");

        selectModelButton = GetNode<Button>("%SelectModelButton");
		loadSelectedModelButton = GetNode<Button>("%LoadSelectedModelButton");

		contextSizeSlider = GetNode<HSlider>("%ContextSizeSlider");
		numGpuLayersSlider = GetNode<HSlider>("%NumGpuLayersSlider");
		contextSizeLabel = GetNode<Label>("%ContextSizeLabel");
		numGpuLayersLabel = GetNode<Label>("%NumGpuLayersLabel");
		selectedModelRichTextLabel = GetNode<RichTextLabel>("%SelectedModelRichTextLabel");

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
		selectedModelRichTextLabel.Text = $"Current model selected: {modelPath}";
    }

	private async void OnLoadSelectedModel()
	{
        await LoadModelAsync(modelPath);
        
    }

	private void OnChatButtonPressed()
	{
        EmitSignal(SignalName.OnChatButtonPressed);
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
