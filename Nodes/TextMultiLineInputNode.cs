using System;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyRoutines.Common.KamiToolKit.Nodes;

public unsafe class TextMultiLineInputNode : TextInputNode
{
    public TextMultiLineInputNode()
    {
        TextLimitsNode.AlignmentType = AlignmentType.BottomRight;

        CurrentTextNode.TextFlags |= TextFlags.MultiLine;
        CurrentTextNode.LineSpacing = 14;

        Flags |= TextInputFlags.MultiLine;
        AllowEnterToComplete = false;

        CollisionNode.AddEvent(AtkEventType.InputReceived, InputComplete);

        Component->InputSanitizationFlags = AllowedEntities.UppercaseLetters | AllowedEntities.LowercaseLetters | AllowedEntities.Numbers |
                                            AllowedEntities.SpecialCharacters | AllowedEntities.CharacterList | AllowedEntities.OtherCharacters |
                                            AllowedEntities.Payloads | AllowedEntities.Unknown9;

        Component->ComponentTextData.MaxLine = byte.MaxValue;
        Component->ComponentTextData.MaxByte = ushort.MaxValue;
    }

    public uint MaxLines
    {
        get => Component->ComponentTextData.MaxLine;
        set => Component->ComponentTextData.MaxLine = value;
    }

    public uint MaxBytes
    {
        get => Component->ComponentTextData.MaxByte;
        set => Component->ComponentTextData.MaxByte = value;
    }

    public override ReadOnlySeString String
    {
        get => base.String;
        set
        {
            base.String = value;
            PlaceholderTextNode.IsVisible = PlaceholderString is not null && value.IsEmpty;
            UpdateHeightForContent();
        }
    }

    public override Action<ReadOnlySeString>? OnInputReceived
    {
        get => base.OnInputReceived;
        set
        {
            base.OnInputReceived = _ => UpdateHeightForContent();
            base.OnInputReceived += value;
        }
    }

    public bool AutoUpdateHeight { get; set; }

    public Action<float>? HeightChanged { get; set; }

    public void UpdateHeightForContent()
    {
        if (!AutoUpdateHeight) return;

        var text = String;
        var lineCount = Math.Max(1, text.ToString().Split('\r', '\n').Length);
        var lineHeight = CurrentTextNode.LineSpacing;
        var contentHeight = Math.Max(Height, lineCount * lineHeight + 20);

        var oldHeight = Height;
        Height = contentHeight;

        if (Math.Abs(contentHeight - oldHeight) > 0.1f)
            HeightChanged?.Invoke(Height);
    }

    private void InputComplete()
    {
        if (UIInputData.Instance()->IsKeyPressed(SeVirtualKey.RETURN))
        {
            var textInputComponent = Node->GetAsAtkComponentTextInput();
            var cursorPos = textInputComponent->CursorPos;

            using (var utf8String = new Utf8String())
            {
                utf8String.SetString("\r");
                textInputComponent->WriteString(&utf8String);
            }

            textInputComponent->CursorPos = cursorPos + 1;
            textInputComponent->SelectionStart = cursorPos + 1;
            textInputComponent->SelectionEnd = cursorPos + 1;
        }

        OnInputComplete?.Invoke(Component->EvaluatedString.AsSpan());
    }
}
