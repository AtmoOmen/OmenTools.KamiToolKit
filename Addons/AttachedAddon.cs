using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Helpers;

namespace OmenTools.KamiToolKit.Addons;

public abstract unsafe class AttachedAddon : NativeAddon
{
    protected virtual AttachedAddonPosition AttachPosition =>
        AttachedAddonPosition.LeftTop;

    protected virtual Vector2 PositionOffset =>
        Vector2.Zero;

    protected virtual bool CanOpenAddon =>
        true;

    protected AtkUnitBase* HostAddon =>
        AddonHelper.GetByName(hostAddonName);

    private readonly string hostAddonName;
    private readonly bool   runSetupForCurrentHostAddon;

    private bool isClosingAddonOnly;

    protected AttachedAddon
    (
        string              hostAddon,
        params AddonEvent[] hostAddonEvents
    )
    {
        hostAddonName               = hostAddon;
        runSetupForCurrentHostAddon = hostAddonEvents.Contains(AddonEvent.PostSetup);

        foreach (var eventType in new[] { AddonEvent.PostDraw, AddonEvent.PreFinalize }.Concat(hostAddonEvents).Distinct())
            IAddonLifecycle.Instance().RegisterListener(eventType, hostAddon, OnHostAddonLifecycle);

        IFramework.Instance().RunOnTick
        (() =>
            {
                if (!HostAddon->IsAddonAndNodesReady())
                    return;

                if (runSetupForCurrentHostAddon)
                    OnHostAddon(AddonEvent.PostSetup, null);

                if (CanOpenAddon)
                    OpenAddon();
            }
        );
    }

    public override void Dispose()
    {
        IAddonLifecycle.Instance().UnregisterListener(OnHostAddonLifecycle);

        isClosingAddonOnly = true;
        base.Dispose();
    }

    protected virtual void OnHostAddon
    (
        AddonEvent type,
        AddonArgs? args
    )
    {
    }

    protected virtual void OnAttachedAddonUpdate
    (
        AtkUnitBase* addon,
        AtkUnitBase* hostAddon
    )
    {
    }

    protected virtual void OnAttachedAddonFinalize
    (
        AtkUnitBase* addon
    )
    {
    }

    protected virtual bool CanCloseHostAddon
    (
        AtkUnitBase* hostAddon
    ) =>
        hostAddon != null && hostAddon->IsVisible;

    protected sealed override void OnUpdate
    (
        AtkUnitBase* addon
    )
    {
        var hostAddon = HostAddon;

        if (!HostAddon->IsAddonAndNodesReady())
        {
            CloseAddonOnly();
            return;
        }

        var hostPosition = new Vector2(hostAddon->RootNode->ScreenX,    hostAddon->RootNode->ScreenY);
        var hostSize     = new Vector2(hostAddon->GetScaledWidth(true), hostAddon->GetScaledHeight(true));
        var addonSize    = new Vector2(addon->GetScaledWidth(true),     addon->GetScaledHeight(true));

        var position = AttachPosition switch
        {
            AttachedAddonPosition.LeftTop      => new(hostPosition.X - addonSize.X, hostPosition.Y),
            AttachedAddonPosition.LeftCenter   => new(hostPosition.X - addonSize.X, hostPosition.Y              + (hostSize.Y - addonSize.Y) / 2f),
            AttachedAddonPosition.LeftBottom   => new(hostPosition.X - addonSize.X, hostPosition.Y + hostSize.Y - addonSize.Y),
            AttachedAddonPosition.TopLeft      => hostPosition with { Y = hostPosition.Y - addonSize.Y },
            AttachedAddonPosition.TopCenter    => new(hostPosition.X              + (hostSize.X - addonSize.X) / 2f, hostPosition.Y - addonSize.Y),
            AttachedAddonPosition.TopRight     => new(hostPosition.X + hostSize.X - addonSize.X, hostPosition.Y                     - addonSize.Y),
            AttachedAddonPosition.RightTop     => new(hostPosition.X              + hostSize.X, hostPosition.Y),
            AttachedAddonPosition.RightCenter  => new(hostPosition.X              + hostSize.X, hostPosition.Y              + (hostSize.Y - addonSize.Y) / 2f),
            AttachedAddonPosition.RightBottom  => new(hostPosition.X              + hostSize.X, hostPosition.Y + hostSize.Y - addonSize.Y),
            AttachedAddonPosition.BottomLeft   => hostPosition with { Y = hostPosition.Y + hostSize.Y },
            AttachedAddonPosition.BottomCenter => new(hostPosition.X              + (hostSize.X - addonSize.X) / 2f, hostPosition.Y + hostSize.Y),
            AttachedAddonPosition.BottomRight  => new(hostPosition.X + hostSize.X - addonSize.X, hostPosition.Y                     + hostSize.Y),
            _                                  => hostPosition
        };

        SetWindowPosition(position + PositionOffset);
        OnAttachedAddonUpdate(addon, hostAddon);
    }

    protected sealed override void OnFinalize
    (
        AtkUnitBase* addon
    )
    {
        OnAttachedAddonFinalize(addon);

        if (isClosingAddonOnly)
        {
            isClosingAddonOnly = false;
            return;
        }

        var hostAddon = HostAddon;
        if (!CanCloseHostAddon(hostAddon)) return;

        hostAddon->Close(true);
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
            case AddonEvent.PostDraw when CanOpenAddon:
                OpenAddon();
                break;
            case AddonEvent.PreFinalize:
                CloseAddonOnly();
                break;
        }
    }

    protected void CloseAddonOnly()
    {
        if (!IsOpen) return;

        isClosingAddonOnly = true;
        Close();
    }

    private void OpenAddon()
    {
        if (IsOpen || !HostAddon->IsAddonAndNodesReady()) return;

        Open();
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
