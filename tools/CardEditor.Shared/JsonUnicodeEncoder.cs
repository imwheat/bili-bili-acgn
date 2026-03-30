using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace CardEditor.Shared;

/// <summary>
/// 写入 JSON 时尽量保留 Unicode 字面量；若底层仍输出 <c>\uXXXX</c>，由 <see cref="ExpandJsonUnicodeEscapes"/> 再展开为可读字符。
/// </summary>
public static partial class JsonUnicodeEncoder
{
    /// <summary>允许除代理项外的全部 Unicode 码位以字面形式写入 JSON 字符串。</summary>
    public static readonly JavaScriptEncoder ForWritableJson = JavaScriptEncoder.Create(UnicodeRanges.All);

    /// <summary>
    /// 将 JSON 文本中的 <c>\uXXXX</c> 与 UTF-16 代理对形式转为实际字符，便于在编辑器中阅读中文等。
    /// 不影响已以字面量形式存在的非 ASCII 字符。
    /// </summary>
    public static string ExpandJsonUnicodeEscapes(string json)
    {
        if (string.IsNullOrEmpty(json) || json.IndexOf("\\u", StringComparison.Ordinal) < 0)
            return json;

        // 先处理 UTF-16 代理对（如 emoji）
        var s = SurrogatePairEscapeRegex().Replace(json, m =>
        {
            var high = Convert.ToInt32(m.Groups[1].Value, 16);
            var low = Convert.ToInt32(m.Groups[2].Value, 16);
            if (high is >= 0xD800 and <= 0xDBFF && low is >= 0xDC00 and <= 0xDFFF)
                return char.ConvertFromUtf32(0x10000 + ((high - 0xD800) << 10) + (low - 0xDC00));
            return m.Value;
        });

        // 再处理 BMP 单码点（含中文）
        return BmpUnicodeEscapeRegex().Replace(s, m =>
        {
            var cp = Convert.ToInt32(m.Groups[1].Value, 16);
            if (cp is >= 0xD800 and <= 0xDFFF)
                return m.Value;
            return char.ConvertFromUtf32(cp);
        });
    }

    [GeneratedRegex(@"\\u(D[89ABab][0-9a-fA-F]{2})\\u(D[C-Fc-f][0-9a-fA-F]{2})", RegexOptions.CultureInvariant)]
    private static partial Regex SurrogatePairEscapeRegex();

    [GeneratedRegex(@"\\u([0-9a-fA-F]{4})", RegexOptions.CultureInvariant)]
    private static partial Regex BmpUnicodeEscapeRegex();
}
