using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CardEditor.Shared;

/// <summary>
/// 将卡牌 title/description 合并到游戏内 <c>cards.json</c>。
/// 键为 <c>{命名空间第一段大写}-{类名大写 snake_case}.title</c> / <c>.description</c>，例如 namespace <c>Test.Scripts</c>、className <c>TestCard</c> → <c>TEST-TEST_CARD.title</c>。
/// </summary>
public static class CardLocalizationJsonMerger
{
    private const string LegacyKeyPrefix = "BILIBILIACGN-";

    private static readonly JsonWriterOptions JsonWriteWriterOptions = new()
    {
        Indented = true,
        Encoder = JsonUnicodeEncoder.ForWritableJson
    };

    /// <summary>取命名空间第一段并转大写，如 <c>Test.Scripts</c> → <c>TEST</c>；空则 <c>CARD</c>。</summary>
    public static string NamespaceFirstSegmentUpper(string? namespaceDeclaration)
    {
        if (string.IsNullOrWhiteSpace(namespaceDeclaration))
            return "CARD";
        var t = namespaceDeclaration.Trim().TrimEnd(';').Trim();
        var dot = t.IndexOf('.');
        var first = dot < 0 ? t : t[..dot];
        if (string.IsNullOrEmpty(first))
            return "CARD";
        return first.ToUpperInvariant();
    }

    /// <summary>PascalCase 类名 → <c>TEST_CARD</c> 形式。</summary>
    public static string ClassNameToLocalizationSegment(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return "CARD";
        var s = className.Trim();
        var sb = new System.Text.StringBuilder(s.Length * 2);
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (char.IsUpper(c) && i > 0)
                sb.Append('_');
            sb.Append(char.ToUpperInvariant(c));
        }
        return sb.ToString();
    }

    /// <summary>生成 cards.json 中该卡牌条目的键前缀（不含 <c>.title</c>）。</summary>
    public static string BuildLocalizationKeyBase(string className, string? cardNamespace)
    {
        var ns = NamespaceFirstSegmentUpper(cardNamespace);
        var seg = ClassNameToLocalizationSegment(className);
        return $"{ns}-{seg}";
    }

    /// <summary>创建或更新对应键并写回文件。</summary>
    public static void MergeTitleAndDescription(string path, string className, string? cardNamespace, string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("路径不能为空。", nameof(path));

        var full = Path.GetFullPath(path);
        var dir = Path.GetDirectoryName(full);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        var classSeg = ClassNameToLocalizationSegment(className);
        var baseKey = BuildLocalizationKeyBase(className, cardNamespace);
        var titleKey = $"{baseKey}.title";
        var descKey = $"{baseKey}.description";

        var legacyBase = $"{LegacyKeyPrefix}{classSeg}";
        var legacyTitle = $"{legacyBase}.title";
        var legacyDesc = $"{legacyBase}.description";

        JsonObject root;
        if (File.Exists(full))
        {
            var text = File.ReadAllText(full);
            var parsed = JsonNode.Parse(text);
            root = parsed as JsonObject ?? new JsonObject();
        }
        else
            root = new JsonObject();

        if (!string.Equals(legacyTitle, titleKey, StringComparison.Ordinal))
        {
            root.Remove(legacyTitle);
            root.Remove(legacyDesc);
        }

        root[titleKey] = title ?? "";
        root[descKey] = description ?? "";

        File.WriteAllText(full, WriteJsonObjectIndented(root));
    }

    /// <summary><see cref="JsonNode.ToJsonString(JsonSerializerOptions)"/> 可能未把 <c>Encoder</c> 传到字符串写入路径，此处用 <see cref="Utf8JsonWriter"/> 显式指定。</summary>
    private static string WriteJsonObjectIndented(JsonObject root)
    {
        using var ms = new MemoryStream();
        using (var writer = new Utf8JsonWriter(ms, JsonWriteWriterOptions))
        {
            root.WriteTo(writer);
        }
        return JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(Encoding.UTF8.GetString(ms.ToArray()));
    }
}
