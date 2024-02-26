// This is a single-script implementation of LLamaSharp for a Godot-based GUI
// 

using Godot;
using System;
using LLama;
using LLama.Common;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using LLama.Native;
using System.Net;

public partial class HomeScreen : Control
{
    
    public enum AppState
    {
        Idle,
        ModelSelection,
        ModelLoaded,
        DatabaseSelection,
        DatabaseLoaded,
        ModelAndDatabaseLoaded,
        StandardConversation,
        DatabaseConversation
    }

   
    private string[] filePaths;
    private string modelPath;
    private string fullModelName;
    private bool isModelLoaded = false;
    private bool isDatabaseLoaded = false;

    [Export]
    private LineEdit lineEditInput;
    [Export]
    private Label currentModelLabel;
    [Export]
    private RichTextLabel mainTextOutput;
    [Export]
    private Button submitButton, manageModelsButton, startNewChatButton;

    // Model related fields (assuming these are part of the LLama library)
    private LLamaWeights model;
    private LLamaEmbedder embedder;
    private LLamaContext context;
    private ModelParams modelParams;
    private ChatSession session;
    private InteractiveExecutor executor;

    [Signal]
    public delegate void NewChatMessageEventHandler(string message);


    private AppState currentState;

    private AppState CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            UpdateUIForState();
        }
    }
    

    public override void _Ready()
    {
        

        NewChatMessage += StreamChatMessage;

        lineEditInput.TextSubmitted += OnTextSubmitted;
        submitButton.Pressed += OnChatButtonPressed;
        manageModelsButton.Pressed += OnManageModelsButtonPressed;
        startNewChatButton.Pressed += OnStartNewChatButtonPressed;

        CheckModelStatus();
    }

    private void UpdateUIForState()
    {
        switch (CurrentState)
        {
            case AppState.Idle:
                manageModelsButton.Show();
                startNewChatButton.Hide();
                break;
            case AppState.ModelSelection:
                manageModelsButton.Show();
                startNewChatButton.Hide();
                break;
            case AppState.ModelLoaded:
                manageModelsButton.Hide();
                startNewChatButton.Show();
                break;
            case AppState.DatabaseSelection:
                manageModelsButton.Hide();
                startNewChatButton.Show();
                break;
        }
    }

    private void StreamChatMessage(string message)
    {
        mainTextOutput.Text += message;
    }

    private void CheckModelStatus()
    {
        if (!isModelLoaded)
        {
            OnManageModelsButtonPressed();
        }
    }

    private async void OnChatButtonPressed()
    {
        string prompt = lineEditInput.Text;
        await SubmitPromptAsync(prompt);
    }

    private void OnStartNewChatButtonPressed()
    {
        if (CurrentState == AppState.ModelLoaded || CurrentState == AppState.ModelAndDatabaseLoaded)
        {
            // Reset conversation UI and start new chat
            mainTextOutput.Text = "Chatbot powered by Local LLM, enter your query!";
            if (isDatabaseLoaded)
            {
                CurrentState = AppState.DatabaseConversation;
            }
            else
            {
                CurrentState = AppState.StandardConversation;
            }

        }
    }

    private void OnManageModelsButtonPressed()
    {
        currentState = AppState.ModelSelection;
        mainTextOutput.Text = "Hey there! Choose an AI model to load and have fun!\n";
        
    }


    private async void OnTextSubmitted(string text)
    {
        switch (currentState)
        {
            case AppState.Idle:
                break;
            case AppState.ModelSelection:
                HandleModelSelection(text);
                break;
            case AppState.ModelLoaded:
                OnStartNewChatButtonPressed();
                break;
            case AppState.StandardConversation:
                mainTextOutput.Text = "";
                await SubmitPromptAsync(text);
                break;
            default:
                GD.Print("Not expecting text input at this moment.");
                break;
        }
    }
    
    public async Task SubmitPromptAsync(string prompt)
    {
        // Clear the UI on the main thread before starting the background task
        ClearUI();

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

    public void ClearUI()
    {
        lineEditInput.Text = "";
        mainTextOutput.Text = "";
    }

    private void HandleModelSelection(string text)
    {
        if (int.TryParse(text, out int index) && index >= 1 && index <= filePaths.Length)
        {
            index--; // Adjust for zero-based indexing
            modelPath = filePaths[index];
            fullModelName = Path.GetFileNameWithoutExtension(modelPath);
            GD.Print($"Model selected: {fullModelName}");
            mainTextOutput.Text += $"\nModel selected: {fullModelName}\n";
            LoadModel(modelPath);
        }
        else
        {
            mainTextOutput.Text += "\nInvalid input, please enter a number corresponding to the model list.\n";
            lineEditInput.Clear(); // Clear the input for a new attempt
        }
    }


    private void LoadModel(string modelPath)
    {
        lineEditInput.Text = "";
        
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
        mainTextOutput.Text += $"Model: {fullModelName} from {modelPath} loaded\n";
        
        currentState = AppState.ModelLoaded;

        OnStartNewChatButtonPressed();
    }


	public override void _Process(double _delta)
	{
        if (currentState == AppState.Idle && !isModelLoaded)
        {
            CheckModelStatus();
        }
	}
}
