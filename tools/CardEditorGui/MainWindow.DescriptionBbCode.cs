using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Button = System.Windows.Controls.Button;

namespace CardEditorGui;

public partial class MainWindow
{
    private int? _descriptionSavedSelStart;
    private int? _descriptionSavedSelLen;

    private void InitializeDescriptionBbCode()
    {
        RefreshDescriptionPreview();
        TxtDescription.TextChanged += (_, _) =>
        {
            RefreshDescriptionPreview();
        };
        TxtDescription.PreviewKeyDown += TxtDescription_PreviewKeyDown;
    }

    private void RefreshDescriptionPreview()
    {
        RtbDescriptionPreview.Document = BbCodeFlowDocument.Parse(TxtDescription.Text);
    }

    private void SaveDescriptionSelectionSnapshot()
    {
        _descriptionSavedSelStart = TxtDescription.SelectionStart;
        _descriptionSavedSelLen = TxtDescription.SelectionLength;
    }

    private void ClearDescriptionSelectionSnapshot()
    {
        _descriptionSavedSelStart = null;
        _descriptionSavedSelLen = null;
    }

    private void GetDescriptionSelection(out int start, out int len)
    {
        if (_descriptionSavedSelStart.HasValue && _descriptionSavedSelLen.HasValue)
        {
            start = _descriptionSavedSelStart.Value;
            len = _descriptionSavedSelLen.Value;
            ClearDescriptionSelectionSnapshot();
            return;
        }
        start = TxtDescription.SelectionStart;
        len = TxtDescription.SelectionLength;
    }

    private void DescriptionToolbar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            SaveDescriptionSelectionSnapshot();
    }

    private void BtnDescriptionColorMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
            return;
        SaveDescriptionSelectionSnapshot();
        if (sender is not Button btn || btn.ContextMenu == null)
            return;
        btn.ContextMenu.PlacementTarget = btn;
        btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
        btn.ContextMenu.IsOpen = true;
        e.Handled = true;
    }

    private void BtnDescriptionAnimationMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
            return;
        SaveDescriptionSelectionSnapshot();
        if (sender is not Button btn || btn.ContextMenu == null)
            return;
        btn.ContextMenu.PlacementTarget = btn;
        btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
        btn.ContextMenu.IsOpen = true;
        e.Handled = true;
    }

    private void DescriptionColorMenu_Closed(object sender, RoutedEventArgs e) =>
        ClearDescriptionSelectionSnapshot();

    private void DescriptionAnimationMenu_Closed(object sender, RoutedEventArgs e) =>
        ClearDescriptionSelectionSnapshot();

    private void DescriptionWrapTag_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement el || el.Tag is not string tag)
            return;
        var parts = tag.Split('|');
        if (parts.Length != 4)
            return;
        WrapDescriptionPair(parts[0], parts[1], parts[2], parts[3]);
    }

    /// <param name="tagNameForExpand">用于选区扩展，如 b、font、size。</param>
    /// <param name="openLiteral">完整左标签，如 [b] 或 [font=Arial]。</param>
    /// <param name="closeLiteral">右标签，如 [/b]。</param>
    private void WrapDescriptionPair(string tagNameForExpand, string tagNameForToggle, string openLiteral, string closeLiteral)
    {
        var tb = TxtDescription;
        var text = tb.Text ?? "";
        GetDescriptionSelection(out var start, out var len);
        if (start > text.Length)
            start = text.Length;

        var tExpand = tagNameForExpand;
        DescriptionTagPairUtilities.TryExpandSelectionInsideTagPair(text, ref start, ref len, tExpand);
        if (start < 0 || len < 0 || start + len > text.Length)
            return;

        var segment = text.Substring(start, len);
        if (DescriptionTagPairUtilities.TryParseExactSingleWrapper(segment, tagNameForToggle, out var inner))
        {
            var newText = string.Concat(text.AsSpan(0, start), inner, text.AsSpan(start + len));
            tb.Text = newText;
            tb.Focus();
            tb.Select(start, inner.Length);
        }
        else
        {
            var core = segment;
            var wrapped = openLiteral + core + closeLiteral;
            var newText = string.Concat(text.AsSpan(0, start), wrapped, text.AsSpan(start + len));
            tb.Text = newText;
            tb.Focus();
            tb.Select(start + openLiteral.Length, core.Length);
        }
        MarkDirty();
        RefreshDescriptionPreview();
    }

    private void DescriptionInsertColorRed_Click(object sender, RoutedEventArgs e) =>
        WrapDescriptionExclusive("[color=red]", "[/color]", "color");

    private void DescriptionExclusiveWrapTag_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement el || el.Tag is not string tag)
            return;
        var parts = tag.Split('|', 3);
        if (parts.Length != 3)
            return;
        WrapDescriptionExclusive(parts[0], parts[1], parts[2]);
    }

    /// <param name="logicalName">用于判断是否「同一层」以执行取消，如 gold、color、aqua、ancient_banner。</param>
    private void WrapDescriptionExclusive(string openTag, string closeTag, string logicalName)
    {
        var tb = TxtDescription;
        var text = tb.Text ?? "";
        GetDescriptionSelection(out var start, out var len);
        if (start > text.Length)
            start = text.Length;

        DescriptionExclusiveBbCode.TryExpandSelectionToInnermostExclusiveWrapper(text, ref start, ref len);
        if (start < 0 || len < 0 || start + len > text.Length)
            return;

        var segment = text.Substring(start, len);
        if (IsSameExclusiveLayer(segment, logicalName, openTag, out var innerCore))
        {
            var newText = string.Concat(text.AsSpan(0, start), innerCore, text.AsSpan(start + len));
            tb.Text = newText;
            tb.Focus();
            tb.Select(start, innerCore.Length);
            MarkDirty();
            RefreshDescriptionPreview();
            return;
        }

        var core = DescriptionExclusiveBbCode.StripExclusiveWrappers(segment);
        var wrapped = openTag + core + closeTag;
        var newText2 = string.Concat(text.AsSpan(0, start), wrapped, text.AsSpan(start + len));
        tb.Text = newText2;
        tb.Focus();
        var innerLen = core.Length;
        tb.Select(start + openTag.Length, innerLen);
        MarkDirty();
        RefreshDescriptionPreview();
    }

    private static bool IsSameExclusiveLayer(string segment, string logicalName, string openTag, out string innerCore)
    {
        innerCore = "";
        if (logicalName.Equals("color", StringComparison.OrdinalIgnoreCase) &&
            segment.StartsWith("[color=", StringComparison.OrdinalIgnoreCase) &&
            openTag.StartsWith("[color=", StringComparison.OrdinalIgnoreCase))
        {
            var valOpen = ExtractColorValue(openTag);
            var valSeg = ExtractColorValueFromSegment(segment);
            if (valOpen.Equals(valSeg, StringComparison.OrdinalIgnoreCase) &&
                TryParseColorBody(segment, out innerCore))
                return true;
            return false;
        }
        var simpleOpen = $"[{logicalName}]";
        var simpleClose = $"[/{logicalName}]";
        if (segment.StartsWith(simpleOpen, StringComparison.OrdinalIgnoreCase) &&
            segment.EndsWith(simpleClose, StringComparison.OrdinalIgnoreCase) &&
            openTag.Equals(simpleOpen, StringComparison.OrdinalIgnoreCase))
        {
            innerCore = segment.Substring(simpleOpen.Length, segment.Length - simpleOpen.Length - simpleClose.Length);
            return true;
        }
        return false;
    }

    private static string ExtractColorValue(string openTag)
    {
        var eq = openTag.IndexOf('=');
        var end = openTag.IndexOf(']');
        if (eq < 0 || end < 0)
            return "";
        return openTag.Substring(eq + 1, end - eq - 1).Trim();
    }

    private static string ExtractColorValueFromSegment(string segment)
    {
        var eq = segment.IndexOf('=');
        var rb = segment.IndexOf(']');
        if (eq < 0 || rb < 0)
            return "";
        return segment.Substring(eq + 1, rb - eq - 1).Trim();
    }

    private static bool TryParseColorBody(string segment, out string inner)
    {
        inner = "";
        var close = "[/color]";
        var last = segment.LastIndexOf(close, StringComparison.OrdinalIgnoreCase);
        if (last < 0)
            return false;
        var first = segment.IndexOf(']');
        if (first < 0 || last <= first)
            return false;
        inner = segment.Substring(first + 1, last - first - 1);
        return true;
    }

    private void TxtDescription_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.B)
        {
            WrapDescriptionPair("b", "b", "[b]", "[/b]");
            e.Handled = true;
            return;
        }
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.I)
        {
            WrapDescriptionPair("i", "i", "[i]", "[/i]");
            e.Handled = true;
            return;
        }
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.U)
        {
            WrapDescriptionPair("u", "u", "[u]", "[/u]");
            e.Handled = true;
            return;
        }
        if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.C)
        {
            WrapDescriptionExclusive("[color=red]", "[/color]", "color");
            e.Handled = true;
            return;
        }
        if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.F)
        {
            WrapDescriptionPair("font", "font", "[font=Arial]", "[/font]");
            e.Handled = true;
            return;
        }
        if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.Z)
        {
            WrapDescriptionPair("size", "size", "[size=24]", "[/size]");
            e.Handled = true;
            return;
        }
    }
}
