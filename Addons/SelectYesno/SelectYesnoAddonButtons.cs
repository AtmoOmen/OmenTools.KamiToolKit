namespace OmenTools.KamiToolKit.Addons.SelectYesno;

[Flags]
public enum SelectYesnoAddonButtons : byte
{
    None = 0,
    Yes  = 1,
    No   = 2,
    Both = Yes | No
}
