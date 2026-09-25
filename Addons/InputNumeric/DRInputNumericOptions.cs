using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.Common.KamiToolKit.Addons.InputNumeric;

public sealed record DRInputNumericOptions
{
    public required ReadOnlySeString Prompt { get; init; }

    public int Value { get; init; }

    public int Min { get; init; }

    public int Max { get; init; } = int.MaxValue;

    public int Step { get; init; } = 1;

    public AlignmentType PromptAlignment { get; init; } = AlignmentType.Left;

    public ReadOnlySeString? ConfirmButtonText { get; init; }

    public ReadOnlySeString? CancelButtonText { get; init; }

    public bool RespectCloseAll { get; init; } = true;

    public AddonPosition? Position { get; init; }

    public int OpenSoundEffectID { get; init; } = 23;

    public ushort ParentID { get; init; }

    public ushort BlockedParentID { get; init; }

    public Action<DRInputNumeric, DRInputNumericResult>? Callback { get; init; }
}