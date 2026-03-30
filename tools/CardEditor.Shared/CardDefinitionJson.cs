using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

public static class CardDefinitionJson
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

    public static string Serialize(CardDefinition definition) =>
        JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(JsonSerializer.Serialize(definition, Options));

    public static CardDefinition Deserialize(string json) =>
        JsonSerializer.Deserialize<CardDefinition>(json, Options)
        ?? throw new InvalidOperationException("JSON 反序列化结果为 null。");

    public static void SaveToFile(CardDefinition definition, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, Serialize(definition));
    }

    public static CardDefinition LoadFromFile(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            throw new FileNotFoundException("找不到卡牌定义文件。", full);
        return Deserialize(File.ReadAllText(full));
    }
}
