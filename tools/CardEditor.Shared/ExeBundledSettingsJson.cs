using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

/// <summary>
/// 读写 exe 同目录下的 <c>settings.json</c>，合并原 CardPool.json、BuffOptions.json 等分散文件。
/// </summary>
public static class ExeBundledSettingsJson
{
    public const string FileName = "settings.json";

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
        Path.Combine(AppContext.BaseDirectory, FileName);

    public static string Serialize(ExeBundledSettings settings) =>
        JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(JsonSerializer.Serialize(settings, Options));

    public static ExeBundledSettings Deserialize(string json) =>
        JsonSerializer.Deserialize<ExeBundledSettings>(json, Options) ?? new ExeBundledSettings();

    public static void SaveToFile(ExeBundledSettings settings, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        settings.SchemaVersion = 2;
        File.WriteAllText(path, Serialize(CoalesceNulls(settings)));
    }

    public static ExeBundledSettings LoadOrCreateDefault()
    {
        var path = GetDefaultFilePath();
        if (File.Exists(path))
        {
            try
            {
                var root = Deserialize(File.ReadAllText(path));
                if (root.SchemaVersion >= 2)
                    return CoalesceNulls(root);
            }
            catch
            {
                // 损坏时尝试从旧文件恢复
            }
        }

        var merged = MigrateFromLegacySeparateFiles();
        merged.SchemaVersion = 2;
        merged = CoalesceNulls(merged);
        SaveToFile(merged, path);
        return merged;
    }

    public static void SaveDefault(ExeBundledSettings settings) =>
        SaveToFile(settings, GetDefaultFilePath());

    private static ExeBundledSettings CoalesceNulls(ExeBundledSettings s)
    {
        s.CardPools ??= [];
        s.BuffOptions ??= [];
        s.CardKeywordOptions ??= [];
        s.CardPlayActionTemplates ??= [];
        s.DynamicVarTemplates ??= [];
        s.PowerEditorSettings ??= new PowerEditorSettings();
        s.KeywordEditorSettings ??= new KeywordEditorSettings();
        return s;
    }

    private static ExeBundledSettings MigrateFromLegacySeparateFiles()
    {
        var root = new ExeBundledSettings();
        var dir = AppContext.BaseDirectory;

        root.CardPools = ReadLegacyCardPools(dir);
        root.BuffOptions = ReadLegacyBuffOptions(dir);
        root.CardKeywordOptions = ReadLegacyCardKeywordOptions(dir);
        root.CardPlayActionTemplates = ReadLegacyCardPlayActionTemplates(dir);
        root.DynamicVarTemplates = ReadLegacyDynamicVarTemplates(dir);
        root.PowerEditorSettings = ReadLegacyPowerEditorSettings(dir);
        root.KeywordEditorSettings = ReadLegacyKeywordEditorSettings(dir);
        return root;
    }

    private static List<CardPoolEntry> ReadLegacyCardPools(string dir)
    {
        var p = Path.Combine(dir, CardPoolJson.FileName);
        if (!File.Exists(p))
            return CardPoolJson.LoadPoolsWithFallback();
        try
        {
            var f = CardPoolJson.Deserialize(File.ReadAllText(p));
            if (f.Pools is { Count: > 0 })
                return f.Pools;
        }
        catch { }
        return CardPoolJson.LoadPoolsWithFallback();
    }

    private static List<BuffOptionEntry> ReadLegacyBuffOptions(string dir)
    {
        var p = Path.Combine(dir, BuffOptionsJson.FileName);
        if (!File.Exists(p))
            return [];
        try
        {
            var f = BuffOptionsJson.DeserializeFile(File.ReadAllText(p));
            return f.Options ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static List<KeywordOptionEntry> ReadLegacyCardKeywordOptions(string dir)
    {
        var p = Path.Combine(dir, CardKeywordOptionsJson.FileName);
        if (!File.Exists(p))
            return CardKeywordOptionsJson.CloneDefaultOptions();
        try
        {
            var f = CardKeywordOptionsJson.DeserializeFile(File.ReadAllText(p));
            var list = f.Options ?? [];
            return list.Count > 0 ? list : CardKeywordOptionsJson.CloneDefaultOptions();
        }
        catch
        {
            return CardKeywordOptionsJson.CloneDefaultOptions();
        }
    }

    private static List<CardPlayActionTemplate> ReadLegacyCardPlayActionTemplates(string dir)
    {
        var p = Path.Combine(dir, CardPlayActionTemplateJson.FileName);
        if (!File.Exists(p))
            return CardPlayActionTemplateCatalog.CloneDefaultTemplates();
        try
        {
            var f = CardPlayActionTemplateJson.DeserializeFile(File.ReadAllText(p));
            if (f.Templates is { Count: > 0 })
                return f.Templates;
        }
        catch { }
        return CardPlayActionTemplateCatalog.CloneDefaultTemplates();
    }

    private static List<DynamicVarTemplate> ReadLegacyDynamicVarTemplates(string dir)
    {
        var p = Path.Combine(dir, DynamicVarTemplateJson.FileName);
        if (!File.Exists(p))
            return DynamicVarTemplateCatalog.CloneDefaultTemplates();
        try
        {
            var f = DynamicVarTemplateJson.DeserializeFile(File.ReadAllText(p));
            if (f.Templates is { Count: > 0 })
                return f.Templates;
        }
        catch { }
        return DynamicVarTemplateCatalog.CloneDefaultTemplates();
    }

    private static PowerEditorSettings ReadLegacyPowerEditorSettings(string dir)
    {
        var p = Path.Combine(dir, PowerEditorSettingsJson.FileName);
        if (!File.Exists(p))
            return new PowerEditorSettings();
        try
        {
            return PowerEditorSettingsJson.DeserializeSettings(File.ReadAllText(p));
        }
        catch
        {
            return new PowerEditorSettings();
        }
    }

    private static KeywordEditorSettings ReadLegacyKeywordEditorSettings(string dir)
    {
        var p = Path.Combine(dir, KeywordEditorSettingsJson.FileName);
        if (!File.Exists(p))
            return new KeywordEditorSettings();
        try
        {
            return KeywordEditorSettingsJson.DeserializeSettings(File.ReadAllText(p));
        }
        catch
        {
            return new KeywordEditorSettings();
        }
    }
}
