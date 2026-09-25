using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text.ReadOnly;

namespace DailyRoutines.Common.KamiToolKit.Nodes;

public class CollaspingHeaderNode : ResNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode DecorationNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode LabelNode { get; }

    /// <summary>
    /// Gets or sets the displayed label.
    /// </summary>
    public ReadOnlySeString String {
        get => LabelNode.String;
        set => LabelNode.String = value;
    }

    /// <summary>
    /// Constructs a new <see cref="CollaspingHeaderNode"/>
    /// </summary>
    public CollaspingHeaderNode() {
        DecorationNode = new SimpleNineGridNode {
            TexturePath        = "ui/uld/journal_Separator.tex",
            TextureCoordinates = new Vector2(0.0f,   0.0f),
            TextureSize        = new Vector2(424.0f, 24.0f),
            Size               = new Vector2(24.0f,  24.0f),
            LeftOffset         = 25.0f,
            RightOffset        = 20.0f,
        };
        DecorationNode.AttachNode(this);

        LabelNode = new TextNode {
            Position      = new Vector2(22.0f, 1.0f),
            TextColor     = ColorHelper.GetColor(7),
            AlignmentType = AlignmentType.Left,
            FontSize      = 12,
            FontType      = FontType.Axis,
        };
        LabelNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        DecorationNode.Size = Size;
        LabelNode.Size      = new Vector2(Width - 22.0f, Height);
    }
}
