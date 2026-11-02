using System.Numerics;
using KamiToolKit.Nodes;

namespace OmenTools.KamiToolKit.Styles;

/// <summary>提供可随尺寸变化重新布局的风格外框，不包含标题栏、关闭按钮和业务内容。</summary>
/// <param name="style">外框所属的界面风格。</param>
public abstract class WindowFrameNode
(
    UIStyle style
) : ResNode
{
    /// <summary>外框使用的风格定义。</summary>
    public UIStyle Style { get; } = style;

    /// <summary>避开外框装饰后的内容起点。</summary>
    public Vector2 ContentStartPosition => new(Style.ContentInsets.X, Style.ContentInsets.Y);

    /// <summary>扣除四周外框内边距后的可用内容尺寸，各方向最小为零。</summary>
    public Vector2 ContentSize => Vector2.Max
        (Vector2.Zero, Size - new Vector2(Style.ContentInsets.X + Style.ContentInsets.Z, Style.ContentInsets.Y + Style.ContentInsets.W));
}
