using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>
/// 动态变量 kind 模版（独立 JSON 文件，与 settings.json 分离）。
/// </summary>
public sealed class DynamicVarTemplate
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("exampleText")]
    public string? ExampleText { get; set; }
}
