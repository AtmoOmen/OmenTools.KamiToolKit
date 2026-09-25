using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyRoutines.Common.KamiToolKit.Nodes;

public class ItemListItemNode : ListItemNode<Item>, IListItemNode
{
    public static float ItemHeight => 32.0f;

    protected readonly IconImageNode IconNode;
    protected readonly TextNode      LabelTextNode;
    protected readonly TextNode      SubLabelTextNode;
    protected readonly TextNode      IDTextNode;

    public ItemListItemNode()
    {
        IconNode = new IconImageNode
        {
            FitTexture = true,
            IconId     = 60072,
        };
        IconNode.AttachNode(this);

        LabelTextNode = new TextNode
        {
            TextFlags     = TextFlags.Ellipsis,
            FontSize      = 14,
            LineSpacing   = 14,
            AlignmentType = AlignmentType.BottomLeft,
            TextColor     = ColorHelper.GetColor(8),
        };
        LabelTextNode.AttachNode(this);

        SubLabelTextNode = new TextNode
        {
            TextFlags     = TextFlags.Ellipsis,
            FontSize      = 12,
            LineSpacing   = 12,
            AlignmentType = AlignmentType.TopLeft,
            TextColor     = ColorHelper.GetColor(3)
        };
        SubLabelTextNode.AttachNode(this);

        IDTextNode = new TextNode
        {
            TextFlags     = TextFlags.Emboss,
            FontSize      = 10,
            AlignmentType = AlignmentType.BottomRight,
            TextColor     = ColorHelper.GetColor(3),
        };
        IDTextNode.AttachNode(this);

        ShowClickableCursor = true;
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        IconNode.Size     = new Vector2(Height - 4.0f, Height - 4.0f);
        IconNode.Position = new Vector2(2.0f,          2.0f);

        LabelTextNode.Size     = new Vector2(Width  - Height - 2.0f - 30.0f, Height / 2.0f);
        LabelTextNode.Position = new Vector2(Height + 2.0f,                  0.0f);

        SubLabelTextNode.Size     = new Vector2(Width  - Height - 2.0f - 10.0f, Height / 2.0f);
        SubLabelTextNode.Position = new Vector2(Height + 2.0f   + 10.0f,        Height / 2.0f);

        IDTextNode.Size     = new Vector2(30.0f,         Height / 2.0f);
        IDTextNode.Position = new Vector2(Width - 30.0f, 0.0f);
    }

    protected override void SetNodeData
    (
        Item itemData
    )
    {
        if (itemData.RowId is 0) return;

        IconNode.IconId         = itemData.Icon;
        LabelTextNode.String    = itemData.Name.ToString();
        SubLabelTextNode.String = itemData.ItemSearchCategory.ValueNullable?.Name.ToString() ?? string.Empty;
        ItemTooltip             = itemData.RowId;
    }
}
