namespace OmenTools.KamiToolKit.Nodes.Background;

/// <summary>
///     Which part of <c>ui/uld/BgParts.tex</c> a <see cref="BackgroundNode" /> draws itself from.
/// </summary>
public enum BackgroundType
{
    /// <summary>
    ///     Gold frame with a hollow center, 32x32 at (1, 1).
    /// </summary>
    GoldFrame,

    /// <summary>
    ///     Opaque dark frame with a light center, 36x36 at (33, 1).
    /// </summary>
    InsetFrame,

    /// <summary>
    ///     Thin gold frame with a hollow center, 36x36 at (69, 1).
    /// </summary>
    GoldOutline,

    /// <summary>
    ///     Dark outlined panel split into two halves, 36x36 at (105, 1).
    /// </summary>
    SplitPanel,

    /// <summary>
    ///     Light outlined frame with a hollow center, 36x36 at (141, 1).
    /// </summary>
    LightFrame,

    /// <summary>
    ///     Even dark panel, 32x32 at (1, 33).
    /// </summary>
    SolidPanel,

    /// <summary>
    ///     Light gray panel, 28x28 at (33, 37).
    /// </summary>
    SoftPanel,

    /// <summary>
    ///     Light gray square, 16x16 at (61, 37).
    /// </summary>
    SoftDot,

    /// <summary>
    ///     Horizontal bar, 64x12 at (77, 37).
    /// </summary>
    HorizontalBar,

    /// <summary>
    ///     Light gray square, 16x16 at (141, 37).
    /// </summary>
    LightDot,

    /// <summary>
    ///     Dark panel, 32x32 at (85, 49).
    /// </summary>
    CenterPanel,

    /// <summary>
    ///     Light gray panel, 24x24 at (117, 49).
    /// </summary>
    CenterSoftPanel,

    /// <summary>
    ///     Even dark panel, 32x32 at (1, 65).
    /// </summary>
    FlatPanel,

    /// <summary>
    ///     Panel split into two halves, 32x32 at (33, 65).
    /// </summary>
    SplitFlatPanel,

    /// <summary>
    ///     Vertical bar, 20x32 at (65, 65).
    /// </summary>
    VerticalBar
}
