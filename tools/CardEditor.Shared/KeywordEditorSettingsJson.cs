using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

/// <summary>读写 exe 同目录下的 <c>KeywordEditorSettings.json</c>。</summary>
public static class KeywordEditorSettingsJson
{
    public const string FileName = "KeywordEditorSettings.json";

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

    public static string Serialize(KeywordEditorSettings settings) =>
        JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(JsonSerializer.Serialize(settings, Options));

    public static KeywordEditorSettings Deserialize(string json) =>
        JsonSerializer.Deserialize<KeywordEditorSettings>(json, Options)
        ?? new KeywordEditorSettings();

    internal static KeywordEditorSettings DeserializeSettings(string json) => Deserialize(json);

    public static KeywordEditorSettings LoadFromFile(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            return new KeywordEditorSettings();
        try
        {
            return Deserialize(File.ReadAllText(full));
        }
        catch
        {
            return new KeywordEditorSettings();
        }
    }

    public static void SaveToFile(KeywordEditorSettings settings, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, Serialize(settings));
    }

    public static KeywordEditorSettings LoadOrCreateDefault() =>
        ExeBundledSettingsJson.LoadOrCreateDefault().KeywordEditorSettings;

    public static void SaveDefault(KeywordEditorSettings settings)
    {
        var root = ExeBundledSettingsJson.LoadOrCreateDefault();
        root.KeywordEditorSettings = settings;
        ExeBundledSettingsJson.SaveDefault(root);
    }
}

