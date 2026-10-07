using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Controllers;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using OmenTools.Extensions;

namespace OmenTools.KamiToolKit.Addons;

public abstract class AttachedAddon : NativeChildAddon
{
    protected virtual AttachedAddonPosition AttachPosition =>
        AttachedAddonPosition.LeftTop;

    protected virtual Vector2 PositionOffset =>
        Vector2.Zero;

    protected virtual bool CanOpenAddon =>
        true;

    public override Vector2 ContentStartPosition =>
        new Vector2(4f, 4f) + ContentPadding;

    public override Vector2 ContentSize =>
        Size - new Vector2(8f, 16f) - (ContentPadding * 2f);

    protected unsafe AtkUnitBase* HostAddon =>
        Controller.ParentAddon;

    private WindowBackgroundTextureNode? backgroundTexture;
    private SimpleImageNode?             backgroundImage;

    private bool isDisposed;

    protected unsafe AttachedAddon
    (
        string              hostAddon,
        params AddonEvent[] hostAddonEvents
    ) : base(NativeAddonController.GetOrCreate(hostAddon))
    {
        ContentPadding = new Vector2(8f, 8f);

        foreach (var eventType in new[] { AddonEvent.PostDraw, AddonEvent.PostClose, AddonEvent.PreFinalize }.Concat(hostAddonEvents).Distinct())
            IAddonLifecycle.Instance().RegisterListener(eventType, hostAddon, OnHostAddonLifecycle);

        IFramework.Instance().RunOnTick
        (() =>
            {
                if (isDisposed || !HostAddon->IsAddonAndNodesReady())
                    return;

                if (hostAddonEvents.Contains(AddonEvent.PostSetup))
                    OnHostAddon(AddonEvent.PostSetup, null);

                if (CanOpenAddon)
                    Open();
            }
        );
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

    public override unsafe void Close()
    {
        var hostAddon = HostAddon;
        var closeHost = !isDisposed && IsRequestedOpen && hostAddon is not null;
        base.Close();

        if (closeHost && HostAddon == hostAddon)
            hostAddon->Close(true);
    }

    protected virtual void OnHostAddon
    (
        AddonEvent type,
        AddonArgs? args
    )
    {
    }

    protected virtual unsafe void OnAttachedAddonUpdate
    (
        AtkUnitBase* addon,
        AtkUnitBase* hostAddon
    )
    {
    }

    protected virtual unsafe void OnAttachedAddonFinalize
    (
        AtkUnitBase* addon
    )
    {
    }

    protected sealed override unsafe void OnUpdate
    (
        AtkUnitBase* addon
    )
    {
        var hostAddon = HostAddon;

        if (!hostAddon->IsAddonAndNodesReady())
            return;

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
                TextureCoordinates = new(6f,  2f),
                TextureSize        = new(24f, 24f),
                Position           = new(4f,  4f)
            };
            backgroundImage.AttachNode(backgroundTexture, NodePosition.AfterTarget);
        }

        var hostSize  = new Vector2(hostAddon->GetScaledWidth(true), hostAddon->GetScaledHeight(true)) / addon->Scale;
        var addonSize = new Vector2(addon->GetScaledWidth(true),     addon->GetScaledHeight(true))     / addon->Scale;

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

        LocalPosition = position + (PositionOffset / addon->Scale);
        OnAttachedAddonUpdate(addon, hostAddon);
        backgroundTexture.Size = Size;
        backgroundImage?.Size  = Vector2.Max(Size - new Vector2(8f, 16f), Vector2.Zero);
    }

    protected sealed override unsafe void OnFinalize
    (
        AtkUnitBase* addon
    )
    {
        backgroundImage?.Dispose();
        backgroundImage = null;
        
        backgroundTexture?.Dispose();
        backgroundTexture = null;
        
        OnAttachedAddonFinalize(addon);
    }

    private void OnHostAddonLifecycle
    (
        AddonEvent type,
        AddonArgs? args
    )
    {
        OnHostAddon(type, args);

        switch (type)
        {
            case AddonEvent.PostDraw when !IsRequestedOpen && CanOpenAddon:
                Open();
                break;
            case AddonEvent.PostClose:
            case AddonEvent.PreFinalize:
                Close();
                break;
        }
    }

    private bool ReleaseHostEvents()
    {
        if (isDisposed)
            return false;

        isDisposed = true;
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
