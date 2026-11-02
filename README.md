# OmenTools.KamiToolKit

OmenTools.KamiToolKit 是依赖于 [OmenTools](https://github.com/AtmoOmen/OmenTools) 为 [KamiToolKit](https://github.com/Dalamud-DailyRoutines/KamiToolKit) 提供功能扩展的库。

与 OmenTools 不同，本项目必须与 OmenTools 与 KamiToolKit 包含在同一解决方案下才能正常编译。

> [!WARNING]
> 由于二次开发带来的差异，本项目不支持原版 KamiToolKit 与其提供的 NuGet 包。

---


## 界面风格

`OmenTools.KamiToolKit.Styles` 提供整套界面风格的复用入口。风格负责窗口外框、内容边距、文字样式、按钮与分隔线，业务模块负责内容和列表布局。

当前提供 `OmenTools.KamiToolKit.Styles.Island.IslandUIStyle`，参考工房生产计划、背包及猫耳小员请求等原生界面，包含木纹背景、绳结边饰、椰树水印、棕色按钮及装饰关闭按钮。正文使用固定深棕色，按钮使用固定白色，避免游戏界面主题改变文字与底板的对比度。

### 独立窗口

在 `NativeAddon` 初始化时通过现有 `CreateWindowNode` 接入，标题、关闭、拖动及焦点处理沿用原生窗口组件：

```csharp
var style = new IslandUIStyle();
var addon = new NativeAddon
{
    InternalName = "ExampleIslandWindow",
    Title = title,
    Size = new(480, 384),
    ContentPadding = Vector2.Zero,
    CreateWindowNode = () => style.CreateWindow()
};
```

`ContentStartPosition` 和 `ContentSize` 已扣除风格边框与标题栏。`ContentPadding` 是额外的业务留白；设为零表示只保留风格自身的内边距。

### 附加面板

`AttachedAddon` 可直接设置 `Style`，自动使用无标题栏的风格窗口，保留原有跟随定位与开关行为：

```csharp
var panel = new ExampleAttachedAddon
{
    InternalName = "ExampleIslandPanel",
    Title = title,
    Size = new(480, 384),
    Style = style,
    ContentPadding = Vector2.Zero
};
```

`ExampleAttachedAddon` 表示业务模块自己的 `AttachedAddon` 派生类。指定 `Style` 后，由风格创建窗口，不再另行指定 `CreateWindowNode` 或覆盖 `HasWindowNode`。

业务内容通过 `ContentStartPosition` 定位，通过 `ContentSize` 获取可用尺寸。内容高度变化后可使用 `SetWindowSize(Size.X, contentHeight + Size.Y - ContentSize.Y)` 保留风格的上下边距；外框和装饰会随尺寸变化更新，无须逐帧处理。

### 内容控件

```csharp
var heading = style.CreateText(UITextRole.Heading);
heading.String = headingText;
heading.Size = new(contentWidth, 24);

var body = style.CreateText();
body.String = bodyText;
body.Size = new(contentWidth, 48);

var button = style.CreateButton();
button.String = buttonText;
button.Size = new(contentWidth, 32);
button.OnClick = onClick;

var separator = style.CreateSeparator();
separator.Width = contentWidth;
```

文字用途包含 `Title`、`Subtitle`、`Heading`、`Body` 和 `Button`。`ApplyTextStyle` 可为已有文字节点应用同样的样式，不会改动其内容、位置、尺寸与事件。正文默认支持换行；标题和副标题默认使用省略号，防止长文字覆盖其他标题栏元素。

仅需要外框时可使用 `style.CreateFrame()`，按其 `ContentStartPosition` 和 `ContentSize` 布置内容。无人岛外框另提供 `ShowWatermark` 与 `ShowDecorations`，可分别控制水印和绳结边饰。

风格对象只保存定义，可以供多个窗口复用；创建出的节点属于各自宿主，不共享节点实例。节点在游戏界面线程创建，挂接后由宿主节点树统一释放。关闭后重新打开窗口时重新创建节点即可。

### 添加其他风格

继承 `UIStyle`，定义内容内边距、推荐最小尺寸、标题栏度量及各类控件的创建方法，并提供对应的 `WindowFrameNode` 实现。`StyledWindowNode` 负责公共窗口行为，各风格只处理自身外观与布局。纹理和配色集中在具体风格中，业务模块无需填写资源坐标或颜色行号。

风格在创建窗口时确定。新增风格不需要修改现有风格的枚举分支，也不需要修改业务模块中的窗口交互逻辑。

---

## 许可证

MIT
