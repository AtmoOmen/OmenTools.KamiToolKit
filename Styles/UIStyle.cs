using ButtonBase = KamiToolKit.Nodes.ButtonBase;
using System.Numerics;
using KamiToolKit.Nodes;

namespace OmenTools.KamiToolKit.Styles;

/// <summary>定义一套界面风格的窗口度量、节点创建方式和文字样式。风格对象可复用，每次创建的节点分别归属其宿主。</summary>
public abstract class UIStyle
{
    /// <summary>外框为内容保留的左、上、右、下内边距，不包含独立窗口标题栏。</summary>
    public abstract Vector4 ContentInsets { get; }

    /// <summary>外框保持完整装饰和有效内容区域所需的最小尺寸。</summary>
    public abstract Vector2 MinimumSize { get; }

    /// <summary>独立窗口标题栏占用的高度，无标题栏面板不占用此高度。</summary>
    public abstract float HeaderHeight { get; }

    /// <summary>独立窗口标题栏左上角相对于外框的偏移。</summary>
    public abstract Vector2 HeaderPosition { get; }

    /// <summary>关闭按钮右侧与窗口右侧的距离，以及按钮顶部与窗口顶部的距离。</summary>
    public abstract Vector2 CloseButtonOffset { get; }

    /// <summary>创建独立的风格外框，调用方负责挂接，宿主节点负责释放。</summary>
    public abstract WindowFrameNode CreateFrame();

    /// <summary>创建带有完整交互状态的风格文字按钮，调用方设置尺寸、文字与点击动作。</summary>
    public abstract TextButtonNode CreateButton();

    /// <summary>创建风格关闭按钮，窗口组件负责接收其原生关闭事件。</summary>
    public abstract ButtonBase CreateCloseButton();

    /// <summary>创建风格水平分隔线，调用方设置宽度，默认高度由具体风格决定。</summary>
    public abstract NineGridNode CreateSeparator();

    /// <summary>按照文字用途更新节点的字体、字号、行距、颜色和文字标记，不修改文字、位置、尺寸及事件。</summary>
    /// <param name="node">需要应用样式的文字节点。</param>
    /// <param name="role">文字在界面中的语义用途。</param>
    public abstract void ApplyTextStyle(TextNode node, UITextRole role);

    /// <summary>创建已应用指定语义样式的文字节点，调用方设置文字、尺寸与位置。</summary>
    /// <param name="role">文字在界面中的语义用途，默认为正文。</param>
    public TextNode CreateText(UITextRole role = UITextRole.Body)
    {
        var node = new TextNode();
        ApplyTextStyle(node, role);
        return node;
    }

    /// <summary>创建可供 NativeAddon.CreateWindowNode 使用的完整窗口，也可选择不显示标题栏以承载附加面板。</summary>
    /// <param name="showHeader">是否显示标题、副标题和关闭按钮，并启用标题栏拖动。</param>
    public StyledWindowNode CreateWindow(bool showHeader = true) => new(this, showHeader);
}
