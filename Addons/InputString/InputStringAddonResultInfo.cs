using Lumina.Text.ReadOnly;

namespace OmenTools.KamiToolKit.Addons.InputString;

public record InputStringAddonResultInfo
(
    InputStringAddonResult Result,
    ReadOnlySeString       Value
);
