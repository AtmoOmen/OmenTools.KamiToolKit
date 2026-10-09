using KamiToolKit.Nodes;

namespace OmenTools.KamiToolKit.Nodes;

public class TextMultiLineInputNode : TextInputNode
{
    private float minimumHeight;
    private float lastHeight;
    private bool  isUpdatingHeight;

    public TextMultiLineInputNode() : base(true)
    {
        minimumHeight = Height;
        lastHeight    = Height;
    }

    public bool AutoUpdateHeight
    {
        get;
        set
        {
            if (field == value) return;

            field         = value;
            minimumHeight = Height;
            UpdateHeightForContent();
        }
    }

    public Action<float>? HeightChanged { get; set; }

    public void UpdateHeightForContent()
    {
        if (!AutoUpdateHeight || isUpdatingHeight) return;

        var textHeight    = CurrentTextNode.GetTextDrawSize(false).Y;
        var contentHeight = MathF.Ceiling(MathF.Max(minimumHeight, MathF.Max(textHeight, CurrentTextNode.LineSpacing) + 20.0f));
        contentHeight = MathF.Min(contentHeight, ushort.MaxValue);
        if (MathF.Abs(Height - contentHeight) < 0.5f) return;

        isUpdatingHeight = true;

        try
        {
            Height = contentHeight;
        }
        finally
        {
            isUpdatingHeight = false;
        }

        HeightChanged?.Invoke(Height);
    }

    protected override void OnTextChanged()
    {
        base.OnTextChanged();
        UpdateHeightForContent();
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        if (!isUpdatingHeight && MathF.Abs(Height - lastHeight) >= 0.5f)
            minimumHeight = Height;

        lastHeight = Height;
        UpdateHeightForContent();
    }
}
