using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using OmenTools.Extensions;

namespace OmenTools.KamiToolKit.Nodes;

public sealed class PaginationNode : SimpleComponentNode
{
    public PaginationNode()
    {
        PreviousPageButtonNode = new TextureButtonNode
        {
            TexturePath        = "ui/uld/ItemSearch.tex",
            TextureCoordinates = new Vector2(0.0f,         36.0f),
            TextureSize        = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT),
            Size               = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT)
        };
        PreviousPageButtonNode.OnClick = () => OnPreviousPage?.Invoke();

        IndicatorTextNode = new TextNode
        {
            FontType         = FontType.MiedingerMed,
            FontSize         = 14,
            LineSpacing      = 0,
            CharSpacing      = 0,
            AlignmentType    = AlignmentType.Center,
            TextColor        = ColorHelper.GetColor(64),
            TextOutlineColor = ColorHelper.GetColor(65),
            TextFlags        = TextFlags.Glare,
            Height           = BUTTON_HEIGHT,
            OnStringUpdated  = UpdateLayout,
            IsVisible        = false
        };

        NextPageButtonNode = new TextureButtonNode
        {
            TexturePath        = "ui/uld/ItemSearch.tex",
            TextureCoordinates = new Vector2(0.0f,         36.0f),
            TextureSize        = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT),
            Size               = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT)
        };
        NextPageButtonNode.ImageNode.ImageNodeFlags = ImageNodeFlags.FlipH;
        NextPageButtonNode.OnClick                  = () => OnNextPage?.Invoke();

        LayoutNode = new HorizontalListNode
        {
            ItemSpacing       = BUTTON_SPACING,
            Height            = BUTTON_HEIGHT,
            FitToContentWidth = true
        };
        LayoutNode.AddNode([PreviousPageButtonNode, IndicatorTextNode, NextPageButtonNode]);
        LayoutNode.AttachNode(this);

        UpdateLayout();
    }

    public TextureButtonNode PreviousPageButtonNode { get; }

    public TextNode IndicatorTextNode { get; }

    public TextureButtonNode NextPageButtonNode { get; }

    public HorizontalListNode LayoutNode { get; }

    public Action? OnPreviousPage { get; set; }

    public Action? OnNextPage { get; set; }

    public bool IsDisplayIndicatorText
    {
        get;
        set
        {
            if (field == value) return;

            field = value;

            IndicatorTextNode.IsVisible = value;

            UpdateLayout();
        }
    }

    protected override void Dispose
    (
        bool disposing,
        bool isNativeDestructor
    )
    {
        if (disposing)
        {
            if (isTrackingTextWidth && !IFramework.Instance().IsFrameworkUnloading)
            {
                IFramework.Instance().Update -= OnFrameworkUpdate;
                isTrackingTextWidth          =  false;
            }

            base.Dispose(disposing, isNativeDestructor);
        }
    }

    private void UpdateLayout()
    {
        ApplyTextWidth();

        if (isTrackingTextWidth) return;

        isTrackingTextWidth = true;

        IFramework.Instance().Update += OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate
    (
        IFramework framework
    )
    {
        if (ApplyTextWidth()) return;

        IFramework.Instance().Update -= OnFrameworkUpdate;
        isTrackingTextWidth          =  false;
    }

    private bool ApplyTextWidth()
    {
        var width = (int)IndicatorTextNode.GetTextDrawSize().X;

        if (width == lastMeasuredWidth) return false;

        lastMeasuredWidth = width;

        IndicatorTextNode.Width = width;

        LayoutNode.RecalculateLayout();

        Size = LayoutNode.Size;

        return true;
    }

    private int? lastMeasuredWidth;
    private bool isTrackingTextWidth;

    private const float BUTTON_WIDTH   = 36.0f;
    private const float BUTTON_HEIGHT  = 28.0f;
    private const float BUTTON_SPACING = 9.0f;
}
