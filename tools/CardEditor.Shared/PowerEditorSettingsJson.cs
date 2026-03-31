using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

/// <summary>读写 exe 同目录下的 <c>PowerEditorSettings.json</c>。</summary>
public static class PowerEditorSettingsJson
{
    public const string FileName = "PowerEditorSettings.json";

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

    public static string Serialize(PowerEditorSettings settings) =>
        JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(JsonSerializer.Serialize(settings, Options));

    public static PowerEditorSettings Deserialize(string json) =>
        JsonSerializer.Deserialize<PowerEditorSettings>(json, Options)
        ?? new PowerEditorSettings();

    internal static PowerEditorSettings DeserializeSettings(string json) => Deserialize(json);

    public static PowerEditorSettings LoadFromFile(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            return new PowerEditorSettings();
        try
        {
            return Deserialize(File.ReadAllText(full));
        }
        catch
        {
            return new PowerEditorSettings();
        }
    }

    public static void SaveToFile(PowerEditorSettings settings, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, Serialize(settings));
    }

    public static PowerEditorSettings LoadOrCreateDefault() =>
        ExeBundledSettingsJson.LoadOrCreateDefault().PowerEditorSettings;

    public static void SaveDefault(PowerEditorSettings settings)
    {
        var root = ExeBundledSettingsJson.LoadOrCreateDefault();
        root.PowerEditorSettings = settings;
        ExeBundledSettingsJson.SaveDefault(root);
    }
}

