using System.Numerics;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.Common.KamiToolKit.Addons.InputNumeric;

public sealed unsafe class DRInputNumeric : NativeAddon
{
    public static DRInputNumeric Open
    (
        DRInputNumericOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(options);
        ValidateOptions(options);

        var addon = new DRInputNumeric(options)
        {
            InternalName              = "DRInputNumeric",
            Title                     = string.Empty,
            Size                      = new Vector2(280.0f, 130.0f),
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
    /// Gets the current value of the numeric input.
    /// </summary>
    public int Value
        => InputNode?.Value ?? 0;

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
            Size             = new(224.0f, 0.0f),
            TextColor        = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
            FontSize         = 12,
            FontType         = FontType.Axis,
            LineSpacing      = 18,
            AlignmentType    = options.PromptAlignment
        };
        PromptNode.AddTextFlags(TextFlags.WordWrap, TextFlags.MultiLine);
        PromptNode.AttachNode(this);

        InputNode = new NumericInputNode();
        InputNode.AttachNode(this);

        ConfirmButton = new TextButtonNode
        {
            Size    = new(100.0f, 28.0f),
            OnClick = () => Select(DRInputNumericResult.Confirmed)
        };
        ConfirmButton.AttachNode(this);

        CancelButton = new TextButtonNode
        {
            Size    = new(100.0f, 28.0f),
            OnClick = () => Select(DRInputNumericResult.Cancelled)
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
        IFramework.Instance().RunOnTick(() => options.Callback?.Invoke(this, DRInputNumericResult.Closed), delayTicks: 1);
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
        InputNode     = null;
        ConfirmButton = null;
        CancelButton  = null;
    }

    private DRInputNumeric
    (
        DRInputNumericOptions options
    )
        => this.options = options;

    private void ApplyOptions
    (
        DRInputNumericOptions dialogOptions
    )
    {
        if (PromptNode is null || InputNode is null || ConfirmButton is null || CancelButton is null)
            return;

        PromptNode.AlignmentType = dialogOptions.PromptAlignment;
        PromptNode.String        = dialogOptions.Prompt;

        SetButtonText(ConfirmButton, dialogOptions.ConfirmButtonText, 572);
        SetButtonText(CancelButton,  dialogOptions.CancelButtonText,  2);

        InputNode.Min   = dialogOptions.Min;
        InputNode.Max   = dialogOptions.Max;
        InputNode.Step  = dialogOptions.Step;
        InputNode.Value = dialogOptions.Value;

        PromptNode.Size = new(224.0f, 0.0f);
        var promptHeight = PromptNode.GetTextDrawSize().Y;

        var inputY  = 22.0f + promptHeight + 8.0f;
        var buttonY = inputY + 28.0f + 10.0f;
        var height  = buttonY + 28.0f + 18.0f;

        SetWindowSize(280.0f, height);

        PromptNode.Size     = new Vector2(224.0f, promptHeight);
        PromptNode.Position = new Vector2(28.0f,  22.0f);

        InputNode.Size     = new Vector2(146.0f, 28.0f);
        InputNode.Position = new Vector2(67.0f,  inputY);

        const float BUTTON_WIDTH = 100.0f;
        const float BUTTON_GAP   = 8.0f;

        var totalButtonWidth = (BUTTON_WIDTH * 2.0f) + BUTTON_GAP;
        var buttonLeft       = (280.0f - totalButtonWidth) / 2.0f;

        ConfirmButton.Position = new Vector2(buttonLeft,  buttonY);
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
        DRInputNumericResult result
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
        DRInputNumericOptions options
    )
    {
        if (options.Min > options.Max)
            throw new ArgumentOutOfRangeException(nameof(options.Min));

        if (options.Step <= 0)
            throw new ArgumentOutOfRangeException(nameof(options.Step));

        if (options.Position?.Position is { } position && (!float.IsFinite(position.X) || !float.IsFinite(position.Y)))
            throw new ArgumentOutOfRangeException(nameof(options.Position));
    }

    private readonly DRInputNumericOptions options;
    private          Vector2?             openPosition;
    private          bool                 hasResult;

    private TextNode?         PromptNode    { get; set; }
    private NumericInputNode? InputNode     { get; set; }
    private TextButtonNode?   ConfirmButton { get; set; }
    private TextButtonNode?   CancelButton  { get; set; }
}
