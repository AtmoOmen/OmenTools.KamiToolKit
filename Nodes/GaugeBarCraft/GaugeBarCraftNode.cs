using System.Numerics;
using KamiToolKit.Enums;

namespace OmenTools.KamiToolKit.Nodes.GaugeBarCraft;

/// <summary>
///     Specialization of <see cref="GaugeBarNode" /> representing one of the synthesis windows gauges.
/// </summary>
public class GaugeBarCraftNode : GaugeBarNode
{
    private CraftGaugeType type;

    /// <summary>
    ///     Constructs a new <see cref="GaugeBarCraftNode" />.
    /// </summary>
    public GaugeBarCraftNode()
    {
        Size        = new Vector2(240.0f, 16.0f);
        TexturePath = "ui/uld/Synthesis.tex";

        ApplyType();
    }

    /// <summary>
    ///     Gets or sets which gauge texture this bar fills itself with.
    /// </summary>
    public CraftGaugeType Type
    {
        get => type;
        set
        {
            type = value;
            ApplyType();
        }
    }

    private void ApplyType()
        => FillV = type switch
        {
            CraftGaugeType.Quality => 48.0f,
            _                      => 60.0f
        };
}
