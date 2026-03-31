using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

public sealed class BuffOptionsFile
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    [JsonPropertyName("options")]
    public List<BuffOptionEntry> Options { get; set; } = [];
}

/// <summary>读写 exe 同目录下的 <c>BuffOptions.json</c>。</summary>
public static class BuffOptionsJson
{
    public const string FileName = "BuffOptions.json";

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

    public static string Serialize(BuffOptionsFile file) =>
        JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(JsonSerializer.Serialize(file, Options));

    public static BuffOptionsFile Deserialize(string json) =>
        JsonSerializer.Deserialize<BuffOptionsFile>(json, Options) ?? new BuffOptionsFile();

    internal static BuffOptionsFile DeserializeFile(string json) => Deserialize(json);

    public static List<BuffOptionEntry> LoadFromFile(string path)
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

    public static void SaveToFile(List<BuffOptionEntry> options, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, Serialize(new BuffOptionsFile { Options = options }));
    }

    public static List<BuffOptionEntry> LoadOrCreateDefault() =>
        ExeBundledSettingsJson.LoadOrCreateDefault().BuffOptions;

    public static void SaveDefault(List<BuffOptionEntry> options)
    {
        var root = ExeBundledSettingsJson.LoadOrCreateDefault();
        root.BuffOptions = options;
        ExeBundledSettingsJson.SaveDefault(root);
    }
}

