using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>
/// 与 CardEditorGui.exe 同目录的 <c>settings.json</c> 根对象（合并原 CardPool、BUFF、关键字选项、模版、Power/Keyword 编辑器等配置）。
/// </summary>
public sealed class ExeBundledSettings
{
    /// <summary>2 = 合并后的单文件格式。</summary>
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 2;

    [JsonPropertyName("cardPools")]
    public List<CardPoolEntry> CardPools { get; set; } = [];

    [JsonPropertyName("buffOptions")]
    public List<BuffOptionEntry> BuffOptions { get; set; } = [];

    [JsonPropertyName("cardKeywordOptions")]
    public List<KeywordOptionEntry> CardKeywordOptions { get; set; } = [];

    [JsonPropertyName("cardPlayActionTemplates")]
    public List<CardPlayActionTemplate> CardPlayActionTemplates { get; set; } = [];

    [JsonPropertyName("dynamicVarTemplates")]
    public List<DynamicVarTemplate> DynamicVarTemplates { get; set; } = [];

    [JsonPropertyName("powerEditorSettings")]
    public PowerEditorSettings PowerEditorSettings { get; set; } = new();

    [JsonPropertyName("keywordEditorSettings")]
    public KeywordEditorSettings KeywordEditorSettings { get; set; } = new();
}
