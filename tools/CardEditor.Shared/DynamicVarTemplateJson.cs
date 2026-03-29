using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

public sealed class DynamicVarTemplateFile
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    [JsonPropertyName("templates")]
    public List<DynamicVarTemplate> Templates { get; set; } = [];
}

/// <summary>读写 exe 同目录下的 <c>DynamicVarTemplate.json</c>（与 AppData 中 settings.json 分离）。</summary>
public static class DynamicVarTemplateJson
{
    public const string FileName = "DynamicVarTemplate.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>与 CardEditorGui.exe 同目录。</summary>
    public static string GetDefaultFilePath() =>
        Path.Combine(AppContext.BaseDirectory, FileName);

    public static string Serialize(DynamicVarTemplateFile file) =>
        JsonSerializer.Serialize(file, Options);

    public static DynamicVarTemplateFile Deserialize(string json) =>
        JsonSerializer.Deserialize<DynamicVarTemplateFile>(json, Options)
        ?? new DynamicVarTemplateFile();

    public static void SaveToFile(List<DynamicVarTemplate> templates, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        var file = new DynamicVarTemplateFile { Templates = templates };
        File.WriteAllText(path, Serialize(file));
    }

    public static List<DynamicVarTemplate> LoadFromFile(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            return DynamicVarTemplateCatalog.CloneDefaultTemplates();
        try
        {
            var file = Deserialize(File.ReadAllText(full));
            if (file.Templates == null || file.Templates.Count == 0)
                return DynamicVarTemplateCatalog.CloneDefaultTemplates();
            return file.Templates;
        }
        catch
        {
            return DynamicVarTemplateCatalog.CloneDefaultTemplates();
        }
    }

    public static List<DynamicVarTemplate> LoadOrCreateDefault() =>
        LoadFromFile(GetDefaultFilePath());

    public static void SaveDefault(List<DynamicVarTemplate> templates) =>
        SaveToFile(templates, GetDefaultFilePath());
}
