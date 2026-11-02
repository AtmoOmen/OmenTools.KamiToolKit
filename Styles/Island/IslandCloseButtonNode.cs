using ButtonBase = KamiToolKit.Nodes.ButtonBase;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;

namespace OmenTools.KamiToolKit.Styles.Island;

/// <summary>组合无人岛圆形饰框与原生关闭图案，保留悬停、按下和禁用状态。</summary>
public sealed class IslandCloseButtonNode : ButtonBase
{
    /// <summary>创建尺寸为 40 × 40 的无人岛关闭按钮。</summary>
    public IslandCloseButtonNode()
    {
        var container = new ResNode
        {
            Size = new(40, 40)
        };
        container.AttachNode(this);
        var decoration = new SimpleImageNode
        {
            TexturePath        = "ui/uld/Mji_Window.tex",
            TextureCoordinates = new(300, 106),
            TextureSize        = new(40, 40),
            Size               = new(40, 40)
        };
        decoration.AttachNode(container);
        var glyph = new SimpleImageNode
        {
            TexturePath        = "ui/uld/WindowA_Button.tex",
            TextureCoordinates = new(0, 0),
            TextureSize        = new(28, 28),
            Size               = new(28, 28),
            Position           = new(6, 5)
        };
        glyph.AttachNode(container);
        Size = new(40, 40);
        LoadTwoPartTimelines(this, container);
        InitializeComponentEvents();
    }
}
