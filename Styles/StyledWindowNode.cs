using ButtonBase = KamiToolKit.Nodes.ButtonBase;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Timelines;

namespace OmenTools.KamiToolKit.Styles;

/// <summary>将风格外框接入原生窗口组件，提供标题、副标题、关闭和拖动行为，可直接用于 NativeAddon.CreateWindowNode。</summary>
public sealed unsafe class StyledWindowNode : WindowNodeBase
{
    private readonly UIStyle       style;
    private readonly bool          showHeader;
    private readonly ResNode       header;
    private readonly CollisionNode headerCollision;
    private readonly TextNode      title;
    private readonly TextNode      subtitle;
    private readonly ButtonBase    closeButton;
    private readonly NineGridNode  headerSeparator;

    /// <summary>创建风格窗口及其独立拥有的全部子节点。</summary>
    /// <param name="style">提供外框、文字、关闭按钮和布局度量的风格。</param>
    /// <param name="showHeader">是否显示标题栏并启用关闭、标题栏拖动；附加面板应传入 false。</param>
    public StyledWindowNode(UIStyle style, bool showHeader = true)
    {
        this.style           = style;
        this.showHeader      = showHeader;
        NodeFlags            = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents;
        CollisionNode.NodeId = 13;
        CollisionNode.NodeFlags = NodeFlags.AnchorTop    |
                                  NodeFlags.AnchorLeft   |
                                  NodeFlags.Visible      |
                                  NodeFlags.Enabled      |
                                  NodeFlags.Fill         |
                                  NodeFlags.HasCollision |
                                  NodeFlags.EmitsEvents;
        Component->ShowFlags = 1;
        Frame                = style.CreateFrame();
        Frame.NodeId         = 11;
        Frame.AttachNode(this);
        header = new()
        {
            NodeId    = 2,
            Height    = style.HeaderHeight,
            IsVisible = showHeader
        };
        header.AttachNode(this);
        headerSeparator = style.CreateSeparator();
        headerSeparator.AttachNode(header);
        headerCollision = new()
        {
            NodeId    = 12,
            Uses      = 2,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.EmitsEvents,
            IsVisible = showHeader
        };
        headerCollision.AttachNode(header);
        title               = style.CreateText(UITextRole.Title);
        title.NodeId        = 3;
        title.AlignmentType = AlignmentType.Left;
        title.AttachNode(header);
        subtitle               = style.CreateText(UITextRole.Subtitle);
        subtitle.NodeId        = 4;
        subtitle.AlignmentType = AlignmentType.Left;
        subtitle.AttachNode(header);
        closeButton        = style.CreateCloseButton();
        closeButton.NodeId = 7;
        closeButton.AttachNode(header);
        Data->ShowCloseButton = (byte)(showHeader ?
                                           1 :
                                           0);
        Data->ShowConfigButton = 0;
        Data->ShowHelpButton   = 0;
        Data->ShowHeader = (byte)(showHeader ?
                                      1 :
                                      0);
        Data->Nodes[0] = title.NodeId;
        Data->Nodes[1] = subtitle.NodeId;
        Data->Nodes[2] = closeButton.NodeId;
        Data->Nodes[6] = header.NodeId;
        AddTimeline
        (
            new TimelineBuilder()
                .BeginFrameSet(1, 29)
                .AddLabelPair(1,  9,  17)
                .AddLabelPair(10, 19, 18)
                .AddLabelPair(20, 29, 7)
                .EndFrameSet()
                .Build()
        );
        InitializeComponentEvents();
        Size = MinimumSize;
    }

    /// <summary>窗口拥有的风格外框，可调整具体风格提供的水印或边饰显示选项。</summary>
    public WindowFrameNode Frame { get; }

    /// <summary>包含当前标题栏高度的推荐最小窗口尺寸。</summary>
    public Vector2 MinimumSize => style.MinimumSize + new Vector2(0, HeaderHeight);

    /// <summary>当前窗口内容区域在外框内的起点，已避开标题栏。</summary>
    public override Vector2 ContentStartPosition => Frame.ContentStartPosition + new Vector2(0, HeaderHeight);

    /// <summary>扣除外框内边距及标题栏后的内容尺寸，各方向最小为零。</summary>
    public override Vector2 ContentSize => Vector2.Max(Vector2.Zero, Frame.ContentSize - new Vector2(0, HeaderHeight));

    /// <summary>当前窗口占用的标题栏高度，无标题栏面板返回零。</summary>
    public override float HeaderHeight => showHeader ?
                                              style.HeaderHeight :
                                              0;

    /// <summary>原生窗口用于焦点处理的标题节点，无标题栏时使用外框节点。</summary>
    public override ResNode WindowHeaderFocusNode => showHeader ?
                                                         header :
                                                         Frame;

    /// <summary>设置主标题与副标题，并根据各自文字宽度重新分配标题栏空间。</summary>
    /// <param name="title">窗口主标题。</param>
    /// <param name="subtitle">可选副标题，省略或为空时不占用横向空间。</param>
    public override void SetTitle(string title, string? subtitle = null)
    {
        base.SetTitle(title, subtitle);
        OnSizeChanged();
    }

    /// <inheritdoc />
    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();
        Frame.Size               = Size;
        header.Position          = style.HeaderPosition;
        header.Width             = Math.Max(0, Width - header.X - style.ContentInsets.Z);
        headerSeparator.Position = new(0, style.HeaderHeight);
        headerSeparator.Width    = header.Width;
        headerCollision.Size     = new(Math.Max(0, header.Width), style.HeaderHeight);
        closeButton.Position     = new(Width - header.X - closeButton.Width - style.CloseButtonOffset.X, style.CloseButtonOffset.Y - header.Y);
        var availableWidth = Math.Max(0, Math.Min(header.Width, closeButton.X - 8));
        var subtitleWidth  = Math.Min(subtitle.GetTextDrawSize().X, availableWidth / 2);
        var titleWidth     = Math.Max(0, availableWidth - subtitleWidth);
        title.Size        = new(titleWidth, style.HeaderHeight);
        subtitle.Position = new(Math.Min(title.GetTextDrawSize().X + 8, titleWidth), 10);
        subtitle.Size     = new(Math.Max(0, availableWidth         - subtitle.X), 20);
    }
}
