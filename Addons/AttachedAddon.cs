using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using OmenTools.Extensions;

namespace OmenTools.KamiToolKit.Addons;

public abstract class AttachedAddon : NativeAddon
{
    protected virtual AttachedAddonPosition AttachPosition =>
        AttachedAddonPosition.LeftTop;

    protected virtual Vector2 PositionOffset =>
        Vector2.Zero;

    /// <summary>
    /// 是否跟随 HostAddon 自动开关。<br/>
    /// 自动开关条件见：<seealso cref="CanOpenAddon"/>。<br/>
    /// 关闭此属性后，使用 Open、Close 或 Toggle 控制面板。
    /// </summary>
    /// <remarks>当 HostAddon 关闭时，一定会跟随关闭。</remarks>
    protected virtual bool AutoOpenAddon =>
        true;

    protected virtual bool CanOpenAddon =>
        true;

    protected override bool UsesWindowConfiguration =>
        false;

    protected override bool CloseOnHide =>
        false;

    /// <summary>
    /// 是否已请求打开，包括等待宿主就绪或旧窗口完成关闭的情况。
    /// </summary>
    public bool IsRequestedOpen { get; private set; }

    public override Vector2 ContentStartPosition =>
        new Vector2(4f, 4f) + ContentPadding;

    public override Vector2 ContentSize =>
        Size - new Vector2(8f, 16f) - (ContentPadding * 2f);

    protected unsafe AtkUnitBase* HostAddon =>
        (AtkUnitBase*)IGameGui.Instance().GetAddonByName(hostAddonName).Address;

    private readonly string hostAddonName;

    private TaskCompletionSource openCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private WindowBackgroundTextureNode? backgroundTexture;
    private SimpleImageNode?             backgroundImage;

    private bool isDisposed;
    private bool isClosing;
    private bool hostClosing;

    protected unsafe AttachedAddon
    (
        string              hostAddon,
        params AddonEvent[] hostAddonEvents
    )
    {
        hostAddonName           = hostAddon;
        HasWindowNode           = false;
        ContentPadding          = new Vector2(8f, 8f);
        RememberClosePosition   = false;
        OpenInBounds            = false;
        EnableContextMenu       = false;
        OpenWindowSoundEffectId = 0;

        foreach (var eventType in new[]
                 {
                     AddonEvent.PostSetup,
                     AddonEvent.PostOpen,
                     AddonEvent.PostDraw,
                     AddonEvent.PostShow,
                     AddonEvent.PostHide,
                     AddonEvent.PostClose,
                     AddonEvent.PreFinalize
                 }.Concat(hostAddonEvents).Distinct())
            IAddonLifecycle.Instance().RegisterListener(eventType, hostAddon, OnHostAddonLifecycle);
    }

    public override void Dispose()
    {
        if (!ReleaseHostEvents())
            return;

        base.Dispose();
        GC.SuppressFinalize(this);
    }

    public override async ValueTask DisposeAsync()
    {
        if (await IFramework.Instance().Run(ReleaseHostEvents))
            await base.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected virtual void OnHostAddon
    (
        AddonEvent type,
        AddonArgs? args
    )
    {
    }

    protected override unsafe void OnReceiveGlobalEvent
    (
        AtkUnitBase*  addon,
        AtkEventType  eventType,
        int           eventParam,
        AtkEvent*     atkEvent,
        AtkEventData* atkEventData
    )
    {
        if (eventType is not (AtkEventType.InputReceived or AtkEventType.InputNavigation))
        {
            base.OnReceiveGlobalEvent(addon, eventType, eventParam, atkEvent, atkEventData);
            return;
        }

        var host = HostAddon;
        if (isClosing || hostClosing || !host->IsAddonAndNodesReady())
            return;

        host->ReceiveGlobalEvent(eventType, eventParam, atkEvent, atkEventData);
    }

    /// <remarks>继承必须要调用 base.OnUpdate(...)</remarks>
    protected override unsafe void OnUpdate
    (
        AtkUnitBase* addon
    )
    {
        var host = HostAddon;
        if (isClosing || hostClosing || !host->IsAddonAndNodesReady())
            return;

        addon->UiFlags             = host->UiFlags;
        addon->IgnoreUIDisplayMode = host->IgnoreUIDisplayMode;

        if (addon->DepthLayer != host->DepthLayer)
            addon->SetDepthLayer(host->DepthLayer);

        if (MathF.Abs(addon->Scale - host->Scale) > 0.0001f)
            addon->SetScale(host->Scale / AtkUnitBase.GetGlobalUIScale(), true);

        if (addon->Alpha != host->Alpha)
            addon->SetAlpha(host->Alpha);

        if (!host->IsVisible)
        {
            if (addon->IsVisible)
                addon->Hide(true, false, 1);
            return;
        }

        if (IsRequestedOpen && !addon->IsVisible)
            addon->Show(true, 1);

        if (!addon->IsReady)
            return;

        openCompletion.TrySetResult();

        if (backgroundTexture is null)
        {
            backgroundTexture = new WindowBackgroundTextureNode(false)
            {
                Offsets         = new(64f, 32f, 32f, 32f),
                NodeFlags       = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill,
                PartsRenderType = 19,
                Size            = Size
            };
            backgroundTexture.AttachNode(this, NodePosition.AsFirstChild);

            backgroundImage = new SimpleImageNode
            {
                IsVisible          = true,
                WrapMode           = WrapMode.Stretch,
                TexturePath        = "ui/uld/WindowA_Gradation.tex",
                TextureCoordinates = new(6f, 2f),
                TextureSize        = new(24f, 24f),
                Position           = new(4f, 4f)
            };
            backgroundImage.AttachNode(backgroundTexture);
        }

        var hostSize  = new Vector2(host->GetScaledWidth(true),  host->GetScaledHeight(true))  / addon->Scale;
        var addonSize = new Vector2(addon->GetScaledWidth(true), addon->GetScaledHeight(true)) / addon->Scale;

        var position = AttachPosition switch
        {
            AttachedAddonPosition.LeftTop      => new(-addonSize.X, 0),
            AttachedAddonPosition.LeftCenter   => new(-addonSize.X, (hostSize.Y - addonSize.Y) / 2f),
            AttachedAddonPosition.LeftBottom   => new(-addonSize.X, hostSize.Y - addonSize.Y),
            AttachedAddonPosition.TopLeft      => new(0, -addonSize.Y),
            AttachedAddonPosition.TopCenter    => new((hostSize.X - addonSize.X) / 2f, -addonSize.Y),
            AttachedAddonPosition.TopRight     => new(hostSize.X - addonSize.X, -addonSize.Y),
            AttachedAddonPosition.RightTop     => hostSize with { Y = 0 },
            AttachedAddonPosition.RightCenter  => hostSize with { Y = (hostSize.Y - addonSize.Y) / 2f },
            AttachedAddonPosition.RightBottom  => hostSize with { Y = hostSize.Y - addonSize.Y },
            AttachedAddonPosition.BottomLeft   => hostSize with { X = 0 },
            AttachedAddonPosition.BottomCenter => hostSize with { X = (hostSize.X - addonSize.X) / 2f },
            AttachedAddonPosition.BottomRight  => hostSize with { X = hostSize.X - addonSize.X },
            _                                  => Vector2.Zero
        };

        var hostPosition = host->RootNode == null ?
                               new Vector2(host->X,           host->Y) :
                               new Vector2(host->RootNode->X, host->RootNode->Y);
        SetWindowPosition(hostPosition + (position * addon->Scale) + PositionOffset);
        var backgroundHeight = MathF.Max(Size.Y, backgroundTexture.PartsList[0]->Height + backgroundTexture.PartsList[6]->Height);
        backgroundTexture.Size   = Size with { Y = backgroundHeight };
        backgroundTexture.ScaleY = Size.Y / backgroundHeight;
        backgroundImage?.Size    = Vector2.Max(backgroundTexture.Size - new Vector2(8f, 16f), Vector2.Zero);
    }

    /// <remarks>继承必须要调用 base.OnFinalize(...)</remarks>
    protected override unsafe void OnFinalize
    (
        AtkUnitBase* addon
    )
    {
        if (!isClosing)
        {
            IsRequestedOpen = false;
            openCompletion.TrySetCanceled();
        }

        isClosing = true;
        backgroundImage?.Dispose();
        backgroundImage = null;

        backgroundTexture?.Dispose();
        backgroundTexture = null;
    }

    private unsafe void OnHostAddonLifecycle
    (
        AddonEvent type,
        AddonArgs? args
    )
    {
        OnHostAddon(type, args);

        switch (type)
        {
            case AddonEvent.PostSetup:
            case AddonEvent.PostOpen:
                hostClosing = false;
                break;

            case AddonEvent.PostClose:
            case AddonEvent.PreFinalize:
                hostClosing = true;
                Close();
                break;

            case AddonEvent.PostHide when args is AddonHideArgs hideArgs:
                if (IsAllocated && !isClosing)
                    InternalAddon->Hide(true, false, hideArgs.SetShowHideFlags);
                break;

            case AddonEvent.PostShow when args is AddonShowArgs showArgs:
                if (IsRequestedOpen && IsAllocated && !isClosing)
                    InternalAddon->Show(true, showArgs.UnsetShowHideFlags);
                break;

            case AddonEvent.PostDraw:
                if (AutoOpenAddon)
                {
                    if (CanOpenAddon)
                        Open();
                    else if (IsRequestedOpen)
                        Close();
                }
                else if (IsRequestedOpen && !IsAllocated)
                    Open();

                break;
        }
    }

    public override unsafe void Open()
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        if (!IsRequestedOpen)
            openCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        IsRequestedOpen = true;

        var host = HostAddon;
        if (IsAllocated || hostClosing || !host->IsAddonAndNodesReady() || !host->IsVisible)
            return;

        isClosing = false;
        AllocateAddon();

        if (InternalAddon == null)
        {
            IsRequestedOpen = false;
            openCompletion.TrySetException(new InvalidOperationException("Unable to allocate the attached addon."));
            return;
        }

        InternalAddon->DisableUnfocusedCloseOnEsc  = true;
        InternalAddon->DisableFocusOnShow          = true;
        InternalAddon->DisableCloseOnLoadScreen    = true;
        InternalAddon->DisableShowHideSoundEffects = true;
        InternalAddon->UiFlags                     = host->UiFlags;
        InternalAddon->IgnoreUIDisplayMode         = host->IgnoreUIDisplayMode;
        InternalAddon->SetScale(host->Scale / AtkUnitBase.GetGlobalUIScale(), true);
        InternalAddon->Open(host->DepthLayer);
    }

    public override async Task OpenAsync()
    {
        await IFramework.Instance().Run(Open);
        await openCompletion.Task;
    }

    public override void Close()
    {
        IsRequestedOpen = false;
        openCompletion.TrySetCanceled();
        if (!IsAllocated || isClosing)
            return;

        isClosing = true;
        base.Close();
    }

    public override void Toggle()
    {
        if (IsRequestedOpen)
            Close();
        else
            Open();
    }

    private bool ReleaseHostEvents()
    {
        if (isDisposed)
            return false;

        isDisposed      = true;
        IsRequestedOpen = false;
        openCompletion.TrySetCanceled();
        IAddonLifecycle.Instance().UnregisterListener(OnHostAddonLifecycle);
        return true;
    }

    public enum AttachedAddonPosition
    {
        LeftTop,
        LeftCenter,
        LeftBottom,
        TopLeft,
        TopCenter,
        TopRight,
        RightTop,
        RightCenter,
        RightBottom,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}
