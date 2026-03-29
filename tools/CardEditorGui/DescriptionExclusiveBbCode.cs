using WpfBrushes = System.Windows.Media.Brushes;
using WpfColor = System.Windows.Media.Color;

namespace CardEditorGui;

/// <summary>颜色/动画类 BBCode 互斥：同一选区只保留一层；与 <see cref="DescriptionTagPairUtilities"/> 配合扩展选区。</summary>
public static class DescriptionExclusiveBbCode
{
    public static readonly HashSet<string> ExclusiveColorTagNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "color", "aqua", "blue", "gold", "green", "orange", "pink", "purple", "red"
    };

    public static readonly HashSet<string> ExclusiveAnimationTagNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "ancient_banner", "fade_in", "fly_in", "jitter", "sine", "thinky_dots"
    };

    public static bool IsExclusiveTag(string name) =>
        ExclusiveColorTagNames.Contains(name) || ExclusiveAnimationTagNames.Contains(name);

    /// <summary><c>[color=…]</c> 走单独分支；此处仅指 <c>[gold]</c> 等颜色简写。</summary>
    public static bool IsNamedColorTag(string name) =>
        name.Length > 0 &&
        !name.Equals("color", StringComparison.OrdinalIgnoreCase) &&
        ExclusiveColorTagNames.Contains(name);

    public static bool IsAnimationTag(string name) => ExclusiveAnimationTagNames.Contains(name);

    public static System.Windows.Media.Brush BrushForNamedColor(string name)
    {
        return name.ToLowerInvariant() switch
        {
            "aqua" => new System.Windows.Media.SolidColorBrush(WpfColor.FromRgb(0x40, 0xE0, 0xD0)),
            "blue" => WpfBrushes.DodgerBlue,
            "gold" => new System.Windows.Media.SolidColorBrush(WpfColor.FromRgb(0xFF, 0xD7, 0x00)),
            "green" => WpfBrushes.LimeGreen,
            "orange" => new System.Windows.Media.SolidColorBrush(WpfColor.FromRgb(0xFF, 0x8C, 0x00)),
            "pink" => new System.Windows.Media.SolidColorBrush(WpfColor.FromRgb(0xFF, 0x69, 0xB4)),
            "purple" => new System.Windows.Media.SolidColorBrush(WpfColor.FromRgb(0x9B, 0x30, 0xFF)),
            "red" => WpfBrushes.OrangeRed,
            _ => WpfBrushes.Black
        };
    }

    /// <summary>剥掉选区最外层、属于互斥名单的 <c>[tag]…[/tag]</c>（含 <c>[color=…]</c>）。</summary>
    public static string StripExclusiveWrappers(string segment)
    {
        var s = segment;
        while (TryStripOneExclusiveOuter(ref s)) { }
        return s;
    }

    private static bool TryStripOneExclusive(ref ReadOnlySpan<char> span, out ReadOnlySpan<char> inner)
    {
        inner = default;
        if (span.Length < 3 || span[0] != '[')
            return false;
        var closeBracket = span.IndexOf(']');
        if (closeBracket <= 1)
            return false;
        var openInner = span[1..closeBracket];
        if (openInner.Length > 0 && openInner[0] == '/')
            return false;
        string tagName;
        string? colorVal = null;
        var eqIdx = openInner.IndexOf('=');
        if (eqIdx >= 0)
        {
            tagName = new string(openInner[..eqIdx]).Trim();
            colorVal = new string(openInner[(eqIdx + 1)..]).Trim();
        }
        else
            tagName = new string(openInner).Trim();
        if (!IsExclusiveTag(tagName))
            return false;
        if (tagName.Equals("color", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(colorVal))
            return false;
        var closeTag = tagName.Equals("color", StringComparison.OrdinalIgnoreCase) ? "color" : tagName;
        if (!TryFindClosingTag(span, closeBracket + 1, closeTag, out var innerStart, out var innerEnd))
            return false;
        inner = span.Slice(innerStart, innerEnd - innerStart);
        return true;
    }

    private static bool TryStripOneExclusiveOuter(ref string s)
    {
        var span = s.AsSpan();
        if (!TryStripOneExclusive(ref span, out var inner))
            return false;
        s = inner.ToString();
        return true;
    }

    /// <summary>从 <paramref name="contentStart"/> 起查找与 <paramref name="tagName"/> 匹配的 <c>[/tag]</c>（处理嵌套同名深度）。</summary>
    public static bool TryFindClosingTag(ReadOnlySpan<char> full, int contentStart, string tagName, out int innerStart, out int innerEnd)
    {
        innerStart = contentStart;
        innerEnd = contentStart;
        var depth = 1;
        var i = contentStart;
        var openPrefix = $"[{tagName}".AsSpan();
        var closeSeq = $"[/{tagName}]".AsSpan();
        while (i < full.Length)
        {
            if (full[i] != '[')
            {
                i++;
                continue;
            }
            var close = full[i..].IndexOf(']');
            if (close < 0)
                break;
            var tagSlice = full.Slice(i, close + 1);
            if (tagSlice.StartsWith("[/", StringComparison.OrdinalIgnoreCase))
            {
                if (tagSlice.Equals(closeSeq, StringComparison.OrdinalIgnoreCase))
                {
                    depth--;
                    if (depth == 0)
                    {
                        innerEnd = i;
                        return true;
                    }
                }
            }
            else if (tagName.Equals("color", StringComparison.OrdinalIgnoreCase))
            {
                if (tagSlice.StartsWith("[color=", StringComparison.OrdinalIgnoreCase))
                    depth++;
            }
            else if (tagName.Equals("font", StringComparison.OrdinalIgnoreCase))
            {
                if (tagSlice.StartsWith("[font=", StringComparison.OrdinalIgnoreCase))
                    depth++;
            }
            else if (tagName.Equals("size", StringComparison.OrdinalIgnoreCase))
            {
                if (tagSlice.StartsWith("[size=", StringComparison.OrdinalIgnoreCase))
                    depth++;
            }
            else if (tagSlice.Length > 1 && tagSlice[1] != '/')
            {
                var innerTag = tagSlice.Slice(1, tagSlice.Length - 2);
                var name = GetTagNameFromOpen(innerTag);
                if (name.Equals(tagName, StringComparison.OrdinalIgnoreCase))
                    depth++;
            }
            i += close + 1;
        }
        return false;
    }

    private static ReadOnlySpan<char> GetTagNameFromOpen(ReadOnlySpan<char> innerNoBrackets)
    {
        var eq = innerNoBrackets.IndexOf('=');
        return eq >= 0 ? innerNoBrackets[..eq].Trim() : innerNoBrackets.Trim();
    }

    /// <summary>若选区落在某互斥标签对内，扩展为整段 <c>[tag]…[/tag]</c>。</summary>
    public static bool TryExpandSelectionToInnermostExclusiveWrapper(string text, ref int start, ref int len)
    {
        if (len == 0 || string.IsNullOrEmpty(text))
            return false;
        var end = start + len;
        if (end > text.Length)
            return false;
        for (var p = 0; p < text.Length && p <= start; p++)
        {
            if (text[p] != '[')
                continue;
            var rb = text.IndexOf(']', p + 1);
            if (rb < 0)
                break;
            var openInner = text.Substring(p + 1, rb - p - 1);
            if (openInner.StartsWith("/", StringComparison.Ordinal))
                continue;
            var tagName = ParseOpenTagName(openInner, out var isColor);
            if (!IsExclusiveTag(tagName))
                continue;
            var closeName = isColor ? "color" : tagName;
            if (!TryFindClosingTag(text.AsSpan(), rb + 1, closeName, out var innerStart, out var innerEnd))
                continue;
            var segStart = p;
            var segEnd = text.IndexOf(']', innerEnd) + 1;
            if (segEnd <= segStart)
                continue;
            if (start >= innerStart && end <= innerEnd)
            {
                start = segStart;
                len = segEnd - segStart;
                return true;
            }
            if (start == segStart && end == segEnd)
            {
                start = segStart;
                len = segEnd - segStart;
                return true;
            }
        }
        return false;
    }

    private static string ParseOpenTagName(string openInner, out bool isColor)
    {
        isColor = false;
        var eq = openInner.IndexOf('=');
        if (eq >= 0)
        {
            var name = openInner[..eq].Trim();
            isColor = name.Equals("color", StringComparison.OrdinalIgnoreCase);
            return name;
        }
        return openInner.Trim();
    }
}
