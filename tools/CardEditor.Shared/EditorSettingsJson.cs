using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

public static class EditorSettingsJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Encoder = JsonUnicodeEncoder.ForWritableJson
    };

    public static string GetDefaultSettingsPath()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BiliBiliACGN",
            "CardEditor");
        return Path.Combine(dir, "settings.json");
    }

    public static string Serialize(EditorSettings settings) =>
        JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(JsonSerializer.Serialize(settings, Options));

    public static EditorSettings Deserialize(string json) =>
        JsonSerializer.Deserialize<EditorSettings>(json, Options)
        ?? new EditorSettings();

    public static void SaveToFile(EditorSettings settings, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, Serialize(settings));
    }

    public static EditorSettings LoadFromFile(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            return new EditorSettings();
        return Deserialize(File.ReadAllText(full));
    }

    public static EditorSettings LoadOrCreateDefault()
    {
        var path = GetDefaultSettingsPath();
        return File.Exists(path) ? LoadFromFile(path) : new EditorSettings();
    }

    public static void SaveDefault(EditorSettings settings) =>
        SaveToFile(settings, GetDefaultSettingsPath());
}
