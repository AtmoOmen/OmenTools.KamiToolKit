using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.Common.KamiToolKit.Addons.SelectYesno;

public sealed record DRSelectYesnoOptions
{
    public required ReadOnlySeString Prompt { get; init; }

    public DRSelectYesnoButtons Buttons { get; init; } = DRSelectYesnoButtons.Both;

    public ReadOnlySeString? YesButtonText { get; init; }

    public ReadOnlySeString? NoButtonText { get; init; }

    public AlignmentType PromptAlignment { get; init; } = AlignmentType.Left;

    public bool RespectCloseAll { get; init; } = true;

    public AddonPosition? Position { get; init; }

    public int OpenSoundEffectID { get; init; } = 23;
    
    public ushort ParentID { get; init; }
    
    public ushort BlockedParentID { get; init; }

    public Action<DRSelectYesno, DRSelectYesnoResult>? Callback { get; init; }
}
