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

namespace DailyRoutines.Common.KamiToolKit.Addons.SelectYesno;

public sealed unsafe class DRSelectYesno : NativeAddon
{
    public static DRSelectYesno Open
    (
        DRSelectYesnoOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(options);
        ValidateOptions(options);

        var addon = new DRSelectYesno(options)
        {
            InternalName              = "DRSelectYesno",
            Title                     = string.Empty,
            Size                      = new Vector2(400.0f, 96.0f),
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
            Size          = new(344.0f, 0.0f),
            TextColor     = ColorHelper.GetColor(1),
            FontSize      = 14,
            FontType      = FontType.Axis,
            LineSpacing   = 18,
            AlignmentType = options.PromptAlignment
        };
        PromptNode.AddTextFlags(TextFlags.WordWrap, TextFlags.MultiLine);
        PromptNode.AttachNode(this);

        PrimaryButton = new TextButtonNode
        {
            Size    = new(100.0f, 28.0f),
            OnClick = () => Select(DRSelectYesnoResult.Yes)
        };
        PrimaryButton.AttachNode(this);

        SecondaryButton = new TextButtonNode
        {
            Size    = new(100.0f, 28.0f),
            OnClick = () => Select(DRSelectYesnoResult.No)
        };
        SecondaryButton.AttachNode(this);

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
        IFramework.Instance().RunOnTick(() => options.Callback?.Invoke(this, DRSelectYesnoResult.Closed), delayTicks: 1);
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
        PromptNode      = null;
        PrimaryButton   = null;
        SecondaryButton = null;
    }

    private DRSelectYesno
    (
        DRSelectYesnoOptions options
    )
        => this.options = options;

    private void ApplyOptions
    (
        DRSelectYesnoOptions dialogOptions
    )
    {
        if (PromptNode is null || PrimaryButton is null || SecondaryButton is null)
            return;

        PromptNode.AlignmentType = dialogOptions.PromptAlignment;
        PromptNode.String        = dialogOptions.Prompt;

        SetButtonText(PrimaryButton,   dialogOptions.YesButtonText, 3);
        SetButtonText(SecondaryButton, dialogOptions.NoButtonText,  4);

        var buttons       = dialogOptions.Buttons;
        var showPrimary   = (buttons & DRSelectYesnoButtons.Yes) != 0;
        var showSecondary = (buttons & DRSelectYesnoButtons.No)  != 0;

        PrimaryButton.IsVisible   = showPrimary;
        SecondaryButton.IsVisible = showSecondary;

        PromptNode.Size = new(344.0f, 0.0f);
        var promptHeight = PromptNode.GetTextDrawSize().Y;

        var buttonCount = (showPrimary ?
                               1 :
                               0) +
                          (showSecondary ?
                               1 :
                               0);
        var buttonHeight = buttonCount == 0 ?
                               0.0f :
                               28.0f;
        var buttonSpacing = buttonCount == 0 ?
                                0.0f :
                                8.0f;
        var height = MathF.Max(96.0f, 20.0f + promptHeight + buttonSpacing + buttonHeight + 18.0f);

        SetWindowSize(400.0f, height);

        PromptNode.Size     = new Vector2(344.0f, promptHeight);
        PromptNode.Position = new Vector2(28.0f,  20.0f);

        const float BUTTON_WIDTH = 100.0f;
        const float BUTTON_GAP   = 8.0f;

        var totalButtonWidth = buttonCount == 2 ?
                                   (BUTTON_WIDTH * 2.0f) + BUTTON_GAP :
                                   BUTTON_WIDTH;
        var buttonLeft = (400.0f - totalButtonWidth) / 2.0f;
        var buttonY    = height - buttonHeight - 18.0f;

        var visibleButtons = new List<TextButtonNode>(buttonCount);
        if (showPrimary)
            visibleButtons.Add(PrimaryButton);
        if (showSecondary)
            visibleButtons.Add(SecondaryButton);

        for (var index = 0; index < visibleButtons.Count; index++)
        {
            var button          = visibleButtons[index];
            var navigationIndex = index + 1;
            var previousIndex = index == 0 ?
                                    visibleButtons.Count :
                                    index;
            var nextIndex = index == visibleButtons.Count - 1 ?
                                1 :
                                index + 2;

            button.Position = new Vector2(buttonLeft + (index * (BUTTON_WIDTH + BUTTON_GAP)), buttonY);
            button.NavIndex = navigationIndex;
            button.NavLeft  = previousIndex;
            button.NavRight = nextIndex;
        }

        if (!showPrimary)
            PrimaryButton.NavIndex = 0;
        if (!showSecondary)
            SecondaryButton.NavIndex = 0;

        if (InternalAddon is not null)
            InternalAddon->FocusNode = showPrimary ? PrimaryButton : showSecondary ? SecondaryButton : RootNode;

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
            AddonPositionAlignment.TopLeft     => Vector2.Zero,
            AddonPositionAlignment.TopCenter   => new(addonSize.X / 2.0f, 0.0f),
            AddonPositionAlignment.TopRight    => addonSize with { Y = 0.0f },
            AddonPositionAlignment.RightCenter => addonSize with { Y = addonSize.Y / 2.0f },
            AddonPositionAlignment.BottomRight => new(addonSize.X, addonSize.Y),
            AddonPositionAlignment.BottomCenter => addonSize with { X = addonSize.X / 2.0f },
            AddonPositionAlignment.BottomLeft  => addonSize with { X = 0.0f },
            AddonPositionAlignment.LeftCenter  => new(0.0f, addonSize.Y / 2.0f),
            AddonPositionAlignment.Center      => new(addonSize.X / 2.0f, addonSize.Y / 2.0f),
            _                                  => Vector2.Zero
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
        DRSelectYesnoResult result
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
        DRSelectYesnoOptions options
    )
    {
        if ((options.Buttons & ~DRSelectYesnoButtons.Both) != 0)
            throw new ArgumentOutOfRangeException(nameof(options.Buttons));

        if (options.Position?.Position is { } position && (!float.IsFinite(position.X) || !float.IsFinite(position.Y)))
            throw new ArgumentOutOfRangeException(nameof(options.Position));
    }

    private readonly DRSelectYesnoOptions options;
    private          Vector2?             openPosition;
    private          bool                 hasResult;

    private TextNode?       PromptNode      { get; set; }
    private TextButtonNode? PrimaryButton   { get; set; }
    private TextButtonNode? SecondaryButton { get; set; }
}
