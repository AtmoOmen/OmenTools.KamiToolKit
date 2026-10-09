using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes.Simplified;
using ButtonBase = KamiToolKit.Nodes.ButtonBase;

namespace OmenTools.KamiToolKit.Nodes.CircleButton;

public sealed class CircleButtonNode : ButtonBase
{
    public CircleButtonNode()
    {
        ImageNode = new SimpleImageNode
        {
            TexturePath        = "ui/uld/CircleButtons.tex",
            TextureCoordinates = Vector2.Zero,
            TextureSize        = new Vector2(28f),
            WrapMode           = WrapMode.Stretch
        };
        ImageNode.AttachNode(this);

        LoadTwoPartTimelines(this, ImageNode);

        InitializeComponentEvents();
    }

    public SimpleImageNode ImageNode { get; }

    public CircleButtonIcon Icon
    {
        get;
        set
        {
            var region = value switch
            {
                CircleButtonIcon.Gear                                     => new Vector4(0f,   0f,   28f, 28f),
                CircleButtonIcon.Filter                                   => new Vector4(28f,  0f,   28f, 28f),
                CircleButtonIcon.Sort                                     => new Vector4(56f,  0f,   28f, 28f),
                CircleButtonIcon.QuestionMark                             => new Vector4(84f,  0f,   28f, 28f),
                CircleButtonIcon.Refresh                                  => new Vector4(112f, 0f,   28f, 28f),
                CircleButtonIcon.ChatBubble                               => new Vector4(140f, 0f,   28f, 28f),
                CircleButtonIcon.LeftArrow or CircleButtonIcon.RightArrow => new Vector4(168f, 0f,   28f, 28f),
                CircleButtonIcon.UpArrow                                  => new Vector4(196f, 0f,   28f, 28f),
                CircleButtonIcon.Chest                                    => new Vector4(224f, 0f,   28f, 28f),
                CircleButtonIcon.Document                                 => new Vector4(0f,   28f,  28f, 28f),
                CircleButtonIcon.Edit                                     => new Vector4(28f,  28f,  28f, 28f),
                CircleButtonIcon.Add                                      => new Vector4(56f,  28f,  28f, 28f),
                CircleButtonIcon.RightArrowSmall                          => new Vector4(84f,  28f,  28f, 28f),
                CircleButtonIcon.MusicNote                                => new Vector4(112f, 28f,  28f, 28f),
                CircleButtonIcon.Sprout                                   => new Vector4(140f, 28f,  28f, 28f),
                CircleButtonIcon.Dice                                     => new Vector4(168f, 28f,  28f, 28f),
                CircleButtonIcon.DownArrow                                => new Vector4(196f, 28f,  28f, 28f),
                CircleButtonIcon.Text                                     => new Vector4(224f, 28f,  28f, 28f),
                CircleButtonIcon.Eye                                      => new Vector4(0f,   56f,  28f, 28f),
                CircleButtonIcon.Envelope                                 => new Vector4(28f,  56f,  28f, 28f),
                CircleButtonIcon.Volume                                   => new Vector4(56f,  56f,  28f, 28f),
                CircleButtonIcon.Mute                                     => new Vector4(84f,  56f,  28f, 28f),
                CircleButtonIcon.Waveform                                 => new Vector4(112f, 56f,  28f, 28f),
                CircleButtonIcon.CheckedBox                               => new Vector4(140f, 56f,  28f, 28f),
                CircleButtonIcon.Cross                                    => new Vector4(168f, 56f,  28f, 28f),
                CircleButtonIcon.Medal                                    => new Vector4(196f, 56f,  28f, 28f),
                CircleButtonIcon.GoldGear                                 => new Vector4(0f,   84f,  28f, 28f),
                CircleButtonIcon.GoldFilter                               => new Vector4(28f,  84f,  28f, 28f),
                CircleButtonIcon.Reset                                    => new Vector4(56f,  84f,  28f, 28f),
                CircleButtonIcon.GoldRing                                 => new Vector4(84f,  84f,  28f, 28f),
                CircleButtonIcon.ExclamationMark                          => new Vector4(112f, 84f,  28f, 28f),
                CircleButtonIcon.GoldDocument                             => new Vector4(140f, 84f,  28f, 28f),
                CircleButtonIcon.ChatSettings                             => new Vector4(168f, 84f,  28f, 28f),
                CircleButtonIcon.DeliveryCart                             => new Vector4(196f, 84f,  28f, 28f),
                CircleButtonIcon.MagnifyingGlass                          => new Vector4(0f,   112f, 24f, 24f),
                CircleButtonIcon.EditSmall                                => new Vector4(24f,  112f, 24f, 24f),
                CircleButtonIcon.Hat                                      => new Vector4(48f,  112f, 24f, 24f),
                CircleButtonIcon.Helmet                                   => new Vector4(72f,  112f, 24f, 24f),
                CircleButtonIcon.Sword                                    => new Vector4(96f,  112f, 24f, 24f),
                CircleButtonIcon.Face                                     => new Vector4(120f, 112f, 24f, 24f),
                CircleButtonIcon.PersonStanding                           => new Vector4(144f, 112f, 24f, 24f),
                CircleButtonIcon.PaintBucket                              => new Vector4(0f,   136f, 24f, 24f),
                CircleButtonIcon.EyeSmall                                 => new Vector4(24f,  136f, 24f, 24f),
                CircleButtonIcon.Undo                                     => new Vector4(48f,  136f, 24f, 24f),
                CircleButtonIcon.PinnedDocument                           => new Vector4(72f,  136f, 24f, 24f),
                CircleButtonIcon.CrossSmall                               => new Vector4(96f,  136f, 24f, 24f),
                _                                                         => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };

            field                        =  value;
            ImageNode.TextureCoordinates =  new Vector2(region.X, region.Y);
            ImageNode.TextureSize        =  new Vector2(region.Z, region.W);
            ImageNode.ImageNodeFlags     &= ~ImageNodeFlags.FlipH;

            if (value == CircleButtonIcon.RightArrow)
                ImageNode.ImageNodeFlags |= ImageNodeFlags.FlipH;
        }
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        ImageNode.Size = Size;
    }
}
