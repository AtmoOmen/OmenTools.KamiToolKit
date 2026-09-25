using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;
using OmenTools.Interop.Game.Models;

namespace OmenTools.KamiToolKit.Addons.InputString;

public sealed record InputStringAddonOptions
{
    public required ReadOnlySeString Prompt { get; init; }

    public ReadOnlySeString? Label { get; init; }

    public ReadOnlySeString? Value { get; init; }

    public string? Placeholder { get; init; }

    public int MaxCharacters { get; init; }

    public AlignmentType PromptAlignment { get; init; } = AlignmentType.Left;

    public ReadOnlySeString? ConfirmButtonText { get; init; }

    public ReadOnlySeString? CancelButtonText { get; init; }

    public bool RespectCloseAll { get; init; } = true;

    public AddonPosition? Position { get; init; }

    public int OpenSoundEffectID { get; init; } = 23;

    public ushort ParentID { get; init; }

    public ushort BlockedParentID { get; init; }

    public Action<InputStringAddon, InputStringAddonResult>? Callback { get; init; }
}
