using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Enums;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes.Simplified;

namespace OmenTools.KamiToolKit.Nodes;

/// <summary>
///     Implementation of the games GaugeBar and associated component.
/// </summary>
public unsafe class GaugeBarNode : ComponentNode<AtkComponentGaugeBar, AtkUldComponentDataGaugeBar>
{
    /// <summary>
    ///     Constructs a new <see cref="GaugeBarNode" />.
    /// </summary>
    public GaugeBarNode()
    {
        SetInternalComponentType(ComponentType.GaugeBar);

        AddDrawFlags(DrawFlags.ClickableCursor);

        BackdropNode = CreateNineGridNode(0, 0, 64.0f, 16, 8, 8, 2, 3, 32.0f);
        IncreaseNode = CreateNineGridNode(0, 2, 64.0f, 12, 8, 8, 2, 3, 96.0f);
        DecreaseNode = CreateNineGridNode(0, 2, 64.0f, 12, 8, 8, 2, 3, 84.0f);
        FillNode     = CreateNineGridNode(0, 2, 64.0f, 12, 8, 8, 2, 3, 60.0f);
        BorderNode   = CreateNineGridNode(0, 2, 64.0f, 12, 8, 8, 2, 3, 108.0f);

        Data->Nodes[0] = FillNode.NodeId;
        Data->Nodes[1] = BackdropNode.NodeId;
        Data->Nodes[3] = IncreaseNode.NodeId;
        Data->Nodes[4] = DecreaseNode.NodeId;
        Data->Nodes[5] = BorderNode.NodeId;
        Data->Min      = 0;
        Data->Max      = 0;
        Data->Value    = 0;

        ApplyFillNodes();

        InitializeComponentEvents();

        ApplyFillNodes();

        Component->MinValue = 0;
        Component->MaxValue = 100;

        SetValueInstant(0);
    }

    /// <summary>
    ///     Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SimpleNineGridNode BackdropNode { get; }

    /// <summary>
    ///     Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SimpleNineGridNode FillNode { get; }

    /// <summary>
    ///     Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SimpleNineGridNode IncreaseNode { get; }

    /// <summary>
    ///     Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SimpleNineGridNode DecreaseNode { get; }

    /// <summary>
    ///     Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SimpleNineGridNode BorderNode { get; }

    /// <summary>
    ///     Gets or sets the gauges current value.
    /// </summary>
    public int Value
    {
        get => Component->Values[0].ValueInt;
        set => Component->SetGaugeValue(value, 0, false);
    }

    /// <summary>
    ///     Gets or sets the gauges lowest representable value.
    /// </summary>
    public int MinValue
    {
        get => Component->MinValue;
        set
        {
            if (value > Component->MaxValue) return;

            var previous = Component->MinValue;

            Component->MinValue = value;

            if (value > previous && Value < value)
                SetValueInstant(value);
        }
    }

    /// <summary>
    ///     Gets or sets the gauges highest representable value.
    /// </summary>
    public int MaxValue
    {
        get => Component->MaxValue;
        set
        {
            if (value < Component->MinValue) return;

            var previous = Component->MaxValue;

            Component->MaxValue = value;

            if (value < previous && Value > value)
                SetValueInstant(value);
        }
    }

    /// <summary>
    ///     Gets or sets the color of the gauges backdrop texture.
    /// </summary>
    public virtual Vector4 BackgroundColor
    {
        get => BackdropNode.Color;
        set => BackdropNode.Color = value;
    }

    /// <summary>
    ///     Gets or sets the bars color.
    /// </summary>
    public Vector4 BarColor
    {
        get => FillNode.Color;
        set
        {
            FillNode.Color     = value;
            IncreaseNode.Color = value;
            DecreaseNode.Color = value;
        }
    }

    /// <summary>
    ///     Gets or sets the texture used to draw this gauges bar.
    /// </summary>
    public string TexturePath
    {
        set
        {
            BackdropNode.TexturePath = value;
            FillNode.TexturePath     = value;
            IncreaseNode.TexturePath = value;
            DecreaseNode.TexturePath = value;
            BorderNode.TexturePath   = value;
        }
    }

    /// <summary>
    ///     Gets or sets the texture coordinate of the filled part, selected by its vertical offset.
    /// </summary>
    public float FillV
    {
        set => FillNode.TextureCoordinates = new Vector2(0.0f, value);
    }

    /// <summary>
    ///     Sets the gauges value without animating towards it.
    /// </summary>
    public void SetValueInstant
    (
        int newValue
    )
        => Component->SetGaugeValue(newValue, 0, true);

    /// <inheritdoc />
    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        BackdropNode.Size = new Vector2(Width, 16.0f);
        BorderNode.Size   = new Vector2(Width, 12.0f);
        FillNode.Size     = new Vector2(Width, 12.0f);

        if (Width <= (ushort)Component->MarginX)
            Component->MaxFillPositionX = 0;
        else
            Component->MaxFillPositionX = (short)(Width - (ushort)Component->MarginX);
    }

    private SimpleNineGridNode CreateNineGridNode
    (
        float positionX,
        float positionY,
        float width,
        float height,
        float leftOffset,
        float rightOffset,
        float topOffset,
        float bottomOffset,
        float textureV
    )
    {
        var node = new SimpleNineGridNode
        {
            Position           = new Vector2(positionX, positionY),
            TextureCoordinates = new Vector2(0.0f,      textureV),
            TextureSize        = new Vector2(width,     height),
            Offsets            = new Vector4(topOffset, bottomOffset, leftOffset, rightOffset)
        };

        node.AttachNode(this);

        ref var uldManager = ref ComponentBase->UldManager;
        uldManager.AddNodeToObjectList(node);

        return node;
    }

    private void ApplyFillNodes()
    {
        Component->BackdropImageNode            = (AtkImageNode*)BackdropNode.ResNode;
        Component->PrimaryFill.MainFillNode     = FillNode;
        Component->PrimaryFill.IncreaseFillNode = IncreaseNode;
        Component->PrimaryFill.DecreaseFillNode = DecreaseNode;
        Component->BorderNineGridNode           = BorderNode;
    }
}
