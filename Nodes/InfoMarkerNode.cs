using KamiToolKit.Nodes.Simplified;

namespace DailyRoutines.Common.KamiToolKit.Nodes;

public class InfoMarkerNode : SimpleNineGridNode
{
    public InfoMarkerNode()
    {
        TexturePath        = "ui/uld/img04/CircleButtons_hr1.tex";
        TextureCoordinates = new(112, 84);
        TextureSize        = new(28f);
    }
}
