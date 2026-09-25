using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;

namespace OmenTools.KamiToolKit.Addons.InputString;

public sealed unsafe class InputStringAddon : NativeAddon
{
    public static InputStringAddon Open
    (
        InputStringAddonOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(options);
        ValidateOptions(options);

        var addon = new InputStringAddon(options)
        {
            InternalName              = "DRInputString",
            Title                     = string.Empty,
            Size                      = new Vector2(WINDOW_WIDTH, 154.0f),
            OpenInBounds              = true,
            OpenWindowSoundEffectId   = options.OpenSoundEffectID,
            RespectCloseAll           = options.RespectCloseAll,
            DisableClamping           = false,
            EnableContextMenu         = false,
            DisableScaleContextOption = true,
            RememberClosePosition     = false,
            ParentAddonId             = options.ParentID,
            BlockedParentAddonId      = options.BlockedParentID,
            CreateWindowNode = () =>
            {
                var window = new WindowNode();
                window.ShowCloseButton               = true;
                window.ShowConfigButton              = false;
                window.ShowHelpButton                = false;
                window.HeaderContainerNode.IsVisible = true;
                window.HeaderCollisionNode.IsVisible = true;
                window.TitleNode.IsVisible           = false;
                window.SubtitleNode.IsVisible        = false;
                window.DividingLineNode.IsVisible    = false;
                return window;
            }
        };

        addon.Open();
        return addon;
    }

    /// <summary>
    /// Gets the text currently entered.
    /// </summary>
    public string Value
        => InputNode?.String.ToString() ?? string.Empty;

    protected override void OnSetup
    (
        AtkUnitBase*   addon,
        Span<AtkValue> atkValueSpan
    )
    {
        hasResult = false;

        if (WindowNode is WindowNode window)
        {
            window.ShowCloseButton               = true;
            window.HeaderContainerNode.IsVisible = true;
            window.HeaderCollisionNode.IsVisible = true;
            window.TitleNode.IsVisible           = false;
            window.SubtitleNode.IsVisible        = false;
            window.DividingLineNode.IsVisible    = false;
        }

        PromptNode = new TextNode
        {
            Size             = new(PROMPT_WIDTH, 0.0f),
            TextColor        = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
            FontSize         = 14,
            FontType         = FontType.Axis,
            LineSpacing      = 21,
            AlignmentType    = options.PromptAlignment
        };
        PromptNode.AddTextFlags(TextFlags.WordWrap, TextFlags.MultiLine);
        PromptNode.AttachNode(this);

        LabelNode = new TextNode
        {
            Size             = new(LABEL_WIDTH, LABEL_HEIGHT),
            TextColor        = ColorHelper.GetColor(3),
            TextOutlineColor = ColorHelper.GetColor(7),
            FontSize         = 12,
            FontType         = FontType.Axis
        };
        LabelNode.AttachNode(this);

        InputNode = new TextInputNode();
        InputNode.AttachNode(this);

        InputNode.OnInputComplete = _ => Select(InputStringAddonResult.Confirmed);
        InputNode.OnEscapeEntered = () => Select(InputStringAddonResult.Cancelled);

        ConfirmButton = new TextButtonNode
        {
            Size    = new(BUTTON_WIDTH, BUTTON_HEIGHT),
            OnClick = () => Select(InputStringAddonResult.Confirmed)
        };
        ConfirmButton.AttachNode(this);

        CancelButton = new TextButtonNode
        {
            Size    = new(BUTTON_WIDTH, BUTTON_HEIGHT),
            OnClick = () => Select(InputStringAddonResult.Cancelled)
        };
        CancelButton.AttachNode(this);

        ApplyOptions(options);
    }

    protected override void OnHide
    (
        AtkUnitBase* addon
    )
    {
        if (hasResult)
            return;

        hasResult = true;
        IFramework.Instance().RunOnTick(() => options.Callback?.Invoke(this, InputStringAddonResult.Closed), delayTicks: 1);
    }

    protected override void OnUpdate
    (
        AtkUnitBase* addon
    )
    {
        if (openPosition == null) return;

        SetWindowPosition(openPosition.Value);
        openPosition = null;
    }

    protected override void OnFinalize
    (
        AtkUnitBase* addon
    )
    {
        PromptNode    = null;
        LabelNode     = null;
        InputNode     = null;
        ConfirmButton = null;
        CancelButton  = null;
    }

    private InputStringAddon
    (
        InputStringAddonOptions options
    )
        => this.options = options;

    private void ApplyOptions
    (
        InputStringAddonOptions dialogOptions
    )
    {
        if (PromptNode is null || LabelNode is null || InputNode is null || ConfirmButton is null || CancelButton is null)
            return;

        PromptNode.AlignmentType = dialogOptions.PromptAlignment;
        PromptNode.String        = dialogOptions.Prompt;

        var hasLabel = dialogOptions.Label is not null;

        LabelNode.IsVisible = hasLabel;
        LabelNode.String    = dialogOptions.Label ?? string.Empty;

        SetButtonText(ConfirmButton, dialogOptions.ConfirmButtonText, 572);
        SetButtonText(CancelButton,  dialogOptions.CancelButtonText,  2);

        InputNode.String            = dialogOptions.Value ?? string.Empty;
        InputNode.PlaceholderString = dialogOptions.Placeholder;
        InputNode.MaxCharacters     = dialogOptions.MaxCharacters;
        InputNode.ShowLimitText     = dialogOptions.MaxCharacters > 0;

        PromptNode.Size = new(PROMPT_WIDTH, 0.0f);
        var promptHeight = PromptNode.GetTextDrawSize().Y;

        var labelY  = PROMPT_Y + promptHeight + PROMPT_TO_LABEL_GAP;
        var inputY  = hasLabel ? labelY + LABEL_TO_INPUT_GAP : labelY;
        var buttonY = inputY + INPUT_HEIGHT + INPUT_TO_BUTTON_GAP;
        var height  = buttonY + BUTTON_HEIGHT + BOTTOM_PADDING;

        SetWindowSize(WINDOW_WIDTH, height);

        PromptNode.Size     = new Vector2(PROMPT_WIDTH, promptHeight);
        PromptNode.Position = new Vector2(PROMPT_X,     PROMPT_Y);

        LabelNode.Size     = new Vector2(LABEL_WIDTH, LABEL_HEIGHT);
        LabelNode.Position = new Vector2(LABEL_X,     labelY);

        InputNode.Size     = new Vector2(INPUT_WIDTH, INPUT_HEIGHT);
        InputNode.Position = new Vector2(INPUT_X,     inputY);

        var totalButtonWidth = (BUTTON_WIDTH * 2.0f) + BUTTON_GAP;
        var buttonLeft       = (WINDOW_WIDTH - totalButtonWidth) / 2.0f;

        ConfirmButton.Position = new Vector2(buttonLeft, buttonY);
        CancelButton.Position  = new Vector2(buttonLeft + BUTTON_WIDTH + BUTTON_GAP, buttonY);

        ConfirmButton.NavIndex = 1;
        ConfirmButton.NavLeft  = 2;
        ConfirmButton.NavRight = 2;
        CancelButton.NavIndex  = 2;
        CancelButton.NavLeft   = 1;
        CancelButton.NavRight  = 1;

        if (InternalAddon is not null)
            InternalAddon->FocusNode = InputNode;

        var screenSize  = (Vector2)AtkStage.Instance()->ScreenSize;
        var maxPosition = Vector2.Max(Vector2.Zero, screenSize - Size);
        var position = options.Position is { } addonPosition ?
                           Vector2.Clamp(GetWindowPosition(addonPosition, RootNode.Node->GetNodeState().Size), Vector2.Zero, maxPosition) :
                           maxPosition / 2.0f;
        openPosition = position;
    }

    private static Vector2 GetWindowPosition
    (
        AddonPosition addonPosition,
        Vector2       addonSize
    )
    {
        var offset = addonPosition.Alignment switch
        {
            AddonPositionAlignment.TopLeft      => Vector2.Zero,
            AddonPositionAlignment.TopCenter    => new(addonSize.X / 2.0f, 0.0f),
            AddonPositionAlignment.TopRight     => addonSize with { Y = 0.0f },
            AddonPositionAlignment.RightCenter  => addonSize with { Y = addonSize.Y / 2.0f },
            AddonPositionAlignment.BottomRight  => new(addonSize.X, addonSize.Y),
            AddonPositionAlignment.BottomCenter => addonSize with { X = addonSize.X / 2.0f },
            AddonPositionAlignment.BottomLeft   => addonSize with { X = 0.0f },
            AddonPositionAlignment.LeftCenter   => new(0.0f, addonSize.Y / 2.0f),
            AddonPositionAlignment.Center       => new(addonSize.X / 2.0f, addonSize.Y / 2.0f),
            _                                   => Vector2.Zero
        };

        return addonPosition.Position - offset;
    }

    private static void SetButtonText
    (
        TextButtonNode    button,
        ReadOnlySeString? text,
        uint              defaultTextID
    )
    {
        button.TextId = 0;
        button.String = text ?? ISeStringEvaluator.Instance().EvaluateFromAddon(defaultTextID, []);
    }

    private void Select
    (
        InputStringAddonResult result
    )
    {
        if (hasResult)
            return;

        hasResult = true;
        var callback = options.Callback;
        Close();
        callback?.Invoke(this, result);
    }

    private static void ValidateOptions
    (
        InputStringAddonOptions options
    )
    {
        if (options.MaxCharacters < 0)
            throw new ArgumentOutOfRangeException(nameof(options.MaxCharacters));

        if (options.Position?.Position is { } position && (!float.IsFinite(position.X) || !float.IsFinite(position.Y)))
            throw new ArgumentOutOfRangeException(nameof(options.Position));
    }

    private const float WINDOW_WIDTH         = 320.0f;
    private const float PROMPT_X             = 17.0f;
    private const float PROMPT_Y             = 15.0f;
    private const float PROMPT_WIDTH         = 276.0f;
    private const float LABEL_X              = 18.0f;
    private const float LABEL_WIDTH          = 284.0f;
    private const float LABEL_HEIGHT         = 21.0f;
    private const float INPUT_X              = 17.0f;
    private const float INPUT_WIDTH          = 286.0f;
    private const float INPUT_HEIGHT         = 28.0f;
    private const float BUTTON_WIDTH         = 100.0f;
    private const float BUTTON_HEIGHT        = 28.0f;
    private const float BUTTON_GAP           = 8.0f;
    private const float PROMPT_TO_LABEL_GAP  = 18.0f;
    private const float LABEL_TO_INPUT_GAP   = 16.0f;
    private const float INPUT_TO_BUTTON_GAP  = 10.0f;
    private const float BOTTOM_PADDING       = 18.0f;

    private readonly InputStringAddonOptions options;
    private          Vector2?             openPosition;
    private          bool                 hasResult;

    private TextNode?       PromptNode    { get; set; }
    private TextNode?       LabelNode     { get; set; }
    private TextInputNode?  InputNode     { get; set; }
    private TextButtonNode? ConfirmButton { get; set; }
    private TextButtonNode? CancelButton  { get; set; }
}
