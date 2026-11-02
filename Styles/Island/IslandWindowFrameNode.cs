using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;

namespace OmenTools.KamiToolKit.Styles.Island;

/// <summary>绘制无人岛木纹外框、绳结边饰和椰树水印，随自身尺寸变化调整背景与装饰。</summary>
public sealed class IslandWindowFrameNode : WindowFrameNode
{
    private readonly NineGridNode    background;
    private readonly SimpleImageNode watermark;
    private readonly SimpleImageNode topDecoration;
    private readonly SimpleImageNode bottomDecoration;

    /// <summary>创建无人岛外框及其独立拥有的装饰节点。</summary>
    /// <param name="style">提供内边距和最小尺寸的无人岛风格。</param>
    public IslandWindowFrameNode(IslandUIStyle style) : base(style)
    {
        background = new()
        {
            Size            = Size,
            Offsets         = new(32, 32, 32, 32),
            PartsRenderType = 17,
            Parts =
            [
                new Part
                {
                    TexturePath        = "ui/uld/MJI_Bg.tex",
                    TextureCoordinates = new(0, 0),
                    Size               = new(128, 128)
                },
                new Part
                {
                    TexturePath        = "ui/uld/MJI_BgV.tex",
                    TextureCoordinates = new(0, 0),
                    Size               = new(64, 128)
                },
                new Part
                {
                    TexturePath        = "ui/uld/MJI_Bg.tex",
                    TextureCoordinates = new(128, 0),
                    Size               = new(128, 128)
                },
                new Part
                {
                    TexturePath        = "ui/uld/MJI_BgH.tex",
                    TextureCoordinates = new(0, 0),
                    Size               = new(128, 64)
                },
                new Part
                {
                    TexturePath        = "ui/uld/MJI_BgHV.tex",
                    TextureCoordinates = new(0, 0),
                    Size               = new(64, 64)
                },
                new Part
                {
                    TexturePath        = "ui/uld/MJI_BgH.tex",
                    TextureCoordinates = new(128, 0),
                    Size               = new(128, 64)
                },
                new Part
                {
                    TexturePath        = "ui/uld/MJI_Bg.tex",
                    TextureCoordinates = new(0, 128),
                    Size               = new(128, 128)
                },
                new Part
                {
                    TexturePath        = "ui/uld/MJI_BgV.tex",
                    TextureCoordinates = new(0, 128),
                    Size               = new(64, 128)
                },
                new Part
                {
                    TexturePath        = "ui/uld/MJI_Bg.tex",
                    TextureCoordinates = new(128, 128),
                    Size               = new(128, 128)
                }
            ]
        };
        background.AttachNode(this);
        watermark = new SimpleImageNode
        {
            TexturePath        = "ui/uld/Mji_Window.tex",
            TextureCoordinates = new(0, 168),
            TextureSize        = new(248, 208),
            Size               = new(248, 208),
            Position           = new(Size.X - 262, 16)
        };
        watermark.AttachNode(this);
        topDecoration = new SimpleImageNode
        {
            TexturePath        = "ui/uld/Mji_Window.tex",
            TextureCoordinates = new(0, 106),
            TextureSize        = new(220, 62),
            Size               = new(220, 62),
            Position           = new(8, 0),
            Origin             = new(0, 31),
            RotationDegrees    = 90
        };
        topDecoration.AttachNode(this);
        bottomDecoration = new()
        {
            TexturePath        = "ui/uld/Mji_Window.tex",
            TextureCoordinates = new(0, 106),
            TextureSize        = new(220, 62),
            Size               = new(220, 62),
            Origin             = new(0, 31),
            RotationDegrees    = 90
        };
        bottomDecoration.AttachNode(this);
        Size = style.MinimumSize;
    }

    /// <summary>控制椰树水印是否显示，不影响内容区域大小。</summary>
    public bool ShowWatermark
    {
        get => watermark.IsVisible;
        set => watermark.IsVisible = value;
    }

    /// <summary>控制两侧绳结边饰是否显示，不影响内容区域大小。</summary>
    public bool ShowDecorations
    {
        get => topDecoration.IsVisible;
        set
        {
            topDecoration.IsVisible    = value;
            bottomDecoration.IsVisible = value;
        }
    }

    /// <inheritdoc />
    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();
        var backgroundHeight = Math.Max(Height, 256);
        background.Size           = new(Width, backgroundHeight);
        background.ScaleY         = Height / backgroundHeight;
        watermark.Position        = new(Width - 262, 16);
        bottomDecoration.Position = new(Width - 9, Height - 247);
    }
}
