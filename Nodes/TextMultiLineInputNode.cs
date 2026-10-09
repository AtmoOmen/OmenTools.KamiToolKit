using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace OmenTools.KamiToolKit.Nodes;

public unsafe class TextMultiLineInputNode : TextInputNode
{
    private float minimumHeight;
    private float lastHeight;
    private bool  isUpdatingHeight;

    public TextMultiLineInputNode() : base(true)
    {
        minimumHeight = Height;
        lastHeight    = Height;
    }

    public bool AutoUpdateHeight
    {
        get;
        set
        {
            if (field == value) return;

            field         = value;
            minimumHeight = Height;
            UpdateHeightForContent();
        }
    }

    public Action<float>? HeightChanged { get; set; }

    public void UpdateHeightForContent()
    {
        if (!AutoUpdateHeight || isUpdatingHeight) return;

        var textHeight    = CurrentTextNode.GetTextDrawSize(false).Y;
        var contentHeight = MathF.Ceiling(MathF.Max(minimumHeight, MathF.Max(textHeight, CurrentTextNode.LineSpacing) + 20.0f));
        contentHeight = MathF.Min(contentHeight, ushort.MaxValue);
        if (MathF.Abs(Height - contentHeight) < 0.5f) return;

        isUpdatingHeight = true;

        try
        {
            Height = contentHeight;
        }
        finally
        {
            isUpdatingHeight = false;
        }

        HeightChanged?.Invoke(Height);
    }

    protected override void OnTextChanged()
    {
        base.OnTextChanged();
        UpdateHeightForContent();
    }

    protected override void OnUpdate
    (
        AtkComponentBase* thisPtr,
        float             delta
    )
    {
        base.OnUpdate(thisPtr, delta);
        if (IsDisposed || AllowEnterToComplete || !IsFocused) return;

        var inputData = UIInputData.Instance();
        if (!inputData->IsKeyDown(SeVirtualKey.CONTROL) || !inputData->IsKeyPressed(SeVirtualKey.RETURN)) return;

        var textInput = AtkStage.Instance()->AtkInputManager->TextInput;
        if (textInput->CompletionDepth != 0) return;

        var textService = textInput->TextService;
        var isComposing = (delegate* unmanaged<TextService*, bool>)(*(void***)textService)[17];
        if (isComposing(textService)) return;

        var inputEvent     = new AtkEvent();
        var inputEventData = new AtkEventData();
        inputEventData.InputData.InputId  = (int)SeVirtualKey.RETURN;
        inputEventData.InputData.State    = InputState.Down;
        inputEventData.InputData.Modifier = ModifierFlag.Ctrl;
        base.OnReceiveEvent(thisPtr, AtkEventType.InputReceived, 0x300, &inputEvent, &inputEventData);
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        if (!isUpdatingHeight && MathF.Abs(Height - lastHeight) >= 0.5f)
            minimumHeight = Height;

        lastHeight = Height;
        UpdateHeightForContent();
    }
}
