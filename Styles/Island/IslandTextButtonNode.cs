using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;

namespace OmenTools.KamiToolKit.Styles.Island;

/// <summary>使用无人岛棕色纹理和固定白色文字的按钮，保留原生按钮的全部交互状态。</summary>
public sealed class IslandTextButtonNode : TextButtonNode
{
    /// <summary>创建无人岛按钮，默认高度为 32；调用方设置宽度、文字和点击动作。</summary>
    /// <param name="style">提供按钮文字样式的无人岛风格。</param>
    public IslandTextButtonNode(IslandUIStyle style)
    {
        var texture = (SimpleNineGridNode)BackgroundNode;
        texture.TexturePath        = "ui/uld/Mji_Window.tex";
        texture.TextureCoordinates = new(304, 348);
        texture.TextureSize        = new(100, 28);
        texture.Offsets            = new(0, 0, 16, 16);
        style.ApplyTextStyle(LabelNode, UITextRole.Button);
        Size = new(100, 32);
    }
}
