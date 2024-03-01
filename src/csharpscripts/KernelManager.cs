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
using System.Diagnostics;

public partial class KernelManager : Control
{

    [Signal]
    public delegate void OnChatButtonPressedEventHandler();
    [Signal]
    public delegate void NewChatMessageEventHandler(string message);

    private Label gpuLayerCountLabel;

    private Button chatButton, initializeKernelButton, ingestFilesButton;

	public IKernelMemory memory;

    private ModelManager modelManager;
    private DatabaseManager databaseManager;

    private uint contextSize = 4096;
    private uint seed = 0;
    private int gpuLayerCount = 33;

    private string modelPath = null;
    private string databaseFolderPath = null;
	private string[] filePaths;
	

	private bool validModelPath = false;
    private bool validDatabaseFolderPath = false;

	public override void _Ready()
	{
        gpuLayerCountLabel = GetNode<Label>("%GpuLayerCountLabel");


        chatButton = GetNode<Button>("%ChatButton");
        initializeKernelButton = GetNode<Button>("%InitializeKernelButton");
        ingestFilesButton = GetNode<Button>("%IngestFilesButton");

        modelManager = GetNode<ModelManager>("%ModelManager");
        databaseManager = GetNode<DatabaseManager>("%DatabaseManager");

        modelManager.OnModelSelected += OnModelSelected;
        databaseManager.OnDatabaseFolderSelected += OnDatabaseFolderSelected;
        databaseManager.OnAddFilesToDatabase += OnAddFilesToDatabase;

        initializeKernelButton.Pressed += OnInitializeKernelButtonPressed;
        ingestFilesButton.Pressed += async () => await IngestDocumentsAsync();
        chatButton.Pressed += () => EmitSignal(SignalName.OnChatButtonPressed);
    }


    private void OnAddFilesToDatabase(string[] filePaths)
    {
        this.filePaths = filePaths;
    }

    private async void OnIngestFilesButtonPressed()
    {
        await IngestDocumentsAsync();
    }

    private async void OnDatabaseProcessFiles()
    {
        await IngestDocumentsAsync();
    }

    private async void OnInitializeKernelButtonPressed()
    {
        await InitializeKernelAsync();
        ingestFilesButton.Disabled = false;
        chatButton.Disabled = false;
    }


    private void OnDatabaseFolderSelected(string databaseFolderPath)
    {
        this.databaseFolderPath = databaseFolderPath;
        validDatabaseFolderPath = true;
        if (validModelPath && validDatabaseFolderPath)
        {
            initializeKernelButton.Disabled = false;
        }
    }

    private void OnModelSelected(string modelPath)
    {
        this.modelPath = modelPath;
        validModelPath = true;
        if (validModelPath && validDatabaseFolderPath)
        {
            initializeKernelButton.Disabled = false;
        }
    }

    public void HideUI()
    {
        CallDeferred("hide");
    }

    public void ShowUI()
    {
        CallDeferred("show");
    }


	private async Task InitializeKernelAsync()
	{
		await Task.Run(() =>
		{
            memory = CreateMemoryWithLocalStorage();

        });
        gpuLayerCountLabel.Text = $"Gpu Layer Count: {gpuLayerCount}";

    }

    private IKernelMemory CreateMemoryWithLocalStorage()
    {
        LLama.Common.InferenceParams infParams = new() 
        { 
            AntiPrompts = ["\n\n"] 
        };

        LLamaSharpConfig lsConfig = new(modelPath)
        {
            ContextSize = contextSize,
            Seed = seed,
            GpuLayerCount = gpuLayerCount,
            DefaultInferenceParams = infParams 
        };

        SearchClientConfig searchClientConfig = new()
        {
            MaxMatchesCount = 1,
            AnswerTokens = 256,
        };

        TextPartitioningOptions parseOptions = new()
        {
            MaxTokensPerParagraph = 300,
            MaxTokensPerLine = 100,
            OverlappingTokens = 30
        };

        SimpleFileStorageConfig storageConfig = new()
        {
            Directory = databaseFolderPath,
            StorageType = FileSystemTypes.Disk,
        };

        SimpleVectorDbConfig vectorDbConfig = new()
        {
            Directory = databaseFolderPath,
            StorageType = FileSystemTypes.Disk,
        };


        return new KernelMemoryBuilder()
            .WithSimpleFileStorage(storageConfig)
            .WithSimpleVectorDb(vectorDbConfig)
            .WithLLamaSharpDefaults(lsConfig)
            .WithSearchClientConfig(searchClientConfig)
            .With(parseOptions)
            .Build();
    }

    private async Task IngestDocumentsAsync()
    {
        for (int i = 0; i < filePaths.Length; i++)
        {
            string path = filePaths[i];
            Stopwatch sw = Stopwatch.StartNew();
            GD.Print($"Importing {i + 1} of {filePaths.Length}: {path}");
            await memory.ImportDocumentAsync(path, steps: Constants.PipelineWithoutSummary);
            GD.Print($"Completed in {sw.Elapsed}\n");
        }
    }


    public async Task SubmitPromptAsync(string prompt)
	{
		await Task.Run(async () =>
		{
            Stopwatch sw = Stopwatch.StartNew();
            CallDeferred(nameof(DeferredEmitNewChatMessage), $"Generating answer...\n");
            MemoryAnswer answer = await memory.AskAsync(prompt);
            CallDeferred(nameof(DeferredEmitNewChatMessage), $"Answer generated in {sw.Elapsed}\n");

            CallDeferred(nameof(DeferredEmitNewChatMessage), $"Answer: {answer.Result}\n");
            foreach (var source in answer.RelevantSources)
            {
                CallDeferred(nameof(DeferredEmitNewChatMessage), $"Source: {source.SourceName}\n");
            }
        });
	}

    //public async Task SubmitPromptAsync(string prompt)
    //{
    //    await Task.Run(async () =>
    //    {
    //        await foreach (var text in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { Temperature = 0.5f, AntiPrompts = ["\n\n"] }))
    //        {
    //            CallDeferred(nameof(DeferredEmitNewChatMessage), text);
    //        }
    //    });
    //}



    public void DeferredEmitNewChatMessage(string message)
	{
		EmitSignal(nameof(NewChatMessage), message);
	}
	public override void _Process(double delta)
	{
	}
}
