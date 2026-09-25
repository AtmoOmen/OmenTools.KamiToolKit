namespace DailyRoutines.Common.KamiToolKit.Addons.SelectYesno;

[Flags]
public enum DRSelectYesnoButtons : byte
{
    None = 0,
    Yes  = 1,
    No   = 2,
    Both = Yes | No
}
