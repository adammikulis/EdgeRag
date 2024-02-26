using Godot;
using LLama.Common;
using LLama;
using System;
using static HomeScreen;
using static System.Collections.Specialized.BitVector32;
using System.Threading.Tasks;

public partial class LLMManager : Node
{
	[Export]
	FileDialog modelFileDialog;

    [Signal]
    public delegate void NewChatMessageEventHandler(string message);


    private LLamaWeights model;
    private LLamaEmbedder embedder;
    private LLamaContext context;
    private ModelParams modelParams;
    private ChatSession session;
    private InteractiveExecutor executor;

    private bool isModelLoaded = false;

    public override void _Ready()
	{
		modelFileDialog.FileSelected += OnModelSelected; 

	}

    public void ShowModelFileDialog()
    {
        if (modelFileDialog != null)
        {
            modelFileDialog.PopupCentered();
        }
    }


    private void OnModelSelected(string filePath)
	{
		LoadModel(filePath);
    }

    private void LoadModel(string modelPath)
    {
        modelParams = new ModelParams(modelPath)
        {
            ContextSize = 4096, // This can be changed by the user according to memory usage and model capability
            EmbeddingMode = true, // This must be set to true to generate embeddings for vector search
            GpuLayerCount = -1 // Set your number of layers to offload to the GPU here, depending on VRAM available (you can mix CPU with GPU for hybrid inference)
        };
        model = LLamaWeights.LoadFromFile(modelParams);
        embedder = new LLamaEmbedder(model, modelParams);
        context = model.CreateContext(modelParams);
        executor = new InteractiveExecutor(context);
        session = new ChatSession(executor);
        isModelLoaded = true;
        GD.Print($"{modelPath} loaded\n");
    }

    public async Task SubmitPromptAsync(string prompt)
    {
        await Task.Run(async () =>
        {
            await foreach (var text in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { Temperature = 0.25f, AntiPrompts = ["<end>"] }))
            {
                if (text != "<end>")
                {
                    CallDeferred(nameof(DeferredEmitNewChatMessage), text);
                }
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
