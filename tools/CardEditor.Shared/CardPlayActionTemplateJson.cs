using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

public sealed class CardPlayActionTemplateFile
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    [JsonPropertyName("templates")]
    public List<CardPlayActionTemplate> Templates { get; set; } = [];
}

/// <summary>读写 exe 同目录下的 <c>CardPlayActionTemplate.json</c>（与 AppData 中 settings.json 分离）。</summary>
public static class CardPlayActionTemplateJson
{
    public const string FileName = "CardPlayActionTemplate.json";

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

    public static string Serialize(CardPlayActionTemplateFile file) =>
        JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(JsonSerializer.Serialize(file, Options));

    public static CardPlayActionTemplateFile Deserialize(string json) =>
        JsonSerializer.Deserialize<CardPlayActionTemplateFile>(json, Options)
        ?? new CardPlayActionTemplateFile();

    internal static CardPlayActionTemplateFile DeserializeFile(string json) => Deserialize(json);

    public static void SaveToFile(List<CardPlayActionTemplate> templates, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        var file = new CardPlayActionTemplateFile { Templates = templates };
        File.WriteAllText(path, Serialize(file));
    }

    public static List<CardPlayActionTemplate> LoadFromFile(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            return CardPlayActionTemplateCatalog.CloneDefaultTemplates();
        try
        {
            var f = Deserialize(File.ReadAllText(full));
            if (f.Templates == null || f.Templates.Count == 0)
                return CardPlayActionTemplateCatalog.CloneDefaultTemplates();
            return f.Templates;
        }
        catch
        {
            return CardPlayActionTemplateCatalog.CloneDefaultTemplates();
        }
    }

    public static List<CardPlayActionTemplate> LoadOrCreateDefault()
    {
        var root = ExeBundledSettingsJson.LoadOrCreateDefault();
        var list = root.CardPlayActionTemplates;
        if (list.Count > 0)
            return list;
        var d = CardPlayActionTemplateCatalog.CloneDefaultTemplates();
        root.CardPlayActionTemplates = d;
        ExeBundledSettingsJson.SaveDefault(root);
        return d;
    }

    public static void SaveDefault(List<CardPlayActionTemplate> templates)
    {
        var root = ExeBundledSettingsJson.LoadOrCreateDefault();
        root.CardPlayActionTemplates = templates;
        ExeBundledSettingsJson.SaveDefault(root);
    }
}
