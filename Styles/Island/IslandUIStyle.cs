using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;

namespace OmenTools.KamiToolKit.Styles.Island;

/// <summary>提供无人岛木纹窗口、棕色按钮、金棕分隔线及不随游戏主题变色的配套文字样式。</summary>
public sealed class IslandUIStyle : UIStyle
{
    /// <summary>为无人岛边框与绳结装饰保留左右各 36、上下各 32 的空间。</summary>
    public override Vector4 ContentInsets => new(36, 32, 36, 32);

    /// <summary>保证背景九宫格与两侧装饰完整显示的最小外框尺寸。</summary>
    public override Vector2 MinimumSize => new(320, 256);

    /// <summary>带标题栏窗口在外框内额外保留 32 的标题高度。</summary>
    public override float HeaderHeight => 32;

    /// <summary>标题栏沿用无人岛窗口的原生位置，位于左侧 24、顶部 20。</summary>
    public override Vector2 HeaderPosition => new(24, 20);

    /// <summary>关闭按钮位于窗口右侧内缩 1、顶部内缩 2 的位置。</summary>
    public override Vector2 CloseButtonOffset => new(1, 2);

    /// <summary>创建带木纹背景、椰树水印和绳结边饰的无人岛外框。</summary>
    public override IslandWindowFrameNode CreateFrame() => new(this);

    /// <summary>创建无人岛棕色文字按钮，包含悬停、按下和禁用状态。</summary>
    public override IslandTextButtonNode CreateButton() => new(this);

    /// <summary>创建带无人岛圆形装饰的关闭按钮，默认尺寸为 40 × 40。</summary>
    public override IslandCloseButtonNode CreateCloseButton() => new();

    /// <summary>创建高度为 12 的无人岛水平分隔线，左右端保持原始形状。</summary>
    public override NineGridNode CreateSeparator() => new SimpleNineGridNode
    {
        TexturePath        = "ui/uld/Mji_Window.tex",
        TextureCoordinates = new(220, 150),
        TextureSize        = new(64, 12),
        Offsets            = new(0, 0, 30, 30),
        Size               = new(64, 12)
    };

    /// <summary>按文字用途应用无人岛字体与固定色号；正文使用深棕色，按钮使用白色，不受游戏界面主题影响。</summary>
    /// <param name="node">需要应用样式的文字节点。</param>
    /// <param name="role">文字用途，决定字号、字体、换行方式与颜色。</param>
    public override void ApplyTextStyle(TextNode node, UITextRole role)
    {
        node.FontType = role == UITextRole.Title ?
                            FontType.TrumpGothic :
                            FontType.Axis;
        node.FontSize = role switch
        {
            UITextRole.Title                          => 23,
            UITextRole.Subtitle or UITextRole.Heading => 12,
            _                                         => 14
        };
        node.LineSpacing = role == UITextRole.Body ?
                               20u :
                               24u;
        node.TextColorID = role == UITextRole.Button ?
                               50u :
                               68u;
        node.TextOutlineColorID = 7;
        node.TextFlags = role switch
        {
            UITextRole.Body                         => TextFlags.WordWrap | TextFlags.MultiLine,
            UITextRole.Title or UITextRole.Subtitle => TextFlags.Ellipsis,
            _                                       => 0
        };
    }
}
