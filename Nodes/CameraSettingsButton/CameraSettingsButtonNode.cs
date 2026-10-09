using System.Numerics;
using KamiToolKit.Enums;
using KamiToolKit.Nodes.Simplified;
using ButtonBase = KamiToolKit.Nodes.ButtonBase;

namespace OmenTools.KamiToolKit.Nodes.CameraSettingsButton;

public sealed class CameraSettingsButtonNode : ButtonBase
{
    public CameraSettingsButtonNode()
    {
        ImageNode = new SimpleImageNode
        {
            TexturePath        = "ui/uld/CameraSettings.tex",
            TextureCoordinates = Vector2.Zero,
            TextureSize        = new Vector2(36f),
            WrapMode           = WrapMode.Stretch
        };
        ImageNode.AttachNode(this);

        LoadTwoPartTimelines(this, ImageNode);

        InitializeComponentEvents();
    }

    public SimpleImageNode ImageNode { get; }

    public CameraSettingsButtonIcon Icon
    {
        get;
        set
        {
            var region = value switch
            {
                CameraSettingsButtonIcon.EyeWithRays         => new Vector4(0f,   0f,   36f, 36f),
                CameraSettingsButtonIcon.LightWithRays       => new Vector4(36f,  0f,   36f, 36f),
                CameraSettingsButtonIcon.PersonLookingBack   => new Vector4(72f,  0f,   36f, 36f),
                CameraSettingsButtonIcon.PersonRaisingArms   => new Vector4(108f, 0f,   36f, 36f),
                CameraSettingsButtonIcon.GoldFrame           => new Vector4(144f, 0f,   36f, 36f),
                CameraSettingsButtonIcon.SilverFrame         => new Vector4(180f, 0f,   36f, 36f),
                CameraSettingsButtonIcon.PersonStanding      => new Vector4(216f, 0f,   36f, 36f),
                CameraSettingsButtonIcon.PortraitFrame       => new Vector4(252f, 0f,   18f, 26f),
                CameraSettingsButtonIcon.ActivePortraitFrame => new Vector4(270f, 0f,   18f, 26f),
                CameraSettingsButtonIcon.GoldSquare          => new Vector4(252f, 26f,  14f, 14f),
                CameraSettingsButtonIcon.Pause               => new Vector4(0f,   36f,  36f, 36f),
                CameraSettingsButtonIcon.FastForward         => new Vector4(36f,  36f,  36f, 36f),
                CameraSettingsButtonIcon.ResetCamera         => new Vector4(72f,  36f,  36f, 36f),
                CameraSettingsButtonIcon.ResetPosition       => new Vector4(108f, 36f,  36f, 36f),
                CameraSettingsButtonIcon.PauseAll            => new Vector4(144f, 36f,  36f, 36f),
                CameraSettingsButtonIcon.PlayAll             => new Vector4(180f, 36f,  36f, 36f),
                CameraSettingsButtonIcon.Gradient            => new Vector4(216f, 36f,  36f, 36f),
                CameraSettingsButtonIcon.Move                => new Vector4(100f, 72f,  24f, 24f),
                CameraSettingsButtonIcon.VerticalSeparator   => new Vector4(124f, 72f,  8f,  24f),
                CameraSettingsButtonIcon.FastForwardSmall    => new Vector4(132f, 72f,  18f, 18f),
                CameraSettingsButtonIcon.FilterSmall         => new Vector4(150f, 72f,  18f, 18f),
                CameraSettingsButtonIcon.ColorSwatch         => new Vector4(240f, 72f,  18f, 18f),
                CameraSettingsButtonIcon.RedLight            => new Vector4(41f,  73f,  18f, 18f),
                CameraSettingsButtonIcon.GreenLight          => new Vector4(61f,  73f,  18f, 18f),
                CameraSettingsButtonIcon.BlueLight           => new Vector4(81f,  73f,  18f, 18f),
                CameraSettingsButtonIcon.RotateSmall         => new Vector4(168f, 73f,  18f, 18f),
                CameraSettingsButtonIcon.LightSmall          => new Vector4(186f, 73f,  18f, 18f),
                CameraSettingsButtonIcon.LightingSmall       => new Vector4(204f, 73f,  18f, 18f),
                CameraSettingsButtonIcon.FunnelSmall         => new Vector4(222f, 73f,  18f, 18f),
                CameraSettingsButtonIcon.MoveSmall           => new Vector4(1f,   74f,  18f, 18f),
                CameraSettingsButtonIcon.ZoomSmall           => new Vector4(21f,  74f,  18f, 18f),
                CameraSettingsButtonIcon.CameraSettings      => new Vector4(0f,   96f,  48f, 40f),
                CameraSettingsButtonIcon.ScreenEffect        => new Vector4(48f,  96f,  48f, 40f),
                CameraSettingsButtonIcon.LightingSettings    => new Vector4(96f,  96f,  48f, 40f),
                CameraSettingsButtonIcon.MotionSettings      => new Vector4(144f, 96f,  48f, 40f),
                CameraSettingsButtonIcon.PoseSettings        => new Vector4(192f, 96f,  48f, 40f),
                CameraSettingsButtonIcon.AdvancedSettings    => new Vector4(240f, 96f,  48f, 40f),
                CameraSettingsButtonIcon.Light1              => new Vector4(0f,   136f, 36f, 36f),
                CameraSettingsButtonIcon.Light2              => new Vector4(36f,  136f, 36f, 36f),
                CameraSettingsButtonIcon.Light3              => new Vector4(72f,  136f, 36f, 36f),
                CameraSettingsButtonIcon.Light               => new Vector4(108f, 136f, 36f, 36f),
                CameraSettingsButtonIcon.ResetLighting       => new Vector4(144f, 136f, 36f, 36f),
                CameraSettingsButtonIcon.Reset               => new Vector4(180f, 136f, 36f, 36f),
                CameraSettingsButtonIcon.TiltHead            => new Vector4(216f, 136f, 36f, 36f),
                CameraSettingsButtonIcon.LockTime            => new Vector4(0f,   172f, 36f, 36f),
                CameraSettingsButtonIcon.FaceLight           => new Vector4(36f,  172f, 36f, 36f),
                CameraSettingsButtonIcon.CharacterMotion     => new Vector4(72f,  172f, 36f, 36f),
                CameraSettingsButtonIcon.CharacterGroup      => new Vector4(108f, 172f, 48f, 40f),
                CameraSettingsButtonIcon.TargetMarker        => new Vector4(156f, 172f, 40f, 24f),
                CameraSettingsButtonIcon.ResetGaze           => new Vector4(204f, 176f, 36f, 36f),
                CameraSettingsButtonIcon.ResetExpression     => new Vector4(240f, 176f, 36f, 36f),
                CameraSettingsButtonIcon.RightArrowSmall     => new Vector4(156f, 196f, 16f, 16f),
                CameraSettingsButtonIcon.Edit                => new Vector4(0f,   208f, 48f, 40f),
                CameraSettingsButtonIcon.Guide               => new Vector4(48f,  208f, 48f, 40f),
                CameraSettingsButtonIcon.Light1Marker        => new Vector4(96f,  212f, 24f, 36f),
                CameraSettingsButtonIcon.Light2Marker        => new Vector4(120f, 212f, 24f, 36f),
                CameraSettingsButtonIcon.Light3Marker        => new Vector4(144f, 212f, 24f, 36f),
                CameraSettingsButtonIcon.Eye                 => new Vector4(168f, 212f, 36f, 36f),
                CameraSettingsButtonIcon.Repeat              => new Vector4(204f, 212f, 36f, 36f),
                CameraSettingsButtonIcon.Play                => new Vector4(240f, 212f, 36f, 36f),
                _                                            => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };

            field                        = value;
            ImageNode.TextureCoordinates = new Vector2(region.X, region.Y);
            ImageNode.TextureSize        = new Vector2(region.Z, region.W);
        }
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        ImageNode.Size = Size;
    }
}
