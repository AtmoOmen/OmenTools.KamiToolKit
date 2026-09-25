using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;

namespace DailyRoutines.Common.KamiToolKit.Nodes;

public abstract class ButtonListNode : SimpleComponentNode;

public abstract unsafe class ButtonListNode<T> : ButtonListNode
{
    public NineGridNode BackgroundNode { get; }

    public ScrollingNode<VerticalListNode> ScrollingListNode { get; }

    public T? SelectedOption { get; set; }

    public List<T>? Options
    {
        get;
        set
        {
            field = value;
            RebuildNodeList();
        }
    }

    public Action<T>? OnOptionSelected { get; set; }

    public int MaxButtons
    {
        get;
        set
        {
            field = value;
            RebuildNodeList();
        }
    } = 5;

    public bool AutoResizeHeight { get; set; }

    public void Show()
    {
        IsVisible = true;

        AddDrawFlags(DrawFlags.RenderOnTop);

        if (ParentAddon is not null)
            SetFocusable(ParentAddon);
    }

    public void Hide()
    {
        IsVisible = false;

        RemoveDrawFlags(DrawFlags.RenderOnTop);

        if (ParentAddon is not null)
            ClearFocusable(ParentAddon);
    }

    public void Toggle(bool newState)
    {
        if (newState)
            Show();
        else
            Hide();
    }

    public void SetFocusable(AtkUnitBase* addon)
    {
        foreach (ref var focusableNode in addon->AdditionalFocusableNodes)
        {
            if (focusableNode.Value is null)
            {
                focusableNode = ResNode;
                isFocusSet    = true;
            }
        }
    }

    public void ClearFocusable(AtkUnitBase* addon)
    {
        foreach (ref var focusableNode in addon->AdditionalFocusableNodes)
        {
            if (focusableNode.Value == ResNode)
            {
                focusableNode = null;
                isFocusSet    = false;
            }
        }
    }

    protected ButtonListNode()
    {
        BackgroundNode = new SimpleNineGridNode
        {
            TexturePath        = "ui/uld/ListB.tex",
            TextureCoordinates = new Vector2(0.0f,  0.0f),
            TextureSize        = new Vector2(32.0f, 32.0f),
            TopOffset          = 10,
            BottomOffset       = 12,
            LeftOffset         = 10,
            RightOffset        = 10
        };
        BackgroundNode.AttachNode(this);

        ScrollingListNode = new ScrollingNode<VerticalListNode>
        {
            ContentNode =
            {
                FirstItemSpacing = 2.0f,
                ItemSpacing      = 3.0f,
                FitContents      = true
            },
            AutoHideScrollBar = true
        };
        ScrollingListNode.AttachNode(this);

        BuildTimelines();
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        BackgroundNode.Size = Size;

        ScrollingListNode.Size     = new Vector2(Width - 8.0f, Height - 14.0f);
        ScrollingListNode.Position = new Vector2(2.0f,         4.0f);

        ScrollingListNode.ContentNode.Position = new Vector2(4.0f, 0.0f);
        ScrollingListNode.RecalculateSizes();

        foreach (var node in ScrollingListNode.ContentNode.Nodes)
            node.Width = Width - 16.0f;
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor)
    {
        if (disposing)
        {
            if (isFocusSet && !isNativeDestructor)
            {
                if (ParentAddon is not null)
                    ClearFocusable(ParentAddon);
            }

            base.Dispose(disposing, isNativeDestructor);
        }
    }

    protected float ButtonNodeHeight { get; set; } = 22.0f;

    protected abstract string GetLabelForOption(T option);

    protected virtual void OnOptionClick(ListButtonNode listButton, T option)
    {
        if (Options is null) return;

        selectedButtonNode?.Selected = false;
        selectedButtonNode           = listButton;
        selectedButtonNode.Selected  = true;

        SelectedOption = option;
        OnOptionSelected?.Invoke(SelectedOption);
    }

    private void RebuildNodeList()
    {
        ScrollingListNode.ContentNode.Clear();

        foreach (var option in Options ?? [])
        {
            var newButton = new ListButtonNode
            {
                Size   = new Vector2(Width - 16.0f, ButtonNodeHeight),
                String = GetLabelForOption(option)
            };

            newButton.OnClick = () => OnOptionClick(newButton, option);
            ScrollingListNode.ContentNode.AddNode(newButton);
        }

        if (AutoResizeHeight)
            Height = ScrollingListNode.ContentNode.Nodes.Sum(nodes => nodes.Height) + 24.0f;
        else
            ScrollingListNode.RecalculateSizes();
    }

    private void BuildTimelines() =>
        AddTimeline
        (
            new TimelineBuilder()
                .BeginFrameSet(1, 29)
                .AddLabel(1,  17, AtkTimelineJumpBehavior.Start,    0)
                .AddLabel(9,  0,  AtkTimelineJumpBehavior.PlayOnce, 0)
                .AddLabel(10, 18, AtkTimelineJumpBehavior.Start,    0)
                .AddLabel(19, 0,  AtkTimelineJumpBehavior.PlayOnce, 0)
                .AddLabel(20, 7,  AtkTimelineJumpBehavior.Start,    0)
                .AddLabel(29, 0,  AtkTimelineJumpBehavior.PlayOnce, 0)
                .EndFrameSet()
                .Build()
        );

    private ListButtonNode? selectedButtonNode;
    private bool            isFocusSet;
}

public class TextButtonListNode : ButtonListNode<string>
{
    protected override string GetLabelForOption(string option) => option;
}
