using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using WpfBrush = System.Windows.Media.Brush;
using WpfBrushes = System.Windows.Media.Brushes;
using WpfColor = System.Windows.Media.Color;

namespace CardEditorGui;

/// <summary>将 BBCode 源码解析为 WPF <see cref="FlowDocument"/>，供卡牌描述只读预览。</summary>
public static class BbCodeFlowDocument
{
    /// <param name="dynamicVarFinalByKind">动态变量 kind → 预览数值（通常为 FinalValue）；存在时 <c>{Kind:diff()}</c> / <c>{Kind:Diff()}</c> 会显示为数值。</param>
    public static FlowDocument Parse(string? source, IReadOnlyDictionary<string, decimal>? dynamicVarFinalByKind = null)
    {
        var doc = new FlowDocument
        {
            PagePadding = new Thickness(0),
            FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
            FontSize = 12
        };
        var para = new Paragraph { Margin = new Thickness(0) };
        doc.Blocks.Add(para);
        if (string.IsNullOrEmpty(source))
            return doc;
        var pos = 0;
        ParseContent(source, ref pos, para.Inlines, null, dynamicVarFinalByKind);
        return doc;
    }

    private static void ParseContent(string s, ref int pos, InlineCollection parent, string? untilClose, IReadOnlyDictionary<string, decimal>? vars)
    {
        var sb = new StringBuilder();
        while (pos < s.Length)
        {
            if (s[pos] == '[')
            {
                Flush(sb, parent, vars);
                if (!TryReadTag(s, ref pos, out var tag))
                {
                    sb.Append('[');
                    continue;
                }
                if (tag.IsClosing)
                {
                    if (untilClose != null && TagNamesMatch(tag.Name, untilClose))
                        return;
                    parent.Add(new Run(tag.Raw) { Foreground = WpfBrushes.Gray });
                    continue;
                }
                if (tag.Name.Equals("color", StringComparison.OrdinalIgnoreCase) && tag.Value != null)
                {
                    var span = new Span();
                    ApplyColor(span, tag.Value);
                    ParseContent(s, ref pos, span.Inlines, "color", vars);
                    parent.Add(span);
                    continue;
                }
                if (tag.Name.Equals("font", StringComparison.OrdinalIgnoreCase) && tag.Value != null)
                {
                    var span = new Span { FontFamily = new System.Windows.Media.FontFamily(tag.Value) };
                    ParseContent(s, ref pos, span.Inlines, "font", vars);
                    parent.Add(span);
                    continue;
                }
                if (tag.Name.Equals("size", StringComparison.OrdinalIgnoreCase) && tag.Value != null &&
                    double.TryParse(tag.Value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var sz))
                {
                    var span = new Span { FontSize = sz };
                    ParseContent(s, ref pos, span.Inlines, "size", vars);
                    parent.Add(span);
                    continue;
                }
                if (tag.Name.Equals("b", StringComparison.OrdinalIgnoreCase))
                {
                    var span = new Span { FontWeight = FontWeights.Bold };
                    ParseContent(s, ref pos, span.Inlines, "b", vars);
                    parent.Add(span);
                    continue;
                }
                if (tag.Name.Equals("i", StringComparison.OrdinalIgnoreCase))
                {
                    var span = new Span { FontStyle = FontStyles.Italic };
                    ParseContent(s, ref pos, span.Inlines, "i", vars);
                    parent.Add(span);
                    continue;
                }
                if (tag.Name.Equals("u", StringComparison.OrdinalIgnoreCase))
                {
                    var span = new Span { TextDecorations = TextDecorations.Underline };
                    ParseContent(s, ref pos, span.Inlines, "u", vars);
                    parent.Add(span);
                    continue;
                }
                if (DescriptionExclusiveBbCode.IsNamedColorTag(tag.Name))
                {
                    var span = new Span();
                    span.Foreground = DescriptionExclusiveBbCode.BrushForNamedColor(tag.Name);
                    ParseContent(s, ref pos, span.Inlines, tag.Name, vars);
                    parent.Add(span);
                    continue;
                }
                if (DescriptionExclusiveBbCode.IsAnimationTag(tag.Name))
                {
                    var span = new Span
                    {
                        FontStyle = FontStyles.Italic,
                        Foreground = new System.Windows.Media.SolidColorBrush(WpfColor.FromRgb(0x6A, 0x5A, 0x8C))
                    };
                    span.TextDecorations = TextDecorations.Underline;
                    ParseContent(s, ref pos, span.Inlines, tag.Name, vars);
                    parent.Add(span);
                    continue;
                }
                parent.Add(new Run(tag.Raw) { Foreground = WpfBrushes.DarkGray });
                continue;
            }
            sb.Append(s[pos++]);
        }
        Flush(sb, parent, vars);
    }

    /// <summary>匹配 <c>{Kind:diff()}</c> 或 <c>{Kind:Diff()}</c>（与编辑器模版示例一致）。</summary>
    private static readonly Regex DiffPlaceholderRegex = new(
        @"\{([^{}:]+):([dD]iff)\(\)\}",
        RegexOptions.Compiled);

    private static void Flush(StringBuilder sb, InlineCollection parent, IReadOnlyDictionary<string, decimal>? vars)
    {
        if (sb.Length == 0)
            return;
        var chunk = sb.ToString();
        sb.Clear();
        AppendPlainWithDynamicVarSubs(parent, chunk, vars);
    }

    private static void AppendPlainWithDynamicVarSubs(InlineCollection parent, string text, IReadOnlyDictionary<string, decimal>? vars)
    {
        if (vars == null || vars.Count == 0)
        {
            parent.Add(new Run(text));
            return;
        }
        var last = 0;
        foreach (Match m in DiffPlaceholderRegex.Matches(text))
        {
            if (m.Index > last)
                parent.Add(new Run(text.Substring(last, m.Index - last)));
            var kind = m.Groups[1].Value.Trim();
            if (kind.Length > 0 && vars.TryGetValue(kind, out var val))
            {
                parent.Add(new Run(FormatDecimalForPreview(val))
                {
                    Foreground = WpfBrushes.DodgerBlue,
                    ToolTip = m.Value
                });
            }
            else
                parent.Add(new Run(m.Value));
            last = m.Index + m.Length;
        }
        if (last < text.Length)
            parent.Add(new Run(text.Substring(last)));
    }

    private static string FormatDecimalForPreview(decimal d)
    {
        if (d == decimal.Truncate(d))
            return decimal.ToInt64(d).ToString(CultureInfo.InvariantCulture);
        return d.ToString(CultureInfo.InvariantCulture);
    }

    private static bool TagNamesMatch(string a, string b) =>
        string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    private static void ApplyColor(Span span, string value)
    {
        var v = value.Trim();
        try
        {
            var conv = new System.Windows.Media.BrushConverter();
            if (conv.ConvertFromString(v) is WpfBrush b)
            {
                span.Foreground = b;
                return;
            }
        }
        catch
        {
            /* fall through */
        }
        span.Foreground = WpfBrushes.Black;
    }

    private static bool TryReadTag(string s, ref int pos, out TagParse tag)
    {
        var start = pos;
        if (pos >= s.Length || s[pos] != '[')
        {
            tag = default;
            return false;
        }
        var end = s.IndexOf(']', pos + 1);
        if (end < 0)
        {
            tag = default;
            return false;
        }
        var inner = s.Substring(pos + 1, end - pos - 1);
        pos = end + 1;
        var raw = s.Substring(start, end - start + 1);
        if (inner.StartsWith("/", StringComparison.Ordinal))
        {
            tag = new TagParse(raw, true, inner.Substring(1).Trim(), null);
            return true;
        }
        var eq = inner.IndexOf('=');
        if (eq >= 0)
            tag = new TagParse(raw, false, inner[..eq].Trim(), inner[(eq + 1)..].Trim());
        else
            tag = new TagParse(raw, false, inner.Trim(), null);
        return true;
    }

    private readonly struct TagParse(string raw, bool isClosing, string name, string? value)
    {
        public string Raw { get; } = raw;
        public bool IsClosing { get; } = isClosing;
        public string Name { get; } = name;
        public string? Value { get; } = value;
    }
}
