using KamiToolKit.Nodes.Simplified;
using WrapMode = KamiToolKit.Enums.WrapMode;

namespace DailyRoutines.Common.KamiToolKit.Nodes;

public sealed class HorizontalDashedLineNode : SimpleImageNode
{
    public HorizontalDashedLineNode()
    {
        TexturePath        = "ui/uld/Lines.tex";
        TextureCoordinates = new(0f, 7f);
        TextureSize        = new(24f, 8f);
        WrapMode           = WrapMode.Tile;
    }
}
