namespace OmenTools.KamiToolKit.Styles;

/// <summary>界面文字的语义用途，由界面风格决定字体、字号、行距和颜色。</summary>
public enum UITextRole
{
    /// <summary>独立窗口的主标题。</summary>
    Title,

    /// <summary>窗口副标题或紧凑说明。</summary>
    Subtitle,

    /// <summary>内容区域的分组标题。</summary>
    Heading,

    /// <summary>内容区域的正文和结果说明。</summary>
    Body,

    /// <summary>按钮上的操作文字。</summary>
    Button
}
