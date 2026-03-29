namespace CardEditorGui;

/// <summary>非互斥标签（粗体/斜体/字体/字号）的选区扩展与「再点一次去掉」判断。</summary>
public static class DescriptionTagPairUtilities
{
    /// <summary>若选区落在 <c>[tag]…[/tag]</c> 内，扩展为整段（含标签）；<paramref name="tagName"/> 为 <c>b</c>、<c>font</c>、<c>size</c> 等。</summary>
    public static bool TryExpandSelectionInsideTagPair(string text, ref int start, ref int len, string tagName)
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
            var name = ParseOpenName(openInner);
            if (!name.Equals(tagName, StringComparison.OrdinalIgnoreCase))
                continue;
            if (!DescriptionExclusiveBbCode.TryFindClosingTag(text.AsSpan(), rb + 1, tagName, out var innerStart, out var innerEnd))
                continue;
            var segStart = p;
            var closeBracket = text.IndexOf(']', innerEnd);
            if (closeBracket < 0)
                continue;
            var segEnd = closeBracket + 1;
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

    private static string ParseOpenName(string openInner)
    {
        var eq = openInner.IndexOf('=');
        return eq >= 0 ? openInner[..eq].Trim() : openInner.Trim();
    }

    /// <summary>整段是否恰好为一层 <c>[tag]inner[/tag]</c>（与 <paramref name="tagName"/> 一致，忽略大小写）。</summary>
    public static bool TryParseExactSingleWrapper(string segment, string tagName, out string inner)
    {
        inner = "";
        if (segment.Length < 5 || segment[0] != '[')
            return false;
        var rb = segment.IndexOf(']', 1);
        if (rb < 1)
            return false;
        var openInner = segment.Substring(1, rb - 1);
        if (openInner.StartsWith("/", StringComparison.Ordinal))
            return false;
        var name = ParseOpenName(openInner);
        if (!name.Equals(tagName, StringComparison.OrdinalIgnoreCase))
            return false;
        if (!DescriptionExclusiveBbCode.TryFindClosingTag(segment.AsSpan(), rb + 1, tagName, out var innerStart, out var innerEnd))
            return false;
        var closeBracket = segment.IndexOf(']', innerEnd);
        if (closeBracket < 0 || closeBracket + 1 != segment.Length)
            return false;
        inner = segment.Substring(innerStart, innerEnd - innerStart);
        return true;
    }
}
