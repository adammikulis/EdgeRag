using Godot;
using System.Threading.Tasks;

[Tool]
public partial class LLMToolPlugin : EditorPlugin
{
    private VBoxContainer customControl;
    private LineEdit inputLine;
    private Button submitButton;
    private RichTextLabel responseLabel;
    private LLMManager llmManager;


    public override void _EnterTree()
    {
        llmManager = GetNode<LLMManager>("/root/llm");

        customControl = new VBoxContainer();
        AddControlToDock(DockSlot.LeftBl, customControl);
        customControl.Name = "LLM Chat";

        inputLine = new LineEdit();
        customControl.AddChild(inputLine);

        submitButton = new Button { Text = "Submit" };
        submitButton.Pressed += (OnSubmitPressed);
        customControl.AddChild(submitButton);

        responseLabel = new RichTextLabel();
        customControl.AddChild(responseLabel);

        if (llmManager != null)
        {
            llmManager.NewChatMessage += OnNewChatMessageReceived;
            llmManager.ShowModelFileDialog();
        }

    }

    public override void _ExitTree()
    {
        if (llmManager != null)
        {
            llmManager.NewChatMessage -= OnNewChatMessageReceived;
        }

    }

    private void OnSubmitPressed()
    {
        if (llmManager != null && !string.IsNullOrWhiteSpace(inputLine.Text))
        {
            Task.Run(() => llmManager.SubmitPromptAsync(inputLine.Text));
        }
    }

    private void OnNewChatMessageReceived(string message)
    {
        responseLabel.CallDeferred("AddText", message + "\n");
    }
}
