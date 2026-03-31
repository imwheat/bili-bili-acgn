using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

public sealed class CardKeywordOptionsFile
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    [JsonPropertyName("options")]
    public List<KeywordOptionEntry> Options { get; set; } = [];
}

/// <summary>读写 exe 同目录下的 <c>CardKeywordOptions.json</c>（升级效果关键字下拉）。</summary>
public static class CardKeywordOptionsJson
{
    public const string FileName = "CardKeywordOptions.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Encoder = JsonUnicodeEncoder.ForWritableJson
    };

    public static string GetDefaultFilePath() =>
        ExeBundledSettingsJson.GetDefaultFilePath();

    public static string Serialize(CardKeywordOptionsFile file) =>
        JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(JsonSerializer.Serialize(file, Options));

    public static CardKeywordOptionsFile Deserialize(string json) =>
        JsonSerializer.Deserialize<CardKeywordOptionsFile>(json, Options) ?? new CardKeywordOptionsFile();

    internal static CardKeywordOptionsFile DeserializeFile(string json) => Deserialize(json);

    public static List<KeywordOptionEntry> LoadFromFile(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            return [];
        try
        {
            var f = Deserialize(File.ReadAllText(full));
            return f.Options ?? [];
        }
        catch
        {
            return [];
        }
    }

    public static void SaveToFile(List<KeywordOptionEntry> options, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, Serialize(new CardKeywordOptionsFile { Options = options }));
    }

    public static List<KeywordOptionEntry> LoadOrCreateDefault()
    {
        var root = ExeBundledSettingsJson.LoadOrCreateDefault();
        var list = root.CardKeywordOptions;
        if (list.Count > 0)
            return list;
        var d = CloneDefaultOptions();
        root.CardKeywordOptions = d;
        ExeBundledSettingsJson.SaveDefault(root);
        return d;
    }

    public static List<KeywordOptionEntry> CloneDefaultOptions() =>
        CardKeywordCatalog.CloneDefault().Select(k => new KeywordOptionEntry { Name = k.Name, Notes = k.Notes }).ToList();

    public static void SaveDefault(List<KeywordOptionEntry> options)
    {
        var root = ExeBundledSettingsJson.LoadOrCreateDefault();
        root.CardKeywordOptions = options;
        ExeBundledSettingsJson.SaveDefault(root);
    }
}
