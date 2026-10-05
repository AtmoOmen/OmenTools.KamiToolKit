using System.Numerics;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace OmenTools.KamiToolKit.Nodes.Background;

/// <summary>
///     Specialization of <see cref="NineGridNode" /> drawing one part of <c>ui/uld/BgParts.tex</c>.
/// </summary>
/// <remarks>
///     Every part of the texture is loaded up front, switching <see cref="Type" /> only changes the drawn part Id.
/// </remarks>
public sealed unsafe class BackgroundNode : NineGridNode
{
    /// <summary>
    ///     Constructs a new <see cref="BackgroundNode" />.
    /// </summary>
    public BackgroundNode()
    {
        PartsList.Add
        (
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(1f, 1f),
                Size               = new(32f, 32f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(33f, 1f),
                Size               = new(36f, 36f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(69f, 1f),
                Size               = new(36f, 36f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(105f, 1f),
                Size               = new(36f, 36f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(141f, 1f),
                Size               = new(36f, 36f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(1f, 33f),
                Size               = new(32f, 32f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(33f, 37f),
                Size               = new(28f, 28f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(61f, 37f),
                Size               = new(16f, 16f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(77f, 37f),
                Size               = new(64f, 12f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(141f, 37f),
                Size               = new(16f, 16f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(85f, 49f),
                Size               = new(32f, 32f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(117f, 49f),
                Size               = new(24f, 24f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(1f, 65f),
                Size               = new(32f, 32f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(33f, 65f),
                Size               = new(32f, 32f)
            },
            new Part
            {
                TexturePath        = TEXTURE_PATH,
                TextureCoordinates = new(65f, 65f),
                Size               = new(20f, 32f)
            }
        );

        ApplyType(BackgroundType.GoldFrame);
    }

    /// <summary>
    ///     Gets or sets which background part of the texture is drawn.
    /// </summary>
    public BackgroundType Type
    {
        get => (BackgroundType)PartId;
        set
        {
            if (PartId == (uint)value) return;

            ApplyType(value);
        }
    }

    /// <summary>
    ///     Applies the given background part and matches the nine grid offsets to its size.
    /// </summary>
    private void ApplyType
    (
        BackgroundType type
    )
    {
        PartId = (uint)type;

        var part = PartsList[(int)PartId];
        Offsets = new Vector4(MathF.Min(part->Width, part->Height) / 4.0f);
    }

    #region 常量

    private const string TEXTURE_PATH = "ui/uld/BgParts.tex";

    #endregion
}
